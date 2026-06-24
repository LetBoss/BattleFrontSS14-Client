// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Exceptions.IRuntimeLog
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.Exceptions;

[NotContentImplementable]
public interface IRuntimeLog
{
  int ExceptionCount { get; }

  void LogException(Exception exception, string? catcher = null);

  string Display();
}
