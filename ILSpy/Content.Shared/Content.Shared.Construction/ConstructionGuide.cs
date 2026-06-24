using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Construction;

[Serializable]
[NetSerializable]
public sealed class ConstructionGuide
{
	public readonly ConstructionGuideEntry[] Entries;

	public ConstructionGuide(ConstructionGuideEntry[] entries)
	{
		Entries = entries;
	}
}
