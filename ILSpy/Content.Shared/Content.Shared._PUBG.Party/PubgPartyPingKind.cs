using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public enum PubgPartyPingKind : byte
{
	Normal,
	Enemy,
	Item
}
