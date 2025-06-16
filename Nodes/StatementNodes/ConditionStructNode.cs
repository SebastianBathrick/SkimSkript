namespace SkimSkript.Nodes.StatementNodes
{
    internal class ConditionStructNode : ControlStructNode
    {
        private Node _condition;

        public Node Condition => _condition;

        public ConditionStructNode(Node condition, Node block) : base(block) => _condition = condition;

    }
}
