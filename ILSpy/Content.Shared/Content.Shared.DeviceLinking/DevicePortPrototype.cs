using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.DeviceLinking;

public abstract class DevicePortPrototype
{
	[DataField("name", false, 1, true, false, null)]
	public string Name;

	[DataField("description", false, 1, true, false, null)]
	public string Description;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
