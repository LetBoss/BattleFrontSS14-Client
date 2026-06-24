using Robust.Shared.GameObjects;

namespace Content.Shared.Revenant;

public sealed class HarvestDoAfterComplete : EntityEventArgs
{
	public readonly EntityUid Target;

	public HarvestDoAfterComplete(EntityUid target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
	}
}
