using JustLogger;

namespace JollyRoger
{
    /// <summary>
    /// Handles the processing and execution of command line flags.
    /// </summary>
    /// <remarks>
    /// The FlagHandler is responsible for parsing command line arguments, identifying flags,
    /// validating their parameters, and executing the associated actions. It provides a
    /// centralized way to manage all command line options for an application.
    /// 
    /// The handler automatically includes built-in help flags (--help/-h and --full-help/-F)
    /// that provide usage information for all registered flags.
    /// 
    /// To use the FlagHandler: create a new instance, optionally set a logger using
    /// <see cref="SetLogger"/>, add custom flags using <see cref="AddFlags"/>, and process
    /// command line arguments by calling <see cref="TryExecuteFlagAction"/> for each flag.
    /// </remarks>
    internal class FlagHandler
    {
        #region Constants
        private const string HELP_TITLE = "Help";
        private const string HELP_PARAM_LABEL = "Parameters";

        private const string FULL_HELP_NAME = "full-help";
        private const string FULL_HELP_SHORT_NAME = "F";
        private const string FULL_HELP_DESC = "Displays a detailed list of valid flags for program.";

        private const string HELP_NAME = "help";
        private const string HELP_SHORT_NAME = "h";
        private const string HELP_DESC = "Displays a list of valid flags for program.";
        #endregion

        #region Data Members
        private List<Flag>? _flags;
        private Logger? _logger;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the prefixed name of the help command.
        /// </summary>
        /// <value>The string "--help".</value>
        public static string HelpCommandPrefixedName => Flag.FlagPrefix + HELP_NAME;

        /// <summary>
        /// Gets the prefixed name of the full help command.
        /// </summary>
        /// <value>The string "--full-help".</value>
        public static string FullHelpCommandPrefixedName => Flag.FlagPrefix + FULL_HELP_NAME;

        /// <summary>
        /// Gets the logger instance used by this handler.
        /// </summary>
        /// <value>The logger instance, or throws an exception if not set.</value>
        /// <exception cref="NullReferenceException">Thrown when the logger has not been set using <see cref="SetLogger"/>.</exception>
        private Logger Logger =>
            _logger ?? throw new NullReferenceException("Logger is null");

        /// <summary>
        /// Gets the list of registered flags.
        /// </summary>
        /// <value>A list containing all registered flags, including built-in help flags.</value>
        public List<Flag> Flags =>
            _flags ??= new() {
                new Flag()
                .SetName(HELP_NAME)
                .SetShortName(HELP_SHORT_NAME)
                .SetDescription(HELP_DESC)
                .SetParameterCount(0)
                .SetAction(HelpAction),

                new Flag()
                .SetName(FULL_HELP_NAME)
                .SetShortName(FULL_HELP_SHORT_NAME)
                .SetDescription(FULL_HELP_DESC)
                .SetParameterCount(0)
                .SetAction(FullHelpAction),
            };
        #endregion

        #region Configuration
        /// <summary>
        /// Sets the logger to be used by this handler for output and error reporting.
        /// </summary>
        /// <param name="logger">The logger instance to use for output.</param>
        /// <returns>This handler instance for method chaining.</returns>
        /// <remarks>
        /// If no logger is set, accessing the <see cref="Logger"/> property will throw an exception.
        /// It's recommended to set a logger before processing any flags to ensure proper error reporting.
        /// </remarks>
        public FlagHandler SetLogger(Logger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Adds custom flags to this handler's flag list.
        /// </summary>
        /// <param name="flags">The flags to add to the handler.</param>
        /// <returns>This handler instance for method chaining.</returns>
        /// <remarks>
        /// Each flag should be properly configured with a name, short name, parameter count, and action before being added.
        /// Duplicate flag names or short names will not be automatically detected, so care
        /// should be taken to ensure uniqueness.
        /// </remarks>
        public FlagHandler AddFlags(params Flag[] flags)
        {
            Flags.AddRange(flags);
            return this;
        }
        #endregion

        #region Public Flag Operations
        /// <summary>
        /// Determines whether a given argument is a flag.
        /// </summary>
        /// <param name="argument">The argument to check.</param>
        /// <returns>True if the argument starts with a flag prefix (-- or -), false otherwise.</returns>
        /// <remarks>
        /// This method should be called before sending an argument to <see cref="TryExecuteFlagAction"/>
        /// to determine if it represents a flag or a parameter.
        /// 
        /// Valid flag prefixes are "--" for full flag names (e.g., "--help") and "-" for short flag names (e.g., "-h").
        /// </remarks>
        public static bool IsFlag(string argument) =>
            argument.StartsWith(Flag.FlagPrefix) || argument.StartsWith(Flag.ShortFlagPrefix);

        /// <summary>
        /// Attempts to execute the flag at the specified index in the arguments array.
        /// </summary>
        /// <param name="flagIndex">The index of the flag to execute in the arguments array.</param>
        /// <param name="arguments">The complete array of command line arguments.</param>
        /// <param name="endIndex">The index after the flag and its parameters (set to flagIndex if an error occurs).</param>
        /// <returns>True if the flag was successfully executed, false if an error occurred.</returns>
        /// <remarks>
        /// This method extracts the flag name, finds the corresponding flag, validates parameters,
        /// and executes the flag's action. If the flag is not found, the caller should cease execution
        /// as this indicates an invalid or unsupported command line option.
        /// 
        /// The endIndex parameter is updated to indicate where processing should continue
        /// after this flag and its parameters have been processed.
        /// </remarks>
        public bool TryExecuteFlagAction(int flagIndex, string[] arguments, out int endIndex)
        {
            string flagName = arguments[flagIndex].Trim();
            var selectedFlag = GetFlag(flagName);

            // Remains the same because the caller will not call again & it reduces lines of code
            endIndex = flagIndex;

            if (selectedFlag == null)
            {
                Logger.Error("{FlagName} is not a valid flag", flagName);
                return false;
            }

            // If the flag doesn't require parameters, execute the flag
            if (!selectedFlag.DoesRequireParameters)
            {
                endIndex++;
                selectedFlag.Action.Invoke(null);
                return true;
            }

            /* If the flag requires parameters but doesn't have any, return false (includes variadic
             * flags that need at least one parameter) */
            if (flagIndex + 1 == arguments.Length && selectedFlag.MinParameterCount != 0)
            {
                Logger.Error("Flag {FlagName} requires parameters", selectedFlag.PrefixedName);
                return false;
            }

            var parameters = GetParameters(flagIndex, arguments, out endIndex);

            if (!IsValidParameters(selectedFlag, parameters))
            {
                endIndex = flagIndex + 1;
                Logger.Error("Flag {FlagName} has invalid parameters", selectedFlag.PrefixedName);
                return false;
            }

            selectedFlag.Action.Invoke(parameters);
            return true;
        }
        #endregion

        #region Private Flag Operations
        private Flag? GetFlag(string flagName)
        {
            foreach (var flag in Flags)
                if (flag.IsMatch(flagName))
                    return flag;

            return null;
        }

        private bool IsValidParameters(Flag flag, List<string> parameters)
        {
            var isValid = true;

            // If non-variadic, the number of parameters must match
            if (!flag.IsVariadicParameters && parameters.Count != flag.MinParameterCount)
                isValid = false;

            // If variadic, the number of parameters must be at least the parameter count
            if (flag.IsVariadicParameters && parameters.Count < flag.MinParameterCount)
                isValid = false;

            return isValid;
        }

        private List<string> GetParameters(int flagIndex, string[] arguments, out int endIndex)
        {
            var parameters = new List<string>();

            // Get all parameters until the next flag is found or the end of the arguments is reached
            for (endIndex = flagIndex + 1; endIndex < arguments.Length; endIndex++)
                // If the next argument is a flag, stop (shorthand or otherwise)
                if (IsFlag(arguments[endIndex]))
                    return parameters;
                else
                    parameters.Add(arguments[endIndex]);

            // Error checking is done elsewhere
            return parameters;
        }
        #endregion

        #region Built-in Flag Actions
        /// <summary>
        /// Displays basic help information for all registered flags.
        /// </summary>
        /// <param name="parameters">Not used (always null for help flags).</param>
        /// <remarks>
        /// This action is automatically registered for the --help/-h flags and displays
        /// a concise list of all available flags with their descriptions.
        /// </remarks>
        private void HelpAction(List<string>? parameters = null)
        {
            Logger.Interface("[{Title}]: The following is a list of valid commands:\n", HELP_TITLE);

            foreach (var flag in Flags)
            {
                Logger.Interface(
                    "[{Name} or {ShortName}]: " + flag.Description,
                    flag.PrefixedName,
                    flag.ShortPrefixedName
                 );
            }
        }

        /// <summary>
        /// Displays detailed help information for all registered flags.
        /// </summary>
        /// <param name="parameters">Not used (always null for help flags).</param>
        /// <remarks>
        /// This action is automatically registered for the --full-help/-F flags and displays
        /// comprehensive information about all flags, including parameter requirements,
        /// variadic status, and any additional information that has been added.
        /// </remarks>
        private void FullHelpAction(List<string>? parameters = null)
        {
            Logger.Interface("[{Title}]: The following is a detailed list of valid commands:", HELP_TITLE);

            foreach (var flag in Flags)
            {
                Logger.Interface(
                    "\n[{Name} or {ShortName}]: " + flag.Description,
                    flag.PrefixedName,
                    flag.ShortPrefixedName
                 );

                Logger.Interface(
                    "{Label}:\n\tMinimum Parameters: {Count}\n\tVarying Argument Count: {Bool}\n",
                    HELP_PARAM_LABEL,
                    flag.MinParameterCount,
                    flag.IsVariadicParameters
                    );

                for (int i = 0; i < flag.AdditionalInfo?.Length; i++)
                {
                    var addInfo = flag.AdditionalInfo[i];
                    Logger.Interface("{Label}: " + $"{addInfo.value}", addInfo.label, addInfo.properties);
                }
            }
        }    
        #endregion
    }
}
