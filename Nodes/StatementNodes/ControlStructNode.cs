namespace SkimSkript.Nodes
{
    /// <summary>Abstract class representing a control structure with a condition of execution for a stored block.</summary>
    public abstract class ControlStructNode : StatementNode
    {
        private Node _condition;
        private Node _block;

        /// <summary>Node representing the condition in which the structure's block would be executed.</summary>
        public Node Condition => _condition;

        /// <summary>Node representing the structure's block that can potentially be executed.</summary>
        public Node Block => _block;

        /// <param name="condition">Node representing the condition in which the structure's block would be executed.</param>
        /// <param name="block">Node representing the structure's block that can potentially be executed.</param>
        public ControlStructNode(Node condition, Node block)
        {
            _condition = condition;
            _block = block;
        }

        public override string ToString() => $"({_condition}){_block}";
    }
}
