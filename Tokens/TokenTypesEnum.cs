namespace SkimSkript.Tokens
{
    public enum TokenType : byte
    {
        #region Expressions
        // TODO: Get rid of magic numbers associating expression operator enums with token types. VERY BAD PRACTICE.
        Add = 0, //Marked explicitly to be associated with enum for operators in math expressions.
        SubtractUnary,
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

        #region Data Types
        // Keywords for data types
        /// <summary>
        /// "integer" or "int"
        /// </summary>
        IntegerKeyword,

        /// <summary>
        /// "floating point" or "float"
        /// </summary>
        FloatKeyword,

        /// <summary>
        /// "boolean" or "bool"
        /// </summary>
        BoolKeyword,

        /// <summary>
        /// "string"
        /// </summary>
        StringKeyword,

        /// <summary>
        /// "list"
        /// </summary>
        ListKeyword,


        #endregion

        #region Literals
        /// <summary>
        /// Interger literal.
        /// </summary>
        Integer,

        /// <summary>
        /// Floating point literal.
        /// </summary>
        Float,

        /// <summary>
        /// String literal.
        /// </summary>
        String,

        /// <summary>
        /// Boolean literal.
        /// </summary>
        True,

        /// <summary>
        /// Boolean literal.
        /// </summary>
        False,
        #endregion

        #region Collections
        /// <summary>
        /// "["
        /// </summary>
        CollectionOpen,

        /// <summary>
        /// "]"
        /// </summary>
        CollectionClose,
        #endregion

        #region Functions
        /// <summary>
        /// "def" or "define"
        /// </summary>
        FunctionDefine,

        /// <summary>
        /// "function"
        /// </summary>
        FunctionLabel,

        /// <summary>
        /// "void"
        /// </summary>
        FunctionVoid,

        /// <summary>
        /// "run" or "call"
        /// </summary>
        FunctionCallStart,
        
        /// <summary>
        /// "value of"
        /// </summary>
        FunctionCallStartExpression,

        /// <summary>
        /// "reference to" or "ref"
        /// </summary>
        PassByReference,

        /// <summary>
        /// "return" or "give back
        /// </summary>
        Return,
        #endregion

        #region Control Structures
        /// <summary>
        /// "if" or "perhaps"
        /// </summary>
        If,

        /// <summary>
        /// "else if" or "instead if" or "instead perhaps" or "alternatively if" or "elif"
        /// </summary>
        ElseIf,

        /// <summary>
        /// "else" or "otherwise"
        /// </summary>
        Else,
        
        /// <summary>
        /// "while" or "repeat code while" or "repeat while"
        /// </summary>
        WhileLoop,

        /// <summary>
        /// "then"
        /// </summary>
        Then,

        /// <summary>
        /// "repeat"
        /// </summary>
        RepeatLoop,

        /// <summary>
        /// "times"
        /// </summary>
        RepeatLoopTrail,
        #endregion

        #region Variables
        /// <summary>
        /// "declare"
        /// </summary>
        DeclarationStart,

        /// <summary>
        /// "as"
        /// </summary>
        VariableInitialize,

        /// <summary>
        /// "set"
        /// </summary>
        AssignmentStart,

        /// <summary>
        /// "to" or "="
        /// </summary>
        AssignmentOperator,
        #endregion

        #region Misc. Statements
        /// <summary>
        /// "try"
        /// </summary>
        Try,

        /// <summary>
        /// "catch"
        /// </summary>
        Catch,
        #endregion

        #region Miscellaneous
        /// <summary>
        /// function, variable, or parameter identifier
        /// </summary>
        Identifier,

        /// <summary>
        /// "{"
        /// </summary>
        BlockOpen,

        /// <summary>
        /// "}"
        /// </summary>
        BlockClose,

        /// <summary>
        /// "assert"
        /// </summary>
        Assertion,

        /// <summary>
        /// "//"
        /// </summary>
        CommentStart,

        /// <summary>
        /// Used as TokenType for EOF token container error handling
        /// </summary>
        EndOfFile,

        /// <summary>
        /// Used as TokenType for expected factor parsing error handling
        /// </summary>
        Factor,

        /// <summary>
        /// Used as TokenType for expected data type parsing error handling
        /// </summary>
        DataTypeKeyword,

        /// <summary>
        /// Used as TokenType for expected statement start parsing error handling
        /// </summary>
        StatementStartToken,

        /// <summary>
        /// Used as a placeholder value for TokenTypes not yet defined
        /// </summary>
        Undefined,
        #endregion
    }
}
