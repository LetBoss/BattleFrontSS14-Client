using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct OverwatchSavedLocation(int Longitude, int Latitude, string Comment);
