using SkimSkript.LexicalAnalysis.Helpers;

namespace SkimSkript.Helpers.LexicalAnalysis
{
    internal struct Lexeme
    {
        public LexemeType type;
        public int lineIndex;
        public int startColumn;
        public int endColumn;

        public Lexeme(LexemeType type, int lineIndex, int startColumn, int endColumn)
        {
            this.type = type;
            this.lineIndex = lineIndex;
            this.startColumn = startColumn;
            this.endColumn = endColumn;
        }
    }
}
