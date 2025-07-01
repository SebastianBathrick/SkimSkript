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

        public int GetLexemeLineByIndex(int lexemeIndex) => _lexemeList[lexemeIndex].lineIndex;

        public int GetLexemeColumnByIndex(int lexemeIndex) => _lexemeList[lexemeIndex].startColumn;

        public string GetLexemesAsString(int startIndex, int endIndex)
        {
            var sb = new StringBuilder();

            for (int i = startIndex; i <= endIndex; i++)
                sb.Append(GetLexemeSpan(i));

            return sb.ToString();
        }

        public string GetLexemesAsStringLines(int startIndex, int endIndex)
        {
            var sb = new StringBuilder();

            if (startIndex != 0)
            {
                sb.Append(GetLineBreaks(startIndex - 1, startIndex));

                if (_lexemeList[startIndex].lineIndex != _lexemeList[startIndex - 1].lineIndex)
                {
                    sb.Append((_lexemeList[startIndex].lineIndex + 1).ToString("D3") + "| ");
                    sb.Append(new string(' ', _lexemeList[startIndex].startColumn));
                }
            }
            else
                sb.Append((_lexemeList[startIndex].lineIndex + 1).ToString("D3") + "| ");

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (i - 1 > 0 && _lexemeList[i - 1].lineIndex != _lexemeList[i].lineIndex && i != startIndex)
                    sb.Append((_lexemeList[i].lineIndex + 1).ToString("D3") + "| ");

                sb.Append(GetLexemeSpan(i).ToString());

                if (i != endIndex)
                    sb.Append(GetLineBreaks(i, i + 1));

                if (i != _lexemeList.Count - 1 && _lexemeList[i + 1].lineIndex == _lexemeList[i].lineIndex)
                    if (_lexemeList[i + 1].startColumn - _lexemeList[i].endColumn > 1)
                        sb.Append(' ');
            }

            return sb.ToString();
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

        /// <param name="upperMostIndex">Index of line closest to the top of the file.</param>
        /// <param name="lowerMostIndex">Index of line closest to the bottom of the file.</param>
        private string GetLineBreaks(int upperMostIndex, int lowerMostIndex)
        {
            var lineBreakCount = _lexemeList[lowerMostIndex].lineIndex - _lexemeList[upperMostIndex].lineIndex;

            if (lineBreakCount == 0)
                return string.Empty;

            var line = _lexemeList[upperMostIndex].lineIndex + 2;
            var sb = new StringBuilder("\n");

            for (int i = 0; i < lineBreakCount - 1; i++)
            {
                sb.AppendLine($"{line++:D3}| ");
            }

            return sb.ToString();
        }

        public List<int> GetLexemeIndexesOnLine(int lineIndex)
        {
            var lines = new List<int>();

            for(int i = 0; i < _lexemeList.Count; i++)
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