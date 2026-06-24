using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Reagent;

[Serializable]
[NetSerializable]
public struct ReagentEffectsGuideEntry(FixedPoint2 metabolismRate, string[] effectDescriptions)
{
	public FixedPoint2 MetabolismRate = metabolismRate;

	public string[] EffectDescriptions = effectDescriptions;
}
