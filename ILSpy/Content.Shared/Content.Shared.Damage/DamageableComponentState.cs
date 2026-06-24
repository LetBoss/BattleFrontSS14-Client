using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Damage;

[Serializable]
[NetSerializable]
public sealed class DamageableComponentState : ComponentState
{
	public readonly Dictionary<string, FixedPoint2> DamageDict;

	public readonly string? DamageContainerId;

	public readonly string? ModifierSetId;

	public readonly FixedPoint2? HealthBarThreshold;

	public DamageableComponentState(Dictionary<string, FixedPoint2> damageDict, string? damageContainerId, string? modifierSetId, FixedPoint2? healthBarThreshold)
	{
		DamageDict = damageDict;
		DamageContainerId = damageContainerId;
		ModifierSetId = modifierSetId;
		HealthBarThreshold = healthBarThreshold;
	}
}
