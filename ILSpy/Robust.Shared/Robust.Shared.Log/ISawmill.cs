using System;

namespace Robust.Shared.Log;

public interface ISawmill
{
	string Name { get; }

	LogLevel? Level { get; set; }

	void AddHandler(ILogHandler handler);

	void RemoveHandler(ILogHandler handler);

	bool IsLogLevelEnabled(LogLevel level)
	{
		return true;
	}

	void Log(LogLevel level, string message, params object?[] args);

	void Log(LogLevel level, Exception? exception, string message, params object?[] args);

	void Log(LogLevel level, string message);

	void Verbose(string message, params object?[] args)
	{
		Log(LogLevel.Verbose, message, args);
	}

	void Verbose(string message)
	{
		Log(LogLevel.Verbose, message);
	}

	void Debug(string message, params object?[] args);

	void Debug(string message);

	void Info(string message, params object?[] args);

	void Info(string message);

	void Warning(string message, params object?[] args);

	void Warning(string message);

	void Error(string message, params object?[] args);

	void Error(string message);

	void Fatal(string message, params object?[] args);

	void Fatal(string message);
}
