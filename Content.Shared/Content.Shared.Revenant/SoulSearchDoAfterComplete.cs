using Robust.Shared.GameObjects;

namespace Content.Shared.Revenant;

public sealed class SoulSearchDoAfterComplete : EntityEventArgs
{
	public readonly EntityUid Target;

	public SoulSearchDoAfterComplete(EntityUid target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
	}
}
