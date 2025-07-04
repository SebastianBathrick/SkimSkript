using SkimSkript.Syntax;
using System.Text;
using System.Xml.Linq;

namespace SkimSkript.Nodes
{
    /// <summary>Class representing a built-in function node for reading user input and returning it as a string.</summary>
    public class ReadNode : BuiltInFunctionNode
    {
        StringBuilder? _stringBuilder = new StringBuilder();

        private StringBuilder StrBuilder => _stringBuilder ??= new StringBuilder();

        public ReadNode() : base(BuiltInFunctionID.Read, typeof(StringValueNode), isVariadic: true) { }

        /// <summary>Called to retrieve keyboard input via the console window.</summary>
        /// <returns>Node containing the string value of the keyboard input entered.</returns>
        public override Node? Call(Node[]? arguments)
        {
            if (arguments != null)
            {
                if (StrBuilder.Length != 0)
                    StrBuilder.Clear();

                for(int i = 0; i < arguments.Length; i++)
                {
                    StrBuilder.Append(arguments[i].ToString());

                    if(i < arguments.Length - 1)
                        StrBuilder.Append("\n");
                }

                StrBuilder.Append(' '); // So user input isn't squished next to prompt

                Console.Write(value: StrBuilder.ToString());
            }

            var inputValue = Console.ReadLine();
            inputValue ??= string.Empty;
            return new StringValueNode(inputValue);
        }
    }
}
