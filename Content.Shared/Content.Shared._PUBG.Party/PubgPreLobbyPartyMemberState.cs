using System;
using System.Collections.Generic;
using Content.Shared.Preferences;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPreLobbyPartyMemberState
{
	public NetUserId UserId { get; }

	public string Ckey { get; }

	public int Level { get; }

	public int Xp { get; }

	public int MaxXp { get; }

	public bool IsReady { get; }

	public bool InPreLobby { get; }

	public HumanoidCharacterProfile? Profile { get; }

	public Dictionary<string, string> CurrentOutfit { get; }

	public PubgPreLobbyPartyMemberState(NetUserId userId, string ckey, int level, int xp, int maxXp, bool isReady, bool inPreLobby, HumanoidCharacterProfile? profile, Dictionary<string, string>? currentOutfit)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserId = userId;
		Ckey = ckey;
		Level = level;
		Xp = xp;
		MaxXp = maxXp;
		IsReady = isReady;
		InPreLobby = inPreLobby;
		Profile = profile;
		CurrentOutfit = ((currentOutfit != null) ? new Dictionary<string, string>(currentOutfit) : new Dictionary<string, string>());
	}
}
