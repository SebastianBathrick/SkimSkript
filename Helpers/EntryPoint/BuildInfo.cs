using System.Reflection;

public static class BuildInfo
{
    /// <summary>
    /// Gets just the version number and build date
    /// </summary>
    /// <returns>Version and build date (e.g., "0.0.1-alpha - July 06, 2025")</returns>
    public static string GetVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string version = GetVersionNumber(assembly);
        string buildDate = GetBuildDate(assembly);

        return $"{version} - {buildDate}";
    }

    /// <summary>
    /// Gets complete version information with all details
    /// </summary>
    /// <returns>Full version information</returns>
    public static string GetFullVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();

        string product = GetProduct(assembly);
        string version = GetVersionNumber(assembly);
        string commitHash = GetCommitHash(assembly);
        string buildDate = GetBuildDate(assembly);
        string author = GetAuthor(assembly);

        return $"{product} v{version}\n" +
               $"Built on {buildDate}\n" +
               $"Commit: {commitHash}\n" +
               $"By {author}";
    }



    // Helper methods
    private static string GetVersionNumber(Assembly assembly)
    {
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0]
            ?? assembly.GetName().Version?.ToString()
            ?? "Unknown";
    }

    private static string GetCommitHash(Assembly assembly)
    {
        string? fullVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        if (fullVersion?.Contains('+') == true)
        {
            return fullVersion.Split('+')[1];
        }
        return "Unknown";
    }

    private static string GetBuildDate(Assembly assembly)
    {
        try
        {
            var location = assembly.Location;
            if (!string.IsNullOrEmpty(location))
            {
                var fileInfo = new FileInfo(location);
                return fileInfo.CreationTime.ToString("MMMM dd, yyyy 'at' HH:mm");
            }
        }
        catch
        {
            // Fallback if we can't get file info
        }
        return "Unknown";
    }

    private static string GetProduct(Assembly assembly)
    {
        return assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product
            ?? assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title
            ?? Path.GetFileNameWithoutExtension(assembly.Location)
            ?? "Unknown Application";
    }

    private static string GetAuthor(Assembly assembly)
    {
        return assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company
            ?? "Unknown";
    }

    private static string GetCopyright(Assembly assembly)
    {
        return assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright
            ?? "";
    }

    private static string GetDescription(Assembly assembly) =>
        assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description
            ?? "No description available";
    private static string GetTargetFramework() =>
         System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

}