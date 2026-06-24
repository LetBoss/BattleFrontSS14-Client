using Robust.Shared.GameObjects;

namespace Content.Shared.EntityEffects;

public record EntityEffectBaseArgs
{
	public EntityUid TargetEntity;

	public IEntityManager EntityManager;

	public EntityEffectBaseArgs(EntityUid targetEntity, IEntityManager entityManager)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TargetEntity = targetEntity;
		EntityManager = entityManager;
	}
}
