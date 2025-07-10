using JollyRoger;
using JustLogger;
using SkimSkript.MainComponents;

namespace SkimSkript.Helpers.EntryPoint
{
    internal static class ProgramFlags
    {
        #region File Flag Constants
        private const string FILE_FLAG_NAME = "file";
        private const string FILE_FLAG_SHORT_NAME = "f";
        private const string FILE_FLAG_DESC =
            "Opens file(s) containing source code at path(s) provided as parameters.";

        private const int FILE_FLAG_MIN_PARAMETERS = 1;
        private const bool FILE_FLAG_IS_VARIADIC = true;

        private const string FILE_FLAG_ADD_INFO_LABEL_1 = "Note";
        private const string FILE_FLAG_ADD_INFO_VAL_1 =
            "Flag not required if only file paths are provided as arguments.";

        private const string FILE_FLAG_ADD_INFO_LABEL_2 = "Example";
        private const string FILE_FLAG_ADD_INFO_VAL_2 =
            $"--{FILE_FLAG_NAME} <.SK FILE PATH>";
        #endregion

        #region Debug Flag Constants
        private const string DEBUG_FLAG_NAME = "debug";
        private const string DEBUG_FLAG_SHORT_NAME = "d";
        private const string DEBUG_FLAG_DESC =
            "Enables interpreter main component debug messages and benchmarks.";

        private const int DEBUG_FLAG_MIN_PARAMETERS = 0;
        private const bool DEBUG_FLAG_IS_VARIADIC = true;

        private const string DEBUG_FLAG_ADD_INFO_LABEL_1 = "Note";
        private const string DEBUG_FLAG_ADD_INFO_VAL_1 =
            "If no parameter is provided debugging will be enabled for all main components.";

        private const string DEBUG_FLAG_ADD_INFO_LABEL_2 = "Valid Parameter Values";
        private const string DEBUG_FLAG_ADD_INFO_VAL_2 =
            $"{DEBUG_LEXER}, {DEBUG_PARSER}, and {DEBUG_INTERPRETER}.";

        private const string DEBUG_FLAG_ADD_INFO_LABEL_3 = "Example";
        private const string DEBUG_FLAG_ADD_INFO_VAL_3 =
            $"--{DEBUG_FLAG_NAME} <VALID PARAMETER VALUE>";

        private const string DEBUG_LEXER = "lexer";
        private const string DEBUG_PARSER = "parser";
        private const string DEBUG_INTERPRETER = "interpreter";
        #endregion

        #region Verbose Flag Constants
        private const string VERBOSE_FLAG_NAME = "verbose";
        private const string VERBOSE_FLAG_SHORT_NAME = "v";
        private const string VERBOSE_FLAG_DESC =
            "Enables interpreter main component verbose messages.";

        private const int VERBOSE_FLAG_MIN_PARAMETERS = 0;

        private const bool VERBOSE_FLAG_IS_VARIADIC = true;

        private const string VERBOSE_FLAG_ADD_INFO_LABEL_1 = "Note";
        private const string VERBOSE_FLAG_ADD_INFO_VAL_1 =
            "If no parameter is provided verbose will be enabled for all main components.";

        private const string VERBOSE_FLAG_ADD_INFO_LABEL_2 = "Valid Parameter Values";
        private const string VERBOSE_FLAG_ADD_INFO_VAL_2 =
            $"{DEBUG_LEXER}, {DEBUG_PARSER}, and {DEBUG_INTERPRETER}.";

        private const string VERBOSE_FLAG_ADD_INFO_LABEL_3 = "Example";
        private const string VERBOSE_FLAG_ADD_INFO_VAL_3 =
            $"--{VERBOSE_FLAG_NAME} <VALID PARAMETER VALUE>";
        #endregion
        
        #region Fields
        private static FlagHandler? _handler;
        private static Logger? _logger;
        #endregion

        #region Properties
        public static string NoArgsErrorMessage =>
            "{Requirement}. Use argument {Help} to display a list of valid commands.";

        public static object[] NoArgsErrorProperties =>
            [
            "One or more arguments required", 
            FlagHandler.HelpCommandPrefixedName, 
            FlagHandler.FullHelpCommandPrefixedName
            ];

        private static FlagHandler Handler => _handler ?? (_handler = new FlagHandler());

        private static Logger Logger => _logger ??
            throw new NullReferenceException("Logger null while evaluating arguments");
        #endregion

        #region Public Methods
        public static bool IsFlag(string[] args) => FlagHandler.IsFlag(args.First());

        public static bool TryEvaluateArguments(string[] args, Logger logger)
        {
            int argIndex = 0;
            _logger = logger;

            Handler
                .SetLogger(Logger)
                .AddFlags(CreateFlags());

            do
            {
                if (!Handler.TryExecuteFlagAction(argIndex, args, out argIndex))
                    return false;
            }                
            while (argIndex < args.Length && FlagHandler.IsFlag(args[argIndex]));

            return true;
        }
        #endregion

        #region Private Methods
        private static Flag[] CreateFlags() =>
        [
            new Flag()
            .SetName(FILE_FLAG_NAME)
            .SetShortName(FILE_FLAG_SHORT_NAME)
            .SetDescription(FILE_FLAG_DESC)
            .SetParameterCount(FILE_FLAG_MIN_PARAMETERS)
            .SetVariadic(FILE_FLAG_IS_VARIADIC)
            .SetAction(FileFlagAction)
            .AddAdditionalInfo(
                new AdditionalInfo(
                    FILE_FLAG_ADD_INFO_LABEL_1, FILE_FLAG_ADD_INFO_VAL_1),
                new AdditionalInfo(
                    FILE_FLAG_ADD_INFO_LABEL_2, FILE_FLAG_ADD_INFO_VAL_2)
                ),

            new Flag()
            .SetName(DEBUG_FLAG_NAME)
            .SetShortName(DEBUG_FLAG_SHORT_NAME)
            .SetDescription(DEBUG_FLAG_DESC)
            .SetParameterCount(DEBUG_FLAG_MIN_PARAMETERS)
            .SetVariadic(DEBUG_FLAG_IS_VARIADIC)
            .SetAction((parameters) => DebugFlagAction(parameters))
            .AddAdditionalInfo(
                new AdditionalInfo(
                    DEBUG_FLAG_ADD_INFO_LABEL_1, DEBUG_FLAG_ADD_INFO_VAL_1),
                new AdditionalInfo(
                    DEBUG_FLAG_ADD_INFO_LABEL_2, DEBUG_FLAG_ADD_INFO_VAL_2),
                new AdditionalInfo(
                    DEBUG_FLAG_ADD_INFO_LABEL_3, DEBUG_FLAG_ADD_INFO_VAL_3)
                ),

            new Flag()
            .SetName(VERBOSE_FLAG_NAME)
            .SetShortName(VERBOSE_FLAG_SHORT_NAME)
            .SetDescription(VERBOSE_FLAG_DESC)
            .SetParameterCount(VERBOSE_FLAG_MIN_PARAMETERS)
            .SetVariadic(VERBOSE_FLAG_IS_VARIADIC)
            .SetAction((parameters) => VerboseFlagAction(parameters))
            .AddAdditionalInfo(
                new AdditionalInfo(
                    VERBOSE_FLAG_ADD_INFO_LABEL_1, VERBOSE_FLAG_ADD_INFO_VAL_1),
                new AdditionalInfo(
                    VERBOSE_FLAG_ADD_INFO_LABEL_2, VERBOSE_FLAG_ADD_INFO_VAL_2),
                new AdditionalInfo(
                    VERBOSE_FLAG_ADD_INFO_LABEL_3, VERBOSE_FLAG_ADD_INFO_VAL_3)
                ),
            ];

        private static void FileFlagAction(List<string>? parameters = null)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Program.SourceCodePaths.AddRange(parameters);
        }

        private static void VerboseFlagAction(List<string>? parameters = null)
        {
            if (parameters == null || parameters.Count == 0)
            {
                Program.SetVerboseMainComponents(
                    MainComponentType.Lexer, 
                    MainComponentType.Parser, 
                    MainComponentType.Interpreter
                    );
                Logger.Info("Verbose enabled for all main components");
                return;
            }

            var verboseList = new List<MainComponentType>();

            foreach (var parameter in parameters)
                if (parameter == DEBUG_LEXER && !verboseList.Contains(MainComponentType.Lexer))
                {
                    verboseList.Add(MainComponentType.Lexer);
                    Logger.Info("Verbose enabled for lexer");
                }
                else if (parameter == DEBUG_PARSER && !verboseList.Contains(MainComponentType.Parser))
                {
                    verboseList.Add(MainComponentType.Parser);
                    Logger.Info("Verbose enabled for parser");
                }
                else if (parameter == DEBUG_INTERPRETER && !verboseList.Contains(MainComponentType.Interpreter))
                {
                    verboseList.Add(MainComponentType.Interpreter);
                    Logger.Info("Verbose enabled for interpreter");
                }
                else
                    Logger.Warning("{Parameter} is not a verboseable main component and will be skipped", parameter);

            Program.SetVerboseMainComponents(verboseList.ToArray());
        }

        private static void DebugFlagAction(List<string>? parameters = null)
        {
            if (parameters == null || parameters.Count == 0)
            {
                Program.SetDebuggedMainComponents(
                    MainComponentType.Lexer, 
                    MainComponentType.Parser, 
                    MainComponentType.Interpreter
                    );
                Logger.Info("Debugging enabled for all main components");
                return;
            }

            var debugList = new List<MainComponentType>();

            foreach (var parameter in parameters)
                if (parameter == DEBUG_LEXER && !debugList.Contains(MainComponentType.Lexer))
                {
                    debugList.Add(MainComponentType.Lexer);
                    Logger.Info("Debugging enabled for lexer");
                }
                else if (parameter == DEBUG_PARSER && !debugList.Contains(MainComponentType.Parser))
                {
                    debugList.Add(MainComponentType.Parser);
                    Logger.Info("Debugging enabled for parser");
                }
                else if (parameter == DEBUG_INTERPRETER && !debugList.Contains(MainComponentType.Interpreter))
                {
                    debugList.Add(MainComponentType.Interpreter);
                    Logger.Info("Debugging enabled for interpreter");
                }
                else
                    Logger.Warning("{Parameter} is not a debuggable main component and will be skipped", parameter);

            Program.SetDebuggedMainComponents(debugList.ToArray());
        }
        #endregion
    }
}
