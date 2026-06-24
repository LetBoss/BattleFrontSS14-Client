using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class FlavorProfileModificationEvent : EntityEventArgs
{
	public EntityUid User { get; }

	public HashSet<string> Flavors { get; }

	public FlavorProfileModificationEvent(EntityUid user, HashSet<string> flavors)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Flavors = flavors;
	}
}
