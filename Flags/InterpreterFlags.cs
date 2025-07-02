using Flags;
using JustLogger;

namespace SkimSkript
{
    internal static class InterpreterFlags
    {
        #region Constants
        private const string FILE_FLAG_NAME = "file";
        private const string FILE_FLAG_SHORT_NAME = "f";
        private const string FILE_FLAG_DESC = "Opens file(s) containing source code at path(s) provided " +
            "(Flag not required if only file paths are provided as arguments).";
        private const int FILE_FLAG_MIN_PARAMETERS = 1;
        private const bool FILE_FLAG_IS_VARIADIC = true;
        #endregion

        #region Fields
        private static FlagHandler? _handler;
        private static Logger? _logger;
        #endregion

        #region Properties
        private static FlagHandler Handler => _handler ?? (_handler = new FlagHandler());
        private static Logger Logger => _logger ?? 
            throw new NullReferenceException("Logger null while evaluating arguments");
        #endregion

        #region Public Methods
        public static void EvaluateArguments(string[] args, Logger logger)
        {
            int argIndex = 0;
            _logger = logger;

            if (!FlagHandler.IsFlag(args[argIndex]))
            {
                Program.AddSourceCodePaths(args);
                return;
            }

            Handler
                .SetLogger(Logger)
                .AddFlags(CreateFlags());

            do
                Handler.ExecuteFlag(argIndex, args, out argIndex);
            while (argIndex < args.Length && FlagHandler.IsFlag(args[argIndex]));
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
            .SetAction((parameters) => FileFlagAction(parameters)),   
        ];

        private static void FileFlagAction(List<string>? parameters = null)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            Program.SourceCodePaths.AddRange(parameters);
        }
        #endregion
    }
}
