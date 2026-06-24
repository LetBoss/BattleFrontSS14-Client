using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry;

[ByRefEvent]
public readonly record struct ReactionEntityEvent(ReactionMethod Method, ReagentPrototype Reagent, ReagentQuantity ReagentQuantity, Solution? Source);
