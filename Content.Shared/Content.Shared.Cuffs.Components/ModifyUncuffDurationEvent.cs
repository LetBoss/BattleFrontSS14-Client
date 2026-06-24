using Robust.Shared.GameObjects;

namespace Content.Shared.Cuffs.Components;

[ByRefEvent]
public record struct ModifyUncuffDurationEvent(EntityUid User, EntityUid Target, float Duration);
