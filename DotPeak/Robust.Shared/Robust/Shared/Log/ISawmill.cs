// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Log.ISawmill
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Log;

public interface ISawmill
{
  string Name { get; }

  LogLevel? Level { get; set; }

  void AddHandler(ILogHandler handler);

  void RemoveHandler(ILogHandler handler);

  bool IsLogLevelEnabled(LogLevel level) => true;

  void Log(LogLevel level, string message, params object?[] args);

  void Log(LogLevel level, Exception? exception, string message, params object?[] args);

  void Log(LogLevel level, string message);

  void Verbose(string message, params object?[] args) => this.Log(LogLevel.Verbose, message, args);

  void Verbose(string message) => this.Log(LogLevel.Verbose, message);

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
