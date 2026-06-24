using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction;

[ByRefEvent]
public record struct AccessibleOverrideEvent(EntityUid User, EntityUid Target)
{
	public readonly EntityUid User = User;

	public readonly EntityUid Target = Target;

	public bool Handled = false;

	public bool Accessible = false;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid User, out EntityUid Target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		User = this.User;
		Target = this.Target;
	}
}
