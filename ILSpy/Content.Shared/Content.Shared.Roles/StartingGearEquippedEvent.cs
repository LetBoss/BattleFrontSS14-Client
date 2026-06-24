using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Roles;

[ByRefEvent]
public record struct StartingGearEquippedEvent(EntityUid Entity)
{
	public readonly EntityUid Entity = Entity;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid Entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Entity = this.Entity;
	}
}
