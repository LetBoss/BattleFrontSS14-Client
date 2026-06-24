using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

public sealed class UseInHandEvent : HandledEntityEventArgs
{
	public EntityUid User;

	public bool ApplyDelay = true;

	public UseInHandEvent(EntityUid user)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
	}
}
