// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Log.LogExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Serilog.Events;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.Log;

public static class LogExt
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static LogLevel ToRobust(this LogEventLevel level) => (LogLevel) level;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static LogEventLevel ToSerilog(this LogLevel level) => (LogEventLevel) level;
}
