using SkimSkript;

class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Error: One or file path arguments required.");
            return -1;
        }

        var core = new SkimSkriptCore();

        foreach(var filepath in args)
        {
            try
            {
                if(!TryGetFileContents(filepath, out var fileContents))
                    return -1; // Error already logged in TryGetFileContents

                core.Execute(fileContents!);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error processing file '{filepath}': {ex.Message}");
                return -1;
            }
        }

        return core.WasExecutionSuccessful ? 0 : -1;
    }

    private static bool TryGetFileContents(string filePath, out string[]? linesOfCode)
    {
        linesOfCode = null;

        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Error: File '{filePath}' does not exist.");
            return false;
        }

        try
        {
            linesOfCode = File.ReadLines(filePath).ToArray();
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error reading file: " + ex.Message);
            return false;
        }
    }
}
