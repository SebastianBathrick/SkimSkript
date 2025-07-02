namespace SkimSkript.Tokens
{
    /// <summary>Enum that serves as a label to determine how the token should be treated during parsing.</summary>
    public enum TokenType : byte
    {
        #region Expressions
        // TODO: Get rid of magic numbers associating expression operator enums with token types. VERY BAD PRACTICE.
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

        ParenthesisOpen,
        ParenthesisClose,
        #endregion

        #region Value Data Types
        // Keywords for data types
        IntegerKeyword,
        FloatKeyword,
        BoolKeyword,
        StringKeyword,

        // Literals
        Integer,
        Float,
        String,
        True,
        False,
        #endregion

        #region Collections Data Types
        // Keywords for data types
        ListKeyword,

        // Used for list literals & the subscript operator
        CollectionOpen,
        CollectionClose,
        #endregion

        #region Functions
        // Function definitions and return types
        FunctionIntDefine,
        FunctionFloatDefine,
        FunctionBoolDefine,
        FunctionStringDefine,
        FunctionVoidDefine,

        FunctionImpliedBlock,
        FunctionCallStart,
        FunctionCallStartExpression,
        PassByReference,
        Return,
        #endregion

        #region Control Structures
        If,
        ElseIf,
        Else,
        WhileLoop,
        Then,
        RepeatLoop,
        RepeatLoopTrail,
        #endregion

        #region Variables
        DeclarationStart,
        AssignmentStart,
        VariableInitialize,
        AssignmentOperator,
        #endregion

        #region Miscellaneous
        Identifier,
        BlockOpen,
        BlockClose,
        Assertion,
        CommentStart,

        // Error Checking
        EndOfFile,
        Factor,
        DataType,
        StatementStartToken,

        // Utility TokenTypes (used for lexing)
        PartialPhrase,
        Undefined,

        #endregion
    }
}
