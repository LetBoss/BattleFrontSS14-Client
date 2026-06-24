using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Sponsor;

[Serializable]
[NetSerializable]
public sealed class SponsorSandboxStateMessage : EntityEventArgs
{
	public bool AllowSpawnEntities { get; }

	public bool AllowSpawnTiles { get; }

	public bool AllowSpawnDecals { get; }

	public bool AllowEraseEntities { get; }

	public bool AllowEraseTiles { get; }

	public bool AllowSponsorArena { get; }

	public bool AllowSponsorAghost { get; }

	public bool BlockEraseMinds { get; }

	public bool IsMiniGameSandbox { get; }

	public List<string> DisallowedEntityIds { get; }

	public SponsorSandboxStateMessage(bool allowSpawnEntities, bool allowSpawnTiles, bool allowSpawnDecals, bool allowEraseEntities, bool allowEraseTiles, bool allowSponsorArena, bool allowSponsorAghost, bool blockEraseMinds, bool isMiniGameSandbox, List<string> disallowedEntityIds)
	{
		AllowSpawnEntities = allowSpawnEntities;
		AllowSpawnTiles = allowSpawnTiles;
		AllowSpawnDecals = allowSpawnDecals;
		AllowEraseEntities = allowEraseEntities;
		AllowEraseTiles = allowEraseTiles;
		AllowSponsorArena = allowSponsorArena;
		AllowSponsorAghost = allowSponsorAghost;
		BlockEraseMinds = blockEraseMinds;
		IsMiniGameSandbox = isMiniGameSandbox;
		DisallowedEntityIds = disallowedEntityIds;
	}
}
