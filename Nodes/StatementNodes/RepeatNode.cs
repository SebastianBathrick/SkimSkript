namespace SkimSkript.Nodes.StatementNodes
{
    internal class RepeatNode : ConditionStructNode
    {
        public RepeatNode(Node condition, Node block, int endLexemeIndex) : base(condition, block, endLexemeIndex)
        {
        }
    }
}
