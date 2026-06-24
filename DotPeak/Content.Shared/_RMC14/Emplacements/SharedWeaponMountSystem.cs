// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Emplacements.SharedWeaponMountSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Content.Shared.Tools.Systems;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
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
    this.SubscribeLocalEvent<WeaponMountComponent, InteractUsingEvent>(new EntityEventRefHandler<WeaponMountComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<WeaponMountComponent, FoldAttemptEvent>(new EntityEventRefHandler<WeaponMountComponent, FoldAttemptEvent>(this.OnFoldAttempt));
    this.SubscribeLocalEvent<WeaponMountComponent, AnchorAttemptEvent>(new EntityEventRefHandler<WeaponMountComponent, AnchorAttemptEvent>(this.OnAnchorAttempt));
    this.SubscribeLocalEvent<WeaponMountComponent, UnanchorAttemptEvent>(new EntityEventRefHandler<WeaponMountComponent, UnanchorAttemptEvent>(this.OnUnanchorAttempt));
    this.SubscribeLocalEvent<WeaponMountComponent, UseInHandEvent>(new EntityEventRefHandler<WeaponMountComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<WeaponMountComponent, StrapAttemptEvent>(new EntityEventRefHandler<WeaponMountComponent, StrapAttemptEvent>(this.OnStrapAttempt));
    this.SubscribeLocalEvent<WeaponMountComponent, StrappedEvent>(new EntityEventRefHandler<WeaponMountComponent, StrappedEvent>(this.OnStrapped));
    this.SubscribeLocalEvent<WeaponMountComponent, UnstrappedEvent>(new EntityEventRefHandler<WeaponMountComponent, UnstrappedEvent>(this.OnUnStrapped));
    this.SubscribeLocalEvent<WeaponMountComponent, ExaminedEvent>(new EntityEventRefHandler<WeaponMountComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<WeaponMountComponent, MapInitEvent>(new EntityEventRefHandler<WeaponMountComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<WeaponMountComponent, DismountActionEvent>(new EntityEventRefHandler<WeaponMountComponent, DismountActionEvent>(this.OnDismountAction));
    this.SubscribeLocalEvent<WeaponControllerComponent, MoveInputEvent>(new EntityEventRefHandler<WeaponControllerComponent, MoveInputEvent>(this.OnMountedMoveInput));
    this.SubscribeLocalEvent<WeaponMountComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<WeaponMountComponent, GetVerbsEvent<AlternativeVerb>>(this.OnAltVerb));
    this.SubscribeLocalEvent<WeaponMountComponent, BreakageEventArgs>(new EntityEventRefHandler<WeaponMountComponent, BreakageEventArgs>(this.OnBreak));
    this.SubscribeLocalEvent<WeaponMountComponent, DamageModifyEvent>(new EntityEventRefHandler<WeaponMountComponent, DamageModifyEvent>(this.OnDamageModified));
    this.SubscribeLocalEvent<WeaponMountComponent, RMCCheckTileFreeEvent>(new EntityEventRefHandler<WeaponMountComponent, RMCCheckTileFreeEvent>(this.OnCheckTileFree));
    this.SubscribeLocalEvent<WeaponMountComponent, GetIFFGunUserEvent>(new EntityEventRefHandler<WeaponMountComponent, GetIFFGunUserEvent>(this.OnGetGunUser));
    this.SubscribeLocalEvent<WeaponMountComponent, InteractHandEvent>(new EntityEventRefHandler<WeaponMountComponent, InteractHandEvent>(this.OnInteractHand), new Type[1]
    {
      typeof (SharedBuckleSystem)
    });
    this.SubscribeLocalEvent<WeaponMountComponent, CanDropTargetEvent>(new EntityEventRefHandler<WeaponMountComponent, CanDropTargetEvent>(this.OnMountCanDropTarget), after: new Type[1]
    {
      typeof (SharedBuckleSystem)
    });
    this.SubscribeLocalEvent<WeaponMountComponent, MountableWeaponRelayedEvent<OverheatedEvent>>(new EntityEventRefHandler<WeaponMountComponent, MountableWeaponRelayedEvent<OverheatedEvent>>(this.OnWeaponOverheated));
    this.SubscribeLocalEvent<WeaponMountComponent, MountableWeaponRelayedEvent<HeatGainedEvent>>(new EntityEventRefHandler<WeaponMountComponent, MountableWeaponRelayedEvent<HeatGainedEvent>>(this.OnWeaponHeatGained));
    this.SubscribeLocalEvent<WeaponMountComponent, AttachToMountDoAfterEvent>(new EntityEventRefHandler<WeaponMountComponent, AttachToMountDoAfterEvent>(this.OnAttachToMount));
    this.SubscribeLocalEvent<WeaponMountComponent, SecureToMountDoAfterEvent>(new EntityEventRefHandler<WeaponMountComponent, SecureToMountDoAfterEvent>(this.OnSecureToMount));
    this.SubscribeLocalEvent<WeaponMountComponent, DetachFromMountDoAfterEvent>(new EntityEventRefHandler<WeaponMountComponent, DetachFromMountDoAfterEvent>(this.OnDetachFromMount));
    this.SubscribeLocalEvent<WeaponMountComponent, MountDeployDoafterEvent>(new EntityEventRefHandler<WeaponMountComponent, MountDeployDoafterEvent>(this.OnMountDeploy));
    this.SubscribeLocalEvent<WeaponMountComponent, MountUnDeployDoAfterEvent>(new EntityEventRefHandler<WeaponMountComponent, MountUnDeployDoAfterEvent>(this.OnMountUndeploy));
  }

  private void OnMapInit(Entity<WeaponMountComponent> ent, ref MapInitEvent args)
  {
    this.EnsureDismountAction(ent);
    if (!ent.Comp.FixedWeaponPrototype.HasValue)
      return;
    ContainerSlot containerSlot = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.WeaponSlotId);
    containerSlot.OccludesLight = false;
    if (containerSlot.ContainedEntities.Count > 0)
      return;
    EntProtoId? fixedWeaponPrototype = ent.Comp.FixedWeaponPrototype;
    EntityUid uid = this.SpawnInContainerOrDrop(fixedWeaponPrototype.HasValue ? (string) fixedWeaponPrototype.GetValueOrDefault() : (string) null, (EntityUid) ent, ent.Comp.WeaponSlotId);
    ent.Comp.MountedEntity = new EntityUid?(uid);
    this.DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "MountedEntity");
    MountableWeaponComponent comp;
    if (!this.TryComp<MountableWeaponComponent>(uid, out comp))
      return;
    comp.MountedTo = new NetEntity?(this.GetNetEntity(ent.Owner));
    this.Dirty(uid, (IComponent) comp);
  }

  private void OnInteractUsing(Entity<WeaponMountComponent> ent, ref InteractUsingEvent args)
  {
    ItemSlot itemSlot;
    BallisticAmmoProviderComponent comp1;
    if (ent.Comp.MountedEntity.HasValue && this._slots.TryGetSlot(ent.Comp.MountedEntity.Value, "gun_magazine", out itemSlot) && this.TryComp<BallisticAmmoProviderComponent>(args.Used, out comp1))
    {
      HandsComponent comp2;
      if (!this.TryComp<HandsComponent>(args.User, out comp2) || !this._slots.CanInsert(ent.Comp.MountedEntity.Value, args.Used, new EntityUid?(args.User), itemSlot, true) || !this._hands.TryDrop((Entity<HandsComponent>) args.User, args.Used))
        return;
      if (itemSlot.Item.HasValue)
        this._hands.TryPickupAnyHand(args.User, itemSlot.Item.Value, handsComp: comp2);
      this._slots.TryInsert(ent.Comp.MountedEntity.Value, "gun_magazine", args.Used, new EntityUid?(args.User), excludeUserAudio: true);
      WeaponMountComponentVisualLayers key = WeaponMountComponentVisualLayers.MountedAmmo;
      FoldableComponent comp3;
      if (this.TryComp<FoldableComponent>((EntityUid) ent, out comp3) && comp3.IsFolded)
        key = WeaponMountComponentVisualLayers.FoldedAmmo;
      this._appearance.SetData((EntityUid) ent, (Enum) key, (object) (comp1.Count > 0));
    }
    else
    {
      FoldableComponent comp4;
      if (ent.Comp.IsWeaponLocked || this.TryComp<FoldableComponent>((EntityUid) ent, out comp4) && comp4.IsFolded)
        return;
      if (this.HasComp<MountableWeaponComponent>(args.Used) && this.Transform((EntityUid) ent).Anchored && !ent.Comp.MountedEntity.HasValue)
        this.TryAttachToMount(ent, args.User, args.Used);
      else if (this._tool.HasQuality(args.Used, (string) ent.Comp.RotationTool))
      {
        this.RotateMount(ent, new EntityUid?(args.User));
      }
      else
      {
        BaseContainer container;
        if (!this._tool.HasQuality(args.Used, (string) ent.Comp.DismantlingTool) || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.WeaponSlotId, out container) || container.ContainedEntities.Count <= 0 || ent.Comp.IsWeaponSecured)
          return;
        this.TryDetachFromMount(ent, args.User, new EntityUid?(args.Used));
      }
    }
  }

  private void OnUseInHand(Entity<WeaponMountComponent> ent, ref UseInHandEvent args)
  {
    if (!ent.Comp.MountedEntity.HasValue)
      return;
    args.Handled = true;
    if (!this.CanDeployPopup(ent, args.User, out EntityCoordinates _, out Angle _))
      return;
    this._doAfter.TryStartDoAfter(new DoAfterArgs(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, ent.Comp.AssembleDelay, (DoAfterEvent) new MountDeployDoafterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(args.User))
    {
      NeedHand = true,
      BreakOnMove = true,
      BreakOnHandChange = true
    }));
  }

  private void OnFoldAttempt(Entity<WeaponMountComponent> ent, ref FoldAttemptEvent args)
  {
    if (!this.Transform((EntityUid) ent).Anchored && !ent.Comp.MountedEntity.HasValue)
      return;
    args.Cancelled = true;
  }

  private void OnAnchorAttempt(Entity<WeaponMountComponent> ent, ref AnchorAttemptEvent args)
  {
    FoldableComponent comp;
    if (!this.TryComp<FoldableComponent>((EntityUid) ent, out comp) || !comp.IsFolded)
      return;
    args.Cancel();
  }

  private void OnUnanchorAttempt(Entity<WeaponMountComponent> ent, ref UnanchorAttemptEvent args)
  {
    FoldableComponent comp;
    if (this.TryComp<FoldableComponent>((EntityUid) ent, out comp) && comp.IsFolded)
    {
      args.Cancel();
    }
    else
    {
      if (!ent.Comp.MountedEntity.HasValue)
        return;
      args.Cancel();
      if (!ent.Comp.IsWeaponSecured)
      {
        this._doAfter.TryStartDoAfter(new DoAfterArgs(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, ent.Comp.AssembleDelay, (DoAfterEvent) new SecureToMountDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(args.Tool))
        {
          NeedHand = true,
          BreakOnMove = true,
          BreakOnHandChange = true
        }));
      }
      else
      {
        if (comp == null)
          return;
        this.TryUndeployMount(ent, args.User, new EntityUid?(args.Tool));
      }
    }
  }

  private void OnAttachToMount(Entity<WeaponMountComponent> ent, ref AttachToMountDoAfterEvent args)
  {
    MountableWeaponComponent comp;
    if (args.Cancelled || !this.TryComp<MountableWeaponComponent>(args.Used, out comp))
      return;
    EntityUid? used = args.Used;
    if (!used.HasValue || !this.CanAssembleMount(ent, args.User))
      return;
    ContainerSlot containerSlot = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.WeaponSlotId);
    containerSlot.OccludesLight = false;
    if (containerSlot.ContainedEntities.Count > 0)
      return;
    SharedContainerSystem container1 = this._container;
    used = args.Used;
    Entity<TransformComponent, MetaDataComponent, PhysicsComponent> toInsert = (Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) used.Value;
    ContainerSlot container2 = containerSlot;
    if (!container1.Insert(toInsert, (BaseContainer) container2))
      return;
    comp.MountedTo = new NetEntity?(this.GetNetEntity((EntityUid) ent));
    ent.Comp.MountedEntity = args.Used;
    this._collisionWake.SetEnabled((EntityUid) ent, false);
    this._item.SetSize((EntityUid) ent, ent.Comp.MountedWeaponSize);
    this._rmcFoldable.TryLockFold((EntityUid) ent, true);
    this.DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "MountedEntity");
    this.UpdateAppearance((EntityUid) ent);
    this._audio.PlayPredicted(ent.Comp.RotateSound, (EntityUid) ent, new EntityUid?(args.User));
  }

  private void OnSecureToMount(Entity<WeaponMountComponent> ent, ref SecureToMountDoAfterEvent args)
  {
    if (args.Cancelled)
      return;
    ent.Comp.IsWeaponSecured = true;
    this._buckle.StrapSetEnabled((EntityUid) ent, true);
    MetaDataComponent comp;
    if (this.TryComp(ent.Comp.MountedEntity, out comp) && comp.EntityPrototype != null)
    {
      this._metaData.SetEntityName((EntityUid) ent, comp.EntityName);
      this._metaData.SetEntityDescription((EntityUid) ent, this.Loc.GetString($"emplacement-mount-{comp.EntityPrototype.ID}-description-mounted"));
    }
    this._audio.PlayPredicted(ent.Comp.SecureSound, (EntityUid) ent, new EntityUid?(args.User));
  }

  private void OnDetachFromMount(
    Entity<WeaponMountComponent> ent,
    ref DetachFromMountDoAfterEvent args)
  {
    BaseContainer container;
    if (args.Cancelled || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.WeaponSlotId, out container))
      return;
    this._container.EmptyContainer(container);
    MountableWeaponComponent comp1;
    if (this.TryComp<MountableWeaponComponent>(ent.Comp.MountedEntity, out comp1))
      comp1.MountedTo = new NetEntity?(this.GetNetEntity((EntityUid) ent));
    MetaDataComponent comp2;
    if (this.TryComp(ent.Comp.MountedEntity, out comp2) && comp2.EntityPrototype != null)
    {
      this._metaData.SetEntityName((EntityUid) ent, this.Loc.GetString($"emplacement-mount-{comp2.EntityPrototype.ID}-name"));
      this._metaData.SetEntityDescription((EntityUid) ent, this.Loc.GetString($"emplacement-mount-{comp2.EntityPrototype.ID}-description"));
    }
    ent.Comp.MountedEntity = new EntityUid?();
    this._buckle.StrapSetEnabled((EntityUid) ent, false);
    this._collisionWake.SetEnabled((EntityUid) ent, true);
    this._item.SetSize((EntityUid) ent, ent.Comp.MountSize);
    this._rmcFoldable.TryLockFold((EntityUid) ent, false);
    this.DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "MountedEntity");
    this.UpdateAppearance((EntityUid) ent);
    this._audio.PlayPredicted(ent.Comp.DetachSound, (EntityUid) ent, new EntityUid?(args.User));
  }

  private void OnMountDeploy(Entity<WeaponMountComponent> ent, ref MountDeployDoafterEvent args)
  {
    EntityCoordinates coordinates;
    Angle rotation;
    if (args.Cancelled || !this.CanDeployPopup(ent, args.User, out coordinates, out rotation))
      return;
    FoldableComponent comp1;
    if (this.TryComp<FoldableComponent>((EntityUid) ent, out comp1))
      this._foldable.SetFolded((EntityUid) ent, comp1, false);
    MetaDataComponent comp2;
    if (this.TryComp(ent.Comp.MountedEntity, out comp2) && ent.Comp.IsWeaponLocked && comp2.EntityPrototype != null)
      this._metaData.SetEntityDescription((EntityUid) ent, this.Loc.GetString($"emplacement-mount-{comp2.EntityPrototype.ID}-description-mounted"));
    TransformComponent xform = this.Transform((EntityUid) ent);
    this._transform.SetCoordinates((EntityUid) ent, xform, coordinates, new Angle?(rotation));
    this._transform.AnchorEntity((EntityUid) ent, xform);
    this._collisionWake.SetEnabled((EntityUid) ent, false);
    if (ent.Comp.MountOnDeploy && ent.Comp.MountedEntity.HasValue)
    {
      GetAmmoCountEvent args1 = new GetAmmoCountEvent();
      this.RaiseLocalEvent<GetAmmoCountEvent>(ent.Comp.MountedEntity.Value, ref args1);
      if (args1.Count > 0)
        this._buckle.TryBuckle(args.User, new EntityUid?(args.User), (EntityUid) ent, popup: false);
    }
    this.UpdateAppearance((EntityUid) ent);
    this._audio.PlayPredicted(ent.Comp.DeploySound, (EntityUid) ent, new EntityUid?(args.User));
  }

  private void OnMountUndeploy(Entity<WeaponMountComponent> ent, ref MountUnDeployDoAfterEvent args)
  {
    FoldableComponent comp;
    if (args.Cancelled || !this.TryComp<FoldableComponent>((EntityUid) ent, out comp))
      return;
    this.UndeployMount(ent, new EntityUid?(args.User), comp);
  }

  public void UndeployMount(
    Entity<WeaponMountComponent> ent,
    EntityUid? user = null,
    FoldableComponent? foldable = null)
  {
    MetaDataComponent comp1;
    if (this.TryComp(ent.Comp.MountedEntity, out comp1) && ent.Comp.IsWeaponLocked && comp1.EntityPrototype != null)
      this._metaData.SetEntityDescription((EntityUid) ent, this.Loc.GetString($"emplacement-mount-{comp1.EntityPrototype.ID}-description"));
    ent.Comp.IsWeaponSecured = false;
    this._transform.Unanchor((EntityUid) ent);
    if (foldable != null)
      this._foldable.SetFolded((EntityUid) ent, foldable, true);
    this._buckle.StrapSetEnabled((EntityUid) ent, false);
    this._collisionWake.SetEnabled((EntityUid) ent, true);
    HandsComponent comp2;
    if (this.TryComp<HandsComponent>(user, out comp2))
      this._hands.TryPickupAnyHand(user.Value, (EntityUid) ent, handsComp: comp2);
    this.UpdateAppearance((EntityUid) ent);
    this._audio.PlayPredicted(ent.Comp.UndeploySound, (EntityUid) ent, user);
  }

  private bool CanDeployPopup(
    Entity<WeaponMountComponent> ent,
    EntityUid user,
    out EntityCoordinates coordinates,
    out Angle rotation)
  {
    (coordinates, rotation) = this._transform.GetMoverCoordinateRotation(user, this.Transform(user));
    if (ent.Comp.Broken)
    {
      this._popup.PopupClient(this.Loc.GetString("emplacement-mount-deploy-broken", ("mount", (object) ent)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    Direction cardinalDir = ((Angle) ref rotation).GetCardinalDir();
    coordinates = coordinates.Offset(DirectionExtensions.ToVec(cardinalDir));
    if (this._rmcMap.IsTileBlocked(coordinates, CollisionGroup.MidImpassable))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-sentry-need-open-area", ("sentry", (object) ent)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    EntityUid? grid = this._transform.GetGrid((Entity<TransformComponent>) (user, this.Transform(user)));
    MapGridComponent comp;
    return !this.TryComp<MapGridComponent>(grid, out comp) || !this.HasWeaponMountsNearbyPopup((Entity<MapGridComponent>) (grid.Value, comp), user, coordinates, ent.Comp.MountExclusionAreaSize) && (ent.Comp.BarricadeExclusionAreaSize == 0 || !this._barricade.HasBarricadeNearbyPopup((Entity<MapGridComponent>) (grid.Value, comp), user, coordinates, ent.Comp.BarricadeExclusionAreaSize));
  }

  private void OnInteractHand(Entity<WeaponMountComponent> ent, ref InteractHandEvent args)
  {
    if (!this._combatMode.IsInCombatMode(new EntityUid?(args.User)))
    {
      if (!ent.Comp.CanRotateWithoutTool || !ent.Comp.MountedEntity.HasValue || ent.Comp.User.HasValue || !this.Transform((EntityUid) ent).Anchored)
        return;
      Vector2 worldPosition1 = this._transform.GetWorldPosition((EntityUid) ent);
      Vector2 worldPosition2 = this._transform.GetWorldPosition(args.User);
      Vector2 vector2_1 = worldPosition1 - worldPosition2;
      if ((double) vector2_1.LengthSquared() <= 9.9999997473787516E-05)
        return;
      Angle angle1 = Angle.FromWorldVec(vector2_1);
      this._transform.SetWorldRotation((EntityUid) ent, angle1);
      StrapComponent comp;
      if (!this.TryComp<StrapComponent>((EntityUid) ent, out comp))
        return;
      Angle angle2 = Angle.op_UnaryNegation(angle1);
      ref Angle local1 = ref angle2;
      Vector2 vector2_2 = worldPosition2 - worldPosition1;
      ref Vector2 local2 = ref vector2_2;
      Vector2 offset = ((Angle) ref local1).RotateVec(ref local2) - this._rmcBuckle.GetOffset((Entity<RMCBuckleOffsetComponent>) args.User);
      this._buckle.SetBuckleOffset((Entity<StrapComponent>) ((EntityUid) ent, comp), offset);
    }
    else
      args.Handled = true;
  }

  private void OnMountCanDropTarget(Entity<WeaponMountComponent> ent, ref CanDropTargetEvent args)
  {
    args.CanDrop = false;
    args.Handled = true;
  }

  private void OnStrapAttempt(Entity<WeaponMountComponent> ent, ref StrapAttemptEvent args)
  {
    if (ent.Comp.BusyHands > 0 && this._hands.CountFreeHands((Entity<HandsComponent>) args.Buckle.Owner) < ent.Comp.BusyHands)
    {
      if (args.Popup)
        this._popup.PopupClient(ent.Comp.BusyPopup != null ? this.Loc.GetString(ent.Comp.BusyPopup) : this.Loc.GetString("mountable-weapon-no-free-hands"), (EntityUid) args.Buckle, args.User, PopupType.SmallCaution);
      args.Cancelled = true;
    }
    else
    {
      EntityUid? user = args.User;
      EntityUid buckle = (EntityUid) args.Buckle;
      if ((user.HasValue ? (user.GetValueOrDefault() != buckle ? 1 : 0) : 1) == 0)
        return;
      args.Cancelled = true;
    }
  }

  private void OnStrapped(Entity<WeaponMountComponent> ent, ref StrappedEvent args)
  {
    if (!this.TakeHands(ent, args.Buckle.Owner))
    {
      this._buckle.Unbuckle((Entity<BuckleComponent>) (args.Buckle.Owner, args.Buckle.Comp), new EntityUid?(args.Buckle.Owner));
    }
    else
    {
      ent.Comp.User = new EntityUid?((EntityUid) args.Buckle);
      this.EnsureDismountAction(ent);
      this._actions.AddAction((EntityUid) args.Buckle, ref ent.Comp.DismountActionEntity, ent.Comp.DismountAction, (EntityUid) ent);
      if (!ent.Comp.MountedEntity.HasValue)
        return;
      WeaponControllerComponent controllerComponent = this.EnsureComp<WeaponControllerComponent>((EntityUid) args.Buckle);
      controllerComponent.ControlledWeapon = new NetEntity?(this.GetNetEntity(ent.Comp.MountedEntity.Value));
      this.Dirty((EntityUid) args.Buckle, (IComponent) controllerComponent);
      ScopeComponent comp;
      if (!this.TryComp<ScopeComponent>(ent.Comp.MountedEntity.Value, out comp))
        return;
      this._scope.StartScoping((Entity<ScopeComponent>) (ent.Comp.MountedEntity.Value, comp), (EntityUid) args.Buckle);
    }
  }

  private void OnUnStrapped(Entity<WeaponMountComponent> ent, ref UnstrappedEvent args)
  {
    this.FreeHands(ent.Owner, args.Buckle.Owner);
    ent.Comp.User = new EntityUid?();
    this.RemComp<WeaponControllerComponent>((EntityUid) args.Buckle);
    ScopeComponent comp;
    if (this.TryComp<ScopeComponent>(ent.Comp.MountedEntity, out comp))
      this._scope.Unscope((Entity<ScopeComponent>) (ent.Comp.MountedEntity.Value, comp));
    SharedActionsSystem actions = this._actions;
    Entity<ActionsComponent> owner = (Entity<ActionsComponent>) args.Buckle.Owner;
    EntityUid? dismountActionEntity = ent.Comp.DismountActionEntity;
    Entity<ActionComponent>? action = dismountActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) dismountActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actions.RemoveAction(owner, action);
  }

  private void OnDismountAction(Entity<WeaponMountComponent> ent, ref DismountActionEvent args)
  {
    args.Handled = true;
    if (this._net.IsClient)
      return;
    this._buckle.TryUnbuckle(args.Performer, new EntityUid?(args.Performer));
  }

  private void OnMountedMoveInput(Entity<WeaponControllerComponent> ent, ref MoveInputEvent args)
  {
    if (!args.HasDirectionalMovement || this._net.IsClient)
      return;
    this._buckle.TryUnbuckle(ent.Owner, new EntityUid?(ent.Owner), popup: false);
  }

  private void EnsureDismountAction(Entity<WeaponMountComponent> ent)
  {
    if (ent.Comp.DismountAction == null)
      return;
    EntityUid? nullable = ent.Comp.DismountActionEntity;
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      ActionComponent comp;
      if (this.TryComp<ActionComponent>(valueOrDefault, out comp))
      {
        nullable = comp.Container;
        EntityUid owner = ent.Owner;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() != owner ? 1 : 0) : 1) != 0)
        {
          this._actConts.RemoveAction(new Entity<ActionComponent>?((Entity<ActionComponent>) (valueOrDefault, comp)));
          ent.Comp.DismountActionEntity = new EntityUid?();
          this.DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "DismountActionEntity");
        }
      }
    }
    if (!this._actConts.EnsureAction(ent.Owner, ref ent.Comp.DismountActionEntity, ent.Comp.DismountAction))
      return;
    this.DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "DismountActionEntity");
  }

  private bool TakeHands(Entity<WeaponMountComponent> ent, EntityUid user)
  {
    if (ent.Comp.BusyHands <= 0)
      return true;
    if (this._hands.CountFreeHands((Entity<HandsComponent>) user) < ent.Comp.BusyHands)
      return false;
    int num = 0;
    foreach (string enumerateHand in this._hands.EnumerateHands((Entity<HandsComponent>) user))
    {
      if (this._hands.HandIsEmpty((Entity<HandsComponent>) user, enumerateHand))
      {
        EntityUid? virtualItem;
        if (!this._virtualItem.TrySpawnVirtualItemInHand(ent.Owner, user, out virtualItem, empty: enumerateHand))
        {
          this.FreeHands(ent.Owner, user);
          return false;
        }
        this.EnsureComp<UnremoveableComponent>(virtualItem.Value);
        ++num;
        if (num >= ent.Comp.BusyHands)
          return true;
      }
    }
    this.FreeHands(ent.Owner, user);
    return false;
  }

  private void FreeHands(EntityUid mount, EntityUid user)
  {
    this._virtualItem.DeleteInHandsMatching(user, mount);
  }

  private void OnExamine(Entity<WeaponMountComponent> ent, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange || this.HasComp<XenoComponent>(args.Examiner))
      return;
    GunComponent comp;
    int? ammoCount;
    if (this.TryComp<GunComponent>(ent.Comp.MountedEntity, out comp) && this.TryGetWeaponAmmo((EntityUid) ent, out ammoCount, out int? _))
    {
      args.PushMarkup(this.Loc.GetString("gun-magazine-examine", ("color", (object) "yellow"), ("count", (object) ammoCount)));
      args.PushMarkup(this.Loc.GetString("gun-selected-mode-examine", ("color", (object) "cyan"), ("mode", (object) this.Loc.GetString("gun-" + Enum.GetName(typeof (SelectiveFire), (object) comp.SelectedMode)))), 4);
      args.PushMarkup(this.Loc.GetString("gun-fire-rate-examine", ("color", (object) "yellow"), ("fireRate", (object) $"{comp.FireRateModified:0.0}")), 3);
    }
    if (ent.Comp.Broken)
      args.PushMarkup(this.Loc.GetString("emplacement-mount-broken-examine"));
    if (ent.Comp.IsWeaponLocked)
      return;
    using (args.PushGroup("WeaponMountComponent"))
    {
      string messageId = (string) null;
      if (!this.Transform((EntityUid) ent).Anchored && !this._foldable.IsFolded((EntityUid) ent))
        messageId = "emplacement-mount-unanchored-examine";
      else if (!ent.Comp.MountedEntity.HasValue && this.Transform((EntityUid) ent).Anchored)
        messageId = "emplacement-mount-anchored-examine";
      else if (!ent.Comp.IsWeaponSecured && ent.Comp.MountedEntity.HasValue && !this._foldable.IsFolded((EntityUid) ent))
        messageId = "emplacement-mount-weapon-unsecured-examine";
      else if (ent.Comp.IsWeaponSecured && this.Transform((EntityUid) ent).Anchored)
        messageId = "emplacement-mount-weapon-secured-examine";
      if (messageId == null)
        return;
      args.PushMarkup(this.Loc.GetString(messageId, ("color", (object) "cyan")), 1);
    }
  }

  private void OnAltVerb(
    EntityUid uid,
    WeaponMountComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    GunComponent gun;
    if (!this.TryComp<GunComponent>(component.MountedEntity, out gun) || !args.CanAccess || !args.CanInteract || !args.CanComplexInteract || args.Hands == null || this.HasComp<XenoComponent>(args.User))
      return;
    SelectiveFire nextMode = this._gun.GetNextMode(gun);
    if (gun.SelectedMode != gun.AvailableModes)
    {
      AlternativeVerb alternativeVerb1 = new AlternativeVerb();
      alternativeVerb1.Act = (Action) (() => this._gun.SelectFire(component.MountedEntity.Value, gun, nextMode, new EntityUid?(args.User)));
      alternativeVerb1.Text = this.Loc.GetString("gun-selector-verb", ("mode", (object) this.Loc.GetString("gun-" + Enum.GetName(typeof (SelectiveFire), (object) nextMode))));
      alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png"));
      alternativeVerb1.Priority = 3;
      AlternativeVerb alternativeVerb2 = alternativeVerb1;
      args.Verbs.Add(alternativeVerb2);
    }
    FoldableComponent comp1;
    if (component.IsWeaponLocked && this.TryComp<FoldableComponent>(uid, out comp1) && !comp1.IsFolded)
    {
      AlternativeVerb alternativeVerb3 = new AlternativeVerb();
      alternativeVerb3.Act = (Action) (() => this.TryUndeployMount((Entity<WeaponMountComponent>) (uid, component), args.User));
      alternativeVerb3.Text = this.Loc.GetString("emplacement-mount-undeploy");
      alternativeVerb3.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png"));
      alternativeVerb3.Priority = 3;
      AlternativeVerb alternativeVerb4 = alternativeVerb3;
      args.Verbs.Add(alternativeVerb4);
    }
    ItemSlotsComponent comp2;
    if (!this.TryComp<ItemSlotsComponent>(component.MountedEntity, out comp2) || !this.HasComp<FoldableComponent>(uid))
      return;
    foreach (ItemSlot itemSlot in comp2.Slots.Values)
    {
      ItemSlot slot = itemSlot;
      if (!slot.EjectOnInteract && !slot.DisableEject)
      {
        ItemSlotsSystem slots = this._slots;
        EntityUid uid1 = uid;
        EntityUid? user1 = new EntityUid?(args.User);
        ItemSlot slot1 = slot;
        EntityUid? nullable = new EntityUid?();
        EntityUid? popup = nullable;
        if (slots.CanEject(uid1, user1, slot1, popup))
        {
          ActionBlockerSystem actionBlockerSystem = this._actionBlockerSystem;
          EntityUid user2 = args.User;
          nullable = slot.Item;
          EntityUid entityUid = nullable.Value;
          if (actionBlockerSystem.CanPickup(user2, entityUid))
          {
            string entityName;
            if (!(slot.Name != string.Empty))
            {
              nullable = slot.Item;
              entityName = this.Comp<MetaDataComponent>(nullable.Value).EntityName;
            }
            else
              entityName = this.Loc.GetString(slot.Name);
            string str = entityName;
            AlternativeVerb alternativeVerb5 = new AlternativeVerb();
            alternativeVerb5.IconEntity = this.GetNetEntity(slot.Item);
            alternativeVerb5.Act = (Action) (() => this.EjectMagazine(component.MountedEntity.Value, slot, args.User, uid));
            AlternativeVerb alternativeVerb6 = alternativeVerb5;
            if (slot.EjectVerbText == null)
            {
              alternativeVerb6.Text = str;
              alternativeVerb6.Category = VerbCategory.Eject;
            }
            else
              alternativeVerb6.Text = this.Loc.GetString(slot.EjectVerbText);
            alternativeVerb6.Priority = 3;
            args.Verbs.Add(alternativeVerb6);
          }
        }
      }
    }
  }

  public bool HasWeaponMountsNearbyPopup(
    Entity<MapGridComponent> grid,
    EntityUid user,
    EntityCoordinates coordinates,
    int range)
  {
    Vector2i tile = this._mapSystem.LocalToTile((EntityUid) grid, (MapGridComponent) grid, coordinates);
    Box2 localAABB;
    // ISSUE: explicit constructor call
    ((Box2) ref localAABB).\u002Ector((float) (tile.X - range), (float) (tile.Y - range), (float) (tile.X + range), (float) (tile.Y + range));
    foreach (EntityUid localAnchoredEntity in this._mapSystem.GetLocalAnchoredEntities((EntityUid) grid, (MapGridComponent) grid, localAABB))
    {
      WeaponMountComponent comp;
      if (this.TryComp<WeaponMountComponent>(localAnchoredEntity, out comp) && comp.MountedEntity.HasValue)
      {
        this._popup.PopupClient(this.Loc.GetString("emplacement-mount-too-close", ("mount", (object) localAnchoredEntity)), user, new EntityUid?(user), PopupType.SmallCaution);
        return true;
      }
    }
    return false;
  }

  private bool CanAssembleMount(Entity<WeaponMountComponent> ent, EntityUid user)
  {
    if (ent.Comp.MountExclusionAreaSize == 0)
      return true;
    EntityUid? grid = this._transform.GetGrid((Entity<TransformComponent>) ((EntityUid) ent, this.Transform((EntityUid) ent)));
    MapGridComponent comp;
    return !this.TryComp<MapGridComponent>(grid, out comp) || !this.HasWeaponMountsNearbyPopup((Entity<MapGridComponent>) (grid.Value, comp), user, this._transform.GetMoverCoordinates((EntityUid) ent), ent.Comp.MountExclusionAreaSize);
  }

  public void RotateMount(Entity<WeaponMountComponent> ent, EntityUid? user, int rotationDegrees = 90)
  {
    this._transform.SetLocalRotation((EntityUid) ent, Angle.op_Addition(this._transform.GetWorldRotation((EntityUid) ent), Angle.FromDegrees((double) rotationDegrees)));
    this._audio.PlayPredicted(ent.Comp.RotateSound, (EntityUid) ent, user);
    ScopeComponent comp;
    if (!ent.Comp.User.HasValue || !this.TryComp<ScopeComponent>(ent.Comp.MountedEntity, out comp))
      return;
    this._scope.StartScoping((Entity<ScopeComponent>) (ent.Comp.MountedEntity.Value, comp), ent.Comp.User.Value);
  }

  private bool TryAttachToMount(Entity<WeaponMountComponent> ent, EntityUid user, EntityUid used)
  {
    if (!this.CanAssembleMount(ent, user) || !this.IsViableWeapon(used, (EntityUid) ent))
      return false;
    this._doAfter.TryStartDoAfter(new DoAfterArgs(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.AssembleDelay, (DoAfterEvent) new AttachToMountDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(used))
    {
      NeedHand = true,
      BreakOnMove = true,
      BreakOnHandChange = true
    }));
    return true;
  }

  private bool TryDetachFromMount(
    Entity<WeaponMountComponent> ent,
    EntityUid user,
    EntityUid? used = null)
  {
    this._doAfter.TryStartDoAfter(new DoAfterArgs(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.DisassembleDelay, (DoAfterEvent) new DetachFromMountDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), used)
    {
      NeedHand = true,
      BreakOnMove = true,
      BreakOnHandChange = true
    }));
    return true;
  }

  private bool TryUndeployMount(Entity<WeaponMountComponent> ent, EntityUid user, EntityUid? used = null)
  {
    this._doAfter.TryStartDoAfter(new DoAfterArgs(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.DisassembleDelay, (DoAfterEvent) new MountUnDeployDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), used)
    {
      NeedHand = true,
      BreakOnMove = true,
      BreakOnHandChange = true
    }));
    return true;
  }

  private void EjectMagazine(EntityUid uid, ItemSlot slot, EntityUid user, EntityUid mount)
  {
    if (!this._slots.TryEjectToHands(uid, slot, new EntityUid?(user), true))
      return;
    WeaponMountComponentVisualLayers key = WeaponMountComponentVisualLayers.MountedAmmo;
    FoldableComponent comp;
    if (this.TryComp<FoldableComponent>(mount, out comp) && comp.IsFolded)
      key = WeaponMountComponentVisualLayers.FoldedAmmo;
    this._appearance.SetData(mount, (Enum) key, (object) false);
  }

  private void OnBreak(Entity<WeaponMountComponent> ent, ref BreakageEventArgs args)
  {
    FoldableComponent comp1;
    this.TryComp<FoldableComponent>((EntityUid) ent, out comp1);
    ent.Comp.Broken = true;
    this.DirtyField<WeaponMountComponent>(ent.Owner, ent.Comp, "Broken");
    CorrodibleComponent comp2;
    if (this.TryComp<CorrodibleComponent>((EntityUid) ent, out comp2))
      this.XenoAcid.SetCorrodible(comp2, true);
    this.UndeployMount(ent, foldable: comp1);
    this.UpdateAppearance((EntityUid) ent);
  }

  private void OnDamageModified(Entity<WeaponMountComponent> ent, ref DamageModifyEvent args)
  {
    FoldableComponent comp;
    if (!this.TryComp<FoldableComponent>((EntityUid) ent, out comp) || !comp.IsFolded)
      return;
    args.Damage = new DamageSpecifier();
  }

  private void OnCheckTileFree(Entity<WeaponMountComponent> ent, ref RMCCheckTileFreeEvent args)
  {
    if (!this.HasComp<BarricadeComponent>(args.AnchoredEntity))
      return;
    args.IsTileFree = true;
  }

  public bool IsViableWeapon(
    EntityUid weapon,
    EntityUid mount,
    WeaponMountComponent? weaponMountComponent = null)
  {
    return this.Resolve<WeaponMountComponent>(mount, ref weaponMountComponent, false) && this._whitelist.IsWhitelistPassOrNull(weaponMountComponent.MountableWhitelist, weapon);
  }

  private void OnWeaponOverheated(
    Entity<WeaponMountComponent> ent,
    ref MountableWeaponRelayedEvent<OverheatedEvent> args)
  {
    if (args.Args.Damage == null)
      return;
    this._damage.TryChangeDamage(new EntityUid?((EntityUid) ent), args.Args.Damage);
    if (!ent.Comp.MountedEntity.HasValue)
      return;
    this._popup.PopupClient(this.Loc.GetString("emplacement-mounted-weapon-overheated", ("weapon", (object) ent.Comp.MountedEntity.Value)), (EntityUid) ent, ent.Comp.User, PopupType.SmallCaution);
  }

  private void OnWeaponHeatGained(
    Entity<WeaponMountComponent> ent,
    ref MountableWeaponRelayedEvent<HeatGainedEvent> args)
  {
    this.UpdateAppearance((EntityUid) ent);
  }

  private void OnGetGunUser(Entity<WeaponMountComponent> ent, ref GetIFFGunUserEvent args)
  {
    args.GunUser = ent.Comp.User;
  }

  public void UpdateAppearance(EntityUid mount, WeaponMountComponent? mountComponent = null)
  {
    if (!this.Resolve<WeaponMountComponent>(mount, ref mountComponent, false))
      return;
    FoldableComponent comp1;
    if (this.TryComp<FoldableComponent>(mount, out comp1))
    {
      this._appearance.SetData(mount, (Enum) WeaponMountComponentVisualLayers.Mounted, (object) (bool) (comp1.IsFolded ? 0 : (mountComponent.MountedEntity.HasValue ? 1 : 0)));
      this._appearance.SetData(mount, (Enum) WeaponMountComponentVisualLayers.Folded, (object) (bool) (!comp1.IsFolded ? 0 : (mountComponent.MountedEntity.HasValue ? 1 : 0)));
      this._appearance.SetData(mount, (Enum) WeaponMountComponentVisualLayers.Broken, (object) mountComponent.Broken);
    }
    OverheatComponent comp2;
    if (!mountComponent.MountedEntity.HasValue || !this.TryComp<OverheatComponent>(mountComponent.MountedEntity.Value, out comp2))
      return;
    Color color;
    this._appearance.TryGetData<Color>(mount, (Enum) WeaponMountComponentVisualLayers.Overheated, out color);
    bool flag = comp1 == null || !comp1.IsFolded;
    this._appearance.SetData(mount, (Enum) WeaponMountComponentVisualLayers.Overheated, (object) flag);
    float num = Math.Clamp(comp2.Heat / (float) comp2.MaxHeat, 0.0f, 1f);
    this._appearance.SetData(mount, (Enum) WeaponMountComponentVisualLayers.Overheated, (object) ((Color) ref color).WithAlpha(num));
  }

  public bool TryGetWeaponAmmo(
    EntityUid mount,
    [NotNullWhen(true)] out int? ammoCount,
    [NotNullWhen(true)] out int? ammoCapacity,
    WeaponMountComponent? mountComponent = null)
  {
    ammoCount = new int?();
    ammoCapacity = new int?();
    ItemSlot itemSlot;
    if (!this.Resolve<WeaponMountComponent>(mount, ref mountComponent, false) || !mountComponent.MountedEntity.HasValue || !this._slots.TryGetSlot(mountComponent.MountedEntity.Value, "gun_magazine", out itemSlot))
      return false;
    EntityUid? nullable = itemSlot.Item;
    if (!nullable.HasValue)
      return false;
    GetAmmoCountEvent args = new GetAmmoCountEvent();
    nullable = itemSlot.Item;
    this.RaiseLocalEvent<GetAmmoCountEvent>(nullable.Value, ref args);
    ammoCount = new int?(args.Count);
    ammoCapacity = new int?(args.Capacity);
    return true;
  }

  public bool TryGetMountCone(Entity<WeaponMountComponent?> mount, out int shootArc)
  {
    shootArc = 0;
    if (!this.Resolve<WeaponMountComponent>((EntityUid) mount, ref mount.Comp, false) || !mount.Comp.MountedEntity.HasValue)
      return false;
    MountableWeaponComponent comp;
    shootArc = this.TryComp<MountableWeaponComponent>(mount.Comp.MountedEntity.Value, out comp) ? comp.ShootArc : 100;
    return true;
  }

  public bool TryGetMountSeatingRotation(EntityUid mountedWeapon, out Angle rotation)
  {
    rotation = new Angle();
    MountableWeaponComponent comp;
    if (!this.TryComp<MountableWeaponComponent>(mountedWeapon, out comp) || !comp.MountedTo.HasValue)
      return false;
    rotation = this._transform.GetWorldRotation(this.GetEntity(comp.MountedTo.Value));
    return true;
  }
}
