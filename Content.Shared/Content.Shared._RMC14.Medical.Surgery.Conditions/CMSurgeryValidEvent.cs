using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Medical.Surgery.Conditions;

[ByRefEvent]
public record struct CMSurgeryValidEvent(EntityUid Body, EntityUid Part, bool Cancelled = false);
