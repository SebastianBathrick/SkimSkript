namespace SkimSkript.Nodes
{
    public class MathExpressionNode : ExpressionNode<MathOperator>
    {
        public MathExpressionNode(MathOperator operatorType, Node leftOperand, Node rightOperand)
            : base(operatorType, leftOperand, rightOperand) { }
    }
}
