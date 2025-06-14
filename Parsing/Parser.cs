using SkimSkript.Nodes.ValueNodes;
using SkimSkript.TokenManagement;
using SkimSkript.ErrorHandling;
using SkimSkript.Nodes;
using SkimSkript.Nodes.StatementNodes;
using SkimSkript.Nodes.ExpressionNodes;
using SkimSkript.Nodes.CallableNodes;

namespace SkimSkript.Parsing
{
    ///<summary> Represents a parser for converting a list of tokens into an abstract syntax tree (AST).
    ///The root of the AST can be retrieved with the root being in the form of an <see cref="SkimSkript.Nodes.AbstractSyntaxTreeRoot"/>
    ///object.</summary>
    public class Parser
    {
        private TokenContainer _tokens;        

        #region Entry Point
        /// <summary>Constructor that builds AST.</summary>
        /// <param name="tokens">Structure containing tokens gathered during the lexical analysis stage.</param>
        public Parser(TokenContainer tokens)
        {
            _tokens = tokens;        
        }

        /// <summary> Returns AST root with top-level statements and functions as childrern. </summary>
        public AbstractSyntaxTreeRoot ParseTokens()
        {
            List<Node> statements = new();
            List<Node> functions = new ();
            
            while (_tokens.HasTokens)
            {
                //If the next token is a statement then parse it and add it to the entry point.
                if (!IsFunctionDefNext())
                    statements.Add(GetStatement());
                else //Otherwise parse functions.
                    functions.Add(GetFunctionNode()); 
            }

            return new AbstractSyntaxTreeRoot(statements.ToArray(), functions.ToArray());
        }
        #endregion

        #region Function Definitions & Blocks
        /// <summary> Parses a function definition and body (block) and returns its data in a node. </summary>
        private Node GetFunctionNode()
        {
            var returnType = GetFunctionReturnType();
            var functionIdentifier = GetIdentifier();     

            // Opening parenthesis to enclose parameter declarations
            _tokens.MatchAndRemove(TokenType.ParenthesisOpen);
            List<Node>? parameters = null;

            // Keep reading parameters until the closing parenthesis
            while (!_tokens.TryMatchAndRemove(TokenType.ParenthesisClose))
            {
                parameters ??= new List<Node>();
                parameters.Add(GetParameter());
            }
                
            var functionBlock = GetBlock();

            return new FunctionNode(functionIdentifier, returnType, parameters?.ToArray(), functionBlock);
        }

        private Node GetParameter()
        {
            // Optional reference keyword(s) -> data type -> parameter identifierNode
            var isRef = _tokens.TryMatchAndRemove(TokenType.PassByReference);
            var dataType = GetValueNodeType();

            return new ParameterNode(isRef, dataType, GetIdentifier());
        }

        private Node GetBlock()
        {
            //Implicit blocks will only add one statement to the list.
            var isImplicitBlock = !_tokens.TryMatchAndRemove(TokenType.BlockOpen);
            List<Node>? statements = null;

            if(isImplicitBlock)
                return new BlockNode([GetStatement()]);

            while (!_tokens.TryMatchAndRemove(TokenType.BlockClose))
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

        /// <summary>Returns either a function call or a variable assignment statment where the first token is an identifierNode.</summary>
        private Node GetIdentifierStartStatement()
        {
            if (_tokens.TryPeekAheadType(out var tokenType) && tokenType == TokenType.AssignmentOperator)
                return GetAssignment();

            return GetFunctionCall();
        }
        #endregion

        #region Function Statements
        /// <summary>Parses function call and any potential arguments sent.</summary>
        private Node GetFunctionCall()
        {
            _tokens.TryMatchAndRemove([TokenType.FunctionCallStart, TokenType.FunctionCallStartExpression]);

            var identifier = GetIdentifier();
            List<Node>? args = null;

            _tokens.MatchAndRemove(TokenType.ParenthesisOpen);
        
            while (!_tokens.TryMatchAndRemove(TokenType.ParenthesisClose))
            {
                // If a value argument then it's an condition, but if reference, it's an identifierNode
                var isRef = _tokens.TryMatchAndRemove(TokenType.PassByReference);
                var value = !isRef ? GetExpression() : GetIdentifier();
                args ??= new List<Node>();
                args.Add(new ArgumentNode(isRef, value));
            }

            return new FunctionCallNode(identifier, args?.ToArray());
        }

        /// <summary>Parses a return statement with or without an conditionalExpression.</summary>
        private Node GetReturnStatement()
        {
            _tokens.MatchAndRemove(TokenType.Return);
            return new ReturnNode(IsExpressionStartingToken() ? GetExpression() : null);
        }
        #endregion

        #region Variable Statements
        /// <summary>Parses a variable declaration and can potentially parse an initilization in the same statement.</summary>
        private Node GetVariableDeclaration()
        {
            _tokens.TryMatchAndRemove(TokenType.DeclarationStart);

            if(IsCollectionDataType(_tokens.PeekType()))
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
            var isInit = _tokens.TryMatchAndRemove([TokenType.VariableInitialize, TokenType.AssignmentOperator]);

            /* If initialized get an condition (that will potetially be coerced runtime by the interpreter).
             * Otherwise get a default value node for the data type in the declaration. */
            var initValue = isInit ? GetExpression() : GetValueNodeOfType(dataType);

            return new VariableDeclarationNode(identifierNode, dataType, initValue);
        }

        /// <summary>Parses an assignment statement where what is presumably a variable or parameter is assigned an condition.</summary>
        private Node GetAssignment()
        {
            _tokens.TryMatchAndRemove(TokenType.AssignmentStart);
            var identifierNode = GetIdentifier();
            _tokens.MatchAndRemove(TokenType.AssignmentOperator);

            return new AssignmentNode(identifierNode, GetExpression());
        }
        #endregion

        #region Control Structure Statements
        private Node GetIfStatement()
        {
            _tokens.MatchAndRemove(TokenType.If);

            var condition = GetExpression();

            _tokens.TryMatchAndRemove(TokenType.Then);

            var block = GetBlock();
            Node? chainedStructure = null;

            if (_tokens.TryMatchAndRemove(TokenType.ElseIf))
                chainedStructure = GetElseIfStatement();
            else if (_tokens.TryMatchAndRemove(TokenType.Else))
                chainedStructure = GetElseStatement();

            return new IfNode(condition, block, chainedStructure);
        }

        private Node GetElseIfStatement()
        {
            var condition = GetExpression();

            _tokens.TryMatchAndRemove(TokenType.Then);

            var block = GetBlock();
            Node? chainedStructure = null;

            if (_tokens.TryMatchAndRemove(TokenType.ElseIf))
                chainedStructure = GetElseIfStatement();
            else if (_tokens.TryMatchAndRemove(TokenType.Else))
                chainedStructure = GetElseStatement();

            return new ElseIfNode(condition, block, chainedStructure);
        }

        private Node GetElseStatement() => 
            new ElseNode(GetBlock());

        private Node GetWhileLoop()
        {
            _tokens.MatchAndRemove(TokenType.WhileLoop);
            var condition = GetExpression();
            _tokens.TryMatchAndRemove(TokenType.Then);
            return new WhileNode(condition, GetBlock());
        }
        #endregion

        #region Misc. Statements
        /// <summary>Parses an assertion statement with an conditional conditionalExpression.</summary>
        private Node GetAssertionStatement()
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
            var leftTerm = ParseComparisonExpression();

            while (_tokens.TryPeek(out var tokenType) && tokenType >= TokenType.And && tokenType <= TokenType.Xor)
            {
                _tokens.Remove();
                var rightTerm = ParseComparisonExpression();
                leftTerm = new LogicExpressionNode((LogicOperator)tokenType, leftTerm, rightTerm);
            }

            return leftTerm;
        }

        /// <summary>Parses comparison expressions which are the second lowest precedence.</summary>
        private Node ParseComparisonExpression()
        {
            var leftNode = ParseArithmeticExpression();

            while (_tokens.TryPeek(out var tokenType) && tokenType >= TokenType.Equals && tokenType <= TokenType.LessThanOrEqual)
            {
                _tokens.Remove();
                Node rightNode = ParseArithmeticExpression(); // Comparison operators act on arithmetic expressions
                leftNode = new ComparisonExpressionNode((ComparisonOperator)tokenType, leftNode, rightNode);
            }

            return leftNode;
        }

        /// <summary>Parses addition and/or subtraction while serving as the entry point for arithmetic expressions.
        /// All arithmetic operators have a higher precedence than conditional operators and follows standard
        /// operator precedence regarding arithmetic.</summary>
        private Node ParseArithmeticExpression()
        {
            Node leftNode = ParseTerm();

            while (_tokens.TryPeek(out var tokenType) && tokenType == TokenType.Add || tokenType == TokenType.Subtract)
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
            var enclosedExpression = GetExpression();
            _tokens.MatchAndRemove(TokenType.ParenthesisClose);
            return enclosedExpression;
        }

        /// <summary>Handles a term which could possibly contain embedded expressions, terms, or factors.</summary>
        private Node ParseTerm()
        {
            var leftFactor = ParseExponentTerm();

            while (_tokens.TryPeek(out var tokenType) && tokenType >= TokenType.Multiply && tokenType <= TokenType.Exponent)
            {
                _tokens.Remove();
                var rightFactor = ParseExponentTerm();
                leftFactor = new MathExpressionNode((MathOperator)tokenType, leftFactor, rightFactor);
            }

            return leftFactor;
        }

        /// <summary>Handles exponent that happens to be of highest precedence.</summary>
        private Node ParseExponentTerm()
        {
            var baseFactor = ParseFactor();

            while (_tokens.TryPeek(out var tokenType) && tokenType == TokenType.Exponent)
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
            var tokenType = _tokens.PeekType();

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


        #endregion

        #region Helper Methods
        private bool IsCollectionDataType(TokenType tokenType) => tokenType == TokenType.ListKeyword;

        private bool IsFunctionDefNext() =>
            _tokens.PeekType() switch
            {
                TokenType.FunctionIntDefine or TokenType.FunctionFloatDefine or TokenType.FunctionStringDefine or TokenManagement.TokenType.FunctionVoidDefine
                or TokenType.FunctionBoolDefine => true,
                _ => false
            };

        private bool IsExpressionStartingToken() =>
          _tokens.PeekType() switch
          {
              TokenType.FunctionCallStartExpression or TokenType.Integer or
              TokenType.Float or TokenType.String or TokenType.ParenthesisOpen or
              TokenType.False or TokenType.True or TokenType.Subtract or TokenType.Identifier => true,
              _ => false
          };

        private Type? GetFunctionReturnType() =>
            _tokens.RemoveAndGetType() switch
            {
                TokenType.FunctionIntDefine => typeof(IntValueNode),
                TokenType.FunctionFloatDefine => typeof(FloatValueNode),
                TokenType.FunctionStringDefine => typeof(StringValueNode),
                TokenType.FunctionBoolDefine => typeof(BoolValueNode),
                _ => null
            };

        private Type GetValueNodeType() =>
            _tokens.RemoveAndGetType() switch
            {
                TokenType.FloatKeyword => typeof(FloatValueNode),
                TokenType.BoolKeyword => typeof(BoolValueNode),
                TokenType.StringKeyword => typeof(StringValueNode),
                TokenType.IntegerKeyword => typeof(IntValueNode),
                _ => throw new SyntaxError($"Invalid data type token", _tokens, ErrorTokenPosition.Backward)
            };

        private Node GetValueNodeOfType(Type valueNodeType) =>
            valueNodeType switch
            {
                Type t when t == typeof(IntValueNode) => new IntValueNode(),
                Type t when t == typeof(FloatValueNode) => new FloatValueNode(),
                Type t when t == typeof(BoolValueNode) => new BoolValueNode(),
                Type t when t == typeof(StringValueNode) => new StringValueNode(),
                _ => throw new SyntaxError($"Invalid value node type: {valueNodeType}.", _tokens, ErrorTokenPosition.Backward)
            };

        private void GetDeclaredType()
        {
            //var tokenType = _tokens.RemoveAndGetType();

            //if (!IsCollectionDataType(_tokens.PeekType()))
               // return GetTypeValueNode(tokenType);


            // The list keyword trails and can enclose the type
        }

        private Node GetIdentifier() => 
            new IdentifierNode(_tokens.MatchRemoveAndGetLexeme(TokenType.Identifier));

        /// <summary>Returns a factor in the form of a variable identfier or a function call.</summary>
        private Node GetIdentifierFactor()
        {
            if (_tokens.TryPeekAheadType(out TokenType aheadType) && aheadType == TokenType.ParenthesisOpen)
                return GetFunctionCall();

            return new IdentifierNode(_tokens.RemoveAndGetLexeme());
        }
        #endregion
    }
}

