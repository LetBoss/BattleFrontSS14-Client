using Content.Shared.Clothing.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Clothing;

[ByRefEvent]
public readonly record struct ClothingDidEquippedEvent(Entity<ClothingComponent> Clothing);
