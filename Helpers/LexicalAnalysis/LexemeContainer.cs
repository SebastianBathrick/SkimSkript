using SkimSkript.LexicalAnalysis.Helpers;
using System.Text;

namespace SkimSkript.Helpers.LexicalAnalysis
{
    internal class LexemeContainer
    {
        private int _currLexemeIndex = 0;
        private int _currentCharIndex = 0;

        private List<Lexeme> _lexemeList = new();
        private string[] _sourceCode;

        public bool HasLexemes => _currLexemeIndex < _lexemeList.Count - 1;

        public int Count => _lexemeList.Count - _currLexemeIndex + 1;

        public int CurrentLexemeIndex => _currLexemeIndex;

        public LexemeContainer(string[] sourceCode) => _sourceCode = sourceCode;

        public bool TryPeekNextLexeme(out LexemeType? lexemeType)
        {
            if (_currLexemeIndex + 1 == _lexemeList.Count)
            {
                lexemeType = null;
                return false;
            }

            lexemeType = _lexemeList[_currLexemeIndex + 1].type;
            return true;
        }

        public bool TryMoveToNextLexeme()
        {
            if (!HasLexemes)
                return false;
            MoveToNextLexeme();
            return true;
        }

        public int GetLineIndexByLexeme(int lexemeIndex) => _lexemeList[lexemeIndex].lineIndex;

        public int GetColumnIndexByLexeme(int lexemeIndex) => _lexemeList[lexemeIndex].startColumn;

        /// <remarks>Used for parsing data type literals.</remarks>
        public string GetLexemesAsString(int startIndex, int endIndex)
        {
            var sb = new StringBuilder();

            for (int i = startIndex; i <= endIndex; i++)
                sb.Append(GetLexemeSpan(i));

            return sb.ToString();
        }

        /// <summary>Returns an array of lexemes in the specified range.</summary>
        /// <remarks>Used for printing statements</remarks>
        public string[] GetLabeledLines(int startIndex, int endIndex)
        {
            var sb = new StringBuilder();
            var lexemesList = new List<string>();
            var lastLineIndex = _lexemeList[startIndex].lineIndex;

            sb.Append($"Line {(lastLineIndex + 1).ToString("D3")}| ");

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (lastLineIndex != _lexemeList[i].lineIndex)
                {
                    var line = sb.ToString();
                    sb.Clear();
                    lexemesList.Add(line);
                    sb.Append($"Line {(lastLineIndex + 1).ToString("D3")}| ");
                }

                sb.Append(GetLexemeSpan(i)).Append(' ');
            }

            lexemesList.Add(sb.ToString());

            return lexemesList.ToArray();
        }

        public void BackTrackLexemes(int lexemeCount)
        {
            _currLexemeIndex -= lexemeCount;
            _currentCharIndex = _lexemeList[_currLexemeIndex].startColumn;
        }

        public void AddLexeme(LexemeType type, int lineIndex, int startColumn, int endColumn) =>
            _lexemeList.Add(new Lexeme(type, lineIndex, startColumn, endColumn));

        public ReadOnlySpan<char> GetLexemeSpan()
        {
            var lexeme = _lexemeList[_currLexemeIndex];
            return _sourceCode[lexeme.lineIndex].AsSpan(lexeme.startColumn, lexeme.endColumn - lexeme.startColumn + 1);
        }

        public ReadOnlySpan<char> GetLexemeSpan(int lexemeIndex)
        {
            var lexeme = _lexemeList[lexemeIndex];
            return _sourceCode[lexeme.lineIndex].AsSpan(lexeme.startColumn, lexeme.endColumn - lexeme.startColumn + 1);
        }

        public void MoveToNextLexeme()
        {
            _currLexemeIndex++;
            _currentCharIndex = _lexemeList[_currLexemeIndex].startColumn;
        }

        public LexemeType GetLexemeType() => _lexemeList[_currLexemeIndex].type;

        public bool TryGetLexemeChar(out char lexemeChar)
        {
            if (_currentCharIndex > _lexemeList[_currLexemeIndex].endColumn)
            {
                lexemeChar = '`';
                return false;
            }

            lexemeChar = _sourceCode[_lexemeList[_currLexemeIndex].lineIndex][_currentCharIndex++];
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            for (int i = 0; i < _lexemeList.Count; i++)
                sb.AppendLine($"{i}: {_lexemeList[i].type} - {GetLexemeSpan(i).ToString()}");

            return sb.ToString();
        }

        public List<int> GetLexemeIndexesOnLine(int lineIndex)
        {
            var lines = new List<int>();

            for (int i = 0; i < _lexemeList.Count; i++)
                if (_lexemeList[i].lineIndex < lineIndex)
                    continue;
                else if (_lexemeList[i].lineIndex == lineIndex)
                    lines.Add(i);
                else
                    return lines;

            return lines;
        }
    }
}