using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Flash;

[Serializable]
[NetSerializable]
public enum FlashVisuals : byte
{
	Burnt,
	Flashing
}
