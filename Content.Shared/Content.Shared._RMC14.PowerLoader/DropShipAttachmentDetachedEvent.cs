using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.PowerLoader;

[ByRefEvent]
public record struct DropShipAttachmentDetachedEvent(EntityUid Detached);
