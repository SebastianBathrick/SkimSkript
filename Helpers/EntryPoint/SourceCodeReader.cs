using JustLogger;

namespace SkimSkript.Helpers.EntryPoint
{
    /// <summary>
    /// Provides functionality for reading and validating SkimSkript source code files.
    /// </summary>
    internal static class SourceCodeReader
    {
        #region Constants
        private const string EXPECTED_FILE_EXTENSION = ".sk";
        #endregion

        #region Public Methods
        /// <summary>
        /// Attempts to read source code from specified file path.
        /// </summary>
        /// <param name="filePath">Path to source code file.</param>
        /// <param name="sourceCode">Output array containing file lines if successful.</param>
        /// <returns>True if source code was successfully read, false otherwise.</returns>
        public static bool TryGetSourceCode(string filePath, Logger errorLogger, out string[] sourceCode)
        {
            string fileExtension = Path.GetExtension(filePath);

            if (!string.Equals(fileExtension, EXPECTED_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                errorLogger.Error(
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
                errorLogger.Error("File not found: {FilePath}", filePath);
            }
            catch (DirectoryNotFoundException)
            {
                errorLogger.Error("Directory not found: {FilePath}", filePath);
            }
            catch (UnauthorizedAccessException)
            {
                errorLogger.Error("Access denied: {FilePath}", filePath);
            }
            catch (PathTooLongException)
            {
                errorLogger.Error("Path too long: {FilePath}", filePath);
            }
            catch (NotSupportedException)
            {
                errorLogger.Error("File format not supported: {FilePath}", filePath);
            }
            catch (IOException ex)
            {
                errorLogger.Error("IO error reading file {FilePath}: {Message}", filePath, ex.Message);
            }
            catch (OutOfMemoryException)
            {
                errorLogger.Error("Out of memory reading file: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                errorLogger.Error("Unexpected error reading file {FilePath}: {Message}", filePath, ex.Message);
            }

            sourceCode = [];
            return false;
        }
        #endregion
    }
}