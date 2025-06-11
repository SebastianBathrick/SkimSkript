using SkimSkript.Nodes.ValueNodes;
using SkimSkript.TokenManagement;
using SkimSkript.ErrorHandling;
using SkimSkript.Nodes;
using SkimSkript.Nodes.StatementNodes;

namespace SkimSkript.Parsing
{
    ///<summary> Represents a parser for converting a list of tokens into an abstract syntax tree (AST).
    ///The root of the AST can be retrieved with the root being in the form of an <see cref="SkimSkript.Nodes.AbstractSyntaxTreeRoot"/>
    ///object.</summary>
    public class Parser
    {
        private AbstractSyntaxTreeRoot _abstractSyntaxTreeRoot;
        private TokenContainer _tokens;        
        
        /// <summary>Represents an abstract syntax tree (AST) root created during parsing to be utilized runtime.</summary>
        public AbstractSyntaxTreeRoot AbstractSyntaxTreeRoot => _abstractSyntaxTreeRoot;

        /// <summary>Constructor that builds AST.</summary>
        /// <param name="tokens">Structure containing tokens gathered during the lexical analysis stage.</param>
        public Parser(TokenContainer tokens)
        {
            _tokens = tokens;        
            _abstractSyntaxTreeRoot = ParseTokens();
        }

        /// <summary>Parses user-defined functions defintions & bodies alongside any top-level statements,
        /// then stores them in a node representing the AST root.</summary>
        private AbstractSyntaxTreeRoot ParseTokens()
        {
            List<Node> statements = new List<Node>();
            List<Node> functions = new List<Node>();
            
            while (_tokens.HasTokens)
            {
                //If the next token is a statement then parse it and add it to the entry point.
                if (!IsFunctionDefNext())
                    statements.Add(GetStatement());
                else //Otherwise parse functions.
                    functions.Add(GetFunctionNode()); 
            }

            return new AbstractSyntaxTreeRoot(statements, functions);
        }

        #region Function & Blocks
        /// <summary>Parses a function definition along with its body.</summary>
        private FunctionNode GetFunctionNode()
        {
            //If return type is null that means its a void returning function.
            ValueNode? returnTypeNode = GetFunctionReturnTypeNode();

            string identifier = _tokens.RemoveAndGetLexeme();
            List<Node> parameters = new List<Node>();

            //In case parameter declarations are enclosed in parenthesis
            bool isParenthesis = _tokens.TryMatchAndRemove(TokenType.ParenthesisOpen);

            //Create parameters until the block begins
            while (IsParameterDeclarations(isParenthesis))
            {
                bool isRef = _tokens.TryMatchAndRemove(TokenType.PassByReference);
                var parameterDeclaration = (VariableDeclarationNode)GetVariableDeclaration();
                parameters.Add(new ParameterNode(parameterDeclaration, isRef, parameterDeclaration.Identifier));
            }

            if (isParenthesis)
                _tokens.MatchAndRemove(TokenType.ParenthesisClose);

            return new FunctionNode(identifier, returnTypeNode, parameters, GetBlock(true));
        }

        /// <summary>Parses each statement within a block's scope.</summary>
        /// <param name="isFunctionBlock">Indicates whether the statments are within the scope of a function.</param>
        /// <remarks>If no opening block token is found and the block is not for a function it will then be 
        /// implicit, only parsing one statement. Function blocks can be implied using a special token type.
        /// Nested scopes are handled via indirect recursive calls.</remarks>
        private BlockNode GetBlock(bool isFunctionBlock = false)
        {
            //Implicit blocks will only add one statement to the list.
            bool isImplicitBlock = !_tokens.TryMatchAndRemove(TokenType.BlockOpen) && !isFunctionBlock;
            isImplicitBlock = isImplicitBlock || (isFunctionBlock && _tokens.TryMatchAndRemove(TokenType.FunctionImpliedBlock));
            List<Node> statements = new List<Node>();

            do
            {
                statements.Add(GetStatement());
            }
            while (!isImplicitBlock && !_tokens.TryMatchAndRemove(TokenType.BlockClose));

            return new BlockNode(statements);
        }
        #endregion

        #region Statements
        #region Primary Statement Parsing Methods
        /// <summary>Determines using the front-most token what type of statement needs to be parsed
        /// next, and then parses that statement.</summary>
        /// <exception cref="SyntaxError"></exception>
        private StatementNode GetStatement() =>
            _tokens.PeekType() switch
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
                _ => throw new SyntaxError("Expected new statement but instead found an invalid token.", _tokens, ErrorTokenPosition.InPlace)
            };

        /// <summary>Returns either a function call or a variable assignment statment where the first token is an identifier.</summary>
        private StatementNode GetIdentifierStartStatement()
        {
            if (_tokens.TryPeekAheadType(out TokenType tokenType) && tokenType == TokenType.AssignmentOperator)
                return GetAssignment();

            return GetFunctionCall();
        }
        #endregion

        #region Function Statements
        /// <summary>Parses function call and any potential arguments sent.</summary>
        private StatementNode GetFunctionCall()
        {
            _tokens.TryMatchAndRemove(tokenType: TokenType.FunctionCallStart);
            _tokens.TryMatchAndRemove(TokenType.FunctionCallStartExpression);

            string identifier = _tokens.MatchRemoveAndGetLexeme(TokenType.Identifier);

            _tokens.MatchAndRemove(TokenType.ParenthesisOpen);

            var args = new List<(Node, bool)>();

            while (!_tokens.TryMatchAndRemove(TokenType.ParenthesisClose))
                args.Add(GetArgument());

            return new FunctionCallNode(identifier, args);
        }

        /// <summary>Parses a return statement with or without an conditionalExpression.</summary>
        private StatementNode GetReturnStatement()
        {
            _tokens.MatchAndRemove(TokenType.Return);
            return new ReturnNode(IsExpressionStartingToken(_tokens.PeekType()) ? GetExpression() : null);
        }
        #endregion

        #region Variable Statements
        /// <summary>Parses a variable declaration and can potentially parse an initilization in the same statement.</summary>
        private StatementNode GetVariableDeclaration()
        {
            _tokens.TryMatchAndRemove(TokenType.DeclarationStart);

            if(IsCollectionDataType(_tokens.PeekType()))
                return GetCollectionDeclaration();

            return GetValueTypeVariableDeclaration();
        }

        private StatementNode GetCollectionDeclaration()
        {
            throw new NotImplementedException("Collection declarations are not yet implemented.");
        }

        private StatementNode GetValueTypeVariableDeclaration()
        {
            ValueNode valueType = GetTypeValueNode(_tokens.RemoveAndGetType());
            string identifier = _tokens.MatchRemoveAndGetLexeme(TokenType.Identifier);

            //If there is an conditionalExpression, parse it, and store its root in the statement node.
            //An assignment operator can be used prior to an conditionalExpression, or a reserved type meant only for initializing variables.
            if (_tokens.TryMatchAndRemove(TokenType.VariableInitialize) || _tokens.TryMatchAndRemove(TokenType.AssignmentOperator))
                return new VariableDeclarationNode(identifier, GetExpression(), valueType);

            //If there is no conditionalExpression assigned instantiate a ValueNode with a default value.
            return new VariableDeclarationNode(identifier, valueType);
        }

        /// <summary>Parses an assignment statement where what is presumably a variable or parameter is assigned an expression.</summary>
        private StatementNode GetAssignment()
        {
            _tokens.TryMatchAndRemove(TokenType.AssignmentStart);
            string identifer = _tokens.MatchRemoveAndGetLexeme(TokenType.Identifier);
            _tokens.MatchAndRemove(TokenType.AssignmentOperator);

            return new AssignmentNode(new IdentifierNode(identifer), GetExpression());
        }
        #endregion

        #region Control Structure Statements
        /// <summary>Parses an if statement along with recursively parsing else if, and else structures.</summary>
        /// <param name="isSelfElse">Indicates whether the given call is recursive and for an else if or else structure.</param>
        private StatementNode GetIfStatement(bool isSelfElse)
        {
            //Could be if, else if, or else. Type already verified prior to call.
            _tokens.Remove();
            Node condition = !isSelfElse ? GetExpression() : new BoolValueNode(true);
            _tokens.TryMatchAndRemove(TokenType.Then);
            BlockNode block = GetBlock();

            //If either case passes recursively parse the chained statement and then store it in this call's return node.
            if(_tokens.HasTokens)
                if (_tokens.PeekType() == TokenType.ElseIf)
                    return new IfNode(condition, block, (IfNode)GetIfStatement(false));
                else if(_tokens.PeekType() == TokenType.Else)
                    return new IfNode(condition, block, (IfNode)GetIfStatement(true));

            //Return if statement with no else/else if.
            return new IfNode(condition, block, null);
        }

        /// <summary>Parses a while loop with its condition and associated block.</summary>
        private StatementNode GetWhileLoop()
        {
            _tokens.MatchAndRemove(TokenType.WhileLoop);
            Node expression = GetExpression();
            _tokens.TryMatchAndRemove(TokenType.Then);
            return new WhileNode(expression, GetBlock());
        }
        #endregion

        #region Misc. Statements
        /// <summary>Parses an assertion statement with an conditional conditionalExpression.</summary>
        private StatementNode GetAssertionStatement()
        {
            _tokens.MatchAndRemove(TokenType.Assertion);
            var conditionalExpression = GetExpression();
            return new AssertionNode(conditionalExpression);
        }
        #endregion
        #endregion

        #region Expressions
        /// <summary>Parses a logical, comparison, and/or arithmetic expressions. Starting with
        /// logical expressions as they are the lowest precedence level.</summary>
        private Node GetExpression() => ParseLogicalExpression();

        /// <summary>Can parse expressions containing the lowest precedence logical operators. Serves as
        /// the entry point for parsing all expressions (conditionals AND mathematical).</summary>
        private Node ParseLogicalExpression()
        {
            Node leftTerm = ParseComparisonExpression();
            TokenType tokenType;

            while (_tokens.HasTokens && (tokenType = _tokens.PeekType()) >= TokenType.And && tokenType <= TokenType.Xor)
            {
                _tokens.Remove();
                Node rightTerm = ParseComparisonExpression();
                leftTerm = new ConditionExpressionNode((LogicalOperator)tokenType, leftTerm, rightTerm);
            }

            return leftTerm;
        }

        /// <summary>Parses comparison expressions which are the second lowest precedence.</summary>
        private Node ParseComparisonExpression()
        {
            Node leftNode = ParseArithmeticExpression();
            TokenType tokenType;

            while (_tokens.HasTokens && (tokenType = _tokens.PeekType()) >= TokenType.Equals && tokenType <= TokenType.LessThanOrEqual)
            {
                _tokens.Remove();
                Node rightNode = ParseArithmeticExpression(); // Comparison operators act on arithmetic expressions
                leftNode = new ConditionExpressionNode((ComparisonOperator)tokenType, leftNode, rightNode);
            }

            return leftNode;
        }

        /// <summary>Parses addition and/or subtraction while serving as the entry point for arithmetic expressions.
        /// All arithmetic operators have a higher precedence than conditional operators and follows standard
        /// operator precedence regarding arithmetic.</summary>
        private Node ParseArithmeticExpression()
        {
            Node leftNode = ParseTerm();
            TokenType tokenType;

            while (_tokens.HasTokens && ((tokenType = _tokens.PeekType()) == TokenType.Add || tokenType == TokenType.Subtract))
            {
                _tokens.Remove();
                Node rightNode = ParseTerm(); // Low-precedence arithmetic (add/subtract) acts on terms
                leftNode = new MathExpressionNode((MathOperator)tokenType, leftNode, rightNode);
            }

            return leftNode;
        }

        /// <summary>Parses an conditionalExpression enclosed by parenthesis.</summary>
        private Node ParseInnerExpression()
        {
            Node expression = GetExpression();
            _tokens.MatchAndRemove(TokenType.ParenthesisClose);
            return expression;
        }

        /// <summary>Handles a term which could possibly contain embedded expressions, terms, or factors.</summary>
        private Node ParseTerm()
        {
            Node leftFactor = ParseExponentTerm();
            TokenType tokenType;

            while (_tokens.HasTokens && (tokenType = _tokens.PeekType()) >= TokenType.Multiply && tokenType <= TokenType.Exponent)
            {
                _tokens.Remove();
                Node rightFactor = ParseExponentTerm();
                leftFactor = new MathExpressionNode((MathOperator)tokenType, leftFactor, rightFactor);
            }

            return leftFactor;
        }

        /// <summary>Handles exponent that happens to be of highest precedence.</summary>
        private Node ParseExponentTerm()
        {
            Node baseFactor = ParseFactor();
            TokenType tokenType;

            while (_tokens.HasTokens && (tokenType = _tokens.PeekType()) == TokenType.Exponent)
            {
                _tokens.Remove();
                Node exponent = ParseExponentTerm(); // Recursive call for right associativity
                baseFactor = new MathExpressionNode(MathOperator.Exponent, baseFactor, exponent);
            }

            return baseFactor;
        }

        /// <summary>Handles a factor of varying types like literals, variable identifiers, and function calls.</summary>
        private Node ParseFactor()
        {
            TokenType tokenType = _tokens.PeekType();

            if(tokenType == TokenType.Identifier)
                return GetIdentifierFactor();

            string lexeme = _tokens.RemoveAndGetLexeme();

            switch (tokenType)
            {
                case TokenType.Integer: return new IntValueNode(int.Parse(lexeme));
                case TokenType.Float: return new FloatValueNode(float.Parse(lexeme));
                case TokenType.String: return new StringValueNode(in lexeme);
                case TokenType.ParenthesisOpen: return ParseInnerExpression();
                case TokenType.FunctionCallStartExpression: return GetFunctionCall();
                case TokenType.True: return new BoolValueNode(true);
                case TokenType.False: return new BoolValueNode(false);
                case TokenType.Subtract: return new MathExpressionNode(MathOperator.Multiply, new IntValueNode(-1), ParseFactor());
                default: throw new SyntaxError("Invalid factor in conditionalExpression.", _tokens, ErrorTokenPosition.Backward);
            } 
        }

        /// <summary>Returns a factor in the form of a variable identfier or a function call.</summary>
        private Node GetIdentifierFactor()
        {
            if (_tokens.TryPeekAheadType(out TokenType aheadType) && aheadType == TokenType.ParenthesisOpen)
                return GetFunctionCall();

            return new IdentifierNode(_tokens.RemoveAndGetLexeme());
        }
        #endregion

        #region Utility Methods
        #region Token Type Checkers
        private bool IsCollectionDataType(TokenType tokenType) => tokenType == TokenType.ListKeyword;

        // TODO: Refactor token type checkers to accept token type parameter instead of using PeekType() directly and potentially split methods into seperate class.
        private bool IsParameterDeclarations(bool usesParenthesis) =>
           !usesParenthesis && _tokens.PeekType() != TokenType.BlockOpen && _tokens.PeekType() != TokenType.FunctionImpliedBlock
            || usesParenthesis && _tokens.PeekType() != TokenType.ParenthesisClose;

        private bool IsFunctionDefNext() =>
            _tokens.PeekType() switch
            {
                TokenType.FunctionIntDefine or TokenType.FunctionFloatDefine or TokenType.FunctionStringDefine or TokenManagement.TokenType.FunctionVoidDefine
                or TokenType.FunctionBoolDefine => true,
                _ => false
            };

        private bool IsExpressionStartingToken(TokenType tokenType) =>
          tokenType switch
          {
              TokenType.FunctionCallStartExpression or TokenType.Integer or
              TokenType.Float or TokenType.String or TokenType.ParenthesisOpen or
              TokenType.False or TokenType.True or TokenType.Subtract or TokenType.Identifier => true,
              _ => false
          };
        #endregion

        #region Get ValueNode Methods
        private ValueNode? GetFunctionReturnTypeNode() =>
            _tokens.RemoveAndGetType() switch
            {
                TokenType.FunctionIntDefine => new IntValueNode(),
                TokenType.FunctionFloatDefine => new FloatValueNode(),
                TokenType.FunctionStringDefine => new StringValueNode(),
                TokenType.FunctionBoolDefine => new BoolValueNode(),
                _ => null
            };


        /// <summary>Returns a value type node based on the token type.</summary>
        private ValueNode GetTypeValueNode(TokenType tokenType) =>
            tokenType switch
            {
                TokenType.FloatKeyword => new FloatValueNode(),
                TokenType.BoolKeyword => new BoolValueNode(),
                TokenType.StringKeyword => new StringValueNode(),
                _ => new IntValueNode(),
            };
        #endregion

        #region Misc. Methods
        // TODO: Revisit this meth to check if it can be merged with its caller
        private (Node, bool) GetArgument() 
        {
            bool isRef = _tokens.TryMatchAndRemove(TokenType.PassByReference);

            if(!isRef)
                return (GetExpression(), false);
            
            //Pass by reference arguments can only be variable identifiers.
            string varIdentifier = _tokens.MatchRemoveAndGetLexeme(TokenType.Identifier);
            return (new IdentifierNode(varIdentifier), isRef);
        }

        private StatementNode GetIfStatement() => GetIfStatement(false);
        #endregion
        #endregion
    }
}

