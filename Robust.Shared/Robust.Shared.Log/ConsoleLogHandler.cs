using System;
using System.IO;
using System.Text;
using System.Text.Unicode;
using System.Timers;
using Serilog.Events;

namespace Robust.Shared.Log;

public sealed class ConsoleLogHandler : ILogHandler, IDisposable
{
	private static readonly bool WriteAnsiColors;

	private const string AnsiCsi = "\u001b[";

	private const string AnsiFgDefault = "\u001b[39m";

	private const string AnsiFgBlack = "\u001b[30m";

	private const string AnsiFgRed = "\u001b[31m";

	private const string AnsiFgBrightRed = "\u001b[91m";

	private const string AnsiFgGreen = "\u001b[32m";

	private const string AnsiFgBrightGreen = "\u001b[92m";

	private const string AnsiFgYellow = "\u001b[33m";

	private const string AnsiFgBrightYellow = "\u001b[93m";

	private const string AnsiFgBlue = "\u001b[34m";

	private const string AnsiFgBrightBlue = "\u001b[94m";

	private const string AnsiFgMagenta = "\u001b[35m";

	private const string AnsiFgBrightMagenta = "\u001b[95m";

	private const string AnsiFgCyan = "\u001b[36m";

	private const string AnsiFgBrightCyan = "\u001b[96m";

	private const string AnsiFgWhite = "\u001b[37m";

	private const string AnsiFgBrightWhite = "\u001b[97m";

	private const string LogBeforeLevel = "\u001b[39m[";

	private const string LogAfterLevel = "\u001b[39m] ";

	private readonly Stream _stream = new BufferedStream(System.Console.OpenStandardOutput(), 131072);

	private readonly StringBuilder _line = new StringBuilder(1024);

	private readonly Timer _timer = new Timer(0.1);

	private bool _disposed;

	private bool IsConsoleActive
	{
		get
		{
			if (OperatingSystem.IsWindows())
			{
				return WindowsConsole.IsConsoleActive;
			}
			return true;
		}
	}

	static ConsoleLogHandler()
	{
		WriteAnsiColors = !System.Console.IsOutputRedirected;
		if (WriteAnsiColors && OperatingSystem.IsWindows())
		{
			WriteAnsiColors = WindowsConsole.TryEnableVirtualTerminalProcessing();
		}
		try
		{
			System.Console.OutputEncoding = Encoding.UTF8;
		}
		catch
		{
		}
	}

	public ConsoleLogHandler()
	{
		_timer.Start();
		_timer.Elapsed += delegate
		{
			lock (_stream)
			{
				if (IsConsoleActive && !_disposed)
				{
					_stream.Flush();
				}
			}
		};
	}

	public static void TryDetachFromConsoleWindow()
	{
		if (OperatingSystem.IsWindows())
		{
			WindowsConsole.TryDetachFromConsoleWindow();
		}
	}

	public void Log(string sawmillName, LogEvent message)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		LogLevel logLevel = message.Level.ToRobust();
		lock (_stream)
		{
			_line.Clear().Append(LogLevelToString(logLevel)).Append(sawmillName)
				.Append(": ")
				.AppendLine(message.RenderMessage((IFormatProvider)null));
			if (message.Exception != null)
			{
				_line.AppendLine(message.Exception.ToString());
			}
			if (System.Console.OutputEncoding.CodePage == 65001)
			{
				Span<byte> destination = stackalloc byte[1024];
				int length = _line.Length;
				StringBuilder.ChunkEnumerator enumerator = _line.GetChunks().GetEnumerator();
				while (enumerator.MoveNext())
				{
					ReadOnlyMemory<char> current = enumerator.Current;
					int num = current.Length;
					int num2 = 0;
					ReadOnlySpan<char> source = current.Span;
					while (true)
					{
						bool isFinalBlock = num2 + num >= length;
						Utf8.FromUtf16(source, destination, out var charsRead, out var bytesWritten, replaceInvalidSequences: true, isFinalBlock);
						_stream.Write(destination.Slice(0, bytesWritten));
						num2 += charsRead;
						if (charsRead >= num)
						{
							break;
						}
						int num3 = charsRead;
						source = source.Slice(num3, source.Length - num3);
						num -= charsRead;
					}
				}
			}
			else
			{
				System.Console.Write(_line.ToString());
			}
			if (logLevel >= LogLevel.Error && IsConsoleActive)
			{
				_stream.Flush();
			}
		}
	}

	internal static string LogLevelToString(LogLevel level)
	{
		if (WriteAnsiColors)
		{
			return level switch
			{
				LogLevel.Verbose => "\u001b[39m[\u001b[32mVERB\u001b[39m] ", 
				LogLevel.Debug => "\u001b[39m[\u001b[34mDEBG\u001b[39m] ", 
				LogLevel.Info => "\u001b[39m[\u001b[96mINFO\u001b[39m] ", 
				LogLevel.Warning => "\u001b[39m[\u001b[93mWARN\u001b[39m] ", 
				LogLevel.Error => "\u001b[39m[\u001b[91mERRO\u001b[39m] ", 
				LogLevel.Fatal => "\u001b[39m[\u001b[95mFATL\u001b[39m] ", 
				_ => "\u001b[39m[\u001b[37mUNKO\u001b[39m] ", 
			};
		}
		return level switch
		{
			LogLevel.Verbose => "[VERB] ", 
			LogLevel.Debug => "[DEBG] ", 
			LogLevel.Info => "[INFO] ", 
			LogLevel.Warning => "[WARN] ", 
			LogLevel.Error => "[ERRO] ", 
			LogLevel.Fatal => "[FATL] ", 
			_ => "[UNKO] ", 
		};
	}

	public void Dispose()
	{
		lock (_stream)
		{
			_disposed = true;
			_timer.Dispose();
			_stream.Dispose();
		}
	}
}
