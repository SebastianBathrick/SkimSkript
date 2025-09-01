namespace SkimSkript.Nodes
{
    internal class ComparisonExpressionNode : ExpressionNode<ComparisonOperator>
    {
        public ComparisonExpressionNode(ComparisonOperator operatorType, Node leftOperand, Node rightOperand)
            : base(operatorType, leftOperand, rightOperand) { }
    }
}
