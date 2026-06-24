using Robust.Shared.Prototypes;

namespace Content.Shared.Cargo.Prototypes;

[Prototype(null, 1)]
public sealed class CargoMarketPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
