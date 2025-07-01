using SkimSkript.Nodes;

namespace SkimSkript.Monitoring.ErrorHandling
{
    internal class RuntimeException : Exception
    {
        private StatementNode? _statementNode;
        private object[] _properties = [];

        public StatementNode? StatementNode => _statementNode;
        public object[] Properties => _properties;

        public RuntimeException(string message, StatementNode? statementNode = null, params object[] properties) : base(message)
        {
            _statementNode = statementNode;
            _properties = properties;
        }

        public RuntimeException SetStatement(StatementNode statementNode)
        {           
            _statementNode = statementNode;
            return this;
        }

        public RuntimeException SetProperties(params object[] properties)
        {
            _properties = properties;
            return this;
        }
    }
}
