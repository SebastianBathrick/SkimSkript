namespace SkimSkript.Nodes
{
    /// <summary>Represents a call to a user-defined or built-in function.</summary>
    public class FunctionCallNode : StatementNode
    {
        private string _identifier;
        private List<(Node value, bool isRef)>? _arguments;

        /// <summary>The identifier of the function being called.</summary>
        public string Identifier => _identifier;

        /// <summary>A list of nodes storing the arguments themselves each paired with whether it's marked as a reference.</summary>
        /// <remarks>It is presumed that if an argument was marked as reference that the node will store only an identifier.</remarks>
        public List<(Node value, bool isRef)>? Arguments => _arguments;

        /// <summary></summary>
        /// <param name="identifier">The identifier of the function being called.</param>
        /// <param name="arguments">It is presumed that if an argument was marked as reference that the node will store only an identifier.</param>
        public FunctionCallNode(string identifier, List<(Node value, bool isRef)>? arguments)
        {
            _identifier = identifier;
            _arguments = arguments;
        }

        public override string ToString() => $"{_identifier}()";
    }
}
