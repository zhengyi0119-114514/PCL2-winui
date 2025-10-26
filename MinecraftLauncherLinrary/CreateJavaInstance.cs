using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace MinecraftLauncherLinrary;

public static partial class JavaUtils
{
    public static JavaInstance CreateJavaInstance(String javaHomePath)
    {
        var execatableFile = GetJavaExecatableFilePath(javaHomePath);
        if (String.IsNullOrEmpty(execatableFile))
            throw new FileNotFoundException($"File java(.exe) not found in {javaHomePath}/bin");

        using var javaProcess = Process.Start(new ProcessStartInfo()
        {
            FileName = execatableFile,
            Arguments = "-XshowSettings -version",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            StandardErrorEncoding = Encoding.UTF8,
            StandardInputEncoding = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8,
            // UseShellExecute = false,
        })!;
        javaProcess.WaitForExit();
        var output = javaProcess.StandardOutput.ReadToEnd();
        var error = javaProcess.StandardError.ReadToEnd();
        if (String.IsNullOrEmpty(error))
            throw new InvalidOperationException("Java process did not produce any error output");

        return new JavaInstance
        {
            HomePath = javaHomePath,
            MajorVersion = parseJavaVersion(majorVersionRegex().Match(error).Value.Split('=')![1].Trim()),
            Vendor = vorderRegex().Match(error).Value.Split("=")![1].Trim() ?? "UNKDOWN",
            Arch = string.Empty,
            Version = versionRegex().Match(error).Value.Split('=')![1].Trim() ?? "UNKDOWN",
        };
    }
    private static JavaMajorVersion parseJavaVersion(String s)
    {
        try
        {
            if (s.Contains('.'))
                return (JavaMajorVersion)Int32.Parse(s.Split('.')![1]);
            else
                return (JavaMajorVersion)Int32.Parse(s);
        }
        catch (FormatException)
        {
            return JavaMajorVersion.Unknown;
        }
    }
    [GeneratedRegex(@"java\.vm\.vendor = (.+)")]
    private static partial Regex vorderRegex();
    [GeneratedRegex(@"java\.specification\.version = (.+)")]
    private static partial Regex majorVersionRegex();
    [GeneratedRegex(@"java\.vm\.version = (.+)")]
    private static partial Regex versionRegex();
}
