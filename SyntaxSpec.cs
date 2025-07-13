using SkimSkript.Tokens;

namespace SkimSkript.Syntax
{
    /// <summary>Contains reserved words and syntax to aid primarily during lexical analysis and any error handling.</summary>
    public static class SyntaxSpec
    {
        public static readonly (string text, TokenType key)[] reservedWords =
        {
            #region Function Related
            ("def", TokenType.FunctionDefine), 
            ("define", TokenType.FunctionDefine),
            ("function", TokenType.FunctionLabel),
            ("void", TokenType.FunctionVoid),

            ("reference to",TokenType.PassByReference), 
            ("reference", TokenType.PassByReference),
            ("ref",TokenType.PassByReference),

            ("declare", TokenType.DeclarationStart),
            ("as", TokenType.VariableInitialize),

            ("set",TokenType.AssignmentStart),
            ("to",TokenType.AssignmentOperator),
            

            ("int",TokenType.IntegerKeyword),
            ("float",TokenType.FloatKeyword),
            ("bool",TokenType.BoolKeyword),
            ("string",TokenType.StringKeyword),

            ("integer", TokenType.IntegerKeyword),
            ("floating point", TokenType.FloatKeyword),
            ("boolean", TokenType.BoolKeyword),

            ("false",TokenType.False),
            ("true",TokenType.True),


            ("run",TokenType.FunctionCallStart),
            ("invoke", TokenType.FunctionCallStart),
            ("call", TokenType.FunctionCallStart),

            ("value of",TokenType.FunctionCallStartExpression),
            ("the value of",TokenType.FunctionCallStartExpression),
            ("return",TokenType.Return),
            ("give back", TokenType.Return),

            ("perhaps", TokenType.If),
            ("if",TokenType.If),

            ("else if",TokenType.ElseIf),
            ("instead if",TokenType.ElseIf),
            ("instead perhaps", TokenType.ElseIf),
            ("alternatively if",TokenType.ElseIf),
            ("elif", TokenType.ElseIf),

            ("else",TokenType.Else),
            ("otherwise", TokenType.Else),

            ("while",TokenType.WhileLoop),
            ("repeat code while",TokenType.WhileLoop),
            ("repeat while", TokenType.WhileLoop),

            ("repeat", TokenType.RepeatLoop),
            ("passes", TokenType.RepeatLoopTrail),

            ("is", TokenType.Equals),
            ("is not", TokenType.NotEquals),

            ("is at least", TokenType.GreaterThanOrEqual),
            ("greater than or equal", TokenType.GreaterThanOrEqual),

            ("exceeds", TokenType.GreaterThan),
            ("greater than", TokenType.GreaterThan),

            ("is at most", TokenType.LessThanOrEqual),
            ("less than or equal", TokenType.LessThanOrEqual),

            ("is below", TokenType.LessThan),
            ("less than", TokenType.LessThan),

            ("or", TokenType.Or),
            ("and", TokenType.And),
            ("or just", TokenType.Xor),
            ("then", TokenType.Then),

            ("plus", TokenType.Add),
            ("minus", TokenType.SubtractUnary),
            ("divided by", TokenType.Divide),
            ("times", TokenType.Multiply),
            ("multiplied by", TokenType.Multiply),
            ("modulus", TokenType.Modulus),
            ("mod", TokenType.Modulus),
            ("raised to", TokenType.Exponent),
            ("to the power of", TokenType.Exponent),
            ("assert", TokenType.Assertion),

            ("remainder after dividing by", TokenType.Modulus),

            ("assert", TokenType.Assertion),
            ("try", TokenType.Try),
            ("catch", TokenType.Catch)
        };

        public static readonly Dictionary<string, TokenType> operatorDict = new Dictionary<string, TokenType>
        {
            { "+", TokenType.Add },
            { "-", TokenType.SubtractUnary },
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
            { "[", TokenType.CollectionOpen },
            { "]", TokenType.CollectionClose },
        };

        public static readonly string[] BuiltInFunctionIdentifiers = { "print", "read", "clear", };


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
    }

    public enum BuiltInFunctionID : byte
    {
        Print,
        Read,
        Clear
    }
}
