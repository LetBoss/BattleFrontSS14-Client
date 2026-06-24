using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.DeviceNetwork;

[Prototype(null, 1)]
public sealed class DeviceFrequencyPrototype : IPrototype
{
	[DataField("frequency", false, 1, true, false, null)]
	public uint Frequency;

	[DataField("name", false, 1, false, false, null)]
	public string? Name;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
