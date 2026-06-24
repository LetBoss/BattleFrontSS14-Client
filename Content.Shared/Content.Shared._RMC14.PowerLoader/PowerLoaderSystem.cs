using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.ElectronicSystem;
using Content.Shared._RMC14.Dropship.Fabricator;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.PowerLoader.Events;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.PowerLoader;

public sealed class PowerLoaderSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedBuckleSystem _buckle;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedDropshipSystem _dropship;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedMoverController _mover;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedVirtualItemSystem _virtualItem;

	[Dependency]
	private TagSystem _tag;

	private static readonly EntProtoId DefaultHandVisual = EntProtoId.op_Implicit("RMCVirtualDropshipGearRight");

	private EntityQuery<PowerLoaderGrabbableComponent> _powerLoaderGrabbableQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_powerLoaderGrabbableQuery = ((EntitySystem)this).GetEntityQuery<PowerLoaderGrabbableComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ItemComponent, AfterInteractEvent>((EntityEventRefHandler<ItemComponent, AfterInteractEvent>)OnItemAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, MapInitEvent>((EntityEventRefHandler<PowerLoaderComponent, MapInitEvent>)OnPowerLoaderMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, ComponentRemove>((EntityEventRefHandler<PowerLoaderComponent, ComponentRemove>)OnPowerLoaderRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, EntityTerminatingEvent>((EntityEventRefHandler<PowerLoaderComponent, EntityTerminatingEvent>)OnPowerLoaderTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<PowerLoaderComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, StrapAttemptEvent>((EntityEventRefHandler<PowerLoaderComponent, StrapAttemptEvent>)OnStrapAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, StrappedEvent>((EntityEventRefHandler<PowerLoaderComponent, StrappedEvent>)OnStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, UnstrappedEvent>((EntityEventRefHandler<PowerLoaderComponent, UnstrappedEvent>)OnUnstrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, PowerLoaderGrabDoAfterEvent>((EntityEventRefHandler<PowerLoaderComponent, PowerLoaderGrabDoAfterEvent>)OnGrabDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, GetUsedEntityEvent>((EntityEventRefHandler<PowerLoaderComponent, GetUsedEntityEvent>)OnGetUsedEntity, (Type[])null, new Type[1] { typeof(SharedHandsSystem) });
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, UserActivateInWorldEvent>((EntityEventRefHandler<PowerLoaderComponent, UserActivateInWorldEvent>)OnUserGrab, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, DestructionEventArgs>((EntityEventRefHandler<PowerLoaderComponent, DestructionEventArgs>)OnDestruction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, DidEquipHandEvent>((EntityEventRefHandler<PowerLoaderComponent, DidEquipHandEvent>)OnHandsChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, DidUnequipHandEvent>((EntityEventRefHandler<PowerLoaderComponent, DidUnequipHandEvent>)OnHandsChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderGrabbableComponent, PickupAttemptEvent>((EntityEventRefHandler<PowerLoaderGrabbableComponent, PickupAttemptEvent>)OnGrabbablePickupAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderGrabbableComponent, AfterInteractEvent>((EntityEventRefHandler<PowerLoaderGrabbableComponent, AfterInteractEvent>)OnGrabbableAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderGrabbableComponent, CombatModeShouldHandInteractEvent>((EntityEventRefHandler<PowerLoaderGrabbableComponent, CombatModeShouldHandInteractEvent>)OnGrababbleShouldInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderGrabbableComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<PowerLoaderGrabbableComponent, BeforeRangedInteractEvent>)OnGrabbableBeforeRangedInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipWeaponPointComponent, ActivateInWorldEvent>((EntityEventRefHandler<DropshipWeaponPointComponent, ActivateInWorldEvent>)OnPointActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipUtilityPointComponent, ActivateInWorldEvent>((EntityEventRefHandler<DropshipUtilityPointComponent, ActivateInWorldEvent>)OnPointActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipEnginePointComponent, ActivateInWorldEvent>((EntityEventRefHandler<DropshipEnginePointComponent, ActivateInWorldEvent>)OnEngineActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, ActivateInWorldEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, ActivateInWorldEvent>)OnEngineActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipWeaponPointComponent, DropshipDetachDoAfterEvent>((EntityEventRefHandler<DropshipWeaponPointComponent, DropshipDetachDoAfterEvent>)OnDropshipDetach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipUtilityPointComponent, DropshipDetachDoAfterEvent>((EntityEventRefHandler<DropshipUtilityPointComponent, DropshipDetachDoAfterEvent>)OnDropshipDetach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipEnginePointComponent, DropshipDetachDoAfterEvent>((EntityEventRefHandler<DropshipEnginePointComponent, DropshipDetachDoAfterEvent>)OnEngineDetach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, DropshipDetachDoAfterEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, DropshipDetachDoAfterEvent>)OnEngineDetach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipWeaponPointComponent, GetAttachmentSlotEvent>((EntityEventRefHandler<DropshipWeaponPointComponent, GetAttachmentSlotEvent>)OnGetSlot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipUtilityPointComponent, GetAttachmentSlotEvent>((EntityEventRefHandler<DropshipUtilityPointComponent, GetAttachmentSlotEvent>)OnGetSlot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipEnginePointComponent, GetAttachmentSlotEvent>((EntityEventRefHandler<DropshipEnginePointComponent, GetAttachmentSlotEvent>)OnGetSlot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, GetAttachmentSlotEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, GetAttachmentSlotEvent>)OnGetSlot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipWeaponPointComponent, DropshipAttachDoAfterEvent>((EntityEventRefHandler<DropshipWeaponPointComponent, DropshipAttachDoAfterEvent>)OnDropshipAttach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipUtilityPointComponent, DropshipAttachDoAfterEvent>((EntityEventRefHandler<DropshipUtilityPointComponent, DropshipAttachDoAfterEvent>)OnDropshipAttach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipEnginePointComponent, DropshipAttachDoAfterEvent>((EntityEventRefHandler<DropshipEnginePointComponent, DropshipAttachDoAfterEvent>)OnDropshipAttach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, DropshipAttachDoAfterEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, DropshipAttachDoAfterEvent>)OnDropshipAttach, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipFabricatorPrintableComponent, PowerLoaderInteractEvent>((EntityEventRefHandler<DropshipFabricatorPrintableComponent, PowerLoaderInteractEvent>)OnDropshipPartPowerLoaderInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivePowerLoaderPilotComponent, PreventCollideEvent>((EntityEventRefHandler<ActivePowerLoaderPilotComponent, PreventCollideEvent>)OnActivePilotPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivePowerLoaderPilotComponent, KnockedDownEvent>((EntityEventRefHandler<ActivePowerLoaderPilotComponent, KnockedDownEvent>)OnActivePilotStunned, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivePowerLoaderPilotComponent, StunnedEvent>((EntityEventRefHandler<ActivePowerLoaderPilotComponent, StunnedEvent>)OnActivePilotStunned, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivePowerLoaderPilotComponent, MobStateChangedEvent>((EntityEventRefHandler<ActivePowerLoaderPilotComponent, MobStateChangedEvent>)OnActivePilotMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipWeaponPointComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<DropshipWeaponPointComponent, EntRemovedFromContainerMessage>)OnWeaponPointContainerChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipUtilityPointComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<DropshipUtilityPointComponent, EntRemovedFromContainerMessage>)OnUtilityPointContainerChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipEnginePointComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<DropshipEnginePointComponent, EntRemovedFromContainerMessage>)OnEnginePointContainerChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, EntRemovedFromContainerMessage>)OnElectronicPointContainerChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActivePowerLoaderPilotComponent, CatchAttemptEvent>((EntityEventRefHandler<ActivePowerLoaderPilotComponent, CatchAttemptEvent>)OnPowerLoaderPilotCatchAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerLoaderComponent, BeforeMeltedEvent>((EntityEventRefHandler<PowerLoaderComponent, BeforeMeltedEvent>)PowerLoaderBeforeMelted, (Type[])null, (Type[])null);
	}

	private void OnItemAfterInteract(Entity<ItemComponent> ent, ref AfterInteractEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		PowerLoaderComponent loader = default(PowerLoaderComponent);
		HandsComponent hands = default(HandsComponent);
		if (!((HandledEntityEventArgs)args).Handled && !_powerLoaderGrabbableQuery.HasComp(Entity<ItemComponent>.op_Implicit(ent)) && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(ent.Owner), ref container) && ((EntitySystem)this).TryComp<PowerLoaderComponent>(container.Owner, ref loader) && ((EntitySystem)this).TryComp<HandsComponent>(container.Owner, ref hands) && !_hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit((container.Owner, hands))).All((EntityUid held) => held != ent.Owner) && TryDropLoaderHeld(Entity<PowerLoaderComponent>.op_Implicit((container.Owner, loader)), args.ClickLocation, args.Used))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnPowerLoaderMapInit(Entity<PowerLoaderComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_container.EnsureContainer<Container>(Entity<PowerLoaderComponent>.op_Implicit(ent), ent.Comp.VirtualContainerId, (ContainerManagerComponent)null);
	}

	private void OnPowerLoaderRemove(Entity<PowerLoaderComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemoveLoader(ent);
	}

	private void OnPowerLoaderTerminating(Entity<PowerLoaderComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemoveLoader(ent);
	}

	private void OnRefreshSpeed(Entity<PowerLoaderComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent strap = default(StrapComponent);
		if (!((EntitySystem)this).TryComp<StrapComponent>(Entity<PowerLoaderComponent>.op_Implicit(ent), ref strap))
		{
			return;
		}
		int highestSkill = 0;
		foreach (EntityUid buckled in strap.BuckledEntities)
		{
			int skill = _skills.GetSkill(Entity<SkillsComponent>.op_Implicit(buckled), ent.Comp.SpeedSkill);
			if (skill > highestSkill)
			{
				highestSkill = skill;
			}
		}
		if (highestSkill > 0)
		{
			float speed = ent.Comp.SpeedPerSkill * (float)highestSkill;
			args.ModifySpeed(speed, speed);
		}
	}

	private void OnStrapAttempt(Entity<PowerLoaderComponent> ent, ref StrapAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		Entity<BuckleComponent> buckle = args.Buckle;
		if (!_skills.HasSkills(Entity<SkillsComponent>.op_Implicit(buckle.Owner), ent.Comp.Skills))
		{
			if (args.Popup)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-skills-cant-operate", (ValueTuple<string, object>)("target", ent)), Entity<BuckleComponent>.op_Implicit(buckle), args.User);
			}
			args.Cancelled = true;
		}
		else if (_hands.CountFreeHands(Entity<HandsComponent>.op_Implicit(buckle.Owner)) < 2)
		{
			if (args.Popup)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-power-loader-hands-occupied", (ValueTuple<string, object>)("mech", ent)), Entity<BuckleComponent>.op_Implicit(buckle), args.User);
			}
			args.Cancelled = true;
		}
	}

	private void OnStrapped(Entity<PowerLoaderComponent> ent, ref StrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		Entity<BuckleComponent> buckle = args.Buckle;
		InteractionRelayComponent relay = ((EntitySystem)this).EnsureComp<InteractionRelayComponent>(Entity<BuckleComponent>.op_Implicit(buckle));
		((EntitySystem)this).EnsureComp<ActivePowerLoaderPilotComponent>(Entity<BuckleComponent>.op_Implicit(buckle));
		_mover.SetRelay(Entity<BuckleComponent>.op_Implicit(buckle), Entity<PowerLoaderComponent>.op_Implicit(ent));
		_interaction.SetRelay(Entity<BuckleComponent>.op_Implicit(buckle), Entity<PowerLoaderComponent>.op_Implicit(ent), relay);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<PowerLoaderComponent>.op_Implicit(ent));
		SyncHands(ent);
	}

	private void OnUnstrapped(Entity<PowerLoaderComponent> ent, ref UnstrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		Entity<BuckleComponent> buckle = args.Buckle;
		((EntitySystem)this).RemCompDeferred<ActivePowerLoaderPilotComponent>(Entity<BuckleComponent>.op_Implicit(buckle));
		((EntitySystem)this).RemCompDeferred<RelayInputMoverComponent>(Entity<BuckleComponent>.op_Implicit(buckle));
		((EntitySystem)this).RemCompDeferred<InteractionRelayComponent>(Entity<BuckleComponent>.op_Implicit(buckle));
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<PowerLoaderComponent>.op_Implicit(ent));
		DeleteVirtuals(ent, Entity<BuckleComponent>.op_Implicit(buckle));
		if (ent.Comp.DoAfter != null && _doAfter.IsRunning(ent.Comp.DoAfter.Id))
		{
			_doAfter.Cancel(ent.Comp.DoAfter.Id);
		}
	}

	private void OnGrabDoAfter(Entity<PowerLoaderComponent> ent, ref PowerLoaderGrabDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				PickUp(ent, target2);
			}
		}
	}

	private void OnGetUsedEntity(Entity<PowerLoaderComponent> ent, ref GetUsedEntityEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		args.Used = null;
		HandsComponent hands = default(HandsComponent);
		VirtualItemComponent virtualItem = default(VirtualItemComponent);
		PowerLoaderVirtualItemComponent item = default(PowerLoaderVirtualItemComponent);
		foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(ent)))
		{
			if (!((EntitySystem)this).TryComp<HandsComponent>(buckled, ref hands) || !_hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((buckled, hands)), out var held) || !((EntitySystem)this).TryComp<VirtualItemComponent>(held, ref virtualItem) || !((EntitySystem)this).TryComp<PowerLoaderVirtualItemComponent>(virtualItem.BlockingEntity, ref item))
			{
				continue;
			}
			foreach (EntityUid item2 in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(ent.Owner)))
			{
				EntityUid? grabbed = item.Grabbed;
				if (grabbed.HasValue && item2 == grabbed.GetValueOrDefault())
				{
					args.Used = item.Grabbed;
					return;
				}
			}
			args.Used = null;
			break;
		}
	}

	private void OnUserGrab(Entity<PowerLoaderComponent> ent, ref UserActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent strap = default(StrapComponent);
		if (!((EntitySystem)this).TryComp<StrapComponent>(Entity<PowerLoaderComponent>.op_Implicit(ent), ref strap))
		{
			return;
		}
		PowerLoaderGrabEvent grab = new PowerLoaderGrabEvent(Entity<PowerLoaderComponent>.op_Implicit(ent), args.Target, strap.BuckledEntities);
		((EntitySystem)this).RaiseLocalEvent<PowerLoaderGrabEvent>(args.Target, ref grab, false);
		TimeSpan delay;
		if (grab.ToGrab.HasValue)
		{
			PickUp(ent, grab.ToGrab.Value);
		}
		else if (CanPickupPopup(ent, Entity<PowerLoaderGrabbableComponent>.op_Implicit(args.Target), out delay))
		{
			PowerLoaderGrabDoAfterEvent ev = new PowerLoaderGrabDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<PowerLoaderComponent>.op_Implicit(ent), delay, ev, Entity<PowerLoaderComponent>.op_Implicit(ent), args.Target)
			{
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameEvent,
				DistanceThreshold = 2.5f
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				ent.Comp.DoAfter = ev.DoAfter;
			}
		}
	}

	private void OnDestruction(Entity<PowerLoaderComponent> ent, ref DestructionEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid item in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(ent.Owner)).ToList())
		{
			_hands.TryDrop(Entity<HandsComponent>.op_Implicit(ent.Owner), item);
		}
	}

	private void OnHandsChanged<T>(Entity<PowerLoaderComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SyncHands(ent);
	}

	private void OnPointActivateInWorld(Entity<DropshipWeaponPointComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		PowerLoaderComponent loader = default(PowerLoaderComponent);
		if (!((EntitySystem)this).TryComp<PowerLoaderComponent>(args.User, ref loader))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		Entity<PowerLoaderComponent> user = default(Entity<PowerLoaderComponent>);
		user._002Ector(args.User, loader);
		EntityUid target = args.Target;
		ContainerSlot container;
		if (CanDetachPopup(ref user, Entity<DropshipWeaponPointComponent>.op_Implicit(ent), ent.Comp.AmmoContainerSlotId, doPopup: false, out ContainerSlot ammoSlot) && ammoSlot.ContainedEntity.HasValue)
		{
			container = ammoSlot;
		}
		else
		{
			if (!CanDetachPopup(ref user, Entity<DropshipWeaponPointComponent>.op_Implicit(ent), ent.Comp.WeaponContainerSlotId, doPopup: true, out ContainerSlot weaponSlot) || !weaponSlot.ContainedEntity.HasValue)
			{
				return;
			}
			container = weaponSlot;
		}
		StartPointDetach<DropshipWeaponPointComponent>(ent, container, Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(user), loader)), target);
	}

	private void OnPointActivateInWorld(Entity<DropshipUtilityPointComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TryStartPointDetach<DropshipUtilityPointComponent>(ent, ent.Comp.UtilitySlotId, ref args);
	}

	private void OnEngineActivateInWorld(Entity<DropshipEnginePointComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TryStartPointDetach<DropshipEnginePointComponent>(ent, ent.Comp.ContainerId, ref args);
	}

	private void OnEngineActivateInWorld(Entity<DropshipElectronicSystemPointComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		TryStartPointDetach<DropshipElectronicSystemPointComponent>(ent, ent.Comp.ContainerId, ref args);
	}

	private void OnGrabbablePickupAttempt(Entity<PowerLoaderGrabbableComponent> ent, ref PickupAttemptEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !((EntitySystem)this).HasComp<PowerLoaderComponent>(args.User))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnGrabbableAfterInteract(Entity<PowerLoaderGrabbableComponent> ent, ref AfterInteractEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		TryDropLoaderHeld(Entity<PowerLoaderComponent>.op_Implicit(args.User), args.ClickLocation, args.Used);
	}

	private void OnGetSlot(Entity<DropshipWeaponPointComponent> ent, ref GetAttachmentSlotEvent args)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		Entity<PowerLoaderComponent> user = default(Entity<PowerLoaderComponent>);
		user._002Ector(((EntitySystem)this).GetEntity(args.User), (PowerLoaderComponent)null);
		EntityUid? used = default(EntityUid?);
		if (((EntitySystem)this).TryGetEntity(args.Used, ref used))
		{
			ContainerSlot slot;
			if (args.BeingAttached)
			{
				args.CanUse = CanAttachPopup(ref user, ent, used.Value, out slot);
			}
			else
			{
				args.CanUse = CanDetachPopup(ref user, Entity<DropshipWeaponPointComponent>.op_Implicit(ent), ent.Comp.AmmoContainerSlotId, doPopup: false, out slot) || CanDetachPopup(ref user, Entity<DropshipWeaponPointComponent>.op_Implicit(ent), ent.Comp.WeaponContainerSlotId, doPopup: false, out slot);
			}
			if (slot != null)
			{
				args.SlotId = ((BaseContainer)slot).ID;
			}
		}
	}

	private void OnGetSlot(Entity<DropshipUtilityPointComponent> ent, ref GetAttachmentSlotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TryGetSlot(Entity<DropshipUtilityPointComponent>.op_Implicit(ent), ent.Comp.UtilitySlotId, ref args);
	}

	private void OnGetSlot(Entity<DropshipEnginePointComponent> ent, ref GetAttachmentSlotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TryGetSlot(Entity<DropshipEnginePointComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref args);
	}

	private void OnGetSlot(Entity<DropshipElectronicSystemPointComponent> ent, ref GetAttachmentSlotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TryGetSlot(Entity<DropshipElectronicSystemPointComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref args);
	}

	private void OnDropshipAttach(Entity<DropshipWeaponPointComponent> ent, ref DropshipAttachDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetPointContainer(args, out Entity<PowerLoaderComponent> user, out EntityUid _, out EntityUid contained, out BaseContainer slot))
		{
			InsertPoint(user, contained, slot);
			SyncAppearance(Entity<DropshipWeaponPointComponent>.op_Implicit(ent.Owner));
		}
	}

	private void OnDropshipAttach(Entity<DropshipUtilityPointComponent> ent, ref DropshipAttachDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetPointContainer(args, out Entity<PowerLoaderComponent> user, out EntityUid container, out EntityUid contained, out BaseContainer slot))
		{
			DropshipUtilityComponent utilityComp = default(DropshipUtilityComponent);
			if (((EntitySystem)this).TryComp<DropshipUtilityComponent>(contained, ref utilityComp))
			{
				utilityComp.AttachmentPoint = container;
			}
			InsertPoint(user, contained, slot);
			SyncAppearance(Entity<DropshipUtilityPointComponent>.op_Implicit(ent), ent.Comp.UtilitySlotId);
		}
	}

	private void OnDropshipAttach(Entity<DropshipEnginePointComponent> ent, ref DropshipAttachDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetPointContainer(args, out Entity<PowerLoaderComponent> user, out EntityUid _, out EntityUid contained, out BaseContainer slot))
		{
			InsertPoint(user, contained, slot);
			SyncAppearance(Entity<DropshipEnginePointComponent>.op_Implicit(ent), ent.Comp.ContainerId);
		}
	}

	private void OnDropshipAttach(Entity<DropshipElectronicSystemPointComponent> ent, ref DropshipAttachDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetPointContainer(args, out Entity<PowerLoaderComponent> user, out EntityUid _, out EntityUid contained, out BaseContainer slot))
		{
			InsertPoint(user, contained, slot);
			SyncAppearance(Entity<DropshipElectronicSystemPointComponent>.op_Implicit(ent), ent.Comp.ContainerId);
		}
	}

	private void OnDropshipDetach(Entity<DropshipWeaponPointComponent> ent, ref DropshipDetachDoAfterEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetPointContainer(args, out Entity<PowerLoaderComponent> user, out EntityUid _, out EntityUid contained, out BaseContainer slot))
		{
			return;
		}
		_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(contained), slot, true, false, (EntityCoordinates?)null, (Angle?)null);
		DropshipAmmoComponent ammo = default(DropshipAmmoComponent);
		if (((EntitySystem)this).TryComp<DropshipAmmoComponent>(contained, ref ammo) && ammo.Rounds < ammo.RoundsPerShot)
		{
			((EntitySystem)this).QueueDel((EntityUid?)contained);
			string msg = base.Loc.GetString("rmc-power-loader-discard-empty", (ValueTuple<string, object>)("ammo", contained));
			foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(user)))
			{
				_popup.PopupClient(msg, buckled, PopupType.Medium);
			}
		}
		else
		{
			PickUp(Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(user), user.Comp)), contained);
			SyncHands(Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(user), user.Comp)));
		}
		SyncAppearance(Entity<DropshipWeaponPointComponent>.op_Implicit(ent.Owner));
	}

	private void OnDropshipDetach(Entity<DropshipUtilityPointComponent> ent, ref DropshipDetachDoAfterEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DetachPoint(ref args);
		SyncAppearance(Entity<DropshipUtilityPointComponent>.op_Implicit(ent), ent.Comp.UtilitySlotId);
	}

	private void OnEngineDetach(Entity<DropshipEnginePointComponent> ent, ref DropshipDetachDoAfterEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DetachPoint(ref args);
		SyncAppearance(Entity<DropshipEnginePointComponent>.op_Implicit(ent), ent.Comp.ContainerId);
	}

	private void OnEngineDetach(Entity<DropshipElectronicSystemPointComponent> ent, ref DropshipDetachDoAfterEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DetachPoint(ref args);
		SyncAppearance(Entity<DropshipElectronicSystemPointComponent>.op_Implicit(ent), ent.Comp.ContainerId);
	}

	private void OnGrababbleShouldInteract(Entity<PowerLoaderGrabbableComponent> ent, ref CombatModeShouldHandInteractEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<PowerLoaderComponent>(args.User))
		{
			args.Cancelled = true;
		}
	}

	private void OnGrabbableBeforeRangedInteract(Entity<PowerLoaderGrabbableComponent> ent, ref BeforeRangedInteractEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		((HandledEntityEventArgs)args).Handled = true;
		Entity<PowerLoaderComponent> user = default(Entity<PowerLoaderComponent>);
		user._002Ector(args.User, (PowerLoaderComponent)null);
		EntityUid used = args.Used;
		PowerLoaderInteractEvent powerLoaderEv = new PowerLoaderInteractEvent(args.User, target2, args.Used, GetBuckled(args.User).ToList());
		((EntitySystem)this).RaiseLocalEvent<PowerLoaderInteractEvent>(used, ref powerLoaderEv, false);
		if (powerLoaderEv.Handled)
		{
			return;
		}
		GetAttachmentSlotEvent slotEv = new GetAttachmentSlotEvent(((EntitySystem)this).GetNetEntity(Entity<PowerLoaderComponent>.op_Implicit(user), (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(used, (MetaDataComponent)null));
		((EntitySystem)this).RaiseLocalEvent<GetAttachmentSlotEvent>(target2, slotEv, false);
		if (string.IsNullOrWhiteSpace(slotEv.SlotId))
		{
			return;
		}
		ContainerSlot slot = _container.EnsureContainer<ContainerSlot>(target2, slotEv.SlotId, (ContainerManagerComponent)null);
		PowerLoaderAttachableComponent attachableComponent = default(PowerLoaderAttachableComponent);
		if (slotEv.CanUse && ((EntitySystem)this).TryComp<PowerLoaderAttachableComponent>(used, ref attachableComponent) && _tag.HasAnyTag(target2, attachableComponent.AttachableTypes))
		{
			DropshipAttachDoAfterEvent ev = new DropshipAttachDoAfterEvent(((EntitySystem)this).GetNetEntity(target2, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(used, (MetaDataComponent)null), ((BaseContainer)slot).ID);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<PowerLoaderComponent>.op_Implicit(user), attachableComponent.AttachDelay, ev, target2, target2, used)
			{
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameEvent,
				DistanceThreshold = 2.5f
			};
			PowerLoaderComponent loader = default(PowerLoaderComponent);
			if (_doAfter.TryStartDoAfter(doAfter) && ((EntitySystem)this).TryComp<PowerLoaderComponent>(args.User, ref loader))
			{
				loader.DoAfter = ev.DoAfter;
			}
		}
	}

	private void OnActivePilotPreventCollide(Entity<ActivePowerLoaderPilotComponent> ent, ref PreventCollideEvent args)
	{
		args.Cancelled = true;
	}

	private void OnActivePilotStunned<T>(Entity<ActivePowerLoaderPilotComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemovePilot(ent);
	}

	private void OnActivePilotMobStateChanged(Entity<ActivePowerLoaderPilotComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Critical || args.NewMobState == MobState.Dead)
		{
			OnActivePilotStunned(ent, ref args);
		}
	}

	private void OnDropshipPartPowerLoaderInteract(Entity<DropshipFabricatorPrintableComponent> ent, ref PowerLoaderInteractEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		DropshipFabricatorComponent fabricator = default(DropshipFabricatorComponent);
		if (!args.Handled && ((EntitySystem)this).TryComp<DropshipFabricatorComponent>(args.Target, ref fabricator) && ((EntitySystem)this).HasComp<DropshipFabricatorPointsComponent>(fabricator.Account))
		{
			args.Handled = true;
			float delayMultiplier = 1f;
			MovementRelayTargetComponent relay = default(MovementRelayTargetComponent);
			if (((EntitySystem)this).TryComp<MovementRelayTargetComponent>(args.PowerLoader, ref relay))
			{
				delayMultiplier = _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(relay.Source), ent.Comp.RecycleSkill);
			}
			TimeSpan delay = ent.Comp.Delay * delayMultiplier;
			DropshipFabricatoreRecycleDoafterEvent ev = new DropshipFabricatoreRecycleDoafterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.PowerLoader, delay, ev, args.Target, args.Target, args.Used)
			{
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameEvent,
				DistanceThreshold = 2.5f
			};
			PowerLoaderComponent loader = default(PowerLoaderComponent);
			if (_doAfter.TryStartDoAfter(doAfter) && ((EntitySystem)this).TryComp<PowerLoaderComponent>(args.PowerLoader, ref loader))
			{
				loader.DoAfter = ev.DoAfter;
			}
		}
	}

	private bool CanAttachPopup(ref Entity<PowerLoaderComponent?> user, Entity<DropshipWeaponPointComponent> target, EntityUid used, [NotNullWhen(true)] out ContainerSlot? slot)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		slot = null;
		if (!((EntitySystem)this).Resolve<PowerLoaderComponent>(Entity<PowerLoaderComponent>.op_Implicit(user), ref user.Comp, false))
		{
			return false;
		}
		string slotId;
		string msg;
		if (((EntitySystem)this).HasComp<DropshipWeaponComponent>(used))
		{
			slotId = target.Comp.WeaponContainerSlotId;
			msg = base.Loc.GetString("rmc-power-loader-occupied-weapon");
		}
		else if (((EntitySystem)this).HasComp<DropshipAmmoComponent>(used))
		{
			EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(target.Owner));
			if (grid.HasValue)
			{
				EntityUid grid2 = grid.GetValueOrDefault();
				if (_dropship.IsInFlight(Entity<DropshipComponent>.op_Implicit(grid2)))
				{
					return false;
				}
			}
			slotId = target.Comp.AmmoContainerSlotId;
			msg = base.Loc.GetString("rmc-power-loader-occupied-ammo");
			BaseContainer weaponContainer = default(BaseContainer);
			if (!_container.TryGetContainer(Entity<DropshipWeaponPointComponent>.op_Implicit(target), target.Comp.WeaponContainerSlotId, ref weaponContainer, (ContainerManagerComponent)null) || weaponContainer.ContainedEntities.Count == 0)
			{
				msg = base.Loc.GetString("rmc-power-loader-ammo-no-weapon");
				foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(user)))
				{
					_popup.PopupClient(msg, Entity<DropshipWeaponPointComponent>.op_Implicit(target), buckled, PopupType.SmallCaution);
				}
				return false;
			}
		}
		else
		{
			if (!((EntitySystem)this).HasComp<RMCEquipmentDeployerComponent>(used))
			{
				return false;
			}
			slotId = target.Comp.WeaponContainerSlotId;
			msg = base.Loc.GetString("rmc-power-loader-occupied-deployer");
		}
		slot = _container.EnsureContainer<ContainerSlot>(Entity<DropshipWeaponPointComponent>.op_Implicit(target), slotId, (ContainerManagerComponent)null);
		if (!slot.ContainedEntity.HasValue)
		{
			return true;
		}
		foreach (EntityUid buckled2 in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(user)))
		{
			_popup.PopupClient(msg, Entity<DropshipWeaponPointComponent>.op_Implicit(target), buckled2, PopupType.SmallCaution);
		}
		slot = null;
		return false;
	}

	private void CanAttachPopup(ref Entity<PowerLoaderComponent?> user, EntityUid target, string container, EntityUid used, [NotNullWhen(true)] out ContainerSlot? slot)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		slot = null;
		if (!((EntitySystem)this).Resolve<PowerLoaderComponent>(Entity<PowerLoaderComponent>.op_Implicit(user), ref user.Comp, false) || (!((EntitySystem)this).HasComp<DropshipUtilityComponent>(used) && !((EntitySystem)this).HasComp<DropshipEngineComponent>(used) && !((EntitySystem)this).HasComp<DropshipElectronicSystemComponent>(used)))
		{
			return;
		}
		string msg = base.Loc.GetString("rmc-power-loader-occupied");
		slot = _container.EnsureContainer<ContainerSlot>(target, container, (ContainerManagerComponent)null);
		if (!slot.ContainedEntity.HasValue)
		{
			return;
		}
		foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(user)))
		{
			_popup.PopupClient(msg, target, buckled, PopupType.SmallCaution);
		}
		slot = null;
	}

	private bool CanDetachPopup(ref Entity<PowerLoaderComponent?> user, EntityUid target, string containerId, bool doPopup, [NotNullWhen(true)] out ContainerSlot? slot)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		slot = null;
		if (!((EntitySystem)this).Resolve<PowerLoaderComponent>(Entity<PowerLoaderComponent>.op_Implicit(user), ref user.Comp, false))
		{
			return false;
		}
		if (!HasFreeHands(user))
		{
			if (doPopup)
			{
				string msg = base.Loc.GetString("rmc-power-loader-cant-grab-full", (ValueTuple<string, object>)("mech", user.Owner));
				foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(user)))
				{
					_popup.PopupClient(msg, target, buckled, PopupType.SmallCaution);
				}
			}
			return false;
		}
		BaseContainer utilityContainer = default(BaseContainer);
		if (_container.TryGetContainer(target, containerId, ref utilityContainer, (ContainerManagerComponent)null) && utilityContainer.ContainedEntities.Count > 0)
		{
			slot = (ContainerSlot)utilityContainer;
		}
		if (slot == null)
		{
			if (doPopup)
			{
				foreach (EntityUid buckled2 in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(user)))
				{
					string msg2 = base.Loc.GetString("rmc-power-loader-nothing-attached");
					_popup.PopupClient(msg2, Entity<PowerLoaderComponent>.op_Implicit(user), buckled2, PopupType.SmallCaution);
				}
			}
			return false;
		}
		return true;
	}

	private bool HasFreeHands(Entity<PowerLoaderComponent?> user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return _hands.CountFreeHands(Entity<HandsComponent>.op_Implicit(user.Owner)) > 0;
	}

	private bool CanPickupPopup(Entity<PowerLoaderComponent> loader, Entity<PowerLoaderGrabbableComponent?> grabbable, out TimeSpan delay)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		delay = TimeSpan.Zero;
		if (!((EntitySystem)this).Resolve<PowerLoaderGrabbableComponent>(Entity<PowerLoaderGrabbableComponent>.op_Implicit(grabbable), ref grabbable.Comp, false))
		{
			return false;
		}
		if (!HasFreeHands(Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(loader), Entity<PowerLoaderComponent>.op_Implicit(loader)))))
		{
			string msg = base.Loc.GetString("rmc-power-loader-cant-grab-full", (ValueTuple<string, object>)("mech", loader));
			foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(loader)))
			{
				_popup.PopupClient(msg, buckled, buckled, PopupType.SmallCaution);
			}
		}
		delay = grabbable.Comp.Delay;
		return true;
	}

	private IEnumerable<EntityUid> GetBuckled(EntityUid loader)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent strap = default(StrapComponent);
		if (!((EntitySystem)this).TryComp<StrapComponent>(loader, ref strap))
		{
			yield break;
		}
		foreach (EntityUid buckledEntity in strap.BuckledEntities)
		{
			yield return buckledEntity;
		}
	}

	private void SyncHands(Entity<PowerLoaderComponent> loader)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		Container virtualContainer = _container.EnsureContainer<Container>(Entity<PowerLoaderComponent>.op_Implicit(loader), loader.Comp.VirtualContainerId, (ContainerManagerComponent)null);
		foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(loader)))
		{
			EntityUid[] array = _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(buckled)).ToArray();
			foreach (EntityUid hand in array)
			{
				((EntitySystem)this).Del((EntityUid?)hand);
			}
			array = ((BaseContainer)virtualContainer).ContainedEntities.ToArray();
			foreach (EntityUid virt in array)
			{
				_virtualItem.DeleteInHandsMatching(buckled, virt);
				_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(virt), (BaseContainer)(object)virtualContainer, true, false, (EntityCoordinates?)null, (Angle?)null);
				if (_net.IsServer || ((EntitySystem)this).IsClientSide(virt, (MetaDataComponent)null))
				{
					((EntitySystem)this).Del((EntityUid?)virt);
				}
			}
		}
		List<(EntityUid?, EntProtoId, string, HandLocation)> toSpawn = new List<(EntityUid?, EntProtoId, string, HandLocation)>();
		PowerLoaderGrabbableComponent grabbable = default(PowerLoaderGrabbableComponent);
		foreach (string handId in _hands.EnumerateHands(Entity<HandsComponent>.op_Implicit(loader.Owner)))
		{
			if (!_hands.TryGetHand(Entity<HandsComponent>.op_Implicit(loader.Owner), handId, out var hand2))
			{
				continue;
			}
			if (!_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit(loader.Owner), handId, out var held))
			{
				EntProtoId virtualSide = ((hand2.Value.Location == HandLocation.Right) ? loader.Comp.VirtualRight : loader.Comp.VirtualLeft);
				toSpawn.Add((null, virtualSide, null, hand2.Value.Location));
				continue;
			}
			EntProtoId id = DefaultHandVisual;
			if (_powerLoaderGrabbableQuery.TryComp(held, ref grabbable))
			{
				id = ((hand2.Value.Location == HandLocation.Right) ? grabbable.VirtualRight : grabbable.VirtualLeft);
			}
			string name = ((EntitySystem)this).Name(held.Value, (MetaDataComponent)null);
			toSpawn.Add((held, id, name, hand2.Value.Location));
		}
		EntityUid? virtualEnt = default(EntityUid?);
		string hand3 = default(string);
		foreach (var (grabbed, spawnVirtual, name2, location) in toSpawn)
		{
			if (!((EntitySystem)this).TrySpawnInContainer(EntProtoId.op_Implicit(spawnVirtual), Entity<PowerLoaderComponent>.op_Implicit(loader), loader.Comp.VirtualContainerId, ref virtualEnt, (ContainerManagerComponent)null, (ComponentRegistry)null))
			{
				continue;
			}
			PowerLoaderVirtualItemComponent loaderVirtual = ((EntitySystem)this).EnsureComp<PowerLoaderVirtualItemComponent>(virtualEnt.Value);
			loaderVirtual.Grabbed = grabbed;
			((EntitySystem)this).Dirty(virtualEnt.Value, (IComponent)(object)loaderVirtual, (MetaDataComponent)null);
			if (name2 != null)
			{
				_metaData.SetEntityName(virtualEnt.Value, name2, (MetaDataComponent)null, true);
			}
			foreach (EntityUid buckled2 in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(loader)))
			{
				if (Extensions.TryFirstOrDefault<string>(_hands.EnumerateHands(Entity<HandsComponent>.op_Implicit(buckled2)), (Func<string, bool>)((string h) => _hands.TryGetHand(Entity<HandsComponent>.op_Implicit(buckled2), h, out var hand4) && hand4.Value.Location == location), ref hand3) && _virtualItem.TrySpawnVirtualItemInHand(virtualEnt.Value, buckled2, out var virt2, dropOthers: false, hand3))
				{
					((EntitySystem)this).EnsureComp<UnremoveableComponent>(virt2.Value);
				}
			}
		}
	}

	public void TrySyncHands(Entity<PowerLoaderComponent?> loader)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PowerLoaderComponent>(Entity<PowerLoaderComponent>.op_Implicit(loader), ref loader.Comp, false))
		{
			SyncHands(Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(loader), loader.Comp)));
		}
	}

	private void DeleteVirtuals(Entity<PowerLoaderComponent> loader, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid contained in ((BaseContainer)_container.EnsureContainer<Container>(Entity<PowerLoaderComponent>.op_Implicit(loader), loader.Comp.VirtualContainerId, (ContainerManagerComponent)null)).ContainedEntities)
		{
			_virtualItem.DeleteInHandsMatching(user, contained);
		}
	}

	private void RemoveLoader(Entity<PowerLoaderComponent> loader)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(loader)))
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(buckled, (MetaDataComponent)null))
			{
				DeleteVirtuals(loader, buckled);
			}
		}
	}

	private void PickUp(Entity<PowerLoaderComponent> loader, EntityUid target)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (!CanPickupPopup(loader, Entity<PowerLoaderGrabbableComponent>.op_Implicit(target), out var _))
		{
			return;
		}
		string loaderHand = default(string);
		foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(loader)))
		{
			string activeId = _hands.GetActiveHand(Entity<HandsComponent>.op_Implicit(buckled));
			if (activeId != null && _hands.TryGetHand(Entity<HandsComponent>.op_Implicit(buckled), activeId, out var active) && Extensions.TryFirstOrDefault<string>(_hands.EnumerateHands(Entity<HandsComponent>.op_Implicit(loader.Owner)), (Func<string, bool>)((string h) => _hands.TryGetHand(Entity<HandsComponent>.op_Implicit(loader.Owner), h, out var hand) && hand.Value.Location == active.Value.Location), ref loaderHand))
			{
				_hands.DoPickup(Entity<PowerLoaderComponent>.op_Implicit(loader), loaderHand, target);
				SyncHands(loader);
				break;
			}
		}
	}

	public void SyncAppearance(Entity<DropshipWeaponPointComponent?> point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		if (!((EntitySystem)this).Resolve<DropshipWeaponPointComponent>(Entity<DropshipWeaponPointComponent>.op_Implicit(point), ref point.Comp, false))
		{
			return;
		}
		BaseContainer weaponContainer = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<DropshipWeaponPointComponent>.op_Implicit(point), point.Comp.WeaponContainerSlotId, ref weaponContainer, (ContainerManagerComponent)null) || weaponContainer.ContainedEntities.Count == 0)
		{
			_appearance.SetData(Entity<DropshipWeaponPointComponent>.op_Implicit(point), (Enum)DropshipWeaponVisuals.Sprite, (object)"", (AppearanceComponent)null);
			_appearance.SetData(Entity<DropshipWeaponPointComponent>.op_Implicit(point), (Enum)DropshipWeaponVisuals.State, (object)"", (AppearanceComponent)null);
			return;
		}
		bool hasRounds = false;
		int maxRounds = 0;
		int rounds = 0;
		BaseContainer ammoContainer = default(BaseContainer);
		if (_container.TryGetContainer(Entity<DropshipWeaponPointComponent>.op_Implicit(point), point.Comp.AmmoContainerSlotId, ref ammoContainer, (ContainerManagerComponent)null))
		{
			DropshipAmmoComponent ammo = default(DropshipAmmoComponent);
			foreach (EntityUid contained in ammoContainer.ContainedEntities)
			{
				if (((EntitySystem)this).TryComp<DropshipAmmoComponent>(contained, ref ammo))
				{
					rounds = ammo.Rounds;
					maxRounds = ammo.MaxRounds;
					if (ammo.Rounds >= ammo.RoundsPerShot)
					{
						hasRounds = true;
					}
				}
			}
		}
		DropshipWeaponComponent weapon = default(DropshipWeaponComponent);
		DropshipAttachedSpriteComponent attachedSprite = default(DropshipAttachedSpriteComponent);
		foreach (EntityUid contained2 in weaponContainer.ContainedEntities)
		{
			Rsi rsi = null;
			if (((EntitySystem)this).TryComp<DropshipWeaponComponent>(contained2, ref weapon))
			{
				if (!(rounds > 0 && hasRounds))
				{
					rsi = ((rounds <= 0) ? weapon.WeaponAttachedSprite : weapon.AmmoEmptyAttachedSprite);
				}
				else
				{
					rsi = weapon.AmmoAttachedSprite;
					if (rsi != null && weapon.AmmoAttachedSprite != null && rounds != maxRounds)
					{
						foreach (int ammoCount in weapon.AmmoSpriteThresholds)
						{
							if (ammoCount <= rounds)
							{
								rsi = new Rsi(rsi.RsiPath, weapon.AmmoAttachedSprite.RsiState + "_" + ammoCount);
								break;
							}
						}
					}
				}
			}
			else if (((EntitySystem)this).TryComp<DropshipAttachedSpriteComponent>(contained2, ref attachedSprite))
			{
				rsi = attachedSprite.WeaponSlotSprite;
			}
			if (rsi != null)
			{
				_appearance.SetData(Entity<DropshipWeaponPointComponent>.op_Implicit(point), (Enum)DropshipWeaponVisuals.Sprite, (object)((object)rsi.RsiPath/*cast due to constrained. prefix*/).ToString(), (AppearanceComponent)null);
				_appearance.SetData(Entity<DropshipWeaponPointComponent>.op_Implicit(point), (Enum)DropshipWeaponVisuals.State, (object)rsi.RsiState, (AppearanceComponent)null);
			}
		}
	}

	private void SyncAppearance(EntityUid point, string container)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer utilityContainer = default(BaseContainer);
		if (!_container.TryGetContainer(point, container, ref utilityContainer, (ContainerManagerComponent)null) || utilityContainer.ContainedEntities.Count == 0)
		{
			_appearance.SetData(point, (Enum)DropshipUtilityVisuals.Sprite, (object)"", (AppearanceComponent)null);
			_appearance.SetData(point, (Enum)DropshipUtilityVisuals.State, (object)"", (AppearanceComponent)null);
			return;
		}
		DropshipAttachedSpriteComponent utility = default(DropshipAttachedSpriteComponent);
		foreach (EntityUid contained in utilityContainer.ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<DropshipAttachedSpriteComponent>(contained, ref utility))
			{
				Rsi rsi = utility.Sprite;
				if (rsi != null)
				{
					_appearance.SetData(point, (Enum)DropshipUtilityVisuals.Sprite, (object)((object)rsi.RsiPath/*cast due to constrained. prefix*/).ToString(), (AppearanceComponent)null);
					_appearance.SetData(point, (Enum)DropshipUtilityVisuals.State, (object)rsi.RsiState, (AppearanceComponent)null);
					break;
				}
			}
		}
	}

	private void RemovePilot(Entity<ActivePowerLoaderPilotComponent> active)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		_buckle.Unbuckle(Entity<BuckleComponent>.op_Implicit(active.Owner), null);
		((EntitySystem)this).RemCompDeferred<ActivePowerLoaderPilotComponent>(Entity<ActivePowerLoaderPilotComponent>.op_Implicit(active));
	}

	private bool TryGetPointContainer(DropshipDoAfterEvent args, out Entity<PowerLoaderComponent> user, out EntityUid container, out EntityUid contained, [NotNullWhen(true)] out BaseContainer? slot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		user = default(Entity<PowerLoaderComponent>);
		container = default(EntityUid);
		contained = default(EntityUid);
		slot = null;
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled || !args.Target.HasValue)
		{
			return false;
		}
		PowerLoaderComponent powerLoaderComp = default(PowerLoaderComponent);
		if (!((EntitySystem)this).TryComp<PowerLoaderComponent>(args.User, ref powerLoaderComp))
		{
			return false;
		}
		((HandledEntityEventArgs)args).Handled = true;
		user = new Entity<PowerLoaderComponent>(args.User, powerLoaderComp);
		container = ((EntitySystem)this).GetEntity(args.Container);
		contained = ((EntitySystem)this).GetEntity(args.Contained);
		slot = _container.GetContainer(container, args.Slot, (ContainerManagerComponent)null);
		return true;
	}

	private void InsertPoint(Entity<PowerLoaderComponent> user, EntityUid contained, BaseContainer slot)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (slot.ContainedEntities.Count <= 0)
		{
			DropShipAttachmentInsertedEvent ev = new DropShipAttachmentInsertedEvent(contained);
			((EntitySystem)this).RaiseLocalEvent<DropShipAttachmentInsertedEvent>(slot.Owner, ref ev, false);
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(contained), slot, (TransformComponent)null, false);
			SyncHands(Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(user), user.Comp)));
		}
	}

	private void StartPointDetach<T>(Entity<T> ent, ContainerSlot container, Entity<PowerLoaderComponent> user, EntityUid target) where T : IComponent
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		PowerLoaderDetachableComponent detachableComponent = default(PowerLoaderDetachableComponent);
		if (((EntitySystem)this).TryComp<PowerLoaderDetachableComponent>(container.ContainedEntity, ref detachableComponent))
		{
			EntityUid contained = container.ContainedEntity.Value;
			DropshipDetachDoAfterEvent ev = new DropshipDetachDoAfterEvent(((EntitySystem)this).GetNetEntity(Entity<T>.op_Implicit(ent), (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(contained, (MetaDataComponent)null), ((BaseContainer)container).ID);
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<PowerLoaderComponent>.op_Implicit(user), detachableComponent.DetachDelay, ev, target, target)
			{
				BreakOnMove = true,
				DuplicateCondition = DuplicateConditions.SameEvent,
				DistanceThreshold = 2.5f
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				user.Comp.DoAfter = ev.DoAfter;
			}
		}
	}

	private void TryStartPointDetach<T>(Entity<T> ent, string container, ref ActivateInWorldEvent args) where T : IComponent
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		PowerLoaderComponent loader = default(PowerLoaderComponent);
		if (((EntitySystem)this).TryComp<PowerLoaderComponent>(args.User, ref loader))
		{
			((HandledEntityEventArgs)args).Handled = true;
			Entity<PowerLoaderComponent> user = default(Entity<PowerLoaderComponent>);
			user._002Ector(args.User, loader);
			EntityUid target = args.Target;
			if (CanDetachPopup(ref user, Entity<T>.op_Implicit(ent), container, doPopup: true, out ContainerSlot slot) && slot.ContainedEntity.HasValue)
			{
				StartPointDetach<T>(ent, slot, Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(user), loader)), target);
			}
		}
	}

	private void DetachPoint(ref DropshipDetachDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetPointContainer(args, out Entity<PowerLoaderComponent> user, out EntityUid _, out EntityUid contained, out BaseContainer slot))
		{
			DropshipUtilityComponent utilityComp = default(DropshipUtilityComponent);
			if (((EntitySystem)this).TryComp<DropshipUtilityComponent>(contained, ref utilityComp))
			{
				utilityComp.AttachmentPoint = null;
			}
			DropShipAttachmentDetachedEvent ev = new DropShipAttachmentDetachedEvent(contained);
			((EntitySystem)this).RaiseLocalEvent<DropShipAttachmentDetachedEvent>(slot.Owner, ref ev, false);
			_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(contained), slot, true, false, (EntityCoordinates?)null, (Angle?)null);
			PickUp(Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(user), user.Comp)), contained);
			SyncHands(Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(user), user.Comp)));
		}
	}

	private void TryGetSlot(EntityUid ent, string container, ref GetAttachmentSlotEvent args)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Entity<PowerLoaderComponent> user = default(Entity<PowerLoaderComponent>);
		user._002Ector(((EntitySystem)this).GetEntity(args.User), (PowerLoaderComponent)null);
		EntityUid? used = default(EntityUid?);
		if (((EntitySystem)this).TryGetEntity(args.Used, ref used))
		{
			ContainerSlot slot;
			if (args.BeingAttached)
			{
				CanAttachPopup(ref user, ent, container, used.Value, out slot);
			}
			else
			{
				CanDetachPopup(ref user, ent, container, doPopup: true, out slot);
			}
			if (slot != null)
			{
				args.SlotId = ((BaseContainer)slot).ID;
			}
		}
	}

	private void OnWeaponPointContainerChanged(Entity<DropshipWeaponPointComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SyncAppearance(Entity<DropshipWeaponPointComponent>.op_Implicit(ent.Owner));
	}

	private void OnUtilityPointContainerChanged(Entity<DropshipUtilityPointComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (((ContainerModifiedMessage)args).Container.ID == ent.Comp.UtilitySlotId)
		{
			SyncAppearance(Entity<DropshipUtilityPointComponent>.op_Implicit(ent), ent.Comp.UtilitySlotId);
		}
	}

	private void OnEnginePointContainerChanged(Entity<DropshipEnginePointComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (((ContainerModifiedMessage)args).Container.ID == ent.Comp.ContainerId)
		{
			SyncAppearance(Entity<DropshipEnginePointComponent>.op_Implicit(ent), ent.Comp.ContainerId);
		}
	}

	private void OnElectronicPointContainerChanged(Entity<DropshipElectronicSystemPointComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (((ContainerModifiedMessage)args).Container.ID == ent.Comp.ContainerId)
		{
			SyncAppearance(Entity<DropshipElectronicSystemPointComponent>.op_Implicit(ent), ent.Comp.ContainerId);
		}
	}

	private void OnPowerLoaderPilotCatchAttempt(Entity<ActivePowerLoaderPilotComponent> pilot, ref CatchAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private void PowerLoaderBeforeMelted(Entity<PowerLoaderComponent> loader, ref BeforeMeltedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid item in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(loader.Owner)).ToList())
		{
			_hands.TryDrop(Entity<HandsComponent>.op_Implicit(loader.Owner), item);
		}
		StrapComponent strap = default(StrapComponent);
		if (((EntitySystem)this).TryComp<StrapComponent>(Entity<PowerLoaderComponent>.op_Implicit(loader), ref strap))
		{
			EntityUid[] array = strap.BuckledEntities.ToArray();
			foreach (EntityUid buckled in array)
			{
				_buckle.Unbuckle(Entity<BuckleComponent>.op_Implicit(buckled), null);
			}
		}
	}

	private bool TryDropLoaderHeld(Entity<PowerLoaderComponent?> loader, EntityCoordinates clickLocation, EntityUid item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PowerLoaderComponent>(Entity<PowerLoaderComponent>.op_Implicit(loader), ref loader.Comp, false))
		{
			return false;
		}
		if (!_hands.IsHolding(Entity<HandsComponent>.op_Implicit(loader.Owner), item))
		{
			return false;
		}
		EntityCoordinates source = loader.Owner.ToCoordinates();
		EntityCoordinates coords = _transform.GetMoverCoordinates(clickLocation);
		coords = coords.SnapToGrid((IEntityManager?)(object)base.EntityManager, _mapManager);
		float distance = default(float);
		if (!((EntityCoordinates)(ref source)).TryDistance((IEntityManager)(object)base.EntityManager, coords, ref distance))
		{
			return false;
		}
		if (distance < 0.5f)
		{
			string msg = base.Loc.GetString("rmc-power-loader-too-close");
			foreach (EntityUid buckled in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(loader)))
			{
				_popup.PopupClient(msg, Entity<PowerLoaderComponent>.op_Implicit(loader), buckled, PopupType.SmallCaution);
			}
			return true;
		}
		if (distance > 1.5f)
		{
			string msg2 = base.Loc.GetString("rmc-power-loader-too-far");
			foreach (EntityUid buckled2 in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(loader)))
			{
				_popup.PopupClient(msg2, Entity<PowerLoaderComponent>.op_Implicit(loader), buckled2, PopupType.SmallCaution);
			}
			return true;
		}
		if (_rmcMap.IsTileBlocked(coords, CollisionGroup.AirlockLayer | CollisionGroup.Impassable) || _rmcMap.TileHasStructure(coords))
		{
			string msg3 = base.Loc.GetString("rmc-power-loader-cant-drop-occupied", (ValueTuple<string, object>)("drop", item));
			foreach (EntityUid buckled3 in GetBuckled(Entity<PowerLoaderComponent>.op_Implicit(loader)))
			{
				_popup.PopupClient(msg3, Entity<PowerLoaderComponent>.op_Implicit(loader), buckled3, PopupType.SmallCaution);
			}
			return true;
		}
		if (_hands.TryDrop(Entity<HandsComponent>.op_Implicit(loader.Owner), item, coords, checkActionBlocker: false))
		{
			if (_powerLoaderGrabbableQuery.HasComp(item))
			{
				_transform.AnchorEntity(Entity<TransformComponent>.op_Implicit((item, ((EntitySystem)this).Transform(item))), (Entity<MapGridComponent>?)null);
			}
			SyncHands(Entity<PowerLoaderComponent>.op_Implicit((Entity<PowerLoaderComponent>.op_Implicit(loader), loader.Comp)));
		}
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<ActivePowerLoaderPilotComponent> pilots = ((EntitySystem)this).EntityQueryEnumerator<ActivePowerLoaderPilotComponent>();
		EntityUid uid = default(EntityUid);
		ActivePowerLoaderPilotComponent active = default(ActivePowerLoaderPilotComponent);
		BuckleComponent buckle = default(BuckleComponent);
		while (pilots.MoveNext(ref uid, ref active))
		{
			if (!((EntitySystem)this).TryComp<BuckleComponent>(uid, ref buckle) || !((EntitySystem)this).HasComp<PowerLoaderComponent>(buckle.BuckledTo))
			{
				RemovePilot(Entity<ActivePowerLoaderPilotComponent>.op_Implicit((uid, active)));
			}
		}
	}
}
