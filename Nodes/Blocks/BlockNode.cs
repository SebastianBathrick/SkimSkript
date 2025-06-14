namespace SkimSkript.Nodes
{
    /// <summary>Node representing a block containing nested statements.</summary>
    public class BlockNode : Node
    {
        private Node[]? _statements;

        /// <summary> List of nodes representing sequential statements. </summary>
        public Node[]? Statements => _statements;

        public BlockNode(Node[]? statments) => _statements = statments;

        public override string ToString() => String.Empty;
    }
}


