using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Construction.Upgrades;

public sealed class RMCConstructionUpgradedEvent : EntityEventArgs
{
	public readonly EntityUid New;

	public readonly EntityUid Old;

	public RMCConstructionUpgradedEvent(EntityUid newUid, EntityUid oldUid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		New = newUid;
		Old = oldUid;
	}
}
