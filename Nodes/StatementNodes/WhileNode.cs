using SkimSkript.Nodes.StatementNodes;

namespace SkimSkript.Nodes
{
    /// <summary>Class representing a while loop control structure.</summary>
    internal class WhileNode : ConditionStructNode
    {
        public WhileNode(Node condition, Node block) : base(condition, block) { }

        public override string ToString() => $"while{base.ToString()}";
    }
}
