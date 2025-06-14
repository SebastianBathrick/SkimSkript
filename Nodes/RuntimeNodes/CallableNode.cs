namespace SkimSkript.Nodes
{
    /// <summary>Abstract class representing a node containing data associated with a subroutine. That data
    /// includes the subroutine's identifier, parameters it accepts, a return type, and whether or not it's variadic.</summary>
    public abstract class CallableNode : Node
    {
        private string _identifier; // TODO: Change string callable identifier to a node
        private List<Node> _parameters;

        protected bool isVariadic = false;
        protected Node? _returnTypeNode; // TODO: Change the _returnTypeNode to Type instead of Node

        /// <summary>The identifier to be referenced to call the subroutine.</summary>
        public string Identifier => _identifier;

        /// <summary>Flag indicating whether an object can accept a variable number of parameters.</summary>
        /// <remarks>Note: This is not indicitive of the minimum number of statements. That is defined on a
        /// class-by-class basis.</remarks>
        public bool IsVariadic => isVariadic;

        /// <summary>Number of parameters the subroutine accepts.</summary> 
        public int ParameterCount => _parameters.Count;

        /// <summary>A list of nodes that represent the parameters the subroutine accepts/requires. Each node
        /// indicates the paramter data type and whether it's pass-by-reference.</summary>
        public List<Node> Parameters => _parameters;

        /// <summary>An instance of a ValueNode that's indicitive of the return type.</summary>
        public Type? ReturnType => _returnTypeNode != null ? _returnTypeNode.GetType() : null;


        /// <summary>Base constructor for child classes that represent subroutines.</summary>
        /// <param name="identifier">The identifier to be referenced to call this subroutine.</param>
        /// <param name="parameters">A list of nodes that represent the parameters the subroutine accepts/requires. Each node
        /// indicates the paramter data type and whether it's pass-by-reference.</param>
        public CallableNode(string identifier, List<Node>? parameters)
        {
            _identifier = identifier;
            _parameters = parameters == null ? new List<Node>() : parameters;
        }

        public override string ToString() => $"{_identifier} CallableNode";
    }
}
 