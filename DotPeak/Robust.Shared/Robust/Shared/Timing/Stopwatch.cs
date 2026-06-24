// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.Stopwatch
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Timing;

public sealed class Stopwatch : IStopwatch
{
  private readonly System.Diagnostics.Stopwatch _stopwatch;

  public Stopwatch() => this._stopwatch = new System.Diagnostics.Stopwatch();

  public TimeSpan Elapsed => this._stopwatch.Elapsed;

  public void Start() => this._stopwatch.Start();

  public void Restart() => this._stopwatch.Restart();
}
