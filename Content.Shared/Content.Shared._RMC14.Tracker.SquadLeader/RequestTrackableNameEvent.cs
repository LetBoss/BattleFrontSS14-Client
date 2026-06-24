using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Tracker.SquadLeader;

[ByRefEvent]
public record struct RequestTrackableNameEvent(string? Name = null, bool Handled = false);
