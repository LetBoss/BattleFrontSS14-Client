using System;
using System.Collections.Generic;
using Content.Shared.Roles;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable]
[NetSerializable]
public struct GhostRoleInfo
{
	public uint Identifier { get; set; }

	public string Name { get; set; }

	public string Description { get; set; }

	public string Rules { get; set; }

	public HashSet<JobRequirement>? Requirements { get; set; }

	public GhostRoleKind Kind { get; set; }

	public uint RafflePlayerCount { get; set; }

	public TimeSpan RaffleEndTime { get; set; }
}
