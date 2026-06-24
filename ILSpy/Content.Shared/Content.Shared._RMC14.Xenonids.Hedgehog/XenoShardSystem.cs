using System;
using System.Numerics;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Hedgehog;

public sealed class XenoShardSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private CMArmorSystem _armor;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedAuraSystem _aura;

	[Dependency]
	private XenoProjectileSystem _xenoProjectile;

	[Dependency]
	private XenoShieldSystem _shield;

	[Dependency]
	private XenoEnergySystem _energy;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedRMCExplosionSystem _explosion;

	[Dependency]
	private INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoShardComponent, DamageChangedEvent>((EntityEventRefHandler<XenoShardComponent, DamageChangedEvent>)OnShardHitBy, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoShardComponent, CMGetArmorEvent>((EntityEventRefHandler<XenoShardComponent, CMGetArmorEvent>)OnShardGetArmor, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoShardComponent, MapInitEvent>((EntityEventRefHandler<XenoShardComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoShardComponent, XenoEnergyChangedEvent>((EntityEventRefHandler<XenoShardComponent, XenoEnergyChangedEvent>)OnShardLevelChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoSpikeShedComponent, ActionXenoSpikeShedEvent>((EntityEventRefHandler<XenoSpikeShedComponent, ActionXenoSpikeShedEvent>)OnSpikeShed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoSpikeShieldComponent, ActionXenoSpikeShieldEvent>((EntityEventRefHandler<XenoSpikeShieldComponent, ActionXenoSpikeShieldEvent>)OnSpikeShield, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoShardComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoShardComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovementSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoShardComponent, XenoEnergyGainAttemptEvent>((EntityEventRefHandler<XenoShardComponent, XenoEnergyGainAttemptEvent>)OnSpikeEnergyGain, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan currentTime = _timing.CurTime;
		EntityQueryEnumerator<XenoShardComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoShardComponent>();
		EntityUid uid = default(EntityUid);
		XenoShardComponent comp = default(XenoShardComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (comp.SpikeShedCooldownEnd.HasValue)
			{
				TimeSpan value = currentTime;
				TimeSpan? spikeShedCooldownEnd = comp.SpikeShedCooldownEnd;
				if (value >= spikeShedCooldownEnd)
				{
					comp.SpikeShedCooldownEnd = null;
					_popup.PopupEntity(base.Loc.GetString("rmc-shed-spikes-back"), uid, uid, PopupType.Medium);
					((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
				}
			}
		}
	}

	private void OnShardHitBy(Entity<XenoShardComponent> ent, ref DamageChangedEvent args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		XenoEnergyComponent energy = default(XenoEnergyComponent);
		if (args.Damageable.Damage != null && !(args.Damageable.Damage.GetTotal() <= FixedPoint2.Zero) && ((EntitySystem)this).HasComp<ProjectileComponent>(args.Tool) && ((EntitySystem)this).TryComp<XenoEnergyComponent>(Entity<XenoShardComponent>.op_Implicit(ent), ref energy))
		{
			_energy.AddEnergy(Entity<XenoEnergyComponent>.op_Implicit((ent.Owner, energy)), ent.Comp.ShardsOnDamage);
		}
	}

	private void OnShardLevelChanged(Entity<XenoShardComponent> ent, ref XenoEnergyChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateSpikes(ent, args.NewEnergy);
	}

	private void UpdateSpikes(Entity<XenoShardComponent> ent, FixedPoint2 shards)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		StunOnExplosionReceivedComponent explosion = default(StunOnExplosionReceivedComponent);
		if (((EntitySystem)this).TryComp<StunOnExplosionReceivedComponent>(Entity<XenoShardComponent>.op_Implicit(ent), ref explosion))
		{
			if (shards >= 50)
			{
				_explosion.ChangeExplosionStunResistance(Entity<XenoShardComponent>.op_Implicit(ent), explosion, isStunnable: false);
			}
			else
			{
				_explosion.ChangeExplosionStunResistance(Entity<XenoShardComponent>.op_Implicit(ent), explosion, isStunnable: true);
			}
			((EntitySystem)this).Dirty<XenoShardComponent>(ent, (MetaDataComponent)null);
		}
		_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit(ent.Owner));
		UpdateHedgehogSprite(ent);
	}

	private void OnShardGetArmor(Entity<XenoShardComponent> ent, ref CMGetArmorEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		XenoEnergyComponent shards = default(XenoEnergyComponent);
		if (ent.Comp.ShardsPerArmorBonus > 0 && ((EntitySystem)this).TryComp<XenoEnergyComponent>(Entity<XenoShardComponent>.op_Implicit(ent), ref shards))
		{
			float bonusArmor = (float)(shards.Current / ent.Comp.ShardsPerArmorBonus) * ent.Comp.ArmorPerShard;
			args.XenoArmor += (int)bonusArmor;
		}
	}

	private void OnSpikeShed(Entity<XenoSpikeShedComponent> ent, ref ActionXenoSpikeShedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		XenoShardComponent shards = default(XenoShardComponent);
		XenoEnergyComponent energy = default(XenoEnergyComponent);
		if (((EntitySystem)this).TryComp<XenoShardComponent>(Entity<XenoSpikeShedComponent>.op_Implicit(ent), ref shards) && ((EntitySystem)this).TryComp<XenoEnergyComponent>(Entity<XenoSpikeShedComponent>.op_Implicit(ent), ref energy) && _energy.HasEnergyPopup(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoSpikeShedComponent>.op_Implicit(ent), energy)), ent.Comp.MinShards))
		{
			_energy.RemoveEnergy(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoSpikeShedComponent>.op_Implicit(ent), energy)), energy.Current);
			shards.SpikeShedCooldownEnd = _timing.CurTime + TimeSpan.FromSeconds(30L);
			((EntitySystem)this).Dirty(Entity<XenoSpikeShedComponent>.op_Implicit(ent), (IComponent)(object)shards, (MetaDataComponent)null);
			_armor.UpdateArmorValue(Entity<CMArmorComponent>.op_Implicit(ent.Owner));
			UpdateHedgehogSprite(Entity<XenoShardComponent>.op_Implicit((ent.Owner, shards)));
			if (_net.IsServer)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-shed-spikes"), Entity<XenoSpikeShedComponent>.op_Implicit(ent), Entity<XenoSpikeShedComponent>.op_Implicit(ent));
			}
			_audio.PlayPredicted(ent.Comp.Sound, Entity<XenoSpikeShedComponent>.op_Implicit(ent), (EntityUid?)Entity<XenoSpikeShedComponent>.op_Implicit(ent), (AudioParams?)null);
			((EntitySystem)this).EnsureComp<XenoSweepingComponent>(Entity<XenoSpikeShedComponent>.op_Implicit(ent));
			XenoProjectileSystem xenoProjectile = _xenoProjectile;
			EntityUid owner = ent.Owner;
			EntityCoordinates targetCoords = new EntityCoordinates(Entity<XenoSpikeShedComponent>.op_Implicit(ent), Vector2.UnitX * ent.Comp.ShedRadius);
			FixedPoint2 zero = FixedPoint2.Zero;
			EntProtoId projectile = ent.Comp.Projectile;
			int projectileCount = ent.Comp.ProjectileCount;
			Angle deviation = new Angle(Math.PI * 2.0);
			int? projectileHitLimit = ent.Comp.ProjectileHitLimit;
			xenoProjectile.TryShoot(owner, targetCoords, zero, projectile, null, projectileCount, deviation, 20f, null, null, predicted: true, projectileHitLimit);
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoSpikeShedComponent>.op_Implicit(ent));
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnSpikeShield(Entity<XenoSpikeShieldComponent> ent, ref ActionXenoSpikeShieldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		XenoShardComponent shards = default(XenoShardComponent);
		if (((EntitySystem)this).TryComp<XenoShardComponent>(Entity<XenoSpikeShieldComponent>.op_Implicit(ent), ref shards) && _energy.TryRemoveEnergyPopup(Entity<XenoEnergyComponent>.op_Implicit(ent.Owner), ent.Comp.ShardCost))
		{
			ent.Comp.ShieldExpireAt = _timing.CurTime + ent.Comp.ShieldDuration;
			((EntitySystem)this).Dirty<XenoSpikeShieldComponent>(ent, (MetaDataComponent)null);
			_shield.ApplyShield(Entity<XenoSpikeShieldComponent>.op_Implicit(ent), XenoShieldSystem.ShieldType.Hedgehog, ent.Comp.ShieldAmount);
			string selfMsg = base.Loc.GetString("rmc-spike-shield-self");
			string othersMsg = base.Loc.GetString("rmc-spike-shield-others", (ValueTuple<string, object>)("user", ent));
			_popup.PopupPredicted(selfMsg, othersMsg, Entity<XenoSpikeShieldComponent>.op_Implicit(ent), Entity<XenoSpikeShieldComponent>.op_Implicit(ent));
			_aura.GiveAura(Entity<XenoSpikeShieldComponent>.op_Implicit(ent), Color.Blue, ent.Comp.ShieldDuration);
			((EntitySystem)this).Spawn(EntProtoId.op_Implicit(ent.Comp.EffectId), ent.Owner.ToCoordinates());
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void UpdateHedgehogSprite(Entity<XenoShardComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		XenoEnergyComponent energy = default(XenoEnergyComponent);
		if (((EntitySystem)this).TryComp<XenoEnergyComponent>(Entity<XenoShardComponent>.op_Implicit(ent), ref energy))
		{
			int current = energy.Current;
			XenoShardLevel xenoShardLevel = ((current < 150) ? ((current < 50) ? XenoShardLevel.Level1 : XenoShardLevel.Level2) : ((current >= 300) ? XenoShardLevel.Level4 : XenoShardLevel.Level3));
			XenoShardLevel level = xenoShardLevel;
			_appearance.SetData(Entity<XenoShardComponent>.op_Implicit(ent), (Enum)XenoShardVisuals.Level, (object)level, (AppearanceComponent)null);
		}
	}

	private void OnMapInit(Entity<XenoShardComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateHedgehogSprite(ent);
	}

	private void OnRefreshMovementSpeed(Entity<XenoShardComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.SpikeShedCooldownEnd.HasValue)
		{
			args.ModifySpeed(1f + ent.Comp.SpeedModifier, 1f + ent.Comp.SpeedModifier);
		}
	}

	private void OnSpikeEnergyGain(Entity<XenoShardComponent> ent, ref XenoEnergyGainAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.SpikeShedCooldownEnd.HasValue)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
