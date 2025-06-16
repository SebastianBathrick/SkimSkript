using SkimSkript.Nodes;

namespace SkimSkript.Interpretation.Helpers
{
    internal class CoercionInterpreter
    {
        private readonly Type[] _expressionTypePrecidence =
            [typeof(StringValueNode), typeof(FloatValueNode), typeof(IntValueNode)];

        #region Public Methods
        /// <summary> Forcefully an assumed <see cref="ValueNode"/> to a target type of the same base class. </summary>
        public Node CoerceNodeValue(Node node, Type targetType)
        {
            if (node is not ValueNode valueNode)
                throw new ArgumentException($"{node.GetType().Name} is not a coercible type");

            return CoerceValue(valueNode, targetType);
        }

        /// <summary> Coerces two operators based on their relationship and data-type precidence rules </summary>
        public Type CoerceOperands(Node left, Node right, out Node coercedLeft, out Node coercedRight)
        {
            if (left is not ValueNode leftValNode || right is not ValueNode rightValNode)
                throw new ArgumentException(
                    $"Invalid operand coercion. Attempted to coerce non-value types {left.GetType().Name} and {right.GetType().Name}");

            var lType = left.GetType();
            var rType = right.GetType();

            if(lType == rType)
            {
                // If both operands are the same type, no coercion is needed
                coercedLeft = leftValNode;
                coercedRight = rightValNode;
                return lType;
            }

            // Check if an operator has precidence over the other or if their already the same type
            foreach (var type in _expressionTypePrecidence)
                if (TryOperandCoercionOfType(
                    type, leftValNode, rightValNode, lType, rType, out coercedLeft, out coercedRight))
                    return type;

            throw new InvalidCastException(
                $"Invalid operand coercion. Different types but no rules defined for {left.GetType().Name}  and {right.GetType().Name}");
        }

        /// <summary> Coerces evaluated operand to <see cref="BoolValueNode"/> and returns its stored bool value. </summary>
        public bool CoerceLogicOperand(Node operand, out Node coercedOperand)
        {
            var targetType = typeof(BoolValueNode);
            coercedOperand = operand.GetType() == targetType ? operand : CoerceNodeValue(operand, targetType);
            return ((ValueNode)coercedOperand).ToBool();
        }

        /// <summary> Coerces evaluated condition to <see cref="BoolValueNode"/> and returns its stored bool value. </summary>
        /// <remarks> Primarily intended for condition-based control structures. </remarks>
        public bool CoerceCondition(Node evaluatedCondition) =>
            CoerceLogicOperand(evaluatedCondition, out _);
        #endregion

        #region Primary Coercion Methods
        private Node CoerceValue(ValueNode value, Type castType)
        {
            var valueNode = value;

            if (valueNode.GetType() == castType)
                return valueNode;

            switch (castType)
            {
                case Type t when t == typeof(IntValueNode):
                    return new IntValueNode(valueNode.ToInt());
                case Type t when t == typeof(FloatValueNode):
                    return new FloatValueNode(valueNode.ToFloat());
                case Type t when t == typeof(StringValueNode):
                    return new StringValueNode(valueNode.ToString());
                case Type t when t == typeof(BoolValueNode):
                    return new BoolValueNode(valueNode.ToBool());
                default:
                    throw new InvalidCastException(
                        $"Attempted undefined cast from {valueNode.GetType().Name} to {castType.Name}.");
            }
        }

        private bool TryOperandCoercionOfType(
            Type targetType, 
            ValueNode left, ValueNode right,
            Type leftType, Type rightType,
            out Node coercedLeft, 
            out Node coercedRight)
        {
            var isLeftOfType = leftType == targetType;
            var isRightOfType = rightType == targetType;

            if (isLeftOfType || isRightOfType)
            {
                coercedLeft = isLeftOfType ? left : CoerceValue(left, targetType);
                coercedRight = isRightOfType ? right : CoerceValue(right, targetType);
                return true;
            }

            coercedLeft = left;
            coercedRight = right;
            return false;
        }
        #endregion
    }
}
