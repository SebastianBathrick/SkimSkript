namespace SkimSkript.Nodes.ExpressionNodes;

public class MathExpressionNode : ExpressionNode<MathOperator>
{
    public MathExpressionNode(MathOperator operatorType, Node leftOperand, Node rightOperand)
        : base(operatorType, leftOperand, rightOperand) { }
}
