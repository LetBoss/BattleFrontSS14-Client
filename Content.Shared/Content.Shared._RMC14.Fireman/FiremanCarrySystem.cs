using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Sprite;
using Content.Shared.ActionBlocker;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs;
using Content.Shared.MouseRotator;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.Strip;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Fireman;

public sealed class FiremanCarrySystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private SharedRMCSpriteSystem _rmcSprite;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly List<(EntityUid Target, EntityUid Carrier)> _toReparent = new List<(EntityUid, EntityUid)>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, CanDragEvent>((EntityEventRefHandler<FiremanCarriableComponent, CanDragEvent>)OnCarriableCanDrag, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, DragDropDraggedEvent>((EntityEventRefHandler<FiremanCarriableComponent, DragDropDraggedEvent>)OnCarriableDragDropDragged, new Type[1] { typeof(SharedStrippableSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, DoAfterAttemptEvent<FiremanCarryDoAfterEvent>>((EntityEventRefHandler<FiremanCarriableComponent, DoAfterAttemptEvent<FiremanCarryDoAfterEvent>>)OnCarriableFiremanCarryDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, FiremanCarryDoAfterEvent>((EntityEventRefHandler<FiremanCarriableComponent, FiremanCarryDoAfterEvent>)OnCarriableFiremanCarryDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, StandAttemptEvent>((EntityEventRefHandler<FiremanCarriableComponent, StandAttemptEvent>)OnCarriableStandAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, UpdateCanMoveEvent>((EntityEventRefHandler<FiremanCarriableComponent, UpdateCanMoveEvent>)OnCarriableCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, MoveInputEvent>((EntityEventRefHandler<FiremanCarriableComponent, MoveInputEvent>)OnCarriableMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, BreakFiremanCarryDoAfterEvent>((EntityEventRefHandler<FiremanCarriableComponent, BreakFiremanCarryDoAfterEvent>)OnCarriableBreakCarryDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, PullStartedMessage>((EntityEventRefHandler<FiremanCarriableComponent, PullStartedMessage>)OnCarriablePullStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, PullStoppedMessage>((EntityEventRefHandler<FiremanCarriableComponent, PullStoppedMessage>)OnCarriablePullStopped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FiremanCarriableComponent, PullAttemptEvent>((EntityEventRefHandler<FiremanCarriableComponent, PullAttemptEvent>)OnCarriablePullAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CanFiremanCarryComponent, PullStartedMessage>((EntityEventRefHandler<CanFiremanCarryComponent, PullStartedMessage>)OnCarrierPullStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CanFiremanCarryComponent, PullStoppedMessage>((EntityEventRefHandler<CanFiremanCarryComponent, PullStoppedMessage>)OnCarrierPullStopped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CanFiremanCarryComponent, PullSlowdownAttemptEvent>((EntityEventRefHandler<CanFiremanCarryComponent, PullSlowdownAttemptEvent>)OnCarrierPullSlowdownAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CanFiremanCarryComponent, MobStateChangedEvent>((EntityEventRefHandler<CanFiremanCarryComponent, MobStateChangedEvent>)OnCarrierMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CanFiremanCarryComponent, RMCPullToggleEvent>((EntityEventRefHandler<CanFiremanCarryComponent, RMCPullToggleEvent>)OnCarrierPullToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BeingFiremanCarriedComponent, PreventCollideEvent>((EntityEventRefHandler<BeingFiremanCarriedComponent, PreventCollideEvent>)OnBeingCarriedPreventCollide, (Type[])null, (Type[])null);
	}

	private void OnCarriableCanDrag(Entity<FiremanCarriableComponent> ent, ref CanDragEvent args)
	{
		args.Handled = true;
	}

	private void OnCarriableDragDropDragged(Entity<FiremanCarriableComponent> ent, ref DragDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		CanFiremanCarryComponent carrier = default(CanFiremanCarryComponent);
		if (!((EntitySystem)this).TryComp<CanFiremanCarryComponent>(user, ref carrier) || args.Target != user || !_rmcPulling.IsPulling(Entity<PullerComponent>.op_Implicit(user), Entity<PullableComponent>.op_Implicit(ent.Owner)))
		{
			return;
		}
		args.Handled = true;
		if (!_skills.HasSkill(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill, 1))
		{
			_popup.PopupClient(base.Loc.GetString("You aren't trained to carry people!"), Entity<FiremanCarriableComponent>.op_Implicit(ent), user, PopupType.MediumCaution);
			return;
		}
		if (!carrier.AggressiveGrab)
		{
			_popup.PopupClient(base.Loc.GetString("You need to grab them aggressively first!"), Entity<FiremanCarriableComponent>.op_Implicit(ent), user, PopupType.MediumCaution);
			return;
		}
		TimeSpan delay = ent.Comp.Delay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(user), ent.Comp.Skill);
		FiremanCarryDoAfterEvent ev = new FiremanCarryDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<FiremanCarriableComponent>.op_Implicit(ent), Entity<FiremanCarriableComponent>.op_Implicit(ent), user)
		{
			BreakOnMove = true,
			AttemptFrequency = AttemptFrequency.EveryTick,
			ForceVisible = true
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			IdentityEntity target = Identity.Name(Entity<FiremanCarriableComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager, args.User);
			_popup.PopupClient(base.Loc.GetString($"You start loading {target} onto your back."), Entity<FiremanCarriableComponent>.op_Implicit(ent), user, PopupType.Medium);
		}
	}

	private void OnCarriableFiremanCarryDoAfterAttempt(Entity<FiremanCarriableComponent> ent, ref DoAfterAttemptEvent<FiremanCarryDoAfterEvent> args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!_rmcPulling.IsPulling(Entity<PullerComponent>.op_Implicit(args.DoAfter.Args.User), Entity<PullableComponent>.op_Implicit(ent.Owner)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnCarriableFiremanCarryDoAfter(Entity<FiremanCarriableComponent> ent, ref FiremanCarryDoAfterEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		CanFiremanCarryComponent carrier = default(CanFiremanCarryComponent);
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<CanFiremanCarryComponent>(user, ref carrier) && !_transform.IsParentOf(((EntitySystem)this).Transform(ent.Owner), user))
		{
			ent.Comp.BeingCarried = true;
			((EntitySystem)this).Dirty<FiremanCarriableComponent>(ent, (MetaDataComponent)null);
			((EntitySystem)this).EnsureComp<BeingFiremanCarriedComponent>(Entity<FiremanCarriableComponent>.op_Implicit(ent));
			carrier.Carrying = Entity<FiremanCarriableComponent>.op_Implicit(ent);
			((EntitySystem)this).Dirty(user, (IComponent)(object)carrier, (MetaDataComponent)null);
			if (!_timing.ApplyingState && !((EntitySystem)this).HasComp<MouseRotatorComponent>(user))
			{
				((EntitySystem)this).RemCompDeferred<NoRotateOnMoveComponent>(user);
			}
			((HandledEntityEventArgs)args).Handled = true;
			_transform.SetParent(Entity<FiremanCarriableComponent>.op_Implicit(ent), user);
			_transform.SetLocalPosition(Entity<FiremanCarriableComponent>.op_Implicit(ent), Vector2.Zero, (TransformComponent)null);
			_standing.Down(Entity<FiremanCarriableComponent>.op_Implicit(ent), playSound: true, dropHeldItems: true, force: false, changeCollision: true);
			_movementSpeed.RefreshMovementSpeedModifiers(user);
			_rmcSprite.SetRenderOrder(user, 1);
		}
	}

	private void OnCarriableStandAttempt(Entity<FiremanCarriableComponent> ent, ref StandAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BeingCarried || IsBeingAggressivelyGrabbed(Entity<FiremanCarriableComponent>.op_Implicit(ent)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnCarriableCanMove(Entity<FiremanCarriableComponent> ent, ref UpdateCanMoveEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (IsBeingAggressivelyGrabbed(Entity<FiremanCarriableComponent>.op_Implicit(ent)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnCarriableMoveInput(Entity<FiremanCarriableComponent> ent, ref MoveInputEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		if (!args.HasDirectionalMovement || ent.Comp.BreakingFree || (!ent.Comp.BeingCarried && !IsBeingAggressivelyGrabbed(Entity<FiremanCarriableComponent>.op_Implicit(ent))))
		{
			return;
		}
		BreakFiremanCarryDoAfterEvent ev = new BreakFiremanCarryDoAfterEvent();
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<FiremanCarriableComponent>.op_Implicit(ent), ent.Comp.Delay, ev, Entity<FiremanCarriableComponent>.op_Implicit(ent));
		if (!_doAfter.TryStartDoAfter(doAfter))
		{
			return;
		}
		ent.Comp.BreakingFree = true;
		if (!_rmcPulling.IsBeingPulled(Entity<PullableComponent>.op_Implicit(ent.Owner), out var puller))
		{
			return;
		}
		string selfMsg = base.Loc.GetString("rmc-pull-break-start-self", (ValueTuple<string, object>)("puller", puller));
		_popup.PopupClient(selfMsg, Entity<FiremanCarriableComponent>.op_Implicit(ent), Entity<FiremanCarriableComponent>.op_Implicit(ent), PopupType.MediumCaution);
		foreach (ICommonSession recipient2 in Filter.PvsExcept(Entity<FiremanCarriableComponent>.op_Implicit(ent), 2f, (IEntityManager)(object)base.EntityManager).Recipients)
		{
			EntityUid? attachedEntity = recipient2.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid recipient = attachedEntity.GetValueOrDefault();
				IdentityEntity pullerName = Identity.Name(puller, (IEntityManager)(object)base.EntityManager, recipient);
				IdentityEntity pulledName = Identity.Name(Entity<FiremanCarriableComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager, recipient);
				string msg = base.Loc.GetString("rmc-pull-break-start-others", (ValueTuple<string, object>)("puller", pullerName), (ValueTuple<string, object>)("pulled", pulledName));
				_popup.PopupEntity(msg, Entity<FiremanCarriableComponent>.op_Implicit(ent), recipient, PopupType.MediumCaution);
			}
		}
	}

	private void OnCarriableBreakCarryDoAfter(Entity<FiremanCarriableComponent> ent, ref BreakFiremanCarryDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.BreakingFree = false;
		((EntitySystem)this).Dirty<FiremanCarriableComponent>(ent, (MetaDataComponent)null);
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		ent.Comp.BeingCarried = false;
		((EntitySystem)this).RemCompDeferred<BeingFiremanCarriedComponent>(Entity<FiremanCarriableComponent>.op_Implicit(ent));
		if (!_rmcPulling.IsBeingPulled(Entity<PullableComponent>.op_Implicit(ent.Owner), out var puller))
		{
			return;
		}
		StopCarry(Entity<CanFiremanCarryComponent>.op_Implicit(puller), Entity<FiremanCarriableComponent>.op_Implicit((Entity<FiremanCarriableComponent>.op_Implicit(ent), Entity<FiremanCarriableComponent>.op_Implicit(ent))));
		string selfMsg = base.Loc.GetString("rmc-pull-break-finish-self", (ValueTuple<string, object>)("puller", puller));
		_popup.PopupClient(selfMsg, Entity<FiremanCarriableComponent>.op_Implicit(ent), Entity<FiremanCarriableComponent>.op_Implicit(ent), PopupType.MediumCaution);
		foreach (ICommonSession recipient2 in Filter.PvsExcept(Entity<FiremanCarriableComponent>.op_Implicit(ent), 2f, (IEntityManager)(object)base.EntityManager).Recipients)
		{
			EntityUid? attachedEntity = recipient2.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid recipient = attachedEntity.GetValueOrDefault();
				IdentityEntity pullerName = Identity.Name(puller, (IEntityManager)(object)base.EntityManager, recipient);
				IdentityEntity pulledName = Identity.Name(Entity<FiremanCarriableComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager, recipient);
				string msg = base.Loc.GetString("rmc-pull-break-finish-others", (ValueTuple<string, object>)("puller", pullerName), (ValueTuple<string, object>)("pulled", pulledName));
				_popup.PopupEntity(msg, Entity<FiremanCarriableComponent>.op_Implicit(ent), recipient, PopupType.MediumCaution);
			}
		}
	}

	private void OnCarriablePullStarted(Entity<FiremanCarriableComponent> ent, ref PullStartedMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		CanFiremanCarryComponent carrier = default(CanFiremanCarryComponent);
		if (!(args.PulledUid != ent.Owner) && _rmcPulling.IsBeingPulled(Entity<PullableComponent>.op_Implicit(ent.Owner), out var puller) && ((EntitySystem)this).TryComp<CanFiremanCarryComponent>(puller, ref carrier))
		{
			StopPull(Entity<CanFiremanCarryComponent>.op_Implicit((puller, carrier)), Entity<FiremanCarriableComponent>.op_Implicit(ent));
		}
	}

	private void OnCarriablePullStopped(Entity<FiremanCarriableComponent> ent, ref PullStoppedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!(ent.Owner != args.PulledUid))
		{
			_standing.Stand(Entity<FiremanCarriableComponent>.op_Implicit(ent));
		}
	}

	private void OnCarriablePullAttempt(Entity<FiremanCarriableComponent> ent, ref PullAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && (ent.Comp.BeingCarried || IsBeingAggressivelyGrabbed(Entity<FiremanCarriableComponent>.op_Implicit(ent))))
		{
			args.Cancelled = true;
		}
	}

	private void OnCarrierPullStarted(Entity<CanFiremanCarryComponent> ent, ref PullStartedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Owner == args.PullerUid)
		{
			StopPull(ent, args.PulledUid);
		}
	}

	private void OnCarrierPullStopped(Entity<CanFiremanCarryComponent> ent, ref PullStoppedMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Owner == args.PullerUid)
		{
			StopPull(ent, args.PulledUid);
		}
	}

	private void OnCarrierPullSlowdownAttempt(Entity<CanFiremanCarryComponent> ent, ref PullSlowdownAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? carrying = ent.Comp.Carrying;
		EntityUid target = args.Target;
		if (carrying.HasValue && carrying.GetValueOrDefault() == target)
		{
			args.Cancelled = true;
		}
	}

	private void OnCarrierMobStateChanged(Entity<CanFiremanCarryComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? carrying = ent.Comp.Carrying;
		if (carrying.HasValue)
		{
			EntityUid carrying2 = carrying.GetValueOrDefault();
			if (args.NewMobState == MobState.Dead)
			{
				StopCarry(Entity<CanFiremanCarryComponent>.op_Implicit((Entity<CanFiremanCarryComponent>.op_Implicit(ent), Entity<CanFiremanCarryComponent>.op_Implicit(ent))), Entity<FiremanCarriableComponent>.op_Implicit(carrying2));
			}
		}
	}

	private void OnCarrierPullToggle(Entity<CanFiremanCarryComponent> ent, ref RMCPullToggleEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		TimeSpan grabDelay = ent.Comp.AggressiveGrabDelay * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(ent.Owner), ent.Comp.Skill);
		if (ent.Comp.AggressiveGrab || _timing.CurTime < ent.Comp.PullTime + grabDelay)
		{
			return;
		}
		ent.Comp.AggressiveGrab = true;
		((EntitySystem)this).Dirty<CanFiremanCarryComponent>(ent, (MetaDataComponent)null);
		PullerComponent puller = default(PullerComponent);
		if (!((EntitySystem)this).TryComp<PullerComponent>(Entity<CanFiremanCarryComponent>.op_Implicit(ent), ref puller))
		{
			return;
		}
		EntityUid? pulling = puller.Pulling;
		if (!pulling.HasValue)
		{
			return;
		}
		EntityUid pulling2 = pulling.GetValueOrDefault();
		_actionBlocker.UpdateCanMove(pulling2);
		_standing.Down(pulling2, playSound: true, dropHeldItems: true, force: false, changeCollision: true);
		_rmcPulling.PlayPullEffect(Entity<CanFiremanCarryComponent>.op_Implicit(ent), pulling2);
		string selfMsg = base.Loc.GetString("rmc-pull-aggressive-self", (ValueTuple<string, object>)("pulled", pulling2));
		_popup.PopupClient(selfMsg, pulling2, Entity<CanFiremanCarryComponent>.op_Implicit(ent), PopupType.SmallCaution);
		foreach (ICommonSession recipient2 in Filter.PvsExcept(Entity<CanFiremanCarryComponent>.op_Implicit(ent), 2f, (IEntityManager)(object)base.EntityManager).Recipients)
		{
			pulling = recipient2.AttachedEntity;
			if (pulling.HasValue)
			{
				EntityUid recipient = pulling.GetValueOrDefault();
				IdentityEntity pullerName = Identity.Name(Entity<CanFiremanCarryComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager, recipient);
				IdentityEntity pulledName = Identity.Name(pulling2, (IEntityManager)(object)base.EntityManager, recipient);
				string msg = base.Loc.GetString("rmc-pull-aggressive-others", (ValueTuple<string, object>)("puller", pullerName), (ValueTuple<string, object>)("pulled", pulledName));
				_popup.PopupEntity(msg, Entity<CanFiremanCarryComponent>.op_Implicit(ent), recipient, PopupType.SmallCaution);
			}
		}
	}

	private void OnBeingCarriedPreventCollide(Entity<BeingFiremanCarriedComponent> ent, ref PreventCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void StopPull(Entity<CanFiremanCarryComponent> ent, EntityUid target)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			StopCarry(Entity<CanFiremanCarryComponent>.op_Implicit((Entity<CanFiremanCarryComponent>.op_Implicit(ent), Entity<CanFiremanCarryComponent>.op_Implicit(ent))), Entity<FiremanCarriableComponent>.op_Implicit(target));
			_actionBlocker.UpdateCanMove(target);
			ent.Comp.PullTime = _timing.CurTime;
			((EntitySystem)this).Dirty<CanFiremanCarryComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void StopCarry(Entity<CanFiremanCarryComponent?> user, Entity<FiremanCarriableComponent?>? targetNullable)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? carrying = null;
		if (((EntitySystem)this).Resolve<CanFiremanCarryComponent>(Entity<CanFiremanCarryComponent>.op_Implicit(user), ref user.Comp, false))
		{
			carrying = user.Comp.Carrying;
			user.Comp.Carrying = null;
			user.Comp.AggressiveGrab = false;
			((EntitySystem)this).Dirty<CanFiremanCarryComponent>(user, (MetaDataComponent)null);
			_rmcSprite.SetRenderOrder(Entity<CanFiremanCarryComponent>.op_Implicit(user), 0);
		}
		if (!targetNullable.HasValue)
		{
			return;
		}
		Entity<FiremanCarriableComponent> target = targetNullable.GetValueOrDefault();
		if (((EntitySystem)this).Resolve<FiremanCarriableComponent>(Entity<FiremanCarriableComponent>.op_Implicit(target), ref target.Comp, false))
		{
			target.Comp.BeingCarried = false;
			((EntitySystem)this).Dirty<FiremanCarriableComponent>(target, (MetaDataComponent)null);
			((EntitySystem)this).RemCompDeferred<BeingFiremanCarriedComponent>(Entity<FiremanCarriableComponent>.op_Implicit(target));
			EntityUid? val = carrying;
			EntityUid val2 = Entity<FiremanCarriableComponent>.op_Implicit(target);
			if (val.HasValue && val.GetValueOrDefault() == val2)
			{
				_toReparent.Add((Entity<FiremanCarriableComponent>.op_Implicit(target), Entity<CanFiremanCarryComponent>.op_Implicit(user)));
			}
		}
		_standing.Stand(Entity<FiremanCarriableComponent>.op_Implicit(target));
		_actionBlocker.UpdateCanMove(Entity<FiremanCarriableComponent>.op_Implicit(target));
	}

	private bool IsBeingAggressivelyGrabbed(EntityUid target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CanFiremanCarryComponent carrier = default(CanFiremanCarryComponent);
		if (_rmcPulling.IsBeingPulled(Entity<PullableComponent>.op_Implicit(target), out var user) && ((EntitySystem)this).TryComp<CanFiremanCarryComponent>(user, ref carrier))
		{
			return carrier.AggressiveGrab;
		}
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (var (target, carrier) in _toReparent)
			{
				if (((EntitySystem)this).TerminatingOrDeleted(target, (MetaDataComponent)null))
				{
					continue;
				}
				if (((EntitySystem)this).TerminatingOrDeleted(carrier, (MetaDataComponent)null))
				{
					EntityCoordinates coordinates = _transform.GetMoverCoordinates(target);
					if (!((EntitySystem)this).TerminatingOrDeleted(coordinates.EntityId, (MetaDataComponent)null))
					{
						_transform.SetCoordinates(target, coordinates);
					}
				}
				else
				{
					EntityUid parent = _transform.GetMoverCoordinates(target).EntityId;
					if (!(target == parent))
					{
						_transform.SetParent(target, parent);
					}
				}
			}
		}
		finally
		{
			_toReparent.Clear();
		}
	}
}
