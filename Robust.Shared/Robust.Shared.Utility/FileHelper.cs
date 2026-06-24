using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using TerraFX.Interop.Windows;

namespace Robust.Shared.Utility;

internal static class FileHelper
{
	public static bool TryOpenFileRead(string path, [NotNullWhen(true)] out FileStream? stream)
	{
		if (OperatingSystem.IsWindows())
		{
			return TryGetFileWindows(path, out stream);
		}
		if (!File.Exists(path))
		{
			stream = null;
			return false;
		}
		stream = File.OpenRead(path);
		return true;
	}

	private unsafe static bool TryGetFileWindows(string path, [NotNullWhen(true)] out FileStream? stream)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (path.EndsWith("\\"))
		{
			stream = null;
			return false;
		}
		try
		{
			HANDLE val;
			fixed (char* ptr = path)
			{
				val = Windows.CreateFileW(ptr, 2147483648u, 1u, (SECURITY_ATTRIBUTES*)null, 3u, 128u, HANDLE.NULL);
			}
			if (val == HANDLE.INVALID_VALUE)
			{
				int lastSystemError = Marshal.GetLastSystemError();
				if ((uint)(lastSystemError - 2) <= 1u)
				{
					stream = null;
					return false;
				}
				Marshal.ThrowExceptionForHR(HRESULT.op_Implicit(Windows.HRESULT_FROM_WIN32(lastSystemError)));
			}
			SafeFileHandle handle = new SafeFileHandle(HANDLE.op_Implicit(val), ownsHandle: true);
			stream = new FileStream(handle, FileAccess.Read);
			return true;
		}
		catch (UnauthorizedAccessException)
		{
			if (Directory.Exists(path))
			{
				stream = null;
				return false;
			}
			throw;
		}
	}
}
