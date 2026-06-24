using Robust.Shared.GameObjects;

namespace Content.Shared.Wires;

public sealed class WiresPanelSecurityEvent : EntityEventArgs
{
	public readonly string? Examine;

	public readonly bool WiresAccessible;

	public WiresPanelSecurityEvent(string? examine, bool wiresAccessible)
	{
		Examine = examine;
		WiresAccessible = wiresAccessible;
	}
}
