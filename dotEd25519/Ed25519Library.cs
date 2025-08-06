using System.Reflection;
using System.Runtime.InteropServices;

namespace nebulae.dotEd25519;

internal static class Ed25519Library
{
    private static bool _isLoaded;

    internal static void Init()
    {
        if (_isLoaded)
            return;

        NativeLibrary.SetDllImportResolver(typeof(Ed25519Library).Assembly, Resolve);

        _isLoaded = true;
    }

    private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != "ed25519")
            return IntPtr.Zero;

        var libName = GetPlatformLibraryName();
        var assemblyDir = Path.GetDirectoryName(typeof(Ed25519Library).Assembly.Location)!;
        var fullPath = Path.Combine(assemblyDir, libName);

        if (!File.Exists(fullPath))
            throw new DllNotFoundException($"Could not find native ed25519 library at {fullPath}");

        return NativeLibrary.Load(fullPath);
    }

    private static string GetPlatformLibraryName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Path.Combine("runtimes", "win-x64", "native", "ed25519.dll");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return Path.Combine("runtimes", "linux-x64", "native", "libed25519.so");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                return Path.Combine("runtimes", "osx-arm64", "native", "libed25519.dylib");

            return Path.Combine("runtimes", "osx-x64", "native", "libed25519.dylib");
        }

        throw new PlatformNotSupportedException("Unsupported platform");
    }
}
