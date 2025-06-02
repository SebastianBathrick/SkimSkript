namespace SkimSkript.Nodes
{
    /// <summary>Class representing an if statement control structure potentially storing a child else
    /// if or else statement of the same class type.</summary>
    public class IfNode : ControlStructNode
    {
        private IfNode? _elseIfNode;

        /// <summary>Node representing an else if or else control structure.</summary>
        public IfNode? ElseIfNode => _elseIfNode;

        /// <param name="condition">Condition required for the block to execute.</param>
        /// <param name="block">The block that will can be executed a single time or never.</param>
        /// <param name="elseIfNode">Node representing any else/else if structure that is present.</param>
        public IfNode(Node condition, Node block, IfNode? elseIfNode) : base(condition, block) =>
            _elseIfNode = elseIfNode;

        public override string ToString() => $"if{base.ToString()}";
    }
}