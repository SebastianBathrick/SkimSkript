using SkimSkript.Nodes.ValueNodes;
using SkimSkript.Nodes;

namespace SkimSkript.Interpretation.Helpers
{
    /// <summary> Static class providing methods to interpret math operations & facilitate coercion. </summary>
    public static class OperationUtilities
    {
        /// <summary> Returns the result of a math operation specifying the operation type with a 
        /// <see cref="MathOperator"/> and the left and right operands with <see cref="ValueNode"/>s. </summary>
        public static ValueNode PerformMathOperation(ValueNode left, ValueNode right, MathOperator mathOp)
        {
            if (IsStringCoercion(left, right))
                return new StringValueNode(GetStringMathOperationResults(left.ToString(), right.ToString(), mathOp));

            if (IsFloatCoercion(left, right))
                return new FloatValueNode(GetFloatMathOperationResult(left.ToFloat(), right.ToFloat(), mathOp));

            if (IsIntCoercion(left, right))
                return new IntValueNode(GetIntMathOperationResult(left.ToInt(), right.ToInt(), mathOp));

            throw new InvalidDataException("Invalid data type coercion");
        }

        /// <summary> Returns the result of a comparison operation specifying the operation type with a 
        /// <see cref="ComparisonOperator"/> and the left and right operands with <see cref="ValueNode"/>s. </summary>
        public static bool PerformComparisonOperation(ValueNode left, ValueNode right, ComparisonOperator comparisonOp)
        {
            if (IsFloatCoercion(left, right))
                return GetFloatComparisonOperationValue(left.ToFloat(), right.ToFloat(), comparisonOp);

            if (IsIntCoercion(left, right))
                return GetIntComparisonOperationValue(left.ToInt(), right.ToInt(), comparisonOp);

            if (IsStringCoercion(left, right))
                return GetStringComparisonOperationValue(left.ToString(), right.ToString(), comparisonOp);

            return GetBoolComparisonOperationValue(left.ToBool(), right.ToBool(), comparisonOp);
        }

        /// <summary> Returns the result of a logical operation specifying the operation type with a 
        /// <see cref="LogicalOperator"/> and the left and right operands with booleans. </summary>
        public static bool PerformLogicalOperation(bool left, bool right, LogicalOperator logicalOp) =>
        logicalOp switch
        {
            LogicalOperator.And => left && right,
            LogicalOperator.Or => left || right,
            LogicalOperator.Xor => left ^ right,
            _ => throw new InvalidDataException("Invalid logical operation.")
        };

        #region Type Operations
        // TODO: Revisit type operations for runtime error implementation.

        /// <summary> Returns the result of an math operation using ints as operands. </summary>
        private static int GetIntMathOperationResult(int left, int right, MathOperator mathOp) =>
            mathOp switch
            {
                MathOperator.Add => left + right,
                MathOperator.Subtract => left - right,
                MathOperator.Multiply => left * right,
                MathOperator.Divide => left / right,
                MathOperator.Modulus => left % right,
                MathOperator.Exponent => (int)Math.Pow(left, right),
                _ => throw new ArgumentException("Integer arithmetic error.")
            };


        /// <summary> Returns the result of an comparison operation using ints as operands. </summary>
        private static bool GetIntComparisonOperationValue(int left, int right, ComparisonOperator comparisonOp) =>
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
        private static float GetFloatMathOperationResult(float left, float right, MathOperator mathOp) =>
            mathOp switch
            {
                MathOperator.Add => left + right,
                MathOperator.Subtract => left - right,
                MathOperator.Multiply => left * right,
                MathOperator.Divide => left / right,
                MathOperator.Exponent => (float)Math.Pow(left, right),
                MathOperator.Modulus => left - right * (float)Math.Floor(left / right),
                _ => throw new ArgumentException("Floating point arithmetic error.")
            };

        /// <summary> Returns the result of an comparison operation using floats as operands. </summary>
        private static bool GetFloatComparisonOperationValue(float left, float right, ComparisonOperator comparisonOp) =>
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
        private static string GetStringMathOperationResults(string left, string right, MathOperator mathOp) =>
            mathOp switch
            {
                MathOperator.Add => left + right,
                MathOperator.Subtract => left.Replace(right, ""),
                _ => throw new ArgumentException("Invalid math operation for strings.")
    };

        /// <summary> Returns the result of an comparison operation using strings as operands. </summary>
        private static bool GetStringComparisonOperationValue(string left, string right, ComparisonOperator comparisonOp) =>
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
        private static bool GetBoolComparisonOperationValue(bool left, bool right, ComparisonOperator comparisonOp) =>
            comparisonOp switch
            {
                ComparisonOperator.Equals => left == right,
                ComparisonOperator.NotEquals => left != right,
                _ => throw new InvalidDataException("Invalid boolean comparison.")
            };
        #endregion

        #region Is Cast Type Methods
        /// <summary> Determines if either the left, right, or both operands are FloatValueNodes. </summary>
        private static bool IsFloatCoercion(ValueNode left, ValueNode right) => left is FloatValueNode || right is FloatValueNode;

        /// <summary> Determines if either the left, right, or both operands are IntValueNodes. </summary>
        private static bool IsIntCoercion(ValueNode left, ValueNode right) => left is IntValueNode || right is IntValueNode;

        /// <summary> Determines if either the left, right, or both operands are StringValueNodes. </summary>
        private static bool IsStringCoercion(ValueNode left, ValueNode right) => left is StringValueNode || right is StringValueNode;
        #endregion
    }
}
