using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.GridPreloader.Prototypes;

[Prototype(null, 1)]
public sealed class PreloadedGridPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public ResPath Path;

	[DataField(null, false, 1, false, false, null)]
	public int Copies = 1;

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
