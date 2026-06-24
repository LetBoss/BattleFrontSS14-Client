using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

public sealed class SuicideByEnvironmentEvent : HandledEntityEventArgs
{
	public EntityUid Victim { get; set; }

	public SuicideByEnvironmentEvent(EntityUid victim)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Victim = victim;
	}
}
