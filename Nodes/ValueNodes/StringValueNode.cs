using SkimSkript.ErrorHandling;

namespace SkimSkript.Nodes.ValueNodes;

/// <summary><see cref="ValueNode"/> that stores a string while also defining the different rules 
/// surrounding the data type's coercion.</summary>
public class StringValueNode : ValueNode
{
    private const string DEFAULT_VALUE = "";
    private string _value;

    public StringValueNode() => _value = DEFAULT_VALUE;

    public StringValueNode(in string value) => _value = value;

    public override void AssignValue(ValueNode value) => _value = value.ToString();

    public override ValueNode Copy() => new StringValueNode(in _value);

    #region Coercion/Casting Rules
    public override bool ToBool() => _value.Length != 0;

    public override float ToFloat() => float.TryParse(_value, out var result)
            ? result
            : throw new RuntimeException($"\"{_value}\" string cannot be converted to floating-point");

    public override int ToInt() => int.TryParse(_value, out var result)
            ? result
            : throw new RuntimeException($"\"{_value}\" string cannot be converted to integer");

    public override string ToString() => _value;
    #endregion
}
