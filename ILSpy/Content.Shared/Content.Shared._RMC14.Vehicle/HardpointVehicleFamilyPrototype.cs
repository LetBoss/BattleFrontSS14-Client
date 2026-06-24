using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Vehicle;

[Prototype(null, 1)]
public sealed class HardpointVehicleFamilyPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
