namespace SkimSkript.Nodes
{
    /// <summary>Class representing a single user-defined function parameter meant to provide the necessary
    /// data to setup the parameter during a runtime function call.</summary>
    public class ParameterNode : Node
    {
        private bool _isReference;
        private Node _identifierNode;
        private Type _dataType;

        /// <summary>Indicates whether the parameter is meant to be passed by reference or value.</summary>
        public bool IsReference => _isReference;

        /// <summary>IdentfierNode used to reference the parameter within its function scope.</summary>
        public Node IdentifierNode => _identifierNode;

        /// <summary>Data type of the declared parameter.</summary>
        public Type DataType => _dataType;

        public ParameterNode(bool isReference, Type dataType, Node identifierNode)
        {
            _isReference = isReference;
            _identifierNode = identifierNode;
            _dataType = dataType;
        }

        public override string ToString() => (_isReference ? "ref " : String.Empty) + IdentifierNode.ToString();
    }
}
