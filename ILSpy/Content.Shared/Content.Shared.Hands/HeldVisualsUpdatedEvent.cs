using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands;

public sealed class HeldVisualsUpdatedEvent : EntityEventArgs
{
	public readonly EntityUid User;

	public HashSet<string> RevealedLayers;

	public HeldVisualsUpdatedEvent(EntityUid user, HashSet<string> revealedLayers)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		RevealedLayers = revealedLayers;
	}
}
