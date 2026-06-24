using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe;

[Serializable]
[NetSerializable]
public enum LatheVisuals : byte
{
	IsRunning,
	IsInserting,
	InsertingColor
}
