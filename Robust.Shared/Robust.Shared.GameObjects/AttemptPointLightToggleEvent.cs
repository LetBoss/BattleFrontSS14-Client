namespace Robust.Shared.GameObjects;

[ByRefEvent]
public record struct AttemptPointLightToggleEvent(bool Enabled)
{
	public bool Cancelled = false;
}
