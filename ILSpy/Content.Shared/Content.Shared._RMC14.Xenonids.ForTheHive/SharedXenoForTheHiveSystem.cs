using System;
using System.Numerics;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Vents;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.ForTheHive;

public abstract class SharedXenoForTheHiveSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private XenoEnergySystem _energy;

	[Dependency]
	protected SharedPopupSystem _popup;

	[Dependency]
	protected SharedXenoHiveSystem _hive;

	[Dependency]
	protected IGameTiming _timing;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	protected SharedAudioSystem _audio;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPointLightSystem _pointLight;

	[Dependency]
	private MobStateSystem _mob;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	protected SharedTransformSystem _transform;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedBodySystem _body;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedXenoAcidSystem _acid;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private MovementSpeedModifierSystem _movement;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ForTheHiveComponent, XenoForTheHiveActionEvent>((EntityEventRefHandler<ForTheHiveComponent, XenoForTheHiveActionEvent>)OnForTheHiveActivated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveForTheHiveComponent, ComponentStartup>((EntityEventRefHandler<ActiveForTheHiveComponent, ComponentStartup>)OnForTheHiveAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveForTheHiveComponent, ComponentShutdown>((EntityEventRefHandler<ActiveForTheHiveComponent, ComponentShutdown>)OnForTheHiveRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveForTheHiveComponent, ComponentRemove>((EntityEventRefHandler<ActiveForTheHiveComponent, ComponentRemove>)OnForTheHiveGone, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveForTheHiveComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<ActiveForTheHiveComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveForTheHiveComponent, VentEnterAttemptEvent>((EntityEventRefHandler<ActiveForTheHiveComponent, VentEnterAttemptEvent>)OnVentCrawlAttempt, (Type[])null, (Type[])null);
	}

	private void OnForTheHiveActivated(Entity<ForTheHiveComponent> xeno, ref XenoForTheHiveActionEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		if (_container.IsEntityInContainer(Entity<ForTheHiveComponent>.op_Implicit(xeno), (MetaDataComponent)null))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-for-the-hive-container"), Entity<ForTheHiveComponent>.op_Implicit(xeno), Entity<ForTheHiveComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		else if (((EntitySystem)this).HasComp<ActiveForTheHiveComponent>(Entity<ForTheHiveComponent>.op_Implicit(xeno)))
		{
			XenoEnergyComponent acid = default(XenoEnergyComponent);
			if (((EntitySystem)this).TryComp<XenoEnergyComponent>(Entity<ForTheHiveComponent>.op_Implicit(xeno), ref acid))
			{
				_energy.TryRemoveEnergy(Entity<XenoEnergyComponent>.op_Implicit(xeno.Owner), acid.Current / 4);
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-for-the-hive-cancel"), Entity<ForTheHiveComponent>.op_Implicit(xeno), Entity<ForTheHiveComponent>.op_Implicit(xeno));
				((EntitySystem)this).RemCompDeferred<ActiveForTheHiveComponent>(Entity<ForTheHiveComponent>.op_Implicit(xeno));
			}
		}
		else if (_energy.HasEnergyPopup(Entity<XenoEnergyComponent>.op_Implicit(xeno.Owner), xeno.Comp.Minimum))
		{
			ActiveForTheHiveComponent activeForTheHiveComponent = ((EntitySystem)this).EnsureComp<ActiveForTheHiveComponent>(Entity<ForTheHiveComponent>.op_Implicit(xeno));
			activeForTheHiveComponent.Duration = xeno.Comp.Duration;
			activeForTheHiveComponent.TimeLeft = xeno.Comp.Duration;
			activeForTheHiveComponent.BaseDamage = xeno.Comp.BaseDamage;
			activeForTheHiveComponent.MobAcid = xeno.Comp.Acid;
			ForTheHiveShout(Entity<ForTheHiveComponent>.op_Implicit(xeno));
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-for-the-hive-activate"), Entity<ForTheHiveComponent>.op_Implicit(xeno), Entity<ForTheHiveComponent>.op_Implicit(xeno), PopupType.Medium);
		}
	}

	protected virtual void ForTheHiveShout(EntityUid xeno)
	{
	}

	private void OnForTheHiveAdded(Entity<ActiveForTheHiveComponent> xeno, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.NextUpdate = _timing.CurTime + xeno.Comp.UpdateEvery;
		ForTheHiveActivatedEvent ev = new ForTheHiveActivatedEvent();
		((EntitySystem)this).RaiseLocalEvent<ForTheHiveActivatedEvent>(Entity<ActiveForTheHiveComponent>.op_Implicit(xeno), ref ev, false);
		_pointLight.SetEnabled(Entity<ActiveForTheHiveComponent>.op_Implicit(xeno), true, (SharedPointLightComponent)null, (MetaDataComponent)null);
		_movement.RefreshMovementSpeedModifiers(Entity<ActiveForTheHiveComponent>.op_Implicit(xeno));
	}

	private void OnForTheHiveRemoved(Entity<ActiveForTheHiveComponent> xeno, ref ComponentShutdown args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		ForTheHiveCancelledEvent ev = new ForTheHiveCancelledEvent();
		((EntitySystem)this).RaiseLocalEvent<ForTheHiveCancelledEvent>(Entity<ActiveForTheHiveComponent>.op_Implicit(xeno), ref ev, false);
		_pointLight.SetEnabled(Entity<ActiveForTheHiveComponent>.op_Implicit(xeno), false, (SharedPointLightComponent)null, (MetaDataComponent)null);
	}

	private void OnForTheHiveGone(Entity<ActiveForTheHiveComponent> xeno, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movement.RefreshMovementSpeedModifiers(Entity<ActiveForTheHiveComponent>.op_Implicit(xeno));
	}

	private void OnRefreshSpeed(Entity<ActiveForTheHiveComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		float multiplier = xeno.Comp.SlowDown.Float();
		args.ModifySpeed(multiplier, multiplier);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<ActiveForTheHiveComponent> forTheHiveQuery = ((EntitySystem)this).EntityQueryEnumerator<ActiveForTheHiveComponent>();
		EntityUid xeno = default(EntityUid);
		ActiveForTheHiveComponent active = default(ActiveForTheHiveComponent);
		XenoEnergyComponent acid = default(XenoEnergyComponent);
		float distance = default(float);
		MapGridComponent grid2 = default(MapGridComponent);
		while (forTheHiveQuery.MoveNext(ref xeno, ref active))
		{
			if (!(active.NextUpdate <= time) || _mob.IsDead(xeno))
			{
				continue;
			}
			if (active.TimeLeft.TotalSeconds % 2.0 == 0.0 && (active.TimeLeft - active.UpdateEvery).TotalSeconds > 0.0)
			{
				float volume = -3f + (float)((double)active.MaxVolume * (1.0 - active.TimeLeft.TotalSeconds / active.Duration.TotalSeconds));
				if (active.UseWindUpSound)
				{
					_audio.PlayPvs(active.WindingUpSound, xeno, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(volume));
				}
				else
				{
					_audio.PlayPvs(active.WindingDownSound, xeno, (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(volume));
				}
				active.UseWindUpSound = !active.UseWindUpSound;
			}
			active.TimeLeft -= active.UpdateEvery;
			active.NextUpdate += active.UpdateEvery;
			_appearance.SetData(xeno, (Enum)ForTheHiveVisuals.Time, (object)(float)(active.TimeLeft / active.Duration), (AppearanceComponent)null);
			PopupType popupType = PopupType.MediumCaution;
			if (active.TimeLeft / active.Duration <= 0.5)
			{
				popupType = PopupType.LargeCaution;
			}
			if (active.TimeLeft.TotalSeconds > 0.0)
			{
				_popup.PopupEntity(active.TimeLeft.TotalSeconds.ToString(), xeno, xeno, popupType);
				continue;
			}
			if (!((EntitySystem)this).TryComp<XenoEnergyComponent>(xeno, ref acid))
			{
				break;
			}
			float acidRange = (float)Math.Sqrt(Math.Pow((float)acid.Current / active.AcidRangeRatio * 2f + 1f, 2.0) / Math.PI);
			float burnRange = (float)Math.Sqrt(Math.Pow((float)acid.Current / active.BurnRangeRatio * 2f + 1f, 2.0) / Math.PI);
			float maxBurnDamage = (float)acid.Current / active.BurnDamageRatio;
			EntityCoordinates origin = _transform.GetMoverCoordinates(xeno);
			foreach (Entity<BarricadeComponent> cade in _lookup.GetEntitiesInRange<BarricadeComponent>(origin, acidRange, (LookupFlags)110))
			{
				if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno), Entity<TransformComponent>.op_Implicit(cade.Owner), acidRange, CollisionGroup.Impassable))
				{
					continue;
				}
				if (_acid.IsMelted(Entity<BarricadeComponent>.op_Implicit(cade)))
				{
					if (!_acid.CanReplaceAcid(Entity<BarricadeComponent>.op_Implicit(cade), active.AcidStrength))
					{
						continue;
					}
					_acid.RemoveAcid(Entity<BarricadeComponent>.op_Implicit(cade));
				}
				_acid.ApplyAcid(active.Acid, active.AcidStrength, Entity<BarricadeComponent>.op_Implicit(cade), active.AcidDps, 0f, active.AcidTime);
			}
			foreach (Entity<MobStateComponent> mob in _lookup.GetEntitiesInRange<MobStateComponent>(origin, burnRange, (LookupFlags)110))
			{
				if (!_xeno.CanAbilityAttackTarget(xeno, Entity<MobStateComponent>.op_Implicit(mob)))
				{
					continue;
				}
				if (_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno), Entity<TransformComponent>.op_Implicit(mob.Owner), acidRange, CollisionGroup.Impassable))
				{
					ComponentRegistry add = active.MobAcid;
					if (add != null)
					{
						base.EntityManager.AddComponents(Entity<MobStateComponent>.op_Implicit(mob), add, true);
					}
				}
				if (_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno), Entity<TransformComponent>.op_Implicit(mob.Owner), burnRange, CollisionGroup.Impassable) && ((EntityCoordinates)(ref origin)).TryDistance((IEntityManager)(object)base.EntityManager, _transform.GetMoverCoordinates(Entity<MobStateComponent>.op_Implicit(mob)), ref distance))
				{
					float damage = (burnRange - distance) * maxBurnDamage / burnRange;
					_damage.TryChangeDamage(Entity<MobStateComponent>.op_Implicit(mob), _xeno.TryApplyXenoAcidDamageMultiplier(Entity<MobStateComponent>.op_Implicit(mob), active.BaseDamage * damage), ignoreResistances: true, interruptsDoAfters: true, null, xeno, xeno);
				}
			}
			EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(xeno));
			if (!grid.HasValue)
			{
				continue;
			}
			EntityUid gridId = grid.GetValueOrDefault();
			if (!((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				continue;
			}
			foreach (TileRef turf in _map.GetTilesIntersecting(gridId, grid2, Box2.CenteredAround(origin.Position, new Vector2(acidRange * 2f, acidRange * 2f)), false, (Predicate<TileRef>)null))
			{
				if (_interaction.InRangeUnobstructed(_transform.ToMapCoordinates(origin, true), _transform.ToMapCoordinates(_turf.GetTileCenter(turf), true), acidRange, CollisionGroup.Impassable) && !_turf.IsTileBlocked(turf, CollisionGroup.Impassable))
				{
					((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(active.AcidSmoke), _turf.GetTileCenter(turf), (ComponentRegistry)null);
				}
			}
			if (GetHiveCore(xeno, out var _))
			{
				ForTheHiveRespawn(xeno, active.CoreSpawnTime);
			}
			else
			{
				ForTheHiveRespawn(xeno, active.CorpseSpawnTime, atCorpse: true, origin);
			}
			_audio.PlayStatic(active.KaboomSound, Filter.PvsExcept(xeno, 2f, (IEntityManager)null), origin, true, (AudioParams?)null);
			_body.GibBody(xeno);
			((EntitySystem)this).RemCompDeferred<ActiveForTheHiveComponent>(xeno);
		}
	}

	protected bool GetHiveCore(EntityUid xeno, out EntityUid? core)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<HiveCoreComponent, HiveMemberComponent> cores = ((EntitySystem)this).EntityQueryEnumerator<HiveCoreComponent, HiveMemberComponent>();
		EntityUid uid = default(EntityUid);
		HiveCoreComponent hiveCoreComponent = default(HiveCoreComponent);
		HiveMemberComponent hiveMemberComponent = default(HiveMemberComponent);
		while (cores.MoveNext(ref uid, ref hiveCoreComponent, ref hiveMemberComponent))
		{
			if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno), Entity<HiveMemberComponent>.op_Implicit(uid)) && !_mob.IsDead(uid))
			{
				core = uid;
				return true;
			}
		}
		core = null;
		return false;
	}

	protected virtual void ForTheHiveRespawn(EntityUid xeno, TimeSpan time, bool atCorpse = false, EntityCoordinates? corpse = null)
	{
	}

	private void OnVentCrawlAttempt(Entity<ActiveForTheHiveComponent> xeno, ref VentEnterAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_popup.PopupClient(base.Loc.GetString("rmc-vent-crawling-primed"), Entity<ActiveForTheHiveComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		((CancellableEntityEventArgs)args).Cancel();
	}
}
