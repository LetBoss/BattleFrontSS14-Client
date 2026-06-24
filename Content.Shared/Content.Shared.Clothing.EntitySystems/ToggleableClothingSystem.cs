using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Content.Shared.Strip;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class ToggleableClothingSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _netMan;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private ActionContainerSystem _actionContainer;

	[Dependency]
	private InventorySystem _inventorySystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedStrippableSystem _strippable;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ToggleableClothingComponent, ComponentInit>((ComponentEventHandler<ToggleableClothingComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableClothingComponent, MapInitEvent>((ComponentEventHandler<ToggleableClothingComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableClothingComponent, ToggleClothingEvent>((ComponentEventHandler<ToggleableClothingComponent, ToggleClothingEvent>)OnToggleClothing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableClothingComponent, GetItemActionsEvent>((ComponentEventHandler<ToggleableClothingComponent, GetItemActionsEvent>)OnGetActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableClothingComponent, ComponentRemove>((ComponentEventHandler<ToggleableClothingComponent, ComponentRemove>)OnRemoveToggleable, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableClothingComponent, GotUnequippedEvent>((ComponentEventHandler<ToggleableClothingComponent, GotUnequippedEvent>)OnToggleableUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachedClothingComponent, InteractHandEvent>((ComponentEventHandler<AttachedClothingComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachedClothingComponent, GotUnequippedEvent>((ComponentEventHandler<AttachedClothingComponent, GotUnequippedEvent>)OnAttachedUnequip, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachedClothingComponent, ComponentRemove>((ComponentEventHandler<AttachedClothingComponent, ComponentRemove>)OnRemoveAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachedClothingComponent, BeingUnequippedAttemptEvent>((ComponentEventHandler<AttachedClothingComponent, BeingUnequippedAttemptEvent>)OnAttachedUnequipAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableClothingComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>((ComponentEventHandler<ToggleableClothingComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>)GetRelayedVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableClothingComponent, GetVerbsEvent<EquipmentVerb>>((ComponentEventHandler<ToggleableClothingComponent, GetVerbsEvent<EquipmentVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachedClothingComponent, GetVerbsEvent<EquipmentVerb>>((ComponentEventHandler<AttachedClothingComponent, GetVerbsEvent<EquipmentVerb>>)OnGetAttachedStripVerbsEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ToggleableClothingComponent, ToggleClothingDoAfterEvent>((ComponentEventHandler<ToggleableClothingComponent, ToggleClothingDoAfterEvent>)OnDoAfterComplete, (Type[])null, (Type[])null);
	}

	private void GetRelayedVerbs(EntityUid uid, ToggleableClothingComponent component, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnGetVerbs(uid, component, args.Args);
	}

	private void OnGetVerbs(EntityUid uid, ToggleableClothingComponent component, GetVerbsEvent<EquipmentVerb> args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || args.Hands == null || !component.ClothingUid.HasValue || component.Container == null)
		{
			return;
		}
		string text = component.VerbText ?? ((!component.ActionEntity.HasValue) ? null : ((EntitySystem)this).Name(component.ActionEntity.Value, (MetaDataComponent)null));
		if (text == null || !_inventorySystem.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid), component.RequiredFlags))
		{
			return;
		}
		EntityUid wearer = ((EntitySystem)this).Transform(uid).ParentUid;
		if (args.User != wearer && !component.StripDelay.HasValue)
		{
			return;
		}
		EquipmentVerb verb = new EquipmentVerb
		{
			Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/outfit.svg.192dpi.png")),
			Text = base.Loc.GetString(text)
		};
		if (args.User == wearer)
		{
			verb.EventTarget = uid;
			verb.ExecutionEventArgs = new ToggleClothingEvent
			{
				Performer = args.User
			};
		}
		else
		{
			verb.Act = delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				StartDoAfter(args.User, uid, ((EntitySystem)this).Transform(uid).ParentUid, component);
			};
		}
		args.Verbs.Add(verb);
	}

	private void StartDoAfter(EntityUid user, EntityUid item, EntityUid wearer, ToggleableClothingComponent component)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (component.StripDelay.HasValue)
		{
			(TimeSpan Time, bool Stealth) stripTimeModifiers = _strippable.GetStripTimeModifiers(user, wearer, item, component.StripDelay.Value);
			TimeSpan time = stripTimeModifiers.Time;
			bool stealth = stripTimeModifiers.Stealth;
			DoAfterArgs args = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, time, new ToggleClothingDoAfterEvent(), item, wearer, item)
			{
				BreakOnDamage = true,
				BreakOnMove = true,
				DistanceThreshold = 2f
			};
			if (_doAfter.TryStartDoAfter(args) && !stealth)
			{
				string popup = base.Loc.GetString("strippable-component-alert-owner-interact", (ValueTuple<string, object>)("user", Identity.Entity(user, (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("item", item));
				_popupSystem.PopupEntity(popup, wearer, wearer, PopupType.Large);
			}
		}
	}

	private void OnGetAttachedStripVerbsEvent(EntityUid uid, AttachedClothingComponent component, GetVerbsEvent<EquipmentVerb> args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		OnGetVerbs(component.AttachedUid, ((EntitySystem)this).Comp<ToggleableClothingComponent>(component.AttachedUid), args);
	}

	private void OnDoAfterComplete(EntityUid uid, ToggleableClothingComponent component, ToggleClothingDoAfterEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			ToggleClothing(args.User, uid, component);
		}
	}

	private void OnInteractHand(EntityUid uid, AttachedClothingComponent component, InteractHandEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		ToggleableClothingComponent toggleCom = default(ToggleableClothingComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<ToggleableClothingComponent>(component.AttachedUid, ref toggleCom) && toggleCom.Container != null && _inventorySystem.TryUnequip(((EntitySystem)this).Transform(uid).ParentUid, toggleCom.Slot, silent: false, force: true))
		{
			_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(uid), (BaseContainer)(object)toggleCom.Container, (TransformComponent)null, false);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnToggleableUnequip(EntityUid uid, ToggleableClothingComponent component, GotUnequippedEvent args)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && component.Container != null && !component.Container.ContainedEntity.HasValue && component.ClothingUid.HasValue)
		{
			_inventorySystem.TryUnequip(args.Equipee, component.Slot, silent: false, force: true, predicted: false, null, null, reparent: true, checkDoafter: false, triggerHandContact: true);
		}
	}

	private void OnRemoveToggleable(EntityUid uid, ToggleableClothingComponent component, ComponentRemove args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actionsSystem = _actionsSystem;
		EntityUid? actionEntity = component.ActionEntity;
		actionsSystem.RemoveAction(actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		if (component.ClothingUid.HasValue && !_netMan.IsClient)
		{
			((EntitySystem)this).QueueDel((EntityUid?)component.ClothingUid.Value);
		}
	}

	private void OnAttachedUnequipAttempt(EntityUid uid, AttachedClothingComponent component, BeingUnequippedAttemptEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnRemoveAttached(EntityUid uid, AttachedClothingComponent component, ComponentRemove args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		ToggleableClothingComponent toggleComp = default(ToggleableClothingComponent);
		if (((EntitySystem)this).TryComp<ToggleableClothingComponent>(component.AttachedUid, ref toggleComp) && (int)((Component)toggleComp).LifeStage <= 6)
		{
			SharedActionsSystem actionsSystem = _actionsSystem;
			EntityUid? actionEntity = toggleComp.ActionEntity;
			actionsSystem.RemoveAction(actionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			((EntitySystem)this).RemComp(component.AttachedUid, (IComponent)(object)toggleComp);
		}
	}

	private void OnAttachedUnequip(EntityUid uid, AttachedClothingComponent component, GotUnequippedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		ToggleableClothingComponent toggleComp = default(ToggleableClothingComponent);
		if (!_timing.ApplyingState && (int)((Component)component).LifeStage <= 6 && ((EntitySystem)this).TryComp<ToggleableClothingComponent>(component.AttachedUid, ref toggleComp) && (int)((Component)toggleComp).LifeStage <= 6 && toggleComp.ClothingUid.HasValue && toggleComp.Container != null)
		{
			_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(toggleComp.ClothingUid.Value), (BaseContainer)(object)toggleComp.Container, (TransformComponent)null, false);
		}
	}

	private void OnToggleClothing(EntityUid uid, ToggleableClothingComponent component, ToggleClothingEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ToggleClothing(args.Performer, uid, component);
		}
	}

	private void ToggleClothing(EntityUid user, EntityUid target, ToggleableClothingComponent component)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (component.Container != null && component.ClothingUid.HasValue)
		{
			EntityUid parent = ((EntitySystem)this).Transform(target).ParentUid;
			EntityUid? existing;
			if (!component.Container.ContainedEntity.HasValue)
			{
				_inventorySystem.TryUnequip(user, parent, component.Slot, silent: false, force: true);
			}
			else if (_inventorySystem.TryGetSlotEntity(parent, component.Slot, out existing))
			{
				_popupSystem.PopupClient(base.Loc.GetString("toggleable-clothing-remove-first", (ValueTuple<string, object>)("entity", existing)), user, user);
			}
			else
			{
				_inventorySystem.TryEquip(user, parent, component.ClothingUid.Value, component.Slot, silent: false, force: false, predicted: false, null, null, checkDoafter: false, triggerHandContact: true);
			}
		}
	}

	private void OnGetActions(EntityUid uid, ToggleableClothingComponent component, GetItemActionsEvent args)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (component.ClothingUid.HasValue && component.ActionEntity.HasValue && (args.SlotFlags & component.RequiredFlags) == component.RequiredFlags)
		{
			args.AddAction(component.ActionEntity.Value);
		}
	}

	private void OnInit(EntityUid uid, ToggleableClothingComponent component, ComponentInit args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		component.Container = _containerSystem.EnsureContainer<ContainerSlot>(uid, component.ContainerId, (ContainerManagerComponent)null);
	}

	private void OnMapInit(EntityUid uid, ToggleableClothingComponent component, MapInitEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? containedEntity = component.Container.ContainedEntity;
		if (containedEntity.HasValue)
		{
			containedEntity.GetValueOrDefault();
			return;
		}
		if (!component.ClothingUid.HasValue || !component.ActionEntity.HasValue)
		{
			TransformComponent xform = ((EntitySystem)this).Transform(uid);
			component.ClothingUid = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(component.ClothingPrototype), xform.Coordinates);
			AttachedClothingComponent attachedClothing = ((EntitySystem)this).EnsureComp<AttachedClothingComponent>(component.ClothingUid.Value);
			attachedClothing.AttachedUid = uid;
			((EntitySystem)this).Dirty(component.ClothingUid.Value, (IComponent)(object)attachedClothing, (MetaDataComponent)null);
			_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(component.ClothingUid.Value), (BaseContainer)(object)component.Container, xform, false);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
		if (_actionContainer.EnsureAction(uid, ref component.ActionEntity, out ActionComponent action, EntProtoId.op_Implicit(component.Action)))
		{
			_actionsSystem.SetEntityIcon(Entity<ActionComponent>.op_Implicit((component.ActionEntity.Value, action)), component.ClothingUid);
		}
	}
}
