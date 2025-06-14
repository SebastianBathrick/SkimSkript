namespace SkimSkript.Nodes
{
    /// <summary>Class representing a mathematical expression.</summary>
    public class MathExpressionNode : ExpressionNode<MathOperator>
    {
        public MathExpressionNode(MathOperator operatorType, Node leftOperand, Node rightOperand) 
            : base(operatorType, leftOperand, rightOperand)
        {
        }
    }
}
