using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Devour;

[ByRefEvent]
public record struct XenoDevouredEvent(EntityUid Target, EntityUid User);
