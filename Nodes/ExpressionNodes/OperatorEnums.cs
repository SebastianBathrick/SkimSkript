using SkimSkript.TokenManagement;

namespace SkimSkript.Nodes
{
    /// <summary>Enum representing comparison operators for evaluating relational conditions, 
    /// such as equality and inequality.</summary>
    public enum ComparisonOperator : byte
    {
        Equals = TokenType.Equals,
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }

    /// <summary>Enum representing logical operators for combining boolean expressions.</summary>
    public enum LogicOperator : byte
    {
        And = TokenType.And,
        Or,
        Xor
    }

    /// <summary>Enum representing mathematical operators for performing arithmetic operations.</summary>
    public enum MathOperator : byte
    {
        Add = TokenType.Add,
        Subtract,
        Multiply,
        Divide,
        Modulus,
        Exponent
    }
}
