using System;
using Robust.Shared.Serialization;

namespace Content.Shared.CriminalRecords;

[Serializable]
[NetSerializable]
public record struct CrimeHistory(TimeSpan AddTime, string Crime, string? InitiatorName);
