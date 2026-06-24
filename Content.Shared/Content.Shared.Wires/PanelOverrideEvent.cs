using Robust.Shared.GameObjects;

namespace Content.Shared.Wires;

[ByRefEvent]
public record struct PanelOverrideEvent()
{
	public bool Allowed = true;
}
