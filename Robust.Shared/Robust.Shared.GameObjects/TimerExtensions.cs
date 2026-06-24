using System;
using System.Threading;
using Robust.Shared.IoC;

namespace Robust.Shared.GameObjects;

[Obsolete("Use a system update loop instead")]
public static class TimerExtensions
{
	private static TimerComponent EnsureTimerComponent(this EntityUid entity)
	{
		return IoCManager.Resolve<IEntityManager>().EnsureComponent<TimerComponent>(entity);
	}

	[Obsolete("Use a system update loop instead")]
	public static void SpawnTimer(this EntityUid entity, int milliseconds, Action onFired, CancellationToken cancellationToken = default(CancellationToken))
	{
		entity.EnsureTimerComponent().Spawn(milliseconds, onFired, cancellationToken);
	}

	[Obsolete("Use a system update loop instead")]
	public static void SpawnTimer(this EntityUid entity, TimeSpan duration, Action onFired, CancellationToken cancellationToken = default(CancellationToken))
	{
		entity.EnsureTimerComponent().Spawn((int)duration.TotalMilliseconds, onFired, cancellationToken);
	}

	[Obsolete("Use a system update loop instead")]
	public static void SpawnRepeatingTimer(this EntityUid entity, TimeSpan duration, Action onFired, CancellationToken cancellationToken)
	{
		entity.EnsureTimerComponent().SpawnRepeating(duration, onFired, cancellationToken);
	}
}
