using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Attachable;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.CombatMode;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Synth;
using Content.Shared._RMC14.TrainingDummy;
using Content.Shared._RMC14.Vents;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
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
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Devour;

public sealed class XenoDevourSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private SharedMeleeWeaponSystem _meleeWeapon;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	private EntityQuery<DevouredComponent> _devouredQuery;

	private EntityQuery<XenoDevourComponent> _xenoDevourQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_devouredQuery = ((EntitySystem)this).GetEntityQuery<DevouredComponent>();
		_xenoDevourQuery = ((EntitySystem)this).GetEntityQuery<XenoDevourComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ActionBlockIfDevouredComponent, RMCActionUseAttemptEvent>((EntityEventRefHandler<ActionBlockIfDevouredComponent, RMCActionUseAttemptEvent>)OnDevouredActionUseAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevourableComponent, CanDropDraggedEvent>((EntityEventRefHandler<DevourableComponent, CanDropDraggedEvent>)OnDevourableCanDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevourableComponent, DragDropDraggedEvent>((EntityEventRefHandler<DevourableComponent, DragDropDraggedEvent>)OnDevourableDragDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevourableComponent, BeforeRangedInteractEvent>((EntityEventRefHandler<DevourableComponent, BeforeRangedInteractEvent>)OnDevourableBeforeRangedInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevourableComponent, ShouldHandleVirtualItemInteractEvent>((EntityEventRefHandler<DevourableComponent, ShouldHandleVirtualItemInteractEvent>)OnDevourableShouldHandle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, ComponentStartup>((EntityEventRefHandler<DevouredComponent, ComponentStartup>)OnDevouredStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, ComponentRemove>((EntityEventRefHandler<DevouredComponent, ComponentRemove>)OnDevouredRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, EntGotRemovedFromContainerMessage>((EntityEventRefHandler<DevouredComponent, EntGotRemovedFromContainerMessage>)OnDevouredRemovedFromContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, InteractionAttemptEvent>((EntityEventRefHandler<DevouredComponent, InteractionAttemptEvent>)OnDevouredInteractionAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, UpdateCanMoveEvent>((EntityEventRefHandler<DevouredComponent, UpdateCanMoveEvent>)OnDevouredAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, ThrowAttemptEvent>((EntityEventRefHandler<DevouredComponent, ThrowAttemptEvent>)OnDevouredAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, DropAttemptEvent>((EntityEventRefHandler<DevouredComponent, DropAttemptEvent>)OnDevouredAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, UseAttemptEvent>((EntityEventRefHandler<DevouredComponent, UseAttemptEvent>)OnUseAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, PickupAttemptEvent>((EntityEventRefHandler<DevouredComponent, PickupAttemptEvent>)OnDevouredPickupAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, IsEquippingAttemptEvent>((EntityEventRefHandler<DevouredComponent, IsEquippingAttemptEvent>)OnDevouredIsEquippingAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, IsUnequippingAttemptEvent>((EntityEventRefHandler<DevouredComponent, IsUnequippingAttemptEvent>)OnDevouredIsUnequippingAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, RMCCombatModeInteractOverrideUserEvent>((EntityEventRefHandler<DevouredComponent, RMCCombatModeInteractOverrideUserEvent>)OnDevouredTryAttack, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, ShotAttemptedEvent>((EntityEventRefHandler<DevouredComponent, ShotAttemptedEvent>)OnDevouredShotAttempted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevouredComponent, MoveInputEvent>((EntityEventRefHandler<DevouredComponent, MoveInputEvent>)OnDevouredMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UsableWhileDevouredComponent, MeleeHitEvent>((EntityEventRefHandler<UsableWhileDevouredComponent, MeleeHitEvent>)UsuableByDevouredMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, CanDropTargetEvent>((EntityEventRefHandler<XenoDevourComponent, CanDropTargetEvent>)OnXenoCanDropTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, ActivateInWorldEvent>((EntityEventRefHandler<XenoDevourComponent, ActivateInWorldEvent>)OnXenoActivate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, DoAfterAttemptEvent<XenoDevourDoAfterEvent>>((EntityEventRefHandler<XenoDevourComponent, DoAfterAttemptEvent<XenoDevourDoAfterEvent>>)OnXenoDevourDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, XenoDevourDoAfterEvent>((EntityEventRefHandler<XenoDevourComponent, XenoDevourDoAfterEvent>)OnXenoDevourDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, XenoRegurgitateActionEvent>((EntityEventRefHandler<XenoDevourComponent, XenoRegurgitateActionEvent>)OnXenoRegurgitateAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, EntityTerminatingEvent>((EntityEventRefHandler<XenoDevourComponent, EntityTerminatingEvent>)OnXenoTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoDevourComponent, MobStateChangedEvent>)OnXenoMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, RMCCombatModeInteractOverrideUserEvent>((EntityEventRefHandler<XenoDevourComponent, RMCCombatModeInteractOverrideUserEvent>)OnXenoCombatModeInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, InteractUsingEvent>((EntityEventRefHandler<XenoDevourComponent, InteractUsingEvent>)OnXenoDevouredInteractWith, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoDevourComponent, VentEnterAttemptEvent>((EntityEventRefHandler<XenoDevourComponent, VentEnterAttemptEvent>)OnXenoDevouredVentAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UsableWhileDevouredComponent, CMGetArmorPiercingEvent>((EntityEventRefHandler<UsableWhileDevouredComponent, CMGetArmorPiercingEvent>)OnUsableWhileDevouredGetArmorPiercing, (Type[])null, (Type[])null);
	}

	private void OnDevouredActionUseAttempt(Entity<ActionBlockIfDevouredComponent> ent, ref RMCActionUseAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			EntityUid user = args.User;
			if (((EntitySystem)this).HasComp<DevouredComponent>(user))
			{
				args.Cancelled = true;
				_popup.PopupClient(base.Loc.GetString("comp-climbable-cant-interact"), user, user, PopupType.SmallCaution);
			}
		}
	}

	private void OnDevourableShouldHandle(Entity<DevourableComponent> ent, ref ShouldHandleVirtualItemInteractEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoDevourComponent>(args.Event.User))
		{
			EntityUid user = args.Event.User;
			EntityUid? target = args.Event.Target;
			if (target.HasValue && user == target.GetValueOrDefault())
			{
				args.Handle = true;
			}
		}
	}

	private void OnDevourableCanDropDragged(Entity<DevourableComponent> devourable, ref CanDropDraggedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoDevourComponent>(args.User))
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnDevourableDragDropDragged(Entity<DevourableComponent> devourable, ref DragDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.User != args.Target) && StartDevour(args.User, Entity<DevourableComponent>.op_Implicit(devourable)))
		{
			args.Handled = true;
		}
	}

	private void OnDevourableBeforeRangedInteract(Entity<DevourableComponent> ent, ref BeforeRangedInteractEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		EntityUid? target = args.Target;
		if (target.HasValue && !(user != target.GetValueOrDefault()) && StartDevourPulled(args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnDevouredStartup(Entity<DevouredComponent> devoured, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_blocker.UpdateCanMove(Entity<DevouredComponent>.op_Implicit(devoured));
	}

	private void OnDevouredRemove(Entity<DevouredComponent> devoured, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		_blocker.UpdateCanMove(Entity<DevouredComponent>.op_Implicit(devoured));
		BaseContainer container = default(BaseContainer);
		XenoDevourComponent devour = default(XenoDevourComponent);
		if (!_timing.ApplyingState && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<DevouredComponent>.op_Implicit(devoured), null)), ref container) && ((EntitySystem)this).TryComp<XenoDevourComponent>(container.Owner, ref devour) && container.ID != devour.DevourContainerId)
		{
			_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(devoured.Owner), container, true, false, (EntityCoordinates?)null, (Angle?)null);
		}
	}

	private void OnDevouredRemovedFromContainer(Entity<DevouredComponent> devoured, ref EntGotRemovedFromContainerMessage args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			((EntitySystem)this).RemCompDeferred<DevouredComponent>(Entity<DevouredComponent>.op_Implicit(devoured));
		}
	}

	private void OnDevouredInteractionAttempt(Entity<DevouredComponent> ent, ref InteractionAttemptEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Target.HasValue || ((EntitySystem)this).HasComp<UsableWhileDevouredComponent>(args.Target))
		{
			return;
		}
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(ent.Owner), ref container) && ((EntitySystem)this).HasComp<XenoDevourComponent>(container.Owner))
		{
			EntityUid? target = args.Target;
			EntityUid owner = container.Owner;
			if (target.HasValue && !(target.GetValueOrDefault() != owner))
			{
				return;
			}
		}
		args.Cancelled = true;
	}

	private void OnXenoDevouredInteractWith(Entity<XenoDevourComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		DevouredComponent devoured = default(DevouredComponent);
		if (_container.TryGetContainer(Entity<XenoDevourComponent>.op_Implicit(ent), ent.Comp.DevourContainerId, ref container, (ContainerManagerComponent)null) && container.Contains(args.User) && ((EntitySystem)this).TryComp<DevouredComponent>(args.User, ref devoured))
		{
			((HandledEntityEventArgs)args).Handled = true;
			DevouredHandleBreakout(Entity<DevouredComponent>.op_Implicit((args.User, devoured)));
		}
	}

	private void OnDevouredAttempt<T>(Entity<DevouredComponent> devoured, ref T args) where T : CancellableEntityEventArgs
	{
		((CancellableEntityEventArgs)args/*cast due to constrained. prefix*/).Cancel();
	}

	private void OnUseAttempt(Entity<DevouredComponent> ent, ref UseAttemptEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<UsableWhileDevouredComponent>(args.Used))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnDevouredPickupAttempt(Entity<DevouredComponent> ent, ref PickupAttemptEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<UsableWhileDevouredComponent>(args.Item))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnDevouredIsEquippingAttempt(Entity<DevouredComponent> devoured, ref IsEquippingAttemptEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<UsableWhileDevouredComponent>(args.Equipment))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnDevouredIsUnequippingAttempt(Entity<DevouredComponent> devoured, ref IsUnequippingAttemptEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		UsableWhileDevouredComponent usableDevoured = default(UsableWhileDevouredComponent);
		if (!((EntitySystem)this).TryComp<UsableWhileDevouredComponent>(args.Equipment, ref usableDevoured) || !usableDevoured.CanUnequip)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnDevouredShotAttempted(Entity<DevouredComponent> devoured, ref ShotAttemptedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<GunUsableWhileDevouredComponent>(Entity<GunComponent>.op_Implicit(args.Used)))
		{
			args.Cancel();
		}
	}

	private void OnDevouredMoveInput(Entity<DevouredComponent> devoured, ref MoveInputEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.HasDirectionalMovement)
		{
			DevouredHandleBreakout(devoured);
		}
	}

	private void OnDevouredTryAttack(Entity<DevouredComponent> devoured, ref RMCCombatModeInteractOverrideUserEvent args)
	{
		args.Handled = true;
		args.CanInteract = true;
	}

	private void UsuableByDevouredMeleeHit(Entity<UsableWhileDevouredComponent> weapon, ref MeleeHitEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DevouredComponent devoured = default(DevouredComponent);
		if (((EntitySystem)this).TryComp<DevouredComponent>(args.User, ref devoured))
		{
			((HandledEntityEventArgs)args).Handled = true;
			DevouredHandleBreakout(Entity<DevouredComponent>.op_Implicit((args.User, devoured)));
		}
	}

	private void DevouredHandleBreakout(Entity<DevouredComponent> devoured)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted || _timing.CurTime < devoured.Comp.NextDevouredAttackTimeAllowed || ((EntitySystem)this).HasComp<StunnedComponent>(Entity<DevouredComponent>.op_Implicit(devoured)) || !_mobState.IsAlive(Entity<DevouredComponent>.op_Implicit(devoured)))
		{
			return;
		}
		devoured.Comp.NextDevouredAttackTimeAllowed = _timing.CurTime + devoured.Comp.TimeBetweenStruggles;
		((EntitySystem)this).Dirty<DevouredComponent>(devoured, (MetaDataComponent)null);
		BaseContainer container = default(BaseContainer);
		XenoDevourComponent devour = default(XenoDevourComponent);
		if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<DevouredComponent>.op_Implicit(devoured), null)), ref container) || !((EntitySystem)this).TryComp<XenoDevourComponent>(container.Owner, ref devour) || container.ID != devour.DevourContainerId)
		{
			return;
		}
		EntityUid? weapon = _hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(devoured.Owner));
		UsableWhileDevouredComponent usuable = default(UsableWhileDevouredComponent);
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (!weapon.HasValue || !((EntitySystem)this).TryComp<UsableWhileDevouredComponent>(weapon, ref usuable) || !((EntitySystem)this).TryComp<MeleeWeaponComponent>(weapon, ref melee))
		{
			return;
		}
		DamageSpecifier totalDamage = new DamageSpecifier(_meleeWeapon.GetDamage(weapon.Value, Entity<DevouredComponent>.op_Implicit(devoured)));
		BonusMeleeDamageComponent bonusDamageComp = default(BonusMeleeDamageComponent);
		if (((EntitySystem)this).TryComp<BonusMeleeDamageComponent>(weapon.Value, ref bonusDamageComp) && bonusDamageComp.BonusDamage != null)
		{
			totalDamage += bonusDamageComp.BonusDamage;
		}
		TransformChildrenEnumerator children = ((EntitySystem)this).Transform(weapon.Value).ChildEnumerator;
		EntityUid childUid = default(EntityUid);
		AttachableWeaponMeleeModsComponent modsComp = default(AttachableWeaponMeleeModsComponent);
		while (((TransformChildrenEnumerator)(ref children)).MoveNext(ref childUid))
		{
			if (!((EntitySystem)this).TryComp<AttachableWeaponMeleeModsComponent>(childUid, ref modsComp))
			{
				continue;
			}
			foreach (AttachableWeaponMeleeModifierSet set in modsComp.Modifiers)
			{
				if (set.BonusDamage != null)
				{
					totalDamage += set.BonusDamage;
				}
			}
		}
		melee.NextAttack = devoured.Comp.NextDevouredAttackTimeAllowed;
		((EntitySystem)this).Dirty(weapon.Value, (IComponent)(object)melee, (MetaDataComponent)null);
		DamageSpecifier damage = _damage.TryChangeDamage(container.Owner, totalDamage, ignoreResistances: true, interruptsDoAfters: false, null, Entity<DevouredComponent>.op_Implicit(devoured), weapon);
		_audio.PlayPredicted(melee.HitSound, container.Owner.ToCoordinates(), (EntityUid?)Entity<DevouredComponent>.op_Implicit(devoured), (AudioParams?)null);
		if (damage?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(container.Owner, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == devoured.Owner));
			_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { container.Owner }, filter);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(52, 4);
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DevouredComponent>.op_Implicit(devoured), (MetaDataComponent)null), "actor", "ToPrettyString(devoured)");
			handler.AppendLiteral(" attacked while devoured by ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(container.Owner)), "subject", "ToPrettyString(container.Owner)");
			handler.AppendLiteral(" with ");
			handler.AppendFormatted(weapon, "weapon");
			handler.AppendLiteral(" and dealt ");
			handler.AppendFormatted(damage.GetTotal(), "damage", "damage.GetTotal()");
			handler.AppendLiteral(" damage");
			adminLogger.Add(LogType.MeleeHit, LogImpact.Medium, ref handler);
		}
	}

	private void OnXenoCanDropTarget(Entity<XenoDevourComponent> xeno, ref CanDropTargetEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled && ((EntitySystem)this).HasComp<DevourableComponent>(args.Dragged) && xeno.Owner == args.User)
		{
			args.CanDrop = true;
			args.Handled = true;
		}
	}

	private void OnXenoActivate(Entity<XenoDevourComponent> xeno, ref ActivateInWorldEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.User != args.Target) && StartDevourPulled(args.User))
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnXenoDevourDoAfterAttempt(Entity<XenoDevourComponent> ent, ref DoAfterAttemptEvent<XenoDevourDoAfterEvent> args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.DoAfter.Args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (CanDevour(Entity<XenoDevourComponent>.op_Implicit(ent), target2, out XenoDevourComponent _, popup: true))
			{
				return;
			}
		}
		((CancellableEntityEventArgs)args).Cancel();
	}

	private void OnXenoDevourDoAfter(Entity<XenoDevourComponent> xeno, ref XenoDevourDoAfterEvent args)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		if (!CanDevour(Entity<XenoDevourComponent>.op_Implicit(xeno), target2, out XenoDevourComponent _, popup: true))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		XenoTargetDevouredAttemptEvent attemptEv = default(XenoTargetDevouredAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoTargetDevouredAttemptEvent>(target2, ref attemptEv, false);
		if (attemptEv.Cancelled)
		{
			return;
		}
		EntityUid targetName = Identity.Entity(target2, (IEntityManager)(object)base.EntityManager, Entity<XenoDevourComponent>.op_Implicit(xeno));
		ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<XenoDevourComponent>.op_Implicit(xeno), xeno.Comp.DevourContainerId, (ContainerManagerComponent)null);
		if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(target2), (BaseContainer)(object)container, (TransformComponent)null, false))
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-failed", (ValueTuple<string, object>)("target", targetName)), Entity<XenoDevourComponent>.op_Implicit(xeno), Entity<XenoDevourComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		DevouredComponent devouredComponent = ((EntitySystem)this).EnsureComp<DevouredComponent>(target2);
		devouredComponent.WarnAt = _timing.CurTime + xeno.Comp.WarnAfter;
		devouredComponent.RegurgitateAt = _timing.CurTime + xeno.Comp.RegurgitateAfter;
		devouredComponent.NextDevouredAttackTimeAllowed = TimeSpan.Zero;
		_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-self", (ValueTuple<string, object>)("target", targetName)), Entity<XenoDevourComponent>.op_Implicit(xeno), Entity<XenoDevourComponent>.op_Implicit(xeno), PopupType.Medium);
		_popup.PopupEntity(base.Loc.GetString("cm-xeno-devour-target", (ValueTuple<string, object>)("user", xeno.Owner)), Entity<XenoDevourComponent>.op_Implicit(xeno), target2, PopupType.MediumCaution);
		foreach (ICommonSession recipient2 in Filter.PvsExcept(Entity<XenoDevourComponent>.op_Implicit(xeno), 2f, (IEntityManager)null).RemovePlayerByAttachedEntity(target2).Recipients)
		{
			target = recipient2.AttachedEntity;
			if (target.HasValue)
			{
				EntityUid recipient = target.GetValueOrDefault();
				_popup.PopupEntity(base.Loc.GetString("cm-xeno-devour-observer", (ValueTuple<string, object>)("user", Identity.Entity(Entity<XenoDevourComponent>.op_Implicit(xeno), (IEntityManager)(object)base.EntityManager)), (ValueTuple<string, object>)("target", targetName)), Entity<XenoDevourComponent>.op_Implicit(xeno), recipient, PopupType.MediumCaution);
			}
		}
		XenoDevouredEvent ev = new XenoDevouredEvent(target2, xeno.Owner);
		((EntitySystem)this).RaiseLocalEvent<XenoDevouredEvent>(target2, ref ev, true);
	}

	private void OnXenoRegurgitateAction(Entity<XenoDevourComponent> xeno, ref XenoRegurgitateActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<XenoDevourComponent>.op_Implicit(xeno), xeno.Comp.DevourContainerId, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-none-devoured"), Entity<XenoDevourComponent>.op_Implicit(xeno), Entity<XenoDevourComponent>.op_Implicit(xeno));
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		List<EntityUid> list = _container.EmptyContainer(container, false, (EntityCoordinates?)null, true);
		_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-hurl-out"), Entity<XenoDevourComponent>.op_Implicit(xeno), Entity<XenoDevourComponent>.op_Implicit(xeno), PopupType.MediumCaution);
		_audio.PlayPredicted(xeno.Comp.RegurgitateSound, Entity<XenoDevourComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoDevourComponent>.op_Implicit(xeno), (AudioParams?)null);
		foreach (EntityUid ent in list)
		{
			RegurgitateEvent ev = new RegurgitateEvent(((EntitySystem)this).GetNetEntity(xeno.Owner, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(ent, (MetaDataComponent)null));
			((EntitySystem)this).RaiseLocalEvent<RegurgitateEvent>(Entity<XenoDevourComponent>.op_Implicit(xeno), ev, false);
			_stun.TryStun(ent, xeno.Comp.RegurgitationStun, refresh: true);
		}
	}

	private void OnXenoTerminating(Entity<XenoDevourComponent> xeno, ref EntityTerminatingEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			RegurgitateAll(xeno);
		}
	}

	private void OnXenoMobStateChanged(Entity<XenoDevourComponent> xeno, ref MobStateChangedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState == MobState.Dead)
		{
			RegurgitateAll(xeno);
		}
	}

	private void OnXenoCombatModeInteract(Entity<XenoDevourComponent> ent, ref RMCCombatModeInteractOverrideUserEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		EntityUid owner = ent.Owner;
		EntityUid? target = args.Target;
		if (!target.HasValue || owner != target.GetValueOrDefault())
		{
			return;
		}
		target = ((EntitySystem)this).CompOrNull<PullerComponent>(Entity<XenoDevourComponent>.op_Implicit(ent))?.Pulling;
		if (target.HasValue)
		{
			EntityUid pulling = target.GetValueOrDefault();
			if (((EntitySystem)this).HasComp<DevourableComponent>(pulling))
			{
				args.Handled = true;
			}
		}
	}

	private void OnXenoDevouredVentAttempt(Entity<XenoDevourComponent> ent, ref VentEnterAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<XenoDevourComponent>.op_Implicit(ent), ent.Comp.DevourContainerId, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		foreach (EntityUid devouredEnt in container.ContainedEntities)
		{
			if (!((EntitySystem)this).HasComp<InfectStopOnDeathComponent>(devouredEnt))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-vent-crawling-devoured"), Entity<XenoDevourComponent>.op_Implicit(ent), Entity<XenoDevourComponent>.op_Implicit(ent), PopupType.SmallCaution);
				((CancellableEntityEventArgs)args).Cancel();
				break;
			}
		}
	}

	private void OnUsableWhileDevouredGetArmorPiercing(Entity<UsableWhileDevouredComponent> ent, ref CMGetArmorPiercingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (IsHeldByDevoured(Entity<UsableWhileDevouredComponent>.op_Implicit(ent)))
		{
			args.Piercing += 100;
		}
	}

	private bool IsHeldByDevoured(EntityUid item)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer marine = default(BaseContainer);
		BaseContainer xeno = default(BaseContainer);
		XenoDevourComponent devour = default(XenoDevourComponent);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(item, null)), ref marine) && _devouredQuery.HasComp(marine.Owner) && _hands.IsHolding(Entity<HandsComponent>.op_Implicit(marine.Owner), item) && _container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(marine.Owner, null)), ref xeno) && _xenoDevourQuery.TryComp(xeno.Owner, ref devour))
		{
			return xeno.ID == devour.DevourContainerId;
		}
		return false;
	}

	private bool CanDevour(EntityUid xeno, EntityUid victim, [NotNullWhen(true)] out XenoDevourComponent? devour, bool popup = false)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		devour = null;
		if (xeno == victim || !((EntitySystem)this).TryComp<XenoDevourComponent>(xeno, ref devour) || ((EntitySystem)this).HasComp<DevouredComponent>(victim) || !((EntitySystem)this).HasComp<DevourableComponent>(victim))
		{
			return false;
		}
		if (_mobState.IsIncapacitated(xeno) || ((EntitySystem)this).HasComp<XenoNestedComponent>(victim))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-failed-cant-now"), victim, xeno);
			}
			return false;
		}
		if (((EntitySystem)this).HasComp<SynthComponent>(victim) || ((EntitySystem)this).HasComp<RMCTrainingDummyComponent>(victim))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-fake-host"), victim, xeno);
			}
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoComponent>(victim))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-success"), victim, xeno);
			}
			return false;
		}
		if (_mobState.IsDead(victim))
		{
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-failed-target-roting", (ValueTuple<string, object>)("target", victim)), victim, xeno);
			}
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainer(xeno, devour.DevourContainerId, ref container, (ContainerManagerComponent)null) && container.ContainedEntities.Count > 0)
		{
			devour = null;
			if (popup)
			{
				_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-failed-stomach-full"), victim, xeno, PopupType.SmallCaution);
			}
			return false;
		}
		BuckleComponent buckle = default(BuckleComponent);
		if (((EntitySystem)this).TryComp<BuckleComponent>(victim, ref buckle))
		{
			EntityUid? buckledTo = buckle.BuckledTo;
			if (buckledTo.HasValue)
			{
				EntityUid strap = buckledTo.GetValueOrDefault();
				if (popup)
				{
					_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-failed-target-buckled", (ValueTuple<string, object>)("strap", strap), (ValueTuple<string, object>)("target", victim)), victim, xeno);
				}
			}
		}
		return true;
	}

	private bool StartDevour(EntityUid xeno, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		if (!CanDevour(xeno, target, out XenoDevourComponent devour, popup: true))
		{
			return false;
		}
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, xeno, devour.DevourDelay, new XenoDevourDoAfterEvent(), xeno, target)
		{
			BreakOnMove = true,
			AttemptFrequency = AttemptFrequency.EveryTick,
			ForceVisible = true
		};
		IdentityEntity targetName = Identity.Name(target, (IEntityManager)(object)base.EntityManager, xeno);
		_popup.PopupClient(base.Loc.GetString("cm-xeno-devour-start-self", (ValueTuple<string, object>)("target", targetName)), target, xeno);
		_popup.PopupEntity(base.Loc.GetString("cm-xeno-devour-start-target", (ValueTuple<string, object>)("user", xeno)), xeno, target, PopupType.MediumCaution);
		foreach (ICommonSession recipient2 in Filter.PvsExcept(xeno, 2f, (IEntityManager)null).RemovePlayerByAttachedEntity(target).Recipients)
		{
			EntityUid? attachedEntity = recipient2.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid recipient = attachedEntity.GetValueOrDefault();
				_popup.PopupEntity(base.Loc.GetString("cm-xeno-devour-start-observer", (ValueTuple<string, object>)("user", xeno), (ValueTuple<string, object>)("target", targetName)), target, recipient, PopupType.SmallCaution);
			}
		}
		_doAfter.TryStartDoAfter(doAfter);
		return true;
	}

	private bool StartDevourPulled(EntityUid xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = ((EntitySystem)this).CompOrNull<PullerComponent>(xeno)?.Pulling;
		if (val.HasValue)
		{
			EntityUid pulling = val.GetValueOrDefault();
			return StartDevour(xeno, pulling);
		}
		return false;
	}

	private bool Regurgitate(Entity<DevouredComponent> devoured, Entity<XenoDevourComponent?> xeno, bool doFeedback = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoDevourComponent>(Entity<XenoDevourComponent>.op_Implicit(xeno), ref xeno.Comp, true))
		{
			return true;
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<XenoDevourComponent>.op_Implicit(xeno), xeno.Comp.DevourContainerId, ref container, (ContainerManagerComponent)null) || !_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(devoured.Owner), container, true, false, (EntityCoordinates?)null, (Angle?)null))
		{
			return true;
		}
		RegurgitateEvent ev = new RegurgitateEvent(((EntitySystem)this).GetNetEntity(xeno.Owner, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(devoured.Owner, (MetaDataComponent)null));
		((EntitySystem)this).RaiseLocalEvent<RegurgitateEvent>(Entity<XenoDevourComponent>.op_Implicit(xeno), ev, false);
		if (doFeedback)
		{
			DoFeedback(Entity<XenoDevourComponent>.op_Implicit((Entity<XenoDevourComponent>.op_Implicit(xeno), xeno.Comp)));
		}
		return false;
	}

	private void RegurgitateAll(Entity<XenoDevourComponent> xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<XenoDevourComponent>.op_Implicit(xeno), xeno.Comp.DevourContainerId, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		bool any = false;
		DevouredComponent devoured = default(DevouredComponent);
		foreach (EntityUid contained in container.ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<DevouredComponent>(contained, ref devoured) && Regurgitate(Entity<DevouredComponent>.op_Implicit((contained, devoured)), Entity<XenoDevourComponent>.op_Implicit((Entity<XenoDevourComponent>.op_Implicit(xeno), Entity<XenoDevourComponent>.op_Implicit(xeno))), doFeedback: false))
			{
				any = true;
			}
		}
		if (any)
		{
			DoFeedback(xeno);
		}
	}

	private void DoFeedback(Entity<XenoDevourComponent> xeno)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			_popup.PopupEntity(base.Loc.GetString("cm-xeno-devour-hurl-out"), Entity<XenoDevourComponent>.op_Implicit(xeno), Entity<XenoDevourComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			_audio.PlayPvs(xeno.Comp.RegurgitateSound, Entity<XenoDevourComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<DevouredComponent, TransformComponent> devoured = ((EntitySystem)this).EntityQueryEnumerator<DevouredComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		DevouredComponent comp = default(DevouredComponent);
		TransformComponent xform = default(TransformComponent);
		BaseContainer container = default(BaseContainer);
		XenoDevourComponent devour = default(XenoDevourComponent);
		while (devoured.MoveNext(ref uid, ref comp, ref xform))
		{
			if (!_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, xform)), ref container) || !((EntitySystem)this).TryComp<XenoDevourComponent>(container.Owner, ref devour) || container.ID != devour.DevourContainerId)
			{
				((EntitySystem)this).RemCompDeferred<DevouredComponent>(uid);
				continue;
			}
			EntityUid xeno = container.Owner;
			if (_mobState.IsDead(uid))
			{
				Regurgitate(Entity<DevouredComponent>.op_Implicit((uid, comp)), Entity<XenoDevourComponent>.op_Implicit((xeno, devour)));
				continue;
			}
			if (!comp.Warned && time >= comp.WarnAt)
			{
				comp.Warned = true;
				_popup.PopupEntity(base.Loc.GetString("cm-xeno-devour-regurgitate", (ValueTuple<string, object>)("target", uid)), xeno, xeno, PopupType.MediumCaution);
			}
			if (time >= comp.RegurgitateAt && Regurgitate(Entity<DevouredComponent>.op_Implicit((uid, comp)), Entity<XenoDevourComponent>.op_Implicit((xeno, devour))))
			{
				_popup.PopupEntity(base.Loc.GetString("cm-xeno-devour-hurl-out"), xeno, xeno, PopupType.MediumCaution);
			}
		}
	}
}
