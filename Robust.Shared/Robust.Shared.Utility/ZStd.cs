using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpZstd.Interop;

namespace Robust.Shared.Utility;

public static class ZStd
{
	public static int CompressBound(int length)
	{
		return (int)(nuint)Zstd.ZSTD_COMPRESSBOUND((UIntPtr)(nuint)length);
	}

	public unsafe static int Compress(Span<byte> into, ReadOnlySpan<byte> data, int compressionLevel = 3)
	{
		fixed (byte* ptr = into)
		{
			fixed (byte* ptr2 = data)
			{
				UIntPtr num = Zstd.ZSTD_compress((void*)ptr, (UIntPtr)(nuint)into.Length, (void*)ptr2, (UIntPtr)(nuint)data.Length, compressionLevel);
				ZStdException.ThrowIfError(num);
				return (int)(nuint)num;
			}
		}
	}

	[ModuleInitializer]
	internal static void InitZStd()
	{
		try
		{
			NativeLibrary.SetDllImportResolver(typeof(Zstd).Assembly, ResolveZstd);
		}
		catch (InvalidOperationException)
		{
		}
	}

	private static nint ResolveZstd(string name, Assembly assembly, DllImportSearchPath? path)
	{
		if (name == "zstd")
		{
			nint handle2;
			if (OperatingSystem.IsLinux())
			{
				if (NativeLibrary.TryLoad("zstd.so", assembly, path, out var handle))
				{
					return handle;
				}
				if (NativeLibrary.TryLoad("libzstd.so.1", assembly, path, out handle))
				{
					return handle;
				}
				if (NativeLibrary.TryLoad("libzstd.so", assembly, path, out handle))
				{
					return handle;
				}
			}
			else if (OperatingSystem.IsMacOS() && NativeLibrary.TryLoad("libzstd.1.dylib", assembly, path, out handle2))
			{
				return handle2;
			}
		}
		return IntPtr.Zero;
	}
}
