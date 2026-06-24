using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Decals;

[Prototype("palette", 1)]
public sealed class ColorPalettePrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	public string Name { get; private set; }

	[DataField("colors", false, 1, false, false, null)]
	public Dictionary<string, Color> Colors { get; private set; }
}
