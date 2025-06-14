namespace SkimSkript.Nodes
{
    /// <summary>Abstract class representing a control structure with a condition of execution for a stored block.</summary>
    public abstract class ControlStructNode : StatementNode
    {
        private Node _block;

        public Node Block => _block;

        public ControlStructNode(Node block) => _block = block;

        public override string ToString() => $"\n{_block}";
    }
}
