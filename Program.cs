using SkimSkript;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Error: Please provide exactly one file path as an argument.");
            return -1;
        }

        var compiler = new SkimSkriptCore(args[0]);
        return compiler.WasExecutionSuccessful ? 0 : -1;
    }
}
