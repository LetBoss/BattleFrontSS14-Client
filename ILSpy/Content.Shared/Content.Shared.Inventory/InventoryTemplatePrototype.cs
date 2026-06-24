using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Inventory;

[Prototype(null, 1)]
public sealed class InventoryTemplatePrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;

	[DataField("slots", false, 1, false, false, null)]
	public SlotDefinition[] Slots { get; private set; } = Array.Empty<SlotDefinition>();
}
