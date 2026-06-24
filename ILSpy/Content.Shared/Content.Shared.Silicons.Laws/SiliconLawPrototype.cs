using Robust.Shared.Prototypes;

namespace Content.Shared.Silicons.Laws;

[Prototype(null, 1)]
public sealed class SiliconLawPrototype : SiliconLaw, IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
