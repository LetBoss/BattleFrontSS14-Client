using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;

namespace Content.Shared.Lathe;

[ByRefEvent]
public readonly record struct LatheStartPrintingEvent(LatheRecipePrototype Recipe);
