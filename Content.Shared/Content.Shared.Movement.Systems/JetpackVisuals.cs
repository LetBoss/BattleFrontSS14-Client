using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Systems;

[Serializable]
[NetSerializable]
public enum JetpackVisuals : byte
{
	Enabled,
	Layer
}
