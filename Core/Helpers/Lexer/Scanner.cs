using SkimSkript.ErrorHandling;
using System.Text;

namespace SkimSkript.LexicalAnalysis.Helpers
{
    /// <summary> Processes input lines to generate lexemes for use in the evaluator. It identifies 
    /// <see cref="LexemeType"/>s (e.g., numeric, textual, operators) using character analysis and 
    /// queues them for further tokenization. </summary>
    public class Scanner
    {
        private string[] _lines;
        private int _lineIndex, _columnIndex;
        private Dictionary<char, CharType> _charTypeDict;
        private StringBuilder _lexemeStringBuilder = new StringBuilder();
        private List<(LexemeType type, string lexeme, int line)> _lexemeDataList = new List<(LexemeType, string, int)>();

        /// <summary> List where each element contains a lexeme, its type, and the line number it was found on. </summary>
        public List<(LexemeType type, string lexeme, int line)> LexemeDataList => _lexemeDataList;

        private bool IsEndOfLine => _columnIndex == _lines[_lineIndex].Length;

        private bool IsFileContentsLeft => _lineIndex != _lines.Length;

        /// <param name="linesArray"> Lines of code in the source language. </param>
        public Scanner(string[] linesArray)
        {
            _lines = linesArray;
            _charTypeDict = FillCharTypeDict();
            ScanLines();
        }

        /// <summary> Creates lexemes using characters found in the lines of the provided input file. </summary>
        private void ScanLines()
        {
            //Continue iterating line by line until all lexemes have been scanned.
            while (IsFileContentsLeft)
            {
                if (IsEndOfLine)
                {
                    _lineIndex++;
                    _columnIndex = 0;
                    continue;
                }

                //Check what type of character is next & start to the creation of an appropriate lexeme or lack there of.
                switch (PeekCharType())
                {
                    case CharType.Digit: ScanNumeric(); break;
                    case CharType.Quote: ScanTextual(); break;
                    case CharType.Letter: ScanAlphabetic(); break;
                    case CharType.Operator: ScanOperator(); break;
                    case CharType.Delimeter: ScanDelimeter(); break;
                    case CharType.Comment: SkipCommentChars(); break;                   
                    default: DequeueChar(); break; //Invalid characters and whitespace is ignored.
                }
            }
        }

        #region Lexeme Scan Methods
        /// <summary> Scan alphabetic lexeme starting with an alpha char that will represent a reserved word or identifier. </summary>
        private void ScanAlphabetic()
        {
            LexemeType lexemeType = LexemeType.Alphabetic;

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

            CacheLexeme(lexemeType);
        }

        /// <summary> Scan numeric lexeme starting with a digit char that will represent a float or int literal. </summary>
        private void ScanNumeric()
        {
            LexemeType lexemeType = LexemeType.Numeric;

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

            CacheLexeme(lexemeType);
        }

        /// <summary> Scan textual lexeme starting and ending with quotes that will represent a string literal. </summary>
        private void ScanTextual()
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
            CacheLexeme(LexemeType.Textual);
        }

        /// <summary> Scan an operator lexeme containing one or more operator symbols meant to represent a conditional, logical, or math operator. </summary>
        private void ScanOperator()
        {
            //Continue to build lexeme until something other than an operator has been scanned.
            do
            {
                _lexemeStringBuilder.Append(DequeueChar());
            }
            while (!IsEndOfLine && PeekCharType() == CharType.Operator);

            CacheLexeme(LexemeType.Operator);
        }

        /// <summary> Scans a valid delimeter composed of a single character meant to represent such. </summary>
        private void ScanDelimeter()
        {
            _lexemeStringBuilder.Append(DequeueChar());
            CacheLexeme(LexemeType.Delimeter);
        }

        /// <summary> Removes comment symbol and skips the remainder of the current line meant to represent a user comment. </summary>
        private void SkipCommentChars()
        {
            DequeueChar();
            _columnIndex = _lines[_lineIndex].Length;
        }
        #endregion

        #region Dictionary Methods
        private CharType GetCharType(char keyChar) =>
            _charTypeDict.TryGetValue(keyChar, out CharType value) ? value : CharType.WhiteSpaceOrPunctuation;

        private Dictionary<char, CharType> FillCharTypeDict()
        {            
            var charTypeDict = GetPredefinedCharTypes();
            AddRangeToCharTypeDict('a', 'z', CharType.Letter, charTypeDict);
            AddRangeToCharTypeDict('A', 'Z', CharType.Letter, charTypeDict);
            AddRangeToCharTypeDict('0', '9', CharType.Digit, charTypeDict);
            return charTypeDict;
        }

        private void AddRangeToCharTypeDict(char min, char max, CharType charType, Dictionary<char, CharType> charTypeDict)
        {
            for (char i = min; i <= max; i++)
                charTypeDict.Add(i, charType);
        }

        private Dictionary<char, CharType> GetPredefinedCharTypes() => new Dictionary<char, CharType>() // TODO: Migrate to syntax specs
            { { '.', CharType.Decimal }, { '\"', CharType.Quote }, {'#', CharType.Comment }, {'(', CharType.Delimeter},
            {')', CharType.Delimeter}, {'{', CharType.Delimeter}, {'}', CharType.Delimeter}, {'!', CharType.Operator},
            {'%', CharType.Operator}, {'&', CharType.Operator}, {'+', CharType.Operator}, {'-', CharType.Operator},
            {'*', CharType.Operator}, {'/', CharType.Operator}, {'^', CharType.Operator}, {'<', CharType.Operator},
            {'=', CharType.Operator}, {'>', CharType.Operator}, {'|', CharType.Operator} };
        #endregion

        #region Helper Methods
        private CharType GetDequeuedCharType() => GetCharType(_lines[_lineIndex][_columnIndex - 1]);

        private CharType PeekCharType() => GetCharType(_lines[_lineIndex][_columnIndex]);

        private char DequeueChar() => _lines[_lineIndex][_columnIndex++];

        private void CacheLexeme(LexemeType lexemeType)
        {
            _lexemeDataList.Add((lexemeType, _lexemeStringBuilder.ToString(), _lineIndex + 1));
            _lexemeStringBuilder.Clear();
        }
        #endregion
    }
}
