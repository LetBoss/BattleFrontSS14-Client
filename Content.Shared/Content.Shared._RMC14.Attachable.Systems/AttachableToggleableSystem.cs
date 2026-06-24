using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.DoAfter;
using Content.Shared.Fluids;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Light;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;

namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableToggleableSystem : EntitySystem
{
	[Dependency]
	private ActionContainerSystem _actionContainerSystem;

	[Dependency]
	private EntityLookupSystem _entityLookupSystem;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelistSystem;

	[Dependency]
	private MetaDataSystem _metaDataSystem;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private AttachableHolderSystem _attachableHolderSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	[Dependency]
	private UseDelaySystem _useDelaySystem;

	private const string attachableToggleUseDelayID = "RMCAttachableToggle";

	private const int bracingInvalidCollisionGroup = 74;

	private const int bracingRequiredCollisionGroup = 4;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, ActivateInWorldEvent>((EntityEventRefHandler<AttachableToggleableComponent, ActivateInWorldEvent>)OnActivateInWorld, (Type[])null, new Type[1] { typeof(CMGunSystem) });
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableToggleableComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, new Type[1] { typeof(AttachableModifiersSystem) });
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableToggleableInterruptEvent>((EntityEventRefHandler<AttachableToggleableComponent, AttachableToggleableInterruptEvent>)OnAttachableToggleableInterrupt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableToggleActionEvent>((EntityEventRefHandler<AttachableToggleableComponent, AttachableToggleActionEvent>)OnAttachableToggleAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableToggleDoAfterEvent>((EntityEventRefHandler<AttachableToggleableComponent, AttachableToggleDoAfterEvent>)OnAttachableToggleDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableToggleStartedEvent>((EntityEventRefHandler<AttachableToggleableComponent, AttachableToggleStartedEvent>)OnAttachableToggleStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttemptShootEvent>((EntityEventRefHandler<AttachableToggleableComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<GunShotEvent>>((EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<GunShotEvent>>)OnGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, GunShotEvent>((EntityEventRefHandler<AttachableToggleableComponent, GunShotEvent>)OnGunShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, ToggleActionEvent>((EntityEventRefHandler<AttachableToggleableComponent, ToggleActionEvent>)OnToggleAction, new Type[1] { typeof(SharedHandheldLightSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, GrantAttachableActionsEvent>((EntityEventRefHandler<AttachableToggleableComponent, GrantAttachableActionsEvent>)OnGrantAttachableActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, RemoveAttachableActionsEvent>((EntityEventRefHandler<AttachableToggleableComponent, RemoveAttachableActionsEvent>)OnRemoveAttachableActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<HandDeselectedEvent>>((EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<HandDeselectedEvent>>)OnHandDeselected, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<GotEquippedHandEvent>>((EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<GotEquippedHandEvent>>)OnGotEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<GotUnequippedHandEvent>>((EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<GotUnequippedHandEvent>>)OnGotUnequippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, SprayAttemptEvent>((EntityEventRefHandler<AttachableToggleableComponent, SprayAttemptEvent>)OnSprayAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, DroppedEvent>((EntityEventRefHandler<AttachableToggleableComponent, DroppedEvent>)OnDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableComponent, AttachableRelayedEvent<DroppedEvent>>((EntityEventRefHandler<AttachableToggleableComponent, AttachableRelayedEvent<DroppedEvent>>)OnDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableMovementLockedComponent, MoveInputEvent>((EntityEventRefHandler<AttachableMovementLockedComponent, MoveInputEvent>)OnAttachableMovementLockedMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleableSimpleActivateComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableToggleableSimpleActivateComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, new Type[1] { typeof(AttachableModifiersSystem) });
		((EntitySystem)this).SubscribeLocalEvent<AttachableToggleablePreventShootComponent, AttachableAlteredEvent>((EntityEventRefHandler<AttachableToggleablePreventShootComponent, AttachableAlteredEvent>)OnAttachableAltered, (Type[])null, new Type[1] { typeof(AttachableModifiersSystem) });
		((EntitySystem)this).SubscribeLocalEvent<AttachableGunPreventShootComponent, AttemptShootEvent>((EntityEventRefHandler<AttachableGunPreventShootComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
	}

	private void OnAttachableAltered(Entity<AttachableToggleableComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		switch (args.Alteration)
		{
		case AttachableAlteredType.Detached:
		{
			AttachableHolderComponent holderComponent = default(AttachableHolderComponent);
			if (attachable.Comp.SupercedeHolder && ((EntitySystem)this).TryComp<AttachableHolderComponent>(args.Holder, ref holderComponent))
			{
				EntityUid? supercedingAttachable = holderComponent.SupercedingAttachable;
				EntityUid owner = attachable.Owner;
				if (supercedingAttachable.HasValue && supercedingAttachable.GetValueOrDefault() == owner)
				{
					_attachableHolderSystem.SetSupercedingAttachable(Entity<AttachableHolderComponent>.op_Implicit((args.Holder, holderComponent)), null);
				}
			}
			if (attachable.Comp.Active)
			{
				AttachableAlteredEvent ev = args with
				{
					Alteration = AttachableAlteredType.DetachedDeactivated
				};
				((EntitySystem)this).RaiseLocalEvent<AttachableAlteredEvent>(attachable.Owner, ref ev, false);
			}
			attachable.Comp.Attached = false;
			attachable.Comp.Active = false;
			((EntitySystem)this).Dirty<AttachableToggleableComponent>(attachable, (MetaDataComponent)null);
			break;
		}
		case AttachableAlteredType.Attached:
			attachable.Comp.Attached = true;
			break;
		case AttachableAlteredType.Unwielded:
			if (attachable.Comp.WieldedOnly && attachable.Comp.Active)
			{
				Toggle(attachable, args.User, attachable.Comp.DoInterrupt);
			}
			break;
		}
		ActionComponent actionComponent = default(ActionComponent);
		if (attachable.Comp.Action.HasValue && ((EntitySystem)this).TryComp<ActionComponent>(attachable.Comp.Action, ref actionComponent))
		{
			SharedActionsSystem actionsSystem = _actionsSystem;
			EntityUid? supercedingAttachable = attachable.Comp.Action;
			actionsSystem.SetToggled(supercedingAttachable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(supercedingAttachable.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), attachable.Comp.Active);
			_actionsSystem.SetEnabled(Entity<ActionComponent>.op_Implicit((attachable.Comp.Action.Value, actionComponent)), attachable.Comp.Attached);
		}
	}

	private void OnAttachableAltered(Entity<AttachableToggleableSimpleActivateComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (args.User.HasValue)
		{
			switch (args.Alteration)
			{
			case AttachableAlteredType.Activated:
				((EntitySystem)this).RaiseLocalEvent<ActivateInWorldEvent>(attachable.Owner, new ActivateInWorldEvent(args.User.Value, args.Holder, complex: true), false);
				break;
			case AttachableAlteredType.Deactivated:
				((EntitySystem)this).RaiseLocalEvent<ActivateInWorldEvent>(attachable.Owner, new ActivateInWorldEvent(args.User.Value, args.Holder, complex: true), false);
				break;
			case AttachableAlteredType.DetachedDeactivated:
				((EntitySystem)this).RaiseLocalEvent<ActivateInWorldEvent>(attachable.Owner, new ActivateInWorldEvent(args.User.Value, args.Holder, complex: true), false);
				break;
			}
		}
	}

	private void OnAttachableAltered(Entity<AttachableToggleablePreventShootComponent> attachable, ref AttachableAlteredEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		AttachableToggleableComponent toggleableComponent = default(AttachableToggleableComponent);
		if (((EntitySystem)this).TryComp<AttachableToggleableComponent>(attachable.Owner, ref toggleableComponent))
		{
			AttachableGunPreventShootComponent preventShootComponent = default(AttachableGunPreventShootComponent);
			((EntitySystem)this).EnsureComp<AttachableGunPreventShootComponent>(args.Holder, ref preventShootComponent);
			switch (args.Alteration)
			{
			case AttachableAlteredType.Attached:
				preventShootComponent.Message = attachable.Comp.Message;
				preventShootComponent.PreventShoot = (attachable.Comp.ShootWhenActive && !toggleableComponent.Active) || (!attachable.Comp.ShootWhenActive && toggleableComponent.Active);
				break;
			case AttachableAlteredType.Detached:
				preventShootComponent.Message = "";
				preventShootComponent.PreventShoot = false;
				break;
			case AttachableAlteredType.Activated:
				preventShootComponent.PreventShoot = !attachable.Comp.ShootWhenActive;
				break;
			case AttachableAlteredType.Deactivated:
				preventShootComponent.PreventShoot = attachable.Comp.ShootWhenActive;
				break;
			case AttachableAlteredType.DetachedDeactivated:
				preventShootComponent.PreventShoot = false;
				break;
			}
			((EntitySystem)this).Dirty(args.Holder, (IComponent)(object)preventShootComponent, (MetaDataComponent)null);
		}
	}

	private void OnGotEquippedHand(Entity<AttachableToggleableComponent> attachable, ref AttachableRelayedEvent<GotEquippedHandEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (attachable.Comp.Attached)
		{
			((HandledEntityEventArgs)args.Args).Handled = true;
			GrantAttachableActionsEvent addEv = new GrantAttachableActionsEvent(args.Args.User);
			((EntitySystem)this).RaiseLocalEvent<GrantAttachableActionsEvent>(Entity<AttachableToggleableComponent>.op_Implicit(attachable), ref addEv, false);
		}
	}

	private void OnActivateInWorld(Entity<AttachableToggleableComponent> attachable, ref ActivateInWorldEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (attachable.Comp.AttachedOnly && !attachable.Comp.Attached)
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnAttemptShoot(Entity<AttachableToggleableComponent> attachable, ref AttemptShootEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		EntityUid? holderUid;
		WieldableComponent wieldableComponent = default(WieldableComponent);
		if (attachable.Comp.AttachedOnly && !attachable.Comp.Attached)
		{
			args.Cancelled = true;
		}
		else if (attachable.Comp.WieldedUseOnly && (!_attachableHolderSystem.TryGetHolder(attachable.Owner, out holderUid) || !((EntitySystem)this).TryComp<WieldableComponent>(holderUid, ref wieldableComponent) || !wieldableComponent.Wielded))
		{
			args.Cancelled = true;
			if (holderUid.HasValue)
			{
				_popupSystem.PopupClient(base.Loc.GetString("rmc-attachable-shoot-fail-not-wielded", (ValueTuple<string, object>)("holder", holderUid), (ValueTuple<string, object>)("attachable", attachable)), args.User, args.User);
			}
		}
	}

	private void OnGunShot(Entity<AttachableToggleableComponent> attachable, ref AttachableRelayedEvent<GunShotEvent> args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		CheckUserBreakOnRotate(Entity<AttachableDirectionLockedComponent>.op_Implicit(args.Args.User));
		CheckUserBreakOnFullRotate(Entity<AttachableSideLockedComponent>.op_Implicit(args.Args.User), args.Args.FromCoordinates, args.Args.ToCoordinates);
	}

	private void OnGunShot(Entity<AttachableToggleableComponent> attachable, ref GunShotEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		CheckUserBreakOnRotate(Entity<AttachableDirectionLockedComponent>.op_Implicit(args.User));
		CheckUserBreakOnFullRotate(Entity<AttachableSideLockedComponent>.op_Implicit(args.User), args.FromCoordinates, args.ToCoordinates);
	}

	private void OnAttemptShoot(Entity<AttachableGunPreventShootComponent> gun, ref AttemptShootEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && gun.Comp.PreventShoot)
		{
			args.Cancelled = true;
			_popupSystem.PopupClient(gun.Comp.Message, args.User, args.User);
		}
	}

	private void OnHandDeselected(Entity<AttachableToggleableComponent> attachable, ref AttachableRelayedEvent<HandDeselectedEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (attachable.Comp.Attached)
		{
			((HandledEntityEventArgs)args.Args).Handled = true;
			if (attachable.Comp.NeedHand && attachable.Comp.Active)
			{
				Toggle(attachable, args.Args.User, attachable.Comp.DoInterrupt);
			}
		}
	}

	private void OnAttachableToggleableInterrupt(Entity<AttachableToggleableComponent> attachable, ref AttachableToggleableInterruptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (attachable.Comp.Active)
		{
			Toggle(attachable, args.User, attachable.Comp.DoInterrupt);
		}
	}

	private void OnGotUnequippedHand(Entity<AttachableToggleableComponent> attachable, ref AttachableRelayedEvent<GotUnequippedHandEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (attachable.Comp.Attached)
		{
			((HandledEntityEventArgs)args.Args).Handled = true;
			if ((attachable.Comp.NeedHand || attachable.Comp.BreakOnDrop) && attachable.Comp.Active)
			{
				Toggle(attachable, args.Args.User, attachable.Comp.DoInterrupt);
			}
			RemoveAttachableActionsEvent removeEv = new RemoveAttachableActionsEvent(args.Args.User);
			((EntitySystem)this).RaiseLocalEvent<RemoveAttachableActionsEvent>(Entity<AttachableToggleableComponent>.op_Implicit(attachable), ref removeEv, false);
		}
	}

	private void OnSprayAttempt(Entity<AttachableToggleableComponent> attachable, ref SprayAttemptEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && attachable.Comp.AttachedOnly && !attachable.Comp.Attached)
		{
			args.Cancelled = true;
		}
	}

	private void OnDropped(Entity<AttachableToggleableComponent> attachable, ref DroppedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if ((!attachable.Comp.AttachedOnly || attachable.Comp.Attached) && attachable.Comp.BreakOnDrop && attachable.Comp.Active)
		{
			Toggle(attachable, args.User);
		}
	}

	private void OnDropped(Entity<AttachableToggleableComponent> attachable, ref AttachableRelayedEvent<DroppedEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((!attachable.Comp.AttachedOnly || attachable.Comp.Attached) && attachable.Comp.BreakOnDrop && attachable.Comp.Active)
		{
			Toggle(attachable, args.Args.User);
		}
	}

	private void OnAttachableMovementLockedMoveInput(Entity<AttachableMovementLockedComponent> user, ref MoveInputEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!args.HasDirectionalMovement)
		{
			return;
		}
		AttachableToggleableComponent toggleableComponent = default(AttachableToggleableComponent);
		for (int i = user.Comp.AttachableList.Count - 1; i >= 0; i--)
		{
			EntityUid attachableUid = user.Comp.AttachableList[i];
			if (((EntitySystem)this).TryComp<AttachableToggleableComponent>(attachableUid, ref toggleableComponent) && toggleableComponent.Active && toggleableComponent.BreakOnMove)
			{
				Toggle(Entity<AttachableToggleableComponent>.op_Implicit((attachableUid, toggleableComponent)), user.Owner, toggleableComponent.DoInterrupt);
			}
		}
	}

	private void CheckUserBreakOnRotate(Entity<AttachableDirectionLockedComponent?> user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (user.Comp == null)
		{
			AttachableDirectionLockedComponent lockedComponent = default(AttachableDirectionLockedComponent);
			if (!((EntitySystem)this).TryComp<AttachableDirectionLockedComponent>(user.Owner, ref lockedComponent))
			{
				return;
			}
			user.Comp = lockedComponent;
		}
		Angle localRotation = ((EntitySystem)this).Transform(user.Owner).LocalRotation;
		if ((Direction?)((Angle)(ref localRotation)).GetCardinalDir() == user.Comp.LockedDirection)
		{
			return;
		}
		AttachableToggleableComponent toggleableComponent = default(AttachableToggleableComponent);
		for (int i = user.Comp.AttachableList.Count - 1; i >= 0; i--)
		{
			EntityUid attachableUid = user.Comp.AttachableList[i];
			if (((EntitySystem)this).TryComp<AttachableToggleableComponent>(attachableUid, ref toggleableComponent) && toggleableComponent.Active && toggleableComponent.BreakOnRotate)
			{
				Toggle(Entity<AttachableToggleableComponent>.op_Implicit((attachableUid, toggleableComponent)), user.Owner, toggleableComponent.DoInterrupt);
			}
		}
	}

	private void CheckUserBreakOnFullRotate(Entity<AttachableSideLockedComponent?> user, EntityCoordinates playerPos, EntityCoordinates targetPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		if (user.Comp == null)
		{
			AttachableSideLockedComponent lockedComponent = default(AttachableSideLockedComponent);
			if (!((EntitySystem)this).TryComp<AttachableSideLockedComponent>(user.Owner, ref lockedComponent))
			{
				return;
			}
			user.Comp = lockedComponent;
		}
		if (!user.Comp.LockedDirection.HasValue)
		{
			return;
		}
		Angle initialAngle = DirectionExtensions.ToAngle(user.Comp.LockedDirection.Value);
		MapCoordinates playerMapPos = _transformSystem.ToMapCoordinates(playerPos, true);
		Angle currentAngle = DirectionExtensions.ToWorldAngle(_transformSystem.ToMapCoordinates(targetPos, true).Position - playerMapPos.Position);
		double differenceFromLockedAngle = (((Angle)(ref currentAngle)).Degrees - ((Angle)(ref initialAngle)).Degrees + 180.0 + 360.0) % 360.0 - 180.0;
		if (differenceFromLockedAngle > -90.0 && differenceFromLockedAngle < 90.0)
		{
			return;
		}
		AttachableToggleableComponent toggleableComponent = default(AttachableToggleableComponent);
		for (int i = user.Comp.AttachableList.Count - 1; i >= 0; i--)
		{
			EntityUid attachableUid = user.Comp.AttachableList[i];
			if (((EntitySystem)this).TryComp<AttachableToggleableComponent>(attachableUid, ref toggleableComponent) && toggleableComponent.Active && toggleableComponent.BreakOnFullRotate)
			{
				Toggle(Entity<AttachableToggleableComponent>.op_Implicit((attachableUid, toggleableComponent)), user.Owner, toggleableComponent.DoInterrupt);
			}
		}
	}

	private void OnAttachableToggleStarted(Entity<AttachableToggleableComponent> attachable, ref AttachableToggleStartedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.User) && CanStartToggleDoAfter(attachable, ref args))
		{
			string popupText = base.Loc.GetString(LocId.op_Implicit(attachable.Comp.Active ? attachable.Comp.DeactivatePopupText : attachable.Comp.ActivatePopupText), (ValueTuple<string, object>)("attachable", attachable.Owner));
			_doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, GetToggleDoAfter(attachable, args.Holder, args.User, ref popupText), new AttachableToggleDoAfterEvent(args.SlotId, popupText), Entity<AttachableToggleableComponent>.op_Implicit(attachable), attachable.Owner, args.Holder)
			{
				NeedHand = attachable.Comp.DoAfterNeedHand,
				BreakOnMove = attachable.Comp.DoAfterBreakOnMove
			});
			((EntitySystem)this).Dirty<AttachableToggleableComponent>(attachable, (MetaDataComponent)null);
		}
	}

	private bool CanStartToggleDoAfter(Entity<AttachableToggleableComponent> attachable, ref AttachableToggleStartedEvent args, bool silent = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent useDelayComponent = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(attachable.Owner, ref useDelayComponent) && _useDelaySystem.IsDelayed(Entity<UseDelayComponent>.op_Implicit((attachable.Owner, useDelayComponent)), "RMCAttachableToggle"))
		{
			return false;
		}
		_attachableHolderSystem.TryGetUser(attachable.Owner, out var userUid);
		if (attachable.Comp.HeldOnlyActivate && !attachable.Comp.Active && (!userUid.HasValue || !_handsSystem.IsHolding(Entity<HandsComponent>.op_Implicit(userUid.Value), args.Holder, out string _)))
		{
			if (!silent)
			{
				_popupSystem.PopupClient(base.Loc.GetString("rmc-attachable-activation-fail-not-held", (ValueTuple<string, object>)("holder", args.Holder), (ValueTuple<string, object>)("attachable", attachable)), args.User, args.User);
			}
			return false;
		}
		if (attachable.Comp.UserOnly)
		{
			EntityUid? val = userUid;
			EntityUid user = args.User;
			if (!val.HasValue || val.GetValueOrDefault() != user)
			{
				if (!silent)
				{
					_popupSystem.PopupClient(base.Loc.GetString("rmc-attachable-activation-fail-not-owned", (ValueTuple<string, object>)("holder", args.Holder), (ValueTuple<string, object>)("attachable", attachable)), args.User, args.User);
				}
				return false;
			}
		}
		WieldableComponent wieldableComponent = default(WieldableComponent);
		if (!attachable.Comp.Active && attachable.Comp.WieldedOnly && (!((EntitySystem)this).TryComp<WieldableComponent>(args.Holder, ref wieldableComponent) || !wieldableComponent.Wielded))
		{
			if (!silent)
			{
				_popupSystem.PopupClient(base.Loc.GetString("rmc-attachable-activation-fail-not-wielded", (ValueTuple<string, object>)("holder", args.Holder), (ValueTuple<string, object>)("attachable", attachable)), args.User, args.User);
			}
			return false;
		}
		return true;
	}

	private TimeSpan GetToggleDoAfter(Entity<AttachableToggleableComponent> attachable, EntityUid holderUid, EntityUid userUid, ref string popupText)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected I4, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transformComponent = default(TransformComponent);
		if (((EntitySystem)this).TryComp(holderUid, ref transformComponent))
		{
			EntityUid parentUid = transformComponent.ParentUid;
			if (((EntityUid)(ref parentUid)).Valid)
			{
				float extraDoAfter = ((transformComponent.ParentUid == userUid) ? 0f : 0.5f);
				TransformComponent userTransform = default(TransformComponent);
				if (attachable.Comp.InstantToggle == AttachableInstantToggleConditions.Brace && !attachable.Comp.Active && !(transformComponent.ParentUid != userUid) && ((EntitySystem)this).TryComp(userUid, ref userTransform))
				{
					EntityCoordinates coords = userTransform.Coordinates;
					Func<EntityCoordinates, EntityCoordinates, bool> comparer = (EntityCoordinates userCoords, EntityCoordinates entCoords) => false;
					Vector2 coordsShift = new Vector2(0f, 0f);
					Func<HashSet<EntityUid>, EntityUid?> GetBracingSurface = delegate(HashSet<EntityUid> ents)
					{
						//IL_000e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0013: Unknown result type (might be due to invalid IL or missing references)
						//IL_001a: Unknown result type (might be due to invalid IL or missing references)
						//IL_002d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0074: Unknown result type (might be due to invalid IL or missing references)
						//IL_007f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0085: Unknown result type (might be due to invalid IL or missing references)
						//IL_0091: Unknown result type (might be due to invalid IL or missing references)
						FixturesComponent val = default(FixturesComponent);
						foreach (EntityUid current in ents)
						{
							if (((EntitySystem)this).TryComp<FixturesComponent>(current, ref val) && ((EntitySystem)this).Transform(current).Anchored)
							{
								foreach (Fixture current2 in val.Fixtures.Values)
								{
									if ((current2.CollisionLayer & 0x4A) == 0 && (current2.CollisionLayer & 4) != 0 && comparer(coords, ((EntitySystem)this).Transform(current).Coordinates))
									{
										return current;
									}
								}
							}
						}
						return (EntityUid?)null;
					};
					Angle localRotation = userTransform.LocalRotation;
					Direction cardinalDir = ((Angle)(ref localRotation)).GetCardinalDir();
					switch ((int)cardinalDir)
					{
					case 0:
						comparer = (EntityCoordinates userCoords, EntityCoordinates entCoords) => ((EntityCoordinates)(ref entCoords)).Y < ((EntityCoordinates)(ref userCoords)).Y;
						coordsShift = new Vector2(0f, -0.7f);
						break;
					case 4:
						comparer = (EntityCoordinates userCoords, EntityCoordinates entCoords) => ((EntityCoordinates)(ref entCoords)).Y > ((EntityCoordinates)(ref userCoords)).Y;
						coordsShift = new Vector2(0f, 0.7f);
						break;
					case 2:
						comparer = (EntityCoordinates userCoords, EntityCoordinates entCoords) => ((EntityCoordinates)(ref entCoords)).X > ((EntityCoordinates)(ref userCoords)).X;
						coordsShift = new Vector2(0.7f, 0f);
						break;
					case 6:
						comparer = (EntityCoordinates userCoords, EntityCoordinates entCoords) => ((EntityCoordinates)(ref entCoords)).X < ((EntityCoordinates)(ref userCoords)).X;
						coordsShift = new Vector2(-0.7f, 0f);
						break;
					}
					EntityUid? surface = GetBracingSurface(_entityLookupSystem.GetEntitiesInRange(coords, 0.5f, (LookupFlags)6));
					if (surface.HasValue)
					{
						popupText = base.Loc.GetString("attachable-popup-activate-deploy-on-generic", (ValueTuple<string, object>)("attachable", attachable.Owner), (ValueTuple<string, object>)("surface", surface));
						return TimeSpan.FromSeconds(0.0);
					}
					coords = new EntityCoordinates(coords.EntityId, coords.Position + coordsShift);
					surface = GetBracingSurface(_entityLookupSystem.GetEntitiesInRange(coords, 0.5f, (LookupFlags)6));
					if (surface.HasValue)
					{
						popupText = base.Loc.GetString("attachable-popup-activate-deploy-on-generic", (ValueTuple<string, object>)("attachable", attachable.Owner), (ValueTuple<string, object>)("surface", surface));
						return TimeSpan.FromSeconds(0.0);
					}
					popupText = base.Loc.GetString("attachable-popup-activate-deploy-on-ground", (ValueTuple<string, object>)("attachable", attachable.Owner));
				}
				return TimeSpan.FromSeconds(Math.Max(((attachable.Comp.DeactivateDoAfter.HasValue && attachable.Comp.Active) ? attachable.Comp.DeactivateDoAfter.Value : attachable.Comp.DoAfter) + extraDoAfter, 0f));
			}
		}
		return TimeSpan.FromSeconds(0.0);
	}

	private void OnAttachableToggleDoAfter(Entity<AttachableToggleableComponent> attachable, ref AttachableToggleDoAfterEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
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
			AttachableHolderComponent holderComponent = default(AttachableHolderComponent);
			InputMoverComponent input = default(InputMoverComponent);
			if (((EntitySystem)this).HasComp<AttachableToggleableComponent>(target2) && ((EntitySystem)this).TryComp<AttachableHolderComponent>(args.Used, ref holderComponent) && (attachable.Comp.Active || !((EntitySystem)this).TryComp<InputMoverComponent>(args.User, ref input) || (input.HeldMoveButtons & MoveButtons.AnyDirection) == 0 || attachable.Comp.ActivateOnMove))
			{
				FinishToggle(attachable, Entity<AttachableHolderComponent>.op_Implicit((used, holderComponent)), args.SlotId, args.User, args.PopupText);
				((HandledEntityEventArgs)args).Handled = true;
				((EntitySystem)this).Dirty<AttachableToggleableComponent>(attachable, (MetaDataComponent)null);
			}
		}
	}

	private void RemoveUnusedLocks(Entity<AttachableToggleableComponent> attachable, EntityUid? userUid)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if (!userUid.HasValue)
		{
			return;
		}
		AttachableMovementLockedComponent movementLockedComponent = default(AttachableMovementLockedComponent);
		if (attachable.Comp.BreakOnMove && ((EntitySystem)this).TryComp<AttachableMovementLockedComponent>(userUid.Value, ref movementLockedComponent))
		{
			movementLockedComponent.AttachableList.Remove(attachable.Owner);
			if (movementLockedComponent.AttachableList.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<AttachableMovementLockedComponent>(userUid.Value);
			}
		}
		AttachableDirectionLockedComponent directionLockedComponent = default(AttachableDirectionLockedComponent);
		if (attachable.Comp.BreakOnRotate && ((EntitySystem)this).TryComp<AttachableDirectionLockedComponent>(userUid.Value, ref directionLockedComponent))
		{
			directionLockedComponent.AttachableList.Remove(attachable.Owner);
			if (directionLockedComponent.AttachableList.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<AttachableDirectionLockedComponent>(userUid.Value);
			}
		}
		AttachableSideLockedComponent sideLockedComponent = default(AttachableSideLockedComponent);
		if (attachable.Comp.BreakOnFullRotate && ((EntitySystem)this).TryComp<AttachableSideLockedComponent>(userUid.Value, ref sideLockedComponent))
		{
			sideLockedComponent.AttachableList.Remove(attachable.Owner);
			if (sideLockedComponent.AttachableList.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<AttachableSideLockedComponent>(userUid.Value);
			}
		}
	}

	private void FinishToggle(Entity<AttachableToggleableComponent> attachable, Entity<AttachableHolderComponent> holder, string slotId, EntityUid? userUid, string popupText, bool interrupted = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		attachable.Comp.Active = !attachable.Comp.Active;
		AttachableAlteredType mode = (attachable.Comp.Active ? AttachableAlteredType.Activated : (interrupted ? AttachableAlteredType.Interrupted : AttachableAlteredType.Deactivated));
		AttachableAlteredEvent ev = new AttachableAlteredEvent(holder.Owner, mode, userUid);
		((EntitySystem)this).RaiseLocalEvent<AttachableAlteredEvent>(attachable.Owner, ref ev, false);
		AttachableHolderAttachablesAlteredEvent holderEv = new AttachableHolderAttachablesAlteredEvent(attachable.Owner, slotId, mode);
		((EntitySystem)this).RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(holder.Owner, ref holderEv, false);
		_useDelaySystem.SetLength(Entity<UseDelayComponent>.op_Implicit(attachable.Owner), attachable.Comp.UseDelay, "RMCAttachableToggle");
		_useDelaySystem.TryResetDelay(attachable.Owner, checkDelayed: false, null, "RMCAttachableToggle");
		SharedActionsSystem actionsSystem = _actionsSystem;
		EntityUid? action = attachable.Comp.Action;
		actionsSystem.StartUseDelay(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		if (attachable.Comp.ShowTogglePopup && userUid.HasValue)
		{
			_popupSystem.PopupClient(popupText, userUid.Value, userUid.Value);
		}
		_audioSystem.PlayPredicted(attachable.Comp.Active ? attachable.Comp.ActivateSound : attachable.Comp.DeactivateSound, Entity<AttachableToggleableComponent>.op_Implicit(attachable), userUid, (AudioParams?)null);
		if (!attachable.Comp.Active)
		{
			if (attachable.Comp.SupercedeHolder)
			{
				action = holder.Comp.SupercedingAttachable;
				EntityUid owner = attachable.Owner;
				if (action.HasValue && action.GetValueOrDefault() == owner)
				{
					_attachableHolderSystem.SetSupercedingAttachable(holder, null);
				}
			}
			RemoveUnusedLocks(attachable, userUid);
			return;
		}
		if (attachable.Comp.BreakOnMove && userUid.HasValue)
		{
			((EntitySystem)this).EnsureComp<AttachableMovementLockedComponent>(userUid.Value).AttachableList.Add(attachable.Owner);
		}
		Angle localRotation;
		if (attachable.Comp.BreakOnRotate && userUid.HasValue)
		{
			AttachableDirectionLockedComponent directionLockedComponent = ((EntitySystem)this).EnsureComp<AttachableDirectionLockedComponent>(userUid.Value);
			directionLockedComponent.AttachableList.Add(attachable.Owner);
			if (!directionLockedComponent.LockedDirection.HasValue)
			{
				localRotation = ((EntitySystem)this).Transform(userUid.Value).LocalRotation;
				directionLockedComponent.LockedDirection = ((Angle)(ref localRotation)).GetCardinalDir();
			}
		}
		if (attachable.Comp.BreakOnFullRotate && userUid.HasValue)
		{
			AttachableSideLockedComponent sideLockedComponent = ((EntitySystem)this).EnsureComp<AttachableSideLockedComponent>(userUid.Value);
			sideLockedComponent.AttachableList.Add(attachable.Owner);
			if (!sideLockedComponent.LockedDirection.HasValue)
			{
				localRotation = ((EntitySystem)this).Transform(userUid.Value).LocalRotation;
				sideLockedComponent.LockedDirection = ((Angle)(ref localRotation)).GetCardinalDir();
			}
		}
		if (!attachable.Comp.SupercedeHolder)
		{
			return;
		}
		AttachableToggleableComponent toggleableComponent = default(AttachableToggleableComponent);
		if (holder.Comp.SupercedingAttachable.HasValue && ((EntitySystem)this).TryComp<AttachableToggleableComponent>(holder.Comp.SupercedingAttachable, ref toggleableComponent))
		{
			toggleableComponent.Active = false;
			ev = new AttachableAlteredEvent(holder.Owner, AttachableAlteredType.Deactivated);
			((EntitySystem)this).RaiseLocalEvent<AttachableAlteredEvent>(holder.Comp.SupercedingAttachable.Value, ref ev, false);
			if (_attachableHolderSystem.TryGetSlotId(holder.Owner, attachable.Owner, out string deactivatedSlot))
			{
				holderEv = new AttachableHolderAttachablesAlteredEvent(holder.Comp.SupercedingAttachable.Value, deactivatedSlot, AttachableAlteredType.Deactivated);
				((EntitySystem)this).RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(holder.Owner, ref holderEv, false);
			}
		}
		_attachableHolderSystem.SetSupercedingAttachable(holder, attachable.Owner);
	}

	private void Toggle(Entity<AttachableToggleableComponent> attachable, EntityUid? user, bool interrupted = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		AttachableHolderComponent holderComponent = default(AttachableHolderComponent);
		if (_attachableHolderSystem.TryGetHolder(attachable.Owner, out var holderUid) && ((EntitySystem)this).TryComp<AttachableHolderComponent>(holderUid, ref holderComponent) && _attachableHolderSystem.TryGetSlotId(holderUid.Value, attachable.Owner, out string slotId))
		{
			FinishToggle(attachable, Entity<AttachableHolderComponent>.op_Implicit((holderUid.Value, holderComponent)), slotId, user, base.Loc.GetString(LocId.op_Implicit(attachable.Comp.Active ? attachable.Comp.DeactivatePopupText : attachable.Comp.ActivatePopupText), (ValueTuple<string, object>)("attachable", attachable.Owner)), interrupted);
			((EntitySystem)this).Dirty<AttachableToggleableComponent>(attachable, (MetaDataComponent)null);
		}
	}

	private void OnGrantAttachableActions(Entity<AttachableToggleableComponent> ent, ref GrantAttachableActionsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		GrantAttachableActions(ent, args.User);
		RelayAttachableActions(ent, args.User);
	}

	private void GrantAttachableActions(Entity<AttachableToggleableComponent> ent, EntityUid user, bool doSecondTry = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		ActionsContainerComponent actionsContainerComponent = default(ActionsContainerComponent);
		if (!((EntitySystem)this).TryComp<ActionsContainerComponent>(ent.Owner, ref actionsContainerComponent) || actionsContainerComponent.Container == null)
		{
			((EntitySystem)this).EnsureComp<ActionsContainerComponent>(ent.Owner);
			if (doSecondTry)
			{
				GrantAttachableActions(ent, user, doSecondTry: false);
			}
			return;
		}
		bool exists = ent.Comp.Action.HasValue;
		_actionContainerSystem.EnsureAction(Entity<AttachableToggleableComponent>.op_Implicit(ent), ref ent.Comp.Action, ent.Comp.ActionId, actionsContainerComponent);
		EntityUid? action = ent.Comp.Action;
		if (!action.HasValue)
		{
			return;
		}
		EntityUid actionId = action.GetValueOrDefault();
		_actionsSystem.GrantContainedAction(Entity<ActionsComponent>.op_Implicit(user), Entity<ActionsContainerComponent>.op_Implicit(ent.Owner), actionId);
		if (!exists)
		{
			_metaDataSystem.SetEntityName(actionId, ent.Comp.ActionName, (MetaDataComponent)null, true);
			_metaDataSystem.SetEntityDescription(actionId, ent.Comp.ActionDesc, (MetaDataComponent)null);
			Entity<ActionComponent>? action2 = _actionsSystem.GetAction(Entity<ActionComponent>.op_Implicit(actionId));
			if (action2.HasValue)
			{
				Entity<ActionComponent> actionEnt = action2.GetValueOrDefault().AsNullable();
				_actionsSystem.SetIcon(actionEnt, ent.Comp.Icon);
				_actionsSystem.SetIconOn(actionEnt, ent.Comp.IconActive);
				_actionsSystem.SetEnabled(actionEnt, ent.Comp.Attached);
				_actionsSystem.SetUseDelay(actionEnt, ent.Comp.UseDelay);
			}
			((EntitySystem)this).Dirty<AttachableToggleableComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void RelayAttachableActions(Entity<AttachableToggleableComponent> attachable, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		ActionsContainerComponent actionsContainerComponent = default(ActionsContainerComponent);
		if (attachable.Comp.ActionsToRelayWhitelist == null || !((EntitySystem)this).TryComp<ActionsContainerComponent>(attachable.Owner, ref actionsContainerComponent))
		{
			return;
		}
		foreach (EntityUid actionUid in ((BaseContainer)actionsContainerComponent.Container).ContainedEntities)
		{
			if (_entityWhitelistSystem.IsWhitelistPass(attachable.Comp.ActionsToRelayWhitelist, actionUid))
			{
				_actionsSystem.GrantContainedAction(Entity<ActionsComponent>.op_Implicit(user), Entity<ActionsContainerComponent>.op_Implicit((attachable.Owner, actionsContainerComponent)), actionUid);
			}
		}
	}

	private void OnRemoveAttachableActions(Entity<AttachableToggleableComponent> ent, ref RemoveAttachableActionsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		RemoveAttachableActions(ent, args.User);
		RemoveRelayedActions(ent, args.User);
	}

	private void RemoveAttachableActions(Entity<AttachableToggleableComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? action = ent.Comp.Action;
		if (!action.HasValue)
		{
			return;
		}
		EntityUid action2 = action.GetValueOrDefault();
		ActionComponent actionComponent = default(ActionComponent);
		if (((EntitySystem)this).HasComp<InstantActionComponent>(action2) && ((EntitySystem)this).TryComp<ActionComponent>(action2, ref actionComponent))
		{
			action = actionComponent.AttachedEntity;
			if (action.HasValue && !(action.GetValueOrDefault() != user))
			{
				_actionsSystem.RemoveProvidedAction(user, Entity<AttachableToggleableComponent>.op_Implicit(ent), action2);
			}
		}
	}

	private void RemoveRelayedActions(Entity<AttachableToggleableComponent> attachable, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		ActionsContainerComponent actionsContainerComponent = default(ActionsContainerComponent);
		if (attachable.Comp.ActionsToRelayWhitelist == null || !((EntitySystem)this).TryComp<ActionsContainerComponent>(attachable.Owner, ref actionsContainerComponent))
		{
			return;
		}
		foreach (EntityUid actionUid in ((BaseContainer)actionsContainerComponent.Container).ContainedEntities)
		{
			if (_entityWhitelistSystem.IsWhitelistPass(attachable.Comp.ActionsToRelayWhitelist, actionUid))
			{
				_actionsSystem.RemoveProvidedAction(user, attachable.Owner, actionUid);
			}
		}
	}

	private void OnAttachableToggleAction(Entity<AttachableToggleableComponent> attachable, ref AttachableToggleActionEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		AttachableHolderComponent holderComponent = default(AttachableHolderComponent);
		if (attachable.Comp.Attached && _attachableHolderSystem.TryGetHolder(attachable.Owner, out var holderUid) && ((EntitySystem)this).TryComp<AttachableHolderComponent>(holderUid, ref holderComponent) && _attachableHolderSystem.TryGetSlotId(holderUid.Value, attachable.Owner, out string slotId))
		{
			AttachableToggleStartedEvent ev = new AttachableToggleStartedEvent(holderUid.Value, args.Performer, slotId);
			((EntitySystem)this).RaiseLocalEvent<AttachableToggleStartedEvent>(attachable.Owner, ref ev, false);
		}
	}

	private void OnToggleAction(Entity<AttachableToggleableComponent> attachable, ref ToggleActionEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (attachable.Comp.AttachedOnly && !attachable.Comp.Attached)
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}
}
