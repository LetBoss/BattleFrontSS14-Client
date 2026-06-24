using Robust.Shared.GameObjects;

namespace Content.Shared._PUBG.Medicine;

public sealed class PubgMedicalHealAppliedEvent : EntityEventArgs
{
	public EntityUid User { get; }

	public EntityUid Target { get; }

	public float HealedAmount { get; }

	public PubgMedicalHealAppliedEvent(EntityUid user, EntityUid target, float healedAmount = 0f)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Target = target;
		HealedAmount = healedAmount;
	}
}
