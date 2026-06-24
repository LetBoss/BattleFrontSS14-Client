using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.RoundEnd;

[Serializable]
[NetSerializable]
public sealed class PubgRoundEndPartyEntry
{
	public int PartyId { get; }

	public string Username { get; }

	public int Placement { get; }

	public int Kills { get; }

	public int DamageDealt { get; }

	public int DamageTaken { get; }

	public PubgRoundEndPartyEntry(int partyId, string username, int placement, int kills, int damageDealt, int damageTaken)
	{
		PartyId = partyId;
		Username = username;
		Placement = placement;
		Kills = kills;
		DamageDealt = damageDealt;
		DamageTaken = damageTaken;
	}
}
