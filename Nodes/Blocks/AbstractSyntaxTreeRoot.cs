namespace SkimSkript.Nodes
{
    /// <summary>Represents the root of an abstract syntax tree including children in the form of user-defined
    /// userDefinedFunctions and top-level statements.</summary>
    public class AbstractSyntaxTreeRoot : BlockNode
    {
        private Node[]? _functions;

        public bool IsUserDefinedFunctions => _functions != null;

        public Node[]? UserFunctions => _functions;

        public AbstractSyntaxTreeRoot(Node[] topLevelStatements, Node[] userDefinedFunctions) : base(topLevelStatements) =>
            _functions = userDefinedFunctions;
    }
}
