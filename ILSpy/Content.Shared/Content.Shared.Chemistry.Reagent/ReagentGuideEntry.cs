using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Body.Prototypes;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Reagent;

[Serializable]
[NetSerializable]
public struct ReagentGuideEntry
{
	public string ReagentPrototype;

	public Dictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsGuideEntry>? GuideEntries;

	public List<string>? PlantMetabolisms;

	public ReagentGuideEntry(ReagentPrototype proto, IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		PlantMetabolisms = null;
		ReagentPrototype = proto.ID;
		GuideEntries = proto.Metabolisms?.Select((KeyValuePair<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry> x) => (Key: x.Key, x.Value.MakeGuideEntry(prototype, entSys))).ToDictionary(((ProtoId<MetabolismGroupPrototype> Key, ReagentEffectsGuideEntry) x) => x.Key, ((ProtoId<MetabolismGroupPrototype> Key, ReagentEffectsGuideEntry) x) => x.Item2);
		if (proto.PlantMetabolisms.Count > 0)
		{
			PlantMetabolisms = new List<string>((from x in proto.PlantMetabolisms
				select x.GuidebookEffectDescription(prototype, entSys) into x
				where x != null
				select (x)).ToArray());
		}
	}
}
