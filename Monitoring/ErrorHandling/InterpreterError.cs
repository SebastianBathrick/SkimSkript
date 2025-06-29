using SkimSkript.Nodes;

namespace SkimSkript.ErrorHandling
{
    internal enum RuntimeErrorCode
    {
        ArgumentInvalidCount,
        ArgumentPassTypeMismatch,
        ArgumentDataTypeMismatch,
        UndefinedCallable,
    }

    internal class InterpreterError : Exception
    {   
        private readonly RuntimeErrorCode _errorCode;
        private readonly object[] _properties;
        private StatementNode? _statement;

        public StatementNode? Statement => _statement;

        public RuntimeErrorCode ErrorCode => _errorCode;

        public object[] Properties => _properties;

        public InterpreterError(RuntimeErrorCode errorCode, params object[] properties) 
        {
            _properties = properties;
            _errorCode = errorCode;
        }
         
        public void AttachStatement(StatementNode statement) => _statement = statement;



        private readonly Dictionary<RuntimeErrorCode, string> _messages = new()
        {
            {
                RuntimeErrorCode.ArgumentInvalidCount,
                "Call contained {ArgCount} arguments but function definition has {ParamCount} parameters"
            },
            {
                RuntimeErrorCode.ArgumentPassTypeMismatch,
                "Call mismatches argument and parameter pass-by mechanism(s)"
            },
            {
                RuntimeErrorCode.ArgumentDataTypeMismatch,
                "Call mismatches argument and parameter data type(s)"
            },
        };
    }
}
