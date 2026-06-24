using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Item;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Wieldable.Events;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Containers;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableHolderSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedVerbSystem _verbSystem;

	public override void Initialize()
	{
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Expected O, but got Unknown
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Expected O, but got Unknown
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Expected O, but got Unknown
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Expected O, but got Unknown
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Expected O, but got Unknown
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, AttachableAttachDoAfterEvent>((ComponentEventHandler<AttachableHolderComponent, AttachableAttachDoAfterEvent>)OnAttachDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, AttachableDetachDoAfterEvent>((ComponentEventHandler<AttachableHolderComponent, AttachableDetachDoAfterEvent>)OnDetachDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, AttachableHolderAttachToSlotMessage>((ComponentEventHandler<AttachableHolderComponent, AttachableHolderAttachToSlotMessage>)OnAttachableHolderAttachToSlotMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, AttachableHolderDetachMessage>((ComponentEventHandler<AttachableHolderComponent, AttachableHolderDetachMessage>)OnAttachableHolderDetachMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GunShotEvent>((EntityEventRefHandler<AttachableHolderComponent, GunShotEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, BoundUIOpenedEvent>((ComponentEventHandler<AttachableHolderComponent, BoundUIOpenedEvent>)OnAttachableHolderUiOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<AttachableHolderComponent, EntInsertedIntoContainerMessage>)OnAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, MapInitEvent>((EntityEventRefHandler<AttachableHolderComponent, MapInitEvent>)OnHolderMapInit, (Type[])null, new Type[1] { typeof(ContainerFillSystem) });
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GetVerbsEvent<EquipmentVerb>>((EntityEventRefHandler<AttachableHolderComponent, GetVerbsEvent<EquipmentVerb>>)OnAttachableHolderGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GotEquippedHandEvent>((EntityEventRefHandler<AttachableHolderComponent, GotEquippedHandEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GotUnequippedHandEvent>((EntityEventRefHandler<AttachableHolderComponent, GotUnequippedHandEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<AttachableHolderComponent, GunRefreshModifiersEvent>)RelayEvent, (Type[])null, new Type[1] { typeof(SharedWieldableSystem) });
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<AttachableHolderComponent, BeforeRangedInteractEvent>)OnAttachableHolderBeforeRangedInteract, new Type[1] { typeof(SharedStorageSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, InteractUsingEvent>((EntityEventRefHandler<AttachableHolderComponent, InteractUsingEvent>)OnAttachableHolderInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, AfterInteractEvent>((EntityEventRefHandler<AttachableHolderComponent, AfterInteractEvent>)OnAttachableHolderAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, ActivateInWorldEvent>((EntityEventRefHandler<AttachableHolderComponent, ActivateInWorldEvent>)OnAttachableHolderInteractInWorld, new Type[1] { typeof(CMGunSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, ItemWieldedEvent>((EntityEventRefHandler<AttachableHolderComponent, ItemWieldedEvent>)OnHolderWielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, ItemUnwieldedEvent>((EntityEventRefHandler<AttachableHolderComponent, ItemUnwieldedEvent>)OnHolderUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, UniqueActionEvent>((EntityEventRefHandler<AttachableHolderComponent, UniqueActionEvent>)OnAttachableHolderUniqueAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GetGunDamageModifierEvent>((EntityEventRefHandler<AttachableHolderComponent, GetGunDamageModifierEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GunMuzzleFlashAttemptEvent>((EntityEventRefHandler<AttachableHolderComponent, GunMuzzleFlashAttemptEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, HandDeselectedEvent>((EntityEventRefHandler<AttachableHolderComponent, HandDeselectedEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, MeleeHitEvent>((EntityEventRefHandler<AttachableHolderComponent, MeleeHitEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GetWieldableSpeedModifiersEvent>((EntityEventRefHandler<AttachableHolderComponent, GetWieldableSpeedModifiersEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GetWieldDelayEvent>((EntityEventRefHandler<AttachableHolderComponent, GetWieldDelayEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, ContainerGettingInsertedAttemptEvent>((EntityEventRefHandler<AttachableHolderComponent, ContainerGettingInsertedAttemptEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, ContainerGettingRemovedAttemptEvent>((EntityEventRefHandler<AttachableHolderComponent, ContainerGettingRemovedAttemptEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, EntGotRemovedFromContainerMessage>((EntityEventRefHandler<AttachableHolderComponent, EntGotRemovedFromContainerMessage>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GetItemSizeModifiersEvent>((EntityEventRefHandler<AttachableHolderComponent, GetItemSizeModifiersEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GetFireModeValuesEvent>((EntityEventRefHandler<AttachableHolderComponent, GetFireModeValuesEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GetFireModesEvent>((EntityEventRefHandler<AttachableHolderComponent, GetFireModesEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GetDamageFalloffEvent>((EntityEventRefHandler<AttachableHolderComponent, GetDamageFalloffEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GetWeaponAccuracyEvent>((EntityEventRefHandler<AttachableHolderComponent, GetWeaponAccuracyEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, GunGetAmmoSpreadEvent>((EntityEventRefHandler<AttachableHolderComponent, GunGetAmmoSpreadEvent>)RelayEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderComponent, DroppedEvent>((EntityEventRefHandler<AttachableHolderComponent, DroppedEvent>)RelayEvent, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(CMKeyFunctions.RMCActivateAttachableBarrel, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				ToggleAttachable(valueOrDefault, "rmc-aslot-barrel");
			}
		}, (StateInputCmdDelegate)null, false, true)).Bind(CMKeyFunctions.RMCActivateAttachableRail, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				ToggleAttachable(valueOrDefault, "rmc-aslot-rail");
			}
		}, (StateInputCmdDelegate)null, false, true)).Bind(CMKeyFunctions.RMCActivateAttachableStock, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
			if (val.HasValue)
			{
				EntityUid valueOrDefault = val.GetValueOrDefault();
				ToggleAttachable(valueOrDefault, "rmc-aslot-stock");
			}
		}, (StateInputCmdDelegate)null, false, true))
			.Bind(CMKeyFunctions.RMCActivateAttachableUnderbarrel, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
				if (val.HasValue)
				{
					EntityUid valueOrDefault = val.GetValueOrDefault();
					ToggleAttachable(valueOrDefault, "rmc-aslot-underbarrel");
				}
			}, (StateInputCmdDelegate)null, false, true))
			.Bind(CMKeyFunctions.RMCFieldStripHeldItem, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate(ICommonSession? session)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				EntityUid? val = ((session != null) ? session.AttachedEntity : ((EntityUid?)null));
				if (val.HasValue)
				{
					EntityUid valueOrDefault = val.GetValueOrDefault();
					FieldStripHeldItem(valueOrDefault);
				}
			}, (StateInputCmdDelegate)null, false, true))
			.Register<AttachableHolderSystem>();
	}

	public override void Shutdown()
	{
		CommandBinds.Unregister<AttachableHolderSystem>();
	}

	private void OnHolderMapInit(Entity<AttachableHolderComponent> holder, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(holder.Owner);
		EntityCoordinates coords = default(EntityCoordinates);
		((EntityCoordinates)(ref coords))._002Ector(holder.Owner, Vector2.Zero);
		bool doRandom = RandomExtensions.Prob(_random, holder.Comp.RandomAttachmentChance);
		foreach (string slotId in holder.Comp.Slots.Keys)
		{
			AttachableSlot slot = holder.Comp.Slots[slotId];
			EntProtoId<AttachableComponent>? attachment = slot.StartingAttachable;
			if (doRandom)
			{
				List<EntProtoId<AttachableComponent>> random = slot.Random;
				if (random != null && random.Count > 0 && RandomExtensions.Prob(_random, slot.RandomChance))
				{
					attachment = RandomExtensions.Pick<EntProtoId<AttachableComponent>>(_random, (IReadOnlyList<EntProtoId<AttachableComponent>>)random);
				}
			}
			if (attachment.HasValue)
			{
				ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<AttachableHolderComponent>.op_Implicit(holder), slotId, (ContainerManagerComponent)null);
				((BaseContainer)container).OccludesLight = false;
				EntProtoId<AttachableComponent>? val = attachment;
				EntityUid attachableUid = ((EntitySystem)this).Spawn(val.HasValue ? EntProtoId<AttachableComponent>.op_Implicit(val.GetValueOrDefault()) : null, coords);
				_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(attachableUid), (BaseContainer)(object)container, xform, false);
			}
		}
		((EntitySystem)this).Dirty<AttachableHolderComponent>(holder, (MetaDataComponent)null);
	}

	private void OnAttachableHolderBeforeRangedInteract(Entity<AttachableHolderComponent> holder, ref BeforeRangedInteractEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? supercedingAttachable = holder.Comp.SupercedingAttachable;
		if (supercedingAttachable.HasValue)
		{
			EntityUid attachable = supercedingAttachable.GetValueOrDefault();
			BeforeRangedInteractEvent afterInteractEvent = new BeforeRangedInteractEvent(args.User, attachable, args.Target, args.ClickLocation, args.CanReach);
			((EntitySystem)this).RaiseLocalEvent<BeforeRangedInteractEvent>(attachable, afterInteractEvent, false);
			if (((HandledEntityEventArgs)afterInteractEvent).Handled)
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnAttachableHolderInteractUsing(Entity<AttachableHolderComponent> holder, ref InteractUsingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (CanAttach(holder, args.Used))
		{
			StartAttach(holder, args.Used, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
		if (!holder.Comp.SupercedingAttachable.HasValue)
		{
			return;
		}
		InteractUsingEvent interactUsingEvent = new InteractUsingEvent(args.User, args.Used, holder.Comp.SupercedingAttachable.Value, args.ClickLocation);
		((EntitySystem)this).RaiseLocalEvent<InteractUsingEvent>(holder.Comp.SupercedingAttachable.Value, interactUsingEvent, false);
		if (((HandledEntityEventArgs)interactUsingEvent).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			return;
		}
		AfterInteractEvent afterInteractEvent = new AfterInteractEvent(args.User, args.Used, holder.Comp.SupercedingAttachable.Value, args.ClickLocation, canReach: true);
		((EntitySystem)this).RaiseLocalEvent<AfterInteractEvent>(args.Used, afterInteractEvent, false);
		if (((HandledEntityEventArgs)afterInteractEvent).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnAttachableHolderAfterInteract(Entity<AttachableHolderComponent> holder, ref AfterInteractEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? supercedingAttachable = holder.Comp.SupercedingAttachable;
		if (supercedingAttachable.HasValue)
		{
			EntityUid attachable = supercedingAttachable.GetValueOrDefault();
			AfterInteractEvent afterInteractEvent = new AfterInteractEvent(args.User, attachable, args.Target, args.ClickLocation, args.CanReach);
			((EntitySystem)this).RaiseLocalEvent<AfterInteractEvent>(attachable, afterInteractEvent, false);
			if (((HandledEntityEventArgs)afterInteractEvent).Handled)
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void OnAttachableHolderInteractInWorld(Entity<AttachableHolderComponent> holder, ref ActivateInWorldEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && holder.Comp.SupercedingAttachable.HasValue)
		{
			ActivateInWorldEvent activateInWorldEvent = new ActivateInWorldEvent(args.User, holder.Comp.SupercedingAttachable.Value, args.Complex);
			((EntitySystem)this).RaiseLocalEvent<ActivateInWorldEvent>(holder.Comp.SupercedingAttachable.Value, activateInWorldEvent, false);
			((HandledEntityEventArgs)args).Handled = ((HandledEntityEventArgs)activateInWorldEvent).Handled;
		}
	}

	private void OnAttachableHolderUniqueAction(Entity<AttachableHolderComponent> holder, ref UniqueActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (holder.Comp.SupercedingAttachable.HasValue && !((HandledEntityEventArgs)args).Handled)
		{
			((EntitySystem)this).RaiseLocalEvent<UniqueActionEvent>(holder.Comp.SupercedingAttachable.Value, new UniqueActionEvent(args.UserUid), false);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnHolderWielded(Entity<AttachableHolderComponent> holder, ref ItemWieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		AlterAllAttachables(holder, AttachableAlteredType.Wielded);
	}

	private void OnHolderUnwielded(Entity<AttachableHolderComponent> holder, ref ItemUnwieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		AlterAllAttachables(holder, AttachableAlteredType.Unwielded);
	}

	private void OnAttachableHolderDetachMessage(EntityUid holderUid, AttachableHolderComponent holderComponent, AttachableHolderDetachMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		StartDetach(Entity<AttachableHolderComponent>.op_Implicit((holderUid, holderComponent)), args.Slot, ((BaseBoundUserInterfaceEvent)args).Actor);
	}

	private void OnAttachableHolderGetVerbs(Entity<AttachableHolderComponent> holder, ref GetVerbsEvent<EquipmentVerb> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.User))
		{
			return;
		}
		EnsureSlots(holder);
		EntityUid userUid = args.User;
		BaseContainer container = default(BaseContainer);
		AttachableToggleableComponent toggleableComponent = default(AttachableToggleableComponent);
		TransformComponent transformComponent = default(TransformComponent);
		foreach (string slotId in holder.Comp.Slots.Keys)
		{
			if (!_container.TryGetContainer(holder.Owner, slotId, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid contained in container.ContainedEntities)
			{
				if (!((EntitySystem)this).TryComp<AttachableToggleableComponent>(contained, ref toggleableComponent))
				{
					continue;
				}
				if (toggleableComponent.UserOnly)
				{
					if (!((EntitySystem)this).TryComp(holder.Owner, ref transformComponent))
					{
						continue;
					}
					EntityUid parentUid = transformComponent.ParentUid;
					if (!((EntityUid)(ref parentUid)).Valid || transformComponent.ParentUid != userUid)
					{
						continue;
					}
				}
				EquipmentVerb verb = new EquipmentVerb
				{
					Text = toggleableComponent.ActionName,
					IconEntity = ((EntitySystem)this).GetNetEntity(contained, (MetaDataComponent)null),
					Act = delegate
					{
						//IL_0012: Unknown result type (might be due to invalid IL or missing references)
						//IL_0022: Unknown result type (might be due to invalid IL or missing references)
						//IL_0048: Unknown result type (might be due to invalid IL or missing references)
						AttachableToggleStartedEvent attachableToggleStartedEvent = new AttachableToggleStartedEvent(holder.Owner, userUid, slotId);
						((EntitySystem)this).RaiseLocalEvent<AttachableToggleStartedEvent>(contained, ref attachableToggleStartedEvent, false);
					}
				};
				args.Verbs.Add(verb);
			}
		}
	}

	private void OnAttachableHolderAttachToSlotMessage(EntityUid holderUid, AttachableHolderComponent holderComponent, AttachableHolderAttachToSlotMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComponent = default(HandsComponent);
		((EntitySystem)this).TryComp<HandsComponent>(((BaseBoundUserInterfaceEvent)args).Actor, ref handsComponent);
		if (handsComponent != null)
		{
			_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((((BaseBoundUserInterfaceEvent)args).Actor, handsComponent)), out var attachableUid);
			if (attachableUid.HasValue)
			{
				StartAttach(Entity<AttachableHolderComponent>.op_Implicit((holderUid, holderComponent)), attachableUid.Value, ((BaseBoundUserInterfaceEvent)args).Actor, args.Slot);
			}
		}
	}

	private void OnAttachableHolderUiOpened(EntityUid holderUid, AttachableHolderComponent holderComponent, BoundUIOpenedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateStripUi(holderUid);
	}

	public void StartAttach(Entity<AttachableHolderComponent> holder, EntityUid attachableUid, EntityUid userUid, string slotId = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(userUid))
		{
			return;
		}
		List<string> validSlots = GetValidSlots(holder, attachableUid);
		if (validSlots.Count == 0)
		{
			return;
		}
		if (string.IsNullOrEmpty(slotId))
		{
			if (validSlots.Count > 1)
			{
				UserInterfaceComponent userInterfaceComponent = default(UserInterfaceComponent);
				((EntitySystem)this).TryComp<UserInterfaceComponent>(holder.Owner, ref userInterfaceComponent);
				_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit((holder.Owner, userInterfaceComponent)), (Enum)AttachmentUI.ChooseSlotKey, (EntityUid?)userUid, false);
				AttachableHolderChooseSlotUserInterfaceState state = new AttachableHolderChooseSlotUserInterfaceState(validSlots);
				_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(holder.Owner), (Enum)AttachmentUI.ChooseSlotKey, (BoundUserInterfaceState)(object)state);
				return;
			}
			slotId = validSlots[0];
		}
		_doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, userUid, ((EntitySystem)this).Comp<AttachableComponent>(attachableUid).AttachDoAfter, new AttachableAttachDoAfterEvent(slotId), Entity<AttachableHolderComponent>.op_Implicit(holder), holder.Owner, attachableUid)
		{
			NeedHand = true,
			BreakOnMove = true
		});
	}

	private void OnAttachDoAfter(EntityUid uid, AttachableHolderComponent component, AttachableAttachDoAfterEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		target = args.Used;
		if (target.HasValue)
		{
			EntityUid used = target.GetValueOrDefault();
			AttachableHolderComponent holder = default(AttachableHolderComponent);
			if (((EntitySystem)this).TryComp<AttachableHolderComponent>(args.Target, ref holder) && ((EntitySystem)this).HasComp<AttachableComponent>(args.Used) && Attach(Entity<AttachableHolderComponent>.op_Implicit((target2, holder)), used, args.User, args.SlotId))
			{
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	public bool Attach(Entity<AttachableHolderComponent> holder, EntityUid attachableUid, EntityUid userUid, string slotId = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!CanAttach(holder, attachableUid, ref slotId))
		{
			return false;
		}
		ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<AttachableHolderComponent>.op_Implicit(holder), slotId, (ContainerManagerComponent)null);
		((BaseContainer)container).OccludesLight = false;
		if (((BaseContainer)container).Count > 0 && !Detach(holder, ((BaseContainer)container).ContainedEntities[0], userUid, slotId))
		{
			return false;
		}
		if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(attachableUid), (BaseContainer)(object)container, (TransformComponent)null, false))
		{
			return false;
		}
		if (_hands.IsHolding(Entity<HandsComponent>.op_Implicit(userUid), holder.Owner))
		{
			GrantAttachableActionsEvent addEv = new GrantAttachableActionsEvent(userUid);
			((EntitySystem)this).RaiseLocalEvent<GrantAttachableActionsEvent>(attachableUid, ref addEv, false);
		}
		((EntitySystem)this).Dirty<AttachableHolderComponent>(holder, (MetaDataComponent)null);
		_audio.PlayPredicted(((EntitySystem)this).Comp<AttachableComponent>(attachableUid).AttachSound, Entity<AttachableHolderComponent>.op_Implicit(holder), (EntityUid?)userUid, (AudioParams?)null);
		return true;
	}

	private void OnAttached(Entity<AttachableHolderComponent> holder, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		AttachableComponent attachableComponent = default(AttachableComponent);
		if (((EntitySystem)this).TryComp<AttachableComponent>(((ContainerModifiedMessage)args).Entity, ref attachableComponent) && holder.Comp.Slots.ContainsKey(((ContainerModifiedMessage)args).Container.ID))
		{
			UpdateStripUi(holder.Owner, holder.Comp);
			AttachableAlteredEvent ev = new AttachableAlteredEvent(holder.Owner, AttachableAlteredType.Attached);
			((EntitySystem)this).RaiseLocalEvent<AttachableAlteredEvent>(((ContainerModifiedMessage)args).Entity, ref ev, false);
			AttachableHolderAttachablesAlteredEvent holderEv = new AttachableHolderAttachablesAlteredEvent(((ContainerModifiedMessage)args).Entity, ((ContainerModifiedMessage)args).Container.ID, AttachableAlteredType.Attached);
			((EntitySystem)this).RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(Entity<AttachableHolderComponent>.op_Implicit(holder), ref holderEv, false);
		}
	}

	public void StartDetach(Entity<AttachableHolderComponent> holder, string slotId, EntityUid userUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetAttachable(holder, slotId, out Entity<AttachableComponent> attachable) && holder.Comp.Slots.ContainsKey(slotId) && !holder.Comp.Slots[slotId].Locked)
		{
			StartDetach(holder, attachable.Owner, userUid);
		}
	}

	public void StartDetach(Entity<AttachableHolderComponent> holder, EntityUid attachableUid, EntityUid userUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(userUid))
		{
			float delay = ((EntitySystem)this).Comp<AttachableComponent>(attachableUid).AttachDoAfter;
			DoAfterArgs args = new DoAfterArgs((IEntityManager)(object)base.EntityManager, userUid, delay, new AttachableDetachDoAfterEvent(), Entity<AttachableHolderComponent>.op_Implicit(holder), holder.Owner, attachableUid)
			{
				NeedHand = true,
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(args);
		}
	}

	private void OnDetachDoAfter(EntityUid uid, AttachableHolderComponent component, AttachableDetachDoAfterEvent args)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		AttachableHolderComponent holderComponent = default(AttachableHolderComponent);
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && args.Target.HasValue && args.Used.HasValue && ((EntitySystem)this).TryComp<AttachableHolderComponent>(args.Target, ref holderComponent) && ((EntitySystem)this).HasComp<AttachableComponent>(args.Used) && Detach(Entity<AttachableHolderComponent>.op_Implicit((args.Target.Value, holderComponent)), args.Used.Value, args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	public bool Detach(Entity<AttachableHolderComponent> holder, EntityUid attachableUid, EntityUid userUid, string? slotId = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).TerminatingOrDeleted(Entity<AttachableHolderComponent>.op_Implicit(holder), (MetaDataComponent)null) || !((Component)holder.Comp).Running)
		{
			return false;
		}
		if (string.IsNullOrEmpty(slotId) && !TryGetSlotId(holder.Owner, attachableUid, out slotId))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<AttachableHolderComponent>.op_Implicit(holder), slotId, ref container, (ContainerManagerComponent)null) || container.Count <= 0)
		{
			return false;
		}
		if (!TryGetAttachable(holder, slotId, out Entity<AttachableComponent> attachable))
		{
			return false;
		}
		if (!_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(attachable.Owner), container, true, true, (EntityCoordinates?)null, (Angle?)null))
		{
			return false;
		}
		UpdateStripUi(holder.Owner, holder.Comp);
		AttachableAlteredEvent ev = new AttachableAlteredEvent(holder.Owner, AttachableAlteredType.Detached, userUid);
		((EntitySystem)this).RaiseLocalEvent<AttachableAlteredEvent>(attachableUid, ref ev, false);
		AttachableHolderAttachablesAlteredEvent holderEv = new AttachableHolderAttachablesAlteredEvent(attachableUid, slotId, AttachableAlteredType.Detached);
		((EntitySystem)this).RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(holder.Owner, ref holderEv, false);
		RemoveAttachableActionsEvent removeEv = new RemoveAttachableActionsEvent(userUid);
		((EntitySystem)this).RaiseLocalEvent<RemoveAttachableActionsEvent>(attachableUid, ref removeEv, false);
		_audio.PlayPredicted(((EntitySystem)this).Comp<AttachableComponent>(attachableUid).DetachSound, Entity<AttachableHolderComponent>.op_Implicit(holder), (EntityUid?)userUid, (AudioParams?)null);
		((EntitySystem)this).Dirty<AttachableHolderComponent>(holder, (MetaDataComponent)null);
		_hands.TryPickupAnyHand(userUid, Entity<AttachableComponent>.op_Implicit(attachable));
		return true;
	}

	private bool CanAttach(Entity<AttachableHolderComponent> holder, EntityUid attachableUid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		string slotId = "";
		return CanAttach(holder, attachableUid, ref slotId);
	}

	private bool CanAttach(Entity<AttachableHolderComponent> holder, EntityUid attachableUid, ref string slotId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<AttachableComponent>(attachableUid))
		{
			return false;
		}
		if (!string.IsNullOrWhiteSpace(slotId))
		{
			return _whitelist.IsWhitelistPass(holder.Comp.Slots[slotId].Whitelist, attachableUid);
		}
		foreach (string key in holder.Comp.Slots.Keys)
		{
			if (_whitelist.IsWhitelistPass(holder.Comp.Slots[key].Whitelist, attachableUid))
			{
				slotId = key;
				return true;
			}
		}
		return false;
	}

	private Dictionary<string, (string?, bool)> GetSlotsForStripUi(Entity<AttachableHolderComponent> holder)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, (string, bool)> result = new Dictionary<string, (string, bool)>();
		EntityQuery<MetaDataComponent> metaQuery = ((EntitySystem)this).GetEntityQuery<MetaDataComponent>();
		MetaDataComponent metadata = default(MetaDataComponent);
		foreach (string slotId in holder.Comp.Slots.Keys)
		{
			if (TryGetAttachable(holder, slotId, out Entity<AttachableComponent> attachable) && metaQuery.TryGetComponent(attachable.Owner, ref metadata))
			{
				result.Add(slotId, (metadata.EntityName, holder.Comp.Slots[slotId].Locked));
			}
			else
			{
				result.Add(slotId, (null, holder.Comp.Slots[slotId].Locked));
			}
		}
		return result;
	}

	public bool TryGetAttachable(Entity<AttachableHolderComponent> holder, string slotId, out Entity<AttachableComponent> attachable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		attachable = default(Entity<AttachableComponent>);
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<AttachableHolderComponent>.op_Implicit(holder), slotId, ref container, (ContainerManagerComponent)null) || container.Count <= 0)
		{
			return false;
		}
		EntityUid ent = container.ContainedEntities[0];
		AttachableComponent attachableComp = default(AttachableComponent);
		if (!((EntitySystem)this).TryComp<AttachableComponent>(ent, ref attachableComp))
		{
			return false;
		}
		attachable = Entity<AttachableComponent>.op_Implicit((ent, attachableComp));
		return true;
	}

	private void UpdateStripUi(EntityUid holderUid, AttachableHolderComponent? holderComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AttachableHolderComponent>(holderUid, ref holderComponent, true))
		{
			AttachableHolderStripUserInterfaceState state = new AttachableHolderStripUserInterfaceState(GetSlotsForStripUi(Entity<AttachableHolderComponent>.op_Implicit((holderUid, holderComponent))));
			_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(holderUid), (Enum)AttachmentUI.StripKey, (BoundUserInterfaceState)(object)state);
		}
	}

	private void EnsureSlots(Entity<AttachableHolderComponent> holder)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		foreach (string slotId in holder.Comp.Slots.Keys)
		{
			((BaseContainer)_container.EnsureContainer<ContainerSlot>(Entity<AttachableHolderComponent>.op_Implicit(holder), slotId, (ContainerManagerComponent)null)).OccludesLight = false;
		}
	}

	private List<string> GetValidSlots(Entity<AttachableHolderComponent> holder, EntityUid attachableUid, bool ignoreLock = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		if (!((EntitySystem)this).HasComp<AttachableComponent>(attachableUid))
		{
			return list;
		}
		foreach (string slotId in holder.Comp.Slots.Keys)
		{
			if (_whitelist.IsWhitelistPass(holder.Comp.Slots[slotId].Whitelist, attachableUid) && (!ignoreLock || !holder.Comp.Slots[slotId].Locked))
			{
				list.Add(slotId);
			}
		}
		return list;
	}

	private void ToggleAttachable(EntityUid userUid, string slotId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(userUid));
		if (!activeItem.HasValue)
		{
			return;
		}
		EntityUid active = activeItem.GetValueOrDefault();
		AttachableHolderComponent holderComponent = default(AttachableHolderComponent);
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).TryComp<AttachableHolderComponent>(active, ref holderComponent) && ((Component)holderComponent).Running && _actionBlocker.CanInteract(userUid, active) && _container.TryGetContainer(active, slotId, ref container, (ContainerManagerComponent)null) && container.Count > 0)
		{
			EntityUid attachableUid = container.ContainedEntities[0];
			if (((EntitySystem)this).HasComp<AttachableToggleableComponent>(attachableUid))
			{
				AttachableToggleStartedEvent ev = new AttachableToggleStartedEvent(active, userUid, slotId);
				((EntitySystem)this).RaiseLocalEvent<AttachableToggleStartedEvent>(attachableUid, ref ev, false);
			}
		}
	}

	private void FieldStripHeldItem(EntityUid userUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(userUid));
		if (!activeItem.HasValue)
		{
			return;
		}
		EntityUid active = activeItem.GetValueOrDefault();
		AttachableHolderComponent holderComponent = default(AttachableHolderComponent);
		if (!((EntitySystem)this).TryComp<AttachableHolderComponent>(active, ref holderComponent) || !((Component)holderComponent).Running || !_actionBlocker.CanInteract(userUid, active))
		{
			return;
		}
		foreach (Verb verb in _verbSystem.GetLocalVerbs(active, userUid, typeof(Verb)))
		{
			if (verb.Text.Equals(base.Loc.GetString("rmc-verb-strip-attachables")))
			{
				_verbSystem.ExecuteVerb(verb, userUid, active);
				break;
			}
		}
	}

	public void SetSupercedingAttachable(Entity<AttachableHolderComponent> holder, EntityUid? supercedingAttachable)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		holder.Comp.SupercedingAttachable = supercedingAttachable;
		((EntitySystem)this).Dirty<AttachableHolderComponent>(holder, (MetaDataComponent)null);
	}

	public bool TryGetInhandSupercedingGun(EntityUid user, out EntityUid attachable, [NotNullWhen(true)] out GunComponent? gunComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		attachable = default(EntityUid);
		EntityUid? activeItem = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(user));
		if (activeItem.HasValue)
		{
			EntityUid active = activeItem.GetValueOrDefault();
			AttachableHolderComponent holderComp = default(AttachableHolderComponent);
			if (((EntitySystem)this).TryComp<AttachableHolderComponent>(active, ref holderComp) && holderComp.SupercedingAttachable.HasValue)
			{
				if (!((EntitySystem)this).TryComp<GunComponent>(holderComp.SupercedingAttachable, ref gunComp))
				{
					return false;
				}
				attachable = holderComp.SupercedingAttachable.Value;
				return true;
			}
		}
		gunComp = null;
		return false;
	}

	public bool TryGetSlotId(EntityUid holderUid, EntityUid attachableUid, [NotNullWhen(true)] out string? slotId)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		slotId = null;
		AttachableHolderComponent holderComponent = default(AttachableHolderComponent);
		AttachableComponent attachableComponent = default(AttachableComponent);
		if (!((EntitySystem)this).TryComp<AttachableHolderComponent>(holderUid, ref holderComponent) || !((EntitySystem)this).TryComp<AttachableComponent>(attachableUid, ref attachableComponent))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		foreach (string id in holderComponent.Slots.Keys)
		{
			if (_container.TryGetContainer(holderUid, id, ref container, (ContainerManagerComponent)null) && container.Count > 0 && !(container.ContainedEntities[0] != attachableUid))
			{
				slotId = id;
				return true;
			}
		}
		return false;
	}

	public bool HasSlot(Entity<AttachableHolderComponent?> holder, string slotId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (holder.Comp == null)
		{
			AttachableHolderComponent holderComponent = default(AttachableHolderComponent);
			if (!((EntitySystem)this).TryComp<AttachableHolderComponent>(holder.Owner, ref holderComponent))
			{
				return false;
			}
			holder.Comp = holderComponent;
		}
		return holder.Comp.Slots.ContainsKey(slotId);
	}

	public bool TryGetHolder(EntityUid attachable, [NotNullWhen(true)] out EntityUid? holderUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transformComponent = default(TransformComponent);
		if (((EntitySystem)this).TryComp(attachable, ref transformComponent))
		{
			EntityUid parentUid = transformComponent.ParentUid;
			if (((EntityUid)(ref parentUid)).Valid && ((EntitySystem)this).HasComp<AttachableHolderComponent>(transformComponent.ParentUid))
			{
				holderUid = transformComponent.ParentUid;
				return true;
			}
		}
		holderUid = null;
		return false;
	}

	public bool TryGetUser(EntityUid attachable, [NotNullWhen(true)] out EntityUid? userUid)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		userUid = null;
		if (!TryGetHolder(attachable, out var holderUid))
		{
			return false;
		}
		TransformComponent transformComponent = default(TransformComponent);
		if (((EntitySystem)this).TryComp(holderUid, ref transformComponent))
		{
			EntityUid parentUid = transformComponent.ParentUid;
			if (((EntityUid)(ref parentUid)).Valid)
			{
				userUid = transformComponent.ParentUid;
				return true;
			}
		}
		return false;
	}

	public void RelayEvent<T>(Entity<AttachableHolderComponent> holder, ref T args) where T : notnull
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		AttachableRelayedEvent<T> ev = new AttachableRelayedEvent<T>(args, holder.Owner);
		BaseContainer container = default(BaseContainer);
		foreach (string slot in holder.Comp.Slots.Keys)
		{
			if (!_container.TryGetContainer(Entity<AttachableHolderComponent>.op_Implicit(holder), slot, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid contained in container.ContainedEntities)
			{
				((EntitySystem)this).RaiseLocalEvent<AttachableRelayedEvent<T>>(contained, ev, false);
			}
		}
		args = ev.Args;
	}

	private void AlterAllAttachables(Entity<AttachableHolderComponent> holder, AttachableAlteredType alteration)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		foreach (string slotId in holder.Comp.Slots.Keys)
		{
			if (_container.TryGetContainer(Entity<AttachableHolderComponent>.op_Implicit(holder), slotId, ref container, (ContainerManagerComponent)null) && container.Count > 0)
			{
				AttachableAlteredEvent ev = new AttachableAlteredEvent(holder.Owner, alteration);
				((EntitySystem)this).RaiseLocalEvent<AttachableAlteredEvent>(container.ContainedEntities[0], ref ev, false);
			}
		}
	}
}
