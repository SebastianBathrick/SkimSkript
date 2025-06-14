namespace SkimSkript.Nodes.ValueNodes
{
    /// <summary><see cref="ValueNode"/> that stores a boolean while also defining the different rules 
    /// surrounding the data type's coercion.</summary>
    public class BoolValueNode : ValueNode
    {
        private const bool DEFAULT_VALUE = false;
        private bool _value;

        public bool Value => _value;

        public BoolValueNode() => _value = DEFAULT_VALUE;

        public BoolValueNode(bool value) => _value = value;

        public override void AssignValue(ValueNode value) => _value = value.ToBool();


        public override ValueNode Copy() => new BoolValueNode(_value);


        #region Coercion/Casting Rules
        public override bool ToBool() => Value;

        public override float ToFloat() => Value ? 1f : 0f;

        public override int ToInt() => Value ? 1 : 0;

        public override string ToString() => Value ? "true" : "false";
        #endregion
    }
}
