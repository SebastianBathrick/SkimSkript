namespace SkimSkript.Logging
{
    internal class ConsoleLogger : Logger
    {
        private const ANSIColor PROPERTY_COLOR = ANSIColor.BrightCyan;

        public static void WriteAnsiColored(string text, ANSIColor color) =>
            Console.Write($"\x1b[{(int)color}m{text}\x1b[0m");

        protected override void Write((string content, bool isProp)[] segmentedMessage)
        {
            foreach (var segment in segmentedMessage)
                if (segment.isProp)
                    WriteAnsiColored(segment.content, PROPERTY_COLOR);
                else
                    Console.Write(segment.content);

            Console.WriteLine(); // Ensure a new line after the message 
        }
    }
}
