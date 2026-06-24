using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Nutrition;

[ByRefEvent]
public readonly record struct AfterFullyEatenEvent(EntityUid User)
{
	public readonly EntityUid User = User;

	[CompilerGenerated]
	public void Deconstruct(out EntityUid User)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		User = this.User;
	}
}
