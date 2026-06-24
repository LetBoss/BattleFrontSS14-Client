using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Components;

[Serializable]
[NetSerializable]
public enum GeigerVisuals : byte
{
	DangerLevel,
	IsEnabled
}
