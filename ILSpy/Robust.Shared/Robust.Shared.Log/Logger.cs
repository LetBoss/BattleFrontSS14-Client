using System;
using Robust.Shared.IoC;

namespace Robust.Shared.Log;

public static class Logger
{
	private static ILogManager LogManagerSingleton => IoCManager.Resolve<ILogManager>();

	public static ISawmill GetSawmill(string name)
	{
		return LogManagerSingleton.GetSawmill(name);
	}

	[Obsolete("Use ISawmill.Log")]
	public static void LogS(LogLevel logLevel, string sawmillname, string message, params object?[] args)
	{
		LogManagerSingleton.GetSawmill(sawmillname).Log(logLevel, message, args);
	}

	[Obsolete("Use ISawmill.Log")]
	public static void LogS(LogLevel logLevel, string sawmillname, Exception? exception, string message, params object?[] args)
	{
		LogManagerSingleton.GetSawmill(sawmillname).Log(logLevel, exception, message, args);
	}

	[Obsolete("Use ISawmill.Log")]
	public static void LogS(LogLevel logLevel, string sawmillname, string message)
	{
		LogManagerSingleton.GetSawmill(sawmillname).Log(logLevel, message);
	}

	[Obsolete("Use ISawmill.Log")]
	public static void Log(LogLevel logLevel, string message, params object?[] args)
	{
		LogManagerSingleton.RootSawmill.Log(logLevel, message, args);
	}

	[Obsolete("Use ISawmill.Log")]
	public static void Log(LogLevel logLevel, Exception exception, string message, params object?[] args)
	{
		LogManagerSingleton.RootSawmill.Log(logLevel, message, args);
	}

	[Obsolete("Use ISawmill.Log")]
	public static void Log(LogLevel logLevel, string message)
	{
		LogManagerSingleton.RootSawmill.Log(logLevel, message);
	}

	[Obsolete("Use ISawmill.Log")]
	public static void DebugS(string sawmill, string message, params object?[] args)
	{
		LogS(LogLevel.Debug, sawmill, message, args);
	}

	[Obsolete("Use ISawmill.Log")]
	public static void DebugS(string sawmill, string message)
	{
		LogS(LogLevel.Debug, sawmill, message);
	}

	[Obsolete("Use ISawmill.Debug")]
	public static void Debug(string message)
	{
		Log(LogLevel.Debug, message);
	}

	[Obsolete("Use ISawmill.Info")]
	public static void InfoS(string sawmill, string message, params object?[] args)
	{
		LogS(LogLevel.Info, sawmill, message, args);
	}

	[Obsolete("Use ISawmill.Info")]
	public static void InfoS(string sawmill, string message)
	{
		LogS(LogLevel.Info, sawmill, message);
	}

	[Obsolete("Use ISawmill.Info")]
	public static void Info(string message)
	{
		Log(LogLevel.Info, message);
	}

	[Obsolete("Use ISawmill.Warning")]
	public static void WarningS(string sawmill, string message, params object?[] args)
	{
		LogS(LogLevel.Warning, sawmill, message, args);
	}

	[Obsolete("Use ISawmill.Warning")]
	public static void WarningS(string sawmill, string message)
	{
		LogS(LogLevel.Warning, sawmill, message);
	}

	[Obsolete("Use ISawmill.Warning")]
	public static void Warning(string message)
	{
		Log(LogLevel.Warning, message);
	}

	[Obsolete("Use ISawmill.Error")]
	public static void ErrorS(string sawmill, string message, params object?[] args)
	{
		LogS(LogLevel.Error, sawmill, message, args);
	}

	[Obsolete("Use ISawmill.Error")]
	public static void ErrorS(string sawmill, string message)
	{
		LogS(LogLevel.Error, sawmill, message);
	}

	[Obsolete("Use ISawmill.Error")]
	public static void Error(string message, params object?[] args)
	{
		Log(LogLevel.Error, message, args);
	}

	[Obsolete("Use ISawmill.Error")]
	public static void Error(string message)
	{
		Log(LogLevel.Error, message);
	}
}
