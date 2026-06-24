using Robust.Shared.GameObjects;

namespace Content.Shared.Respawn;

public sealed class SpecialRespawnSetupEvent : EntityEventArgs
{
	public EntityUid Entity;

	public SpecialRespawnSetupEvent(EntityUid entity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
	}
}
