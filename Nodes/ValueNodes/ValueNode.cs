namespace SkimSkript.Nodes
{
    /// <summary>Abstract class used to dynamically store values and handle the coercion between various defined data types.</summary>
    /// <remarks>Values can be those stored during runtime in variables, function returns, or literals.</remarks>
    public abstract class ValueNode : Node
    {
        /// <summary>Returns a new instance a <see cref="ValueNode"/> containing the same value as the reciever.</summary>
        /// <remarks>The new instance will be the same child type of <see cref="ValueNode"/> as the reciever.</remarks>
        public abstract ValueNode Copy();

        /// <summary>Assigns a different value to the reciever.</summary>
        /// <param name="value"><see cref="ValueNode"/> of any child type that contains the value to copied and assigned.</param>
        /// <remarks>If the node sent is NOT the same type as the reciever then the assigned value will be coerced to match
        /// the data type stored in the reciever.</remarks>
        public abstract void AssignValue(ValueNode value);

        /// <summary>Returns an integer representation of the value stored in the reciever.</summary>
        /// <remarks>Can potentially throw a parsing error runtime!</remarks>
        public abstract int ToInt();

        /// <summary>Returns a float representation of the value stored in the reciever.</summary>
        /// <remarks>Can potentially throw a parsing error runtime!</remarks>
        public abstract float ToFloat();

        /// <summary>Returns a bool representation of the value stored in the reciever.</summary>
        public abstract bool ToBool();
        
    }
}
