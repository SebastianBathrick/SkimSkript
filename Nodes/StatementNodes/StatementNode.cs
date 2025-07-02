using SkimSkript.Helpers.LexicalAnalysis;
using System.Text;

namespace SkimSkript.Nodes
{

    /// <summary>Abstract class that is meant to be the parent of all nodes that represent 
    /// statements supported by the source language.</summary>
    internal abstract class StatementNode : Node
    {
        private int _lexemeStartIndex, _lexemeEndIndex;
        private bool _isEndLexeme = false;

        public bool IsEndLexeme => _isEndLexeme;

        public StatementNode SetLexemeStartIndex(int lexemeStartIndex)
        {
            _lexemeStartIndex = lexemeStartIndex;
            return this;
        }

        public StatementNode SetLexemeEndIndex(int lexemeEndIndex)
        {
            _lexemeEndIndex = lexemeEndIndex;
            _isEndLexeme = true;
            return this;
        }

        public static string ToString(StatementNode statementNode, LexemeContainer lexemeContainer)
        {
            string[] lines = lexemeContainer
                .GetLabeledLines(
                    statementNode._lexemeStartIndex,
                    statementNode._lexemeEndIndex
                );

            var sb = new StringBuilder();

            foreach (var line in lines)
                sb.AppendLine(line);

            return sb.ToString();
        }
    }
}