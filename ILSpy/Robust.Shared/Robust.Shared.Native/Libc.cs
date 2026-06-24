using System.Runtime.InteropServices;

namespace Robust.Shared.Native;

internal static class Libc
{
	public const int RTLD_LAZY = 1;

	public const int RTLD_NOW = 2;

	public const int RTLD_BINDING_MASK = 3;

	public const int RTLD_NOLOAD = 4;

	public const int RTLD_DEEPBIND = 8;

	public const int RTLD_GLOBAL = 256;

	public const int RTLD_LOCAL = 0;

	public const int RTLD_NODELETE = 4096;

	[DllImport("libdl.so.2")]
	public static extern nint dlopen([MarshalAs(UnmanagedType.LPUTF8Str)] string name, int flags);
}
