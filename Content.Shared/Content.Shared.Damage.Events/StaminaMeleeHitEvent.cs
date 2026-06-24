using System.Collections.Generic;
using Content.Shared.Damage.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage.Events;

public sealed class StaminaMeleeHitEvent : HandledEntityEventArgs
{
	public List<(EntityUid Entity, StaminaComponent Component)> HitList;

	public float Multiplier = 1f;

	public float FlatModifier;

	public StaminaMeleeHitEvent(List<(EntityUid Entity, StaminaComponent Component)> hitList)
	{
		HitList = hitList;
	}
}
