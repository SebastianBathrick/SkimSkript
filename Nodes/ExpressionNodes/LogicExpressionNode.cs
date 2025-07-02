namespace SkimSkript.Nodes
{
    internal class LogicExpressionNode : ExpressionNode<LogicOperator>
    {
        public LogicExpressionNode(LogicOperator operatorType, Node leftOperand, Node rightOperand)
            : base(operatorType, leftOperand, rightOperand) { }

        public bool IsShortCircuit(bool isLeftOperandTrue) =>
            isLeftOperandTrue && Operator == LogicOperator.Or || !isLeftOperandTrue && Operator == LogicOperator.And;
    }
}
