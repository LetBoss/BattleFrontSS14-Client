using Robust.Shared.GameObjects;

namespace Content.Shared.Prying.Components;

[ByRefEvent]
public record struct GetPryTimeModifierEvent
{
	public readonly EntityUid User;

	public float PryTimeModifier;

	public float BaseTime;

	public GetPryTimeModifierEvent(EntityUid user)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		PryTimeModifier = 1f;
		BaseTime = 5f;
		User = user;
	}
}
