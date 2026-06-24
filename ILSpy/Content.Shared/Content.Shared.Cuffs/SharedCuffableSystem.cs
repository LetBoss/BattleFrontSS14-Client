using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Buckle.Components;
using Content.Shared.CombatMode;
using Content.Shared.Cuffs.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Rejuvenate;
using Content.Shared.Stunnable;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared.Cuffs;

public abstract class SharedCuffableSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedVirtualItemSystem _virtualItem;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private UseDelaySystem _delay;

	[Dependency]
	private SharedCombatModeSystem _combatMode;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, HandCountChangedEvent>((EntityEventRefHandler<CuffableComponent, HandCountChangedEvent>)OnHandCountChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UncuffAttemptEvent>((EntityEventRefHandler<UncuffAttemptEvent>)OnUncuffAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<CuffableComponent, EntRemovedFromContainerMessage>)OnCuffsRemovedFromContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<CuffableComponent, EntInsertedIntoContainerMessage>)OnCuffsInsertedIntoContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, RejuvenateEvent>((ComponentEventHandler<CuffableComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, ComponentInit>((ComponentEventHandler<CuffableComponent, ComponentInit>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, AttemptStopPullingEvent>((ComponentEventHandler<CuffableComponent, AttemptStopPullingEvent>)HandleStopPull, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, RemoveCuffsAlertEvent>((EntityEventRefHandler<CuffableComponent, RemoveCuffsAlertEvent>)OnRemoveCuffsAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, UpdateCanMoveEvent>((ComponentEventHandler<CuffableComponent, UpdateCanMoveEvent>)HandleMoveAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, IsEquippingAttemptEvent>((ComponentEventHandler<CuffableComponent, IsEquippingAttemptEvent>)OnEquipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, IsUnequippingAttemptEvent>((ComponentEventHandler<CuffableComponent, IsUnequippingAttemptEvent>)OnUnequipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, BeingPulledAttemptEvent>((ComponentEventHandler<CuffableComponent, BeingPulledAttemptEvent>)OnBeingPulledAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, BuckleAttemptEvent>((EntityEventRefHandler<CuffableComponent, BuckleAttemptEvent>)OnBuckleAttemptEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, UnbuckleAttemptEvent>((EntityEventRefHandler<CuffableComponent, UnbuckleAttemptEvent>)OnUnbuckleAttemptEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, GetVerbsEvent<Verb>>((ComponentEventHandler<CuffableComponent, GetVerbsEvent<Verb>>)AddUncuffVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, UnCuffDoAfterEvent>((ComponentEventHandler<CuffableComponent, UnCuffDoAfterEvent>)OnCuffableDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, PullStartedMessage>((ComponentEventHandler<CuffableComponent, PullStartedMessage>)OnPull, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, PullStoppedMessage>((ComponentEventHandler<CuffableComponent, PullStoppedMessage>)OnPull, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, DropAttemptEvent>((ComponentEventHandler<CuffableComponent, DropAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, PickupAttemptEvent>((ComponentEventHandler<CuffableComponent, PickupAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, AttackAttemptEvent>((ComponentEventHandler<CuffableComponent, AttackAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, UseAttemptEvent>((ComponentEventHandler<CuffableComponent, UseAttemptEvent>)CheckAct, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CuffableComponent, InteractionAttemptEvent>((EntityEventRefHandler<CuffableComponent, InteractionAttemptEvent>)CheckInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandcuffComponent, AfterInteractEvent>((ComponentEventHandler<HandcuffComponent, AfterInteractEvent>)OnCuffAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandcuffComponent, MeleeHitEvent>((ComponentEventHandler<HandcuffComponent, MeleeHitEvent>)OnCuffMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandcuffComponent, AddCuffDoAfterEvent>((ComponentEventHandler<HandcuffComponent, AddCuffDoAfterEvent>)OnAddCuffDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandcuffComponent, VirtualItemDeletedEvent>((ComponentEventHandler<HandcuffComponent, VirtualItemDeletedEvent>)OnCuffVirtualItemDeleted, (Type[])null, (Type[])null);
	}

	private void CheckInteract(Entity<CuffableComponent> ent, ref InteractionAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.CanStillInteract)
		{
			args.Cancelled = true;
		}
	}

	private void OnUncuffAttempt(ref UncuffAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		if (!((EntitySystem)this).Exists(args.User) || ((EntitySystem)this).Deleted(args.User, (MetaDataComponent)null))
		{
			args.Cancelled = true;
			return;
		}
		if (args.User == args.Target)
		{
			CuffableComponent cuffable = default(CuffableComponent);
			if (!((EntitySystem)this).TryComp<CuffableComponent>(args.User, ref cuffable))
			{
				return;
			}
			cuffable.CanStillInteract = true;
			((EntitySystem)this).Dirty(args.User, (IComponent)(object)cuffable, (MetaDataComponent)null);
			if (!_actionBlocker.CanInteract(args.User, args.User))
			{
				args.Cancelled = true;
			}
			cuffable.CanStillInteract = false;
			((EntitySystem)this).Dirty(args.User, (IComponent)(object)cuffable, (MetaDataComponent)null);
		}
		else if (!_actionBlocker.CanInteract(args.User, args.Target))
		{
			args.Cancelled = true;
		}
		if (args.Cancelled)
		{
			_popup.PopupClient(base.Loc.GetString("cuffable-component-cannot-interact-message"), args.Target, args.User);
		}
	}

	private void OnStartup(EntityUid uid, CuffableComponent component, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.Container = _container.EnsureContainer<Container>(uid, ((EntitySystem)this).Factory.GetComponentName(((object)component).GetType()), (ContainerManagerComponent)null);
	}

	private void OnRejuvenate(EntityUid uid, CuffableComponent component, RejuvenateEvent args)
	{
		_container.EmptyContainer((BaseContainer)(object)component.Container, true, (EntityCoordinates?)null, true);
	}

	private void OnCuffsRemovedFromContainer(EntityUid uid, CuffableComponent component, EntRemovedFromContainerMessage args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != ((BaseContainer)(component.Container?)).ID))
		{
			_virtualItem.DeleteInHandsMatching(uid, ((ContainerModifiedMessage)args).Entity);
			UpdateCuffState(uid, component);
		}
	}

	private void OnCuffsInsertedIntoContainer(EntityUid uid, CuffableComponent component, ContainerModifiedMessage args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if ((object)args.Container == component.Container)
		{
			UpdateCuffState(uid, component);
		}
	}

	public void UpdateCuffState(EntityUid uid, CuffableComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent hands = default(HandsComponent);
		bool canInteract = ((EntitySystem)this).TryComp<HandsComponent>(uid, ref hands) && hands.Hands.Count > component.CuffedHandCount;
		if (canInteract != component.CanStillInteract)
		{
			component.CanStillInteract = canInteract;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			_actionBlocker.UpdateCanMove(uid);
			if (component.CanStillInteract)
			{
				_alerts.ClearAlert(uid, component.CuffedAlert);
			}
			else
			{
				_alerts.ShowAlert(uid, component.CuffedAlert);
			}
			CuffedStateChangeEvent ev = default(CuffedStateChangeEvent);
			((EntitySystem)this).RaiseLocalEvent<CuffedStateChangeEvent>(uid, ref ev, false);
		}
	}

	private void OnBeingPulledAttempt(EntityUid uid, CuffableComponent component, BeingPulledAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent pullable = default(PullableComponent);
		if (((EntitySystem)this).TryComp<PullableComponent>(uid, ref pullable) && pullable.Puller.HasValue && !component.CanStillInteract)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnBuckleAttempt(Entity<CuffableComponent> ent, EntityUid? user, ref bool cancelled, bool buckling, bool popup)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (cancelled)
		{
			return;
		}
		EntityUid? val = user;
		EntityUid owner = ent.Owner;
		HandsComponent hands = default(HandsComponent);
		if (val.HasValue && !(val.GetValueOrDefault() != owner) && ((EntitySystem)this).TryComp<HandsComponent>(Entity<CuffableComponent>.op_Implicit(ent), ref hands) && ent.Comp.CuffedHandCount >= hands.Count)
		{
			cancelled = true;
			if (popup)
			{
				string message = (buckling ? base.Loc.GetString("handcuff-component-cuff-interrupt-buckled-message") : base.Loc.GetString("handcuff-component-cuff-interrupt-unbuckled-message"));
				_popup.PopupClient(message, Entity<CuffableComponent>.op_Implicit(ent), user);
			}
		}
	}

	private void OnBuckleAttemptEvent(Entity<CuffableComponent> ent, ref BuckleAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnBuckleAttempt(ent, args.User, ref args.Cancelled, buckling: true, args.Popup);
	}

	private void OnUnbuckleAttemptEvent(Entity<CuffableComponent> ent, ref UnbuckleAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnBuckleAttempt(ent, args.User, ref args.Cancelled, buckling: false, args.Popup);
	}

	private void OnPull(EntityUid uid, CuffableComponent component, PullMessage args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!component.CanStillInteract)
		{
			_actionBlocker.UpdateCanMove(uid);
		}
	}

	private void HandleMoveAttempt(EntityUid uid, CuffableComponent component, UpdateCanMoveEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		PullableComponent pullable = default(PullableComponent);
		if (!component.CanStillInteract && ((EntitySystem)this).TryComp<PullableComponent>(uid, ref pullable) && pullable.BeingPulled)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void HandleStopPull(EntityUid uid, CuffableComponent component, AttemptStopPullingEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (args.User.HasValue && ((EntitySystem)this).Exists(args.User.Value) && args.User.Value == uid && !component.CanStillInteract)
		{
			args.Cancelled = true;
		}
	}

	private void OnRemoveCuffsAlert(Entity<CuffableComponent> ent, ref RemoveCuffsAlertEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			EntityUid target = Entity<CuffableComponent>.op_Implicit(ent);
			EntityUid user = Entity<CuffableComponent>.op_Implicit(ent);
			CuffableComponent comp = ent.Comp;
			TryUncuff(target, user, null, comp);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void AddUncuffVerb(EntityUid uid, CuffableComponent component, GetVerbsEvent<Verb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && component.CuffedHandCount != 0 && args.Hands != null && (!(args.User != args.Target) || args.CanInteract))
		{
			Verb verb = new Verb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					SharedCuffableSystem sharedCuffableSystem = this;
					EntityUid target = uid;
					EntityUid user = args.User;
					CuffableComponent cuffable = component;
					sharedCuffableSystem.TryUncuff(target, user, null, cuffable);
				},
				DoContactInteraction = true,
				Text = base.Loc.GetString("uncuff-verb-get-data-text")
			};
			args.Verbs.Add(verb);
		}
	}

	private void OnCuffableDoAfter(EntityUid uid, CuffableComponent component, UnCuffDoAfterEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		target = args.Args.Used;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid used = target.GetValueOrDefault();
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			EntityUid user = args.Args.User;
			if (!args.Cancelled)
			{
				Uncuff(target2, user, used, component);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("cuffable-component-remove-cuffs-fail-message"), user, user);
			}
		}
	}

	private void OnCuffAfterInteract(EntityUid uid, HandcuffComponent component, AfterInteractEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		if (((EntityUid)(ref target2)).Valid)
		{
			if (!args.CanReach)
			{
				_popup.PopupClient(base.Loc.GetString("handcuff-component-too-far-away-error"), args.User, args.User);
				return;
			}
			bool result = TryCuffing(args.User, target2, uid, component);
			((HandledEntityEventArgs)args).Handled = result;
		}
	}

	private void OnCuffMeleeHit(EntityUid uid, HandcuffComponent component, MeleeHitEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (args.HitEntities.Any())
		{
			TryCuffing(args.User, args.HitEntities.First(), uid, component);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnAddCuffDoAfter(EntityUid uid, HandcuffComponent component, AddCuffDoAfterEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.Args.User;
		CuffableComponent cuffable = default(CuffableComponent);
		if (!((EntitySystem)this).TryComp<CuffableComponent>(args.Args.Target, ref cuffable))
		{
			return;
		}
		EntityUid target = args.Args.Target.Value;
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!args.Cancelled && TryAddNewCuffs(target, user, uid, cuffable))
		{
			component.Used = true;
			_audio.PlayPredicted(component.EndCuffSound, uid, (EntityUid?)user, (AudioParams?)null);
			string popupText = ((user == target) ? "handcuff-component-cuff-self-observer-success-message" : "handcuff-component-cuff-observer-success-message");
			_popup.PopupEntity(base.Loc.GetString(popupText, (ValueTuple<string, object>)("user", Identity.Name(user, (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("target", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), target, Filter.Pvs(target, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhere((Predicate<ICommonSession>)delegate(ICommonSession e)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? attachedEntity = e.AttachedEntity;
				EntityUid val = target;
				if (!attachedEntity.HasValue || !(attachedEntity.GetValueOrDefault() == val))
				{
					attachedEntity = e.AttachedEntity;
					val = user;
					if (!attachedEntity.HasValue)
					{
						return false;
					}
					return attachedEntity.GetValueOrDefault() == val;
				}
				return true;
			}), recordReplay: true);
			if (target == user)
			{
				_popup.PopupClient(base.Loc.GetString("handcuff-component-cuff-self-success-message"), user, user);
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(19, 1);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
				handler.AppendLiteral(" has cuffed himself");
				adminLog.Add(LogType.Action, LogImpact.Medium, ref handler);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("handcuff-component-cuff-other-success-message", (ValueTuple<string, object>)("otherName", Identity.Name(target, (IEntityManager)(object)base.EntityManager, user))), user, user);
				_popup.PopupClient(base.Loc.GetString("handcuff-component-cuff-by-other-success-message", (ValueTuple<string, object>)("otherName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, target))), target, target);
				ISharedAdminLogManager adminLog2 = _adminLog;
				LogStringHandler handler2 = new LogStringHandler(12, 2);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
				handler2.AppendLiteral(" has cuffed ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "player", "ToPrettyString(target)");
				adminLog2.Add(LogType.Action, LogImpact.High, ref handler2);
			}
		}
		else if (target == user)
		{
			_popup.PopupClient(base.Loc.GetString("handcuff-component-cuff-interrupt-self-message"), user, user);
		}
		else
		{
			_popup.PopupClient(base.Loc.GetString("handcuff-component-cuff-interrupt-message", (ValueTuple<string, object>)("targetName", Identity.Name(target, (IEntityManager)(object)base.EntityManager, user))), user, user);
			_popup.PopupClient(base.Loc.GetString("handcuff-component-cuff-interrupt-other-message", (ValueTuple<string, object>)("otherName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, target)), (ValueTuple<string, object>)("otherEnt", user)), target, target);
		}
	}

	private void OnCuffVirtualItemDeleted(EntityUid uid, HandcuffComponent component, VirtualItemDeletedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Uncuff(args.User, null, uid, null, component);
	}

	private void OnHandCountChanged(Entity<CuffableComponent> ent, ref HandCountChangedEvent message)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Container != null)
		{
			bool dirty = false;
			int handCount = ((EntitySystem)this).CompOrNull<HandsComponent>(ent.Owner)?.Count ?? 0;
			while (ent.Comp.CuffedHandCount > handCount && ent.Comp.CuffedHandCount > 0)
			{
				dirty = true;
				IReadOnlyList<EntityUid> containedEntities = ((BaseContainer)ent.Comp.Container).ContainedEntities;
				EntityUid handcuffEntity = containedEntities[containedEntities.Count - 1];
				_transform.PlaceNextTo(Entity<TransformComponent>.op_Implicit(handcuffEntity), Entity<TransformComponent>.op_Implicit(ent.Owner));
			}
			if (dirty)
			{
				UpdateCuffState(ent.Owner, ent.Comp);
			}
		}
	}

	private void UpdateHeldItems(EntityUid uid, EntityUid handcuff, CuffableComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComponent = default(HandsComponent);
		if (!((EntitySystem)this).Resolve<CuffableComponent>(uid, ref component, true) || !((EntitySystem)this).TryComp<HandsComponent>(uid, ref handsComponent))
		{
			return;
		}
		int freeHands = 0;
		foreach (string hand in _hands.EnumerateHands(Entity<HandsComponent>.op_Implicit((uid, handsComponent))))
		{
			if (!_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit((uid, handsComponent)), hand, out var held))
			{
				freeHands++;
			}
			else if (!((EntitySystem)this).HasComp<UnremoveableComponent>(held))
			{
				_hands.DoDrop(Entity<HandsComponent>.op_Implicit(uid), hand);
				freeHands++;
				if (freeHands == 2)
				{
					break;
				}
			}
		}
		if (_virtualItem.TrySpawnVirtualItemInHand(handcuff, uid, out var virtItem1))
		{
			((EntitySystem)this).EnsureComp<UnremoveableComponent>(virtItem1.Value);
		}
		if (_virtualItem.TrySpawnVirtualItemInHand(handcuff, uid, out var virtItem2))
		{
			((EntitySystem)this).EnsureComp<UnremoveableComponent>(virtItem2.Value);
		}
	}

	public bool TryAddNewCuffs(EntityUid target, EntityUid user, EntityUid handcuff, CuffableComponent? component = null, HandcuffComponent? cuff = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<CuffableComponent>(target, ref component, true) || !((EntitySystem)this).Resolve<HandcuffComponent>(handcuff, ref cuff, true))
		{
			return false;
		}
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(handcuff), Entity<TransformComponent>.op_Implicit(target)))
		{
			return false;
		}
		HandsComponent hands = default(HandsComponent);
		if (((EntitySystem)this).TryComp<HandsComponent>(target, ref hands) && hands.Count <= component.CuffedHandCount)
		{
			return false;
		}
		TargetHandcuffedEvent ev = default(TargetHandcuffedEvent);
		((EntitySystem)this).RaiseLocalEvent<TargetHandcuffedEvent>(target, ref ev, false);
		_hands.TryDrop(Entity<HandsComponent>.op_Implicit(user), handcuff);
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(handcuff), (BaseContainer)(object)component.Container, (TransformComponent)null, false);
		UpdateHeldItems(target, handcuff, component);
		return true;
	}

	public bool TryCuffing(EntityUid user, EntityUid target, EntityUid handcuff, HandcuffComponent? handcuffComponent = null, CuffableComponent? cuffable = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandcuffComponent>(handcuff, ref handcuffComponent, true) || !((EntitySystem)this).Resolve<CuffableComponent>(target, ref cuffable, false))
		{
			return false;
		}
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(target, ref hands))
		{
			_popup.PopupClient(base.Loc.GetString("handcuff-component-target-has-no-hands-error", (ValueTuple<string, object>)("targetName", Identity.Name(target, (IEntityManager)(object)base.EntityManager, user))), user, user);
			return true;
		}
		if (cuffable.CuffedHandCount >= hands.Count)
		{
			_popup.PopupClient(base.Loc.GetString("handcuff-component-target-has-no-free-hands-error", (ValueTuple<string, object>)("targetName", Identity.Name(target, (IEntityManager)(object)base.EntityManager, user))), user, user);
			return true;
		}
		if (!_hands.CanDrop(Entity<HandsComponent>.op_Implicit(user), handcuff))
		{
			_popup.PopupClient(base.Loc.GetString("handcuff-component-cannot-drop-cuffs", (ValueTuple<string, object>)("target", Identity.Name(target, (IEntityManager)(object)base.EntityManager, user))), user, user);
			return false;
		}
		float cuffTime = handcuffComponent.CuffTime;
		if (((EntitySystem)this).HasComp<StunnedComponent>(target))
		{
			cuffTime = MathF.Max(0.1f, cuffTime - handcuffComponent.StunBonus);
		}
		if (((EntitySystem)this).HasComp<DisarmProneComponent>(target))
		{
			cuffTime = 0f;
		}
		DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, cuffTime, new AddCuffDoAfterEvent(), handcuff, target, handcuff)
		{
			BreakOnMove = true,
			BreakOnWeightlessMove = false,
			BreakOnDamage = true,
			NeedHand = true,
			DistanceThreshold = 1f,
			ForceVisible = (user != target)
		};
		if (!_doAfter.TryStartDoAfter(doAfterEventArgs))
		{
			return true;
		}
		string popupText = ((user == target) ? "handcuff-component-start-cuffing-self-observer" : "handcuff-component-start-cuffing-observer");
		_popup.PopupEntity(base.Loc.GetString(popupText, (ValueTuple<string, object>)("user", Identity.Name(user, (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("target", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), target, Filter.Pvs(target, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhere((Predicate<ICommonSession>)delegate(ICommonSession e)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? attachedEntity = e.AttachedEntity;
			EntityUid val = target;
			if (!attachedEntity.HasValue || !(attachedEntity.GetValueOrDefault() == val))
			{
				attachedEntity = e.AttachedEntity;
				val = user;
				if (!attachedEntity.HasValue)
				{
					return false;
				}
				return attachedEntity.GetValueOrDefault() == val;
			}
			return true;
		}), recordReplay: true);
		if (target == user)
		{
			_popup.PopupClient(base.Loc.GetString("handcuff-component-target-self"), user, user);
		}
		else
		{
			_popup.PopupClient(base.Loc.GetString("handcuff-component-start-cuffing-target-message", (ValueTuple<string, object>)("targetName", Identity.Name(target, (IEntityManager)(object)base.EntityManager, user))), user, user);
			_popup.PopupEntity(base.Loc.GetString("handcuff-component-start-cuffing-by-other-message", (ValueTuple<string, object>)("otherName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, target))), target, target);
		}
		_audio.PlayPredicted(handcuffComponent.StartCuffSound, handcuff, (EntityUid?)user, (AudioParams?)null);
		return true;
	}

	public bool IsCuffed(Entity<CuffableComponent> target, bool requireFullyCuffed = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent hands = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(Entity<CuffableComponent>.op_Implicit(target), ref hands))
		{
			return false;
		}
		if (target.Comp.CuffedHandCount <= 0)
		{
			return false;
		}
		if (requireFullyCuffed && hands.Count > target.Comp.CuffedHandCount)
		{
			return false;
		}
		return true;
	}

	public void TryUncuff(EntityUid target, EntityUid user, EntityUid? cuffsToRemove = null, CuffableComponent? cuffable = null, HandcuffComponent? cuff = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<CuffableComponent>(target, ref cuffable, true))
		{
			return;
		}
		bool isOwner = user == target;
		if (!cuffsToRemove.HasValue)
		{
			if (((BaseContainer)cuffable.Container).ContainedEntities.Count == 0)
			{
				return;
			}
			cuffsToRemove = cuffable.LastAddedCuffs;
		}
		else if (!((BaseContainer)cuffable.Container).ContainedEntities.Contains(cuffsToRemove.Value))
		{
			((EntitySystem)this).Log.Warning("A user is trying to remove handcuffs that aren't in the owner's container. This should never happen!");
		}
		if (!((EntitySystem)this).Resolve<HandcuffComponent>(cuffsToRemove.Value, ref cuff, true))
		{
			return;
		}
		UncuffAttemptEvent attempt = new UncuffAttemptEvent(user, target);
		((EntitySystem)this).RaiseLocalEvent<UncuffAttemptEvent>(user, ref attempt, true);
		if (attempt.Cancelled)
		{
			return;
		}
		if (!isOwner && !_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target)))
		{
			_popup.PopupClient(base.Loc.GetString("cuffable-component-cannot-remove-cuffs-too-far-message"), user, user);
			return;
		}
		ModifyUncuffDurationEvent ev = new ModifyUncuffDurationEvent(user, target, isOwner ? cuff.BreakoutTime : cuff.UncuffTime);
		((EntitySystem)this).RaiseLocalEvent<ModifyUncuffDurationEvent>(user, ref ev, false);
		float uncuffTime = ev.Duration;
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (isOwner && (!((EntitySystem)this).TryComp<UseDelayComponent>(cuffsToRemove.Value, ref useDelay) || !_delay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((cuffsToRemove.Value, useDelay)), checkDelayed: true)))
		{
			return;
		}
		DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, uncuffTime, new UnCuffDoAfterEvent(), target, target, cuffsToRemove)
		{
			BreakOnMove = true,
			BreakOnWeightlessMove = false,
			BreakOnDamage = true,
			NeedHand = true,
			RequireCanInteract = false,
			DistanceThreshold = 1f,
			ForceVisible = (user != target)
		};
		if (!_doAfter.TryStartDoAfter(doAfterEventArgs))
		{
			return;
		}
		ISharedAdminLogManager adminLog = _adminLog;
		LogStringHandler handler = new LogStringHandler(21, 2);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
		handler.AppendLiteral(" is trying to uncuff ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "subject", "ToPrettyString(target)");
		adminLog.Add(LogType.Action, LogImpact.High, ref handler);
		string popupText = ((user == target) ? "cuffable-component-start-uncuffing-self-observer" : "cuffable-component-start-uncuffing-observer");
		_popup.PopupEntity(base.Loc.GetString(popupText, (ValueTuple<string, object>)("user", Identity.Name(user, (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("target", Identity.Entity(target, (IEntityManager)(object)base.EntityManager))), target, Filter.Pvs(target, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhere((Predicate<ICommonSession>)delegate(ICommonSession e)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? attachedEntity = e.AttachedEntity;
			EntityUid val = target;
			if (!attachedEntity.HasValue || !(attachedEntity.GetValueOrDefault() == val))
			{
				attachedEntity = e.AttachedEntity;
				val = user;
				if (!attachedEntity.HasValue)
				{
					return false;
				}
				return attachedEntity.GetValueOrDefault() == val;
			}
			return true;
		}), recordReplay: true);
		if (target == user)
		{
			_popup.PopupClient(base.Loc.GetString("cuffable-component-start-uncuffing-self"), user, user);
		}
		else
		{
			_popup.PopupClient(base.Loc.GetString("cuffable-component-start-uncuffing-target-message", (ValueTuple<string, object>)("targetName", Identity.Name(target, (IEntityManager)(object)base.EntityManager, user))), user, user);
			_popup.PopupEntity(base.Loc.GetString("cuffable-component-start-uncuffing-by-other-message", (ValueTuple<string, object>)("otherName", Identity.Name(user, (IEntityManager)(object)base.EntityManager, target))), target, target);
		}
		_audio.PlayPredicted(isOwner ? cuff.StartBreakoutSound : cuff.StartUncuffSound, target, (EntityUid?)user, (AudioParams?)null);
	}

	public void Uncuff(EntityUid target, EntityUid? user, EntityUid cuffsToRemove, CuffableComponent? cuffable = null, HandcuffComponent? cuff = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<CuffableComponent>(target, ref cuffable, true) || !((EntitySystem)this).Resolve<HandcuffComponent>(cuffsToRemove, ref cuff, true) || !cuff.Used || cuff.Removing || ((EntitySystem)this).TerminatingOrDeleted(cuffsToRemove, (MetaDataComponent)null) || ((EntitySystem)this).TerminatingOrDeleted(target, (MetaDataComponent)null))
		{
			return;
		}
		if (user.HasValue)
		{
			UncuffAttemptEvent attempt = new UncuffAttemptEvent(user.Value, target);
			((EntitySystem)this).RaiseLocalEvent<UncuffAttemptEvent>(user.Value, ref attempt, false);
			if (attempt.Cancelled)
			{
				return;
			}
		}
		cuff.Removing = true;
		cuff.Used = false;
		_audio.PlayPredicted(cuff.EndUncuffSound, target, user, (AudioParams?)null);
		_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(cuffsToRemove), (BaseContainer)(object)cuffable.Container, true, false, (EntityCoordinates?)null, (Angle?)null);
		if (_net.IsServer)
		{
			if (cuff.BreakOnRemove)
			{
				((EntitySystem)this).QueueDel((EntityUid?)cuffsToRemove);
				EntProtoId? brokenPrototype = cuff.BrokenPrototype;
				EntityUid trash = ((EntitySystem)this).Spawn(brokenPrototype.HasValue ? EntProtoId.op_Implicit(brokenPrototype.GetValueOrDefault()) : null, ((EntitySystem)this).Transform(cuffsToRemove).Coordinates);
				_hands.PickupOrDrop(user, trash);
			}
			else
			{
				_hands.PickupOrDrop(user, cuffsToRemove);
			}
		}
		bool shoved = false;
		if (_combatMode.IsInCombatMode(user))
		{
			EntityUid val = target;
			EntityUid? val2 = user;
			if ((!val2.HasValue || val != val2.GetValueOrDefault()) && user.HasValue)
			{
				DisarmedEvent eventArgs = new DisarmedEvent(target, user.Value, 1f);
				((EntitySystem)this).RaiseLocalEvent<DisarmedEvent>(target, ref eventArgs, false);
				shoved = true;
			}
		}
		if (cuffable.CuffedHandCount == 0)
		{
			if (user.HasValue)
			{
				if (shoved)
				{
					_popup.PopupClient(base.Loc.GetString("cuffable-component-remove-cuffs-push-success-message", (ValueTuple<string, object>)("otherName", Identity.Name(user.Value, (IEntityManager)(object)base.EntityManager, user))), user.Value, user.Value);
				}
				else
				{
					_popup.PopupClient(base.Loc.GetString("cuffable-component-remove-cuffs-success-message"), user.Value, user.Value);
				}
			}
			EntityUid val = target;
			EntityUid? val2 = user;
			if ((!val2.HasValue || val != val2.GetValueOrDefault()) && user.HasValue)
			{
				_popup.PopupEntity(base.Loc.GetString("cuffable-component-remove-cuffs-by-other-success-message", (ValueTuple<string, object>)("otherName", Identity.Name(user.Value, (IEntityManager)(object)base.EntityManager, user))), target, target);
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(27, 2);
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "player", "ToPrettyString(user)");
				handler.AppendLiteral(" has successfully uncuffed ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "player", "ToPrettyString(target)");
				adminLog.Add(LogType.Action, LogImpact.High, ref handler);
			}
			else
			{
				ISharedAdminLogManager adminLog2 = _adminLog;
				LogStringHandler handler2 = new LogStringHandler(37, 1);
				handler2.AppendFormatted(((EntitySystem)this).ToPrettyString(user, (MetaDataComponent)null), "player", "ToPrettyString(user)");
				handler2.AppendLiteral(" has successfully uncuffed themselves");
				adminLog2.Add(LogType.Action, LogImpact.High, ref handler2);
			}
		}
		else if (user.HasValue)
		{
			EntityUid? val2 = user;
			EntityUid val = target;
			if (!val2.HasValue || val2.GetValueOrDefault() != val)
			{
				_popup.PopupClient(base.Loc.GetString("cuffable-component-remove-cuffs-partial-success-message", (ValueTuple<string, object>)("cuffedHandCount", cuffable.CuffedHandCount), (ValueTuple<string, object>)("otherName", Identity.Name(user.Value, (IEntityManager)(object)base.EntityManager, user.Value))), user.Value, user.Value);
				_popup.PopupEntity(base.Loc.GetString("cuffable-component-remove-cuffs-by-other-partial-success-message", (ValueTuple<string, object>)("otherName", Identity.Name(user.Value, (IEntityManager)(object)base.EntityManager, user.Value)), (ValueTuple<string, object>)("cuffedHandCount", cuffable.CuffedHandCount)), target, target);
			}
			else
			{
				_popup.PopupClient(base.Loc.GetString("cuffable-component-remove-cuffs-partial-success-message", (ValueTuple<string, object>)("cuffedHandCount", cuffable.CuffedHandCount)), user.Value, user.Value);
			}
		}
		cuff.Removing = false;
	}

	private void CheckAct(EntityUid uid, CuffableComponent component, CancellableEntityEventArgs args)
	{
		if (!component.CanStillInteract)
		{
			args.Cancel();
		}
	}

	private void OnEquipAttempt(EntityUid uid, CuffableComponent component, IsEquippingAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Equipee == uid)
		{
			CheckAct(uid, component, (CancellableEntityEventArgs)(object)args);
		}
	}

	private void OnUnequipAttempt(EntityUid uid, CuffableComponent component, IsUnequippingAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Unequipee == uid)
		{
			CheckAct(uid, component, (CancellableEntityEventArgs)(object)args);
		}
	}

	public IReadOnlyList<EntityUid> GetAllCuffs(CuffableComponent component)
	{
		return ((BaseContainer)component.Container).ContainedEntities;
	}
}
