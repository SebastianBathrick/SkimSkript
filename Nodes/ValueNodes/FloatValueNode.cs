namespace SkimSkript.Nodes
{
    /// <summary><see cref="ValueNode"/> that stores a float while also defining the different rules 
    /// surrounding the data type's coercion.</summary>
    public class FloatValueNode : ValueNode
    {
        private const float DEFAULT_VALUE = 0.0f;
        private float _value;

        public float Value => _value;

        public FloatValueNode() => _value = DEFAULT_VALUE;

        public FloatValueNode(float value) => _value = value;

        public override void AssignValue(ValueNode value) => _value = value.ToFloat();

        public override ValueNode Copy() => new FloatValueNode(_value);

        #region Coercion/Casting Rules
        public override bool ToBool() => _value != 0;

        public override string ToString() => _value.ToString();

        public override int ToInt() => (int)_value;

        public override float ToFloat() => _value;
        #endregion
    }
}
