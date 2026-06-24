using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Damage;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Crippling;

public sealed class XenoCripplingStrikeSystem : EntitySystem
{
	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedAuraSystem _aura;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoCripplingStrikeComponent, XenoCripplingStrikeActionEvent>((EntityEventRefHandler<XenoCripplingStrikeComponent, XenoCripplingStrikeActionEvent>)OnXenoCripplingStrikeAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, MeleeHitEvent>((EntityEventRefHandler<XenoActiveCripplingStrikeComponent, MeleeHitEvent>)OnXenoCripplingStrikeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, MeleeAttackAttemptEvent>((EntityEventRefHandler<XenoActiveCripplingStrikeComponent, MeleeAttackAttemptEvent>)OnActiveCripplingStrikeMeleeAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoActiveCripplingStrikeComponent, RefreshMovementSpeedModifiersEvent>)OnActiveCripplingRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveCripplingStrikeComponent, ComponentRemove>((EntityEventRefHandler<XenoActiveCripplingStrikeComponent, ComponentRemove>)OnActiveCripplingRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VictimCripplingStrikeDamageComponent, DamageModifyEvent>((EntityEventRefHandler<VictimCripplingStrikeDamageComponent, DamageModifyEvent>)OnVictimCripplingModify, new Type[1] { typeof(CMArmorSystem) }, (Type[])null);
	}

	private void OnXenoCripplingStrikeAction(Entity<XenoCripplingStrikeComponent> xeno, ref XenoCripplingStrikeActionEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _rmcActions.TryUseAction(args))
		{
			((HandledEntityEventArgs)args).Handled = true;
			XenoActiveCripplingStrikeComponent active = ((EntitySystem)this).EnsureComp<XenoActiveCripplingStrikeComponent>(Entity<XenoCripplingStrikeComponent>.op_Implicit(xeno));
			if (xeno.Comp.ResetMeleeCooldown)
			{
				MeleeResetComponent reset = ((EntitySystem)this).EnsureComp<MeleeResetComponent>(Entity<XenoCripplingStrikeComponent>.op_Implicit(xeno));
				_rmcMelee.MeleeResetInit(Entity<MeleeResetComponent>.op_Implicit((xeno.Owner, reset)));
			}
			active.ExpireAt = _timing.CurTime + xeno.Comp.ActiveDuration;
			active.NextSlashBuffed = true;
			active.SlowDuration = xeno.Comp.SlowDuration;
			active.DamageMult = xeno.Comp.DamageMult;
			active.HitText = xeno.Comp.HitText;
			active.DeactivateText = xeno.Comp.DeactivateText;
			active.ExpireText = xeno.Comp.ExpireText;
			active.Speed = xeno.Comp.Speed;
			active.RemoveOnHit = xeno.Comp.RemoveOnHit;
			active.PreventTackle = xeno.Comp.PreventTackle;
			((EntitySystem)this).Dirty(Entity<XenoCripplingStrikeComponent>.op_Implicit(xeno), (IComponent)(object)active, (MetaDataComponent)null);
			_popup.PopupClient(base.Loc.GetString(LocId.op_Implicit(xeno.Comp.ActivateText)), Entity<XenoCripplingStrikeComponent>.op_Implicit(xeno), Entity<XenoCripplingStrikeComponent>.op_Implicit(xeno));
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoCripplingStrikeComponent>.op_Implicit(xeno));
			Color? auraColor = xeno.Comp.AuraColor;
			if (auraColor.HasValue)
			{
				Color color = auraColor.GetValueOrDefault();
				_aura.GiveAura(Entity<XenoCripplingStrikeComponent>.op_Implicit(xeno), color, xeno.Comp.ActiveDuration, 1f);
			}
		}
	}

	private void OnXenoCripplingStrikeHit(Entity<XenoActiveCripplingStrikeComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.NextSlashBuffed || !args.IsHit || args.HitEntities.Count == 0)
		{
			return;
		}
		foreach (EntityUid entity in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoActiveCripplingStrikeComponent>.op_Implicit(xeno), entity) && !((EntitySystem)this).HasComp<VictimCripplingStrikeDamageComponent>(entity))
			{
				VictimCripplingStrikeDamageComponent victim = ((EntitySystem)this).EnsureComp<VictimCripplingStrikeDamageComponent>(entity);
				victim.DamageMult = xeno.Comp.DamageMult;
				((EntitySystem)this).Dirty(entity, (IComponent)(object)victim, (MetaDataComponent)null);
				_slow.TrySlowdown(entity, _xeno.TryApplyXenoDebuffMultiplier(entity, xeno.Comp.SlowDuration), refresh: true, ignoreDurationModifier: true);
				string message = base.Loc.GetString(LocId.op_Implicit(xeno.Comp.HitText), (ValueTuple<string, object>)("target", entity));
				if (_net.IsServer)
				{
					_popup.PopupEntity(message, entity, Entity<XenoActiveCripplingStrikeComponent>.op_Implicit(xeno));
				}
				xeno.Comp.NextSlashBuffed = false;
				break;
			}
		}
		if (xeno.Comp.RemoveOnHit)
		{
			((EntitySystem)this).RemCompDeferred<XenoActiveCripplingStrikeComponent>(Entity<XenoActiveCripplingStrikeComponent>.op_Implicit(xeno));
		}
	}

	private void OnActiveCripplingRefreshSpeed(Entity<XenoActiveCripplingStrikeComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		float? speed = xeno.Comp.Speed;
		if (speed.HasValue)
		{
			float speed2 = speed.GetValueOrDefault();
			args.ModifySpeed(speed2, speed2);
		}
	}

	private void OnActiveCripplingRemove(Entity<XenoActiveCripplingStrikeComponent> xeno, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<XenoActiveCripplingStrikeComponent>.op_Implicit(xeno), (MetaDataComponent)null))
		{
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoActiveCripplingStrikeComponent>.op_Implicit(xeno));
		}
	}

	private void OnVictimCripplingModify(Entity<VictimCripplingStrikeDamageComponent> victim, ref DamageModifyEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		args.Damage *= victim.Comp.DamageMult;
		((EntitySystem)this).RemCompDeferred<VictimCripplingStrikeDamageComponent>(Entity<VictimCripplingStrikeDamageComponent>.op_Implicit(victim));
	}

	private void OnActiveCripplingStrikeMeleeAttempt(Entity<XenoActiveCripplingStrikeComponent> ent, ref MeleeAttackAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.PreventTackle)
		{
			NetEntity netAttacker = ((EntitySystem)this).GetNetEntity(Entity<XenoActiveCripplingStrikeComponent>.op_Implicit(ent), (MetaDataComponent)null);
			if (args.Attack is DisarmAttackEvent disarm)
			{
				args.Attack = new LightAttackEvent(disarm.Target, netAttacker, disarm.Coordinates);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoActiveCripplingStrikeComponent> activeQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoActiveCripplingStrikeComponent>();
		EntityUid uid = default(EntityUid);
		XenoActiveCripplingStrikeComponent active = default(XenoActiveCripplingStrikeComponent);
		while (activeQuery.MoveNext(ref uid, ref active))
		{
			if (time < active.ExpireAt)
			{
				continue;
			}
			if (active.NextSlashBuffed)
			{
				_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(active.ExpireText)), uid, uid, PopupType.SmallCaution);
			}
			else
			{
				LocId? deactivateText = active.DeactivateText;
				if (deactivateText.HasValue)
				{
					LocId deactivateText2 = deactivateText.GetValueOrDefault();
					_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(deactivateText2)), uid, uid, PopupType.MediumCaution);
				}
			}
			((EntitySystem)this).RemCompDeferred<XenoActiveCripplingStrikeComponent>(uid);
		}
	}
}
