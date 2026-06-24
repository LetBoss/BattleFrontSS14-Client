using Content.Shared._RMC14.Stun;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Shared._RMC14.Explosion.Implosion;

public sealed class RMCImplosionSystem : EntitySystem
{
	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	public void Implode(RMCImplosion implosion, MapCoordinates origin)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid entity in _entityLookup.GetEntitiesInRange(origin, implosion.PullRange, (LookupFlags)78))
		{
			_sizeStun.KnockBack(entity, origin, 0f - implosion.PullDistance, 0f - implosion.PullDistance, implosion.PullSpeed, implosion.IgnoreSize);
		}
	}
}
