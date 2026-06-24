using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ReagentGuideChangeset
{
	public Dictionary<string, ReagentGuideEntry> GuideEntries;

	public HashSet<string> Removed;

	public ReagentGuideChangeset(Dictionary<string, ReagentGuideEntry> guideEntries, HashSet<string> removed)
	{
		GuideEntries = guideEntries;
		Removed = removed;
	}
}
