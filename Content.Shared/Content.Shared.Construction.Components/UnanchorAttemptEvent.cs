using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components;

public sealed class UnanchorAttemptEvent : BaseAnchoredAttemptEvent
{
	public UnanchorAttemptEvent(EntityUid user, EntityUid tool)
		: base(user, tool)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
