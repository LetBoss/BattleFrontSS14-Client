using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Visuals;

[Serializable]
[NetSerializable]
public enum PortableScrubberVisuals : byte
{
	IsFull,
	IsRunning,
	IsDraining
}
