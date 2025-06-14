using SkimSkript.Interpretation.Helpers;
using SkimSkript.Nodes.ValueNodes;
using SkimSkript.Nodes.Runtime;
using SkimSkript.Nodes;
using SkimSkript.Nodes.StatementNodes;
using SkimSkript.Nodes.NodeExceptions;
using SkimSkript.Core.Helpers.Interpreter;

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
        private ValueNode? InterpretFunctionCall(FunctionCallNode functionCallNode)
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
            ValueNode? returnData = null;
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
            while (EvaluateExpression(whileNode.Condition).ToBool())
            {
                var exitData = InterpretBlock((BlockNode)whileNode.Block);

                if (exitData.ExitType != BlockExitType.StatementsExhausted)
                    return exitData;
            }

            // Upon reaching the termination condition
            return new BlockExitData(BlockExitType.StatementsExhausted);
        }

        /// <summary>Evaluates an if statement, executing its block if the condition is true, and any else-if or else blocks as appropriate.</summary>
        /// <returns>True if the if structure executed a return statement while interpreting its block.</returns>
        private BlockExitData InterpretIfStructure(IfNode ifNode)
        {
            if (EvaluateExpression(ifNode.Condition).ToBool())
                return InterpretBlock((BlockNode)ifNode.Block);
            else if (ifNode.ElseIfNode != null)
                return InterpretIfStructure(ifNode.ElseIfNode);

            // If the condition evaluates to false
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

            ValueNode coercedInitVal;

            // Coerce the value to be the same type as the variable's data type.
            try
            {
                coercedInitVal = CoerceValue(initVal, dataType);
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
            var coercedAssignVal = CoerceValue(rawAssignVal, varDataType);

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
                var origExpression = (ConditionExpressionNode)assertionNode.Condition;
                var interpretedExpression = EvaluateExpression(origExpression);

                if (!interpretedExpression.ToBool())
                {
                    var rightEval = EvaluateExpression(origExpression.RightOperand);
                    var leftEval = EvaluateExpression(origExpression.LeftOperand);

                    throw new AssertionException(
                        $"\nLeft: {origExpression.LeftOperand} yielded {leftEval}\n" +
                        $"Right: {origExpression.RightOperand} yielded {rightEval}\n");
                }
                    
            }
            catch(Exception ex)
            {
                throw new AssertionException($"{assertionNode.ToString()}\nException was thrown while executing assertion: {ex.Message}");
            }
        }
        #endregion
        #endregion

        #region Functions
        private ValueNode? InterpretUserFunction(FunctionNode functionNode, List<ArgData> interpretedArgs)
        {
            _scope.EnterFunctionScope();

            AddParametersToScope(functionNode.Parameters, interpretedArgs);

            // TODO: Add return checking to functions in semantic analysis
            var bodyExitData = InterpretBlock(functionNode.Block);

            _scope.ExitFunctionScope();

            ValueNode? returnVal = null;

            // If this function returns data coerce the data from the return statement to the function's type
            if (bodyExitData.IsReturnData)
                returnVal = CoerceValue(bodyExitData.ReturnData, functionNode.ReturnType!);

            return returnVal;
        }

        private ValueNode? InterpretBuiltInFunction(
            BuiltInFunctionNode builtInNode, List<ArgData> interpretedArgs)
        {
            var returnValue = builtInNode.Call(interpretedArgs);
            return returnValue != null ? (ValueNode)returnValue : null;
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
                    validatedValueNode = CoerceValue((ValueNode)args[i].source, paramNode.DataType);
                else
                    validatedValueNode = args[i].source; // TODO: Add reference var type checking to semantic analysis

                _scope.AddVariable(identifier, validatedValueNode, paramNode.DataType);
            }
        }
        #endregion

        #region Expressions
        /// <summary>Evaluates an expression node and returns its computed ValueNode.</summary>
        /// <returns>The result of the evaluated expression as a <see cref="ValueNode"/>.</returns>
        private ValueNode EvaluateExpression(Node node)
        {
            if (node is ConditionExpressionNode conditionExpr)
                return EvaluateConditionExpression(conditionExpr);

            if (node is MathExpressionNode mathExpr)
                return EvaluateMathExpression(mathExpr);
            
            return EvaluateFactor(node);
        }

        private ValueNode EvaluateMathExpression(MathExpressionNode expression)
        {
            ValueNode left = EvaluateExpression(expression.LeftOperand);
            ValueNode right = EvaluateExpression(expression.RightOperand);

            // If a conditional is inside an expression, convert to int nodes if necessary
            if (left is BoolValueNode)
                left = new IntValueNode(left.ToInt());

            if (right is BoolValueNode)
                right = new IntValueNode(right.ToInt());

            return OperationUtilities.PerformMathOperation(left, right, expression.MathOperator); ;
        }

        private ValueNode EvaluateConditionExpression(ConditionExpressionNode expression)
        {
            ValueNode left, right;
            bool result;

            if (expression.IsLogical)
            {
                left = new BoolValueNode(EvaluateExpression(expression.LeftOperand).ToBool());

                // TODO: Add short circuiting for and operator
                if (expression.LogicalOperator == LogicalOperator.Or && left.ToBool()) 
                    return new BoolValueNode(true); //Short circuit.

                right = new BoolValueNode(EvaluateExpression(expression.RightOperand).ToBool());
                result = OperationUtilities.PerformLogicalOperation(left.ToBool(), right.ToBool(), expression.LogicalOperator);
                return new BoolValueNode(result);
            }

            left = EvaluateExpression(expression.LeftOperand);
            right = EvaluateExpression(expression.RightOperand);
            result = OperationUtilities.PerformComparisonOperation(left, right, expression.ComparisonOperator);

            return new BoolValueNode(result);
        }

        /// <summary>Processes a factor node and returns its corresponding ValueNode.</summary>
        /// <returns>A <see cref="ValueNode"/> representing the factor’s computed value.</returns>
        private ValueNode EvaluateFactor(Node node) =>
            node switch
            {
                IntValueNode val => new IntValueNode(val.ToInt()),
                BoolValueNode val => new BoolValueNode(val.ToBool()),
                FloatValueNode val => new FloatValueNode(val.ToFloat()),                
                StringValueNode val => new StringValueNode(val.ToString()),               
                FunctionCallNode callNode => InterpretFunctionCall(callNode),
                IdentifierNode idNode => _scope.GetVariableValueCopy(idNode.ToString()),
                _ => throw new InvalidDataException($"\"{node}\" is not a valid factor data type.")
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

        private ValueNode CoerceValue(ValueNode value, Type castType) =>

            castType switch
            {
                Type t when t == typeof(IntValueNode) => new IntValueNode(value.ToInt()),
                Type t when t == typeof(FloatValueNode) => new FloatValueNode(value.ToFloat()),
                Type t when t == typeof(StringValueNode) => new StringValueNode(value.ToString()),
                Type t when t == typeof(BoolValueNode) => new BoolValueNode(value.ToBool()),
                _ => throw new InvalidCastException($"Attempted invalid cast from {value.GetType().Name} to {castType.Name}.")
            };
        #endregion
    }
}
