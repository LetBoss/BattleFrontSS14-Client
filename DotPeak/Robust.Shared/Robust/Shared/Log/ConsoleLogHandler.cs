// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Log.ConsoleLogHandler
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Serilog.Events;
using System;
using System.IO;
using System.Text;
using System.Text.Unicode;
using System.Timers;

#nullable enable
namespace Robust.Shared.Log;

public sealed class ConsoleLogHandler : ILogHandler, IDisposable
{
  private static readonly bool WriteAnsiColors = !Console.IsOutputRedirected;
  private const string AnsiCsi = "\u001B[";
  private const string AnsiFgDefault = "\u001B[39m";
  private const string AnsiFgBlack = "\u001B[30m";
  private const string AnsiFgRed = "\u001B[31m";
  private const string AnsiFgBrightRed = "\u001B[91m";
  private const string AnsiFgGreen = "\u001B[32m";
  private const string AnsiFgBrightGreen = "\u001B[92m";
  private const string AnsiFgYellow = "\u001B[33m";
  private const string AnsiFgBrightYellow = "\u001B[93m";
  private const string AnsiFgBlue = "\u001B[34m";
  private const string AnsiFgBrightBlue = "\u001B[94m";
  private const string AnsiFgMagenta = "\u001B[35m";
  private const string AnsiFgBrightMagenta = "\u001B[95m";
  private const string AnsiFgCyan = "\u001B[36m";
  private const string AnsiFgBrightCyan = "\u001B[96m";
  private const string AnsiFgWhite = "\u001B[37m";
  private const string AnsiFgBrightWhite = "\u001B[97m";
  private const string LogBeforeLevel = "\u001B[39m[";
  private const string LogAfterLevel = "\u001B[39m] ";
  private readonly Stream _stream = (Stream) new BufferedStream(Console.OpenStandardOutput(), 131072 /*0x020000*/);
  private readonly StringBuilder _line = new StringBuilder(1024 /*0x0400*/);
  private readonly Timer _timer = new Timer(0.1);
  private bool _disposed;

  static ConsoleLogHandler()
  {
    if (ConsoleLogHandler.WriteAnsiColors && OperatingSystem.IsWindows())
      ConsoleLogHandler.WriteAnsiColors = WindowsConsole.TryEnableVirtualTerminalProcessing();
    try
    {
      Console.OutputEncoding = Encoding.UTF8;
    }
    catch
    {
    }
  }

  public ConsoleLogHandler()
  {
    this._timer.Start();
    this._timer.Elapsed += (ElapsedEventHandler) ((sender, args) =>
    {
      lock (this._stream)
      {
        if (!this.IsConsoleActive || this._disposed)
          return;
        this._stream.Flush();
      }
    });
  }

  public static void TryDetachFromConsoleWindow()
  {
    if (!OperatingSystem.IsWindows())
      return;
    WindowsConsole.TryDetachFromConsoleWindow();
  }

  private bool IsConsoleActive => !OperatingSystem.IsWindows() || WindowsConsole.IsConsoleActive;

  public void Log(string sawmillName, LogEvent message)
  {
    LogLevel robust = message.Level.ToRobust();
    lock (this._stream)
    {
      this._line.Clear().Append(ConsoleLogHandler.LogLevelToString(robust)).Append(sawmillName).Append(": ").AppendLine(message.RenderMessage());
      if (message.Exception != null)
        this._line.AppendLine(message.Exception.ToString());
      if (Console.OutputEncoding.CodePage == 65001)
      {
        Span<byte> destination = stackalloc byte[1024 /*0x0400*/];
        int length1 = this._line.Length;
        StringBuilder.ChunkEnumerator enumerator = this._line.GetChunks().GetEnumerator();
label_8:
        while (enumerator.MoveNext())
        {
          ReadOnlyMemory<char> current = enumerator.Current;
          int length2 = current.Length;
          int num1 = 0;
          ReadOnlySpan<char> source = current.Span;
          while (true)
          {
            bool isFinalBlock = num1 + length2 >= length1;
            int charsRead;
            int bytesWritten;
            int num2 = (int) Utf8.FromUtf16(source, destination, out charsRead, out bytesWritten, isFinalBlock: isFinalBlock);
            this._stream.Write((ReadOnlySpan<byte>) destination.Slice(0, bytesWritten));
            num1 += charsRead;
            if (charsRead < length2)
            {
              ref ReadOnlySpan<char> local = ref source;
              int start = charsRead;
              source = local.Slice(start, local.Length - start);
              length2 -= charsRead;
            }
            else
              goto label_8;
          }
        }
      }
      else
        Console.Write(this._line.ToString());
      if (robust < LogLevel.Error || !this.IsConsoleActive)
        return;
      this._stream.Flush();
    }
  }

  internal static string LogLevelToString(LogLevel level)
  {
    if (ConsoleLogHandler.WriteAnsiColors)
    {
      string str;
      switch (level)
      {
        case LogLevel.Verbose:
          str = "\u001B[39m[\u001B[32mVERB\u001B[39m] ";
          break;
        case LogLevel.Debug:
          str = "\u001B[39m[\u001B[34mDEBG\u001B[39m] ";
          break;
        case LogLevel.Info:
          str = "\u001B[39m[\u001B[96mINFO\u001B[39m] ";
          break;
        case LogLevel.Warning:
          str = "\u001B[39m[\u001B[93mWARN\u001B[39m] ";
          break;
        case LogLevel.Error:
          str = "\u001B[39m[\u001B[91mERRO\u001B[39m] ";
          break;
        case LogLevel.Fatal:
          str = "\u001B[39m[\u001B[95mFATL\u001B[39m] ";
          break;
        default:
          str = "\u001B[39m[\u001B[37mUNKO\u001B[39m] ";
          break;
      }
      return str;
    }
    string str1;
    switch (level)
    {
      case LogLevel.Verbose:
        str1 = "[VERB] ";
        break;
      case LogLevel.Debug:
        str1 = "[DEBG] ";
        break;
      case LogLevel.Info:
        str1 = "[INFO] ";
        break;
      case LogLevel.Warning:
        str1 = "[WARN] ";
        break;
      case LogLevel.Error:
        str1 = "[ERRO] ";
        break;
      case LogLevel.Fatal:
        str1 = "[FATL] ";
        break;
      default:
        str1 = "[UNKO] ";
        break;
    }
    return str1;
  }

  public void Dispose()
  {
    lock (this._stream)
    {
      this._disposed = true;
      this._timer.Dispose();
      this._stream.Dispose();
    }
  }
}
