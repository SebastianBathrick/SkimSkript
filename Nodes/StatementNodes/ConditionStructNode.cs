namespace SkimSkript.Nodes.StatementNodes
{
    internal class ConditionStructNode : ControlStructNode
    {
        private Node _condition;

        public Node Condition => _condition;

        public ConditionStructNode(Node condition, Node block, int endLexemeIndex) : base(block, endLexemeIndex)
        {
            SetLexemeEndIndex(endLexemeIndex);
            _condition = condition;
        }

    }
}
