using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.PurchaseRequest;

[Prototype(null, 1)]
public sealed class CivPurchaseCatalogPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public Dictionary<string, List<CatalogItem>> Catalog = new Dictionary<string, List<CatalogItem>>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
