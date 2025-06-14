namespace SkimSkript.Nodes
{
    /// <summary>Class representing a user-defined function node with an identifier, parameters, return type, 
    /// and a code block.</summary>
    public class FunctionNode : CallableNode
    {        
        private Node _block;

        /// <summary>Node representing the block that serves as the function body.</summary>
        public Node Block => _block;

        /// <summary>Constructor for a node containing data for the definition and body of a 
        /// user-defined function.</summary>
        /// <param name="identifier">The user-defined identifier used when calling the function.</param>
        /// <param name="returnTypeNode">Instance of the type of node to be returned to the caller.</param>
        /// <param name="parameters">List of nodes representing parameter declarations.</param>
        /// <param name="block">Block of code to be executed during a function call.</param>
        public FunctionNode(string identifier, Type? returnType, List<Node> parameters, Node block) : base(identifier, parameters, returnType) =>
            _block = block;
    }
}
