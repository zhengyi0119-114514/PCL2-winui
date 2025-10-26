using System;
using System.Runtime.InteropServices.Marshalling;

namespace MinecraftLauncherLinrary;

public record struct JavaInstance
{
    public required JavaMajorVersion MajorVersion;
    public required String HomePath;
    public required String Vendor;
    public required String Arch;
    public String Version;
}
public enum JavaMajorVersion : Byte
{
    Unknown = 0,
    JDK5 = 5,
    JDK6 = 6,
    JDK7 = 7,
    JDK8 = 8,
    JDK11 = 11,
    JDK17 = 17,
    JDK21 = 21,
    JDK23 = 23,
}