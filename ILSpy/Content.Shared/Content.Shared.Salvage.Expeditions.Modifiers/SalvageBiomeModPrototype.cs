using Content.Shared.Parallax.Biomes;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Salvage.Expeditions.Modifiers;

[Prototype(null, 1)]
public sealed class SalvageBiomeModPrototype : IPrototype, ISalvageMod
{
	[DataField("weather", false, 1, false, false, null)]
	public bool Weather = true;

	[DataField("biome", false, 1, true, false, typeof(PrototypeIdSerializer<BiomeTemplatePrototype>))]
	public string? BiomePrototype;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("desc", false, 1, false, false, null)]
	public LocId Description { get; private set; } = LocId.op_Implicit(string.Empty);

	[DataField("cost", false, 1, false, false, null)]
	public float Cost { get; private set; }
}
