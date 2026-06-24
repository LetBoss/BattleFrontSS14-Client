// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.GameLoopEventSource
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Diagnostics.Tracing;

#nullable enable
namespace Robust.Shared.Timing;

[EventSource(Name = "Robust.GameLoop")]
internal sealed class GameLoopEventSource : EventSource
{
  public static GameLoopEventSource Log { get; } = new GameLoopEventSource();

  [Event(1)]
  public void CannotKeepUp() => this.WriteEvent(1);

  [Event(2)]
  public void InputStart() => this.WriteEvent(2);

  [Event(3)]
  public void InputStop() => this.WriteEvent(3);

  [Event(4)]
  public void TickStart(uint tick) => this.WriteEvent(4, (long) tick);

  [Event(5)]
  public void TickStop(uint tick) => this.WriteEvent(5, (long) tick);

  [Event(6)]
  public void UpdateStart() => this.WriteEvent(6);

  [Event(7)]
  public void UpdateStop() => this.WriteEvent(7);

  [Event(8)]
  public void SleepStart() => this.WriteEvent(8);

  [Event(9)]
  public void SleepStop() => this.WriteEvent(9);
}
