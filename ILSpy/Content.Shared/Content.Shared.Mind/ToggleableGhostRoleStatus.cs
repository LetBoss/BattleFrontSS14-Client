using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Mind;

[Serializable]
[NetSerializable]
public enum ToggleableGhostRoleStatus : byte
{
	Off,
	Searching,
	On
}
