using System;
using System.Text.RegularExpressions;

namespace MinecraftLauncherLinrary;

public static partial class JavaUtils
{
    /// <returns>"java(.exe)" file path</returns>
    public static String? GetJavaExecatableFilePath(String javaHomePath)
    {
        String execatableExtension = Environment.OSVersion.Platform switch
        {
            PlatformID.Unix => String.Empty,
            PlatformID.Win32NT => ".exe",
            _ => throw new NotImplementedException("你到底在用什么神经玩意运行这个程序的?"),
        };
        String expectedPath = Path.Combine(javaHomePath, "bin", "java") + execatableExtension;
        if (File.Exists(expectedPath))
            return expectedPath;
        else
            return String.Empty;
    }

    /// <summary>
    /// Scan java form (homebrew,default,ProgramFiles,ProgramFileX86)
    /// </summary>
    /// <returns>
    /// java instances
    /// </returns>
    public static List<JavaInstance> ScanJavaInstances()
    {
        List<JavaInstance> instances = new();
        // Windows 
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            String[] javaVendors = [
                "Java",
                "OpenJDK",
                "Eclipse Adoptium",
                "Amazon Corretto",
                "Zulu",
                "Microsoft",
                "BellSoft",
            ];
            /**
             * 搜索 "C:/Program File(x86)/$VENDOR/*"
             * 以及 "$PATH/java"
             */
            //在ProgramFiles中寻找

            foreach (var path in (
                from prefix in new[] {
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                        @"D:\Program Files",
                        @"D:\Program Files (x86)",
                    }
                where Directory.Exists(prefix)
                from vendor in javaVendors
                let vendorDirectory = new DirectoryInfo(Path.Combine(prefix, vendor))
                where vendorDirectory.Exists
                from javaHomePath in vendorDirectory.GetDirectories()
                where openjdkRegex().IsMatch(javaHomePath.FullName) ||
                    bellsoftLibreicaOpenjdkRegex().IsMatch(javaHomePath.FullName) ||
                    zuluRegex().IsMatch(javaHomePath.FullName) ||
                    oracleRegex().IsMatch(javaHomePath.FullName)
                where !String.IsNullOrEmpty(GetJavaExecatableFilePath(javaHomePath.FullName))
                select javaHomePath.FullName
                )
            )
            {
                instances.Add(CreateJavaInstance(path));
            }

            foreach (var dir in (
                Environment.GetEnvironmentVariable("Path")?.Split(Path.PathSeparator) ?? new String[0]
            ).Select((item) => new DirectoryInfo(item)))
            {
                if (File.Exists(Path.Combine(dir.FullName, "java.exe")) && dir.Parent != null)
                {
                    instances.Add(CreateJavaInstance(dir.Parent!.FullName));
                }
            }
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix) // POSIX
        {
            foreach (var javaHomePath in (
                from jvmPath in new[] {
                    "/usr/lib/jvm",
                    "/usr/lib64/jvm",
                    "/usr/local/lib/jvm",
                    "/opt/java",
                    "/opt/jdk",
                    "/opt/homebrew/lib/jvm", // homebrew in MAC OS
                    $"{Environment.GetEnvironmentVariable("HOME")}/.linuxbrew/lib/jvm", // linuxbrew / Homebrew in Linux
                    $"{Environment.GetEnvironmentVariable("JAVA_HOME")}"
                }
                where !String.IsNullOrEmpty(jvmPath)
                where Directory.Exists(jvmPath)
                from javaHome in new DirectoryInfo(jvmPath).GetDirectories()
                where !String.IsNullOrEmpty(GetJavaExecatableFilePath(javaHome.FullName))
                where !DefalutRegex().IsMatch(javaHome.FullName)
                select javaHome.FullName
                     )
            )
            {
                instances.Add(CreateJavaInstance(javaHomePath));
            }
        }
        return instances;
    }

    [GeneratedRegex(@"Liberica([Jj][Dd][Kk])|([Jj][Rr][Ee])-\d{1,2}-([Ff][Uu][Ll][Ll])|([Ll][Ii][Tt][Ee])|([Ss][Tt][Aa][Nn][Dd][Aa][Rr][Dd])")]
    private static partial Regex bellsoftLibreicaOpenjdkRegex();
    [GeneratedRegex(@"(jdk)|(jre)-[\d]+(\.[\d]+(\.[\d]+(\.[\d]+))?)?(-hotspot)?")]
    private static partial Regex openjdkRegex();
    [GeneratedRegex(@"zulu-[\d]{1,2}(-jre)?")]
    private static partial Regex zuluRegex();
    [GeneratedRegex(@"(jre1.[6789].\d{1,2}_[\d]{1,3})|(jdk[\d]{1,2}\.\d+\.\d+_\d+)||((jdk)|(jre)\d+)")]
    private static partial Regex oracleRegex();
    [GeneratedRegex("[dD][eD][fF][aA][uU][lL][tT]")]
    private static partial Regex DefalutRegex();
}
