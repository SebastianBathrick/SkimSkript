using SkimSkript.Nodes;

namespace SkimSkript.ErrorHandling
{
    internal class RuntimeException : SkimSkriptException
    {
        private StatementNode? _statementNode;

        public StatementNode? StatementNode => _statementNode;

        public RuntimeException(string message, StatementNode? statementNode = null, params object[] properties)
            : base(message, properties)
        {
            _statementNode = statementNode;
        }

        public RuntimeException(string message, params object[] properties)
            : base(message, properties) { }

        public RuntimeException SetStatement(StatementNode statementNode)
        {
            _statementNode = statementNode;
            return this;
        }
    }
}
