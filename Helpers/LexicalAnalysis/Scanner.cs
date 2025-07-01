using SkimSkript.Helpers.LexicalAnalysis;
using SkimSkript.ErrorHandling;
using System.Text;

namespace SkimSkript.LexicalAnalysis.Helpers
{
    /// <summary> Processes input lines to generate lexemes for use in the evaluator. It identifies 
    /// <see cref="LexemeType"/>s (e.g., numeric, textual, operators) using character analysis and 
    /// queues them for further tokenization. </summary>
    internal class Scanner
    {
        private string[]? _lines;
        private int _lineIndex, _columnIndex;
        private Dictionary<char, CharType> _charTypeDict = FillCharTypeDict();
        private StringBuilder _lexemeStringBuilder = new StringBuilder();


        private bool IsEndOfLine => _columnIndex == Lines[_lineIndex].Length;

        private bool IsFileContentsLeft => _lineIndex != Lines.Length;

        private string[] Lines => _lines!;

        /// <param name="linesArray"> Lines of code in the source language. </param>
        public LexemeContainer CreateLexemes(string[] linesArray)
        {
            _lines = linesArray;
            return ScanLines();
        }

        /// <summary> Creates lexemes using characters found in the lines of the provided input file. </summary>
        private LexemeContainer ScanLines()
        {
            var lexemeContainer = new LexemeContainer(Lines);

            //Continue iterating line by line until all lexemes have been scanned.
            while (IsFileContentsLeft)
            {
                if (IsEndOfLine)
                {
                    _lineIndex++;
                    _columnIndex = 0;
                    continue;
                }

                var startColumnIndex = _columnIndex; //Store the column index where the lexeme starts.
                LexemeType lexemeType;

                //Check what type of character is next & start to the creation of an appropriate lexeme or lack there of.
                switch (PeekCharType())
                {
                    case CharType.Digit: lexemeType = ScanNumeric(); break;
                    case CharType.Quote: lexemeType = ScanTextual(); break;
                    case CharType.Letter: lexemeType = ScanAlphabetic(); break;
                    case CharType.Operator: lexemeType = ScanOperator(); break;
                    case CharType.Delimeter: lexemeType = ScanDelimeter(); break;
                    case CharType.Comment: SkipCommentChars(); continue;                   
                    default: DequeueChar(); continue; //Invalid characters and whitespace is ignored.
                }

                lexemeContainer.AddLexeme(lexemeType, _lineIndex, startColumnIndex, _columnIndex - 1);
            }

            return lexemeContainer;
        }

        #region Lexeme Scan Methods
        /// <summary> Scan alphabetic lexeme starting with an alpha char that will represent a reserved word or identifier. </summary>
        private LexemeType ScanAlphabetic()
        {
            var lexemeType = LexemeType.Alphabetic;

            //Continue to build the lexeme until something other than an alpha or digit char has been scanned.
            do
            {
                _lexemeStringBuilder.Append(DequeueChar());

                if (IsEndOfLine)
                    break;

                if (PeekCharType() == CharType.Letter)
                    continue;

                //If the character is anything other than a digit exit the loop, as identifiers can 
                //contain digits after the first char.
                if (PeekCharType() != CharType.Digit)
                    break;

                lexemeType = LexemeType.Alphanumeric;
            }
            while (true);

            return lexemeType;
        }

        /// <summary> Scan numeric lexeme starting with a digit char that will represent a float or int literal. </summary>
        private LexemeType ScanNumeric()
        {
            var lexemeType = LexemeType.Numeric;

            //Continue to build lexeme until something other than a digit or a single decimal has been scanned.
            do
            {
                _lexemeStringBuilder.Append(DequeueChar());

                if (IsEndOfLine)
                    break;

                if (PeekCharType() == CharType.Digit)
                    continue;

                //If there's a character other than a decimal OR the char is a decimal but
                //this number already contains a decimal.
                if (PeekCharType() != CharType.Decimal || lexemeType == LexemeType.NumericDecimal)
                    break;

                lexemeType = LexemeType.NumericDecimal;
            } while (true);

            //If numeric ends with a decimal then that means the decimal is not a part of the lexeme.
            if (lexemeType == LexemeType.NumericDecimal && GetDequeuedCharType() == CharType.Decimal)
            {
                //Remove the decimal from the lexeme and change the type back to numeric.
                _lexemeStringBuilder.Remove(_lexemeStringBuilder.Length - 1, 1);
                lexemeType = LexemeType.Numeric;
            }

            return lexemeType;
        }

        /// <summary> Scan textual lexeme starting and ending with quotes that will represent a string literal. </summary>
        private LexemeType ScanTextual()
        {
            DequeueChar(); //The starting quote won't be included to make using the literal in the future easier.

            //Continue to build lexeme until a closing quote has been scanned.
            while (PeekCharType() != CharType.Quote)
            {
                _lexemeStringBuilder.Append(DequeueChar());

                if (IsEndOfLine)
                    throw new LexicalError($"String literal missing a closing quote on same line.", _lineIndex + 1, _lines[_lineIndex ]);
            }
                
            DequeueChar(); //The closing quote is not included for the same reason mentioned above.
            return LexemeType.Textual;
        }

        /// <summary> Scan an operator lexeme containing one or more operator symbols meant to represent a conditional, logical, or math operator. </summary>
        private LexemeType ScanOperator()
        {
            //Continue to build lexeme until something other than an operator has been scanned.
            do
            {
                _lexemeStringBuilder.Append(DequeueChar());
            }
            while (!IsEndOfLine && PeekCharType() == CharType.Operator);

            return LexemeType.Operator;
        }

        /// <summary> Scans a valid delimeter composed of a single character meant to represent such. </summary>
        private LexemeType ScanDelimeter()
        {
            _lexemeStringBuilder.Append(DequeueChar());
            return LexemeType.Delimeter;
        }

        /// <summary> Removes comment symbol and skips the remainder of the current line meant to represent a user comment. </summary>
        private void SkipCommentChars()
        {
            DequeueChar();
            _columnIndex = Lines[_lineIndex].Length;
        }
        #endregion

        #region Helper Methods
        private CharType GetDequeuedCharType() => GetCharType(Lines[_lineIndex][_columnIndex - 1]);

        private CharType PeekCharType() => GetCharType(Lines[_lineIndex][_columnIndex]);

        private char DequeueChar() => Lines[_lineIndex][_columnIndex++];
        #endregion

        #region Dictionary Methods
        private CharType GetCharType(char keyChar) =>
            _charTypeDict.TryGetValue(keyChar, out CharType value) ? value : CharType.WhiteSpaceOrPunctuation;

        private static Dictionary<char, CharType> FillCharTypeDict()
        {
            var charTypeDict = GetPredefinedCharTypes();
            AddRangeToCharTypeDict('a', 'z', CharType.Letter, charTypeDict);
            AddRangeToCharTypeDict('A', 'Z', CharType.Letter, charTypeDict);
            AddRangeToCharTypeDict('0', '9', CharType.Digit, charTypeDict);
            return charTypeDict;
        }

        private static void AddRangeToCharTypeDict(char min, char max, CharType charType, Dictionary<char, CharType> charTypeDict)
        {
            for (char i = min; i <= max; i++)
                charTypeDict.Add(i, charType);
        }

        private static Dictionary<char, CharType> GetPredefinedCharTypes() => new Dictionary<char, CharType>() // TODO: Migrate to syntax specs
            { { '.', CharType.Decimal }, { '\"', CharType.Quote }, {'#', CharType.Comment }, {'(', CharType.Delimeter},
            {')', CharType.Delimeter}, {'{', CharType.Delimeter}, {'}', CharType.Delimeter}, {'!', CharType.Operator},
            {'%', CharType.Operator}, {'&', CharType.Operator}, {'+', CharType.Operator}, {'-', CharType.Operator},
            {'*', CharType.Operator}, {'/', CharType.Operator}, {'^', CharType.Operator}, {'<', CharType.Operator},
            {'=', CharType.Operator}, {'>', CharType.Operator}, {'|', CharType.Operator} };
        #endregion
    }
}
