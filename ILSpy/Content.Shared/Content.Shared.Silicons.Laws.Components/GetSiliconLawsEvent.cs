using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Silicons.Laws.Components;

[ByRefEvent]
public record struct GetSiliconLawsEvent(EntityUid Entity)
{
	public EntityUid Entity = Entity;

	public SiliconLawset Laws = new SiliconLawset();

	public bool Handled = false;

	[CompilerGenerated]
	public readonly void Deconstruct(out EntityUid Entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Entity = this.Entity;
	}
}
