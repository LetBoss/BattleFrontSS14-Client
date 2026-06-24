using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.NPC.Prototypes;

[Serializable]
[NetSerializable]
public record struct FactionData
{
	[ViewVariables]
	public HashSet<ProtoId<NpcFactionPrototype>> Friendly;

	[ViewVariables]
	public HashSet<ProtoId<NpcFactionPrototype>> Hostile;
}
