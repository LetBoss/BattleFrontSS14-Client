using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.HUD;

[Prototype(null, 1)]
public sealed class HudThemePrototype : IPrototype, IComparable<HudThemePrototype>
{
	[DataField(null, false, 1, false, false, null)]
	public int Order;

	[DataField("name", false, 1, true, false, null)]
	public string Name { get; private set; } = string.Empty;

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;

	[DataField("path", false, 1, true, false, null)]
	public string Path { get; private set; } = string.Empty;

	public int CompareTo(HudThemePrototype? other)
	{
		return Order.CompareTo(other?.Order);
	}
}
