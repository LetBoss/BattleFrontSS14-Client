// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.IGameLoop
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Timing;

internal interface IGameLoop
{
  event EventHandler<FrameEventArgs> Input;

  event EventHandler<FrameEventArgs> Tick;

  event EventHandler<FrameEventArgs> Update;

  event EventHandler<FrameEventArgs> Render;

  bool SingleStep { get; set; }

  bool Running { get; set; }

  int MaxQueuedTicks { get; set; }

  TimeSpan LimitMinFrameTime { get; set; }

  SleepMode SleepMode { get; set; }

  void Run();
}
