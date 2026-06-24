using System;
using System.Runtime.InteropServices;
using SharpZstd.Interop;

namespace Robust.Shared.Utility;

[Serializable]
internal sealed class ZStdException : Exception
{
	public ZStdException()
	{
	}

	public ZStdException(string message)
		: base(message)
	{
	}

	public ZStdException(string message, Exception inner)
		: base(message, inner)
	{
	}

	public unsafe static ZStdException FromCode(nuint code)
	{
		return new ZStdException(Marshal.PtrToStringUTF8((nint)Zstd.ZSTD_getErrorName((UIntPtr)code)));
	}

	public static void ThrowIfError(nuint code)
	{
		if (Zstd.ZSTD_isError((UIntPtr)code) != 0)
		{
			throw FromCode(code);
		}
	}
}
