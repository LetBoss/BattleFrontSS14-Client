using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Eye;

[ByRefEvent]
public readonly record struct QueenEyeActionUpdated(Entity<QueenEyeActionComponent> Queen);
