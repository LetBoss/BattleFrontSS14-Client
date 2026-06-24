using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Disposal.Components;

[Serializable]
[NetSerializable]
public enum DisposalTubeVisualState
{
	Free,
	Anchored
}
