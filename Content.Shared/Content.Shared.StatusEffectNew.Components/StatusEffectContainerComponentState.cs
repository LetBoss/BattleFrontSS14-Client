using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.StatusEffectNew.Components;

[Serializable]
[NetSerializable]
public sealed class StatusEffectContainerComponentState(HashSet<NetEntity> activeStatusEffects) : ComponentState
{
	public readonly HashSet<NetEntity> ActiveStatusEffects = activeStatusEffects;
}
