using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Balance;

[Prototype(null, 1)]
public sealed class CivCommanderComebackBonusPrototype : IPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public int CheckIntervalSeconds = 240;

	[DataField(null, false, 1, false, false, null)]
	public List<CivCommanderComebackBonusTier> Tiers = new List<CivCommanderComebackBonusTier>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
