using SkimSkript.Nodes;

namespace SkimSkript.ErrorHandling
{


    internal class RuntimeError : Exception
    {   
        private readonly RuntimeErrorCode _errorCode;
        private readonly object[] _properties;
        private StatementNode? _statement;

        public StatementNode? Statement => _statement;

        public RuntimeErrorCode ErrorCode => _errorCode;

        public object[] Properties => _properties;

        public RuntimeError(RuntimeErrorCode errorCode, params object[] properties) 
        {
            _properties = properties;
            _errorCode = errorCode;
        }
         
        public void AttachStatement(StatementNode statement) => _statement = statement;
    }
}
