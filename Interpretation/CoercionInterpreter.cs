using SkimSkript.Nodes;
using SkimSkript.Nodes.ValueNodes;

namespace SkimSkript.Interpretation.Helpers
{
    internal class CoercionInterpreter
    {
        private readonly Type[] _expressionTypePrecidence =
            [typeof(StringValueNode), typeof(FloatValueNode), typeof(IntValueNode)];
        
        public Node CoerceNodeValue(Node node, Type castType)
        {
            if (node is not ValueNode valueNode)
                throw new ArgumentException($"{node.GetType().Name} is not a coercible type");

            return CoerceValue(valueNode, castType);
        }

        private Node CoerceValue(ValueNode value, Type castType)
        {
            var valueNode = value;

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

        public Type CoerceOperands(Node left, Node right, out Node coercedLeft, out Node coercedRight)
        {
            if (left is not ValueNode leftValNode || right is not ValueNode rightValNode)
                throw new ArgumentException(
                    $"Invalid operand coercion. Attempted to coerce non-value types {left.GetType().Name} and {right.GetType().Name}");

            var lType = left.GetType();
            var rType = right.GetType();

            // Check if an operator has precidence over the other or if their already the same type
            foreach(var type in _expressionTypePrecidence)
                if (TryOperandCoercionOfType(
                    type, leftValNode, rightValNode, lType, rType, out coercedLeft, out coercedRight))
                    return type;

            if (lType != rType)
                throw new InvalidCastException(
                    $"Invalid operand coercion. Different types but no rules defined for {left.GetType().Name}  and {right.GetType().Name}");
                
            coercedLeft = left; coercedRight = right;
            return lType;
        }

        public bool CoerceLogicOperand(Node operand, out Node coercedOperand)
        {
            var targetType = typeof(BoolValueNode);
            coercedOperand = operand.GetType() == targetType ? operand : CoerceNodeValue(operand, targetType);
            return ((ValueNode)coercedOperand).ToBool();
        }

        public bool CoerceCondition(Node evaluatedCondition) =>
            CoerceLogicOperand(evaluatedCondition, out _);

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
    }
}
