using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.UserInterface;

public sealed class IntrinsicUIOpenAttemptEvent : CancellableEntityEventArgs
{
	public EntityUid User { get; }

	public Enum? Key { get; }

	public IntrinsicUIOpenAttemptEvent(EntityUid who, Enum? key)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		User = who;
		Key = key;
	}
}
