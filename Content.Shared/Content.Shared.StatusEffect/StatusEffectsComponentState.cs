using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StatusEffect;

[Serializable]
[NetSerializable]
public sealed class StatusEffectsComponentState : ComponentState
{
	public Dictionary<string, StatusEffectState> ActiveEffects;

	public List<string> AllowedEffects;

	public StatusEffectsComponentState(Dictionary<string, StatusEffectState> activeEffects, List<string> allowedEffects)
	{
		ActiveEffects = activeEffects;
		AllowedEffects = allowedEffects;
	}
}
