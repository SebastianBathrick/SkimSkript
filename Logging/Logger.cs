using System.Text;

namespace SkimSkript.Logging
{
    internal abstract class Logger
    {
        #region Constants
        protected const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Warning;

        private const bool DEFAULT_LOG_LABELS_ENABLED = true;
        private const char MESSAGE_INSERT_START = '{';
        private const char MESSAGE_INSERT_END = '}';
        #endregion

        #region Variables & Properties
        private LogLevel _logLevel = DEFAULT_LOG_LEVEL;
        private bool _labelsEnabled = DEFAULT_LOG_LABELS_ENABLED;

        public LogLevel LogLevel => _logLevel;
        #endregion

        #region Data Members Public Interface
        /// <summary>Returns true if the provided log level is enabled for output.</summary>
        public bool IsLevelEnabled(LogLevel level) => level <= _logLevel;

        /// <summary>Set the minimum log level that will be output.</summary>
        /// <param name="level">The minimum level.</param>
        public Logger SetMinimumLogLevel(LogLevel level)
        {
            _logLevel = level;
            return this;
        }

        /// <summary>Enable or disable log level labels in output.</summary>
        /// <param name="isEnabled">True to enable labels.</param>
        public Logger ToggleLogLabels(bool isEnabled)
        {
            _labelsEnabled = isEnabled;
            return this;
        }
        #endregion

        #region Logging Methods
        /// <summary>Logs an information message.</summary>
        public void Info(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Information)) return;
            ProcessMessage(message, LogLevel.Information, properties);
        }

        /// <summary>Logs a warning message.</summary>
        public void Warning(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Warning)) return;
            ProcessMessage(message, LogLevel.Warning, properties);
        }

        /// <summary>Logs an error message.</summary>
        public void Error(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Error)) return;
            ProcessMessage(message, LogLevel.Error, properties);
        }

        /// <summary>Logs an error with an exception.</summary>
        public void Error(Exception ex, string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Error)) return;
            ProcessMessage($"{message} Exception thrown: {ex.Message}", LogLevel.Error, properties);
        }

        /// <summary>Logs a fatal message.</summary>
        public void Fatal(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Fatal)) return;
            ProcessMessage(message, LogLevel.Fatal, properties);
        }

        /// <summary>Logs a fatal message with an exception.</summary>
        public void Fatal(Exception ex, string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Fatal)) return;
            ProcessMessage($"{message} Exception thrown: {ex.Message}", LogLevel.Fatal, properties);
        }

        /// <summary>Logs a debug message.</summary>
        public void Debug(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Debug)) return;
            ProcessMessage(message, LogLevel.Debug, properties);
        }

        /// <summary>Logs a verbose message.</summary>
        public void Verbose(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Verbose)) return;
            ProcessMessage(message, LogLevel.Verbose, properties);
        }
        #endregion

        #region Log Processing
        /// <summary>Implement in derived classes to write out the message.</summary>
        protected abstract void Write((string content, bool isProp)[] segmentedMessage);

        /// <summary>Prepares and sends the log message to the implementation's Write method.</summary>
        private void ProcessMessage(string message, LogLevel level, params object[] properties)
        {
            string labeledMessage = _labelsEnabled ? $"{GetLabel(level)} {message}" : message;
            var segmentedMessage = GetSegmentedMessage(labeledMessage, properties);
            Write(segmentedMessage);
        }

        /// <summary>Splits message into segments based on where property inserts are.</summary>
        protected (string content, bool isProp)[] GetSegmentedMessage(string message, params object[] properties)
        {
            var sb = new StringBuilder();
            var segments = new List<(string content, bool isProp)>();
            int propIndex = 0;

            foreach (var c in message)
            {
                if (c == MESSAGE_INSERT_START)
                {
                    if (sb.Length > 0)
                    {
                        segments.Add((sb.ToString(), false));
                        sb.Clear();
                    }
                }
                else if (c == MESSAGE_INSERT_END)
                {
                    if (propIndex < properties.Length)
                    {
                        var propText = properties[propIndex]?.ToString() ?? sb.ToString();
                        segments.Add((propText, true));
                        propIndex++;
                        sb.Clear();
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
                segments.Add((sb.ToString(), false));

            return segments.ToArray();
        }

        /// <summary>Returns the label for a log level, e.g., [Info]:</summary>
        private string GetLabel(LogLevel level) => $"[{level}]:";
        #endregion
    }
}
