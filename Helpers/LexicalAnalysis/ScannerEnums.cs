namespace SkimSkript.LexicalAnalysis.Helpers
{
    ///<summary>Labels assigned to each lexeme by the <see cref="Scanner"/> to instruct
    ///the <see cref="Evaluator"/> on how each lexeme should be processed. </summary>
    public enum LexemeType
    {
        Alphabetic,
        Alphanumeric,
        Numeric,
        NumericDecimal,
        Textual,
        Operator,
        Delimeter,
    }

    ///<summary>Labels assigned to each ASCII char in order to determine how the 
    ///<see cref="Scanner"/> should handle each while identifying lexemes.</summary>
    public enum CharType : byte
    {
        WhiteSpaceOrPunctuation = 0,
        Letter,
        Digit,
        Decimal,
        Quote,
        Operator,
        Delimeter,
        Comment,
    }
}
