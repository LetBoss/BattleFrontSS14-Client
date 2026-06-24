using System;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public readonly record struct PubgAdminPartyPlayerInfo(NetUserId UserId, string Ckey, string CharacterName, int PartyId);
