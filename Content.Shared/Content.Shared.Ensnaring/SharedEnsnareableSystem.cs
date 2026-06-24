using System;
using Content.Shared.Alert;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Strip.Components;
using Content.Shared.Throwing;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Shared.Ensnaring;

public abstract class SharedEnsnareableSystem : EntitySystem
{
	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private MovementSpeedModifierSystem _speedModifier;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	protected SharedContainerSystem Container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	private SharedStaminaSystem _stamina;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EnsnareableComponent, ComponentInit>((EntityEventRefHandler<EnsnareableComponent, ComponentInit>)OnEnsnareInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnareableComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<EnsnareableComponent, RefreshMovementSpeedModifiersEvent>)MovementSpeedModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnareableComponent, EnsnareEvent>((ComponentEventHandler<EnsnareableComponent, EnsnareEvent>)OnEnsnare, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnareableComponent, EnsnareRemoveEvent>((ComponentEventHandler<EnsnareableComponent, EnsnareRemoveEvent>)OnEnsnareRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnareableComponent, EnsnaredChangedEvent>((ComponentEventHandler<EnsnareableComponent, EnsnaredChangedEvent>)OnEnsnareChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnareableComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<EnsnareableComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnareableComponent, StrippingEnsnareButtonPressed>((ComponentEventHandler<EnsnareableComponent, StrippingEnsnareButtonPressed>)OnStripEnsnareMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnareableComponent, RemoveEnsnareAlertEvent>((EntityEventRefHandler<EnsnareableComponent, RemoveEnsnareAlertEvent>)OnRemoveEnsnareAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnareableComponent, EnsnareableDoAfterEvent>((ComponentEventHandler<EnsnareableComponent, EnsnareableDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnaringComponent, ComponentRemove>((ComponentEventHandler<EnsnaringComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnaringComponent, StepTriggerAttemptEvent>((ComponentEventRefHandler<EnsnaringComponent, StepTriggerAttemptEvent>)AttemptStepTrigger, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnaringComponent, StepTriggeredOffEvent>((ComponentEventRefHandler<EnsnaringComponent, StepTriggeredOffEvent>)OnStepTrigger, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnsnaringComponent, ThrowDoHitEvent>((ComponentEventHandler<EnsnaringComponent, ThrowDoHitEvent>)OnThrowHit, (Type[])null, (Type[])null);
	}

	protected virtual void OnEnsnareInit(Entity<EnsnareableComponent> ent, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Container = Container.EnsureContainer<Container>(ent.Owner, "ensnare", (ContainerManagerComponent)null);
	}

	private void OnHandleState(EntityUid uid, EnsnareableComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseLocalEvent<EnsnaredChangedEvent>(uid, new EnsnaredChangedEvent(component.IsEnsnared), false);
	}

	private void OnDoAfter(EntityUid uid, EnsnareableComponent component, DoAfterEvent args)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		EnsnaringComponent ensnaring = default(EnsnaringComponent);
		if (!args.Args.Target.HasValue || ((HandledEntityEventArgs)args).Handled || !((EntitySystem)this).TryComp<EnsnaringComponent>(args.Args.Used, ref ensnaring))
		{
			return;
		}
		EntityUid user;
		EntityUid? target;
		if (args.Cancelled || !Container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(args.Args.Used.Value), (BaseContainer)(object)component.Container, true, false, (EntityCoordinates?)null, (Angle?)null))
		{
			user = args.User;
			target = args.Target;
			if (target.HasValue && user == target.GetValueOrDefault())
			{
				Popup.PopupPredicted(base.Loc.GetString("ensnare-component-try-free-fail", (ValueTuple<string, object>)("ensnare", args.Args.Used)), uid, args.User, PopupType.MediumCaution);
			}
			else if (args.Target.HasValue)
			{
				Popup.PopupPredicted(base.Loc.GetString("ensnare-component-try-free-fail-other", (ValueTuple<string, object>)("ensnare", args.Args.Used), (ValueTuple<string, object>)("user", args.Target)), uid, args.User, PopupType.MediumCaution);
			}
			return;
		}
		component.IsEnsnared = ((BaseContainer)component.Container).ContainedEntities.Count > 0;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		ensnaring.Ensnared = null;
		_hands.PickupOrDrop(args.Args.User, args.Args.Used.Value);
		user = args.User;
		target = args.Target;
		if (target.HasValue && user == target.GetValueOrDefault())
		{
			Popup.PopupPredicted(base.Loc.GetString("ensnare-component-try-free-complete", (ValueTuple<string, object>)("ensnare", args.Args.Used)), uid, args.User, PopupType.Medium);
		}
		else if (args.Target.HasValue)
		{
			Popup.PopupPredicted(base.Loc.GetString("ensnare-component-try-free-complete-other", (ValueTuple<string, object>)("ensnare", args.Args.Used), (ValueTuple<string, object>)("user", args.Target)), uid, args.User, PopupType.Medium);
		}
		UpdateAlert(args.Args.Target.Value, component);
		EnsnareRemoveEvent ev = new EnsnareRemoveEvent(ensnaring.WalkSpeed, ensnaring.SprintSpeed);
		((EntitySystem)this).RaiseLocalEvent<EnsnareRemoveEvent>(uid, ev, false);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private void OnEnsnare(EntityUid uid, EnsnareableComponent component, EnsnareEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		component.WalkSpeed *= args.WalkSpeed;
		component.SprintSpeed *= args.SprintSpeed;
		_speedModifier.RefreshMovementSpeedModifiers(uid);
		EnsnaredChangedEvent ev = new EnsnaredChangedEvent(component.IsEnsnared);
		((EntitySystem)this).RaiseLocalEvent<EnsnaredChangedEvent>(uid, ev, false);
	}

	private void OnEnsnareRemove(EntityUid uid, EnsnareableComponent component, EnsnareRemoveEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		component.WalkSpeed /= args.WalkSpeed;
		component.SprintSpeed /= args.SprintSpeed;
		_speedModifier.RefreshMovementSpeedModifiers(uid);
		EnsnaredChangedEvent ev = new EnsnaredChangedEvent(component.IsEnsnared);
		((EntitySystem)this).RaiseLocalEvent<EnsnaredChangedEvent>(uid, ev, false);
	}

	private void OnEnsnareChange(EntityUid uid, EnsnareableComponent component, EnsnaredChangedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(uid, component);
	}

	private void UpdateAppearance(EntityUid uid, EnsnareableComponent component, AppearanceComponent? appearance = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Appearance.SetData(uid, (Enum)EnsnareableVisuals.IsEnsnared, (object)component.IsEnsnared, appearance);
	}

	private void MovementSpeedModify(EntityUid uid, EnsnareableComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		if (component.IsEnsnared)
		{
			args.ModifySpeed(component.WalkSpeed, component.SprintSpeed);
		}
	}

	public void TryFree(EntityUid target, EntityUid user, EntityUid ensnare, EnsnaringComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<EnsnareableComponent>(target))
		{
			return;
		}
		float freeTime = ((user == target) ? component.BreakoutTime : component.FreeTime);
		bool breakOnMove = !component.CanMoveBreakout;
		DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, freeTime, new EnsnareableDoAfterEvent(), target, target, ensnare)
		{
			BreakOnMove = breakOnMove,
			BreakOnDamage = false,
			NeedHand = true,
			BreakOnDropItem = false
		};
		if (_doAfter.TryStartDoAfter(doAfterEventArgs))
		{
			if (user == target)
			{
				Popup.PopupPredicted(base.Loc.GetString("ensnare-component-try-free", (ValueTuple<string, object>)("ensnare", ensnare)), target, target);
			}
			else
			{
				Popup.PopupPredicted(base.Loc.GetString("ensnare-component-try-free-other", (ValueTuple<string, object>)("ensnare", ensnare), (ValueTuple<string, object>)("user", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), user, user);
			}
		}
	}

	private void OnStripEnsnareMessage(EntityUid uid, EnsnareableComponent component, StrippingEnsnareButtonPressed args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EnsnaringComponent ensnaring = default(EnsnaringComponent);
		foreach (EntityUid entity in ((BaseContainer)component.Container).ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<EnsnaringComponent>(entity, ref ensnaring))
			{
				TryFree(uid, ((BaseBoundUserInterfaceEvent)args).Actor, entity, ensnaring);
				break;
			}
		}
	}

	private void OnRemoveEnsnareAlert(Entity<EnsnareableComponent> ent, ref RemoveEnsnareAlertEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EnsnaringComponent ensnaringComponent = default(EnsnaringComponent);
		foreach (EntityUid ensnare in ((BaseContainer)ent.Comp.Container).ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<EnsnaringComponent>(ensnare, ref ensnaringComponent))
			{
				TryFree(Entity<EnsnareableComponent>.op_Implicit(ent), Entity<EnsnareableComponent>.op_Implicit(ent), ensnare, ensnaringComponent);
				((HandledEntityEventArgs)args).Handled = true;
				break;
			}
		}
	}

	private void OnComponentRemove(EntityUid uid, EnsnaringComponent component, ComponentRemove args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		EnsnareableComponent ensnared = default(EnsnareableComponent);
		if (((EntitySystem)this).TryComp<EnsnareableComponent>(component.Ensnared, ref ensnared) && ensnared.IsEnsnared)
		{
			ForceFree(uid, component);
		}
	}

	private void AttemptStepTrigger(EntityUid uid, EnsnaringComponent component, ref StepTriggerAttemptEvent args)
	{
		args.Continue = true;
	}

	private void OnStepTrigger(EntityUid uid, EnsnaringComponent component, ref StepTriggeredOffEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TryEnsnare(args.Tripper, uid, component);
	}

	private void OnThrowHit(EntityUid uid, EnsnaringComponent component, ThrowDoHitEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (component.CanThrowTrigger && TryEnsnare(args.Target, uid, component))
		{
			_audio.PlayPvs(component.EnsnareSound, uid, (AudioParams?)null);
		}
	}

	public bool TryEnsnare(EntityUid target, EntityUid ensnare, EnsnaringComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		EnsnareableComponent ensnareable = default(EnsnareableComponent);
		if (!((EntitySystem)this).TryComp<EnsnareableComponent>(target, ref ensnareable))
		{
			return false;
		}
		if ((float)((BaseContainer)ensnareable.Container).ContainedEntities.Count >= component.MaxEnsnares)
		{
			return false;
		}
		Container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(ensnare), (BaseContainer)(object)ensnareable.Container, (TransformComponent)null, false);
		StaminaComponent stamina = default(StaminaComponent);
		if (((EntitySystem)this).TryComp<StaminaComponent>(target, ref stamina))
		{
			_stamina.TakeStaminaDamage(target, component.StaminaDamage, with: ensnare, component: stamina);
		}
		component.Ensnared = target;
		ensnareable.IsEnsnared = true;
		((EntitySystem)this).Dirty(target, (IComponent)(object)ensnareable, (MetaDataComponent)null);
		UpdateAlert(target, ensnareable);
		EnsnareEvent ev = new EnsnareEvent(component.WalkSpeed, component.SprintSpeed);
		((EntitySystem)this).RaiseLocalEvent<EnsnareEvent>(target, ev, false);
		return true;
	}

	public void ForceFree(EntityUid ensnare, EnsnaringComponent component)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		EnsnareableComponent ensnareable = default(EnsnareableComponent);
		if (component.Ensnared.HasValue && ((EntitySystem)this).TryComp<EnsnareableComponent>(component.Ensnared, ref ensnareable))
		{
			EntityUid target = component.Ensnared.Value;
			Container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(ensnare), (BaseContainer)(object)ensnareable.Container, true, true, (EntityCoordinates?)null, (Angle?)null);
			ensnareable.IsEnsnared = ((BaseContainer)ensnareable.Container).ContainedEntities.Count > 0;
			((EntitySystem)this).Dirty(component.Ensnared.Value, (IComponent)(object)ensnareable, (MetaDataComponent)null);
			component.Ensnared = null;
			UpdateAlert(target, ensnareable);
			EnsnareRemoveEvent ev = new EnsnareRemoveEvent(component.WalkSpeed, component.SprintSpeed);
			((EntitySystem)this).RaiseLocalEvent<EnsnareRemoveEvent>(ensnare, ev, false);
		}
	}

	public void UpdateAlert(EntityUid target, EnsnareableComponent component)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!component.IsEnsnared)
		{
			_alerts.ClearAlert(target, component.EnsnaredAlert);
		}
		else
		{
			_alerts.ShowAlert(target, component.EnsnaredAlert);
		}
	}
}
