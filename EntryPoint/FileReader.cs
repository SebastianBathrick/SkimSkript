namespace SkimSkript.EntryPoint
{
    internal class FileReader
    {
        public string[] GetLinesOfCode(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found");

            return File.ReadLines(filePath).ToArray();
        }

    }
}
