using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Procedural;

[Prototype(null, 1)]
public sealed class SalvageDifficultyPrototype : IPrototype
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("color", false, 1, false, false, null)]
	public Color Color = Color.White;

	[DataField("lootBudget", false, 1, true, false, null)]
	public float LootBudget;

	[DataField("mobBudget", false, 1, true, false, null)]
	public float MobBudget;

	[DataField("modifierBudget", false, 1, false, false, null)]
	public float ModifierBudget;

	[DataField("recommendedPlayers", false, 1, true, false, null)]
	public int RecommendedPlayers;

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
