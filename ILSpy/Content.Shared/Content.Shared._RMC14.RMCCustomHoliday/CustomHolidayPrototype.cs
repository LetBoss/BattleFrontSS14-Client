using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.RMCCustomHoliday;

[Prototype(null, 1)]
public sealed class CustomHolidayPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string Name { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public int BeginDay { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string BeginMonth { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string Description { get; private set; }
}
