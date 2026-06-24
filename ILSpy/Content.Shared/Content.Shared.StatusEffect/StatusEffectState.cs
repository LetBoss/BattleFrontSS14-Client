using System;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StatusEffect;

[Serializable]
[NetSerializable]
public sealed class StatusEffectState
{
	[ViewVariables]
	public (TimeSpan, TimeSpan) Cooldown;

	[ViewVariables]
	public bool CooldownRefresh = true;

	[ViewVariables]
	public string? RelevantComponent;

	public StatusEffectState((TimeSpan, TimeSpan) cooldown, bool refresh, string? relevantComponent = null)
	{
		Cooldown = cooldown;
		CooldownRefresh = refresh;
		RelevantComponent = relevantComponent;
	}

	public StatusEffectState(StatusEffectState toCopy)
	{
		Cooldown = (toCopy.Cooldown.Item1, toCopy.Cooldown.Item2);
		CooldownRefresh = toCopy.CooldownRefresh;
		RelevantComponent = toCopy.RelevantComponent;
	}
}
