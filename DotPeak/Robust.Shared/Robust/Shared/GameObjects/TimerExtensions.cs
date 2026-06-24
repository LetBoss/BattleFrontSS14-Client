// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.TimerExtensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using System;
using System.Threading;

#nullable enable
namespace Robust.Shared.GameObjects;

[Obsolete("Use a system update loop instead")]
public static class TimerExtensions
{
  private static TimerComponent EnsureTimerComponent(this EntityUid entity)
  {
    return IoCManager.Resolve<IEntityManager>().EnsureComponent<TimerComponent>(entity);
  }

  [Obsolete("Use a system update loop instead")]
  public static void SpawnTimer(
    this EntityUid entity,
    int milliseconds,
    Action onFired,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    entity.EnsureTimerComponent().Spawn(milliseconds, onFired, cancellationToken);
  }

  [Obsolete("Use a system update loop instead")]
  public static void SpawnTimer(
    this EntityUid entity,
    TimeSpan duration,
    Action onFired,
    CancellationToken cancellationToken = default (CancellationToken))
  {
    entity.EnsureTimerComponent().Spawn((int) duration.TotalMilliseconds, onFired, cancellationToken);
  }

  [Obsolete("Use a system update loop instead")]
  public static void SpawnRepeatingTimer(
    this EntityUid entity,
    TimeSpan duration,
    Action onFired,
    CancellationToken cancellationToken)
  {
    entity.EnsureTimerComponent().SpawnRepeating(duration, onFired, cancellationToken);
  }
}
