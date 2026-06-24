using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Xenoarchaeology.Equipment.Components;

[Serializable]
[NetSerializable]
public enum ArtifactState
{
	None,
	Ready,
	Unlocking,
	Cooldown
}
