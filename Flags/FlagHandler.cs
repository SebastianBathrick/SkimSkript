using System.Text;
using JustLogger;

namespace Flags
{
    internal class FlagHandler
    {
        #region Constants
        private const string HELP_FLAG_NAME = "help";
        private const string HELP_FLAG_SHORT_NAME = "h";
        private const string HELP_DESC = "Displays a list of valid flags for program.";
        #endregion

        #region Data Members
        private List<Flag>? _flags;
        private Logger? _logger;
        #endregion

        #region Properties
        private Logger Logger => 
            _logger ?? throw new NullReferenceException("Logger is null");
        
        /// <summary>
        /// Gets  list of registered flags.
        /// </summary>
        /// <remarks>Automatically initializes with help flag.</remarks>
        public List<Flag> Flags => 
            _flags ??= new() {
                new Flag()
                .SetName(HELP_FLAG_NAME)
                .SetShortName(HELP_FLAG_SHORT_NAME)
                .SetDescription(HELP_DESC)
                .SetParameterCount(0)
                .SetAction(HelpFlagAction)
            };
        #endregion

        #region Configuration
        /// <summary>
        /// Sets logger to be used by handler.
        /// </summary>
        /// <param name="logger">Logger instance to use.</param>
        /// <returns>This handler instance for method chaining.</returns>
        /// <remarks>If null, exception will be thrown after attempt to access it.</remarks>
        public FlagHandler SetLogger(Logger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Adds flags to handler flag list.
        /// </summary>
        /// <param name="flags">Flags to add to handler.</param>
        /// <returns>This handler instance for method chaining.</returns>
        public FlagHandler AddFlags(params Flag[] flags)
        {
            Flags.AddRange(flags);
            return this;
        }
        #endregion

        #region Public Flag Operations
        /// <summary>
        /// Checks if given argument is a flag.
        /// </summary>
        /// <param name="argument">Argument to check.</param>
        /// <returns>True if argument is a flag, false otherwise.</returns>
        /// <remarks>Should be called before sending argument to <see cref="ExecuteFlag"/>.</remarks>
        public static bool IsFlag(string argument) => 
            argument.StartsWith(Flag.FlagPrefix) || argument.StartsWith(Flag.ShortFlagPrefix);

        /// <summary>
        /// Executes flag at given index.
        /// </summary>
        /// <param name="flagIndex">Index of flag to execute.</param>
        /// <param name="arguments">Arguments to execute flag with.</param>
        /// <param name="endIndex">Index after flag and its parameters (is set to flagIndex if error occurs).</param>
        /// <returns>True if flag was executed, false otherwise.</returns>
        /// <remarks>If flag is not found, caller should cease execution.</remarks>
        public bool ExecuteFlag(int flagIndex, string[] arguments, out int endIndex)
        {
           string flagName = arguments[flagIndex].Trim();
           var selectedFlag = GetFlag(flagName);

            // Remains the same because the caller will not call again & it reduces lines of code
            endIndex = flagIndex;

           if(selectedFlag == null)
           {
                Logger.Error("Flag {FlagName} not found", flagName);
                return false;
           }

           // If the flag doesn't require parameters, execute the flag
           if(!selectedFlag.DoesRequireParameters)
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

            if(!IsValidParameters(selectedFlag, parameters))
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
            foreach(var flag in Flags)
                if(flag.IsMatch(flagName))
                    return flag;

            return null;
        }

        private bool IsValidParameters(Flag flag, List<string> parameters)
        {
            var isValid = true;

            // If non-variadic, the number of parameters must match
            if(!flag.IsVariadicParameters && parameters.Count != flag.MinParameterCount)
                isValid = false;

            // If variadic, the number of parameters must be at least the parameter count
            if(flag.IsVariadicParameters && parameters.Count < flag.MinParameterCount)
                isValid = false;

            return isValid;
        }

        private List<string> GetParameters(int flagIndex, string[] arguments, out int endIndex)
        {
            var parameters = new List<string>();

            // Get all parameters until the next flag is found or the end of the arguments is reached
            for(endIndex = flagIndex + 1; endIndex < arguments.Length; endIndex++)
                // If the next argument is a flag, stop (shorthand or otherwise)
                if(IsFlag(arguments[endIndex]))
                    return parameters;
                else
                    parameters.Add(arguments[endIndex]);

            // Error checking is done elsewhere
            return parameters;
        }
        #endregion

        #region Built-in Flag Actions
        private void HelpFlagAction(List<string>? parameters = null)
        {
            Logger.Interface("[{Title}]: The following is a list of valid flags:", HELP_FLAG_NAME.ToUpper());
            
            foreach(var flag in Flags)
            {
                Logger.Interface(
                    "\n[{Name} or {ShortName}]: " + flag.Description,
                    flag.PrefixedName,
                    flag.ShortPrefixedName
                 );

                Logger.Interface(
                    "\n\tMinimum Parameters: {Count}\n\tVarying Argument Count: {Bool}",
                    flag.MinParameterCount,
                    flag.IsVariadicParameters
                    );
            }
        }
        #endregion
    }
}
