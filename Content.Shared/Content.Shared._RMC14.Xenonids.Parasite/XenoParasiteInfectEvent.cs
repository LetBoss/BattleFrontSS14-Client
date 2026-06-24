using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Parasite;

public record struct XenoParasiteInfectEvent(EntityUid Target, EntityUid Parasite);
