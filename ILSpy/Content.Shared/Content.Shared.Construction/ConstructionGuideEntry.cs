using System;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Construction;

[Serializable]
[NetSerializable]
public sealed class ConstructionGuideEntry
{
	public int? EntryNumber { get; set; }

	public int Padding { get; set; }

	public string Localization { get; set; } = string.Empty;

	public (string, object)[]? Arguments { get; set; }

	public SpriteSpecifier? Icon { get; set; }
}
