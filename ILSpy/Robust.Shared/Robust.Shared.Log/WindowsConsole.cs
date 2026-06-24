using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TerraFX.Interop.Windows;

namespace Robust.Shared.Log;

internal static class WindowsConsole
{
	internal static class NativeMethods
	{
		public const int CodePageUtf8 = 65001;
	}

	private static bool _freedConsole;

	public static bool IsConsoleActive => !_freedConsole;

	public unsafe static bool TryEnableVirtualTerminalProcessing()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			HANDLE stdHandle = Windows.GetStdHandle(4294967285u);
			Unsafe.SkipInit(out uint num);
			Windows.GetConsoleMode(stdHandle, &num);
			Windows.SetConsoleMode(stdHandle, num | 4);
			Windows.GetConsoleMode(stdHandle, &num);
			return (num & 4) == 4;
		}
		catch (DllNotFoundException)
		{
			return false;
		}
		catch (EntryPointNotFoundException)
		{
			return false;
		}
	}

	public static void TryDetachFromConsoleWindow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!(Windows.GetConsoleWindow() == default(HWND)) && !Debugger.IsAttached && !System.Console.IsOutputRedirected && !System.Console.IsErrorRedirected && !System.Console.IsInputRedirected)
		{
			_freedConsole = BOOL.op_Implicit(Windows.FreeConsole());
		}
	}
}
