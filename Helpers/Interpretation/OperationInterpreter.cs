using SkimSkript.ErrorHandling;
using SkimSkript.Nodes;

namespace SkimSkript.Interpretation.Helpers
{
    public class OperationInterpreter
    {
        private enum OperandDataType { Integer, Float, String, Bool }

        private Dictionary<Type, OperandDataType> _dataTypeMap = new()
        {
            {typeof(IntValueNode), OperandDataType.Integer },
            {typeof(FloatValueNode), OperandDataType.Float },
            {typeof(BoolValueNode), OperandDataType.Bool },
            {typeof(StringValueNode), OperandDataType.String },
        };


        public Node PerformMathOperation(Node left, Node right, MathOperator mathOp)
        {
            GetValueNodeOperands(left, right, out var leftValueNode, out var rightValueNode);
            var operandDataType = GetOperandDataType(leftValueNode);
            
            switch (operandDataType)
            {
                case OperandDataType.Integer:
                    int intLeft = leftValueNode.ToInt();
                    int intRight = rightValueNode.ToInt();
                    var intResult = GetMathResultInt(intLeft, intRight, mathOp);
                    return new IntValueNode(intResult);

                case OperandDataType.Float:
                    float floatLeft = leftValueNode.ToFloat();
                    float floatRight = rightValueNode.ToFloat();
                    var floatResult = GetMathResultFloat(floatLeft, floatRight, mathOp);
                    return new FloatValueNode(floatResult);

                case OperandDataType.Bool:
                    bool leftBool = leftValueNode.ToBool();
                    bool rightBool = rightValueNode.ToBool();
                    var boolResult = GetMathResultBool(leftBool, rightBool, mathOp);
                    return new BoolValueNode(boolResult);

                case OperandDataType.String:
                    string leftString = leftValueNode.ToString();
                    string rightString = rightValueNode.ToString();
                    var stringResult = GetMathResultString(leftString, rightString, mathOp);

                    return new StringValueNode(stringResult);

                default:
                    throw new InvalidOperationException(
                        $"{mathOp} is not supported for {operandDataType}.");
            }
        }

        public Node PerformComparisonOperation(Node left, Node right, ComparisonOperator comparisonOp)
        {
            GetValueNodeOperands(left, right, out var leftValueNode, out var rightValueNode);
            var operandDataType = GetOperandDataType(leftValueNode);
            bool result;

            switch(operandDataType)
            {
                case OperandDataType.Integer:
                    result = GetIntComparisonOperationValue(
                        leftValueNode.ToInt(), rightValueNode.ToInt(), comparisonOp);
                    break;
                case OperandDataType.Float:
                    result = GetFloatComparisonOperationValue(
                        leftValueNode.ToFloat(), rightValueNode.ToFloat(), comparisonOp);
                    break;
                case OperandDataType.String:
                    result = GetStringComparisonOperationValue(
                        leftValueNode.ToString(), rightValueNode.ToString(), comparisonOp);
                    break;
                case OperandDataType.Bool:
                    result = GetBoolComparisonOperationValue(
                        leftValueNode.ToBool(), rightValueNode.ToBool(), comparisonOp);
                    break;
                default:
                    throw new InvalidOperationException($"{comparisonOp} is not supported for {operandDataType}.");
            }

            return new BoolValueNode(result);
        }

        public Node PerformLogicOperation(Node left, Node right, LogicOperator logicOp)
        {
            if (left is not BoolValueNode leftBoolNode || right is not BoolValueNode rightBoolNode)
                throw new InvalidOperationException(
                    $"None {typeof(BoolValueNode).Name}(s) used in logic operation");

            return new BoolValueNode(
                GetLogicOperationResult(leftBoolNode.ToBool(), rightBoolNode.ToBool(), logicOp));
        }



        #region Type Operations
        // TODO: Revisit type operations for runtime error implementation.

        /// <summary> Returns the result of an math operation using ints as operands. </summary>
        private int GetMathResultInt(int left, int right, MathOperator mathOp)
        {
            switch(mathOp)
            {
                case MathOperator.Add: return left + right;
                case MathOperator.Subtract: return left - right;
                case MathOperator.Multiply: return left * right;
                case MathOperator.Divide: 
                    if(right == 0)
                        throw new RuntimeException("Integer division by zero. {Left} divided by {Right}", null, left, right);

                    return left / right;
                case MathOperator.Modulus: return left % right;
                case MathOperator.Exponent: return (int)Math.Pow(left, right);
                default:
                    throw new ArgumentException("Invalid math operation for integers.");
            }
        }



        /// <summary> Returns the result of an comparison operation using ints as operands. </summary>
        private bool GetIntComparisonOperationValue(int left, int right, ComparisonOperator comparisonOp) =>
            comparisonOp switch
            {
                ComparisonOperator.Equals => left == right,
                ComparisonOperator.NotEquals => left != right,
                ComparisonOperator.GreaterThan => left > right,
                ComparisonOperator.GreaterThanOrEqual => left >= right,
                ComparisonOperator.LessThanOrEqual => left <= right,
                ComparisonOperator.LessThan => left < right,
                _ => throw new ArgumentException("Invalid comparison operation for integers.")
            };

        /// <summary> Returns the result of an math operation using floats as operands. </summary>
        private float GetMathResultFloat(float left, float right, MathOperator mathOp)
        {
            switch (mathOp)
            {
                case MathOperator.Add: return left + right;
                case MathOperator.Subtract: return left - right;
                case MathOperator.Multiply: return left * right;
                case MathOperator.Divide:
                    if (right == 0)
                        throw new RuntimeException("Floating-point division by zero. {Left} divided by {Right}", 
                            null, left, right);

                    return left / right;
                case MathOperator.Modulus: return left % right;
                case MathOperator.Exponent: return left - right * (float)Math.Floor(left / right);
                default:
                    throw new ArgumentException("Invalid math operation for floating-points.");
            }
        }

        /// <summary>
        /// Evaluates a boolean expression using math-like operators,
        /// where the result is still a boolean value.
        /// </summary>
        private bool GetMathResultBool(bool left, bool right, MathOperator mathOp) =>
            mathOp switch
            {
                // Logical OR: true if either operand is true
                MathOperator.Add => left || right,

                // Logical "A but not B": true if left is true and right is false
                MathOperator.Subtract => left && !right,

                // Logical AND: true if both operands are true
                MathOperator.Multiply => left && right,

                // Logical "only if left is false and right is true"
                // (NOT logical division or implication — just a unique behavior)
                MathOperator.Divide => !left && right,

                // Logical implication (right ⇒ left): true if right is false or left is true
                MathOperator.Exponent => !right || left,

                // Logical XOR: true if operands differ
                MathOperator.Modulus => left != right,

                // Fallback for undefined operators
                _ => throw new ArgumentException("Boolean arithmetic error.")
            };

        /// <summary> Returns the result of an comparison operation using floats as operands. </summary>
        private bool GetFloatComparisonOperationValue(float left, float right, ComparisonOperator comparisonOp) =>
            comparisonOp switch
            {
                ComparisonOperator.Equals => left == right,
                ComparisonOperator.NotEquals => left != right,
                ComparisonOperator.GreaterThan => left > right,
                ComparisonOperator.GreaterThanOrEqual => left >= right,
                ComparisonOperator.LessThanOrEqual => left <= right,
                ComparisonOperator.LessThan => left < right,
                _ => throw new ArgumentException("Invalid comparison operation for floats.")
            };

        /// <summary> Returns the result of an math operation using strings as operands. </summary>
        private string GetMathResultString(string left, string right, MathOperator mathOp) =>
            mathOp switch
            {
                MathOperator.Add => left + right,
                MathOperator.Subtract => left.Replace(right, ""),
                _ => throw new ArgumentException("Invalid math operation for strings.")
            };


        /// <summary> Returns the result of an comparison operation using strings as operands. </summary>
        private bool GetStringComparisonOperationValue(string left, string right, ComparisonOperator comparisonOp) =>
            comparisonOp switch
            {
                ComparisonOperator.Equals => left == right,
                ComparisonOperator.NotEquals => left != right,
                ComparisonOperator.GreaterThan => string.Compare(left, right, StringComparison.Ordinal) > 0,
                ComparisonOperator.GreaterThanOrEqual => string.Compare(left, right, StringComparison.Ordinal) >= 0,
                ComparisonOperator.LessThan => string.Compare(left, right, StringComparison.Ordinal) < 0,
                ComparisonOperator.LessThanOrEqual => string.Compare(left, right, StringComparison.Ordinal) <= 0,
                _ => throw new ArgumentException("Invalid comparison operation for strings.")
            };


        /// <summary> Returns the result of an comparison operation using bools as operands. </summary>
        private bool GetBoolComparisonOperationValue(bool left, bool right, ComparisonOperator comparisonOp) =>
            comparisonOp switch
            {
                ComparisonOperator.Equals => left == right,
                ComparisonOperator.NotEquals => left != right,
                _ => throw new InvalidDataException("Invalid boolean comparison.")
            };

        /// <summary> Returns the result of a logical operation specifying the operation type with a 
        /// <see cref="LogicOperator"/> and the left and right operands with booleans. </summary>
        private bool GetLogicOperationResult(bool left, bool right, LogicOperator logicalOp) =>
            logicalOp switch
            {
                LogicOperator.And => left && right,
                LogicOperator.Or => left || right,
                LogicOperator.Xor => left ^ right,
                _ => throw new InvalidDataException("Invalid logical operation.")
            };
        #endregion

        #region Helper Methods
        private OperandDataType GetOperandDataType(ValueNode valueNode)
        {
            if (!_dataTypeMap.TryGetValue(valueNode.GetType(), out var operandDataType))
                throw new InvalidOperationException(
                    $"Undefined {typeof(OperandDataType).Name} for {valueNode.GetType().Name}");
            return operandDataType;
        }

        private void GetValueNodeOperands(Node left, Node right, out ValueNode leftValueNode, out ValueNode rightValueNode)
        {
            if (left is not ValueNode castedLeft || right is not ValueNode castedRight)
                throw new InvalidOperationException(
                    $"Operation failed. Invalid Operands {left.GetType().Name} and {right.GetType().Name}");
            leftValueNode = castedLeft;
            rightValueNode = castedRight;
        }
        #endregion
    }
}
