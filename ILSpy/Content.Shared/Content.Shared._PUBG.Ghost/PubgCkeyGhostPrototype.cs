using System;
using System.Collections.Generic;
using Content.Shared.Ghost;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Ghost;

[Prototype(null, 1)]
public sealed class PubgCkeyGhostPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("ckeys", false, 1, true, false, null)]
	public List<string> Ckeys { get; private set; } = new List<string>();

	[DataField("proto", false, 1, true, false, null)]
	public EntProtoId<GhostComponent> GhostEntityPrototype { get; private set; }

	[DataField("adminProto", false, 1, false, false, null)]
	public EntProtoId<GhostComponent>? AdminGhostEntityPrototype { get; private set; }

	public bool MatchesCkey(string ckey)
	{
		foreach (string ckey2 in Ckeys)
		{
			if (ckey2.Equals(ckey, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}
}
