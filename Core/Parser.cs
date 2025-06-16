using SkimSkript.TokenManagement;
using SkimSkript.ErrorHandling;
using SkimSkript.Nodes;
using SkimSkript.Logging;

namespace SkimSkript.Parsing
{
    ///<summary> Represents a parser for converting a list of tokens into an abstract syntax tree (AST).
    ///The root of the AST can be retrieved with the root being in the form of an <see cref="SkimSkript.Nodes.AbstractSyntaxTreeRoot"/>
    ///object.</summary>
    internal class Parser
    {
        private TokenContainer? _tokens;
        private Logger _log;

        private TokenContainer Tokens => _tokens!;

        public Parser(Logger log) => _log = log;

        /// <summary> Returns AST root with top-level statements and functions as childrern. </summary>
        public AbstractSyntaxTreeRoot BuildAbstractSyntaxTree(TokenContainer tokens)
        {
            _tokens = tokens;

            List<Node> statements = new();
            List<Node> functions = new ();
            
            while (Tokens.HasTokens)
            {
                //If the next token is a statement then parse it and add it to the entry point.
                if (!IsFunctionDefNext())
                {
                    statements.Add(GetStatement());
                    _log.Verbose("Parsed top-level statement: {Statement}", statements.Last());
                }                   
                else //Otherwise parse functions.
                    functions.Add(GetFunctionNode()); 
            }

            return new AbstractSyntaxTreeRoot(statements.ToArray(), functions.ToArray());
        }

        #region Function Definitions & Blocks
        /// <summary> Parses a function definition and body (block) and returns its data in a node. </summary>
        private Node GetFunctionNode()
        {
            var returnType = GetFunctionReturnType();
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
                
            return new FunctionNode(identifier, returnType, parameters?.ToArray(), GetBlock());
        }

        private Node GetParameter()
        {
            // Optional reference keyword(s) -> data type -> parameter identifierNode
            var isRef = Tokens.TryMatchAndRemove(TokenType.PassByReference);
            var dataType = GetValueNodeType();

            return new ParameterNode(isRef, dataType, GetIdentifier());
        }

        private Node GetBlock()
        {
            //Implicit blocks will only add one statement to the list.
            var isImplicitBlock = !Tokens.TryMatchAndRemove(TokenType.BlockOpen);
            List<Node>? statements = null;

            if(isImplicitBlock)
                return new BlockNode([GetStatement()]);

            while (!Tokens.TryMatchAndRemove(TokenType.BlockClose))
            {
                statements ??= new List<Node>();
                statements.Add(GetStatement());
            }          

            return new BlockNode(statements?.ToArray());
        }
        #endregion

        #region Statements
        #region Primary Statement Parsing Methods
        /// <summary>Determines using the front-most token what type of statement needs to be parsed
        /// next, and then parses that statement.</summary>
        /// <exception cref="SyntaxError"></exception>
        private Node GetStatement() =>
            Tokens.PeekType() switch
            {
                TokenType.DeclarationStart or TokenType.IntegerKeyword or
                TokenType.StringKeyword or TokenType.FloatKeyword or
                TokenType.BoolKeyword => GetVariableDeclaration(),           
                TokenType.FunctionCallStart => GetFunctionCall(),
                TokenType.Return => GetReturnStatement(),
                TokenType.AssignmentStart => GetAssignment(),
                TokenType.WhileLoop => GetWhileLoop(),
                TokenType.If => GetIfStatement(),    
                TokenType.Identifier => GetIdentifierStartStatement(), //The function will be called here,
                TokenType.Assertion => GetAssertionStatement(),
                _ => throw new SyntaxError("Expected new statement but instead found an invalid token.", Tokens, ErrorTokenPosition.InPlace)
            };

        /// <summary>Returns either a function call or a variable assignment statment where the first token is an identifierNode.</summary>
        private Node GetIdentifierStartStatement()
        {
            if (Tokens.TryPeekAheadType(out var tokenType) && tokenType == TokenType.AssignmentOperator)
                return GetAssignment();

            return GetFunctionCall();
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

            return new FunctionCallNode(identifier, args?.ToArray());
        }

        /// <summary>Parses a return statement with or without an conditionalExpression.</summary>
        private Node GetReturnStatement()
        {
            Tokens.MatchAndRemove(TokenType.Return);
            return new ReturnNode(IsExpressionStartingToken() ? GetExpression() : null);
        }
        #endregion

        #region Variable Statements
        /// <summary>Parses a variable declaration and can potentially parse an initilization in the same statement.</summary>
        private Node GetVariableDeclaration()
        {
            Tokens.TryMatchAndRemove(TokenType.DeclarationStart);

            if(IsCollectionDataType(Tokens.PeekType()))
                return GetCollectionDeclaration();

            return GetValueTypeVariableDeclaration();
        }

        private Node GetCollectionDeclaration()
        {
            throw new NotImplementedException("Collection declarations are not yet implemented.");
        }

        /// <summary> Parses a variable or parameter declaration with a value data type. </summary>
        private Node GetValueTypeVariableDeclaration()
        {
            var dataType = GetValueNodeType();
            var identifierNode = GetIdentifier();

            /* AssignmentOperator can be used for both initialization and assignment. However VariableInitialize
             * is only used for initialization which is why the tokens are seperate. */
            var isInit = Tokens.TryMatchAndRemove([TokenType.VariableInitialize, TokenType.AssignmentOperator]);

            /* If initialized get an condition (that will potetially be coerced runtime by the interpreter).
             * Otherwise get a default value node for the data type in the declaration. */
            var initValue = isInit ? GetExpression() : GetValueNodeOfType(dataType);

            return new VariableDeclarationNode(identifierNode, dataType, initValue);
        }

        /// <summary>Parses an assignment statement where what is presumably a variable or parameter is assigned an condition.</summary>
        private Node GetAssignment()
        {
            Tokens.TryMatchAndRemove(TokenType.AssignmentStart);
            var identifierNode = GetIdentifier();
            Tokens.MatchAndRemove(TokenType.AssignmentOperator);

            return new AssignmentNode(identifierNode, GetExpression());
        }
        #endregion

        #region Control Structure Statements
        private Node GetIfStatement(bool isElseIf=false)
        {
            /* For this method to even be called the if token will be verified and that's why
             * it can be Try. It is Try because this method is reused for else-if statements.*/
            Tokens.TryMatchAndRemove(TokenType.If);

            var condition = GetExpression();

            Tokens.TryMatchAndRemove(TokenType.Then);

            var block = GetBlock();
            Node? chainedStructure = null;

            if (Tokens.TryMatchAndRemove(TokenType.ElseIf))
                chainedStructure = GetIfStatement(true);
            else if (Tokens.TryMatchAndRemove(TokenType.Else))
                chainedStructure = new ElseNode(GetBlock());

            return !isElseIf ? 
                new IfNode(condition, block, chainedStructure) : 
                new ElseIfNode(condition, block,chainedStructure);
        }

        private Node GetWhileLoop()
        {
            Tokens.MatchAndRemove(TokenType.WhileLoop);
            var condition = GetExpression();
            Tokens.TryMatchAndRemove(TokenType.Then);
            return new WhileNode(condition, GetBlock());
        }
        #endregion

        #region Misc. Statements
        /// <summary>Parses an assertion statement with an conditional conditionalExpression.</summary>
        private Node GetAssertionStatement()
        {
            Tokens.MatchAndRemove(TokenType.Assertion);
            var conditionalExpression = GetExpression();
            return new AssertionNode(conditionalExpression);
        }
        #endregion
        #endregion

        #region Expressions
        #region Primary Expression Parsing
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

            while (Tokens.TryPeek(out var tokenType) && tokenType == TokenType.Add || tokenType == TokenType.Subtract)
            {
                Tokens.Remove();
                Node rightNode = ParseTerm(); // Low-precedence arithmetic (add/subtract) acts on terms
                leftNode = new MathExpressionNode((MathOperator)tokenType, leftNode, rightNode);
            }

            return leftNode;
        }

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

        private Node ParseFactor()
        {
            var tokenType = Tokens.PeekType();

            if (tokenType == TokenType.Identifier)
                return GetIdentifierFactor();

            string lexeme = Tokens.RemoveAndGetLexeme();

            switch (tokenType)
            {
                case TokenType.Integer: return new IntValueNode(int.Parse(lexeme));
                case TokenType.Float: return new FloatValueNode(float.Parse(lexeme));
                case TokenType.String: 
                    lexeme = lexeme.Trim('"'); // Remove surrounding quotes from string literals
                    return new StringValueNode(in lexeme);
                case TokenType.ParenthesisOpen: return ParseInnerExpression();
                case TokenType.FunctionCallStartExpression: return GetFunctionCall();
                case TokenType.True: return new BoolValueNode(true);
                case TokenType.False: return new BoolValueNode(false);
                case TokenType.Subtract: return new MathExpressionNode(MathOperator.Multiply, new IntValueNode(-1), ParseFactor());
                default: throw new SyntaxError("Invalid factor in conditionalExpression.", Tokens, ErrorTokenPosition.Backward);
            }
        }
        #endregion

        #region Extra Feature Expressions Parsing
        private Node ParseInnerExpression()
        {
            var enclosedExpression = GetExpression();
            Tokens.MatchAndRemove(TokenType.ParenthesisClose);
            return enclosedExpression;
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
        #endregion

        #region Helper Methods
        private bool IsCollectionDataType(TokenType tokenType) => tokenType == TokenType.ListKeyword;

        private bool IsFunctionDefNext() =>
            Tokens.PeekType() switch
            {
                TokenType.FunctionIntDefine or TokenType.FunctionFloatDefine or TokenType.FunctionStringDefine or TokenManagement.TokenType.FunctionVoidDefine
                or TokenType.FunctionBoolDefine => true,
                _ => false
            };

        private bool IsExpressionStartingToken() =>
          Tokens.PeekType() switch
          {
              TokenType.FunctionCallStartExpression or TokenType.Integer or
              TokenType.Float or TokenType.String or TokenType.ParenthesisOpen or
              TokenType.False or TokenType.True or TokenType.Subtract or TokenType.Identifier => true,
              _ => false
          };

        private Type? GetFunctionReturnType() =>
            Tokens.RemoveAndGetType() switch
            {
                TokenType.FunctionIntDefine => typeof(IntValueNode),
                TokenType.FunctionFloatDefine => typeof(FloatValueNode),
                TokenType.FunctionStringDefine => typeof(StringValueNode),
                TokenType.FunctionBoolDefine => typeof(BoolValueNode),
                _ => null
            };

        private Type GetValueNodeType() =>
            Tokens.RemoveAndGetType() switch
            {
                TokenType.FloatKeyword => typeof(FloatValueNode),
                TokenType.BoolKeyword => typeof(BoolValueNode),
                TokenType.StringKeyword => typeof(StringValueNode),
                TokenType.IntegerKeyword => typeof(IntValueNode),
                _ => throw new SyntaxError($"Invalid data type token", Tokens, ErrorTokenPosition.Backward)
            };

        private Node GetValueNodeOfType(Type valueNodeType) =>
            valueNodeType switch
            {
                Type t when t == typeof(IntValueNode) => new IntValueNode(),
                Type t when t == typeof(FloatValueNode) => new FloatValueNode(),
                Type t when t == typeof(BoolValueNode) => new BoolValueNode(),
                Type t when t == typeof(StringValueNode) => new StringValueNode(),
                _ => throw new SyntaxError($"Invalid value node type: {valueNodeType}.", Tokens, ErrorTokenPosition.Backward)
            };

        private void GetDeclaredType()
        {
            //var tokenType = _tokens.RemoveAndGetType();

            //if (!IsCollectionDataType(_tokens.PeekType()))
               // return GetTypeValueNode(tokenType);


            // The list keyword trails and can enclose the type
        }

        private Node GetIdentifier() => 
            new IdentifierNode(Tokens.MatchRemoveAndGetLexeme(TokenType.Identifier));

        /// <summary>Returns a factor in the form of a variable identfier or a function call.</summary>
        private Node GetIdentifierFactor()
        {
            if (Tokens.TryPeekAheadType(out TokenType aheadType) && aheadType == TokenType.ParenthesisOpen)
                return GetFunctionCall();

            return new IdentifierNode(Tokens.RemoveAndGetLexeme());
        }
        #endregion
    }
}

