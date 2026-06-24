using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared._RMC14.Buckle;
using Content.Shared._RMC14.Construction;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Folded;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Scoping;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared._RMC14.Weapons.Ranged.Overheat;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.CombatMode;
using Content.Shared.Construction.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.Foldable;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Emplacements;

public abstract class SharedWeaponMountSystem : EntitySystem
{
	[Dependency]
	protected SharedXenoAcidSystem XenoAcid;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private ActionBlockerSystem _actionBlockerSystem;

	[Dependency]
	private ActionContainerSystem _actConts;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private BarricadeSystem _barricade;

	[Dependency]
	private SharedBuckleSystem _buckle;

	[Dependency]
	private RMCBuckleSystem _rmcBuckle;

	[Dependency]
	private CollisionWakeSystem _collisionWake;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedCombatModeSystem _combatMode;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private FoldableSystem _foldable;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedVirtualItemSystem _virtualItem;

	[Dependency]
	private RMCFoldableSystem _rmcFoldable;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private SharedScopeSystem _scope;

	[Dependency]
	private ItemSlotsSystem _slots;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	private const string AmmoExamineColor = "yellow";

	private const string FireRateExamineColor = "yellow";

	private const string ModeExamineColor = "cyan";

	private const string ToolExamineColor = "cyan";

	private const string MagazineKey = "gun_magazine";

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, InteractUsingEvent>((EntityEventRefHandler<WeaponMountComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, FoldAttemptEvent>((EntityEventRefHandler<WeaponMountComponent, FoldAttemptEvent>)OnFoldAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, AnchorAttemptEvent>((EntityEventRefHandler<WeaponMountComponent, AnchorAttemptEvent>)OnAnchorAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, UnanchorAttemptEvent>((EntityEventRefHandler<WeaponMountComponent, UnanchorAttemptEvent>)OnUnanchorAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, UseInHandEvent>((EntityEventRefHandler<WeaponMountComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, StrapAttemptEvent>((EntityEventRefHandler<WeaponMountComponent, StrapAttemptEvent>)OnStrapAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, StrappedEvent>((EntityEventRefHandler<WeaponMountComponent, StrappedEvent>)OnStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, UnstrappedEvent>((EntityEventRefHandler<WeaponMountComponent, UnstrappedEvent>)OnUnStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, ExaminedEvent>((EntityEventRefHandler<WeaponMountComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, MapInitEvent>((EntityEventRefHandler<WeaponMountComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, DismountActionEvent>((EntityEventRefHandler<WeaponMountComponent, DismountActionEvent>)OnDismountAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponControllerComponent, MoveInputEvent>((EntityEventRefHandler<WeaponControllerComponent, MoveInputEvent>)OnMountedMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<WeaponMountComponent, GetVerbsEvent<AlternativeVerb>>)OnAltVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, BreakageEventArgs>((EntityEventRefHandler<WeaponMountComponent, BreakageEventArgs>)OnBreak, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, DamageModifyEvent>((EntityEventRefHandler<WeaponMountComponent, DamageModifyEvent>)OnDamageModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, RMCCheckTileFreeEvent>((EntityEventRefHandler<WeaponMountComponent, RMCCheckTileFreeEvent>)OnCheckTileFree, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, GetIFFGunUserEvent>((EntityEventRefHandler<WeaponMountComponent, GetIFFGunUserEvent>)OnGetGunUser, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, InteractHandEvent>((EntityEventRefHandler<WeaponMountComponent, InteractHandEvent>)OnInteractHand, new Type[1] { typeof(SharedBuckleSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, CanDropTargetEvent>((EntityEventRefHandler<WeaponMountComponent, CanDropTargetEvent>)OnMountCanDropTarget, (Type[])null, new Type[1] { typeof(SharedBuckleSystem) });
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, MountableWeaponRelayedEvent<OverheatedEvent>>((EntityEventRefHandler<WeaponMountComponent, MountableWeaponRelayedEvent<OverheatedEvent>>)OnWeaponOverheated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, MountableWeaponRelayedEvent<HeatGainedEvent>>((EntityEventRefHandler<WeaponMountComponent, MountableWeaponRelayedEvent<HeatGainedEvent>>)OnWeaponHeatGained, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, AttachToMountDoAfterEvent>((EntityEventRefHandler<WeaponMountComponent, AttachToMountDoAfterEvent>)OnAttachToMount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, SecureToMountDoAfterEvent>((EntityEventRefHandler<WeaponMountComponent, SecureToMountDoAfterEvent>)OnSecureToMount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, DetachFromMountDoAfterEvent>((EntityEventRefHandler<WeaponMountComponent, DetachFromMountDoAfterEvent>)OnDetachFromMount, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, MountDeployDoafterEvent>((EntityEventRefHandler<WeaponMountComponent, MountDeployDoafterEvent>)OnMountDeploy, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, MountUnDeployDoAfterEvent>((EntityEventRefHandler<WeaponMountComponent, MountUnDeployDoAfterEvent>)OnMountUndeploy, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<WeaponMountComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		EnsureDismountAction(ent);
		if (!ent.Comp.FixedWeaponPrototype.HasValue)
		{
			return;
		}
		ContainerSlot obj = _container.EnsureContainer<ContainerSlot>(Entity<WeaponMountComponent>.op_Implicit(ent), ent.Comp.WeaponSlotId, (ContainerManagerComponent)null);
		((BaseContainer)obj).OccludesLight = false;
		if (((BaseContainer)obj).ContainedEntities.Count <= 0)
		{
			EntProtoId? fixedWeaponPrototype = ent.Comp.FixedWeaponPrototype;
			EntityUid weapon = ((EntitySystem)this).SpawnInContainerOrDrop(fixedWeaponPrototype.HasValue ? EntProtoId.op_Implicit(fixedWeaponPrototype.GetValueOrDefault()) : null, Entity<WeaponMountComponent>.op_Implicit(ent), ent.Comp.WeaponSlotId, (TransformComponent)null, (ContainerManagerComponent)null, (ComponentRegistry)null);
			ent.Comp.MountedEntity = weapon;
			((EntitySystem)this).DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "MountedEntity", (MetaDataComponent)null);
			MountableWeaponComponent mountedWeapon = default(MountableWeaponComponent);
			if (((EntitySystem)this).TryComp<MountableWeaponComponent>(weapon, ref mountedWeapon))
			{
				mountedWeapon.MountedTo = ((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null);
				((EntitySystem)this).Dirty(weapon, (IComponent)(object)mountedWeapon, (MetaDataComponent)null);
			}
		}
	}

	private void OnInteractUsing(Entity<WeaponMountComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		BallisticAmmoProviderComponent ballistic = default(BallisticAmmoProviderComponent);
		FoldableComponent foldable = default(FoldableComponent);
		if (ent.Comp.MountedEntity.HasValue && _slots.TryGetSlot(ent.Comp.MountedEntity.Value, "gun_magazine", out ItemSlot itemSlot) && ((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(args.Used, ref ballistic))
		{
			HandsComponent hands = default(HandsComponent);
			if (((EntitySystem)this).TryComp<HandsComponent>(args.User, ref hands) && _slots.CanInsert(ent.Comp.MountedEntity.Value, args.Used, args.User, itemSlot, swap: true) && _hands.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), args.Used))
			{
				if (itemSlot.Item.HasValue)
				{
					_hands.TryPickupAnyHand(args.User, itemSlot.Item.Value, checkActionBlocker: true, animateUser: false, animate: true, hands);
				}
				_slots.TryInsert(ent.Comp.MountedEntity.Value, "gun_magazine", args.Used, args.User, null, excludeUserAudio: true);
				WeaponMountComponentVisualLayers ammoSpriteKey = WeaponMountComponentVisualLayers.MountedAmmo;
				FoldableComponent foldableComp = default(FoldableComponent);
				if (((EntitySystem)this).TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref foldableComp) && foldableComp.IsFolded)
				{
					ammoSpriteKey = WeaponMountComponentVisualLayers.FoldedAmmo;
				}
				_appearance.SetData(Entity<WeaponMountComponent>.op_Implicit(ent), (Enum)ammoSpriteKey, (object)(ballistic.Count > 0), (AppearanceComponent)null);
			}
		}
		else if (!ent.Comp.IsWeaponLocked && (!((EntitySystem)this).TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref foldable) || !foldable.IsFolded))
		{
			BaseContainer container = default(BaseContainer);
			if (((EntitySystem)this).HasComp<MountableWeaponComponent>(args.Used) && ((EntitySystem)this).Transform(Entity<WeaponMountComponent>.op_Implicit(ent)).Anchored && !ent.Comp.MountedEntity.HasValue)
			{
				TryAttachToMount(ent, args.User, args.Used);
			}
			else if (_tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.RotationTool)))
			{
				RotateMount(ent, args.User);
			}
			else if (_tool.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(ent.Comp.DismantlingTool)) && _container.TryGetContainer(Entity<WeaponMountComponent>.op_Implicit(ent), ent.Comp.WeaponSlotId, ref container, (ContainerManagerComponent)null) && container.ContainedEntities.Count > 0 && !ent.Comp.IsWeaponSecured)
			{
				TryDetachFromMount(ent, args.User, args.Used);
			}
		}
	}

	private void OnUseInHand(Entity<WeaponMountComponent> ent, ref UseInHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.MountedEntity.HasValue)
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (CanDeployPopup(ent, args.User, out var _, out var _))
			{
				DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, ent.Comp.AssembleDelay, new MountDeployDoafterEvent(), Entity<WeaponMountComponent>.op_Implicit(ent), Entity<WeaponMountComponent>.op_Implicit(ent), args.User)
				{
					NeedHand = true,
					BreakOnMove = true,
					BreakOnHandChange = true
				};
				_doAfter.TryStartDoAfter(new DoAfterArgs(doAfterArgs));
			}
		}
	}

	private void OnFoldAttempt(Entity<WeaponMountComponent> ent, ref FoldAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Transform(Entity<WeaponMountComponent>.op_Implicit(ent)).Anchored || ent.Comp.MountedEntity.HasValue)
		{
			args.Cancelled = true;
		}
	}

	private void OnAnchorAttempt(Entity<WeaponMountComponent> ent, ref AnchorAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldable = default(FoldableComponent);
		if (((EntitySystem)this).TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref foldable) && foldable.IsFolded)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnUnanchorAttempt(Entity<WeaponMountComponent> ent, ref UnanchorAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldable = default(FoldableComponent);
		if (((EntitySystem)this).TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref foldable) && foldable.IsFolded)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
		else if (ent.Comp.MountedEntity.HasValue)
		{
			((CancellableEntityEventArgs)args).Cancel();
			if (!ent.Comp.IsWeaponSecured)
			{
				DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, ent.Comp.AssembleDelay, new SecureToMountDoAfterEvent(), Entity<WeaponMountComponent>.op_Implicit(ent), Entity<WeaponMountComponent>.op_Implicit(ent), args.Tool)
				{
					NeedHand = true,
					BreakOnMove = true,
					BreakOnHandChange = true
				};
				_doAfter.TryStartDoAfter(new DoAfterArgs(doAfterArgs));
			}
			else if (foldable != null)
			{
				TryUndeployMount(ent, args.User, args.Tool);
			}
		}
	}

	private void OnAttachToMount(Entity<WeaponMountComponent> ent, ref AttachToMountDoAfterEvent args)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		MountableWeaponComponent weapon = default(MountableWeaponComponent);
		if (!args.Cancelled && ((EntitySystem)this).TryComp<MountableWeaponComponent>(args.Used, ref weapon) && args.Used.HasValue && CanAssembleMount(ent, args.User))
		{
			ContainerSlot container = _container.EnsureContainer<ContainerSlot>(Entity<WeaponMountComponent>.op_Implicit(ent), ent.Comp.WeaponSlotId, (ContainerManagerComponent)null);
			((BaseContainer)container).OccludesLight = false;
			if (((BaseContainer)container).ContainedEntities.Count <= 0 && _container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used.Value), (BaseContainer)(object)container, (TransformComponent)null, false))
			{
				weapon.MountedTo = ((EntitySystem)this).GetNetEntity(Entity<WeaponMountComponent>.op_Implicit(ent), (MetaDataComponent)null);
				ent.Comp.MountedEntity = args.Used;
				_collisionWake.SetEnabled(Entity<WeaponMountComponent>.op_Implicit(ent), false, (CollisionWakeComponent)null);
				_item.SetSize(Entity<WeaponMountComponent>.op_Implicit(ent), ent.Comp.MountedWeaponSize);
				_rmcFoldable.TryLockFold(Entity<WeaponMountComponent>.op_Implicit(ent), locked: true);
				((EntitySystem)this).DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "MountedEntity", (MetaDataComponent)null);
				UpdateAppearance(Entity<WeaponMountComponent>.op_Implicit(ent));
				_audio.PlayPredicted(ent.Comp.RotateSound, Entity<WeaponMountComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
			}
		}
	}

	private void OnSecureToMount(Entity<WeaponMountComponent> ent, ref SecureToMountDoAfterEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			ent.Comp.IsWeaponSecured = true;
			_buckle.StrapSetEnabled(Entity<WeaponMountComponent>.op_Implicit(ent), enabled: true);
			MetaDataComponent mountedMeta = default(MetaDataComponent);
			if (((EntitySystem)this).TryComp(ent.Comp.MountedEntity, ref mountedMeta) && mountedMeta.EntityPrototype != null)
			{
				_metaData.SetEntityName(Entity<WeaponMountComponent>.op_Implicit(ent), mountedMeta.EntityName, (MetaDataComponent)null, true);
				_metaData.SetEntityDescription(Entity<WeaponMountComponent>.op_Implicit(ent), base.Loc.GetString("emplacement-mount-" + mountedMeta.EntityPrototype.ID + "-description-mounted"), (MetaDataComponent)null);
			}
			_audio.PlayPredicted(ent.Comp.SecureSound, Entity<WeaponMountComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	private void OnDetachFromMount(Entity<WeaponMountComponent> ent, ref DetachFromMountDoAfterEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!args.Cancelled && _container.TryGetContainer(Entity<WeaponMountComponent>.op_Implicit(ent), ent.Comp.WeaponSlotId, ref container, (ContainerManagerComponent)null))
		{
			_container.EmptyContainer(container, false, (EntityCoordinates?)null, true);
			MountableWeaponComponent attachedWeapon = default(MountableWeaponComponent);
			if (((EntitySystem)this).TryComp<MountableWeaponComponent>(ent.Comp.MountedEntity, ref attachedWeapon))
			{
				attachedWeapon.MountedTo = ((EntitySystem)this).GetNetEntity(Entity<WeaponMountComponent>.op_Implicit(ent), (MetaDataComponent)null);
			}
			MetaDataComponent mountedMeta = default(MetaDataComponent);
			if (((EntitySystem)this).TryComp(ent.Comp.MountedEntity, ref mountedMeta) && mountedMeta.EntityPrototype != null)
			{
				_metaData.SetEntityName(Entity<WeaponMountComponent>.op_Implicit(ent), base.Loc.GetString("emplacement-mount-" + mountedMeta.EntityPrototype.ID + "-name"), (MetaDataComponent)null, true);
				_metaData.SetEntityDescription(Entity<WeaponMountComponent>.op_Implicit(ent), base.Loc.GetString("emplacement-mount-" + mountedMeta.EntityPrototype.ID + "-description"), (MetaDataComponent)null);
			}
			ent.Comp.MountedEntity = null;
			_buckle.StrapSetEnabled(Entity<WeaponMountComponent>.op_Implicit(ent), enabled: false);
			_collisionWake.SetEnabled(Entity<WeaponMountComponent>.op_Implicit(ent), true, (CollisionWakeComponent)null);
			_item.SetSize(Entity<WeaponMountComponent>.op_Implicit(ent), ent.Comp.MountSize);
			_rmcFoldable.TryLockFold(Entity<WeaponMountComponent>.op_Implicit(ent), locked: false);
			((EntitySystem)this).DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "MountedEntity", (MetaDataComponent)null);
			UpdateAppearance(Entity<WeaponMountComponent>.op_Implicit(ent));
			_audio.PlayPredicted(ent.Comp.DetachSound, Entity<WeaponMountComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	private void OnMountDeploy(Entity<WeaponMountComponent> ent, ref MountDeployDoafterEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || !CanDeployPopup(ent, args.User, out var coordinates, out var rotation))
		{
			return;
		}
		FoldableComponent foldable = default(FoldableComponent);
		if (((EntitySystem)this).TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref foldable))
		{
			_foldable.SetFolded(Entity<WeaponMountComponent>.op_Implicit(ent), foldable, folded: false);
		}
		MetaDataComponent mountedMeta = default(MetaDataComponent);
		if (((EntitySystem)this).TryComp(ent.Comp.MountedEntity, ref mountedMeta) && ent.Comp.IsWeaponLocked && mountedMeta.EntityPrototype != null)
		{
			_metaData.SetEntityDescription(Entity<WeaponMountComponent>.op_Implicit(ent), base.Loc.GetString("emplacement-mount-" + mountedMeta.EntityPrototype.ID + "-description-mounted"), (MetaDataComponent)null);
		}
		TransformComponent xform = ((EntitySystem)this).Transform(Entity<WeaponMountComponent>.op_Implicit(ent));
		_transform.SetCoordinates(Entity<WeaponMountComponent>.op_Implicit(ent), xform, coordinates, (Angle?)rotation, true, (TransformComponent)null, (TransformComponent)null);
		_transform.AnchorEntity(Entity<WeaponMountComponent>.op_Implicit(ent), xform);
		_collisionWake.SetEnabled(Entity<WeaponMountComponent>.op_Implicit(ent), false, (CollisionWakeComponent)null);
		if (ent.Comp.MountOnDeploy && ent.Comp.MountedEntity.HasValue)
		{
			GetAmmoCountEvent ammoCountEvent = default(GetAmmoCountEvent);
			((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(ent.Comp.MountedEntity.Value, ref ammoCountEvent, false);
			if (ammoCountEvent.Count > 0)
			{
				_buckle.TryBuckle(args.User, args.User, Entity<WeaponMountComponent>.op_Implicit(ent), null, popup: false);
			}
		}
		UpdateAppearance(Entity<WeaponMountComponent>.op_Implicit(ent));
		_audio.PlayPredicted(ent.Comp.DeploySound, Entity<WeaponMountComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
	}

	private void OnMountUndeploy(Entity<WeaponMountComponent> ent, ref MountUnDeployDoAfterEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldable = default(FoldableComponent);
		if (!args.Cancelled && ((EntitySystem)this).TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref foldable))
		{
			UndeployMount(ent, args.User, foldable);
		}
	}

	public void UndeployMount(Entity<WeaponMountComponent> ent, EntityUid? user = null, FoldableComponent? foldable = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent mountedMeta = default(MetaDataComponent);
		if (((EntitySystem)this).TryComp(ent.Comp.MountedEntity, ref mountedMeta) && ent.Comp.IsWeaponLocked && mountedMeta.EntityPrototype != null)
		{
			_metaData.SetEntityDescription(Entity<WeaponMountComponent>.op_Implicit(ent), base.Loc.GetString("emplacement-mount-" + mountedMeta.EntityPrototype.ID + "-description"), (MetaDataComponent)null);
		}
		ent.Comp.IsWeaponSecured = false;
		_transform.Unanchor(Entity<WeaponMountComponent>.op_Implicit(ent));
		if (foldable != null)
		{
			_foldable.SetFolded(Entity<WeaponMountComponent>.op_Implicit(ent), foldable, folded: true);
		}
		_buckle.StrapSetEnabled(Entity<WeaponMountComponent>.op_Implicit(ent), enabled: false);
		_collisionWake.SetEnabled(Entity<WeaponMountComponent>.op_Implicit(ent), true, (CollisionWakeComponent)null);
		HandsComponent hands = default(HandsComponent);
		if (((EntitySystem)this).TryComp<HandsComponent>(user, ref hands))
		{
			_hands.TryPickupAnyHand(user.Value, Entity<WeaponMountComponent>.op_Implicit(ent), checkActionBlocker: true, animateUser: false, animate: true, hands);
		}
		UpdateAppearance(Entity<WeaponMountComponent>.op_Implicit(ent));
		_audio.PlayPredicted(ent.Comp.UndeploySound, Entity<WeaponMountComponent>.op_Implicit(ent), user, (AudioParams?)null);
	}

	private bool CanDeployPopup(Entity<WeaponMountComponent> ent, EntityUid user, out EntityCoordinates coordinates, out Angle rotation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		(coordinates, rotation) = _transform.GetMoverCoordinateRotation(user, ((EntitySystem)this).Transform(user));
		if (ent.Comp.Broken)
		{
			string msg = base.Loc.GetString("emplacement-mount-deploy-broken", (ValueTuple<string, object>)("mount", ent));
			_popup.PopupClient(msg, user, user, PopupType.SmallCaution);
			return false;
		}
		Direction direction = ((Angle)(ref rotation)).GetCardinalDir();
		coordinates = ((EntityCoordinates)(ref coordinates)).Offset(DirectionExtensions.ToVec(direction));
		if (_rmcMap.IsTileBlocked(coordinates, CollisionGroup.MidImpassable))
		{
			string msg2 = base.Loc.GetString("rmc-sentry-need-open-area", (ValueTuple<string, object>)("sentry", ent));
			_popup.PopupClient(msg2, user, user, PopupType.SmallCaution);
			return false;
		}
		EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit((user, ((EntitySystem)this).Transform(user))));
		MapGridComponent mapGrid = default(MapGridComponent);
		if (((EntitySystem)this).TryComp<MapGridComponent>(grid, ref mapGrid))
		{
			if (HasWeaponMountsNearbyPopup(Entity<MapGridComponent>.op_Implicit((grid.Value, mapGrid)), user, coordinates, ent.Comp.MountExclusionAreaSize))
			{
				return false;
			}
			if (ent.Comp.BarricadeExclusionAreaSize != 0 && _barricade.HasBarricadeNearbyPopup(Entity<MapGridComponent>.op_Implicit((grid.Value, mapGrid)), user, coordinates, ent.Comp.BarricadeExclusionAreaSize))
			{
				return false;
			}
		}
		return true;
	}

	private void OnInteractHand(Entity<WeaponMountComponent> ent, ref InteractHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (!_combatMode.IsInCombatMode(args.User))
		{
			if (!ent.Comp.CanRotateWithoutTool || !ent.Comp.MountedEntity.HasValue || ent.Comp.User.HasValue || !((EntitySystem)this).Transform(Entity<WeaponMountComponent>.op_Implicit(ent)).Anchored)
			{
				return;
			}
			Vector2 mountPos = _transform.GetWorldPosition(Entity<WeaponMountComponent>.op_Implicit(ent));
			Vector2 userPos = _transform.GetWorldPosition(args.User);
			Vector2 diff = mountPos - userPos;
			if (diff.LengthSquared() > 0.0001f)
			{
				Angle rot = Angle.FromWorldVec(diff);
				_transform.SetWorldRotation(Entity<WeaponMountComponent>.op_Implicit(ent), rot);
				StrapComponent strap = default(StrapComponent);
				if (((EntitySystem)this).TryComp<StrapComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref strap))
				{
					Angle val = -rot;
					Vector2 vector = userPos - mountPos;
					Vector2 localOffset = ((Angle)(ref val)).RotateVec(ref vector) - _rmcBuckle.GetOffset(Entity<RMCBuckleOffsetComponent>.op_Implicit(args.User));
					_buckle.SetBuckleOffset(Entity<StrapComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(ent), strap)), localOffset);
				}
			}
		}
		else
		{
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnMountCanDropTarget(Entity<WeaponMountComponent> ent, ref CanDropTargetEvent args)
	{
		args.CanDrop = false;
		args.Handled = true;
	}

	private void OnStrapAttempt(Entity<WeaponMountComponent> ent, ref StrapAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BusyHands > 0 && _hands.CountFreeHands(Entity<HandsComponent>.op_Implicit(args.Buckle.Owner)) < ent.Comp.BusyHands)
		{
			if (args.Popup)
			{
				string msg = ((ent.Comp.BusyPopup != null) ? base.Loc.GetString(ent.Comp.BusyPopup) : base.Loc.GetString("mountable-weapon-no-free-hands"));
				_popup.PopupClient(msg, Entity<BuckleComponent>.op_Implicit(args.Buckle), args.User, PopupType.SmallCaution);
			}
			args.Cancelled = true;
		}
		else
		{
			EntityUid? user = args.User;
			EntityUid val = Entity<BuckleComponent>.op_Implicit(args.Buckle);
			if (!user.HasValue || user.GetValueOrDefault() != val)
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnStrapped(Entity<WeaponMountComponent> ent, ref StrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		if (!TakeHands(ent, args.Buckle.Owner))
		{
			_buckle.Unbuckle(Entity<BuckleComponent>.op_Implicit((args.Buckle.Owner, args.Buckle.Comp)), args.Buckle.Owner);
			return;
		}
		ent.Comp.User = Entity<BuckleComponent>.op_Implicit(args.Buckle);
		EnsureDismountAction(ent);
		_actions.AddAction(Entity<BuckleComponent>.op_Implicit(args.Buckle), ref ent.Comp.DismountActionEntity, ent.Comp.DismountAction, Entity<WeaponMountComponent>.op_Implicit(ent));
		if (ent.Comp.MountedEntity.HasValue)
		{
			WeaponControllerComponent weaponController = ((EntitySystem)this).EnsureComp<WeaponControllerComponent>(Entity<BuckleComponent>.op_Implicit(args.Buckle));
			weaponController.ControlledWeapon = ((EntitySystem)this).GetNetEntity(ent.Comp.MountedEntity.Value, (MetaDataComponent)null);
			((EntitySystem)this).Dirty(Entity<BuckleComponent>.op_Implicit(args.Buckle), (IComponent)(object)weaponController, (MetaDataComponent)null);
			ScopeComponent scope = default(ScopeComponent);
			if (((EntitySystem)this).TryComp<ScopeComponent>(ent.Comp.MountedEntity.Value, ref scope))
			{
				_scope.StartScoping(Entity<ScopeComponent>.op_Implicit((ent.Comp.MountedEntity.Value, scope)), Entity<BuckleComponent>.op_Implicit(args.Buckle));
			}
		}
	}

	private void OnUnStrapped(Entity<WeaponMountComponent> ent, ref UnstrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		FreeHands(ent.Owner, args.Buckle.Owner);
		ent.Comp.User = null;
		((EntitySystem)this).RemComp<WeaponControllerComponent>(Entity<BuckleComponent>.op_Implicit(args.Buckle));
		ScopeComponent scope = default(ScopeComponent);
		if (((EntitySystem)this).TryComp<ScopeComponent>(ent.Comp.MountedEntity, ref scope))
		{
			_scope.Unscope(Entity<ScopeComponent>.op_Implicit((ent.Comp.MountedEntity.Value, scope)));
		}
		SharedActionsSystem actions = _actions;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(args.Buckle.Owner);
		EntityUid? dismountActionEntity = ent.Comp.DismountActionEntity;
		actions.RemoveAction(performer, dismountActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(dismountActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
	}

	private void OnDismountAction(Entity<WeaponMountComponent> ent, ref DismountActionEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		if (!_net.IsClient)
		{
			_buckle.TryUnbuckle(args.Performer, args.Performer);
		}
	}

	private void OnMountedMoveInput(Entity<WeaponControllerComponent> ent, ref MoveInputEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (args.HasDirectionalMovement && !_net.IsClient)
		{
			_buckle.TryUnbuckle(ent.Owner, ent.Owner, null, popup: false);
		}
	}

	private void EnsureDismountAction(Entity<WeaponMountComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.DismountAction == null)
		{
			return;
		}
		EntityUid? dismountActionEntity = ent.Comp.DismountActionEntity;
		if (dismountActionEntity.HasValue)
		{
			EntityUid actionUid = dismountActionEntity.GetValueOrDefault();
			ActionComponent action = default(ActionComponent);
			if (((EntitySystem)this).TryComp<ActionComponent>(actionUid, ref action))
			{
				dismountActionEntity = action.Container;
				EntityUid owner = ent.Owner;
				if (!dismountActionEntity.HasValue || dismountActionEntity.GetValueOrDefault() != owner)
				{
					_actConts.RemoveAction(Entity<ActionComponent>.op_Implicit((actionUid, action)));
					ent.Comp.DismountActionEntity = null;
					((EntitySystem)this).DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "DismountActionEntity", (MetaDataComponent)null);
				}
			}
		}
		if (_actConts.EnsureAction(ent.Owner, ref ent.Comp.DismountActionEntity, ent.Comp.DismountAction))
		{
			((EntitySystem)this).DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "DismountActionEntity", (MetaDataComponent)null);
		}
	}

	private bool TakeHands(Entity<WeaponMountComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.BusyHands <= 0)
		{
			return true;
		}
		if (_hands.CountFreeHands(Entity<HandsComponent>.op_Implicit(user)) < ent.Comp.BusyHands)
		{
			return false;
		}
		int busy = 0;
		foreach (string hand in _hands.EnumerateHands(Entity<HandsComponent>.op_Implicit(user)))
		{
			if (_hands.HandIsEmpty(Entity<HandsComponent>.op_Implicit(user), hand))
			{
				if (!_virtualItem.TrySpawnVirtualItemInHand(ent.Owner, user, out var item, dropOthers: false, hand))
				{
					FreeHands(ent.Owner, user);
					return false;
				}
				((EntitySystem)this).EnsureComp<UnremoveableComponent>(item.Value);
				busy++;
				if (busy >= ent.Comp.BusyHands)
				{
					return true;
				}
			}
		}
		FreeHands(ent.Owner, user);
		return false;
	}

	private void FreeHands(EntityUid mount, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_virtualItem.DeleteInHandsMatching(user, mount);
	}

	private void OnExamine(Entity<WeaponMountComponent> ent, ref ExaminedEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsInDetailsRange || ((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		GunComponent gunComponent = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(ent.Comp.MountedEntity, ref gunComponent) && TryGetWeaponAmmo(Entity<WeaponMountComponent>.op_Implicit(ent), out var ammoCount, out var _))
		{
			args.PushMarkup(base.Loc.GetString("gun-magazine-examine", (ValueTuple<string, object>)("color", "yellow"), (ValueTuple<string, object>)("count", ammoCount)));
			args.PushMarkup(base.Loc.GetString("gun-selected-mode-examine", (ValueTuple<string, object>)("color", "cyan"), (ValueTuple<string, object>)("mode", base.Loc.GetString("gun-" + Enum.GetName(typeof(SelectiveFire), gunComponent.SelectedMode)))), 4);
			args.PushMarkup(base.Loc.GetString("gun-fire-rate-examine", (ValueTuple<string, object>)("color", "yellow"), (ValueTuple<string, object>)("fireRate", $"{gunComponent.FireRateModified:0.0}")), 3);
		}
		if (ent.Comp.Broken)
		{
			args.PushMarkup(base.Loc.GetString("emplacement-mount-broken-examine"));
		}
		if (ent.Comp.IsWeaponLocked)
		{
			return;
		}
		using (args.PushGroup("WeaponMountComponent"))
		{
			string message = null;
			if (!((EntitySystem)this).Transform(Entity<WeaponMountComponent>.op_Implicit(ent)).Anchored && !_foldable.IsFolded(Entity<WeaponMountComponent>.op_Implicit(ent)))
			{
				message = "emplacement-mount-unanchored-examine";
			}
			else if (!ent.Comp.MountedEntity.HasValue && ((EntitySystem)this).Transform(Entity<WeaponMountComponent>.op_Implicit(ent)).Anchored)
			{
				message = "emplacement-mount-anchored-examine";
			}
			else if (!ent.Comp.IsWeaponSecured && ent.Comp.MountedEntity.HasValue && !_foldable.IsFolded(Entity<WeaponMountComponent>.op_Implicit(ent)))
			{
				message = "emplacement-mount-weapon-unsecured-examine";
			}
			else if (ent.Comp.IsWeaponSecured && ((EntitySystem)this).Transform(Entity<WeaponMountComponent>.op_Implicit(ent)).Anchored)
			{
				message = "emplacement-mount-weapon-secured-examine";
			}
			if (message != null)
			{
				args.PushMarkup(base.Loc.GetString(message, (ValueTuple<string, object>)("color", "cyan")), 1);
			}
		}
	}

	private void OnAltVerb(EntityUid uid, WeaponMountComponent component, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (!((EntitySystem)this).TryComp<GunComponent>(component.MountedEntity, ref gun) || !args.CanAccess || !args.CanInteract || !args.CanComplexInteract || args.Hands == null || ((EntitySystem)this).HasComp<XenoComponent>(args.User))
		{
			return;
		}
		SelectiveFire nextMode = _gun.GetNextMode(gun);
		if (gun.SelectedMode != gun.AvailableModes)
		{
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_002d: Unknown result type (might be due to invalid IL or missing references)
					_gun.SelectFire(component.MountedEntity.Value, gun, nextMode, args.User);
				},
				Text = base.Loc.GetString("gun-selector-verb", (ValueTuple<string, object>)("mode", base.Loc.GetString("gun-" + Enum.GetName(typeof(SelectiveFire), nextMode)))),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png")),
				Priority = 3
			};
			args.Verbs.Add(verb);
		}
		FoldableComponent foldable = default(FoldableComponent);
		if (component.IsWeaponLocked && ((EntitySystem)this).TryComp<FoldableComponent>(uid, ref foldable) && !foldable.IsFolded)
		{
			AlternativeVerb dismantleVerb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryUndeployMount(Entity<WeaponMountComponent>.op_Implicit((uid, component)), args.User);
				},
				Text = base.Loc.GetString("emplacement-mount-undeploy"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png")),
				Priority = 3
			};
			args.Verbs.Add(dismantleVerb);
		}
		ItemSlotsComponent itemSlots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(component.MountedEntity, ref itemSlots) || !((EntitySystem)this).HasComp<FoldableComponent>(uid))
		{
			return;
		}
		foreach (ItemSlot slot in itemSlots.Slots.Values)
		{
			if (!slot.EjectOnInteract && !slot.DisableEject && _slots.CanEject(uid, args.User, slot) && _actionBlockerSystem.CanPickup(args.User, slot.Item.Value))
			{
				string verbSubject = ((slot.Name != string.Empty) ? base.Loc.GetString(slot.Name) : ((EntitySystem)this).Comp<MetaDataComponent>(slot.Item.Value).EntityName);
				AlternativeVerb verb2 = new AlternativeVerb
				{
					IconEntity = ((EntitySystem)this).GetNetEntity(slot.Item, (MetaDataComponent)null),
					Act = delegate
					{
						//IL_001b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0031: Unknown result type (might be due to invalid IL or missing references)
						//IL_003c: Unknown result type (might be due to invalid IL or missing references)
						EjectMagazine(component.MountedEntity.Value, slot, args.User, uid);
					}
				};
				if (slot.EjectVerbText == null)
				{
					verb2.Text = verbSubject;
					verb2.Category = VerbCategory.Eject;
				}
				else
				{
					verb2.Text = base.Loc.GetString(slot.EjectVerbText);
				}
				verb2.Priority = 3;
				args.Verbs.Add(verb2);
			}
		}
	}

	public bool HasWeaponMountsNearbyPopup(Entity<MapGridComponent> grid, EntityUid user, EntityCoordinates coordinates, int range)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		Vector2i position = _mapSystem.LocalToTile(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), coordinates);
		Box2 checkArea = default(Box2);
		((Box2)(ref checkArea))._002Ector((float)(position.X - range), (float)(position.Y - range), (float)(position.X + range), (float)(position.Y + range));
		WeaponMountComponent mount = default(WeaponMountComponent);
		foreach (EntityUid anchored in _mapSystem.GetLocalAnchoredEntities(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), checkArea))
		{
			if (((EntitySystem)this).TryComp<WeaponMountComponent>(anchored, ref mount) && mount.MountedEntity.HasValue)
			{
				string msg = base.Loc.GetString("emplacement-mount-too-close", (ValueTuple<string, object>)("mount", anchored));
				_popup.PopupClient(msg, user, user, PopupType.SmallCaution);
				return true;
			}
		}
		return false;
	}

	private bool CanAssembleMount(Entity<WeaponMountComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.MountExclusionAreaSize == 0)
		{
			return true;
		}
		EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(ent), ((EntitySystem)this).Transform(Entity<WeaponMountComponent>.op_Implicit(ent)))));
		MapGridComponent mapGrid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(grid, ref mapGrid))
		{
			return true;
		}
		if (HasWeaponMountsNearbyPopup(Entity<MapGridComponent>.op_Implicit((grid.Value, mapGrid)), user, _transform.GetMoverCoordinates(Entity<WeaponMountComponent>.op_Implicit(ent)), ent.Comp.MountExclusionAreaSize))
		{
			return false;
		}
		return true;
	}

	public void RotateMount(Entity<WeaponMountComponent> ent, EntityUid? user, int rotationDegrees = 90)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		_transform.SetLocalRotation(Entity<WeaponMountComponent>.op_Implicit(ent), _transform.GetWorldRotation(Entity<WeaponMountComponent>.op_Implicit(ent)) + Angle.FromDegrees((double)rotationDegrees), (TransformComponent)null);
		_audio.PlayPredicted(ent.Comp.RotateSound, Entity<WeaponMountComponent>.op_Implicit(ent), user, (AudioParams?)null);
		ScopeComponent scope = default(ScopeComponent);
		if (ent.Comp.User.HasValue && ((EntitySystem)this).TryComp<ScopeComponent>(ent.Comp.MountedEntity, ref scope))
		{
			_scope.StartScoping(Entity<ScopeComponent>.op_Implicit((ent.Comp.MountedEntity.Value, scope)), ent.Comp.User.Value);
		}
	}

	private bool TryAttachToMount(Entity<WeaponMountComponent> ent, EntityUid user, EntityUid used)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!CanAssembleMount(ent, user))
		{
			return false;
		}
		if (!IsViableWeapon(used, Entity<WeaponMountComponent>.op_Implicit(ent)))
		{
			return false;
		}
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.AssembleDelay, new AttachToMountDoAfterEvent(), Entity<WeaponMountComponent>.op_Implicit(ent), Entity<WeaponMountComponent>.op_Implicit(ent), used)
		{
			NeedHand = true,
			BreakOnMove = true,
			BreakOnHandChange = true
		};
		_doAfter.TryStartDoAfter(new DoAfterArgs(doAfterArgs));
		return true;
	}

	private bool TryDetachFromMount(Entity<WeaponMountComponent> ent, EntityUid user, EntityUid? used = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.DisassembleDelay, new DetachFromMountDoAfterEvent(), Entity<WeaponMountComponent>.op_Implicit(ent), Entity<WeaponMountComponent>.op_Implicit(ent), used)
		{
			NeedHand = true,
			BreakOnMove = true,
			BreakOnHandChange = true
		};
		_doAfter.TryStartDoAfter(new DoAfterArgs(doAfterArgs));
		return true;
	}

	private bool TryUndeployMount(Entity<WeaponMountComponent> ent, EntityUid user, EntityUid? used = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		DoAfterArgs undeployDoAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, ent.Comp.DisassembleDelay, new MountUnDeployDoAfterEvent(), Entity<WeaponMountComponent>.op_Implicit(ent), Entity<WeaponMountComponent>.op_Implicit(ent), used)
		{
			NeedHand = true,
			BreakOnMove = true,
			BreakOnHandChange = true
		};
		_doAfter.TryStartDoAfter(new DoAfterArgs(undeployDoAfterArgs));
		return true;
	}

	private void EjectMagazine(EntityUid uid, ItemSlot slot, EntityUid user, EntityUid mount)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (_slots.TryEjectToHands(uid, slot, user, excludeUserAudio: true))
		{
			WeaponMountComponentVisualLayers ammoSpriteKey = WeaponMountComponentVisualLayers.MountedAmmo;
			FoldableComponent foldableComp = default(FoldableComponent);
			if (((EntitySystem)this).TryComp<FoldableComponent>(mount, ref foldableComp) && foldableComp.IsFolded)
			{
				ammoSpriteKey = WeaponMountComponentVisualLayers.FoldedAmmo;
			}
			_appearance.SetData(mount, (Enum)ammoSpriteKey, (object)false, (AppearanceComponent)null);
		}
	}

	private void OnBreak(Entity<WeaponMountComponent> ent, ref BreakageEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldable = default(FoldableComponent);
		((EntitySystem)this).TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref foldable);
		ent.Comp.Broken = true;
		((EntitySystem)this).DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "Broken", (MetaDataComponent)null);
		CorrodibleComponent corrodible = default(CorrodibleComponent);
		if (((EntitySystem)this).TryComp<CorrodibleComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref corrodible))
		{
			XenoAcid.SetCorrodible(corrodible, isCorrodible: true);
		}
		UndeployMount(ent, null, foldable);
		UpdateAppearance(Entity<WeaponMountComponent>.op_Implicit(ent));
	}

	private void OnDamageModified(Entity<WeaponMountComponent> ent, ref DamageModifyEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		FoldableComponent foldable = default(FoldableComponent);
		if (((EntitySystem)this).TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(ent), ref foldable) && foldable.IsFolded)
		{
			args.Damage = new DamageSpecifier();
		}
	}

	private void OnCheckTileFree(Entity<WeaponMountComponent> ent, ref RMCCheckTileFreeEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<BarricadeComponent>(args.AnchoredEntity))
		{
			args.IsTileFree = true;
		}
	}

	public bool IsViableWeapon(EntityUid weapon, EntityUid mount, WeaponMountComponent? weaponMountComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<WeaponMountComponent>(mount, ref weaponMountComponent, false))
		{
			return false;
		}
		return _whitelist.IsWhitelistPassOrNull(weaponMountComponent.MountableWhitelist, weapon);
	}

	private void OnWeaponOverheated(Entity<WeaponMountComponent> ent, ref MountableWeaponRelayedEvent<OverheatedEvent> args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (args.Args.Damage != null)
		{
			_damage.TryChangeDamage(Entity<WeaponMountComponent>.op_Implicit(ent), args.Args.Damage);
			if (ent.Comp.MountedEntity.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("emplacement-mounted-weapon-overheated", (ValueTuple<string, object>)("weapon", ent.Comp.MountedEntity.Value)), Entity<WeaponMountComponent>.op_Implicit(ent), ent.Comp.User, PopupType.SmallCaution);
			}
		}
	}

	private void OnWeaponHeatGained(Entity<WeaponMountComponent> ent, ref MountableWeaponRelayedEvent<HeatGainedEvent> args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(Entity<WeaponMountComponent>.op_Implicit(ent));
	}

	private void OnGetGunUser(Entity<WeaponMountComponent> ent, ref GetIFFGunUserEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		args.GunUser = ent.Comp.User;
	}

	public void UpdateAppearance(EntityUid mount, WeaponMountComponent? mountComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<WeaponMountComponent>(mount, ref mountComponent, false))
		{
			FoldableComponent foldable = default(FoldableComponent);
			if (((EntitySystem)this).TryComp<FoldableComponent>(mount, ref foldable))
			{
				_appearance.SetData(mount, (Enum)WeaponMountComponentVisualLayers.Mounted, (object)(!foldable.IsFolded && mountComponent.MountedEntity.HasValue), (AppearanceComponent)null);
				_appearance.SetData(mount, (Enum)WeaponMountComponentVisualLayers.Folded, (object)(foldable.IsFolded && mountComponent.MountedEntity.HasValue), (AppearanceComponent)null);
				_appearance.SetData(mount, (Enum)WeaponMountComponentVisualLayers.Broken, (object)mountComponent.Broken, (AppearanceComponent)null);
			}
			OverheatComponent overheat = default(OverheatComponent);
			if (mountComponent.MountedEntity.HasValue && ((EntitySystem)this).TryComp<OverheatComponent>(mountComponent.MountedEntity.Value, ref overheat))
			{
				Color color = default(Color);
				_appearance.TryGetData<Color>(mount, (Enum)WeaponMountComponentVisualLayers.Overheated, ref color, (AppearanceComponent)null);
				bool showHeated = foldable == null || !foldable.IsFolded;
				_appearance.SetData(mount, (Enum)WeaponMountComponentVisualLayers.Overheated, (object)showHeated, (AppearanceComponent)null);
				float alpha = Math.Clamp(overheat.Heat / (float)overheat.MaxHeat, 0f, 1f);
				_appearance.SetData(mount, (Enum)WeaponMountComponentVisualLayers.Overheated, (object)((Color)(ref color)).WithAlpha(alpha), (AppearanceComponent)null);
			}
		}
	}

	public bool TryGetWeaponAmmo(EntityUid mount, [NotNullWhen(true)] out int? ammoCount, [NotNullWhen(true)] out int? ammoCapacity, WeaponMountComponent? mountComponent = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		ammoCount = null;
		ammoCapacity = null;
		if (!((EntitySystem)this).Resolve<WeaponMountComponent>(mount, ref mountComponent, false) || !mountComponent.MountedEntity.HasValue)
		{
			return false;
		}
		if (!_slots.TryGetSlot(mountComponent.MountedEntity.Value, "gun_magazine", out ItemSlot itemSlot) || !itemSlot.Item.HasValue)
		{
			return false;
		}
		GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
		((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(itemSlot.Item.Value, ref ammoEv, false);
		ammoCount = ammoEv.Count;
		ammoCapacity = ammoEv.Capacity;
		return true;
	}

	public bool TryGetMountCone(Entity<WeaponMountComponent?> mount, out int shootArc)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		shootArc = 0;
		if (!((EntitySystem)this).Resolve<WeaponMountComponent>(Entity<WeaponMountComponent>.op_Implicit(mount), ref mount.Comp, false) || !mount.Comp.MountedEntity.HasValue)
		{
			return false;
		}
		MountableWeaponComponent weapon = default(MountableWeaponComponent);
		shootArc = (((EntitySystem)this).TryComp<MountableWeaponComponent>(mount.Comp.MountedEntity.Value, ref weapon) ? weapon.ShootArc : 100);
		return true;
	}

	public bool TryGetMountSeatingRotation(EntityUid mountedWeapon, out Angle rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		rotation = default(Angle);
		MountableWeaponComponent weapon = default(MountableWeaponComponent);
		if (!((EntitySystem)this).TryComp<MountableWeaponComponent>(mountedWeapon, ref weapon) || !weapon.MountedTo.HasValue)
		{
			return false;
		}
		rotation = _transform.GetWorldRotation(((EntitySystem)this).GetEntity(weapon.MountedTo.Value));
		return true;
	}
}
