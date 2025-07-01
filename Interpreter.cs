using SkimSkript.ErrorHandling;
using SkimSkript.Interpretation.Helpers;
using SkimSkript.Nodes;

namespace SkimSkript.Interpretation
{
    /// <summary> 
    /// Responsible for executing the program represented by an Abstract Syntax Tree (AST) generated 
    /// from the parsed source code. It handles function calls, variable management, expressions, and 
    /// control structures (e.g., if, while) to produce runtime results. 
    /// </summary>
    public class Interpreter
    {
        #region Data Members
        private ScopeManager _scope = new();
        private CoercionInterpreter? _coercionInterpreter;
        private OperationInterpreter? _operationInterpreter;
        private Dictionary<string, CallableNode>? _callableFunctionsDict;
        private Node[]? _userFunctions;
        #endregion

        #region Properties
        private CoercionInterpreter CoercionInterpreter =>
            _coercionInterpreter ??= new CoercionInterpreter();

        private OperationInterpreter OperationInterpreter =>
            _operationInterpreter ??= new OperationInterpreter();

        private Dictionary<string, CallableNode> CallableFunctions =>
            _callableFunctionsDict ??= GetFunctionMap(_userFunctions);
        #endregion

        #region Execution
        public void Execute(AbstractSyntaxTreeRoot treeRoot)
        {
            _userFunctions = treeRoot.UserFunctions;
            InterpretBlock(treeRoot);
        }

        /// <summary> Interprets block, executes its statements, and returns info about how the block exited. </summary>
        private BlockExitData InterpretBlock(Node block)
        {
            var coercedBlock = (BlockNode)block;
            BlockExitData? exitData = null;

            if(coercedBlock.Statements != null)
            {
                _scope.EnterScope();
                foreach (var statement in coercedBlock.Statements!)
                {
                    exitData = InterpretStatement(statement);

                    // If the block is forcefully being exited
                    if (exitData != null && exitData.ExitType != BlockExitType.StatementsExhausted)
                        break;
                }
                _scope.ExitScope();
            }    
          
            return exitData ?? new BlockExitData(BlockExitType.StatementsExhausted);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Interprets user-defined function call by entering function's scope, initializing its parameters
        /// with provided evaluated args, interpreting it's block, and returning any resulting data from the block's exit.
        /// </summary>
        private Node? InterpretUserFunction(FunctionNode functionNode, Node[]? evaluatedArgs)
        {
            _scope.EnterFunctionScope();

            InitializeParameters(functionNode.Parameters, evaluatedArgs);

            // TODO: Append return checking to functions in semantic analysis
            var bodyExitData = InterpretBlock(functionNode.Block);

            _scope.ExitFunctionScope();

            if (functionNode.IsVoid)
                return null;

            // If this function returns data coerce the data from the return statement to the function's type
            return CoercionInterpreter.CoerceNodeValue(bodyExitData.ReturnData, functionNode.ReturnType!);
        }

        /// <summary> Interprets built-in function call by calling the C# class method with the evaluated call args. </summary>
        private Node? InterpretBuiltInFunction(BuiltInFunctionNode builtInNode, Node[]? arguments)
        {
            var returnValue = builtInNode.Call(arguments);
            return returnValue != null ? returnValue : null;
        }

        /// <summary> Adds previously evaluated parameters to current scope. </summary>
        private void InitializeParameters(Node[]? parameters, Node[]? evaluatedArgs)
        {
            if (parameters == null)
                return;

            for(int i = 0; i < parameters.Length; i++)
            {
                var identifier = GetIdentifier(((ParameterNode)parameters[i]).IdentifierNode);
                _scope.AddVariable(identifier, evaluatedArgs![i], evaluatedArgs![i].GetType());
            }
        }
        #endregion

        #region Statements
        /// <summary> Interprets a single statement node and returns any exit data if applicable. </summary>
        private BlockExitData? InterpretStatement(Node node)
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
        /// <summary> Interprets function call by interpreting args, parameters, and the function body. </summary>
        private Node? InterpretFunctionCall(FunctionCallNode functionCallNode)
        {
            var identifier = GetIdentifier(functionCallNode.IdentifierNode);

            //TODO: Handle function not found exception.
            var callableNode = CallableFunctions[identifier];
            var args = functionCallNode.Arguments;
            var parameters = callableNode.Parameters;
            Node[]? evaluatedArgs = null;

            if(parameters != null || args != null)
            {
                var isVariadic = callableNode.IsVariadic;

                // Throw an exception if the argument to parameter ratio is invalid.
                ValidateArgCount(args, parameters, isVariadic);

                // For each pass-by-value arg evaluate the expression and coerce it to the parameter's data type.
                evaluatedArgs = GetEvaluatedArguments(identifier, args, parameters, isVariadic);
            }


            if (callableNode is FunctionNode userFunction)
                return InterpretUserFunction(userFunction, evaluatedArgs);
            else
                return InterpretBuiltInFunction((BuiltInFunctionNode)callableNode, evaluatedArgs);
        }

        /// <summary> 
        /// Returns array containing coerced and evaluated pass-by-value arguments and pointers to variables
        /// in pass-byrerence arguments.
        /// </summary>
        private Node[]? GetEvaluatedArguments(
            string functionIdentifier, Node[]? args, Node[]? parameters, bool isVariadic)
        {
            var evaluatedArgs = new Node[args!.Length];

            for(int i = 0; i < evaluatedArgs.Length; i++)
            {
                var rawArg = (ArgumentNode)args[i];

                // If reference get the variable pointer, otherwise evaluate the expression.
                if (rawArg.IsReference)
                    evaluatedArgs[i] = _scope.GetVariablePointer(GetIdentifier(rawArg.Value));
                else
                    evaluatedArgs[i] = EvaluateExpression(rawArg.Value);

                if (parameters == null)
                    continue;

                var parameter = (ParameterNode)parameters[i];

                if (parameter.IsReference != rawArg.IsReference)
                    throw new InterpreterError(RuntimeErrorCode.ArgumentPassTypeMismatch);

                if (parameter.DataType != evaluatedArgs[i].GetType())
                    throw new InterpreterError(RuntimeErrorCode.ArgumentDataTypeMismatch);

                // If pass-by-value coerce the evaluated argument to the parameter's data type.
                if (!rawArg.IsReference)
                    evaluatedArgs[i] = CoercionInterpreter.CoerceNodeValue(
                        evaluatedArgs[i], parameter.DataType);
            }

            return evaluatedArgs;
        }

        /// <summary>Handles return statement and sets the last returned value.</summary>
        private BlockExitData InterpretReturnStatement(ReturnNode returnNode)
        {
            Node? returnData = null;
            if(returnNode.Expression != null)
                returnData = EvaluateExpression(returnNode.Expression);

            return new BlockExitData(BlockExitType.ReturnStatement, returnData);
        }

        /// <summary> Validates the argument count against the parameters of the function being called. </summary>
        private void ValidateArgCount(Node[]? args, Node[]? parameters, bool isVariadic)
        {
            if (!isVariadic)
            {
                var isInvalidArgCount = args == null && parameters != null || args != null && parameters == null;
                isInvalidArgCount = isInvalidArgCount || args!.Length != parameters!.Length;

                if (isInvalidArgCount)
                {
                    var argCount = args == null ? 0 : args.Length;
                    var paramCount = parameters == null ? 0 : parameters.Length;
                    throw new InterpreterError(RuntimeErrorCode.ArgumentInvalidCount, argCount, paramCount);
                }
            }
        }
        #endregion

        #region Control Structures
        private BlockExitData InterpretWhileStructure(WhileNode whileNode)
        {
            while (CoercionInterpreter.CoerceCondition(EvaluateExpression(whileNode.Condition)))
            {
                var exitData = InterpretBlock(whileNode.Block);

                if (exitData.ExitType != BlockExitType.StatementsExhausted)
                    return exitData;
            }

            return new BlockExitData(BlockExitType.StatementsExhausted);
        }

        private BlockExitData InterpretIfStructure(IfNode ifNode)
        {
            var evaluatedCondition = EvaluateExpression(ifNode.Condition);

            if (CoercionInterpreter.CoerceCondition(evaluatedCondition))
                return InterpretBlock(ifNode.Block);
            else if (ifNode.ChainedStructure != null)
                if (ifNode.ChainedStructure is ElseIfNode elseIfNode)
                    return InterpretElseIfStructure(elseIfNode);
                else
                    return InterpretElseStructure((ElseNode)ifNode.ChainedStructure);

            return new BlockExitData(BlockExitType.StatementsExhausted);
        }

        private BlockExitData InterpretElseIfStructure(ElseIfNode elseIfNode)
        {
            var evaluatedCondition = EvaluateExpression(elseIfNode.Condition);

            if (CoercionInterpreter.CoerceCondition(evaluatedCondition))
                return InterpretBlock(elseIfNode.Block);
            else if (elseIfNode.ChainedStructure != null)
                if (elseIfNode.ChainedStructure is ElseIfNode elseIfNodeChained)
                    return InterpretElseIfStructure(elseIfNodeChained);
                else
                    return InterpretElseStructure((ElseNode)elseIfNode.ChainedStructure);

            return new BlockExitData(BlockExitType.StatementsExhausted);
        }

        private BlockExitData InterpretElseStructure(ElseNode elseNode) =>
            InterpretBlock(elseNode.Block);
        #endregion

        #region Variable Statements
        /// <summary> Declares a new variable in the current scope. </summary>
        private void InterpretVariableDeclaration(VariableDeclarationNode declarationNode)
        {          
            var dataType = declarationNode.DataType;
            var identifier = GetIdentifier(declarationNode.IdentifierNode);

            // Variable declarations can be assigned any expression type during parsing.
            var initVal = EvaluateExpression(declarationNode.AssignedExpression);

            Node coercedInitVal;

            // Coerce the value to be the same type as the variable's data type.
            try
            {
                coercedInitVal = CoercionInterpreter.CoerceNodeValue(initVal, dataType);
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
            var coercedAssignVal = CoercionInterpreter.CoerceNodeValue(rawAssignVal, varDataType);

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
                if (!CoercionInterpreter.CoerceLogicOperand(interpretedExpression, out _))
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
        /// <summary> Evaluates math, comparison, or logic expression.</summary>
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

            CoercionInterpreter.CoerceOperands(left, right, out var coercedLeft, out var coercedRight);
            return OperationInterpreter.PerformMathOperation(coercedLeft, coercedRight, expression.Operator);
        }

        private Node EvaluateLogicExpression(LogicExpressionNode expression)
        {
            var left = EvaluateExpression(expression.LeftOperand);

            // Coerces node to boolean node and returns the node's stored value
            var isLeftTrue = CoercionInterpreter.CoerceLogicOperand(left, out var coercedLeft);

            if (expression.IsShortCircuit(isLeftTrue))
                return coercedLeft;

            var right = EvaluateExpression(expression.RightOperand);

            CoercionInterpreter.CoerceLogicOperand(right, out var coercedRight);
            return OperationInterpreter.PerformLogicOperation(coercedLeft, coercedRight, expression.Operator);
        }

        private Node EvaluateComparisonExpression(ComparisonExpressionNode expression)
        {
            var left = EvaluateExpression(expression.LeftOperand);
            var right = EvaluateExpression(expression.RightOperand);

            CoercionInterpreter.CoerceOperands(left, right, out var coercedLeft, out var coercedRight);
            return OperationInterpreter.PerformComparisonOperation(coercedLeft, coercedRight, expression.Operator);
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

        /// <summary> Returns dictionary of built-in & user-defined functions mapped to their idenifiers. </summary>
        private Dictionary<string, CallableNode> GetFunctionMap(Node[]? userDefinedFunctions)
        {
            var funcDict = new Dictionary<string, CallableNode>();

            foreach (BuiltInFunctionNode function in BuiltInFunctionNode.GetFunctionInstances())
                funcDict.Add(GetIdentifier(function.IdentfierNode), function);

            if (userDefinedFunctions != null)
                foreach (FunctionNode function in userDefinedFunctions)
                    funcDict.Add(GetIdentifier(function.IdentfierNode), function);

            return funcDict;
        }
        #endregion
    }
}
