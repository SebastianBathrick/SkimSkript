using SkimSkript.Syntax;
using System.Text;

namespace SkimSkript.Nodes
{
    /// <summary>Class representing a built-in function node for reading user input and returning it as a string.</summary>
    public class ReadNode : BuiltInFunctionNode
    {
        StringBuilder? _stringBuilder = new StringBuilder();

        private StringBuilder StringBuilder => _stringBuilder ??= new StringBuilder();

        public ReadNode() : base(BuiltInFunctionID.Read, typeof(StringValueNode), isVariadic: true) { }

        /// <summary>Called to retrieve keyboard input via the console window.</summary>
        /// <returns>Node containing the string value of the keyboard input entered.</returns>
        public override Node? Call(Node[]? arguments)
        {
            if (arguments != null)
            {
                if (StringBuilder.Length != 0)
                    StringBuilder.Clear();

                foreach (var node in arguments)
                    StringBuilder.Append(node.ToString()).Append('\n');

                Console.Write(value: StringBuilder.ToString());
            }

            var inputValue = Console.ReadLine();
            inputValue ??= string.Empty;
            return new StringValueNode(inputValue);
        }
    }
}
