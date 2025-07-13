using SkimSkript.Nodes;
using SkimSkript.Nodes.StatementNodes;
using SkimSkript.Tokens;
using System.ComponentModel.DataAnnotations;

namespace SkimSkript.MainComponents
{
    /// <summary> 
    ///Represents a parser for converting a list of tokens into an abstract syntax tree (AST).
    ///The root of the AST can be retrieved with the root being in the form of an <see cref="AbstractSyntaxTreeRoot"/>
    ///object.
    /// </summary>
    internal class Parser : MainComponent<TokenContainer, AbstractSyntaxTreeRoot>
    {
        #region Constants
        private const int TYPE_START_FUNC_PEEK_OFFSET = 3;
        #endregion

        #region Data Members
        private TokenContainer? _tokens;
        #endregion

        #region Properties
        private TokenContainer Tokens => _tokens!;

        public override MainComponentType ComponentType => MainComponentType.Parser;

        public Parser(IEnumerable<MainComponentType> debuggedTypes, IEnumerable<MainComponentType> verboseTypes) : base(debuggedTypes, verboseTypes) { }
        #endregion

        #region Entry Point
        /// <summary> 
        /// Returns AST root with top-level statements and functions as childrern. 
        /// </summary>
        protected override AbstractSyntaxTreeRoot OnExecute(TokenContainer tokens)
        {
            _tokens = tokens;

            List<Node> statements = new();
            List<Node> functions = new();

            

            while (Tokens.HasTokens)
            {
                _logger?.Verbose("Parsing tokens. {Count} tokens remaining", Tokens.Count);

                if (IsFunctionDefinitionNext())
                    functions.Add(GetFunctionNode()); // Function definitions and bodies    
                else 
                    statements.Add(GetStatement()); // Top-level statements                   
            }

            return new AbstractSyntaxTreeRoot(statements.ToArray(), functions.ToArray());
        }
        #endregion

        #region Function Definitions
        /// <summary>
        /// Parses a function definition and body (block) and returns its data in a node. 
        /// </summary>
        private Node GetFunctionNode()
        {
            Tokens.TryMatchAndRemove(TokenType.FunctionDefine);

            Type? returnType = null;

            if(IsDataTypeToken(Tokens.PeekType()))
                returnType = GetValueNodeType();

            // Only use try for void seeing that this has already been differentiated from a function call
            Tokens.TryMatchAndRemove(TokenType.FunctionVoid);
            Tokens.TryMatchAndRemove(TokenType.FunctionLabel);

            var identifier = GetIdentifier();

            // Opening parenthesis to enclose parameter declarations
            Tokens.MatchAndRemove(TokenType.ParenthesisOpen);
            List<Node>? parameters = null;

            // Keep reading parameters until the closing parenthesis
            while (!Tokens.TryMatchAndRemove(TokenType.ParenthesisClose))
            {
                parameters ??= new List<Node>();
                parameters.Add(GetParameter());
            }

            _logger?.Verbose("Creating function node. Parsing block");
            return new FunctionNode(identifier, returnType, parameters?.ToArray(), GetBlock());
        }

        private Node GetParameter()
        {
            // Optional reference keyword(s) -> data type -> parameter identifierNode
            var isRef = Tokens.TryMatchAndRemove(TokenType.PassByReference);
            var dataType = GetValueNodeType();

            return new ParameterNode(isRef, dataType, GetIdentifier());
        }
        #endregion

        #region Statements
        #region Gathering Statements
        private Node GetBlock()
        {
            _logger?.Verbose("Parsing block that starts with Token at index {Index}", Tokens.CurrentTokenIndex);

            var isImplicitBlock = !Tokens.TryMatchAndRemove(TokenType.BlockOpen);
            List<Node>? statements = null;

            if (isImplicitBlock)
            {
                _logger?.Verbose("No block open. Parsing implicit block");
                return new BlockNode([GetStatement()]);
            }
                

            while (!Tokens.TryMatchAndRemove(TokenType.BlockClose))
            {
                statements ??= new List<Node>();
                statements.Add(GetStatement());
            }

            return new BlockNode(statements?.ToArray());
        }

        /// <summary>Determines using the front-most token what type of newNode needs to be parsed
        /// next, and then parses that newNode.</summary>
        /// <exception cref="SyntaxError"></exception>
        private Node GetStatement()
        {
            Node newNode;
            int lexemeStartIndex = Tokens.GetLexemeStartIndex();
            var tokenType = Tokens.PeekType();

            _logger?.Verbose("Parsing statement that starts with Token {Type} at index {Index}", tokenType, Tokens.CurrentTokenIndex);

            switch (tokenType)
            {
                case TokenType.DeclarationStart:
                    newNode = GetVariableDeclaration(); 
                    break;
                case TokenType.FunctionCallStart:
                    newNode = GetFunctionCall(); 
                    break;
                case TokenType.Return:
                    newNode = GetReturnStatement(); 
                    break;
                case TokenType.AssignmentStart:
                    newNode = GetAssignment(); 
                    break;
                case TokenType.WhileLoop:
                    newNode = GetWhileLoop(); 
                    break;
                case TokenType.If:
                    newNode = GetIfStatement(); 
                    break;
                case TokenType.Identifier:
                    newNode = GetIdentifierStartStatement(); 
                    break;
                case TokenType.Assertion:
                    newNode = GetAssertionStatement(); 
                    break;
                case TokenType.RepeatLoop:
                    newNode = GetRepeatLoop(); 
                    break;
                case TokenType.Try:
                    newNode = GetTryStatement();
                    break;
                default:
                    if(IsDataTypeToken(tokenType))
                        newNode = GetVariableDeclaration();
                    else
                        throw Tokens.GetTokenExceptionError(TokenType.StatementStartToken);
                    break;
            }

            var statementNode = (StatementNode)newNode;
            statementNode.SetLexemeStartIndex(lexemeStartIndex);

            if (!statementNode.IsEndLexeme)
                statementNode.SetLexemeEndIndex(Tokens.GetLexemeEndIndex());

            return statementNode;
        }
        #endregion

        #region Function Statements
        /// <summary>Parses function call and any potential arguments sent.</summary>
        private Node GetFunctionCall()
        {
            Tokens.TryMatchAndRemove([TokenType.FunctionCallStart, TokenType.FunctionCallStartExpression]);

            var identifier = GetIdentifier();
            List<Node>? args = null;

            Tokens.MatchAndRemove(TokenType.ParenthesisOpen);

            while (!Tokens.TryMatchAndRemove(TokenType.ParenthesisClose))
            {
                // If a value argument then it's an condition, but if reference, it's an identifierNode
                var isRef = Tokens.TryMatchAndRemove(TokenType.PassByReference);
                var value = !isRef ? GetExpression() : GetIdentifier();
                args ??= new List<Node>();
                args.Add(new ArgumentNode(isRef, value));
            }

            _logger?.Verbose("Creating function call");
            return new FunctionCallNode(identifier, args?.ToArray());
        }

        /// <summary>Parses a return newNode with or without an conditionalExpression.</summary>
        private Node GetReturnStatement()
        {
            Tokens.MatchAndRemove(TokenType.Return);
            _logger?.Verbose("Creating return statement. Parsing return expression");
            return new ReturnNode(IsExpressionStartingToken() ? GetExpression() : null);
        }
        #endregion

        #region Variable Statements
        /// <summary>Parses a variable declaration and can potentially parse an initilization in the same newNode.</summary>
        private Node GetVariableDeclaration()
        {
            Tokens.TryMatchAndRemove(TokenType.DeclarationStart);

            if (IsCollectionDataType(Tokens.PeekType()))
                return GetCollectionDeclaration();

            return GetValueTypeVariableDeclaration();
        }

        private Node GetCollectionDeclaration()
        {
            throw new NotImplementedException("Collection declarations are not yet implemented.");
        }

        /// <summary> 
        /// Parses a variable or parameter declaration with a value data type. 
        /// </summary>
        private Node GetValueTypeVariableDeclaration()
        {
            Type dataType = GetValueNodeType();
            var identifierNode = GetIdentifier();

            Node? initValue;


            /* AssignmentOperator can be used for both initialization and assignment. However VariableInitialize
            * is only used for initialization which is why the tokens are seperate. */
            var isInit = Tokens.TryMatchAndRemove([TokenType.VariableInitialize, TokenType.AssignmentOperator]);

            /* If initialized get an condition (that will potetially be coerced runtime by the interpreter).
                * Otherwise get a default value node for the data type in the declaration. */
            initValue = isInit ? GetExpression() : GetValueNodeOfType(dataType);

            _logger?.Verbose("Creating value variable declaration. Is Intialized: {Value}  Data Type: {Value}", isInit, dataType.Name);
            return new VariableDeclarationNode(identifierNode, dataType, initValue);
        }

        /// <summary>Parses an assignment newNode where what is presumably a variable or parameter is assigned an condition.</summary>
        private Node GetAssignment()
        {
            Tokens.TryMatchAndRemove(TokenType.AssignmentStart);
            var identifierNode = GetIdentifier();
            Tokens.MatchAndRemove(TokenType.AssignmentOperator);

            _logger?.Verbose("Creating assignment and parsing assigned expression");
            return new AssignmentNode(identifierNode, GetExpression());
        }
        #endregion

        #region Control Structure Statements
        private Node GetIfStatement(bool isElseIf = false)
        {
            /* For this method to even be called the if token will be verified and that's why
             * it can be Try. It is Try because this method is reused for else-if statements.*/
            Tokens.TryMatchAndRemove(TokenType.If);

            var condition = GetExpression();

            Tokens.TryMatchAndRemove(TokenType.Then);
            var statementEndLexeme = Tokens.GetLexemeEndIndex();

            var block = GetBlock();
            Node? chainedStructure = null;

            if (Tokens.TryMatchAndRemove(TokenType.ElseIf))
                chainedStructure = GetIfStatement(true);
            else if (Tokens.TryMatchAndRemove(TokenType.Else))
                chainedStructure = new ElseNode(GetBlock(), statementEndLexeme);

            _logger?.Verbose("Creating if statement. Is Else-If: {Value}     Is Chained Structure: {Value}", isElseIf, chainedStructure != null);
            return !isElseIf ?
                new IfNode(condition, block, chainedStructure, statementEndLexeme) :
                new ElseIfNode(condition, block, chainedStructure, statementEndLexeme);
        }

        private Node GetWhileLoop()
        {
            Tokens.Remove(TokenType.WhileLoop);
            var condition = GetExpression();
            Tokens.TryMatchAndRemove(TokenType.Then);
            var statementEndLexeme = Tokens.GetLexemeEndIndex();

            _logger?.Verbose("Creating while loop. Parsing block");
            return new WhileNode(condition, GetBlock(), statementEndLexeme);
        }

        private Node GetRepeatLoop()
        {
            Tokens.Remove(TokenType.RepeatLoop);
            var condition = GetExpression();
            Tokens.TryMatchAndRemove(TokenType.RepeatLoopTrail);
            var statementEndLexeme = Tokens.GetLexemeEndIndex();

            _logger?.Verbose("Creating repeat loop. Parsing block");
            return new RepeatNode(condition, GetBlock(), statementEndLexeme);
        }
        #endregion

        #region Monitoring Statements
        private Node GetAssertionStatement()
        {
            Tokens.Remove(TokenType.Assertion);
            var conditionalExpression = GetExpression();
            _logger?.Verbose("Creating assertion statement");
            return new AssertionNode(conditionalExpression);
        }

        private Node GetTryStatement()
        {
            _logger?.Verbose("Creating try statement. Parsing try block");
            // "try" --> block --> "catch" --> optional "(" --> optional identifier --> optional ")" --> block
            Tokens.Remove(TokenType.Try);
            var tryBlock = GetBlock();

            Tokens.Remove(TokenType.Catch);

            // Optional variable declaration of message variable that will contain the exception message
            var messageNode = GetOptionalCatchMessage();
            var catchBlock = GetBlock();
                   
            return new TryCatchNode(tryBlock, catchBlock, messageNode);
        }
        #endregion
        #endregion

        #region Expressions
        #region Math, Logic, & Comparisons
        /// <summary>Parses a logical, comparison, and/or arithmetic expressions. Starting with
        /// logical expressions as they are the lowest precedence level.</summary>
        private Node GetExpression() => ParseLogicalExpression();

        private Node ParseLogicalExpression()
        {
            var leftTerm = ParseComparisonExpression();

            while (Tokens.TryPeek(out var tokenType) && tokenType >= TokenType.And && tokenType <= TokenType.Xor)
            {
                Tokens.Remove();
                var rightTerm = ParseComparisonExpression();
                leftTerm = new LogicExpressionNode((LogicOperator)tokenType, leftTerm, rightTerm);
            }

            return leftTerm;
        }

        private Node ParseComparisonExpression()
        {
            var leftNode = ParseArithmeticExpression();

            while (Tokens.TryPeek(out var tokenType) && tokenType >= TokenType.Equals && tokenType <= TokenType.LessThanOrEqual)
            {
                Tokens.Remove();
                Node rightNode = ParseArithmeticExpression(); // Comparison operators act on arithmetic expressions
                leftNode = new ComparisonExpressionNode((ComparisonOperator)tokenType, leftNode, rightNode);
            }

            return leftNode;
        }

        private Node ParseArithmeticExpression()
        {
            Node leftNode = ParseTerm();

            while (Tokens.TryPeek(out var tokenType) && tokenType == TokenType.Add || tokenType == TokenType.SubtractUnary)
            {
                Tokens.Remove();
                Node rightNode = ParseTerm(); // Low-precedence arithmetic (add/subtract) acts on terms
                leftNode = new MathExpressionNode((MathOperator)tokenType, leftNode, rightNode);
            }

            return leftNode;
        }

        private Node ParseInnerExpression()
        {
            var enclosedExpression = GetExpression();
            Tokens.MatchAndRemove(TokenType.ParenthesisClose);
            return enclosedExpression;
        }
        #endregion

        #region Terms
        private Node ParseTerm()
        {
            var leftFactor = ParseExponentTerm();

            while (Tokens.TryPeek(out var tokenType) && tokenType >= TokenType.Multiply && tokenType <= TokenType.Exponent)
            {
                Tokens.Remove();
                var rightFactor = ParseExponentTerm();
                leftFactor = new MathExpressionNode((MathOperator)tokenType, leftFactor, rightFactor);
            }

            return leftFactor;
        }

        private Node ParseExponentTerm()
        {
            var baseFactor = ParseFactor();

            while (Tokens.TryPeek(out var tokenType) && tokenType == TokenType.Exponent)
            {
                Tokens.Remove();
                Node exponent = ParseExponentTerm(); // Recursive call for right associativity
                baseFactor = new MathExpressionNode(MathOperator.Exponent, baseFactor, exponent);
            }

            return baseFactor;
        }
        #endregion

        #region Factors
        private Node ParseFactor()
        {
            var tokenType = Tokens.PeekType();

            if (tokenType == TokenType.Identifier)
                return GetIdentifierFactor();

            string lexeme = Tokens.RemoveAndGetLexeme();

            switch (tokenType)
            {
                case TokenType.Integer:
                    return new IntValueNode(int.Parse(lexeme));
                case TokenType.Float:
                    return new FloatValueNode(float.Parse(lexeme));
                case TokenType.String:
                    lexeme = lexeme.Trim('"'); // Remove surrounding quotes from string literals
                    return new StringValueNode(in lexeme);
                case TokenType.ParenthesisOpen:
                    return ParseInnerExpression();
                case TokenType.FunctionCallStartExpression:
                    return GetFunctionCall();
                case TokenType.True:
                    return new BoolValueNode(true);
                case TokenType.False:
                    return new BoolValueNode(false);
                case TokenType.SubtractUnary:
                    return new MathExpressionNode(MathOperator.Multiply, new IntValueNode(-1), ParseFactor());
                default:
                    throw Tokens.GetTokenExceptionError(TokenType.Factor, tokenIndexOffset: 1);

            }
        }
        #endregion
        #endregion

        #region Helper Methods
        #region Type Helper Methods
        private bool IsCatchMessageNext() => 
            Tokens.PeekIsType(TokenType.Identifier) && !Tokens.PeekIsType(TokenType.AssignmentOperator);

        private bool IsDataTypeToken(TokenType tokenType) =>
            tokenType == TokenType.IntegerKeyword || tokenType == TokenType.FloatKeyword ||
            tokenType == TokenType.BoolKeyword || tokenType == TokenType.StringKeyword || 
            tokenType == TokenType.ListKeyword;

        private bool IsCollectionDataType(TokenType tokenType) => tokenType == TokenType.ListKeyword;

        private bool IsExpressionStartingToken() =>
          Tokens.PeekType() switch
          {
              TokenType.FunctionCallStartExpression or TokenType.Integer or
              TokenType.Float or TokenType.String or TokenType.ParenthesisOpen or
              TokenType.False or TokenType.True or TokenType.SubtractUnary or TokenType.Identifier => true,
              _ => false
          };

        private Type GetValueNodeType() =>
            Tokens.RemoveAndGetType() switch
            {
                TokenType.FloatKeyword => typeof(FloatValueNode),
                TokenType.BoolKeyword => typeof(BoolValueNode),
                TokenType.StringKeyword => typeof(StringValueNode),
                TokenType.IntegerKeyword => typeof(IntValueNode),
                _ => throw Tokens.GetTokenExceptionError(TokenType.DataTypeKeyword, tokenIndexOffset: 1)
            };

        private Node GetValueNodeOfType(Type valueNodeType) =>
            valueNodeType switch
            {
                Type t when t == typeof(IntValueNode) => new IntValueNode(),
                Type t when t == typeof(FloatValueNode) => new FloatValueNode(),
                Type t when t == typeof(BoolValueNode) => new BoolValueNode(),
                Type t when t == typeof(StringValueNode) => new StringValueNode(),
                _ => throw Tokens.GetTokenExceptionError(TokenType.DataTypeKeyword, tokenIndexOffset: 1)
            };
        #endregion

        #region Identifiers
        private Node GetIdentifier() =>
            new IdentifierNode(Tokens.MatchRemoveAndGetLexeme(TokenType.Identifier));

        /// <summary>
        /// Returns a factor in the form of a variable identfier or a function call.
        /// </summary>
        private Node GetIdentifierFactor()
        {
            if (Tokens.TryPeekAheadType(out TokenType aheadType) && aheadType == TokenType.ParenthesisOpen)
                return GetFunctionCall();

            return new IdentifierNode(Tokens.RemoveAndGetLexeme());
        }

        /// <summary>
        /// Returns either a function call or a variable assignment statement where the first token is an identifierNode.
        /// </summary>
        private Node GetIdentifierStartStatement()
        {
            if (Tokens.TryPeekAheadType(out var tokenType) && tokenType == TokenType.AssignmentOperator)
                return GetAssignment();

            return GetFunctionCall();
        }
        #endregion

        #region Functions
        private bool IsFunctionDefinitionNext()
        {
            var tokenType = Tokens.PeekType();

            // If "void", "def"
            if (IsFunctionLeadingToken(tokenType))
            {
                _logger?.Verbose("Function definition detected. Token: {Token}", tokenType);
                return true;
            }
                

            // Only other way for token to be function definiton is if it is a data type token
            if (!IsDataTypeToken(tokenType))
                return false;

            // At this point the token could still be a data type for a variable declaration.
            if(TryPeekFunctionParenthesis())
            {
                _logger?.Verbose("Function definition detected. Token: {Token}", tokenType);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Peeks ahead to see if there is a open parenthesis (relative to data type unique to function defs)
        /// </summary>
        /// <returns></returns>
        private bool TryPeekFunctionParenthesis()
        {
            var ahead = Tokens.PeekAhead();

            if (ahead == TokenType.FunctionLabel)
                return true;

            ahead = Tokens.PeekAhead(2);

            if (ahead == TokenType.ParenthesisOpen)
                return true;

            if (ahead != TokenType.Identifier)
                return false;

            ahead = Tokens.PeekAhead(3);

            return ahead == TokenType.ParenthesisOpen;
        }

        private bool IsFunctionLeadingToken(TokenType tokenType)
        {
            // If "def", "define", "function", or "void" it's the first token of a function definition
            switch (tokenType)
            {
                case TokenType.FunctionDefine:
                case TokenType.FunctionLabel:
                case TokenType.FunctionVoid:
                    return true;
            }

            return false; // If not a type above it still could be a data type token
        }
        #endregion
        
        #region  Statements
        private Node? GetOptionalCatchMessage()
        {
            // Optional "("
            Tokens.TryMatchAndRemove(TokenType.ParenthesisOpen);
            Node? messageNode = null;

            // Check for a message identifier and make sure it's not a variable assignment
            if (IsCatchMessageNext())
            {
                /* An optional message variable can be created in catch that will be initialized to
                 * the exception message. */
                var identifier = GetIdentifier();
                var valueSource = new StringValueNode();
                var dataType = valueSource.GetType();

                messageNode = new VariableDeclarationNode(identifier, dataType, valueSource);
            }

            // Optional ")"
            Tokens.TryMatchAndRemove(TokenType.ParenthesisClose);
            return messageNode;
        }
        #endregion
        #endregion
    }
}

