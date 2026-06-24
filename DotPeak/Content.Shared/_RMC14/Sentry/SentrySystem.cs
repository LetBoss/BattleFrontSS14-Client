// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sentry.SentrySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Interaction;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.NPC;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Sentry;

public sealed class SentrySystem : EntitySystem
{
  private static readonly ProtoId<ToolQualityPrototype> ScrewingQuality = (ProtoId<ToolQualityPrototype>) "Screwing";
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private FixtureSystem _fixture;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private RMCInteractionSystem _rmcInteraction;
  [Dependency]
  private SharedRMCNPCSystem _rmcNpc;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedToolSystem _tools;
  [Dependency]
  private DamageableSystem _damageableSystem;
  [Dependency]
  private SharedSentryTargetingSystem _targeting;
  [Dependency]
  private GunIFFSystem _gunIFF;
  [Dependency]
  private SharedPointLightSystem _pointLight;
  private readonly HashSet<EntityUid> _toUpdate = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SentryComponent, MapInitEvent>(new EntityEventRefHandler<SentryComponent, MapInitEvent>(this.OnSentryMapInit));
    this.SubscribeLocalEvent<SentryComponent, PickupAttemptEvent>(new EntityEventRefHandler<SentryComponent, PickupAttemptEvent>(this.OnSentryPickupAttempt));
    this.SubscribeLocalEvent<SentryComponent, UseInHandEvent>(new EntityEventRefHandler<SentryComponent, UseInHandEvent>(this.OnSentryUseInHand));
    this.SubscribeLocalEvent<SentryComponent, SentryDeployDoAfterEvent>(new EntityEventRefHandler<SentryComponent, SentryDeployDoAfterEvent>(this.OnSentryDeployDoAfter));
    this.SubscribeLocalEvent<SentryComponent, ActivateInWorldEvent>(new EntityEventRefHandler<SentryComponent, ActivateInWorldEvent>(this.OnSentryActivateInWorld));
    this.SubscribeLocalEvent<SentryComponent, AttemptShootEvent>(new EntityEventRefHandler<SentryComponent, AttemptShootEvent>(this.OnSentryAttemptShoot));
    this.SubscribeLocalEvent<SentryComponent, InteractUsingEvent>(new EntityEventRefHandler<SentryComponent, InteractUsingEvent>(this.OnSentryInteractUsing));
    this.SubscribeLocalEvent<SentryComponent, SentryInsertMagazineDoAfterEvent>(new EntityEventRefHandler<SentryComponent, SentryInsertMagazineDoAfterEvent>(this.OnSentryInsertMagazineDoAfter));
    this.SubscribeLocalEvent<SentryComponent, SentryDisassembleDoAfterEvent>(new EntityEventRefHandler<SentryComponent, SentryDisassembleDoAfterEvent>(this.OnSentryDisassembleDoAfter));
    this.SubscribeLocalEvent<SentryComponent, ExaminedEvent>(new EntityEventRefHandler<SentryComponent, ExaminedEvent>(this.OnSentryExamined));
    this.SubscribeLocalEvent<SentryComponent, CombatModeShouldHandInteractEvent>(new EntityEventRefHandler<SentryComponent, CombatModeShouldHandInteractEvent>(this.OnSentryShouldInteract));
    this.SubscribeLocalEvent<SentrySpikesComponent, AttackedEvent>(new EntityEventRefHandler<SentrySpikesComponent, AttackedEvent>(this.OnSentrySpikesAttacked));
    this.Subs.BuiEvents<SentryComponent>((object) SentryUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<SentryComponent>) (subs => subs.Event<SentryUpgradeBuiMsg>(new EntityEventRefHandler<SentryComponent, SentryUpgradeBuiMsg>(this.OnSentryUpgradeBuiMsg))));
  }

  private void OnSentryMapInit(Entity<SentryComponent> sentry, ref MapInitEvent args)
  {
    this._toUpdate.Add((EntityUid) sentry);
    EntProtoId<BallisticAmmoProviderComponent>? startingMagazine = sentry.Comp.StartingMagazine;
    if (startingMagazine.HasValue)
      this.TrySpawnInContainer((string) startingMagazine.GetValueOrDefault(), (EntityUid) sentry, sentry.Comp.ContainerSlotId, out EntityUid? _);
    this.UpdateState(sentry);
  }

  private void OnSentryPickupAttempt(Entity<SentryComponent> sentry, ref PickupAttemptEvent args)
  {
    if (args.Cancelled || sentry.Comp.Mode == SentryMode.Item)
      return;
    args.Cancel();
  }

  private void OnSentryUseInHand(Entity<SentryComponent> sentry, ref UseInHandEvent args)
  {
    args.Handled = true;
    if (!this.CanDeployPopup(sentry, args.User, out EntityCoordinates _, out Angle _))
      return;
    SentryDeployDoAfterEvent @event = new SentryDeployDoAfterEvent();
    TimeSpan delay = sentry.Comp.DeployDelay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) args.User, sentry.Comp.DelaySkill);
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) sentry))
    {
      BreakOnMove = true
    });
  }

  private void OnSentryDeployDoAfter(
    Entity<SentryComponent> sentry,
    ref SentryDeployDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityCoordinates coordinates;
    Angle rotation;
    if (!this.CanDeployPopup(sentry, args.User, out coordinates, out rotation))
      return;
    sentry.Comp.Mode = SentryMode.Off;
    this.Dirty<SentryComponent>(sentry);
    TransformComponent xform = this.Transform((EntityUid) sentry);
    this._transform.SetCoordinates((EntityUid) sentry, xform, coordinates, new Angle?(rotation));
    this._transform.AnchorEntity((EntityUid) sentry, xform);
    this._rmcInteraction.SetMaxRotation((Entity<MaxRotationComponent>) sentry.Owner, rotation, sentry.Comp.MaxDeviation);
    this._targeting.ApplyDeployerFactions(sentry.Owner, args.User);
    this.UpdateState(sentry);
  }

  private void OnSentryActivateInWorld(
    Entity<SentryComponent> sentry,
    ref ActivateInWorldEvent args)
  {
    ref SentryMode local = ref sentry.Comp.Mode;
    if (local == SentryMode.Item || sentry.Comp.IsLocked)
      return;
    args.Handled = true;
    EntityUid user = args.User;
    if (local == SentryMode.Off)
    {
      foreach (Entity<SentryComponent> entity in this._entityLookup.GetEntitiesInRange<SentryComponent>(this._transform.GetMapCoordinates((EntityUid) sentry), (float) sentry.Comp.DefenseCheckRange))
      {
        if (sentry != entity && entity.Comp.Mode == SentryMode.On)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-sentry-too-close", ("defense", (object) entity)), (EntityUid) sentry, new EntityUid?(user));
          return;
        }
      }
      local = SentryMode.On;
      this._popup.PopupClient(this.Loc.GetString("rmc-sentry-on", (nameof (sentry), (object) sentry)), (EntityUid) sentry, new EntityUid?(user));
    }
    else
    {
      local = SentryMode.Off;
      this._popup.PopupClient(this.Loc.GetString("rmc-sentry-off", (nameof (sentry), (object) sentry)), (EntityUid) sentry, new EntityUid?(user));
    }
    this.Dirty<SentryComponent>(sentry);
    this.UpdateState(sentry);
  }

  private void OnSentryAttemptShoot(Entity<SentryComponent> ent, ref AttemptShootEvent args)
  {
    if (!(args.User != ent.Owner))
      return;
    args.Cancelled = true;
  }

  private void OnSentryInteractUsing(Entity<SentryComponent> sentry, ref InteractUsingEvent args)
  {
    if (sentry.Comp.IsLocked)
      return;
    EntityUid user1 = args.User;
    EntityUid used1 = args.Used;
    SentryUpgradeItemComponent comp;
    if (this.TryComp<SentryUpgradeItemComponent>(used1, out comp))
      this.OpenUpgradeMenu(sentry, (Entity<SentryUpgradeItemComponent>) (used1, comp), user1);
    else if (this.HasComp<MultitoolComponent>(used1))
      this.StartDisassemble(sentry, user1);
    else if (this._tools.HasQuality(used1, (string) SentrySystem.ScrewingQuality))
    {
      if (sentry.Comp.Mode == SentryMode.Off)
      {
        this._transform.SetWorldRotation((EntityUid) sentry, Angle.op_Addition(this._transform.GetWorldRotation((EntityUid) sentry), Angle.FromDegrees(90.0)));
        RMCInteractionSystem rmcInteraction = this._rmcInteraction;
        Entity<MaxRotationComponent> owner = (Entity<MaxRotationComponent>) sentry.Owner;
        Angle localRotation = this.Transform((EntityUid) sentry).LocalRotation;
        Angle angle = DirectionExtensions.ToAngle(((Angle) ref localRotation).GetCardinalDir());
        Angle maxDeviation = sentry.Comp.MaxDeviation;
        rmcInteraction.SetMaxRotation(owner, angle, maxDeviation);
        this.UpdateState(sentry);
        this._audio.PlayPredicted(sentry.Comp.ScrewdriverSound, (EntityUid) sentry, new EntityUid?(user1));
        this._popup.PopupPredicted(this.Loc.GetString("rmc-sentry-rotate-self", (nameof (sentry), (object) sentry)), this.Loc.GetString("rmc-sentry-rotate-others", ("user", (object) user1), (nameof (sentry), (object) sentry)), user1, new EntityUid?(user1));
        args.Handled = true;
      }
      else
        this._popup.PopupClient(sentry.Comp.Mode != SentryMode.On ? this.Loc.GetString("rmc-sentry-item-norot", (nameof (sentry), (object) sentry)) : this.Loc.GetString("rmc-sentry-active-norot", (nameof (sentry), (object) sentry)), (EntityUid) sentry, new EntityUid?(user1));
    }
    else
    {
      if (!this.HasComp<BallisticAmmoProviderComponent>(used1))
        return;
      ProtoId<TagPrototype>? magazineTag = sentry.Comp.MagazineTag;
      if (magazineTag.HasValue)
      {
        ProtoId<TagPrototype> valueOrDefault = magazineTag.GetValueOrDefault();
        if (!this._tag.HasTag(used1, valueOrDefault))
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-sentry-magazine-does-not-fit", (nameof (sentry), (object) sentry), ("magazine", (object) used1)), (EntityUid) sentry, new EntityUid?(user1), PopupType.SmallCaution);
          return;
        }
      }
      args.Handled = true;
      if (!this.CanInsertMagazinePopup(sentry, user1, used1, out ContainerSlot _))
        return;
      TimeSpan timeSpan = sentry.Comp.MagazineDelay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) user1, sentry.Comp.Skill);
      SentryInsertMagazineDoAfterEvent magazineDoAfterEvent = new SentryInsertMagazineDoAfterEvent();
      EntityManager entityManager = this.EntityManager;
      EntityUid user2 = user1;
      TimeSpan delay = timeSpan;
      SentryInsertMagazineDoAfterEvent @event = magazineDoAfterEvent;
      EntityUid? eventTarget = new EntityUid?((EntityUid) sentry);
      EntityUid? nullable = new EntityUid?(used1);
      EntityUid? target = new EntityUid?();
      EntityUid? used2 = nullable;
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user2, delay, (DoAfterEvent) @event, eventTarget, target, used2)
      {
        BreakOnMove = true
      }))
        return;
      this._popup.PopupPredicted(this.Loc.GetString("rmc-sentry-magazine-swap-start-user", ("magazine", (object) used1), (nameof (sentry), (object) sentry)), this.Loc.GetString("rmc-sentry-magazine-swap-start-others", ("user", (object) user1), ("magazine", (object) used1), (nameof (sentry), (object) sentry)), user1, new EntityUid?(user1));
    }
  }

  private void OnSentryInsertMagazineDoAfter(
    Entity<SentryComponent> sentry,
    ref SentryInsertMagazineDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? used = args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault = used.GetValueOrDefault();
    args.Handled = true;
    EntityUid user = args.User;
    ContainerSlot slot;
    if (!this.CanInsertMagazinePopup(sentry, user, valueOrDefault, out slot))
      return;
    this._container.EmptyContainer((BaseContainer) slot, destination: new EntityCoordinates?(this._transform.GetMoverCoordinates(user)));
    if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) valueOrDefault, (BaseContainer) slot))
      return;
    this._popup.PopupPredicted(this.Loc.GetString("rmc-sentry-magazine-swap-finish-user", ("magazine", (object) valueOrDefault), (nameof (sentry), (object) sentry)), this.Loc.GetString("rmc-sentry-magazine-swap-finish-others", ("user", (object) user), ("magazine", (object) valueOrDefault), (nameof (sentry), (object) sentry)), user, new EntityUid?(user));
    this._audio.PlayPredicted(sentry.Comp.MagazineSwapSound, (EntityUid) sentry, new EntityUid?(user));
  }

  private void OnSentryDisassembleDoAfter(
    Entity<SentryComponent> sentry,
    ref SentryDisassembleDoAfterEvent args)
  {
    EntityUid user = args.User;
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    sentry.Comp.Mode = SentryMode.Item;
    this.RemCompDeferred<MaxRotationComponent>((EntityUid) sentry);
    this._transform.Unanchor(sentry.Owner, this.Transform((EntityUid) sentry));
    this.UpdateState(sentry);
    this._popup.PopupPredicted(this.Loc.GetString("rmc-sentry-disassemble-finish-self", (nameof (sentry), (object) sentry)), this.Loc.GetString("rmc-sentry-disassemble-finish-others", ("user", (object) user), (nameof (sentry), (object) sentry)), (EntityUid) sentry, new EntityUid?(user));
  }

  private void OnSentryExamined(Entity<SentryComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("SentryComponent"))
    {
      if (Angle.op_Implicit(ent.Comp.MaxDeviation) < Angle.op_Implicit(Angle.FromDegrees(180.0)))
      {
        string markup = this.Loc.GetString("rmc-sentry-limited-rotation", ("degrees", (object) (int) ((Angle) ref ent.Comp.MaxDeviation).Degrees));
        args.PushMarkup(markup);
      }
      if (!ent.Comp.IsLocked)
      {
        string markup = this.Loc.GetString("rmc-sentry-disassembled-with-multitool");
        args.PushMarkup(markup);
      }
      if (ent.Comp.Mode != SentryMode.Off)
        return;
      string markup1 = this.Loc.GetString("rmc-sentry-rotate-with-screwdriver");
      args.PushMarkup(markup1);
    }
  }

  private void OnSentryShouldInteract(
    Entity<SentryComponent> ent,
    ref CombatModeShouldHandInteractEvent args)
  {
    args.Cancelled = true;
  }

  private void OnSentryUpgradeBuiMsg(
    Entity<SentryComponent> oldSentry,
    ref SentryUpgradeBuiMsg args)
  {
    this._ui.CloseUi((Entity<UserInterfaceComponent>) oldSentry.Owner, (Enum) SentryUiKey.Key);
    EntityUid actor = args.Actor;
    EntProtoId upgrade = args.Upgrade;
    Entity<SentryUpgradeItemComponent> upgradeItem = new Entity<SentryUpgradeItemComponent>();
    if (upgrade == new EntProtoId() || !this.CanUpgradePopup(oldSentry, ref upgradeItem, actor, new EntProtoId?(upgrade)) || this._net.IsClient)
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) oldSentry);
    Angle worldRotation = this._transform.GetWorldRotation((EntityUid) oldSentry);
    this.QueueDel(new EntityUid?((EntityUid) upgradeItem));
    this.QueueDel(new EntityUid?((EntityUid) oldSentry));
    EntityUid entityUid = this.Spawn((string) upgrade, mapCoordinates, rotation: worldRotation);
    SentryUpgradedEvent args1 = new SentryUpgradedEvent((EntityUid) oldSentry, entityUid, actor);
    this.RaiseLocalEvent<SentryUpgradedEvent>(entityUid, ref args1);
  }

  private void UpdateState(Entity<SentryComponent> sentry)
  {
    string deployFixture = sentry.Comp.DeployFixture;
    FixturesComponent comp;
    Fixture fixtureOrNull = deployFixture == null || !this.TryComp<FixturesComponent>((EntityUid) sentry, out comp) ? (Fixture) null : this._fixture.GetFixtureOrNull((EntityUid) sentry, deployFixture, comp);
    switch (sentry.Comp.Mode)
    {
      case SentryMode.Item:
        if (fixtureOrNull != null)
          this._physics.SetHard((EntityUid) sentry, fixtureOrNull, false);
        this._rmcNpc.SleepNPC((EntityUid) sentry);
        this._appearance.SetData((EntityUid) sentry, (Enum) SentryLayers.Layer, (object) SentryMode.Item);
        this._pointLight.SetEnabled((EntityUid) sentry, false);
        break;
      case SentryMode.Off:
        if (fixtureOrNull != null)
          this._physics.SetHard((EntityUid) sentry, fixtureOrNull, true);
        this._rmcNpc.SleepNPC((EntityUid) sentry);
        this._appearance.SetData((EntityUid) sentry, (Enum) SentryLayers.Layer, (object) SentryMode.Off);
        this._pointLight.SetEnabled((EntityUid) sentry, false);
        break;
      case SentryMode.On:
        if (fixtureOrNull != null)
          this._physics.SetHard((EntityUid) sentry, fixtureOrNull, true);
        this._rmcNpc.WakeNPC((EntityUid) sentry);
        this._appearance.SetData((EntityUid) sentry, (Enum) SentryLayers.Layer, (object) SentryMode.On);
        this._pointLight.SetEnabled((EntityUid) sentry, true);
        break;
    }
  }

  private bool CanDeployPopup(
    Entity<SentryComponent> sentry,
    EntityUid user,
    out EntityCoordinates coordinates,
    out Angle rotation)
  {
    coordinates = new EntityCoordinates();
    rotation = new Angle();
    (EntityCoordinates Coords, Angle worldRot) coordinateRotation = this._transform.GetMoverCoordinateRotation(user, this.Transform(user));
    coordinates = coordinateRotation.Coords;
    // ISSUE: explicit reference operation
    rotation = DirectionExtensions.ToAngle(((Angle) @coordinateRotation.worldRot).GetCardinalDir());
    Direction cardinalDir = ((Angle) ref rotation).GetCardinalDir();
    coordinates = coordinates.Offset(DirectionExtensions.ToVec(cardinalDir));
    if (this._rmcMap.CanBuildOn(coordinates))
      return true;
    this._popup.PopupClient(this.Loc.GetString("rmc-sentry-need-open-area", (nameof (sentry), (object) sentry)), user, new EntityUid?(user), PopupType.SmallCaution);
    return false;
  }

  private bool CanInsertMagazinePopup(
    Entity<SentryComponent> sentry,
    EntityUid user,
    EntityUid used,
    [NotNullWhen(true)] out ContainerSlot? slot)
  {
    slot = (ContainerSlot) null;
    slot = this._container.EnsureContainer<ContainerSlot>((EntityUid) sentry, sentry.Comp.ContainerSlotId);
    if (!this._container.CanInsert(used, (BaseContainer) slot, true))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-sentry-magazine-invalid", ("item", (object) used)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    BallisticAmmoProviderComponent comp;
    if (!this.TryComp<BallisticAmmoProviderComponent>(slot.ContainedEntity, out comp) || comp.Count <= 0 || this.HasComp<BypassInteractionChecksComponent>(user))
      return true;
    this._popup.PopupClient(this.Loc.GetString("rmc-sentry-magazine-swap-not-empty"), user, new EntityUid?(user), PopupType.SmallCaution);
    return false;
  }

  private void OpenUpgradeMenu(
    Entity<SentryComponent> sentry,
    Entity<SentryUpgradeItemComponent> upgrade,
    EntityUid user)
  {
    if (!this.CanUpgradePopup(sentry, ref upgrade, user, new EntProtoId?()))
      return;
    this._ui.OpenUi((Entity<UserInterfaceComponent>) sentry.Owner, (Enum) SentryUiKey.Key, new EntityUid?(user));
  }

  private bool CanUpgradePopup(
    Entity<SentryComponent> sentry,
    ref Entity<SentryUpgradeItemComponent> upgradeItem,
    EntityUid user,
    EntProtoId? upgrade)
  {
    EntProtoId[] upgrades = sentry.Comp.Upgrades;
    if (upgrades == null || upgrades.Length <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-sentry-upgrade-not-upgradeable", (nameof (sentry), (object) sentry)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (sentry.Comp.Mode != SentryMode.Item)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-sentry-upgrade-not-item", (nameof (sentry), (object) sentry)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (upgradeItem == new Entity<SentryUpgradeItemComponent>())
    {
      EntityUid? uid;
      SentryUpgradeItemComponent comp;
      if (!this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out uid) || !this.TryComp<SentryUpgradeItemComponent>(uid, out comp))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-sentry-upgrade-not-holding", (nameof (sentry), (object) sentry)), user, new EntityUid?(user), PopupType.SmallCaution);
        return false;
      }
      upgradeItem = (Entity<SentryUpgradeItemComponent>) (uid.Value, comp);
    }
    if (!upgrade.HasValue || ((IEnumerable<EntProtoId>) upgrades).Contains<EntProtoId>(upgrade.Value))
      return true;
    this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) user)} tried to upgrade sentry {this.ToPrettyString(new EntityUid?((EntityUid) sentry))} to invalid upgrade {upgrade.Value}");
    return false;
  }

  private void StartDisassemble(Entity<SentryComponent> sentry, EntityUid user)
  {
    if (sentry.Comp.Mode == SentryMode.Item)
      return;
    SentryDisassembleDoAfterEvent @event = new SentryDisassembleDoAfterEvent();
    TimeSpan delay = sentry.Comp.UndeployDelay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) user, sentry.Comp.DelaySkill);
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) sentry))
    {
      BreakOnMove = true
    }))
      return;
    this._popup.PopupPredicted(this.Loc.GetString("rmc-sentry-disassemble-start-self", (nameof (sentry), (object) sentry)), this.Loc.GetString("rmc-sentry-disassemble-start-others", (nameof (user), (object) user), (nameof (sentry), (object) sentry)), (EntityUid) sentry, new EntityUid?(user));
  }

  public bool TrySetMode(Entity<SentryComponent> sentry, SentryMode mode)
  {
    if (sentry.Comp.Mode == mode)
      return false;
    sentry.Comp.Mode = mode;
    this.UpdateState(sentry);
    this.Dirty((EntityUid) sentry, (IComponent) sentry.Comp);
    return true;
  }

  public bool TryGetSentryAmmo(
    EntityUid sentry,
    [NotNullWhen(true)] out int? ammoCount,
    [NotNullWhen(true)] out int? ammoCapacity,
    SentryComponent? sentryComponent = null)
  {
    ammoCount = new int?();
    ammoCapacity = new int?();
    BaseContainer container;
    if (!this.Resolve<SentryComponent>(sentry, ref sentryComponent, false) || !this._container.TryGetContainer(sentry, sentryComponent.ContainerSlotId, out container) || container.Count == 0)
      return false;
    GetAmmoCountEvent args = new GetAmmoCountEvent();
    this.RaiseLocalEvent<GetAmmoCountEvent>(container.ContainedEntities[0], ref args);
    ammoCount = new int?(args.Count);
    ammoCapacity = new int?(args.Capacity);
    return true;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
    {
      this._toUpdate.Clear();
    }
    else
    {
      try
      {
        foreach (EntityUid uid in this._toUpdate)
        {
          SentryComponent comp;
          if (this.TryComp<SentryComponent>(uid, out comp))
            this.UpdateState((Entity<SentryComponent>) (uid, comp));
        }
      }
      finally
      {
        this._toUpdate.Clear();
      }
    }
  }

  private void OnSentrySpikesAttacked(Entity<SentrySpikesComponent> sentry, ref AttackedEvent args)
  {
    SentryComponent comp;
    if (!this.TryComp<SentryComponent>((EntityUid) sentry, out comp) || comp.Mode != SentryMode.On)
      return;
    this._damageableSystem.TryChangeDamage(new EntityUid?(args.User), sentry.Comp.SpikeDamage, origin: new EntityUid?((EntityUid) sentry), tool: new EntityUid?((EntityUid) sentry));
    this._popup.PopupPredicted(this.Loc.GetString("rmc-sentry-spikes-self"), this.Loc.GetString("rmc-sentry-spikes-others", ("target", (object) args.User)), (EntityUid) sentry, new EntityUid?(args.User), PopupType.SmallCaution);
  }
}
