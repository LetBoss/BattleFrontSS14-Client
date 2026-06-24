using Content.Shared.Nutrition.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared.Nutrition;

public record struct FoodSequenceIngredientAddedEvent(EntityUid Start, EntityUid Element, ProtoId<FoodSequenceElementPrototype> Proto, EntityUid? User = null);
