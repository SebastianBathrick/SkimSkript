namespace SkimSkript.Nodes
{
    /// <summary>Class representing a single user-defined function parameter meant to provide the necessary
    /// data to setup the parameter during a runtime function call.</summary>
    public class ParameterNode : Node
    {
        private bool _isReference;     
        private string _identifier;
        private Node _valueSource; // TODO: Refactor to reduce complexity through removing _valueSource.

        /// <summary>Indicates whether the parameter is meant to be passed by reference or value.</summary>
        public bool IsReference => _isReference; //To later be used in semantic analysis...

        /// <summary>Identifier used to reference the parameter within its function scope.</summary>
        public string Identifier => _identifier;

        /// <summary>Node containing the value and type associated with the parameter declaration.</summary>
        public Node ValueSource => _valueSource;

        /// <param name="valueSource">Node containing the value and type associated with the parameter declaration.</param>
        /// <param name="isReference">Indicates whether the parameter is meant to be passed by reference or value.</param>
        /// <param name="identifier">Identifier used to reference the parameter within its function scope.</param>
        public ParameterNode(Node valueSource, bool isReference, string? identifier)
        {
            _valueSource = valueSource;
            _isReference = isReference;
            _identifier = identifier != null ? identifier : string.Empty;
        }

        public override string ToString() => (_isReference ? "ref " : String.Empty) + _valueSource.ToString();
    }
}
