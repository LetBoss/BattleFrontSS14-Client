#define NETCOREAPP
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Robust.Shared;

internal static class DllMapHelper
{
	[Conditional("NETCOREAPP")]
	public static void RegisterSimpleMap(Assembly assembly, string baseName)
	{
		RegisterExplicitMap(assembly, baseName + ".dll", "lib" + baseName + ".so", "lib" + baseName + ".dylib");
	}

	[Conditional("NETCOREAPP")]
	public static void RegisterExplicitMap(Assembly assembly, string baseName, string linuxName, string macName)
	{
		NativeLibrary.SetDllImportResolver(assembly, delegate(string name, Assembly assembly2, DllImportSearchPath? path)
		{
			if (name != baseName)
			{
				return IntPtr.Zero;
			}
			string text = null;
			if (OperatingSystem.IsLinux())
			{
				text = linuxName;
			}
			if (OperatingSystem.IsMacOS())
			{
				text = macName;
			}
			nint handle;
			return (text != null && NativeLibrary.TryLoad(text, assembly2, path, out handle)) ? handle : IntPtr.Zero;
		});
	}
}
