using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Reaction;

[Prototype(null, 1)]
public sealed class ReactionPrototype : IPrototype, IComparable<ReactionPrototype>
{
	[DataField("reactants", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<ReactantPrototype, ReagentPrototype>))]
	public Dictionary<string, ReactantPrototype> Reactants = new Dictionary<string, ReactantPrototype>();

	[DataField("minTemp", false, 1, false, false, null)]
	public float MinimumTemperature;

	[DataField("conserveEnergy", false, 1, false, false, null)]
	public bool ConserveEnergy = true;

	[DataField("maxTemp", false, 1, false, false, null)]
	public float MaximumTemperature = float.PositiveInfinity;

	[DataField("requiredMixerCategories", false, 1, false, false, null)]
	public List<ProtoId<MixingCategoryPrototype>>? MixingCategories;

	[DataField("products", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, ReagentPrototype>))]
	public Dictionary<string, FixedPoint2> Products = new Dictionary<string, FixedPoint2>();

	[DataField("effects", false, 1, false, false, null)]
	public List<EntityEffect> Effects = new List<EntityEffect>();

	[DataField("impact", false, 1, false, true, null)]
	public LogImpact Impact = LogImpact.Low;

	[DataField("quantized", false, 1, false, false, null)]
	public bool Quantized;

	[DataField("priority", false, 1, false, false, null)]
	public int Priority;

	[DataField(null, false, 1, false, false, null)]
	public bool Source;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	public string Name { get; private set; } = string.Empty;

	[DataField("sound", false, 1, false, true, null)]
	public SoundSpecifier Sound { get; private set; } = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/Chemistry/bubbles.ogg", (AudioParams?)null);

	public int CompareTo(ReactionPrototype? other)
	{
		if (other == null)
		{
			return -1;
		}
		if (Priority != other.Priority)
		{
			return other.Priority - Priority;
		}
		if (Products.Count != other.Products.Count)
		{
			return Products.Count - other.Products.Count;
		}
		return string.Compare(ID, other.ID, StringComparison.Ordinal);
	}
}
