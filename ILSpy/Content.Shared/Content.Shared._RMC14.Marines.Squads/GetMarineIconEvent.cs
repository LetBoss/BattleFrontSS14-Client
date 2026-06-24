using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Marines.Squads;

[ByRefEvent]
public record struct GetMarineIconEvent(SpriteSpecifier? Icon, SpriteSpecifier? Background, Color? BackgroundColor);
