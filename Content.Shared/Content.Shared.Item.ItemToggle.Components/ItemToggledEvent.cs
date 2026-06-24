using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Item.ItemToggle.Components;

[ByRefEvent]
public readonly record struct ItemToggledEvent(bool Predicted, bool Activated, EntityUid? User)
{
	public readonly bool Predicted = Predicted;

	public readonly bool Activated = Activated;

	public readonly EntityUid? User = User;

	[CompilerGenerated]
	public void Deconstruct(out bool Predicted, out bool Activated, out EntityUid? User)
	{
		Predicted = this.Predicted;
		Activated = this.Activated;
		User = this.User;
	}
}
