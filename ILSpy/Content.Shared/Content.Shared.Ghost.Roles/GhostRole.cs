using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable]
[NetSerializable]
public sealed class GhostRole
{
	public NetEntity Id;

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;
}
