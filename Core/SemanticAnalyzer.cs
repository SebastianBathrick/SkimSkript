using SkimSkript.Nodes;
using SkimSkript.ErrorHandling;
using SkimSkript.Nodes.Runtime;
using SkimSkript.Semantics.Helper;

namespace SkimSkript.Semantics
{
    /// <summary>Class meant to analyze an abstract syntax tree AST semantics prior to interpretation.</summary>
    public class SemanticAnalyzer
    {
        private List<FunctionSemantics> _functionList = new List<FunctionSemantics>();
        private Dictionary<string, int> _variableScopeDict = new Dictionary<string, int>();
        private int _currentScopeDepth = 0;
        private bool _isFunctionReturning = false;

        /// <param name="root">Root of an abstract syntax tree root that requires semantic analysis.</param>
        public SemanticAnalyzer(AbstractSyntaxTreeRoot root)
        {
            CacheFunctionDefinitionData(root.Functions);
            AnalyzeBlock(root);
            AnalyzeFunctionBodies(root.Functions);
        }

        /// <summary>Gathers function definition data to be checked during functon call statements.</summary>
        private void CacheFunctionDefinitionData(List<Node> functionNodes)
        {
            List<Node> allFunctionsList = new List<Node>(functionNodes);
            allFunctionsList.AddRange(BuiltInFunctionNode.GetFunctionInstances());

            foreach (CallableNode function in allFunctionsList)
                _functionList.Add(new FunctionSemantics(function.Identifier, function.ParameterCount, function.IsVariadic, function.ReturnType));
        }

        /// <summary>Checks the semantics of each statement within the scope of a function's block
        /// and those nested within.</summary>
        private void AnalyzeFunctionBodies(List<Node> functionNodes)
        {
            /*
            foreach(FunctionNode function in functionNodes)
            {
                foreach (ParameterNode parameter in function.Parameters)
                    AnalyzeVariableDeclaration((VariableDeclarationNode)parameter.ValueSource);

                AnalyzeBlock((BlockNode)function.Block, GetFunctionWithIdentifier(function.Identifier));

                //Exit parameter scope.
                RemoveCurrentScopeVariables();
            }
            */
        }

        /// <summary>Iterates through each statement and analyzes the semantics of each.</summary>
        private bool AnalyzeBlock(BlockNode blockNode, FunctionSemantics? functionSemantics = null)
        {
            _currentScopeDepth++;

            foreach (StatementNode statement in blockNode.Statements)
            {
                switch (statement)
                {
                    case FunctionCallNode funcNode: AnalyzeFunctionCall(funcNode); break;
                    case AssignmentNode assignNode: AnalyzeAssignment(assignNode); break;
                    case ControlStructNode controlNode: AnalyzeControlStructure(controlNode, functionSemantics); break;
                    case VariableDeclarationNode varNode: AnalyzeVariableDeclaration(varNode); break;
                    case ReturnNode returnNode: AnalyzeFunctionReturn(returnNode, functionSemantics); break;
                }

                if (_isFunctionReturning) // TODO: Add error if there's unreachable code.
                    break;
            }

            //The variables declared in this scope are no longer needed...
            RemoveCurrentScopeVariables();
            _currentScopeDepth--;
            return false;
        }

        private void AnalyzeFunctionReturn(ReturnNode returnNode, FunctionSemantics? functionSemantics)
        {
            if (functionSemantics == null)
                throw new SemanticError("Return statement outside of function.");

            bool isExpression = returnNode.IsExpression;

            if (functionSemantics.IsVoid)
                if (isExpression)
                    throw new SemanticError($"Void Function \"{functionSemantics.Identifier}\" has a return statement containing an expression.");
                else
                {
                    if (!isExpression)
                        throw new SemanticError("Lexeme returning function does not return expression.");

                    AnalyzeExpression(returnNode.Expression);
                }

            _isFunctionReturning = true;
        }

        private void AnalyzeVariableDeclaration(VariableDeclarationNode varNode)
        {
            _variableScopeDict.Add(varNode.Identifier, _currentScopeDepth);
            AnalyzeExpression(varNode.AssignedExpression);
        }


        private void AnalyzeControlStructure(ControlStructNode controlNode, FunctionSemantics? functionSemantics)
        {
            AnalyzeExpression(controlNode.Condition);
            AnalyzeBlock((BlockNode)controlNode.Block, functionSemantics);
        }

        private void AnalyzeAssignment(AssignmentNode assignNode)
        {
            AnalyzeVariableReference((IdentifierNode)assignNode.IdentifierNode);
            AnalyzeExpression(assignNode.AssignedExpression);
        }

        private FunctionSemantics AnalyzeFunctionCall(FunctionCallNode callNode)
        {
            // TODO: Add to check if arguments were appropriately marked as reference.
            string identifier = callNode.Identifier;
            FunctionSemantics? functionSemantics = GetFunctionWithIdentifier(identifier);

            if (functionSemantics == null)
                throw new SemanticError($"Unknown function \"{callNode.Identifier}\" called.");

            int paramCount = functionSemantics.ParameterCount;

            if (callNode.Arguments == null)
                return functionSemantics;

            int argCount = callNode.Arguments.Count;

            if (argCount != paramCount && !functionSemantics.IsVariadic)
                throw new SemanticError($"Function call using {argCount} arguments despite {identifier} only accepting {paramCount}.");

            return functionSemantics;
        }

        // TODO: Fix top-level variable scope handling
        private void AnalyzeVariableReference(IdentifierNode identifierNode)
        {
            /*
            string identifier = identifierNode.ToString();

            if (!_variableScopeDict.ContainsKey(identifier))
                throw new SemanticError($"Reference to unknown variable \"{identifier}\".");
            */
        }

        private FunctionSemantics? GetFunctionWithIdentifier(string identifier)
        {
            for (int i = 0; i < _functionList.Count; i++)
                if (_functionList[i].Identifier == identifier)
                    return _functionList[i];
            return null;
        }

        private void RemoveCurrentScopeVariables()
        {
            foreach (var variable in _variableScopeDict)
                if (variable.Value == _currentScopeDepth)
                    _variableScopeDict.Remove(variable.Key);
        }

        #region Expression Analysis

        private void AnalyzeExpression(Node expression)
        {
            if (expression is ExpressionNode)
            {
                ExpressionNode expr = (ExpressionNode)expression;
                AnalyzeTerm(expr.LeftOperand);
                AnalyzeTerm(expr.RightOperand);
            }
            else
                AnalyzeTerm(expression);
        }

        private void AnalyzeTerm(Node term)
        {
            if (term is ExpressionNode)
                AnalyzeExpression(term);
            else
                AnalyzeFactor(term);
        }

        private void AnalyzeFactor(Node factor)
        {
            if (factor is FunctionCallNode functionCall)
                if (AnalyzeFunctionCall(functionCall).IsVoid)
                    throw new SemanticError($"Expression calls function \"{functionCall.Identifier}\" despite there being no return value.");
                else if (factor is IdentifierNode identifierNode)
                    AnalyzeVariableReference(identifierNode);
        }
        #endregion
    }
}
