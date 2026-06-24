// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Log.Logger
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using System;

#nullable enable
namespace Robust.Shared.Log;

public static class Logger
{
  private static ILogManager LogManagerSingleton => IoCManager.Resolve<ILogManager>();

  public static ISawmill GetSawmill(string name) => Logger.LogManagerSingleton.GetSawmill(name);

  [Obsolete("Use ISawmill.Log")]
  public static void LogS(
    LogLevel logLevel,
    string sawmillname,
    string message,
    params object?[] args)
  {
    Logger.LogManagerSingleton.GetSawmill(sawmillname).Log(logLevel, message, args);
  }

  [Obsolete("Use ISawmill.Log")]
  public static void LogS(
    LogLevel logLevel,
    string sawmillname,
    Exception? exception,
    string message,
    params object?[] args)
  {
    Logger.LogManagerSingleton.GetSawmill(sawmillname).Log(logLevel, exception, message, args);
  }

  [Obsolete("Use ISawmill.Log")]
  public static void LogS(LogLevel logLevel, string sawmillname, string message)
  {
    Logger.LogManagerSingleton.GetSawmill(sawmillname).Log(logLevel, message);
  }

  [Obsolete("Use ISawmill.Log")]
  public static void Log(LogLevel logLevel, string message, params object?[] args)
  {
    Logger.LogManagerSingleton.RootSawmill.Log(logLevel, message, args);
  }

  [Obsolete("Use ISawmill.Log")]
  public static void Log(
    LogLevel logLevel,
    Exception exception,
    string message,
    params object?[] args)
  {
    Logger.LogManagerSingleton.RootSawmill.Log(logLevel, message, args);
  }

  [Obsolete("Use ISawmill.Log")]
  public static void Log(LogLevel logLevel, string message)
  {
    Logger.LogManagerSingleton.RootSawmill.Log(logLevel, message);
  }

  [Obsolete("Use ISawmill.Log")]
  public static void DebugS(string sawmill, string message, params object?[] args)
  {
    Logger.LogS(LogLevel.Debug, sawmill, message, args);
  }

  [Obsolete("Use ISawmill.Log")]
  public static void DebugS(string sawmill, string message)
  {
    Logger.LogS(LogLevel.Debug, sawmill, message);
  }

  [Obsolete("Use ISawmill.Debug")]
  public static void Debug(string message) => Logger.Log(LogLevel.Debug, message);

  [Obsolete("Use ISawmill.Info")]
  public static void InfoS(string sawmill, string message, params object?[] args)
  {
    Logger.LogS(LogLevel.Info, sawmill, message, args);
  }

  [Obsolete("Use ISawmill.Info")]
  public static void InfoS(string sawmill, string message)
  {
    Logger.LogS(LogLevel.Info, sawmill, message);
  }

  [Obsolete("Use ISawmill.Info")]
  public static void Info(string message) => Logger.Log(LogLevel.Info, message);

  [Obsolete("Use ISawmill.Warning")]
  public static void WarningS(string sawmill, string message, params object?[] args)
  {
    Logger.LogS(LogLevel.Warning, sawmill, message, args);
  }

  [Obsolete("Use ISawmill.Warning")]
  public static void WarningS(string sawmill, string message)
  {
    Logger.LogS(LogLevel.Warning, sawmill, message);
  }

  [Obsolete("Use ISawmill.Warning")]
  public static void Warning(string message) => Logger.Log(LogLevel.Warning, message);

  [Obsolete("Use ISawmill.Error")]
  public static void ErrorS(string sawmill, string message, params object?[] args)
  {
    Logger.LogS(LogLevel.Error, sawmill, message, args);
  }

  [Obsolete("Use ISawmill.Error")]
  public static void ErrorS(string sawmill, string message)
  {
    Logger.LogS(LogLevel.Error, sawmill, message);
  }

  [Obsolete("Use ISawmill.Error")]
  public static void Error(string message, params object?[] args)
  {
    Logger.Log(LogLevel.Error, message, args);
  }

  [Obsolete("Use ISawmill.Error")]
  public static void Error(string message) => Logger.Log(LogLevel.Error, message);
}
