using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Stealth.Components;

[Serializable]
[NetSerializable]
public sealed class StealthComponentState : ComponentState
{
	public readonly float Visibility;

	public readonly TimeSpan? LastUpdated;

	public readonly bool Enabled;

	public StealthComponentState(float stealthLevel, TimeSpan? lastUpdated, bool enabled)
	{
		Visibility = stealthLevel;
		LastUpdated = lastUpdated;
		Enabled = enabled;
	}
}
