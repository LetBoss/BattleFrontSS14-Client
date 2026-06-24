using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Prototypes;

[Prototype(null, 1)]
public sealed class GasPrototype : IPrototype
{
	[DataField("gasVisbilityFactor", false, 1, false, false, null)]
	public float GasVisibilityFactor = 20f;

	[DataField("name", false, 1, false, false, null)]
	public string Name { get; set; } = "";

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("specificHeat", false, 1, false, false, null)]
	public float SpecificHeat { get; private set; }

	[DataField("heatCapacityRatio", false, 1, false, false, null)]
	public float HeatCapacityRatio { get; private set; } = 1.4f;

	[DataField("molarMass", false, 1, false, false, null)]
	public float MolarMass { get; set; } = 1f;

	[DataField("gasMolesVisible", false, 1, false, false, null)]
	public float GasMolesVisible { get; private set; } = 0.25f;

	public float GasMolesVisibleMax => GasMolesVisible * GasVisibilityFactor;

	[DataField("gasOverlayTexture", false, 1, false, false, null)]
	public string GasOverlayTexture { get; private set; } = string.Empty;

	[DataField("gasOverlayState", false, 1, false, false, null)]
	public string GasOverlayState { get; set; } = string.Empty;

	[DataField("gasOverlaySprite", false, 1, false, false, null)]
	public string GasOverlaySprite { get; set; } = string.Empty;

	[DataField("overlayPath", false, 1, false, false, null)]
	public string OverlayPath { get; private set; } = string.Empty;

	[DataField("reagent", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
	public string? Reagent { get; private set; }

	[DataField("color", false, 1, false, false, null)]
	public string Color { get; private set; } = string.Empty;

	[DataField("pricePerMole", false, 1, false, false, null)]
	public float PricePerMole { get; set; }
}
