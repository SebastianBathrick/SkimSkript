using System;
using System.Collections.Generic;

namespace SkimSkript.Nodes.Runtime
{
    /// <summary>Class representing a node that stores the value of a variable as the interpreted program executes.</summary>
    public class VariableNode : Node
    {
        private int _referenceCount;
        private Type _dataType;
        private Node? _value;

        /// <summary>A node storing the value of the variable or a null reference if the variable was never initialized.</summary>
        /// <remarks>The stored data-type will be the same as what was defined in the variable declaration.</remarks>
        public Node? Value => _value;

        /// <summary>Type of node associated with variable's data-type.</summary>
        /// <remarks>This can indicate a variable's type even if it is not initialized.</remarks>
        public Type DataType => _dataType;

        /// <summary>Constructor to initialize variable, and define its static data type.</summary>
        /// <param name="value">Instance of a node that is associated with the data type.</param>
        /// <param name="dataType">Type representing a child class of <see cref="Node"> associated with 
        /// the variable's type.</param>
        public VariableNode(Node? value, Type dataType)
        {          
            _dataType = dataType;
            _value = value;
            _referenceCount = 0;
        }

        /// <summary>Replaces the stored node containing the variable's value with a node sent as an argument.</summary>
        public void AssignValue(Node node) => _value = node;

        /// <summary>Adds to a count indicating how many functions reference this specific node.</summary>
        public void AddToFunctionReferenceCount() => _referenceCount++; 
        
        public override string ToString() => _value.ToString();            
    }
}
