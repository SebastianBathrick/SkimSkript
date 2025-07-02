using JustLogger;
using System.Diagnostics;

namespace SkimSkript
{
    public enum MainComponentType
    {
        Lexer,
        Parser,
        Interpreter
    }

    internal abstract class MainComponent<T, Y> where T : notnull where Y : notnull
    {
        private bool _isDebugging;
        private string _name;
        
        private Stopwatch? _executionTimer;
        private long _executionTimeMs;

        public abstract MainComponentType ComponentType { get; }
        public bool IsDebugging => _isDebugging;
        public long ExecutionTimeMs => _executionTimeMs;

        public MainComponent(IEnumerable<MainComponentType> debuggedTypes)
        {
            _isDebugging = debuggedTypes.Contains(ComponentType);
            _name = GetType().Name;

            if (Log.IsLoggerSet)
                Log.Info("{ClassName} initialized with debug mode: {State}", _name, _isDebugging);
        }

        public Y Execute(T componentInput)
        {
            Log.Info("Executing {ClassName}", _name);

            if(!_isDebugging)
            {
                var normReturnData = OnExecute(componentInput);
                Log.Info("{ClassName} execution completed", _name);
                return normReturnData;
            }

            Log.Verbose("Input for {ClassName}: {Input}", _name, componentInput);
            Log.Debug("Starting {ClassName} execution timer", _name);
            _executionTimer = Stopwatch.StartNew();

            var debugReturnData = OnExecute(componentInput);

            _executionTimer?.Stop();
            _executionTimeMs = _executionTimer!.ElapsedMilliseconds;
            Log.Debug("{ClassName} execution in {ExecutionTime} ms", _name, _executionTimeMs);
            Log.Verbose("Output for {ClassName}: {Output}", _name, debugReturnData);
            return debugReturnData;
        }

        protected abstract Y OnExecute(T componentInput);
    }
}
