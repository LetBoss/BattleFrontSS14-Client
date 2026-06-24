// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.TimerComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Exceptions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.GameObjects;

[RegisterComponent]
[Obsolete("Use a system update loop instead")]
public sealed class TimerComponent : 
  Component,
  ISerializationGenerated<TimerComponent>,
  ISerializationGenerated
{
  [Robust.Shared.IoC.Dependency]
  private readonly IRuntimeLog _runtimeLog;
  private readonly List<(Robust.Shared.Timing.Timer timer, CancellationToken source)> _timers = new List<(Robust.Shared.Timing.Timer, CancellationToken)>();

  public int TimerCount => this._timers.Count;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool RemoveOnEmpty { get; set; } = true;

  public void Update(float frameTime)
  {
    for (int index = 0; index < this._timers.Count; ++index)
    {
      (Robust.Shared.Timing.Timer timer, CancellationToken source) timer1 = this._timers[index];
      Robust.Shared.Timing.Timer timer2 = timer1.timer;
      if (!timer1.source.IsCancellationRequested)
        timer2.Update(frameTime, this._runtimeLog);
    }
    this._timers.RemoveAll((Predicate<(Robust.Shared.Timing.Timer, CancellationToken)>) (timer => !timer.timer.IsActive || timer.source.IsCancellationRequested));
  }

  public void AddTimer(Robust.Shared.Timing.Timer timer, CancellationToken cancellationToken = default (CancellationToken))
  {
    this._timers.Add((timer, cancellationToken));
  }

  public Task Delay(int milliseconds, CancellationToken cancellationToken = default (CancellationToken))
  {
    TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
    this.Spawn(milliseconds, (Action) (() => tcs.SetResult((object) null)), cancellationToken);
    return (Task) tcs.Task;
  }

  public Task Delay(TimeSpan duration, CancellationToken cancellationToken = default (CancellationToken))
  {
    return this.Delay((int) duration.TotalMilliseconds, cancellationToken);
  }

  public void Spawn(int milliseconds, Action onFired, CancellationToken cancellationToken = default (CancellationToken))
  {
    this.AddTimer(new Robust.Shared.Timing.Timer(milliseconds, false, onFired), cancellationToken);
  }

  public void Spawn(TimeSpan duration, Action onFired, CancellationToken cancellationToken = default (CancellationToken))
  {
    this.Spawn((int) duration.TotalMilliseconds, onFired, cancellationToken);
  }

  public void SpawnRepeating(int milliseconds, Action onFired, CancellationToken cancellationToken)
  {
    this.AddTimer(new Robust.Shared.Timing.Timer(milliseconds, true, onFired), cancellationToken);
  }

  public void SpawnRepeating(
    TimeSpan duration,
    Action onFired,
    CancellationToken cancellationToken)
  {
    this.SpawnRepeating((int) duration.TotalMilliseconds, onFired, cancellationToken);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TimerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TimerComponent) target1;
    serialization.TryCustomCopy<TimerComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TimerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TimerComponent target1 = (TimerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TimerComponent target1 = (TimerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TimerComponent target1 = (TimerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TimerComponent Component.Instantiate() => new TimerComponent();
}
