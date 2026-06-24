using Robust.Shared.GameObjects;

namespace Content.Shared.Silicons.Borgs.Components;

[ByRefEvent]
public readonly record struct BorgModuleSelectedEvent(EntityUid Chassis);
