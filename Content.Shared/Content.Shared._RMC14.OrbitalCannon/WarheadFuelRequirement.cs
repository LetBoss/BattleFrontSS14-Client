using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.OrbitalCannon;

[Serializable]
[DataRecord]
[NetSerializable]
public readonly record struct WarheadFuelRequirement(EntProtoId<OrbitalCannonWarheadComponent> Warhead, int Fuel);
