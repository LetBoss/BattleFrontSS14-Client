using System;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Weapons.Ranged.Stacks;

public sealed class GunStacksSystem : EntitySystem
{
	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private CMArmorSystem _rmcArmor;

	[Dependency]
	private CMGunSystem _rmcGun;

	[Dependency]
	private RMCSelectiveFireSystem _rmcSelectiveFire;

	[Dependency]
	private IGameTiming _timing;

	private EntityQuery<GunStacksComponent> _gunStacksQuery;

	private EntityQuery<RMCSelectiveFireComponent> _selectiveFireQuery;

	private EntityQuery<XenoComponent> _xenoQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_gunStacksQuery = ((EntitySystem)this).GetEntityQuery<GunStacksComponent>();
		_selectiveFireQuery = ((EntitySystem)this).GetEntityQuery<RMCSelectiveFireComponent>();
		_xenoQuery = ((EntitySystem)this).GetEntityQuery<XenoComponent>();
		((EntitySystem)this).SubscribeLocalEvent<GunStacksComponent, AmmoShotEvent>((EntityEventRefHandler<GunStacksComponent, AmmoShotEvent>)OnStacksAmmoShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunStacksActiveComponent, GetGunDamageModifierEvent>((EntityEventRefHandler<GunStacksActiveComponent, GetGunDamageModifierEvent>)OnStacksActiveGetGunDamageModifier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunStacksActiveComponent, GunGetFireRateEvent>((EntityEventRefHandler<GunStacksActiveComponent, GunGetFireRateEvent>)OnStacksActiveGetGunFireRate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunStacksActiveComponent, DroppedEvent>((EntityEventRefHandler<GunStacksActiveComponent, DroppedEvent>)OnStacksActiveDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunStacksProjectileComponent, ProjectileHitEvent>((EntityEventRefHandler<GunStacksProjectileComponent, ProjectileHitEvent>)OnStacksProjectileHit, (Type[])null, (Type[])null);
	}

	private void OnStacksAmmoShot(Entity<GunStacksComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		GunStacksActiveComponent active = default(GunStacksActiveComponent);
		foreach (EntityUid bullet in args.FiredProjectiles)
		{
			GunStacksProjectileComponent stacks = ((EntitySystem)this).EnsureComp<GunStacksProjectileComponent>(bullet);
			stacks.Gun = Entity<GunStacksComponent>.op_Implicit(ent);
			((EntitySystem)this).Dirty(bullet, (IComponent)(object)stacks, (MetaDataComponent)null);
			CMArmorPiercingComponent piercing = ((EntitySystem)this).EnsureComp<CMArmorPiercingComponent>(bullet);
			int shotsHit = 0;
			if (((EntitySystem)this).TryComp<GunStacksActiveComponent>(Entity<GunStacksComponent>.op_Implicit(ent), ref active))
			{
				shotsHit = active.Hits;
			}
			int ap = Math.Min(ent.Comp.MaxAP, ent.Comp.IncreaseAP * shotsHit);
			_rmcArmor.SetArmorPiercing(Entity<CMArmorPiercingComponent>.op_Implicit((bullet, piercing)), ap);
		}
	}

	private void OnStacksActiveGetGunDamageModifier(Entity<GunStacksActiveComponent> ent, ref GetGunDamageModifierEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		GunStacksComponent gunStacks = default(GunStacksComponent);
		if (((EntitySystem)this).TryComp<GunStacksComponent>(Entity<GunStacksActiveComponent>.op_Implicit(ent), ref gunStacks) && ent.Comp.Hits > 0)
		{
			args.Multiplier += gunStacks.DamageIncrease;
		}
	}

	private void OnStacksActiveGetGunFireRate(Entity<GunStacksActiveComponent> ent, ref GunGetFireRateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		GunStacksComponent gunStacks = default(GunStacksComponent);
		if (((EntitySystem)this).TryComp<GunStacksComponent>(Entity<GunStacksActiveComponent>.op_Implicit(ent), ref gunStacks) && ent.Comp.Hits > 0)
		{
			args.FireRate = gunStacks.SetFireRate;
		}
	}

	private void OnStacksActiveDropped(Entity<GunStacksActiveComponent> ent, ref DroppedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Reset(ent);
	}

	private void OnStacksProjectileHit(Entity<GunStacksProjectileComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent projectile = default(ProjectileComponent);
		if (!ent.Comp.Gun.HasValue || !_gunStacksQuery.HasComp(ent.Comp.Gun) || (((EntitySystem)this).TryComp<ProjectileComponent>(Entity<GunStacksProjectileComponent>.op_Implicit(ent), ref projectile) && projectile.ProjectileSpent))
		{
			return;
		}
		EntityUid target = args.Target;
		if (_xenoQuery.HasComp(target) && !_mobState.IsDead(target))
		{
			GunStacksActiveComponent gun = default(GunStacksActiveComponent);
			if (!((EntitySystem)this).TryComp<GunStacksActiveComponent>(ent.Comp.Gun, ref gun))
			{
				gun = ((EntitySystem)this).EnsureComp<GunStacksActiveComponent>(ent.Comp.Gun.Value);
			}
			gun.Hits++;
			gun.ExpireAt = _timing.CurTime + gun.StacksExpire;
			EntityUid? shooter = args.Shooter;
			if (shooter.HasValue)
			{
				EntityUid shooter2 = shooter.GetValueOrDefault();
				if (_net.IsServer)
				{
					string msg = ((gun.Hits == 1) ? base.Loc.GetString("rmc-gun-stacks-hit-single") : base.Loc.GetString("rmc-gun-stacks-hit-multiple", (ValueTuple<string, object>)("hits", gun.Hits)));
					_popup.PopupEntity(msg, shooter2, shooter2);
				}
			}
		}
		RefreshGunStats(ent.Comp.Gun.Value);
		((EntitySystem)this).Dirty<GunStacksProjectileComponent>(ent, (MetaDataComponent)null);
	}

	private void Reset(Entity<GunStacksActiveComponent> gun)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemComp<GunStacksActiveComponent>(gun.Owner);
		if (_net.IsServer)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-gun-stacks-reset", (ValueTuple<string, object>)("weapon", gun.Owner)), Entity<GunStacksActiveComponent>.op_Implicit(gun), PopupType.SmallCaution);
		}
		RefreshGunStats(gun.Owner);
	}

	private void RefreshGunStats(EntityUid gun)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_rmcGun.RefreshGunDamageMultiplier(Entity<GunDamageModifierComponent>.op_Implicit(gun));
		RMCSelectiveFireComponent selective = default(RMCSelectiveFireComponent);
		if (_selectiveFireQuery.TryComp(gun, ref selective))
		{
			_rmcSelectiveFire.RefreshFireModeGunValues(Entity<RMCSelectiveFireComponent>.op_Implicit((gun, selective)));
			((EntitySystem)this).Dirty(gun, (IComponent)(object)selective, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<GunStacksActiveComponent> gunStackQuery = ((EntitySystem)this).EntityQueryEnumerator<GunStacksActiveComponent>();
		EntityUid uid = default(EntityUid);
		GunStacksActiveComponent active = default(GunStacksActiveComponent);
		while (gunStackQuery.MoveNext(ref uid, ref active))
		{
			if (!(active.ExpireAt > time))
			{
				Reset(Entity<GunStacksActiveComponent>.op_Implicit((uid, active)));
			}
		}
	}
}
