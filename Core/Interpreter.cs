using SkimSkript.Interpretation.Helpers;
using SkimSkript.Nodes.ValueNodes;
using SkimSkript.Nodes.Runtime;
using SkimSkript.Nodes;
using SkimSkript.Nodes.StatementNodes;
using SkimSkript.Nodes.NodeExceptions;

namespace SkimSkript.Interpretation
{
    /// <summary> The <c>Interpreter</c> class is responsible for executing the program represented by an
    ///  Abstract Syntax Tree (AST) generated from the parsed source code. It handles function calls, 
    ///  variable management, expressions,  and control structures (e.g., if, while) to produce 
    ///  runtime results. </summary>
    public class Interpreter
    {        
        private ScopeManager _scope = new ScopeManager();
        private ValueNode _lastReturnedValue = new IntValueNode();
        private Dictionary<string, CallableNode> _callableFunctionsDict; 

        public Interpreter(AbstractSyntaxTreeRoot root)
        {
            _callableFunctionsDict = AddCallableFunctionsToMap(root.Functions);
            InterpretBlock(root);
        }

        /// <summary>Executes a block node, entering a new scope and executing each statement.</summary>
        /// <returns>True if the loop executed a return statement while interpreting its block.</returns>
        private bool InterpretBlock(Node block)
        {            
            _scope.EnterScope();          
            List<Node> statements = ((BlockNode)block).Statements;
            bool isReturning = false;

            foreach (StatementNode statement in statements)
                if (isReturning = InterpretStatement(statement))
                    break;
            
            if(!isReturning)
                _scope.ExitScope();

            return isReturning;
        }

        /// <summary>Returns all user-defined & built-in functions in a map with keys defined by
        /// their identifier matched to their <see cref="CallableNode"/>.</summary>
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
        /// <summary>Evaluates a statement node and performs the appropriate action based on its type.
        /// Returns true if a return statement was executed and false if otherwise.</summary>
        private bool InterpretStatement(StatementNode node)
        {
            switch (node)
            {
                case VariableDeclarationNode varNode: InterpretVariableDeclaration(varNode); break;
                case FunctionCallNode functionCallNode: InterpretFunctionCall(functionCallNode); break;
                case AssignmentNode assignmentNode: AssignValueToVariable(assignmentNode); break;
                case AssertionNode assertionNode: InterpretAssertion(assertionNode); break;
                case IfNode ifNode: return InterpretIfStructure(ifNode);
                case WhileNode whileNode: return InterpretWhileStructure(whileNode);
                case ReturnNode returnNode: InterpretReturnStatement(returnNode); return true;
            }

            return false;
        }

        /// <summary>Invokes a function call node and returns its result as a ValueNode.</summary>
        /// <returns>The result of the function call as a <see cref="ValueNode"/>. If the function has no return, 
        /// returns a default <see cref="IntValueNode"/> with a value of 0.</returns>
        private ValueNode InterpretFunctionCall(FunctionCallNode functionCallNode)
        {
            var callArgs = functionCallNode.Arguments;

            // Get values from expressions and references to variables sent by the caller
            var interpretedArgs = InterpretArguments(functionCallNode.Arguments);

            // Get function definiion and body
            //TODO: Handle function not found exception.
            var callableNode = _callableFunctionsDict[functionCallNode.Identifier]; 

            // If not user-defined, then it must be a built-in function.
            if (callableNode is BuiltInFunctionNode)
            {
                var builtInFunctionNode = (BuiltInFunctionNode)callableNode;

                var returnValue = builtInFunctionNode.Call(interpretedArgs);
                returnValue ??= new IntValueNode(); //TODO: Handle built-in return types properly.

                return (ValueNode)returnValue;
            }

            var functionNode = (FunctionNode)callableNode;

            // Enter the scope of the function body
            _scope.EnterFunctionScope();

            // Declare parameters in the newly added function scope
            AssignValuesToParameters(functionNode.Parameters, interpretedArgs);

            // Execute each statement in the function body block.
            InterpretBlock(functionNode.Block);

            // Exit the scope of the function body to caller scope.
            _scope.ExitFunctionScope();

            return _lastReturnedValue; //TODO: Handle user-defined return types properly.
        }

        /// <summary>Evaluates a while loop, executing its block while the condition is true.</summary>
        /// <returns>True if the loop executed a return statement while interpreting its block.</returns>
        private bool InterpretWhileStructure(WhileNode whileNode)
        {
            while (EvaluateExpression(whileNode.Condition).ToBool())
                if (InterpretBlock((BlockNode)whileNode.Block))
                    return true; //Exit block early.

            return false;
        }

        /// <summary>Evaluates an if statement, executing its block if the condition is true, and any else-if or else blocks as appropriate.</summary>
        /// <returns>True if the if structure executed a return statement while interpreting its block.</returns>
        private bool InterpretIfStructure(IfNode ifNode)
        {
            if (EvaluateExpression(ifNode.Condition).ToBool())
                return InterpretBlock((BlockNode)ifNode.Block);
            else if (ifNode.ElseIfNode != null)
                return InterpretIfStructure(ifNode.ElseIfNode);

            //A lone if statement successfully executed block without returning.
            return false;
        }

        /// <summary>Handles return statement and sets the last returned value.</summary>
        private void InterpretReturnStatement(ReturnNode returnNode)  =>     
            _lastReturnedValue = returnNode.Expression != null ? 
            EvaluateExpression(returnNode.Expression) : _lastReturnedValue;

        /// <summary>Declares a new variable in the current scope.</summary>
        private void InterpretVariableDeclaration(VariableDeclarationNode declarationNode)
        {          
            var dataType = declarationNode.DataType;
            var identifier = declarationNode.Identifier;

            // Variable declarations can be assigned any expression type during parsing.
            var initVal = EvaluateExpression(declarationNode.AssignedExpression);

            // Coerce the value to be the same type as the variable's data type.
            var coercedInitVal = CoerceValue(initVal, dataType);

            if(coercedInitVal == null)
                throw new InvalidDataException(
                    $"Variable \"{identifier}\" cannot be initialized with a value of type {initVal.GetType().Name}.\n" +
                    $"Expected type: {dataType.Name}.");

            /* Even if there was no explicit initialization, the parser will still store a value 
             * node with a default value in the declaration node. Works like C# primitives. */
            _scope.AddVariable(identifier, coercedInitVal, dataType);
        }

        /// <summary>Evaluates an assertionNode statement and ensures the condition associated is true.</summary>
        /// <exception cref="AssertionException">
        /// Thrown if the conditional expression stored in the statement is false.
        /// </exception>
        private void InterpretAssertion(AssertionNode assertionNode)
        {
            try
            {
                var interpretedExpression = EvaluateExpression(assertionNode.Condition);

                if (!interpretedExpression.ToBool())
                    throw new AssertionException($"{assertionNode.ToString()}\nInstead left left operand evaluated to: {interpretedExpression}");
            }
            catch(Exception ex)
            {
                throw new AssertionException($"{assertionNode.ToString()}\nException was thrown while executing assertion: {ex.Message}");
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
                FunctionCallNode callNode => InterpretFunctionCall(callNode).Copy(),
                IdentifierNode idNode => _scope.GetVariableValueCopy(idNode.ToString()).Copy(),
                _ => throw new InvalidDataException($"\"{node}\" is not a valid factor data type.")
            };
        #endregion

        #region Helper Methods
        /// <summary>Assigns a value to an existing variable in the current scope.</summary>
        private void AssignValueToVariable(AssignmentNode assignment)
        {
            // The expression can be of any ValueNode type, even after evaluation
            var rawAssignVal = EvaluateExpression(assignment.AssignedExpression);

            var identifier = assignment.IdentifierNode.ToString();
            var varDataType = _scope.GetVariableDataType(identifier);

            // Coerce the evaluated expression to the variable's data type
            var coercedAssignVal = CoerceValue(rawAssignVal, varDataType);

            if(coercedAssignVal == null)
                throw new InvalidDataException(
                    $"Variable \"{assignment.IdentifierNode}\" cannot be assigned a value of type {rawAssignVal.GetType().Name}.\n" +
                    $"Expected type: {varDataType.Name}.");

            _scope.AssignValueToVariable(identifier, coercedAssignVal);
        }

        private List<(Node source, bool isRef)> InterpretArguments(List<(Node source, bool isRef)> arguments)
        {
            List<(Node source, bool isRef)> evaluatedArgs = new();

            foreach (var arg in arguments)
            {
                var rawArg = arg.source;
                var isRefArg = arg.isRef;

                if(!isRefArg)
                {
                    // Value type arguments are just evaluated into a ValueNode
                    var evaluatedValueArg = EvaluateExpression(rawArg);
                    evaluatedArgs.Add((rawArg, isRefArg));
                    continue;
                }

                if(rawArg is not IdentifierNode)
                    throw new InvalidDataException($"Reference argument must be an identifier, but got: {arg.source}.");

                // Pointer will maintain ValueNode type, because coerced values are new instances of ValueNode.
                var variablePointer = _scope.GetVariablePointer(rawArg.ToString());
                evaluatedArgs.Add((variablePointer, isRefArg));
            }

            return evaluatedArgs;
        }

        // TODO: Refactor method to avoid instantiating VariableDeclarationNodes for each use of a pass-by-value parameter.
        private void AssignValuesToParameters(List<Node> parameters, List<(Node source, bool isRef)>? processedArgs)
        {
            for (int i = 0; processedArgs != null && i < processedArgs.Count; i++)
            {
                var arg = processedArgs[i];
                var parameter = (ParameterNode)parameters[i];

                if (processedArgs[i].isRef)
                    //Reuse the VariableNode associated with the variable passed as a reference argument.
                    _scope.AddVariable(parameter.Identifier, (VariableNode)arg.source);
                else
                {
                    //Get parameter's data type
                    Type paramType = ((VariableDeclarationNode)parameter.ValueSource).DataType.GetType();
                    //Cast an argument to the parameter's data type.
                    ValueNode castedArg = CoerceValue((ValueNode)(processedArgs[i].source), paramType);

                    InterpretVariableDeclaration(new VariableDeclarationNode(parameter.Identifier, paramType, castedArg));
                }
            }
        }

        private ValueNode? CoerceValue(ValueNode value, Type castType) =>

            castType switch
            {
                Type t when t == typeof(IntValueNode) => new IntValueNode(value.ToInt()),
                Type t when t == typeof(FloatValueNode) => new FloatValueNode(value.ToFloat()),
                Type t when t == typeof(StringValueNode) => new StringValueNode(value.ToString()),
                Type t when t == typeof(BoolValueNode) => new BoolValueNode(value.ToBool()),
                _ => null
            };
        #endregion
    }
}
