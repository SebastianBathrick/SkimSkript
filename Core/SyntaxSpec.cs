using SkimSkript.TokenManagement;

namespace SkimSkript.Syntax
{
    /// <summary>Contains reserved words and syntax to aid primarily during lexical analysis and any error handling.</summary>
    public static class SyntaxSpec
    {
        public static readonly (string text, TokenType key)[] reservedWords =
        {
            #region Function Related
            // Function Definitions (Shorthand)
            ("def", TokenType.FunctionVoidDefine), ("def int", TokenType.FunctionIntDefine),
            ("def float", TokenType.FunctionFloatDefine), ("def bool", TokenType.FunctionFloatDefine),
            ("def string", TokenType.FunctionStringDefine),

            ("def integer", TokenType.FunctionIntDefine),
            ("def floating point", TokenType.FunctionFloatDefine), ("def boolean", TokenType.FunctionFloatDefine),

            // Define (Data Type) Function
            ("define function", TokenType.FunctionVoidDefine), ("define int function", TokenType.FunctionIntDefine),
            ("define float function", TokenType.FunctionFloatDefine), ("define bool function", TokenType.FunctionFloatDefine),
            ("define string function", TokenType.FunctionStringDefine),

            ("define integer function", TokenType.FunctionIntDefine),
            ("define floating point function", TokenType.FunctionFloatDefine), ("define boolean function", TokenType.FunctionFloatDefine),

            ("reference to",TokenType.PassByReference), ("ref",TokenType.PassByReference),
            #endregion

            #region Variable Declaration & Initialization
            ("declare", TokenType.DeclarationStart),
            ("set",TokenType.AssignmentStart),
            ("to",TokenType.AssignmentOperator),
            ("as", TokenType.VariableInitialize),
            #endregion

            ("run",TokenType.FunctionCallStart),
            ("value of",TokenType.FunctionCallStartExpression),
            ("return",TokenType.Return),
            ("give back", TokenType.Return),
            ("false",TokenType.False),
            ("true",TokenType.True),
            ("if",TokenType.If),
            ("else if",TokenType.ElseIf),
            ("instead if",TokenType.ElseIf),
            ("alternatively if",TokenType.ElseIf),
            ("elif", TokenType.ElseIf),
            ("else",TokenType.Else),
            ("otherwise", TokenType.Else),
            ("while",TokenType.WhileLoop),
            ("repeat code while",TokenType.WhileLoop),
            ("int",TokenType.IntegerKeyword),
            ("integer", TokenType.IntegerKeyword),
            ("floating point", TokenType.FloatKeyword),
            ("boolean", TokenType.BoolKeyword),
            ("float",TokenType.FloatKeyword),
            ("string",TokenType.StringKeyword),
            ("bool",TokenType.BoolKeyword),
            ("as follows", TokenType.FunctionImpliedBlock),
            ("is", TokenType.Equals),
            ("is not", TokenType.NotEquals),
            ("is at least", TokenType.GreaterThanOrEqual),
            ("exceeds", TokenType.GreaterThan),
            ("is at most", TokenType.LessThanOrEqual),
            ("is below", TokenType.LessThan),
            ("or", TokenType.Or),
            ("and", TokenType.And),
            ("or just", TokenType.Xor),
            ("then", TokenType.Then),
            ("left after dividing by", TokenType.Modulus),
            ("assert", TokenType.Assertion)
        };

        public static readonly Dictionary<string, TokenType> operatorDict = new Dictionary<string, TokenType>
        {
            { "+", TokenType.Add },
            { "-", TokenType.Subtract },
            { "*", TokenType.Multiply },
            { "/", TokenType.Divide },
            { "%", TokenType.Modulus },
            { "^", TokenType.Exponent },
            { ">", TokenType.GreaterThan },
            { "<", TokenType.LessThan },
            { "==", TokenType.Equals },
            { "!=", TokenType.NotEquals },
            { ">=", TokenType.GreaterThanOrEqual },
            { "<=", TokenType.LessThanOrEqual },
            { "||", TokenType.Or },
            { "&&", TokenType.And },
            { "^^", TokenType.Xor },
            { "=", TokenType.AssignmentOperator },
            { "=>", TokenType.FunctionImpliedBlock },
            { "[", TokenType.CollectionOpen },
            { "]", TokenType.CollectionClose },
        };

        public static readonly string[] BuiltInFunctionIdentifiers = { "print", "read", "clear", };

        #region Getters
        public static string GetReservedWordLexeme(TokenType tokenType)
        {
            foreach (var word in reservedWords)
                if (word.key == tokenType)
                    return word.text;

            return String.Empty;
        }

        public static TokenType GetDelimeterType(char lexeme)
        {
            switch (lexeme)
            {
                case '(': return TokenType.ParenthesisOpen;
                case ')': return TokenType.ParenthesisClose;
                case '{': return TokenType.BlockOpen;
                default: return TokenType.BlockClose;
            }
        }

        public static string GetDelimeterLexeme(TokenType tokenType) =>
            tokenType switch
            {
                TokenType.ParenthesisOpen => "(",
                TokenType.ParenthesisClose => ")",
                TokenType.BlockOpen => "{",
                TokenType.BlockClose => "}",
            };

        public static string GetOperatorLexeme(TokenType tokenType)
        {
            foreach (var op in operatorDict)
                if (op.Value == tokenType)
                    return op.Key;
            return String.Empty;
        }
        #endregion

        #region Methods for ErrorHandler


        public static bool IsDelimeterType(TokenType tokenType) => tokenType is TokenType.ParenthesisOpen
            or TokenType.ParenthesisClose or TokenType.BlockOpen or TokenType.BlockClose;

        public static bool IsOperatorTokenType(TokenType tokenType) => tokenType is TokenType.Add
            or TokenType.Subtract or TokenType.Multiply or TokenType.Divide or TokenType.Modulus
            or TokenType.Exponent or TokenType.GreaterThan or TokenType.LessThan or TokenType.Equals
            or TokenType.GreaterThanOrEqual or TokenType.LessThanOrEqual or TokenType.NotEquals
            or TokenType.And or TokenType.Or or TokenType.Xor;
        #endregion
    }

    public enum BuiltInFunctionID : byte
    {
        Print,
        Read,
        Clear
    }
}
