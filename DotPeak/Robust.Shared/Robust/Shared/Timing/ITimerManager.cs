// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.ITimerManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System.Threading;

#nullable enable
namespace Robust.Shared.Timing;

[NotContentImplementable]
public interface ITimerManager
{
  void AddTimer(Timer timer, CancellationToken cancellationToken = default (CancellationToken));

  void UpdateTimers(FrameEventArgs frameEventArgs);
}
