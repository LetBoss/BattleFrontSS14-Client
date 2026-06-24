using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.Borgs.Components;

[Serializable]
[NetSerializable]
public enum BorgVisualLayers : byte
{
	Body,
	Light,
	LightStatus
}
