namespace Flags
{
    /// <summary>
    /// Represents a command line flag.
    /// </summary>
    /// <remarks>SetName, <see cref="SetShortName"/>, <see cref="SetParameterCount"/>, and <see cref="SetAction"/>
    /// must be called before executing the flag using <see cref="FlagHandler.ExecuteFlag"/>.</remarks>
    internal class Flag
    {
        #region Constants
        private const int DEFAULT_MIN_PARAM_COUNT = 0;
        private const bool DEFAULT_IS_VARIADIC = false;
        private const string FLAG_PREFIX = "--";
        private const string SHORT_FLAG_PREFIX = "-";
        private const string DEFAULT_DESC = "No description";
        #endregion

        #region Fields
        private string _name = string.Empty;
        private string _shortName = string.Empty;
        private string _desc = DEFAULT_DESC;

        private int _minParameterCount;
        private bool _isVariadic = DEFAULT_IS_VARIADIC;
        private Action<List<string>?> _action = (parameters) => throw new NotImplementedException();
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the prefix used for full flag names.
        /// </summary>
        public static string FlagPrefix => FLAG_PREFIX;

        /// <summary>
        /// Gets the prefix used for short flag names.
        /// </summary>
        public static string ShortFlagPrefix => SHORT_FLAG_PREFIX;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the prefixed name of the flag.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if name is not set.</exception>
        public string PrefixedName => 
            _name != string.Empty ? FlagPrefix + _name : throw new InvalidOperationException("Name is not set");

        /// <summary>
        /// Gets the short prefixed name of the flag.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if short name is not set.</exception>
        public string ShortPrefixedName => 
            _shortName != string.Empty ? ShortFlagPrefix + _shortName : throw new InvalidOperationException("Short name is not set");

        /// <summary>
        /// Gets the description of the flag.
        /// </summary>
        public string Description => _desc;

        /// <summary>
        /// Gets the minimum number of parameters required by the flag.
        /// </summary>
        public int MinParameterCount => _minParameterCount;

        /// <summary>
        /// Gets whether the flag requires parameters.
        /// </summary>
        public bool DoesRequireParameters => _minParameterCount != DEFAULT_MIN_PARAM_COUNT;

        /// <summary>
        /// Gets whether the flag accepts a varying number of parameters.
        /// </summary>
        public bool IsVariadicParameters  => _isVariadic;

        /// <summary>
        /// Gets the action to be executed when the flag is invoked.
        /// </summary>
        public Action<List<string>?> Action => _action;
        #endregion

        #region Configuration Methods
        /// <summary>
        /// Sets the name of the flag.
        /// </summary>
        /// <param name="name">The name of the flag without prefix.</param>
        /// <returns>This flag instance for method chaining.</returns>
        /// <remarks>Must be called before executing the flag.</remarks>
        public Flag SetName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Sets the short name of the flag.
        /// </summary>
        /// <param name="shortName">The short name of the flag without prefix.</param>
        /// <returns>This flag instance for method chaining.</returns>
        /// <remarks>Must be called before executing the flag.</remarks>
        public Flag SetShortName(string shortName)
        {
            _shortName = shortName;
            return this;
        }

        /// <summary>
        /// Sets the description of the flag.
        /// </summary>
        /// <param name="desc">The description text for the flag.</param>
        /// <returns>This flag instance for method chaining.</returns>
        public Flag SetDescription(string desc)
        {
            _desc = desc; 
            return this;
        }

        /// <summary>
        /// Sets the minimum number of parameters required by the flag.
        /// </summary>
        /// <param name="parameterCount">The minimum number of parameters required.</param>
        /// <returns>This flag instance for method chaining.</returns>
        public Flag SetParameterCount(int parameterCount)
        {
            _minParameterCount = parameterCount;
            return this;
        }

        /// <summary>
        /// Sets whether the flag can accept a varying number of parameters.
        /// </summary>
        /// <param name="isVariadic">True if the flag accepts varying parameters, false otherwise.</param>
        /// <returns>This flag instance for method chaining.</returns>
        public Flag SetVariadic(bool isVariadic)
        {
            _isVariadic = isVariadic;
            return this;
        }

        /// <summary>
        /// Sets the action to be executed when the flag is invoked.
        /// </summary>
        /// <param name="action">The action delegate to execute.</param>
        /// <returns>This flag instance for method chaining.</returns>
        /// <remarks>Must be called before executing the flag.</remarks>
        public Flag SetAction(Action<List<string>?> action)
        {
            _action = action;
            return this;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Checks if the given flag name matches this flag's name or short name.
        /// </summary>
        /// <param name="flagName">The flag name to check (with prefix).</param>
        /// <returns>True if the name matches, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown if name or short name is not set.</exception>
        public bool IsMatch(string flagName) => 
            flagName == PrefixedName || flagName == ShortPrefixedName;
        #endregion
    }
}