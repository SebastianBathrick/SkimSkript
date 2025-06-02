namespace SkimSkript.Nodes
{
    /// <summary>Class representing a while loop control structure.</summary>
    public class WhileNode : ControlStructNode
    {
        /// <param name="condition">Condition required for the block to execute.</param>
        /// <param name="block">The block that might execute once, multiple times, or never.</param>
        public WhileNode(Node condition, Node block) : base(condition, block) { }
        public override string ToString() => $"while{base.ToString()}";
    }
}
