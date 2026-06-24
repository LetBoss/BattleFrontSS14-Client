// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Log.WindowsConsole
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Diagnostics;
using TerraFX.Interop.Windows;

#nullable disable
namespace Robust.Shared.Log;

internal static class WindowsConsole
{
  private static bool _freedConsole;

  public static unsafe bool TryEnableVirtualTerminalProcessing()
  {
    try
    {
      HANDLE stdHandle = TerraFX.Interop.Windows.Windows.GetStdHandle(4294967285U);
      uint num;
      TerraFX.Interop.Windows.Windows.GetConsoleMode(stdHandle, &num);
      TerraFX.Interop.Windows.Windows.SetConsoleMode(stdHandle, num | 4U);
      TerraFX.Interop.Windows.Windows.GetConsoleMode(stdHandle, &num);
      return ((int) num & 4) == 4;
    }
    catch (DllNotFoundException ex)
    {
      return false;
    }
    catch (EntryPointNotFoundException ex)
    {
      return false;
    }
  }

  public static bool IsConsoleActive => !WindowsConsole._freedConsole;

  public static void TryDetachFromConsoleWindow()
  {
    if (TerraFX.Interop.Windows.Windows.GetConsoleWindow() == new HWND() || Debugger.IsAttached || Console.IsOutputRedirected || Console.IsErrorRedirected || Console.IsInputRedirected)
      return;
    WindowsConsole._freedConsole = (bool) TerraFX.Interop.Windows.Windows.FreeConsole();
  }

  internal static class NativeMethods
  {
    public const int CodePageUtf8 = 65001;
  }
}
