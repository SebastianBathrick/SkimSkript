using SkimSkript.Interpretation.Helpers;
using SkimSkript.Nodes;
using SkimSkript.Nodes.ExpressionNodes;
using SkimSkript.Nodes.NodeExceptions;
using SkimSkript.Nodes.Runtime;
using SkimSkript.Nodes.StatementNodes;

namespace SkimSkript.Interpretation
{
    using ArgData = (Node source, bool isRef);

    /// <summary> The <c>Interpreter</c> class is responsible for executing the program represented by an
    ///  Abstract Syntax Tree (AST) generated from the parsed source code. It handles function calls, 
    ///  variable management, expressions,  and control structures (e.g., if, while) to produce 
    ///  runtime results. </summary>
    public class Interpreter
    {        
        private ScopeManager _scope = new();
        private CoercionInterpreter _coercionInterpreter = new();
        private OperationInterpreter _operationInterpreter = new();
        private Dictionary<string, CallableNode> _callableFunctionsDict; 

        public Interpreter(AbstractSyntaxTreeRoot root)
        {
            _callableFunctionsDict = AddCallableFunctionsToMap(root.Functions);
            InterpretBlock(root);
        }

        /// <summary> Interprets block and its executible statements then returns data about exit. </summary>
        private BlockExitData InterpretBlock(Node block)
        {            
            _scope.EnterScope();          
            var statements = ((BlockNode)block).Statements;
            BlockExitData? exitData = null;

            foreach (StatementNode statement in statements)
            {
                exitData = InterpretStatement(statement);

                // If it has exit data but it wasn't a control structure that executed all statements
                if (exitData != null && exitData.ExitType != BlockExitType.StatementsExhausted)
                    break;
            }

            _scope.ExitScope();
            return exitData ?? new BlockExitData(BlockExitType.StatementsExhausted);
        }

        /// <summary> Returns dictionary of built-in & user-defined functions mapped to their idenifiers. </summary>
        private Dictionary<string, CallableNode> AddCallableFunctionsToMap(List<Node> functions)
        {
            var funcDict = new Dictionary<string, CallableNode>();

            //Add built in functions
            foreach (BuiltInFunctionNode function in BuiltInFunctionNode.GetFunctionInstances())
                funcDict.Add(function.Identifier, function);

            //Add user-defined functions
            foreach (FunctionNode function in functions)
                funcDict.Add(function.Identifier, function);

            return funcDict;
        }

        #region Functions
        private Node? InterpretUserFunction(FunctionNode functionNode, List<ArgData> interpretedArgs)
        {
            _scope.EnterFunctionScope();

            AddParametersToScope(functionNode.Parameters, interpretedArgs);

            // TODO: Add return checking to functions in semantic analysis
            var bodyExitData = InterpretBlock(functionNode.Block);

            _scope.ExitFunctionScope();

            if (functionNode.IsVoid)
                return null;

            // If this function returns data coerce the data from the return statement to the function's type
            return _coercionInterpreter.CoerceNodeValue(bodyExitData.ReturnData, functionNode.ReturnType!);
        }

        private Node? InterpretBuiltInFunction(
            BuiltInFunctionNode builtInNode, List<ArgData> interpretedArgs)
        {
            var returnValue = builtInNode.Call(interpretedArgs);
            return returnValue != null ? returnValue : null;
        }

        /// <summary> Evaluates parameter nodes and adds parameters to scope. </summary>
        private void AddParametersToScope(List<Node> parameterDeclarations, List<ArgData> args)
        {
            for (int i = 0; i < parameterDeclarations.Count; i++)
            {
                var paramNode = (ParameterNode)parameterDeclarations[i];
                var identifier = GetIdentifier(paramNode.IdentifierNode);
                Node validatedValueNode;

                /* Value type arguments are coerced to match the type of the parameter while referenced 
                 * variable pointers must maintain their type and be the same as the parameter type */
                if (!args[i].isRef)
                    validatedValueNode = _coercionInterpreter.CoerceNodeValue(args[i].source, paramNode.DataType);
                else
                    validatedValueNode = args[i].source; // TODO: Add reference var type checking to semantic analysis

                _scope.AddVariable(identifier, validatedValueNode, paramNode.DataType);
            }
        }
        #endregion

        #region Statements
        /// <summary> Interprets a single statement node and returns any exit data if applicable. </summary>
        private BlockExitData? InterpretStatement(StatementNode node)
        {
            switch (node)
            {
                case VariableDeclarationNode varNode: 
                    InterpretVariableDeclaration(varNode); break;
                case FunctionCallNode functionCallNode: 
                    InterpretFunctionCall(functionCallNode); break;
                case AssignmentNode assignmentNode: 
                    AssignValueToVariable(assignmentNode); break;
                case AssertionNode assertionNode: 
                    InterpretAssertion(assertionNode); break;
                case IfNode ifNode: 
                    return InterpretIfStructure(ifNode);
                case WhileNode whileNode: 
                    return InterpretWhileStructure(whileNode);
                case ReturnNode returnNode: 
                    return InterpretReturnStatement(returnNode); 
            }

            return null; // Only a single non-return statement was executed, so no exit data.
        }

        #region Function Statements
        /// <summary> Interprets function call by interpreting arguments, parameters, and the function body. </summary>
        private Node? InterpretFunctionCall(FunctionCallNode functionCallNode)
        {
            // Evaluates argument expressions (no coercion) and finds pointers to variable references 
            var interpretedArgs = InterpretArguments(functionCallNode.Arguments);

            //TODO: Handle function not found exception.
            var callableNode = _callableFunctionsDict[functionCallNode.Identifier];

            if (callableNode is FunctionNode)
                return InterpretUserFunction((FunctionNode)callableNode, interpretedArgs);

            return InterpretBuiltInFunction((BuiltInFunctionNode)callableNode, interpretedArgs);
        }

        private List<ArgData> InterpretArguments(List<ArgData> rawArguments)
        {
            List<ArgData> evaluatedArguments = new();

            foreach (var arg in rawArguments)
            {
                var rawArgNode = arg.source;
                var isRefArg = arg.isRef;

                // Lexeme type rawArguments are just evaluated into a ValueNode
                if (!isRefArg)
                {
                    var evaluatedValueArg = EvaluateExpression(rawArgNode);

                    // source = evaluatedValueArg & isRef = isRefArg
                    evaluatedArguments.Add((evaluatedValueArg, isRefArg));
                    continue;
                }

                // Assumes this is an identifier node
                var variableIdentifier = GetIdentifier(rawArgNode);

                // Pointer will maintain ValueNode type, because coerced values are new instances of ValueNode.
                var variablePointer = _scope.GetVariablePointer(variableIdentifier);

                // source = variablePointer & isRef = isRefArg
                evaluatedArguments.Add((variablePointer, isRefArg));
            }

            return evaluatedArguments;
        }

        /// <summary>Handles return statement and sets the last returned value.</summary>
        private BlockExitData InterpretReturnStatement(ReturnNode returnNode)
        {
            Node? returnData = null;
            if(returnNode.Expression != null)
                returnData = EvaluateExpression(returnNode.Expression);

            return new BlockExitData(BlockExitType.ReturnStatement, returnData);
        }
        #endregion

        #region Control Structures
        /// <summary>Evaluates a while loop, executing its block while the condition is true.</summary>
        /// <returns>True if the loop executed a return statement while interpreting its block.</returns>
        private BlockExitData InterpretWhileStructure(WhileNode whileNode)
        {
            while (_coercionInterpreter.CoerceCondition(EvaluateExpression(whileNode.Condition)))
            {
                var exitData = InterpretBlock((BlockNode)whileNode.Block);

                if (exitData.ExitType != BlockExitType.StatementsExhausted)
                    return exitData;
            }

            return new BlockExitData(BlockExitType.StatementsExhausted);
        }

        /// <summary>Evaluates an if statement, executing its block if the condition is true, and any else-if or else blocks as appropriate.</summary>
        /// <returns>True if the if structure executed a return statement while interpreting its block.</returns>
        private BlockExitData InterpretIfStructure(IfNode ifNode)
        {
            var evaluatedCondition = EvaluateExpression(ifNode.Condition);

            if (_coercionInterpreter.CoerceCondition(evaluatedCondition))
                return InterpretBlock((BlockNode)ifNode.Block);
            else if (ifNode.ElseIfNode != null)
                return InterpretIfStructure(ifNode.ElseIfNode);

            return new BlockExitData(BlockExitType.StatementsExhausted);
        }
        #endregion

        #region Variable Statements
        /// <summary>Declares a new variable in the current scope.</summary>
        private void InterpretVariableDeclaration(VariableDeclarationNode declarationNode)
        {          
            var dataType = declarationNode.DataType;
            var identifier = declarationNode.Identifier;

            // Variable declarations can be assigned any expression type during parsing.
            var initVal = EvaluateExpression(declarationNode.AssignedExpression);

            Node coercedInitVal;

            // Coerce the value to be the same type as the variable's data type.
            try
            {
                coercedInitVal = _coercionInterpreter.CoerceNodeValue(initVal, dataType);
            }
            catch
            {
                throw new InvalidDataException(
                    $"Variable \"{identifier}\" cannot be initialized with a value of type {initVal.GetType().Name}.\n" +
                    $"Expected type: {dataType.Name}.");
            }

            /* Even if there was no explicit initialization, the parser will still store a value 
             * node with a default value in the declaration node. Works like C# primitives. */
            _scope.AddVariable(identifier, coercedInitVal, dataType);
        }

        private void AssignValueToVariable(AssignmentNode assignment)
        {
            // The expression can be of any ValueNode type, even after evaluation
            var rawAssignVal = EvaluateExpression(assignment.AssignedExpression);

            var identifier = assignment.IdentifierNode.ToString();
            var varDataType = _scope.GetVariableDataType(identifier);

            // Coerce the evaluated expression to the variable's data type
            var coercedAssignVal = _coercionInterpreter.CoerceNodeValue(rawAssignVal, varDataType);

            if (coercedAssignVal == null)
                throw new InvalidDataException(
                    $"Variable \"{assignment.IdentifierNode}\" cannot be assigned a value of type {rawAssignVal.GetType().Name}.\n" +
                    $"Expected type: {varDataType.Name}.");

            _scope.AssignValueToVariable(identifier, coercedAssignVal);
        }
        #endregion

        #region Misc. Statements
        /// <summary>Evaluates an assertionNode statement and ensures the condition associated is true.</summary>
        /// <exception cref="AssertionException"> Thrown if the conditional expression stored in the statement is false. </exception>
        private void InterpretAssertion(AssertionNode assertionNode)
        {
            try
            {
                var interpretedExpression = EvaluateExpression(assertionNode.Condition);
                if (!_coercionInterpreter.CoerceLogicOperand(interpretedExpression, out _))
                    throw new AssertionException("Expression evaluated to false");        
            }
            catch(Exception ex)
            {
                throw new AssertionException($"{assertionNode.ToString()}\nException was thrown while executing assertion: {ex.Message}");
            }
        }
        #endregion
        #endregion

        #region Expressions
        /// <summary>Evaluates an expression node and returns its computed ValueNode.</summary>
        /// <returns>The result of the evaluated expression as a <see cref="ValueNode"/>.</returns>
        private Node EvaluateExpression(Node node)
        {
            if (node is LogicExpressionNode logicNode)
                return EvaluateLogicExpression(logicNode);

            if(node is ComparisonExpressionNode comparisonNode)
                return EvaluateComparisonExpression(comparisonNode);

            if (node is MathExpressionNode mathExpr)
                return EvaluateMathExpression(mathExpr);
            
            return EvaluateFactor(node);
        }

        private Node EvaluateMathExpression(MathExpressionNode expression)
        {
            var left = EvaluateExpression(expression.LeftOperand);
            var right = EvaluateExpression(expression.RightOperand);

            _coercionInterpreter.CoerceOperands(left, right, out var coercedLeft, out var coercedRight);
            return _operationInterpreter.PerformMathOperation(coercedLeft, coercedRight, expression.Operator);
        }

        private Node EvaluateLogicExpression(LogicExpressionNode expression)
        {
            var left = EvaluateExpression(expression.LeftOperand);

            // Coerces node to boolean node and returns the node's stored value
            var isLeftTrue = _coercionInterpreter.CoerceLogicOperand(left, out var coercedLeft);

            if (expression.IsShortCircuit(isLeftTrue))
                return coercedLeft;

            var right = EvaluateExpression(expression.RightOperand);

            _coercionInterpreter.CoerceLogicOperand(right, out var coercedRight);
            return _operationInterpreter.PerformLogicOperation(coercedLeft, coercedRight, expression.Operator);
        }

        private Node EvaluateComparisonExpression(ComparisonExpressionNode expression)
        {
            var left = EvaluateExpression(expression.LeftOperand);
            var right = EvaluateExpression(expression.RightOperand);

            _coercionInterpreter.CoerceOperands(left, right, out var coercedLeft, out var coercedRight);
            return _operationInterpreter.PerformComparisonOperation(coercedLeft, coercedRight, expression.Operator);
        }

        /// <summary>Processes a factor node and returns its corresponding ValueNode.</summary>
        /// <returns>A <see cref="ValueNode"/> representing the factor’s computed value.</returns>
        private Node EvaluateFactor(Node node) =>
            node switch
            {      
                FunctionCallNode callNode => InterpretFunctionCall(callNode),
                IdentifierNode idNode => _scope.GetVariableValueCopy(idNode.ToString()),
                _ => node
            };
        #endregion

        #region Helper Methods
        /// <summary>Assigns a value to an existing variable in the current scope.</summary>
        private string GetIdentifier(Node identifierNode)
        {
            var nodeType = identifierNode.GetType();
            if (nodeType != typeof(IdentifierNode))
                throw new InvalidDataException($"Expected {typeof(IdentifierNode).Name} but found {nodeType.Name}");

            return ((IdentifierNode)identifierNode).Lexeme;
        }
        #endregion
    }
}
