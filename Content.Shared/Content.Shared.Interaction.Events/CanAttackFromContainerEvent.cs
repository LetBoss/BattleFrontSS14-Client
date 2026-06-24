using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events;

public sealed class CanAttackFromContainerEvent : EntityEventArgs
{
	public EntityUid Uid;

	public EntityUid? Target;

	public bool CanAttack;

	public CanAttackFromContainerEvent(EntityUid uid, EntityUid? target = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		Target = target;
	}
}
