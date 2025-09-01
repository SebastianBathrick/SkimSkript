using SkimSkript.ErrorHandling;
using SkimSkript.Interpretation.Helpers;
using SkimSkript.Nodes;
using SkimSkript.Nodes.StatementNodes;

namespace SkimSkript.MainComponents
{
    /// <summary> 
    /// Responsible for executing the program represented by an Abstract Syntax Tree (AST) generated 
    /// from the parsed source code. It handles function calls, variable management, expressions, and 
    /// control structures (e.g., if, while), and any other statement types to produce runtime results. 
    /// </summary>
    internal class Interpreter : MainComponent<AbstractSyntaxTreeRoot, int>
    {
        #region Constants
        private const int INTERP_ERROR_EXIT_CODE = -2;
        private const int SRC_CODE_ERR_EXIT_CODE = -1;
        private const int DEFAULT_EXIT_CODE = 0;
        #endregion

        #region Data Members
        private ScopeManager _scope = new();
        private Dictionary<string, CallableNode>? _callableFunctionsDict;
        private StatementNode? _currentStatementNode;
        private Node[]? _userFunctions; 
        private ValueNode? _exitCodeNode;
        #endregion

        #region Properties
        public override MainComponentType ComponentType => MainComponentType.Interpreter;

        public int SourceCodeErrorExitCode => SRC_CODE_ERR_EXIT_CODE;

        public int InterpreterErrorExitCode => INTERP_ERROR_EXIT_CODE;

        private Dictionary<string, CallableNode> CallableFunctions =>
            _callableFunctionsDict ??= GetFunctionMap(_userFunctions);

        public bool IsStringExitCode() => _exitCodeNode is StringValueNode;
        #endregion

        #region Primary Execution
        public Interpreter(IEnumerable<MainComponentType> debuggedTypes, IEnumerable<MainComponentType> verboseTypes) : base(debuggedTypes, verboseTypes) { }

        protected override int OnExecute(AbstractSyntaxTreeRoot treeRoot)
        {
            _userFunctions = treeRoot.UserFunctions;
            BlockExitData exitData;
            try
            {
                exitData = RunBlock(treeRoot);
            }
            catch (RuntimeException ex)
            {
                if (_currentStatementNode != null)
                    ex.SetStatement(_currentStatementNode);
                throw;
            }
            catch
            {
                throw; // Interpreter bug that needs to be fixed
            }

            if (exitData.ExitType == BlockExitType.StatementsExhausted)
                return DEFAULT_EXIT_CODE;

            // Assuming the exit was a top-level return statement...

            // If no data was returned, return 0 as default exit code
            if (exitData.ReturnData == null)
                return DEFAULT_EXIT_CODE;

            try
            {
                return ((ValueNode)exitData.ReturnData).ToInt();
            }
            catch
            {
                return SRC_CODE_ERR_EXIT_CODE;
            }
        }

        /// <summary> Interprets block, executes its statements, and returns info about how the block exited. </summary>
        private BlockExitData RunBlock(Node block)
        {
            var coercedBlock = (BlockNode)block;
            BlockExitData? exitData = null;

            if (coercedBlock.Statements != null)
            {
                _scope.EnterScope();
                foreach (var statement in coercedBlock.Statements!)
                {
                    exitData = RunStatement(statement);

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
        private Node? RunUserFunction(FunctionNode functionNode, Node[]? evaluatedArgs)
        {
            _scope.EnterFunctionScope();

            EvaluateParameters(functionNode.Parameters, evaluatedArgs);

            // TODO: Append return checking to functions in semantic analysis
            var bodyExitData = RunBlock(functionNode.Block);

            _scope.ExitFunctionScope();

            if (functionNode.IsVoid)
                return null;

            // If this function returns data coerce the data from the return statement to the function's type
            return CoercionInterpreter.CoerceNodeValue(bodyExitData.ReturnData, functionNode.ReturnType!);
        }

        /// <summary> Interprets built-in function call by calling the C# class method with the evaluated call args. </summary>
        private Node? RunBuiltInFunction(BuiltInFunctionNode builtInNode, Node[]? arguments)
        {
            var returnValue = builtInNode.Call(arguments);
            return returnValue != null ? returnValue : null;
        }

        /// <summary> Adds previously evaluated parameters to current scope. </summary>
        private void EvaluateParameters(Node[]? parameters, Node[]? evaluatedArgs)
        {
            if (parameters == null)
                return;

            for (int i = 0; i < parameters.Length; i++)
            {
                var identifier = EvaluateIdentifier(((ParameterNode)parameters[i]).IdentifierNode);
                _scope.AddVariable(identifier, evaluatedArgs![i], evaluatedArgs![i].GetType());
            }
        }
        #endregion

        #region Statements
        /// <summary> Interprets a single statement node and returns any exit data if applicable. </summary>
        private BlockExitData? RunStatement(Node node)
        {
            _currentStatementNode = (StatementNode)node;

            switch (node)
            {
                case VariableDeclarationNode varNode:
                    RunVariableDeclaration(varNode); break;
                case FunctionCallNode functionCallNode:
                    RunFunctionCall(functionCallNode); break;
                case AssignmentNode assignmentNode:
                    AssignVariableValue(assignmentNode); break;
                case AssertionNode assertionNode:
                    RunAssertion(assertionNode); break;
                case IfNode ifNode:
                    return RunIfStructure(ifNode);
                case WhileNode whileNode:
                    return RunWhileStructure(whileNode);
                case ReturnNode returnNode:
                    return InterpretReturnStatement(returnNode);
                case RepeatNode repeatNode:
                    return RunRepeatLoop(repeatNode);
                case TryCatchNode tryCatchNode:
                    return RunTryCatch(tryCatchNode);
            }

            return null; // Only a single non-return statement was executed, so no exit data.
        }

        #region Function Statements
        /// <summary> Interprets function call by interpreting args, parameters, and the function body. </summary>
        private Node? RunFunctionCall(FunctionCallNode functionCallNode)
        {
            var identifier = EvaluateIdentifier(functionCallNode.IdentifierNode);

            //TODO: Handle function not found exception.
            var callableNode = CallableFunctions[identifier];
            var parameters = callableNode.Parameters;
            var args = functionCallNode.Arguments;
            
            Node[]? evaluatedArgs = null;

            var isVariadic = callableNode.IsVariadic;

            // For each pass-by-value arg evaluate the expression and coerce it to the paramNode's data type.
            evaluatedArgs = GetEvaluatedArguments(identifier, args, parameters, isVariadic);


            if (callableNode is FunctionNode userFunction)
                return RunUserFunction(userFunction, evaluatedArgs);
            else
                return RunBuiltInFunction((BuiltInFunctionNode)callableNode, evaluatedArgs);
        }

        /// <summary> 
        /// Returns array containing coerced and evaluated pass-by-value arguments and pointers to variables
        /// in pass-byrerence arguments.
        /// </summary>
        private Node[]? GetEvaluatedArguments(
            string functionIdentifier, Node[] args, Node[] parameters, bool isVariadic)
        {
            int argCount = args != null ? args.Length : 0;
            int parameterCount = parameters != null ? parameters.Length : 0;

            // If function has fixed paramNode count but the argument count doesn't match
            if (!isVariadic && parameterCount != argCount)
                throw new RuntimeException(
                    "Call to function {Function} used invalid number of arguments", functionIdentifier);

            // If no arguments were sent and none are required
            if (argCount == 0)
                return null;

            var evaluatedArgs = new Node[args!.Length];

            for (int i = 0; i < evaluatedArgs.Length; i++)
            {
                var argNode = (ArgumentNode)args[i];

                if(i >= parameterCount)
                {
                    evaluatedArgs[i] = EvaluateExpression(argNode.Value);
                    continue;
                }

                var paramNode = (ParameterNode)(parameters![i]);

                if (argNode.IsReference != paramNode.IsReference)
                    throw new RuntimeException(
                        "Call to function {Function} used invalid argument pass-by type", functionIdentifier);

                if (!argNode.IsReference)
                {
                    var evalArg = EvaluateExpression(argNode.Value);
                    evaluatedArgs[i] = CoercionInterpreter.CoerceNodeValue(evalArg, paramNode.DataType);
                    continue;
                }

                var varPointer = (ValueNode)_scope.GetVariablePointer(EvaluateIdentifier(argNode.Value));

                if (varPointer.GetType() != paramNode.DataType)
                    throw new RuntimeException(
                        "Call to function {Function} used invalid data type for reference argument", functionIdentifier);

                evaluatedArgs[i] = varPointer;
            }

            return evaluatedArgs;
        }



        /// <summary>Handles return statement and sets the last returned value.</summary>
        private BlockExitData InterpretReturnStatement(ReturnNode returnNode)
        {
            Node? returnData = null;
            if (returnNode.Expression != null)
                returnData = EvaluateExpression(returnNode.Expression);

            return new BlockExitData(BlockExitType.ReturnStatement, returnData);
        }
        #endregion

        #region Control Structures
        private BlockExitData RunWhileStructure(WhileNode whileNode)
        {


            while (CoercionInterpreter.CoerceCondition(EvaluateExpression(whileNode.Condition)))
            {
                var exitData = RunBlock(whileNode.Block);

                if (exitData.ExitType != BlockExitType.StatementsExhausted)
                    return exitData;
            }

            return new BlockExitData(BlockExitType.StatementsExhausted);
        }

        private BlockExitData RunIfStructure(IfNode ifNode)
        {
            var evaluatedCondition = EvaluateExpression(ifNode.Condition);

            if (CoercionInterpreter.CoerceCondition(evaluatedCondition))
                return RunBlock(ifNode.Block);
            else if (ifNode.ChainedStructure != null)
                if (ifNode.ChainedStructure is ElseIfNode elseIfNode)
                    return RunElseIfStructure(elseIfNode);
                else
                    return RunElseStructure((ElseNode)ifNode.ChainedStructure);

            return new BlockExitData(BlockExitType.StatementsExhausted);
        }

        private BlockExitData RunElseIfStructure(ElseIfNode elseIfNode)
        {
            var evaluatedCondition = EvaluateExpression(elseIfNode.Condition);

            if (CoercionInterpreter.CoerceCondition(evaluatedCondition))
                return RunBlock(elseIfNode.Block);
            else if (elseIfNode.ChainedStructure != null)
                if (elseIfNode.ChainedStructure is ElseIfNode elseIfNodeChained)
                    return RunElseIfStructure(elseIfNodeChained);
                else
                    return RunElseStructure((ElseNode)elseIfNode.ChainedStructure);

            return new BlockExitData(BlockExitType.StatementsExhausted);
        }

        private BlockExitData RunElseStructure(ElseNode elseNode) =>
            RunBlock(elseNode.Block);

        private BlockExitData RunRepeatLoop(RepeatNode repeatNode)
        {
            var evaluatedCondition = (ValueNode)EvaluateExpression(repeatNode.Condition);
            var targetCount = evaluatedCondition.ToInt();
            int iterations = 0;

            while (iterations++ < targetCount)
            {
                var exitData = RunBlock(repeatNode.Block);

                if (exitData.ExitType != BlockExitType.StatementsExhausted)
                    return exitData;

                targetCount = ((ValueNode)EvaluateExpression(repeatNode.Condition)).ToInt();
            }

            return new BlockExitData(BlockExitType.StatementsExhausted);
        }
        #endregion

        #region Variable Statements
        /// <summary> Declares a new variable in the current scope. </summary>
        private void RunVariableDeclaration(VariableDeclarationNode declarationNode)
        {
            var dataType = declarationNode.DataType;
            var identifier = EvaluateIdentifier(declarationNode.IdentifierNode);

            // Variable declarations can be assigned any expression type during parsing.
            var initVal = EvaluateExpression(declarationNode.AssignedExpression);

            Node coercedInitVal;

            // Coerce the value to be the same type as the variable's data type.
            coercedInitVal = CoercionInterpreter.CoerceNodeValue(initVal, dataType);

            /* Even if there was no explicit initialization, the parser will still store a value 
             * node with a default value in the declaration node. Works like C# primitives. */
            _scope.AddVariable(identifier, coercedInitVal, dataType);
        }

        private void AssignVariableValue(AssignmentNode assignment)
        {
            // The expression can be of any ValueNode type, even after evaluation
            var rawAssignVal = EvaluateExpression(assignment.AssignedExpression);

            var identifier = assignment.IdentifierNode.ToString();
            var varDataType = _scope.GetVariableDataType(identifier);

            // Coerce the evaluated expression to the variable's data type
            var coercedAssignVal = CoercionInterpreter.CoerceNodeValue(rawAssignVal, varDataType);

            _scope.AssignValueToVariable(identifier, coercedAssignVal);
        }
        #endregion

        #region Monitoring Statements
        /// <summary>Evaluates an assertionNode statement and ensures the condition associated is true.</summary>
        /// <exception cref="AssertionException"> Thrown if the conditional expression stored in the statement is false. </exception>
        private void RunAssertion(AssertionNode assertionNode)
        {
            var interpretedExpression = EvaluateExpression(assertionNode.Condition);
            var evaluatedCondition = CoercionInterpreter.CoerceLogicOperand(interpretedExpression, out _);

            if (!evaluatedCondition)
            {
                Node leftOperand;
                Node rightOperand;

                if (assertionNode.Condition is LogicExpressionNode logicExpr)
                {
                    leftOperand = logicExpr.LeftOperand;
                    rightOperand = logicExpr.RightOperand;
                }
                else if (assertionNode.Condition is ComparisonExpressionNode comparisonExpr)
                {
                    leftOperand = comparisonExpr.LeftOperand;
                    rightOperand = comparisonExpr.RightOperand;
                }
                else if (assertionNode.Condition is MathExpressionNode mathExpr)
                {
                    leftOperand = assertionNode.Condition;
                    rightOperand = new BoolValueNode(false);
                }
                else
                    throw new RuntimeException("Assertion failed", assertionNode);

                var leftEval = EvaluateExpression(leftOperand);
                var rightEval = EvaluateExpression(rightOperand);

                throw new RuntimeException(
                    "Assertion failed. Evaluated left operand: {Left} Evaluated right operand: {Right}",
                    assertionNode, leftEval, rightEval
                    );
            }
        }

        private BlockExitData RunTryCatch(TryCatchNode tryCatchNode)
        {
            try
            {
                return RunBlock(tryCatchNode.TryBlock);
            }
            catch(Exception e)
            {
                if(tryCatchNode.CatchMessage !=  null)
                {
                    var msgVar = (VariableDeclarationNode)tryCatchNode.CatchMessage;

                    RunVariableDeclaration(msgVar);
                    StringValueNode pointer = (StringValueNode)_scope.GetVariablePointer(msgVar.IdentifierNode.ToString());

                    pointer.AssignValue(new StringValueNode(e.Message));
                }
                    

                return RunBlock(tryCatchNode.CatchBlock);
            }
        }
        #endregion
        #endregion

        #region Expressions
        /// <summary> 
        /// Evaluates math, comparison, or logic expression.
        /// </summary>
        private Node EvaluateExpression(Node node)
        {
            if (node is LogicExpressionNode logicNode)
                return EvaluateLogicExpression(logicNode);

            if (node is ComparisonExpressionNode comparisonNode)
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

        /// <summary>
        /// Processes a factor node and returns its corresponding ValueNode.
        /// </summary>
        /// <returns>A <see cref="ValueNode"/> representing the factor’s final value.</returns>
        private Node? EvaluateFactor(Node node) =>
            node switch
            {
                FunctionCallNode callNode => RunFunctionCall(callNode),
                IdentifierNode idNode => _scope.GetVariableValueCopy(idNode.ToString()),
                _ => node
            };
        #endregion

        #region Helper Methods
        /// <summary>Assigns a value to an existing variable in the current scope.</summary>
        private string EvaluateIdentifier(Node identifierNode)
        {
            var nodeType = identifierNode.GetType();
            return ((IdentifierNode)identifierNode).Lexeme;
        }

        /// <summary> Returns dictionary of built-in & user-defined functions mapped to their idenifiers. </summary>
        private Dictionary<string, CallableNode> GetFunctionMap(Node[]? userDefinedFunctions)
        {
            var funcDict = new Dictionary<string, CallableNode>();

            foreach (BuiltInFunctionNode function in BuiltInFunctionNode.GetFunctionInstances())
                funcDict.Add(EvaluateIdentifier(function.IdentfierNode), function);

            if (userDefinedFunctions != null)
                foreach (FunctionNode function in userDefinedFunctions)
                    funcDict.Add(EvaluateIdentifier(function.IdentfierNode), function);

            return funcDict;
        }
        #endregion
    }
}
