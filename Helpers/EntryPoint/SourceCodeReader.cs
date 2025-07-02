using JustLogger;

namespace SkimSkript.Helpers.EntryPoint
{
    /// <summary>
    /// Provides functionality for reading and validating SkimSkript source code files.
    /// </summary>
    internal static class SourceCodeReader
    {
        #region Constants
        private const string EXPECTED_FILE_EXTENSION = ".skim";
        #endregion

        #region Public Methods
        /// <summary>
        /// Attempts to read source code from specified file path.
        /// </summary>
        /// <param name="filePath">Path to source code file.</param>
        /// <param name="sourceCode">Output array containing file lines if successful.</param>
        /// <returns>True if source code was successfully read, false otherwise.</returns>
        public static bool TryGetSourceCode(string filePath, out string[] sourceCode)
        {
            string fileExtension = Path.GetExtension(filePath);

            if (!string.Equals(fileExtension, EXPECTED_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                Log.Error(
                    "Wrong file extension. Expected: {ExpectedExtension}, " +
                    "Actual: {ActualExtension} for file: {FilePath}", 
                    EXPECTED_FILE_EXTENSION, fileExtension, filePath
                    );
                sourceCode = [];
                return false;
            }

            try
            {
                sourceCode = File.ReadAllLines(filePath);
                return true;
            }
            catch (FileNotFoundException)
            {
                Log.Error("File not found: {FilePath}", filePath);
            }
            catch (DirectoryNotFoundException)
            {
                Log.Error("Directory not found: {FilePath}", filePath);
            }
            catch (UnauthorizedAccessException)
            {
                Log.Error("Access denied: {FilePath}", filePath);
            }
            catch (PathTooLongException)
            {
                Log.Error("Path too long: {FilePath}", filePath);
            }
            catch (NotSupportedException)
            {
                Log.Error("File format not supported: {FilePath}", filePath);
            }
            catch (IOException ex)
            {
                Log.Error("IO error reading file {FilePath}: {Message}", filePath, ex.Message);
            }
            catch (OutOfMemoryException)
            {
                Log.Error("Out of memory reading file: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error reading file {FilePath}: {Message}", filePath, ex.Message);
            }

            sourceCode = [];
            return false;
        }
        #endregion
    }
} 