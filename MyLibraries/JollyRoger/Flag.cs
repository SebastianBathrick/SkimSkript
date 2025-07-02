namespace JollyRoger
{
    /// <summary>
    /// Represents a command line flag that can be processed by the <see cref="FlagHandler"/>.
    /// </summary>
    /// <remarks>
    /// A flag represents a command line option that can be invoked with parameters. Flags have
    /// both a full name (e.g., "--help") and a short name (e.g., "-h") for convenience.
    /// 
    /// Before a flag can be executed, the following methods must be called:
    /// <see cref="SetName"/>, <see cref="SetShortName"/>, <see cref="SetParameterCount"/>, and <see cref="SetAction"/>.
    /// 
    /// Optional configuration includes setting a description, making the flag variadic (accepting
    /// varying numbers of parameters), and adding additional information for help output.
    /// </remarks>
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

        private AdditionalInfo[]? _additionalInfo;
        private object[,]? _additionalInfoProperties;

        private int _minParameterCount;
        private bool _isVariadic = DEFAULT_IS_VARIADIC;
        private Action<List<string>?> _action = (parameters) => throw new NotImplementedException();
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the prefix used for full flag names (e.g., "--help").
        /// </summary>
        /// <value>The string "--" used to prefix full flag names.</value>
        public static string FlagPrefix => FLAG_PREFIX;

        /// <summary>
        /// Gets the prefix used for short flag names (e.g., "-h").
        /// </summary>
        /// <value>The string "-" used to prefix short flag names.</value>
        public static string ShortFlagPrefix => SHORT_FLAG_PREFIX;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the full name of the flag with its prefix.
        /// </summary>
        /// <value>The prefixed name of the flag (e.g., "--help").</value>
        /// <exception cref="InvalidOperationException">Thrown when the name has not been set using <see cref="SetName"/>.</exception>
        public string PrefixedName =>
            _name != string.Empty ? FlagPrefix + _name : throw new InvalidOperationException("Name is not set");

        /// <summary>
        /// Gets the short name of the flag with its prefix.
        /// </summary>
        /// <value>The prefixed short name of the flag (e.g., "-h").</value>
        /// <exception cref="InvalidOperationException">Thrown when the short name has not been set using <see cref="SetShortName"/>.</exception>
        public string ShortPrefixedName =>
            _shortName != string.Empty ? ShortFlagPrefix + _shortName : throw new InvalidOperationException("Short name is not set");

        /// <summary>
        /// Gets the description of the flag.
        /// </summary>
        /// <value>The description text that explains what the flag does.</value>
        public string Description => _desc;

        /// <summary>
        /// Gets the additional information associated with this flag.
        /// </summary>
        /// <value>An array of <see cref="AdditionalInfo"/> objects, or null if none have been added.</value>
        /// <remarks>
        /// Additional information can include examples, usage notes, or other contextual details
        /// that are displayed in detailed help output.
        /// </remarks>
        public AdditionalInfo[]? AdditionalInfo => _additionalInfo;

        /// <summary>
        /// Gets the minimum number of parameters required by this flag.
        /// </summary>
        /// <value>The minimum number of parameters that must be provided when invoking this flag.</value>
        public int MinParameterCount => _minParameterCount;

        /// <summary>
        /// Gets whether this flag requires parameters to be provided.
        /// </summary>
        /// <value>True if the flag requires at least one parameter, false if it accepts no parameters.</value>
        /// <remarks>
        /// This is determined by checking if <see cref="MinParameterCount"/> is greater than zero.
        /// </remarks>
        public bool DoesRequireParameters => _minParameterCount != DEFAULT_MIN_PARAM_COUNT;

        /// <summary>
        /// Gets whether this flag can accept a varying number of parameters.
        /// </summary>
        /// <value>True if the flag is variadic (can accept more than the minimum parameter count), false otherwise.</value>
        /// <remarks>
        /// Variadic flags can accept additional parameters beyond the minimum required count.
        /// For example, a flag with MinParameterCount = 1 and IsVariadicParameters = true
        /// can accept 1, 2, 3, or more parameters.
        /// </remarks>
        public bool IsVariadicParameters => _isVariadic;

        /// <summary>
        /// Gets the action delegate that is executed when this flag is invoked.
        /// </summary>
        /// <value>The action that processes the flag and its parameters.</value>
        /// <remarks>
        /// The action receives a list of parameters as strings, or null if no parameters are provided.
        /// </remarks>
        public Action<List<string>?> Action => _action;
        #endregion

        #region Configuration Methods
        /// <summary>
        /// Sets the full name of the flag.
        /// </summary>
        /// <param name="name">The name of the flag without the prefix (e.g., "help" for "--help").</param>
        /// <returns>This flag instance for method chaining.</returns>
        /// <remarks>
        /// This method must be called before the flag can be executed.
        /// The name should be descriptive and follow common command line conventions (lowercase, hyphenated).
        /// </remarks>
        public Flag SetName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Sets the short name of the flag.
        /// </summary>
        /// <param name="shortName">The short name of the flag without the prefix (e.g., "h" for "-h").</param>
        /// <returns>This flag instance for method chaining.</returns>
        /// <remarks>
        /// This method must be called before the flag can be executed.
        /// Short names are typically single characters and provide a convenient shorthand for users.
        /// </remarks>
        public Flag SetShortName(string shortName)
        {
            _shortName = shortName;
            return this;
        }

        /// <summary>
        /// Sets the description of the flag.
        /// </summary>
        /// <param name="desc">The description text that explains what the flag does.</param>
        /// <returns>This flag instance for method chaining.</returns>
        public Flag SetDescription(string desc)
        {
            _desc = desc;
            return this;
        }

        /// <summary>
        /// Sets the minimum number of parameters required by this flag.
        /// </summary>
        /// <param name="parameterCount">The minimum number of parameters that must be provided.</param>
        /// <returns>This flag instance for method chaining.</returns>
        /// <remarks>
        /// This method must be called before the flag can be executed.
        /// If the flag is variadic (see <see cref="SetVariadic"/>), additional parameters beyond this minimum are allowed.
        /// </remarks>
        public Flag SetParameterCount(int parameterCount)
        {
            _minParameterCount = parameterCount;
            return this;
        }

        /// <summary>
        /// Sets whether this flag can accept a varying number of parameters.
        /// </summary>
        /// <param name="isVariadic">True if the flag accepts varying parameters, false if it requires exactly the minimum count.</param>
        /// <returns>This flag instance for method chaining.</returns>
        /// <remarks>
        /// When set to true, the flag can accept additional parameters beyond the minimum required count.
        /// When set to false, the flag must receive exactly the number of parameters specified by <see cref="SetParameterCount"/>.
        /// </remarks>
        public Flag SetVariadic(bool isVariadic)
        {
            _isVariadic = isVariadic;
            return this;
        }

        /// <summary>
        /// Sets the action to be executed when this flag is invoked.
        /// </summary>
        /// <param name="action">The action delegate that processes the flag and its parameters.</param>
        /// <returns>This flag instance for method chaining.</returns>
        /// <remarks>
        /// This method must be called before the flag can be executed.
        /// The action receives a list of parameters as strings. If no parameters are provided, the list will be null.
        /// The action should handle parameter validation and perform the actual work associated with the flag.
        /// </remarks>
        public Flag SetAction(Action<List<string>?> action)
        {
            _action = action;
            return this;
        }

        /// <summary>
        /// Adds additional information to this flag.
        /// </summary>
        /// <param name="additionalInfo">The additional information objects to add.</param>
        /// <returns>This flag instance for method chaining.</returns>
        /// <remarks>
        /// Additional information can include examples, usage notes, or other contextual details
        /// that are displayed in detailed help output. This information is optional and can be
        /// added incrementally.
        /// </remarks>
        public Flag AddAdditionalInfo(params AdditionalInfo[] additionalInfo)
        {
            _additionalInfo ??= [];
            var addInfoList = _additionalInfo.ToList();
            addInfoList.AddRange(additionalInfo);

            _additionalInfo = addInfoList.ToArray();
            return this;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Checks if the given flag name matches this flag's name or short name.
        /// </summary>
        /// <param name="flagName">The flag name to check, including the prefix (e.g., "--help" or "-h").</param>
        /// <returns>True if the name matches this flag's full name or short name, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when either the name or short name has not been set.</exception>
        /// <remarks>
        /// This method is used internally by the <see cref="FlagHandler"/> to identify which flag
        /// should be executed when processing command line arguments.
        /// </remarks>
        public bool IsMatch(string flagName) =>
            flagName == PrefixedName || flagName == ShortPrefixedName;
        #endregion
    }
}