using System;
using System.Linq;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Acid;

public abstract class SharedXenoAcidSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedDropshipSystem _dropship;

	[Dependency]
	private SharedEntityStorageSystem _entityStorage;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	protected IPrototypeManager PrototypeManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private XenoEnergySystem _xenoEnergy;

	protected int CorrosiveAcidTickDelaySeconds;

	protected ProtoId<DamageTypePrototype> CorrosiveAcidDamageTypeStr = ProtoId<DamageTypePrototype>.op_Implicit("Heat");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidComponent, XenoCorrosiveAcidEvent>((EntityEventRefHandler<XenoAcidComponent, XenoCorrosiveAcidEvent>)OnXenoCorrosiveAcid, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidComponent, DoAfterAttemptEvent<XenoCorrosiveAcidDoAfterEvent>>((EntityEventRefHandler<XenoAcidComponent, DoAfterAttemptEvent<XenoCorrosiveAcidDoAfterEvent>>)OnXenoCorrosiveAcidDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoAcidComponent, XenoCorrosiveAcidDoAfterEvent>((EntityEventRefHandler<XenoAcidComponent, XenoCorrosiveAcidDoAfterEvent>)OnXenoCorrosiveAcidDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InheritAcidComponent, AmmoShotEvent>((EntityEventRefHandler<InheritAcidComponent, AmmoShotEvent>)OnAmmoShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InheritAcidComponent, GrenadeContentThrownEvent>((EntityEventRefHandler<InheritAcidComponent, GrenadeContentThrownEvent>)OnGrenadeContentThrown, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCCorrosiveAcidTickDelaySeconds, (Action<int>)delegate(int obj)
		{
			CorrosiveAcidTickDelaySeconds = obj;
			OnXenoAcidSystemCVarsUpdated();
		}, true);
		EntitySystemSubscriptionExt.CVar<string>(((EntitySystem)this).Subs, _config, RMCCVars.RMCCorrosiveAcidDamageType, (Action<string>)delegate(string obj)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			CorrosiveAcidDamageTypeStr = ProtoId<DamageTypePrototype>.op_Implicit(obj);
			OnXenoAcidSystemCVarsUpdated();
		}, true);
	}

	private void OnXenoAcidSystemCVarsUpdated()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<DamageableCorrodingComponent> damageableCorrodingQuery = ((EntitySystem)this).EntityQueryEnumerator<DamageableCorrodingComponent>();
		EntityUid uid = default(EntityUid);
		DamageableCorrodingComponent damageableCorrodingComponent = default(DamageableCorrodingComponent);
		while (damageableCorrodingQuery.MoveNext(ref uid, ref damageableCorrodingComponent))
		{
			damageableCorrodingComponent.Damage = new DamageSpecifier(PrototypeManager.Index<DamageTypePrototype>(CorrosiveAcidDamageTypeStr), damageableCorrodingComponent.Dps * (float)CorrosiveAcidTickDelaySeconds);
		}
	}

	private void OnXenoCorrosiveAcid(Entity<XenoAcidComponent> xeno, ref XenoCorrosiveAcidEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		EntityUid val = target;
		EntityUid e = val;
		DropshipWeaponPointComponent weapon = default(DropshipWeaponPointComponent);
		string text;
		if (((EntitySystem)this).TryComp<DropshipWeaponPointComponent>(e, ref weapon))
		{
			text = weapon.WeaponContainerSlotId;
		}
		else
		{
			EntityUid e2 = val;
			DropshipUtilityPointComponent utility = default(DropshipUtilityPointComponent);
			if (((EntitySystem)this).TryComp<DropshipUtilityPointComponent>(e2, ref utility))
			{
				text = utility.UtilitySlotId;
			}
			else
			{
				EntityUid e3 = val;
				DropshipElectronicSystemPointComponent electronic = default(DropshipElectronicSystemPointComponent);
				if (((EntitySystem)this).TryComp<DropshipElectronicSystemPointComponent>(e3, ref electronic))
				{
					text = electronic.ContainerId;
				}
				else
				{
					EntityUid e4 = val;
					DropshipEnginePointComponent engine = default(DropshipEnginePointComponent);
					text = ((!((EntitySystem)this).TryComp<DropshipEnginePointComponent>(e4, ref engine)) ? string.Empty : engine.ContainerId);
				}
			}
		}
		string containerId = text;
		if (!string.IsNullOrEmpty(containerId))
		{
			if (!_dropship.TryGetAttachmentContained(target, containerId, out var containedEntity))
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-acid-not-corrodible", (ValueTuple<string, object>)("target", target)), Entity<XenoAcidComponent>.op_Implicit(xeno), Entity<XenoAcidComponent>.op_Implicit(xeno), PopupType.SmallCaution);
				return;
			}
			target = containedEntity;
		}
		if (!(xeno.Owner != args.Performer) && CheckCorrodiblePopupsWithReplacement(xeno, target, args.Strength, out var time, out var _))
		{
			((HandledEntityEventArgs)args).Handled = true;
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoAcidComponent>.op_Implicit(xeno), time * args.ApplyTimeMultiplier, new XenoCorrosiveAcidDoAfterEvent(args), Entity<XenoAcidComponent>.op_Implicit(xeno), target)
			{
				BreakOnMove = true,
				RequireCanInteract = false,
				AttemptFrequency = AttemptFrequency.StartAndEnd
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnXenoCorrosiveAcidDoAfterAttempt(Entity<XenoAcidComponent> ent, ref DoAfterAttemptEvent<XenoCorrosiveAcidDoAfterEvent> args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && _mobState.IsIncapacitated(Entity<XenoAcidComponent>.op_Implicit(ent)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnXenoCorrosiveAcidDoAfter(Entity<XenoAcidComponent> xeno, ref XenoCorrosiveAcidDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		CorrodibleComponent corrodible = default(CorrodibleComponent);
		if (!((EntitySystem)this).TryComp<CorrodibleComponent>(target2, ref corrodible) || !corrodible.IsCorrodible || (!xeno.Comp.CanMeltStructures && corrodible.Structure) || (IsMelted(target2) && !CanReplaceAcid(target2, args.Strength)))
		{
			return;
		}
		float mult = corrodible.MeltTimeMult;
		if ((!(args.PlasmaCost != 0) || _xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), args.PlasmaCost)) && (args.EnergyCost == 0 || _xenoEnergy.TryRemoveEnergyPopup(Entity<XenoEnergyComponent>.op_Implicit(xeno.Owner), args.EnergyCost)) && !_net.IsClient)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (_net.IsServer && IsMelted(target2))
			{
				RemoveAcid(target2);
			}
			ApplyAcid(args.AcidId, args.Strength, target2, args.Dps, args.ExpendableLightDps, args.Time * mult);
		}
	}

	private void OnAmmoShot(Entity<InheritAcidComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		TimedCorrodingComponent corroding = default(TimedCorrodingComponent);
		if (!((EntitySystem)this).TryComp<TimedCorrodingComponent>(Entity<InheritAcidComponent>.op_Implicit(ent), ref corroding))
		{
			return;
		}
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			ApplyAcid(corroding.AcidPrototype, corroding.Strength, projectile, corroding.LightDps, corroding.Dps, corroding.CorrodesAt, inherit: true);
		}
	}

	private void OnGrenadeContentThrown(Entity<InheritAcidComponent> ent, ref GrenadeContentThrownEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		TimedCorrodingComponent corroding = default(TimedCorrodingComponent);
		if (((EntitySystem)this).TryComp<TimedCorrodingComponent>(args.Source, ref corroding))
		{
			ApplyAcid(corroding.AcidPrototype, corroding.Strength, Entity<InheritAcidComponent>.op_Implicit(ent), corroding.Dps, corroding.LightDps, corroding.CorrodesAt, inherit: true);
		}
	}

	private bool CheckCorrodiblePopupsWithReplacement(Entity<XenoAcidComponent> xeno, EntityUid target, XenoAcidStrength newStrength, out TimeSpan time, out float mult)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		time = TimeSpan.Zero;
		mult = 1f;
		CorrodibleComponent corrodible = default(CorrodibleComponent);
		if (!((EntitySystem)this).TryComp<CorrodibleComponent>(target, ref corrodible) || !corrodible.IsCorrodible)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-acid-not-corrodible", (ValueTuple<string, object>)("target", target)), Entity<XenoAcidComponent>.op_Implicit(xeno), Entity<XenoAcidComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return false;
		}
		if (IsMelted(target) && !CanReplaceAcid(target, newStrength))
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-acid-already-corroding", (ValueTuple<string, object>)("target", target)), Entity<XenoAcidComponent>.op_Implicit(xeno), Entity<XenoAcidComponent>.op_Implicit(xeno));
			return false;
		}
		if (!xeno.Comp.CanMeltStructures && corrodible.Structure)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-acid-structure-unmeltable"), Entity<XenoAcidComponent>.op_Implicit(xeno), Entity<XenoAcidComponent>.op_Implicit(xeno));
			return false;
		}
		if (newStrength.CompareTo(corrodible.MinimumAcidStrength) < 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-acid-too-weak", (ValueTuple<string, object>)("target", target)), Entity<XenoAcidComponent>.op_Implicit(xeno), Entity<XenoAcidComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return false;
		}
		time = corrodible.TimeToApply;
		mult = corrodible.MeltTimeMult;
		return true;
	}

	public void ApplyAcid(EntProtoId acidId, XenoAcidStrength strength, EntityUid target, float dps, float lightDps, TimeSpan time, bool inherit = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			BaseContainer container = default(BaseContainer);
			EntityUid acid = ((!((EntitySystem)this).HasComp<VisiblyAcidOutsideContainerComponent>(target) || !_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(target), ref container)) ? ((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(acidId), target.ToCoordinates(), (ComponentRegistry)null, default(Angle)) : ((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(acidId), container.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle)));
			if (!inherit)
			{
				time += _timing.CurTime;
			}
			CorrodingEvent ev = new CorrodingEvent(acid, dps, lightDps, strength);
			((EntitySystem)this).RaiseLocalEvent<CorrodingEvent>(target, ref ev, false);
			if (!ev.Cancelled)
			{
				((EntitySystem)this).AddComp<TimedCorrodingComponent>(target, new TimedCorrodingComponent
				{
					Acid = acid,
					AcidPrototype = acidId,
					Strength = strength,
					CorrodesAt = time,
					Dps = dps,
					LightDps = lightDps
				}, false);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<DamageableCorrodingComponent> damageableCorrodingQuery = ((EntitySystem)this).EntityQueryEnumerator<DamageableCorrodingComponent>();
		EntityUid uid = default(EntityUid);
		DamageableCorrodingComponent damageableCorrodingComponent = default(DamageableCorrodingComponent);
		while (damageableCorrodingQuery.MoveNext(ref uid, ref damageableCorrodingComponent))
		{
			if (time > damageableCorrodingComponent.NextDamageAt)
			{
				_damageable.TryChangeDamage(uid, damageableCorrodingComponent.Damage);
				damageableCorrodingComponent.NextDamageAt = time.Add(TimeSpan.FromSeconds(CorrosiveAcidTickDelaySeconds));
			}
			if (time > damageableCorrodingComponent.AcidExpiresAt)
			{
				BeforeMeltedEvent ev = new BeforeMeltedEvent();
				((EntitySystem)this).RaiseLocalEvent<BeforeMeltedEvent>(uid, ref ev, false);
				((EntitySystem)this).QueueDel((EntityUid?)damageableCorrodingComponent.Acid);
				((EntitySystem)this).RemCompDeferred<DamageableCorrodingComponent>(uid);
			}
		}
		EntityQueryEnumerator<TimedCorrodingComponent> timedCorrodingQuery = ((EntitySystem)this).EntityQueryEnumerator<TimedCorrodingComponent>();
		EntityUid uid2 = default(EntityUid);
		TimedCorrodingComponent timedCorrodingComponent = default(TimedCorrodingComponent);
		StorageComponent storage = default(StorageComponent);
		CorrodibleComponent corrodible = default(CorrodibleComponent);
		while (timedCorrodingQuery.MoveNext(ref uid2, ref timedCorrodingComponent))
		{
			if (time < timedCorrodingComponent.CorrodesAt)
			{
				continue;
			}
			BeforeMeltedEvent ev2 = new BeforeMeltedEvent();
			((EntitySystem)this).RaiseLocalEvent<BeforeMeltedEvent>(uid2, ref ev2, false);
			_entityStorage.EmptyContents(uid2);
			if (((EntitySystem)this).TryComp<StorageComponent>(uid2, ref storage))
			{
				EntityUid[] array = ((BaseContainer)storage.Container).ContainedEntities.ToArray();
				foreach (EntityUid contained in array)
				{
					if (!((EntitySystem)this).TryComp<CorrodibleComponent>(contained, ref corrodible) || !corrodible.IsCorrodible)
					{
						_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(contained), (BaseContainer)(object)storage.Container, true, false, (EntityCoordinates?)null, (Angle?)null);
					}
				}
			}
			((EntitySystem)this).QueueDel((EntityUid?)uid2);
			((EntitySystem)this).QueueDel((EntityUid?)timedCorrodingComponent.Acid);
		}
	}

	public bool IsMelted(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<TimedCorrodingComponent>(uid))
		{
			return ((EntitySystem)this).HasComp<DamageableCorrodingComponent>(uid);
		}
		return true;
	}

	public bool CanReplaceAcid(EntityUid target, XenoAcidStrength newStrength)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		XenoAcidStrength? existingStrength = null;
		TimedCorrodingComponent timedCorroding = default(TimedCorrodingComponent);
		DamageableCorrodingComponent damageableCorroding = default(DamageableCorrodingComponent);
		if (((EntitySystem)this).TryComp<TimedCorrodingComponent>(target, ref timedCorroding))
		{
			existingStrength = timedCorroding.Strength;
		}
		else if (((EntitySystem)this).TryComp<DamageableCorrodingComponent>(target, ref damageableCorroding))
		{
			existingStrength = damageableCorroding.Strength;
		}
		if (!existingStrength.HasValue)
		{
			return true;
		}
		return (int?)newStrength > (int?)existingStrength;
	}

	public void RemoveAcid(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		TimedCorrodingComponent timed = default(TimedCorrodingComponent);
		if (((EntitySystem)this).TryComp<TimedCorrodingComponent>(uid, ref timed))
		{
			((EntitySystem)this).QueueDel((EntityUid?)timed.Acid);
			((EntitySystem)this).RemComp<TimedCorrodingComponent>(uid);
		}
		DamageableCorrodingComponent damageable = default(DamageableCorrodingComponent);
		if (((EntitySystem)this).TryComp<DamageableCorrodingComponent>(uid, ref damageable))
		{
			((EntitySystem)this).QueueDel((EntityUid?)damageable.Acid);
			((EntitySystem)this).RemComp<DamageableCorrodingComponent>(uid);
		}
	}

	public void SetCorrodible(CorrodibleComponent component, bool isCorrodible)
	{
		component.IsCorrodible = isCorrodible;
	}
}
