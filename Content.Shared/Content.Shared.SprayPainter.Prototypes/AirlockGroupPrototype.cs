using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.SprayPainter.Prototypes;

[Prototype("AirlockGroup", 1)]
public sealed class AirlockGroupPrototype : IPrototype
{
	[DataField("stylePaths", false, 1, false, false, null)]
	public Dictionary<string, string> StylePaths;

	[DataField("iconPriority", false, 1, false, false, null)]
	public int IconPriority;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
