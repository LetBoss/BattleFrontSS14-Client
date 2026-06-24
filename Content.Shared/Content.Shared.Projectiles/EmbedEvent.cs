using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Projectiles;

[ByRefEvent]
public readonly record struct EmbedEvent(EntityUid? Shooter, EntityUid Embedded)
{
	public readonly EntityUid? Shooter = Shooter;

	public readonly EntityUid Embedded = Embedded;

	[CompilerGenerated]
	public void Deconstruct(out EntityUid? Shooter, out EntityUid Embedded)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Shooter = this.Shooter;
		Embedded = this.Embedded;
	}
}
