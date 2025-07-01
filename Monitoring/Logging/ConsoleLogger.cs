using System.Text;

namespace SkimSkript.Logging
{
    public class ConsoleLogger : Logger
    {
        private const ANSIColor PROPERTY_COLOR = ANSIColor.BrightCyan;
        private const ANSIColor NUMBER_COLOR = ANSIColor.BrightGreen;

        public static string GetColoredContent(string text, ANSIColor color) =>
            $"\x1b[{(int)color}m{text}\x1b[0m";

        protected override void Write((string content, bool isProp)[] segmentedMessage)
        {
            var sb = new StringBuilder();

            foreach (var segment in segmentedMessage)
                if(!segment.isProp)
                    sb.Append(segment.content);
                else if(int.TryParse(segment.content, out _) || float.TryParse(segment.content, out _))
                    sb.Append(GetColoredContent(segment.content, NUMBER_COLOR));
                else
                    sb.Append(GetColoredContent(segment.content, PROPERTY_COLOR));
            
            Console.Write(sb.Append('\n').ToString());
        }

        public override void Clear()
        {
           Console.Clear();
        }
    }
}
