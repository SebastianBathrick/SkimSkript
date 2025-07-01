using SkimSkript.Syntax;
using System.Text;

namespace SkimSkript.Nodes
{
    /// <summary>Class representing a built-in function node for printing a string on a single line, or multiple strings on separate lines.</summary>
    public class PrintNode : BuiltInFunctionNode
    {
        StringBuilder? _stringBuilder = new StringBuilder();

        private StringBuilder StringBuilder => _stringBuilder ??= new StringBuilder();

        public PrintNode() : base(builtInFunctionId:BuiltInFunctionID.Print, isVariadic:true)
        {

        }

        public static void CallInstance(string message)
        {
            new PrintNode().Call([new StringValueNode(message)]);
        }

        /// <summary>Called to print each argument on a seperate line and accepts a variable number of 
        /// arguments with a minimum of 1. Each argument will be printed on its own line.</summary>
        /// <param _name="arguments">List containing argument data with the source of the parameter's
        /// value and whether or not the argument is marked as reference. Each argument that is not
        /// of type string will be coerced prior to the call.</param>
        public override Node? Call(Node[]? arguments)
        {          
            if(arguments == null)
            {
                Console.WriteLine();
                return null;
            }

            if (StringBuilder.Length != 0)
                StringBuilder.Clear();

            foreach(var node in arguments)
                StringBuilder.Append(node.ToString()).Append('\n');

            Console.Write(value: StringBuilder.ToString());
            return null;
        }
    }
}
