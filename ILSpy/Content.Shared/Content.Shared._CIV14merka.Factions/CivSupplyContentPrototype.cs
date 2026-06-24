using System.Collections.Generic;
using Content.Shared._RMC14.Vendors;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Factions;

[Prototype(null, 1)]
public sealed class CivSupplyContentPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public List<CMVendorSection> Sections = new List<CMVendorSection>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, int> RefillPrices = new Dictionary<string, int>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
