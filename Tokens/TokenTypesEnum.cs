namespace SkimSkript.TokenManagement
{
    /// <summary>Enum that serves as a label to determine how the token should be treated during parsing.</summary>
    public enum TokenType : byte
    {
        Add = 0, //Marked explicitly to be associated with enum for operators in math expressions.
        Subtract,
        Multiply,
        Divide,
        Modulus,
        Exponent,

        Equals = 6, //Marked explicitly to be associated with enum for operators in conditional expressions.
        NotEquals,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,

        And,
        Or,
        Xor,

        IntegerKeyword,
        FloatKeyword,
        BoolKeyword,
        StringKeyword,

        Integer,
        Float,
        String,       
        True,
        False,

        FunctionIntDefine,
        FunctionFloatDefine,
        FunctionBoolDefine,
        FunctionStringDefine,
        FunctionVoidDefine,
        FunctionImpliedBlock,
        FunctionCallStart,
        FunctionCallStartExpression,
        FunctionArgPrepositionSingle,
        FunctionArgPrepositionMulti,
        FunctionArgPostpositionNone,
        FunctionArgConjuctive,
        PassByReference,
        Return,

        If,
        ElseIf,
        Else,
        WhileLoop,
        Then,

        BlockOpen,
        BlockClose,
        ParenthesisOpen,
        ParenthesisClose,

        DeclarationStart,
        AssignmentStart,
        VariableInitialize,
        AssignmentOperator,

        PartialPhrase,
        Identifier,

        CommentStart,

        Undefined,
    }
}
