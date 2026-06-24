using System;
using Robust.Shared.Serialization;

namespace Content.Shared.SensorMonitoring;

[Serializable]
[NetSerializable]
public record struct SensorSample(TimeSpan Time, float Value);
