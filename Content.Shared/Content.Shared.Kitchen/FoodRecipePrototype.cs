using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Kitchen;

[Prototype("microwaveMealRecipe", 1)]
public sealed class FoodRecipePrototype : IPrototype
{
	[DataField("name", false, 1, false, false, null)]
	private string _name = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public string Group = "Other";

	[DataField("reagents", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, ReagentPrototype>))]
	private Dictionary<string, FixedPoint2> _ingsReagents = new Dictionary<string, FixedPoint2>();

	[DataField("solids", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, EntityPrototype>))]
	private Dictionary<string, FixedPoint2> _ingsSolids = new Dictionary<string, FixedPoint2>();

	[DataField(null, false, 1, false, false, null)]
	public bool SecretRecipe;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("result", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string Result { get; private set; } = string.Empty;

	[DataField("time", false, 1, false, false, null)]
	public uint CookTime { get; private set; } = 5u;

	public string Name => Loc.GetString(_name);

	public IReadOnlyDictionary<string, FixedPoint2> IngredientsReagents => _ingsReagents;

	public IReadOnlyDictionary<string, FixedPoint2> IngredientsSolids => _ingsSolids;

	public FixedPoint2 IngredientCount()
	{
		FixedPoint2 n = 0;
		n += (FixedPoint2)_ingsReagents.Count;
		foreach (FixedPoint2 i in _ingsSolids.Values)
		{
			n += i;
		}
		return n;
	}
}
