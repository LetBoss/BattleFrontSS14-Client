using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Energy;

[ByRefEvent]
public record struct XenoEnergyChangedEvent(FixedPoint2 NewEnergy);
