using SkimSkript.Helpers.General;
using SkimSkript.Helpers.LexicalAnalysis;
using SkimSkript.Tokens;
using System.Text;

namespace SkimSkript.ErrorHandling
{
    internal class TokenContainerException : SkimSkriptException
    {
        private const string MESSAGE = "Expected {TokenType} but got {TokenType}";

        private LexemeContainer _lexemes;
        private Token _problemToken;

        public override string Message => "[L{Line}:C{Column}] " + base.Message;

        public TokenContainerException(
            TokenType expectedType,
            Token problemToken,
            LexemeContainer lexemes,
            params object[] properties
            ) 
            : base(MESSAGE, GetProperties(properties, expectedType, problemToken, lexemes))
        {
            _problemToken = problemToken;
            _lexemes = lexemes;
        }

        private static object[] GetProperties(
            object[] properties, 
            TokenType expectedType, 
            Token problemToken, 
            LexemeContainer lexemes
            )
        {
            var propsList = properties.ToList();

            var line = lexemes.GetLexemeLineByIndex(problemToken.LexemeStartIndex);
            var column = lexemes.GetLexemeColumnByIndex(problemToken.LexemeEndIndex);

            var expectedStr = StringHelper.SplitPascalCaseManual(expectedType.ToString());
            var gotStr = StringHelper.SplitPascalCaseManual(problemToken.Type.ToString());         

            propsList.AddRange([line, column, expectedStr.ToLower(), gotStr.ToLower()]);
            return propsList.ToArray();
        }
            
        protected override bool TryGetAdditionalContext(out string message, out object[] properties)
        {
            (int start, int end) lineIndexes;
            lineIndexes.start = _lexemes.GetLexemeLineByIndex(_problemToken.LexemeStartIndex);
            lineIndexes.end = _lexemes.GetLexemeLineByIndex(_problemToken.LexemeEndIndex);

            var sb = new StringBuilder();
            var propertiesList = new List<object>();

            for (int i = lineIndexes.start; i <= lineIndexes.end; i++)
            {
                sb.Append($"Line {(i+1).ToString("D3")} | ");
                var list = _lexemes.GetLexemeIndexesOnLine(i);

                var errorUnderline = new StringBuilder(new string(' ', sb.Length));
                
                for(int j = 0; j < list.Count; j++)
                {
                    var lexemeStr = _lexemes.GetLexemeSpan(list[j]).ToString();
                    char underlineChar = ' ';

                    if (list[j] >= _problemToken.LexemeStartIndex && list[j] <= _problemToken.LexemeEndIndex)
                    {
                        sb.Append("{Placeholder}");
                        propertiesList.Add(lexemeStr);
                        underlineChar = '^';
                    }
                    else
                        sb.Append(lexemeStr.ToString());
          
                    sb.Append(' ');
                    errorUnderline.Append(new string(underlineChar, lexemeStr.Length)).Append(' ');
                }

                sb.AppendLine();
                var underline = errorUnderline.ToString();

                if (!string.IsNullOrWhiteSpace(underline))
                {
                    sb.AppendLine("{Underline}");
                    propertiesList.Add(underline);
                }         
            }

            message = sb.ToString();
            properties = propertiesList.ToArray();
            return true;
        }
    }
}
