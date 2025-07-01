namespace SkimSkript.Nodes
{
    internal class AssertionNode : StatementNode
    {
        private Node _condition;

        public Node Condition => _condition;

        public AssertionNode(Node conditionExpression) =>
            _condition = conditionExpression;

        public override string ToString() =>
            $"{_condition}";
    }
}
