namespace Robust.Shared.GameObjects;

public sealed class PointLightToggleEvent : EntityEventArgs
{
	public bool Enabled;

	public PointLightToggleEvent(bool enabled)
	{
		Enabled = enabled;
	}
}
