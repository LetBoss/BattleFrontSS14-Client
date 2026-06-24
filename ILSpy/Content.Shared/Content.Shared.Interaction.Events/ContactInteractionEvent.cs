using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

public sealed class ContactInteractionEvent : HandledEntityEventArgs
{
	public EntityUid Other;

	public ContactInteractionEvent(EntityUid other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Other = other;
	}
}
