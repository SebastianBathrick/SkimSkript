namespace SkimSkript.Nodes
{
    /// <summary>Represents the root of an abstract syntax tree including children in the form of user-defined
    /// functions and top-level statements.</summary>
    public class AbstractSyntaxTreeRoot : BlockNode
    {
        private List<Node> _functions;

        /// <summary>List of nodes representing each user-defined function definition and body.</summary>
        public List<Node> Functions => _functions;

        /// <summary>Constructor for a node that represents the root of an abstract syntax tree.</summary>
        /// <param name="childStatements">List of top-level sequential statements.</param>
        /// <param name="functions">List of each user-defined function definitions alongside the associated bodies.</param>
        public AbstractSyntaxTreeRoot(List<Node> childStatements, List<Node> functions) : base(childStatements) =>
            _functions = functions;
    }
}
