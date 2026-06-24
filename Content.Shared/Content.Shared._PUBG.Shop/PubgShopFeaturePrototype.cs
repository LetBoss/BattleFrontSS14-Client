using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._PUBG.Shop;

[Prototype(null, 1)]
public sealed class PubgShopFeaturePrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public string Kind = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string TitleLocKey = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string DescriptionLocKey = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? Icon;

	[DataField(null, false, 1, false, false, null)]
	public string? IconEntity;

	[DataField(null, false, 1, false, false, null)]
	public int Price;

	[DataField(null, false, 1, false, false, null)]
	public string Currency = "coins";

	[DataField(null, false, 1, false, false, null)]
	public bool Enabled = true;

	[DataField(null, false, 1, false, false, null)]
	public int Order;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
