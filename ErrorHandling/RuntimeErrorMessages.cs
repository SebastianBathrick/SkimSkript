namespace SkimSkript.ErrorHandling
{
    internal enum RuntimeErrorCode
    {
        ArgumentInvalidCount,
        ArgumentPassTypeMismatch,
        ArgumentDataTypeMismatch,
        UndefinedCallable,
    }

    internal class RuntimeErrorMessages
    {
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
