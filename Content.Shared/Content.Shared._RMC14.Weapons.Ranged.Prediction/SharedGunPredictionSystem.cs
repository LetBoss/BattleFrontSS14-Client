using System;
using System.Collections.Generic;
using Content.Shared._RMC14.CCVar;
using Content.Shared.CombatMode;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Weapons.Ranged.Prediction;

public abstract class SharedGunPredictionSystem : EntitySystem
{
	[Dependency]
	private SharedCombatModeSystem _combatMode;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private CMGunSystem _akimbo;

	public bool GunPrediction { get; private set; }

	public override void Initialize()
	{
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, RMCCVars.RMCGunPrediction, (Action<bool>)delegate(bool v)
		{
			GunPrediction = v;
		}, true);
	}

	public List<EntityUid>? ShootRequested(NetEntity netGun, NetCoordinates coordinates, NetEntity? target, List<int>? projectiles, ICommonSession session)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? user = session.AttachedEntity;
		if (!user.HasValue || !_combatMode.IsInCombatMode(user) || !_gun.TryGetGun(user.Value, out EntityUid activeEnt, out GunComponent activeGun))
		{
			return null;
		}
		EntityUid requested = ((EntitySystem)this).GetEntity(netGun);
		EntityUid fireEnt;
		GunComponent fireGun;
		if (activeEnt == requested)
		{
			fireEnt = activeEnt;
			fireGun = activeGun;
		}
		else
		{
			GunComponent offGun = default(GunComponent);
			if (!_akimbo.TryGetAkimboOffHand(user.Value, Entity<GunComponent>.op_Implicit((activeEnt, activeGun)), out var offHand) || !(offHand == requested) || !((EntitySystem)this).TryComp<GunComponent>(requested, ref offGun))
			{
				return null;
			}
			fireEnt = requested;
			fireGun = offGun;
		}
		EntityCoordinates shotCoordinates = ((EntitySystem)this).GetCoordinates(coordinates);
		if (!((EntityCoordinates)(ref shotCoordinates)).IsValid((IEntityManager)(object)base.EntityManager))
		{
			return null;
		}
		fireGun.ShootCoordinates = shotCoordinates;
		fireGun.Target = ((EntitySystem)this).GetEntity(target);
		return _gun.AttemptShoot(user.Value, fireEnt, fireGun, projectiles, session);
	}
}
