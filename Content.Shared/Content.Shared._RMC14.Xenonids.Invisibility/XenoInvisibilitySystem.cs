using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Invisibility;

public sealed class XenoInvisibilitySystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	private readonly HashSet<EntityUid> _contacts = new HashSet<EntityUid>();

	private EntityQuery<MarineComponent> _marineQuery;

	private EntityQuery<MobCollisionComponent> _mobCollisionQuery;

	private EntityQuery<XenoComponent> _xenoQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_marineQuery = ((EntitySystem)this).GetEntityQuery<MarineComponent>();
		_mobCollisionQuery = ((EntitySystem)this).GetEntityQuery<MobCollisionComponent>();
		_xenoQuery = ((EntitySystem)this).GetEntityQuery<XenoComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoTurnInvisibleComponent, XenoTurnInvisibleActionEvent>((EntityEventRefHandler<XenoTurnInvisibleComponent, XenoTurnInvisibleActionEvent>)OnXenoTurnInvisibleAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveInvisibleComponent, ComponentRemove>((EntityEventRefHandler<XenoActiveInvisibleComponent, ComponentRemove>)OnXenoActiveInvisibleRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveInvisibleComponent, MeleeHitEvent>((EntityEventRefHandler<XenoActiveInvisibleComponent, MeleeHitEvent>)OnXenoActiveInvisibleMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveInvisibleComponent, DoAfterAttemptEvent<XenoDevourDoAfterEvent>>((EntityEventRefHandler<XenoActiveInvisibleComponent, DoAfterAttemptEvent<XenoDevourDoAfterEvent>>)OnXenoDevourDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveInvisibleComponent, XenoLeapHitEvent>((EntityEventRefHandler<XenoActiveInvisibleComponent, XenoLeapHitEvent>)OnXenoActiveInvisibleLeapHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoActiveInvisibleComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoActiveInvisibleComponent, RefreshMovementSpeedModifiersEvent>)OnXenoActiveInvisibleRefreshSpeed, (Type[])null, (Type[])null);
	}

	private void OnXenoTurnInvisibleAction(Entity<XenoTurnInvisibleComponent> xeno, ref XenoTurnInvisibleActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
		{
			XenoActiveInvisibleComponent invis = default(XenoActiveInvisibleComponent);
			if (((EntitySystem)this).TryComp<XenoActiveInvisibleComponent>(Entity<XenoTurnInvisibleComponent>.op_Implicit(xeno), ref invis))
			{
				TimeSpan refundedCooldown = GetRefundedCooldown(xeno, invis, xeno.Comp.ManualRefundMultiplier);
				RemoveInvisibility(Entity<XenoActiveInvisibleComponent>.op_Implicit((Entity<XenoTurnInvisibleComponent>.op_Implicit(xeno), invis)), refundedCooldown);
				return;
			}
			XenoActiveInvisibleComponent active = ((EntitySystem)this).EnsureComp<XenoActiveInvisibleComponent>(Entity<XenoTurnInvisibleComponent>.op_Implicit(xeno));
			active.ExpiresAt = _timing.CurTime + xeno.Comp.Duration;
			active.FullCooldown = xeno.Comp.FullCooldown;
			active.SpeedMultiplier = xeno.Comp.SpeedMultiplier;
			StartCooldown(Entity<XenoActiveInvisibleComponent>.op_Implicit((Entity<XenoTurnInvisibleComponent>.op_Implicit(xeno), active)), xeno.Comp.ToggleLockoutTime, toggledStatus: true);
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoTurnInvisibleComponent>.op_Implicit(xeno));
		}
	}

	private void OnXenoActiveInvisibleRemove(Entity<XenoActiveInvisibleComponent> xeno, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno), (MetaDataComponent)null))
		{
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno));
		}
	}

	private void OnXenoActiveInvisibleMeleeHit(Entity<XenoActiveInvisibleComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit || args.HitEntities.Count == 0)
		{
			return;
		}
		using IEnumerator<EntityUid> enumerator = args.HitEntities.GetEnumerator();
		if (enumerator.MoveNext())
		{
			EntityUid entity = enumerator.Current;
			if (_xeno.CanAbilityAttackTarget(Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno), entity))
			{
				RemoveInvisibility(xeno, xeno.Comp.FullCooldown);
			}
		}
	}

	private void OnXenoDevourDoAfterAttempt(Entity<XenoActiveInvisibleComponent> xeno, ref DoAfterAttemptEvent<XenoDevourDoAfterEvent> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		XenoTurnInvisibleComponent turnInvis = default(XenoTurnInvisibleComponent);
		if (!((EntitySystem)this).TryComp<XenoTurnInvisibleComponent>(Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno), ref turnInvis))
		{
			RemoveInvisibility(xeno, xeno.Comp.FullCooldown);
			return;
		}
		TimeSpan devourCooldown = GetRefundedCooldown(Entity<XenoTurnInvisibleComponent>.op_Implicit((Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno), turnInvis)), xeno.Comp, turnInvis.RevealedRefundMultiplier);
		RemoveInvisibility(xeno, devourCooldown);
	}

	private void OnXenoActiveInvisibleLeapHit(Entity<XenoActiveInvisibleComponent> xeno, ref XenoLeapHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		RemoveInvisibility(xeno, xeno.Comp.FullCooldown);
	}

	private void OnXenoActiveInvisibleRefreshSpeed(Entity<XenoActiveInvisibleComponent> xeno, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		float multiplier = xeno.Comp.SpeedMultiplier.Float();
		args.ModifySpeed(multiplier, multiplier);
	}

	private TimeSpan GetRefundedCooldown(Entity<XenoTurnInvisibleComponent> xeno, XenoActiveInvisibleComponent activeInvis, float refundMultiplier)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		double timeRemaining = (activeInvis.ExpiresAt - _timing.CurTime) / xeno.Comp.Duration;
		return xeno.Comp.FullCooldown - timeRemaining * (double)refundMultiplier * xeno.Comp.FullCooldown;
	}

	private void StartCooldown(Entity<XenoActiveInvisibleComponent> xeno, TimeSpan cooldownTime, bool toggledStatus)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> item in _rmcActions.GetActionsWithEvent<XenoTurnInvisibleActionEvent>(Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno)))
		{
			Entity<ActionComponent> actionEnt = item.AsNullable();
			_actions.SetCooldown(actionEnt, cooldownTime);
			_actions.SetToggled(actionEnt, toggledStatus);
		}
	}

	private void RemoveInvisibility(Entity<XenoActiveInvisibleComponent> xeno, TimeSpan cooldownTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<XenoActiveInvisibleComponent>(Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno));
		StartCooldown(xeno, cooldownTime, toggledStatus: false);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno));
		if (!xeno.Comp.DidPopup)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-invisibility-expire"), Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno), Entity<XenoActiveInvisibleComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			xeno.Comp.DidPopup = true;
			((EntitySystem)this).Dirty<XenoActiveInvisibleComponent>(xeno, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoActiveInvisibleComponent> activeQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoActiveInvisibleComponent>();
		EntityUid uid = default(EntityUid);
		XenoActiveInvisibleComponent active = default(XenoActiveInvisibleComponent);
		MobCollisionComponent collision = default(MobCollisionComponent);
		while (activeQuery.MoveNext(ref uid, ref active))
		{
			if (active.ExpiresAt <= time)
			{
				RemoveInvisibility(Entity<XenoActiveInvisibleComponent>.op_Implicit((uid, active)), active.FullCooldown);
			}
			else
			{
				if (!_mobCollisionQuery.TryComp(uid, ref collision) || !collision.Colliding)
				{
					continue;
				}
				_contacts.Clear();
				_physics.GetContactingEntities(Entity<PhysicsComponent>.op_Implicit(uid), _contacts, false);
				bool collidingEnemy = false;
				foreach (EntityUid contact in _contacts)
				{
					if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(uid), Entity<HiveMemberComponent>.op_Implicit(contact)) && (_marineQuery.HasComp(uid) || _xenoQuery.HasComp(contact)))
					{
						collidingEnemy = true;
					}
				}
				if (!collidingEnemy)
				{
					continue;
				}
				if (!active.DidPopup)
				{
					active.DidPopup = true;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)active, (MetaDataComponent)null);
					if (_net.IsServer)
					{
						_popup.PopupEntity(base.Loc.GetString("rmc-xeno-invisibility-expire-bump"), uid, uid, PopupType.MediumCaution);
					}
				}
				RemoveInvisibility(Entity<XenoActiveInvisibleComponent>.op_Implicit((uid, active)), active.FullCooldown);
			}
		}
	}
}
