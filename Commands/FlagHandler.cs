namespace SkimSkript.Commands
{
    internal class FlagHandler
    {
        private List<Flag> _flags = new List<Flag>();

        public FlagHandler AddFlag(Flag flag)
        {
            _flags.Add(flag);
            return this;
        }

        public bool IsFlag(string argument) => argument.StartsWith(Flag.FlagPrefix);

        public bool TryExecuteFlag(int flagIndex, string[] arguments, out int endIndex)
        {
            
           string flagName = arguments[flagIndex].Trim();
           Flag? selectedFlag = null;

           foreach(var flag in _flags)
           {
                if(flag.PrefixedName == flagName)
                {
                    selectedFlag = flag;
                    break;
                }
           }

           if(selectedFlag == null)
                throw new ArgumentException($"Flag {flagName} not found"); // Dev error

           if(selectedFlag.IsParameters)
            {
                if (flagIndex + 1 == arguments.Length)
                    return false; // User error
            }


        }
    }

    internal class Flag
    {
        private const int DEFAULT_PARAMETER_COUNT = 0;
        private const bool DEFAULT_IS_VARIADIC = false;
        private const string FLAG_PREFIX = "--";

        #region Data Members
        private string _name = string.Empty;
        private int _parameterCount;
        private bool _isVariadic = DEFAULT_IS_VARIADIC;
        private Action<List<string>> action = (parameters) => throw new NotImplementedException();
        #endregion

        #region Properties
        public static string FlagPrefix => FLAG_PREFIX;

        public string PrefixedName => FlagPrefix + _name;

        public int ParameterCount => _parameterCount;

        public bool IsParameters => _parameterCount != DEFAULT_PARAMETER_COUNT;

        public bool IsVariadicParameters  => _isVariadic;
        #endregion

        #region Methods
        public Flag SetName(string name)
        {
            _name = name;
            return this;
        }

        public Flag SetParameterCount(int parameterCount)
        {
            _parameterCount = parameterCount;
            return this;
        }

        public Flag SetVariadic(bool isVariadic)
        {
            _isVariadic = isVariadic;
            return this;
        }

        public Flag SetAction(Action<List<string>> action)
        {
            this.action = action;
            return this;
        }
        #endregion
    }
}
