using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Animations;

[Serializable]
[DataRecord]
[NetSerializable]
public readonly record struct RMCKeyFrame(string State, float KeyTime);
