using Robust.Shared.GameObjects;

namespace Content.Client.Examine;

public sealed class ClientExaminedEvent : EntityEventArgs
{
	public readonly EntityUid Examiner;

	public readonly EntityUid Examined;

	public ClientExaminedEvent(EntityUid examined, EntityUid examiner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Examined = examined;
		Examiner = examiner;
	}
}
