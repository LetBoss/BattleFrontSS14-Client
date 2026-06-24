using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Shared.Utility;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Robust.Shared.Log;

public sealed class LogManager : ILogManager, IDisposable
{
	private sealed class Sawmill : ISawmill, IDisposable
	{
		private readonly Logger _sLogger = new LoggerConfiguration().CreateLogger();

		private bool _disposed;

		private LogLevel? _level;

		private readonly ReaderWriterLockSlim _handlerLock = new ReaderWriterLockSlim();

		public string Name { get; }

		public Sawmill? Parent { get; }

		public LogLevel? Level
		{
			get
			{
				return _level;
			}
			set
			{
				if (Name == "root" && !value.HasValue)
				{
					throw new ArgumentException("Cannot set root sawmill level to null.");
				}
				_level = value;
			}
		}

		public List<ILogHandler> Handlers { get; } = new List<ILogHandler>();

		public Sawmill(Sawmill? parent, string name)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			Parent = parent;
			Name = name;
		}

		public void AddHandler(ILogHandler handler)
		{
			_handlerLock.EnterWriteLock();
			try
			{
				Handlers.Add(handler);
			}
			finally
			{
				_handlerLock.ExitWriteLock();
			}
		}

		public void RemoveHandler(ILogHandler handler)
		{
			_handlerLock.EnterWriteLock();
			try
			{
				Handlers.Remove(handler);
			}
			finally
			{
				_handlerLock.ExitWriteLock();
			}
		}

		public bool IsLogLevelEnabled(LogLevel level)
		{
			return level >= GetPracticalLevel();
		}

		public void Log(LogLevel level, Exception? exception, string message, params object?[] args)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			Unsafe.SkipInit(out MessageTemplate val);
			Unsafe.SkipInit(out IEnumerable<LogEventProperty> enumerable);
			if (_sLogger.BindMessageTemplate(message, args, ref val, ref enumerable))
			{
				LogEvent message2 = new LogEvent(DateTimeOffset.Now, level.ToSerilog(), exception, val, enumerable);
				if (IsLogLevelEnabled(level))
				{
					LogInternal(Name, message2);
				}
			}
		}

		public void Log(LogLevel level, string message, params object?[] args)
		{
			if (args.Length != 0 && message.Contains("{0"))
			{
				message = string.Format(message, args);
				args = Array.Empty<object>();
			}
			Log(level, null, message, args);
		}

		public void Log(LogLevel level, string message)
		{
			Log(level, message, Array.Empty<object>());
		}

		private void LogInternal(string sourceSawmill, LogEvent message)
		{
			_handlerLock.EnterReadLock();
			try
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("Sawmill");
				}
				foreach (ILogHandler handler in Handlers)
				{
					handler.Log(sourceSawmill, message);
				}
			}
			finally
			{
				_handlerLock.ExitReadLock();
			}
			Parent?.LogInternal(sourceSawmill, message);
		}

		private LogLevel GetPracticalLevel()
		{
			if (Level.HasValue)
			{
				return Level.Value;
			}
			return Parent?.GetPracticalLevel() ?? LogLevel.Verbose;
		}

		public void Debug(string message, params object?[] args)
		{
			Log(LogLevel.Debug, message, args);
		}

		public void Debug(string message)
		{
			Log(LogLevel.Debug, message);
		}

		public void Info(string message, params object?[] args)
		{
			Log(LogLevel.Info, message, args);
		}

		public void Info(string message)
		{
			Log(LogLevel.Info, message);
		}

		public void Warning(string message, params object?[] args)
		{
			Log(LogLevel.Warning, message, args);
		}

		public void Warning(string message)
		{
			Log(LogLevel.Warning, message);
		}

		public void Error(string message, params object?[] args)
		{
			Log(LogLevel.Error, message, args);
		}

		public void Error(string message)
		{
			Log(LogLevel.Error, message);
		}

		public void Fatal(string message, params object?[] args)
		{
			Log(LogLevel.Fatal, message, args);
		}

		public void Fatal(string message)
		{
			Log(LogLevel.Fatal, message);
		}

		public void Dispose()
		{
			_handlerLock.EnterWriteLock();
			try
			{
				_disposed = true;
				foreach (ILogHandler handler in Handlers)
				{
					if (handler is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
			}
			finally
			{
				_handlerLock.ExitWriteLock();
			}
		}
	}

	public const string SawmillProperty = "Sawmill";

	public const string ROOT = "root";

	private readonly Sawmill rootSawmill;

	private readonly Dictionary<string, Sawmill> sawmills = new Dictionary<string, Sawmill>();

	private readonly ReaderWriterLockSlim _sawmillsLock = new ReaderWriterLockSlim();

	public ISawmill RootSawmill => rootSawmill;

	public IEnumerable<ISawmill> AllSawmills
	{
		get
		{
			using (_sawmillsLock.ReadGuard())
			{
				return sawmills.Values.ToArray();
			}
		}
	}

	public ISawmill GetSawmill(string name)
	{
		_sawmillsLock.EnterReadLock();
		try
		{
			if (sawmills.TryGetValue(name, out Sawmill value))
			{
				return value;
			}
		}
		finally
		{
			_sawmillsLock.ExitReadLock();
		}
		_sawmillsLock.EnterWriteLock();
		try
		{
			return _getSawmillUnlocked(name);
		}
		finally
		{
			_sawmillsLock.ExitWriteLock();
		}
	}

	private Sawmill _getSawmillUnlocked(string name)
	{
		if (sawmills.TryGetValue(name, out Sawmill value))
		{
			return value;
		}
		int num = name.LastIndexOf('.');
		string name2 = ((num != -1) ? name.Substring(0, num) : "root");
		value = new Sawmill(_getSawmillUnlocked(name2), name);
		sawmills.Add(name, value);
		return value;
	}

	public LogManager()
	{
		rootSawmill = new Sawmill(null, "root")
		{
			Level = LogLevel.Debug
		};
		sawmills["root"] = rootSawmill;
	}

	public void Dispose()
	{
		foreach (Sawmill value in sawmills.Values)
		{
			value.Dispose();
		}
	}
}
