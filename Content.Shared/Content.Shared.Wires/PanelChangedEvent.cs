using Robust.Shared.GameObjects;

namespace Content.Shared.Wires;

[ByRefEvent]
public readonly record struct PanelChangedEvent(bool Open);
