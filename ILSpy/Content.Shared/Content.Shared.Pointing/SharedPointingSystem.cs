using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Pointing;

public abstract class SharedPointingSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public sealed class SharedPointingArrowComponentState : ComponentState
	{
		public Vector2 StartPosition { get; init; }

		public TimeSpan EndTime { get; init; }
	}

	protected readonly TimeSpan PointDuration = TimeSpan.FromSeconds(4L);

	protected readonly float PointKeyTimeMove = 0.1f;

	protected readonly float PointKeyTimeHover = 0.5f;

	public bool CanPoint(EntityUid uid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		PointAttemptEvent ev = new PointAttemptEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<PointAttemptEvent>(uid, ev, true);
		return !((CancellableEntityEventArgs)ev).Cancelled;
	}
}
