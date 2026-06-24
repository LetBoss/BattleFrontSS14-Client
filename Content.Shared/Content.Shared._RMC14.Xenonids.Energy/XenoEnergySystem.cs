using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.TrainingDummy;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.Alert;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;
using Content.Shared.Standing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Energy;

public sealed class XenoEnergySystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private StandingStateSystem _stand;

	private void OnXenoPlasmaMapInit(Entity<XenoEnergyComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAlert(ent);
	}

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoEnergyComponent, MapInitEvent>((EntityEventRefHandler<XenoEnergyComponent, MapInitEvent>)OnXenoEnergyMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEnergyComponent, ComponentRemove>((EntityEventRefHandler<XenoEnergyComponent, ComponentRemove>)OnXenoEnergyRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEnergyComponent, MeleeHitEvent>((EntityEventRefHandler<XenoEnergyComponent, MeleeHitEvent>)OnMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEnergyComponent, XenoProjectileHitUserEvent>((EntityEventRefHandler<XenoEnergyComponent, XenoProjectileHitUserEvent>)OnXenoProjectileHitUser, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEnergyComponent, RejuvenateEvent>((EntityEventRefHandler<XenoEnergyComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActionEnergyComponent, RMCActionUseAttemptEvent>((EntityEventRefHandler<XenoActionEnergyComponent, RMCActionUseAttemptEvent>)OnXenoActionEnergyUseAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActionEnergyComponent, RMCActionUseEvent>((EntityEventRefHandler<XenoActionEnergyComponent, RMCActionUseEvent>)OnXenoActionEnergyUse, (Type[])null, (Type[])null);
	}

	private void OnXenoEnergyMapInit(Entity<XenoEnergyComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAlert(ent);
	}

	private void OnXenoEnergyRemove(Entity<XenoEnergyComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlert(Entity<XenoEnergyComponent>.op_Implicit(ent), ent.Comp.Alert);
	}

	private void OnMeleeHit(Entity<XenoEnergyComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit)
		{
			return;
		}
		bool isHit = false;
		bool isDown = false;
		VictimInfectedComponent infect = default(VictimInfectedComponent);
		foreach (EntityUid hit in args.HitEntities)
		{
			if (!_xeno.CanAbilityAttackTarget(xeno.Owner, hit) || (xeno.Comp.IgnoreLateInfected && ((EntitySystem)this).TryComp<VictimInfectedComponent>(hit, ref infect) && infect.CurrentStage >= infect.FinalSymptomsStart))
			{
				continue;
			}
			if (((EntitySystem)this).HasComp<RMCTrainingDummyComponent>(hit))
			{
				return;
			}
			isHit = true;
			if (_stand.IsDown(hit))
			{
				isDown = true;
			}
			break;
		}
		if (isHit)
		{
			AddEnergy(xeno, isDown ? xeno.Comp.GainAttackDowned : xeno.Comp.GainAttack);
			UpdateAlert(xeno);
		}
	}

	private void OnXenoProjectileHitUser(Entity<XenoEnergyComponent> xeno, ref XenoProjectileHitUserEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (xeno.Comp.GainOnProjectiles && _xeno.CanAbilityAttackTarget(Entity<XenoEnergyComponent>.op_Implicit(xeno), args.Hit))
		{
			AddEnergy(xeno, xeno.Comp.GainAttack);
			UpdateAlert(xeno);
		}
	}

	private void OnRejuvenate(Entity<XenoEnergyComponent> ent, ref RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		AddEnergy(ent, ent.Comp.Max);
		UpdateAlert(ent);
	}

	private void UpdateAlert(Entity<XenoEnergyComponent> xeno)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		float level = MathF.Max(0f, xeno.Comp.Current);
		short max = _alerts.GetMaxSeverity(xeno.Comp.Alert);
		int severity = max - ContentHelpers.RoundToLevels(level, xeno.Comp.Max, max + 1);
		int current = xeno.Comp.Current;
		string energyResourceMessage = current + " / " + xeno.Comp.Max;
		AlertsSystem alerts = _alerts;
		EntityUid euid = Entity<XenoEnergyComponent>.op_Implicit(xeno);
		ProtoId<AlertPrototype> alert = xeno.Comp.Alert;
		short? severity2 = (short)severity;
		string dynamicMessage = energyResourceMessage;
		alerts.ShowAlert(euid, alert, severity2, null, autoRemove: false, showCooldown: true, dynamicMessage);
	}

	private void OnXenoActionEnergyUseAttempt(Entity<XenoActionEnergyComponent> action, ref RMCActionUseAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !HasEnergyPopup(Entity<XenoEnergyComponent>.op_Implicit(args.User), action.Comp.Cost))
		{
			args.Cancelled = true;
		}
	}

	private void OnXenoActionEnergyUse(Entity<XenoActionEnergyComponent> action, ref RMCActionUseEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		XenoEnergyComponent energy = default(XenoEnergyComponent);
		if (((EntitySystem)this).TryComp<XenoEnergyComponent>(args.User, ref energy))
		{
			RemoveEnergy(Entity<XenoEnergyComponent>.op_Implicit((args.User, energy)), action.Comp.Cost);
		}
	}

	public void AddEnergy(Entity<XenoEnergyComponent> xeno, int energy, bool popup = true)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		XenoEnergyGainAttemptEvent rev = new XenoEnergyGainAttemptEvent();
		((EntitySystem)this).RaiseLocalEvent<XenoEnergyGainAttemptEvent>(Entity<XenoEnergyComponent>.op_Implicit(xeno), rev, false);
		if (!((CancellableEntityEventArgs)rev).Cancelled)
		{
			if (popup && xeno.Comp.Current < xeno.Comp.Max && energy > 0)
			{
				_popup.PopupClient(base.Loc.GetString(xeno.Comp.PopupGain), Entity<XenoEnergyComponent>.op_Implicit(xeno), Entity<XenoEnergyComponent>.op_Implicit(xeno));
			}
			xeno.Comp.Current = Math.Min(xeno.Comp.Max, xeno.Comp.Current + energy);
			((EntitySystem)this).Dirty<XenoEnergyComponent>(xeno, (MetaDataComponent)null);
			UpdateAlert(xeno);
			XenoEnergyChangedEvent ev = new XenoEnergyChangedEvent(xeno.Comp.Current);
			((EntitySystem)this).RaiseLocalEvent<XenoEnergyChangedEvent>(Entity<XenoEnergyComponent>.op_Implicit(xeno), ref ev, false);
		}
	}

	public bool HasEnergy(Entity<XenoEnergyComponent> xeno, int energy)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return xeno.Comp.Current >= energy;
	}

	public bool HasEnergyPopup(Entity<XenoEnergyComponent?> xeno, int energy, bool predicted = true)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoEnergyComponent>(Entity<XenoEnergyComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			DoPopup();
			return false;
		}
		if (!HasEnergy(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoEnergyComponent>.op_Implicit(xeno), xeno.Comp)), energy))
		{
			DoPopup();
			return false;
		}
		return true;
		void DoPopup()
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			string popup = base.Loc.GetString((xeno.Comp != null) ? xeno.Comp.PopupNotEnough : "rmc-xeno-not-enough-energy");
			if (predicted)
			{
				_popup.PopupClient(popup, Entity<XenoEnergyComponent>.op_Implicit(xeno), Entity<XenoEnergyComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			}
			else
			{
				_popup.PopupEntity(popup, Entity<XenoEnergyComponent>.op_Implicit(xeno), Entity<XenoEnergyComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			}
		}
	}

	public void RemoveEnergy(Entity<XenoEnergyComponent?> xeno, int plasma)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<XenoEnergyComponent>(Entity<XenoEnergyComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			xeno.Comp.Current = int.Max(0, xeno.Comp.Current - plasma);
			UpdateAlert(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoEnergyComponent>.op_Implicit(xeno), xeno.Comp)));
			XenoEnergyChangedEvent ev = new XenoEnergyChangedEvent(xeno.Comp.Current);
			((EntitySystem)this).RaiseLocalEvent<XenoEnergyChangedEvent>(Entity<XenoEnergyComponent>.op_Implicit(xeno), ref ev, false);
			((EntitySystem)this).Dirty<XenoEnergyComponent>(xeno, (MetaDataComponent)null);
		}
	}

	public bool TryRemoveEnergy(Entity<XenoEnergyComponent?> xeno, int energy)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoEnergyComponent>(Entity<XenoEnergyComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			return false;
		}
		if (!HasEnergy(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoEnergyComponent>.op_Implicit(xeno), xeno.Comp)), energy))
		{
			return false;
		}
		RemoveEnergy(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoEnergyComponent>.op_Implicit(xeno), xeno.Comp)), energy);
		return true;
	}

	public bool TryRemoveEnergyPopup(Entity<XenoEnergyComponent?> xeno, int energy)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoEnergyComponent>(Entity<XenoEnergyComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			return false;
		}
		if (TryRemoveEnergy(Entity<XenoEnergyComponent>.op_Implicit((Entity<XenoEnergyComponent>.op_Implicit(xeno), xeno.Comp)), energy))
		{
			return true;
		}
		_popup.PopupClient(base.Loc.GetString(xeno.Comp.PopupNotEnough), Entity<XenoEnergyComponent>.op_Implicit(xeno), Entity<XenoEnergyComponent>.op_Implicit(xeno));
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoEnergyComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoEnergyComponent>();
		EntityUid uid = default(EntityUid);
		XenoEnergyComponent comp = default(XenoEnergyComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (!_mobState.IsDead(uid) && !(time < comp.NextGain))
			{
				comp.NextGain = time + comp.GainEvery;
				if (!comp.GenerationCap.HasValue || comp.Current < comp.GenerationCap)
				{
					AddEnergy(Entity<XenoEnergyComponent>.op_Implicit((uid, comp)), comp.Gain, popup: false);
				}
				((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			}
		}
	}
}
