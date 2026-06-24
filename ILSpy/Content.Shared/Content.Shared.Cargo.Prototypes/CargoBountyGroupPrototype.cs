using Robust.Shared.Prototypes;

namespace Content.Shared.Cargo.Prototypes;

[Prototype(null, 1)]
public sealed class CargoBountyGroupPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
