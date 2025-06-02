namespace SkimSkript.Nodes
{
    /// <summary>Class representing a mathematical expression.</summary>
    public class MathExpressionNode : ExpressionNode
    {
        private MathOperator _mathOperator;

        /// <summary>Enum indicating which math operator is being used in the given expression.</summary>
        public MathOperator MathOperator => _mathOperator;

        /// <summary>Constructor for a node meant to represent a math expression.</summary>
        /// <param name="mathOperator">Enum value representing the math operator used in the expression.</param>
        /// <param name="leftOperand">Node representing the left operand. This can be any factor, term, or
        /// child of <see cref="ExpressionNode"/>.</param>
        /// <param name="rightOperand">Node representing the left operand. This can be any factor, term, or
        /// child of <see cref="ExpressionNode"/>.</param>
        public MathExpressionNode(MathOperator mathOperator, Node leftOperand, Node rightOperand):  base(leftOperand, rightOperand) =>
            _mathOperator = mathOperator;

        public override string ToString() => String.Empty;
    }
}
