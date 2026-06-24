using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Toggleable;

[Serializable]
[NetSerializable]
public enum ToggleableVisuals : byte
{
	Enabled,
	Layer,
	Color
}
