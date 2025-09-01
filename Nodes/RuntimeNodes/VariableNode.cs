namespace SkimSkript.Nodes.Runtime
{
    /// <summary>Class representing a node that stores the value of a variable as the interpreted program executes.</summary>
    public class VariableNode : Node
    {
        private Type _dataType;
        private Node _value;

        /// <summary>A node storing the value of the variable or a null reference if the variable was never initialized.</summary>
        /// <remarks>The stored data-type will be the same as what was defined in the variable declaration.</remarks>
        public Node Value => _value;

        /// <summary>Type of node associated with variable's data-type.</summary>
        /// <remarks>This can indicate a variable's type even if it is not initialized.</remarks>
        public Type DataType => _dataType;

        /// <summary>Constructor to initialize variable, and define its static data type.</summary>
        /// <param _name="value">Instance of a node that is associated with the data type.</param>
        /// <param _name="dataType">Type representing a child class of <see cref="Node"> associated with 
        /// the variable's type.</param>
        public VariableNode(Node value, Type dataType)
        {
            _dataType = dataType;
            _value = value;
        }

        public override string ToString() => _value.ToString();
    }
}
