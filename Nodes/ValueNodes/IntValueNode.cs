namespace SkimSkript.Nodes
{
    /// <summary><see cref="ValueNode"/> that stores a integer while also defining the different rules 
    /// surrounding the data type's coercion.</summary>
    public class IntValueNode : ValueNode
    {
        private const int DEFAULT_VALUE = 0;
        private int _value;

        /// <summary>Creates new instance storing the value of <see cref="IntValueNode.DEFAULT_VALUE"/>.</summary>
        public IntValueNode() => _value = DEFAULT_VALUE;

        public IntValueNode(int value) => _value = value;

        public override void AssignValue(ValueNode value) => _value = value.ToInt();

        public override ValueNode Copy() => new IntValueNode(_value);

        #region Coercion/Casting Rules
        public override bool ToBool() => _value != 0;

        public override string ToString() => _value.ToString();

        public override int ToInt() => _value;

        public override float ToFloat() => _value;
        #endregion
    }
}
