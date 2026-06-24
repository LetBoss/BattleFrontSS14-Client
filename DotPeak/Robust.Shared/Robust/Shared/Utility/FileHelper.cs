// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.FileHelper
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

#nullable enable
namespace Robust.Shared.Utility;

internal static class FileHelper
{
  public static bool TryOpenFileRead(string path, [NotNullWhen(true)] out FileStream? stream)
  {
    if (OperatingSystem.IsWindows())
      return FileHelper.TryGetFileWindows(path, out stream);
    if (!File.Exists(path))
    {
      stream = (FileStream) null;
      return false;
    }
    stream = File.OpenRead(path);
    return true;
  }

  private static unsafe bool TryGetFileWindows(string path, [NotNullWhen(true)] out FileStream? stream)
  {
    if (path.EndsWith("\\"))
    {
      stream = (FileStream) null;
      return false;
    }
    try
    {
      HANDLE fileW;
      try
      {
        IntPtr lpFileName;
        if (path == null)
        {
          lpFileName = IntPtr.Zero;
        }
        else
        {
          fixed (char* chPtr = &path.GetPinnableReference())
            lpFileName = (IntPtr) chPtr;
        }
        HANDLE hTemplateFile = HANDLE.NULL;
        fileW = TerraFX.Interop.Windows.Windows.CreateFileW((char*) lpFileName, 2147483648U /*0x80000000*/, 1U, (SECURITY_ATTRIBUTES*) IntPtr.Zero, 3U, 128U /*0x80*/, hTemplateFile);
      }
      finally
      {
        // ISSUE: fixed variable is out of scope
        // ISSUE: __unpin statement
        __unpin(chPtr);
      }
      if (fileW == HANDLE.INVALID_VALUE)
      {
        int lastSystemError = Marshal.GetLastSystemError();
        bool flag;
        switch (lastSystemError)
        {
          case 2:
          case 3:
            flag = true;
            break;
          default:
            flag = false;
            break;
        }
        if (flag)
        {
          stream = (FileStream) null;
          return false;
        }
        Marshal.ThrowExceptionForHR((int) TerraFX.Interop.Windows.Windows.HRESULT_FROM_WIN32(lastSystemError));
      }
      SafeFileHandle handle = new SafeFileHandle((IntPtr) fileW, true);
      stream = new FileStream(handle, FileAccess.Read);
      return true;
    }
    catch (UnauthorizedAccessException ex)
    {
      if (Directory.Exists(path))
      {
        stream = (FileStream) null;
        return false;
      }
      throw;
    }
  }
}
