// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Log.LogManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable enable
namespace Robust.Shared.Log;

public sealed class LogManager : ILogManager, IDisposable
{
  public const string SawmillProperty = "Sawmill";
  public const string ROOT = "root";
  private readonly LogManager.Sawmill rootSawmill;
  private readonly Dictionary<string, LogManager.Sawmill> sawmills = new Dictionary<string, LogManager.Sawmill>();
  private readonly ReaderWriterLockSlim _sawmillsLock = new ReaderWriterLockSlim();

  public ISawmill RootSawmill => (ISawmill) this.rootSawmill;

  public ISawmill GetSawmill(string name)
  {
    this._sawmillsLock.EnterReadLock();
    try
    {
      LogManager.Sawmill sawmill;
      if (this.sawmills.TryGetValue(name, out sawmill))
        return (ISawmill) sawmill;
    }
    finally
    {
      this._sawmillsLock.ExitReadLock();
    }
    this._sawmillsLock.EnterWriteLock();
    try
    {
      return (ISawmill) this._getSawmillUnlocked(name);
    }
    finally
    {
      this._sawmillsLock.ExitWriteLock();
    }
  }

  public IEnumerable<ISawmill> AllSawmills
  {
    get
    {
      using (this._sawmillsLock.ReadGuard())
        return (IEnumerable<ISawmill>) this.sawmills.Values.ToArray<LogManager.Sawmill>();
    }
  }

  private LogManager.Sawmill _getSawmillUnlocked(string name)
  {
    LogManager.Sawmill sawmillUnlocked1;
    if (this.sawmills.TryGetValue(name, out sawmillUnlocked1))
      return sawmillUnlocked1;
    int length = name.LastIndexOf('.');
    LogManager.Sawmill sawmillUnlocked2 = new LogManager.Sawmill(this._getSawmillUnlocked(length != -1 ? name.Substring(0, length) : "root"), name);
    this.sawmills.Add(name, sawmillUnlocked2);
    return sawmillUnlocked2;
  }

  public LogManager()
  {
    this.rootSawmill = new LogManager.Sawmill((LogManager.Sawmill) null, "root")
    {
      Level = new LogLevel?(LogLevel.Debug)
    };
    this.sawmills["root"] = this.rootSawmill;
  }

  public void Dispose()
  {
    foreach (LogManager.Sawmill sawmill in this.sawmills.Values)
      sawmill.Dispose();
  }

  private sealed class Sawmill : ISawmill, IDisposable
  {
    private readonly Serilog.Core.Logger _sLogger = new LoggerConfiguration().CreateLogger();
    private bool _disposed;
    private LogLevel? _level;
    private readonly ReaderWriterLockSlim _handlerLock = new ReaderWriterLockSlim();

    public string Name { get; }

    public LogManager.Sawmill? Parent { get; }

    public LogLevel? Level
    {
      get => this._level;
      set
      {
        if (this.Name == "root" && !value.HasValue)
          throw new ArgumentException("Cannot set root sawmill level to null.");
        this._level = value;
      }
    }

    public List<ILogHandler> Handlers { get; } = new List<ILogHandler>();

    public Sawmill(LogManager.Sawmill? parent, string name)
    {
      this.Parent = parent;
      this.Name = name;
    }

    public void AddHandler(ILogHandler handler)
    {
      this._handlerLock.EnterWriteLock();
      try
      {
        this.Handlers.Add(handler);
      }
      finally
      {
        this._handlerLock.ExitWriteLock();
      }
    }

    public void RemoveHandler(ILogHandler handler)
    {
      this._handlerLock.EnterWriteLock();
      try
      {
        this.Handlers.Remove(handler);
      }
      finally
      {
        this._handlerLock.ExitWriteLock();
      }
    }

    public bool IsLogLevelEnabled(LogLevel level) => level >= this.GetPracticalLevel();

    public void Log(LogLevel level, Exception? exception, string message, params object?[] args)
    {
      MessageTemplate parsedTemplate;
      IEnumerable<LogEventProperty> boundProperties;
      if (!this._sLogger.BindMessageTemplate(message, args, out parsedTemplate, out boundProperties))
        return;
      LogEvent message1 = new LogEvent(DateTimeOffset.Now, level.ToSerilog(), exception, parsedTemplate, boundProperties);
      if (!this.IsLogLevelEnabled(level))
        return;
      this.LogInternal(this.Name, message1);
    }

    public void Log(LogLevel level, string message, params object?[] args)
    {
      if (args.Length != 0 && message.Contains("{0"))
      {
        message = string.Format(message, args);
        args = Array.Empty<object>();
      }
      this.Log(level, (Exception) null, message, args);
    }

    public void Log(LogLevel level, string message)
    {
      this.Log(level, message, Array.Empty<object>());
    }

    private void LogInternal(string sourceSawmill, LogEvent message)
    {
      this._handlerLock.EnterReadLock();
      try
      {
        if (this._disposed)
          throw new ObjectDisposedException(nameof (Sawmill));
        foreach (ILogHandler handler in this.Handlers)
          handler.Log(sourceSawmill, message);
      }
      finally
      {
        this._handlerLock.ExitReadLock();
      }
      this.Parent?.LogInternal(sourceSawmill, message);
    }

    private LogLevel GetPracticalLevel()
    {
      if (this.Level.HasValue)
        return this.Level.Value;
      LogManager.Sawmill parent = this.Parent;
      return parent == null ? LogLevel.Verbose : parent.GetPracticalLevel();
    }

    public void Debug(string message, params object?[] args)
    {
      this.Log(LogLevel.Debug, message, args);
    }

    public void Debug(string message) => this.Log(LogLevel.Debug, message);

    public void Info(string message, params object?[] args)
    {
      this.Log(LogLevel.Info, message, args);
    }

    public void Info(string message) => this.Log(LogLevel.Info, message);

    public void Warning(string message, params object?[] args)
    {
      this.Log(LogLevel.Warning, message, args);
    }

    public void Warning(string message) => this.Log(LogLevel.Warning, message);

    public void Error(string message, params object?[] args)
    {
      this.Log(LogLevel.Error, message, args);
    }

    public void Error(string message) => this.Log(LogLevel.Error, message);

    public void Fatal(string message, params object?[] args)
    {
      this.Log(LogLevel.Fatal, message, args);
    }

    public void Fatal(string message) => this.Log(LogLevel.Fatal, message);

    public void Dispose()
    {
      this._handlerLock.EnterWriteLock();
      try
      {
        this._disposed = true;
        foreach (ILogHandler handler in this.Handlers)
        {
          if (handler is IDisposable disposable)
            disposable.Dispose();
        }
      }
      finally
      {
        this._handlerLock.ExitWriteLock();
      }
    }
  }
}
