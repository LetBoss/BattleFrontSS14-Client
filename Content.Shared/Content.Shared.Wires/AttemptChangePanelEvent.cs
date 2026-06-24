using Robust.Shared.GameObjects;

namespace Content.Shared.Wires;

[ByRefEvent]
public record struct AttemptChangePanelEvent(bool Open, EntityUid? User, bool Cancelled = false);
