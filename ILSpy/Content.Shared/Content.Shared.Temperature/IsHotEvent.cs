using Robust.Shared.GameObjects;

namespace Content.Shared.Temperature;

public sealed class IsHotEvent : EntityEventArgs
{
	public bool IsHot { get; set; }
}
