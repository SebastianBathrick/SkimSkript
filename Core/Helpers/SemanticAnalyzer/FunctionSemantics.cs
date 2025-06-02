
namespace SkimSkript.Semantics.Helper
{
    /// <summary> Class that represents a container for a single function's data to be used in semantic analyisis. </summary>
    public class FunctionSemantics
    {
        private string _identifier;        
        private int _parameterCount;
        private bool _isVariadic;
        private Type? _returnType;

        public string Identifier => _identifier;

        public int ParameterCount => _parameterCount;

        /// <summary> Whether or not this object contains a return data type. </summary>
        public bool IsVoid => _returnType == null;

        /// <summary> Whether or not the function this represents accepts any number of arguments greater than 1. </summary>
        public bool IsVariadic => _isVariadic;

        public FunctionSemantics(string identifier)
        {
            _identifier = identifier;
        }

        public FunctionSemantics(string identifier, int parameterCount, bool isVariadic, Type? returnType) : this(identifier)
        {
            _parameterCount = parameterCount;
            _isVariadic = isVariadic;
            _returnType = returnType;
        }
    }
}
