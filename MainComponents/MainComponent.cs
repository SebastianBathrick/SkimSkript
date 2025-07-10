using JustLogger;
using JustLogger.ConsoleLogging;
using System.Diagnostics;

namespace SkimSkript.MainComponents
{
    public enum MainComponentType
    {
        Lexer,
        Parser,
        Interpreter
    }

    internal abstract class MainComponent<T, Y> where T : notnull where Y : notnull
    {
        private static long _globalElapsedTime = 0;

        private bool _isDebugging;
        private string _name;

        protected Logger? _logger;
        private Stopwatch? _executionTimer;

        public abstract MainComponentType ComponentType { get; }

        public MainComponent(IEnumerable<MainComponentType> debuggedTypes, IEnumerable<MainComponentType> verboseTypes)
        {
            var isVerbose = verboseTypes.Contains(ComponentType);
            _isDebugging = debuggedTypes.Contains(ComponentType) || isVerbose;
            _name = GetType().Name;

            if (_isDebugging)
            {
                var logLevel = isVerbose ? LogLevel.Verbose : LogLevel.Debug;
                _logger = new ConsoleLogger().SetMinimumLogLevel(logLevel);
                _logger?.Debug("{ClassName} initialized with debug mode: {State}", _name, _isDebugging);
                _executionTimer = Stopwatch.StartNew();
            }

            OnConstructor();

            if (!_isDebugging)
                return;

            _executionTimer?.Stop();
            var elapsedTime = _executionTimer!.ElapsedMilliseconds;
            GlobalClock.AddToGlobalElapsedTime(elapsedTime);

            _logger?.Debug("{ClassName} constructor took {ExecutionTime} ms", _name, elapsedTime!);
            
        }

        public Y Execute(T componentInput)
        {
            if (!_isDebugging)
                return OnExecute(componentInput);

            _logger?.Verbose("Input for {ClassName}: {Input}", _name, componentInput);
            _logger?.Debug("Starting {ClassName} execution timer", _name);
            _executionTimer?.Restart();

            _logger?.Debug("Executing {ClassName}", _name);
            var debugReturnData = OnExecute(componentInput);

            _executionTimer?.Stop();
            var elapsedTime = _executionTimer!.ElapsedMilliseconds;
            GlobalClock.AddToGlobalElapsedTime(elapsedTime);

            _logger?.Debug("{ClassName} executed in {ExecutionTime} ms", _name, elapsedTime);
            _logger?.Verbose("Output for {ClassName}: {Output}", _name, debugReturnData);

            return debugReturnData;
        }

        protected abstract Y OnExecute(T componentInput);

        protected virtual void OnConstructor() => _logger?.Debug("{ClassName} no OnConstructor behavior defined", _name);
    }

    internal static class GlobalClock
    {
        private static long _globalElapsedTime = 0;

        public static long GlobalElapsedTime => _globalElapsedTime;

        public static void AddToGlobalElapsedTime(long elapsedTime) => _globalElapsedTime += elapsedTime;

    }
}
