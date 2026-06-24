using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Dodge;

public sealed class XenoDodgeSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private XenoPlasmaSystem _plasma;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private MovementSpeedModifierSystem _speed;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private XenoSystem _xeno;

	private readonly HashSet<Entity<MobStateComponent>> _crowd = new HashSet<Entity<MobStateComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoDodgeComponent, XenoDodgeActionEvent>((EntityEventRefHandler<XenoDodgeComponent, XenoDodgeActionEvent>)OnXenoActionDodge, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveDodgeComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoActiveDodgeComponent, RefreshMovementSpeedModifiersEvent>)OnActiveDodgeRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveDodgeComponent, ComponentRemove>((EntityEventRefHandler<XenoActiveDodgeComponent, ComponentRemove>)OnActiveDodgeRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveDodgeComponent, AttemptMobCollideEvent>((EntityEventRefHandler<XenoActiveDodgeComponent, AttemptMobCollideEvent>)OnActiveDodgeAttemptMobCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveDodgeComponent, AttemptMobTargetCollideEvent>((EntityEventRefHandler<XenoActiveDodgeComponent, AttemptMobTargetCollideEvent>)OnActiveDodgeAttemptMobTargetCollide, (Type[])null, (Type[])null);
	}

	private void OnXenoActionDodge(Entity<XenoDodgeComponent> xeno, ref XenoDodgeActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_plasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_net.IsClient)
		{
			return;
		}
		((EntitySystem)this).EnsureComp<XenoActiveDodgeComponent>(Entity<XenoDodgeComponent>.op_Implicit(xeno)).ExpiresAt = _timing.CurTime + xeno.Comp.Duration;
		_speed.RefreshMovementSpeedModifiers(Entity<XenoDodgeComponent>.op_Implicit(xeno));
		_popup.PopupEntity(base.Loc.GetString("rmc-xeno-dodge-self"), Entity<XenoDodgeComponent>.op_Implicit(xeno), Entity<XenoDodgeComponent>.op_Implicit(xeno), PopupType.Medium);
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoDodgeActionEvent>(Entity<XenoDodgeComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), toggled: true);
		}
	}

	private void OnActiveDodgeRefresh(Entity<XenoActiveDodgeComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		float modifier = (1.0 + xeno.Comp.SpeedMult + (xeno.Comp.InCrowd ? xeno.Comp.CrowdSpeedAddMult : ((FixedPoint2)0))).Float();
		args.ModifySpeed(modifier, modifier);
	}

	private void OnActiveDodgeRemove(Entity<XenoActiveDodgeComponent> xeno, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).TerminatingOrDeleted(Entity<XenoActiveDodgeComponent>.op_Implicit(xeno), (MetaDataComponent)null))
		{
			return;
		}
		_speed.RefreshMovementSpeedModifiers(Entity<XenoActiveDodgeComponent>.op_Implicit(xeno));
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoDodgeActionEvent>(Entity<XenoActiveDodgeComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), toggled: false);
		}
	}

	private void OnActiveDodgeAttemptMobCollide(Entity<XenoActiveDodgeComponent> ent, ref AttemptMobCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void OnActiveDodgeAttemptMobTargetCollide(Entity<XenoActiveDodgeComponent> ent, ref AttemptMobTargetCollideEvent args)
	{
		args.Cancelled = true;
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoActiveDodgeComponent> dodgeQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoActiveDodgeComponent>();
		EntityUid uid = default(EntityUid);
		XenoActiveDodgeComponent speed = default(XenoActiveDodgeComponent);
		while (dodgeQuery.MoveNext(ref uid, ref speed))
		{
			if (speed.ExpiresAt < time)
			{
				((EntitySystem)this).RemCompDeferred<XenoActiveDodgeComponent>(uid);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-dodge-end"), uid, uid, PopupType.MediumCaution);
				continue;
			}
			_crowd.Clear();
			_lookup.GetEntitiesInRange<MobStateComponent>(((EntitySystem)this).Transform(uid).Coordinates, speed.CrowdRange, _crowd, (LookupFlags)110);
			bool crowd = false;
			foreach (Entity<MobStateComponent> mob in _crowd)
			{
				if (_xeno.CanAbilityAttackTarget(uid, Entity<MobStateComponent>.op_Implicit(mob)) && !_standing.IsDown(Entity<MobStateComponent>.op_Implicit(mob)))
				{
					crowd = true;
					break;
				}
			}
			if (crowd != speed.InCrowd)
			{
				speed.InCrowd = crowd;
				_speed.RefreshMovementSpeedModifiers(uid);
			}
		}
	}
}
