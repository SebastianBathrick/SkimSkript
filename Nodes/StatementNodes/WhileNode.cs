namespace SkimSkript.Nodes
{
    /// <summary>Class representing a while loop control structure.</summary>
    public class WhileNode : ControlStructNode
    {
        private Node _condition;

        public Node Condition => _condition;

        public WhileNode(Node condition, Node block) : base(block) => _condition = condition;

        public override string ToString() => $"while{base.ToString()}";
    }
}
