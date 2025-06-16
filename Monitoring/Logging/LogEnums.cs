namespace SkimSkript.Logging
{
    /// <summary> Represents the logging levels used in the application. </summary>
    internal enum LogLevel
    {
        None = 0,
        UserInterface = 1,
        Fatal = 2,
        Error = 3,
        Warning = 4,
        Information = 5,
        Debug = 6,
        Verbose = 7,
    }

    /// <summary> Represents ANSI color codes for terminal output. </summary>
    internal enum ANSIColor
    {
        Black = 30,
        Red = 31,
        Green = 32,
        Yellow = 33,
        Blue = 34,
        Magenta = 35,
        Cyan = 36,
        White = 37,

        BrightBlack = 90,
        BrightRed = 91,
        BrightGreen = 92,
        BrightYellow = 93,
        BrightBlue = 94,
        BrightMagenta = 95,
        BrightCyan = 96,
        BrightWhite = 97,

        Default = 39 // Resets to terminal's default foreground color
    }
}
