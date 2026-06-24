using System;
using System.Collections.Generic;

namespace Content.Client._PUBG.Sponsor;

public sealed class SponsorSandboxState
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

	public IReadOnlyCollection<string> DisallowedEntityIds { get; }

	public bool Enabled
	{
		get
		{
			if (!AllowSpawnEntities && !AllowSpawnTiles && !AllowSpawnDecals && !AllowEraseEntities)
			{
				return AllowEraseTiles;
			}
			return true;
		}
	}

	public SponsorSandboxState(bool allowSpawnEntities, bool allowSpawnTiles, bool allowSpawnDecals, bool allowEraseEntities, bool allowEraseTiles, bool allowSponsorArena, bool allowSponsorAghost, bool blockEraseMinds, bool isMiniGameSandbox, IReadOnlyCollection<string> disallowedEntityIds)
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

	public static SponsorSandboxState Disabled()
	{
		return new SponsorSandboxState(allowSpawnEntities: false, allowSpawnTiles: false, allowSpawnDecals: false, allowEraseEntities: false, allowEraseTiles: false, allowSponsorArena: false, allowSponsorAghost: false, blockEraseMinds: false, isMiniGameSandbox: false, Array.Empty<string>());
	}
}
