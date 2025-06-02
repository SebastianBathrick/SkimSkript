using SkimSkript.Syntax;
using System.Text;

namespace SkimSkript.Nodes.Runtime
{
    /// <summary>Class representing a built-in function node for printing a string on a single line, or multiple strings on separate lines.</summary>
    public class PrintNode : BuiltInFunctionNode
    {
        StringBuilder _stringBuilder = new StringBuilder();

        public PrintNode() : base(BuiltInFunctionID.Print)
        {
            _returnTypeNode = null;
            isVariadic = true;
        }

        /// <summary>Called to print each argument on a seperate line and accepts a variable number of 
        /// arguments with a minimum of 1. Each argument will be printed on its own line.</summary>
        /// <param name="arguments">List containing argument data with the source of the parameter's
        /// value and whether or not the argument is marked as reference. Each argument that is not
        /// of type string will be coerced prior to the call.</param>
        public override Node? Call(List<(Node source, bool isRef)>? arguments)
        {          
            for(int i = 0; i < arguments.Count; i++)
                _stringBuilder.Append(arguments[i].source.ToString()).Append('\n');

            Console.Write(value: _stringBuilder.ToString());
            _stringBuilder.Clear();
            return null;
        }
    }
}
