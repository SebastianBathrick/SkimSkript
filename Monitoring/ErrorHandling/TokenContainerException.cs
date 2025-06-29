
using SkimSkript.CoreHelpers.LexicalAnalysis;
using SkimSkript.TokenManagement.Tokens;
using System.Text;

namespace SkimSkript.Monitoring.ErrorHandling
{
    internal enum TokenContainerError
    {
        TokenMismatch,
        EndOfFile
    }

    internal class TokenContainerException : SkimSkriptException
    {
        private static readonly string[] _messages =
        {
            "Expected {TokenType} but got {TokenType} instead"
        };

        private LexemeContainer _lexemes;
        private Token _problemToken;

        public TokenContainerException(
            TokenContainerError errorKey, 
            Token problemToken,
            LexemeContainer lexemes,
            params object[] properties
            ) 
            : base(
                  errorKey,
                  _messages, 
                  lexemes.GetLexemeLineByIndex(problemToken.LexemeStartIndex) + 1,
                  lexemes.GetLexemeColumnByIndex(problemToken.LexemeEndIndex) + 1,
                  properties
                  )
        {
            _problemToken = problemToken;
            _lexemes = lexemes;
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
