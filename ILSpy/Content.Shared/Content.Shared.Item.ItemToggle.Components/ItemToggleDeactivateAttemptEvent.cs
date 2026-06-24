using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Item.ItemToggle.Components;

[ByRefEvent]
public record struct ItemToggleDeactivateAttemptEvent(EntityUid? User)
{
	public bool Silent = false;

	public bool Cancelled = false;

	public readonly EntityUid? User = User;

	public string? Popup = null;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid? User)
	{
		User = this.User;
	}
}
