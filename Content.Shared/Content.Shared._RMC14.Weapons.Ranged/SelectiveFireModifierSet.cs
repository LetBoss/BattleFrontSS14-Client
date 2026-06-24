using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Weapons.Ranged;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct SelectiveFireModifierSet(float FireDelay, double MaxScatterModifier, bool UseBurstScatterMult, double UnwieldedScatterMultiplier, int? ShotsToMaxScatter);
