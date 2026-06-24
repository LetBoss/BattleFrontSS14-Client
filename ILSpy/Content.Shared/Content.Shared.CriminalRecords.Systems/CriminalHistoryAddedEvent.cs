using Robust.Shared.GameObjects;

namespace Content.Shared.CriminalRecords.Systems;

[ByRefEvent]
public record struct CriminalHistoryAddedEvent(CrimeHistory History);
