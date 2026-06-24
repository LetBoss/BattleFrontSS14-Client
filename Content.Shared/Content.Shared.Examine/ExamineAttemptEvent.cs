using Robust.Shared.GameObjects;

namespace Content.Shared.Examine;

public sealed class ExamineAttemptEvent : CancellableEntityEventArgs
{
	public readonly EntityUid Examiner;

	public ExamineAttemptEvent(EntityUid examiner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Examiner = examiner;
	}
}
