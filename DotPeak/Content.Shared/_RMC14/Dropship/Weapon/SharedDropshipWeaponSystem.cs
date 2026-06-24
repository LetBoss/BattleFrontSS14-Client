// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Weapon.SharedDropshipWeaponSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.ElectronicSystem;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Systems;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Explosion.Implosion;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Medical.MedevacStretcher;
using Content.Shared._RMC14.OnCollide;
using Content.Shared._RMC14.PowerLoader;
using Content.Shared._RMC14.Rangefinder;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Administration.Logs;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Examine;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.GameTicking;
using Content.Shared.IgnitionSource;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Light.Components;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.ParaDrop;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Throwing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Weapon;

public abstract class SharedDropshipWeaponSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoorSystem _door;
  [Dependency]
  private SharedDropshipSystem _dropship;
  [Dependency]
  private SharedRMCEquipmentDeployerSystem _equipmentDeployer;
  [Dependency]
  private DropshipUtilitySystem _dropshipUtility;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedEyeSystem _eye;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private NameModifierSystem _name;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedOnCollideSystem _onCollide;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPointLightSystem _pointLight;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private PowerLoaderSystem _powerloader;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedRMCFlammableSystem _rmcFlammable;
  [Dependency]
  private SharedRMCExplosionSystem _rmcExplosion;
  [Dependency]
  private RMCImplosionSystem _rmcImplosion;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SquadSystem _squad;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  private static readonly EntProtoId DropshipTargetMarker = (EntProtoId) "RMCLaserDropshipTarget";
  private const string SpotlightState = "spotlights_";
  private readonly HashSet<Entity<DamageableComponent>> _damageables = new HashSet<Entity<DamageableComponent>>();
  private readonly List<EntityUid> _targetsToRemove = new List<EntityUid>();
  private int _nextId = 1;

  public bool CasDebug { get; private set; }

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<FlareSignalComponent, IgnitionEvent>(new EntityEventRefHandler<FlareSignalComponent, IgnitionEvent>(this.OnFlareSignalIgnition));
    this.SubscribeLocalEvent<FlareSignalComponent, GettingPickedUpAttemptEvent>(new EntityEventRefHandler<FlareSignalComponent, GettingPickedUpAttemptEvent>(this.OnFlareSignalPickupAttempt));
    this.SubscribeLocalEvent<FlareSignalComponent, ExaminedEvent>(new EntityEventRefHandler<FlareSignalComponent, ExaminedEvent>(this.OnFlareSignalExamined));
    this.SubscribeLocalEvent<FlareSignalComponent, DroppedEvent>(new EntityEventRefHandler<FlareSignalComponent, DroppedEvent>(this.OnFlareSignalDropped));
    this.SubscribeLocalEvent<FlareSignalComponent, ThrownEvent>(new EntityEventRefHandler<FlareSignalComponent, ThrownEvent>(this.OnFlareSignalThrown));
    this.SubscribeLocalEvent<FlareSignalComponent, GrenadeContentThrownEvent>(new EntityEventRefHandler<FlareSignalComponent, GrenadeContentThrownEvent>(this.OnFlareSignalGrenadeContentThrown));
    this.SubscribeLocalEvent<FlareSignalComponent, StopThrowEvent>(new EntityEventRefHandler<FlareSignalComponent, StopThrowEvent>(this.OnFlareSignalStopThrow));
    this.SubscribeLocalEvent<FlareSignalComponent, ContainerGettingInsertedAttemptEvent>(new EntityEventRefHandler<FlareSignalComponent, ContainerGettingInsertedAttemptEvent>(this.OnFlareSignalContainerGettingInsertedAttempt));
    this.SubscribeLocalEvent<DropshipTerminalWeaponsComponent, MapInitEvent>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, MapInitEvent>(this.OnTerminalMapInit));
    this.SubscribeLocalEvent<DropshipTerminalWeaponsComponent, BoundUIOpenedEvent>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, BoundUIOpenedEvent>(this.OnTerminalBUIOpened));
    this.SubscribeLocalEvent<DropshipTerminalWeaponsComponent, BoundUIClosedEvent>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, BoundUIClosedEvent>(this.OnTerminalBUIClosed));
    this.SubscribeLocalEvent<DropshipTargetComponent, MapInitEvent>(new EntityEventRefHandler<DropshipTargetComponent, MapInitEvent>(this.OnDropshipTargetMapInit));
    this.SubscribeLocalEvent<DropshipTargetComponent, ComponentRemove>(new EntityEventRefHandler<DropshipTargetComponent, ComponentRemove>(this.OnDropshipTargetRemove<ComponentRemove>));
    this.SubscribeLocalEvent<DropshipTargetComponent, EntityTerminatingEvent>(new EntityEventRefHandler<DropshipTargetComponent, EntityTerminatingEvent>(this.OnDropshipTargetRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<DropshipTargetComponent, ExaminedEvent>(new EntityEventRefHandler<DropshipTargetComponent, ExaminedEvent>(this.OnActiveFlareExamined));
    this.SubscribeLocalEvent<ActiveFlareSignalComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<ActiveFlareSignalComponent, RefreshNameModifiersEvent>(this.OnRefreshNameModifier));
    this.SubscribeLocalEvent<DropshipTargetEyeComponent, ComponentRemove>(new EntityEventRefHandler<DropshipTargetEyeComponent, ComponentRemove>(this.OnDropshipTargetEyeRemove<ComponentRemove>));
    this.SubscribeLocalEvent<DropshipTargetEyeComponent, EntityTerminatingEvent>(new EntityEventRefHandler<DropshipTargetEyeComponent, EntityTerminatingEvent>(this.OnDropshipTargetEyeRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<DropshipAmmoComponent, ExaminedEvent>(new EntityEventRefHandler<DropshipAmmoComponent, ExaminedEvent>(this.OnAmmoExamined));
    this.SubscribeLocalEvent<DropshipAmmoComponent, PowerLoaderInteractEvent>(new EntityEventRefHandler<DropshipAmmoComponent, PowerLoaderInteractEvent>(this.OnAmmoInteract));
    this.SubscribeLocalEvent<ActivateDropshipWeaponOnSpawnComponent, MapInitEvent>(new EntityEventRefHandler<ActivateDropshipWeaponOnSpawnComponent, MapInitEvent>(this.OnDropshipWeaponOnSpawnFire));
    this.Subs.BuiEvents<DropshipTerminalWeaponsComponent>((object) DropshipTerminalWeaponsUi.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<DropshipTerminalWeaponsComponent>) (subs =>
    {
      subs.Event<DropshipTerminalWeaponsChangeScreenMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChangeScreenMsg>(this.OnWeaponsChangeScreenMsg));
      subs.Event<DropshipTerminalWeaponsChooseWeaponMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseWeaponMsg>(this.OnWeaponsChooseWeaponMsg));
      subs.Event<DropshipTerminalWeaponsChooseMedevacMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseMedevacMsg>(this.OnWeaponsChooseMedevacMsg));
      subs.Event<DropshipTerminalWeaponsChooseFultonMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseFultonMsg>(this.OnWeaponsChooseFultonMsg));
      subs.Event<DropshipTerminalWeaponsChooseParaDropMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseParaDropMsg>(this.OnWeaponsChooseParaDropMsg));
      subs.Event<DropshipTerminalWeaponsChooseSpotlightMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseSpotlightMsg>(this.OnWeaponsChooseSpotlightMsg));
      subs.Event<DropshipTerminalWeaponsChooseEquipmentDeployerMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsChooseEquipmentDeployerMsg>(this.OnWeaponsChooseEquipmentDeployerMsg));
      subs.Event<DropshipTerminalWeaponsFireMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsFireMsg>(this.OnWeaponsFireMsg));
      subs.Event<DropshipTerminalWeaponsNightVisionMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsNightVisionMsg>(this.OnWeaponsNightVisionMsg));
      subs.Event<DropshipTerminalWeaponsExitMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsExitMsg>(this.OnWeaponsExitMsg));
      subs.Event<DropshipTerminalWeaponsCancelMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsCancelMsg>(this.OnWeaponsCancelMsg));
      subs.Event<DropshipTerminalWeaponsAdjustOffsetMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsAdjustOffsetMsg>(this.OnWeaponsAdjustOffset));
      subs.Event<DropshipTerminalWeaponsResetOffsetMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsResetOffsetMsg>(this.OnWeaponsResetOffset));
      subs.Event<DropshipTerminalWeaponsTargetsPreviousMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsTargetsPreviousMsg>(this.OnWeaponsTargetsPrevious));
      subs.Event<DropshipTerminalWeaponsTargetsNextMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsTargetsNextMsg>(this.OnWeaponsTargetsNext));
      subs.Event<DropshipTerminalWeaponsTargetsSelectMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsTargetsSelectMsg>(this.OnWeaponsTargetsSelect));
      subs.Event<DropshipTerminalWeaponsMedevacPreviousMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsMedevacPreviousMsg>(this.OnWeaponsMedevacPrevious));
      subs.Event<DropshipTerminalWeaponsMedevacNextMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsMedevacNextMsg>(this.OnWeaponsMedevacNext));
      subs.Event<DropshipTerminalWeaponsMedevacSelectMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsMedevacSelectMsg>(this.OnWeaponsMedevacSelect));
      subs.Event<DropshipTerminalWeaponsFultonPreviousMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsFultonPreviousMsg>(this.OnWeaponsFultonPrevious));
      subs.Event<DropshipTerminalWeaponsFultonNextMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsFultonNextMsg>(this.OnWeaponsFultonNext));
      subs.Event<DropshipTerminalWeaponsFultonSelectMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropshipTerminalWeaponsFultonSelectMsg>(this.OnWeaponsFultonSelect));
      subs.Event<DropShipTerminalWeaponsParaDropTargetSelectMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropShipTerminalWeaponsParaDropTargetSelectMsg>(this.OnWeaponsParaDropSelect));
      subs.Event<DropShipTerminalWeaponsSpotlightToggleMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropShipTerminalWeaponsSpotlightToggleMsg>(this.OnWeaponsSpotlightSelect));
      subs.Event<DropShipTerminalWeaponsEquipmentDeployToggleMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropShipTerminalWeaponsEquipmentDeployToggleMsg>(this.OnEquipmentDeploy));
      subs.Event<DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg>(this.OnEquipmentToggleAutoDeploy));
    }));
    this.Subs.CVar<bool>(this._config, RMCCVars.RMCDropshipCASDebug, (Action<bool>) (v => this.CasDebug = v), true);
  }

  private void SetTarget(
    Entity<DropshipTerminalWeaponsComponent> dropshipWeaponsTerminal,
    EntityUid? newTarget)
  {
    EntityUid? nullable = newTarget;
    EntityUid? target = dropshipWeaponsTerminal.Comp.Target;
    Entity<DropshipComponent> dropship;
    if ((nullable.HasValue == target.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == target.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 || !this._dropship.TryGetGridDropship((EntityUid) dropshipWeaponsTerminal, out dropship))
      return;
    dropshipWeaponsTerminal.Comp.Target = newTarget;
    this.Dirty<DropshipTerminalWeaponsComponent>(dropshipWeaponsTerminal);
    DropshipTargetChangedEvent args = new DropshipTargetChangedEvent(this.GetNetEntity(newTarget));
    foreach (EntityUid attachmentPoint in dropship.Comp.AttachmentPoints)
      this.RaiseLocalEvent<DropshipTargetChangedEvent>(attachmentPoint, args);
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev) => this._nextId = 1;

  private void OnFlareSignalIgnition(Entity<FlareSignalComponent> ent, ref IgnitionEvent args)
  {
    if (args.Ignite)
      return;
    if (this.HasComp<PhysicsComponent>((EntityUid) ent))
      this._physics.SetBodyType((EntityUid) ent, BodyType.Dynamic);
    this.RemCompDeferred<DropshipTargetComponent>((EntityUid) ent);
  }

  private void OnFlareSignalPickupAttempt(
    Entity<FlareSignalComponent> ent,
    ref GettingPickedUpAttemptEvent args)
  {
    if (args.Cancelled || !this.IsFlareLit((EntityUid) ent))
      return;
    args.Cancel();
  }

  private void OnFlareSignalExamined(Entity<FlareSignalComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("FlareSignalComponent"))
    {
      ExpendableLightComponent comp;
      if (!this.TryComp<ExpendableLightComponent>((EntityUid) ent, out comp) || comp.CurrentState == ExpendableLightState.Dead)
        return;
      args.PushMarkup(this.Loc.GetString("rmc-laser-designator-signal-flare-examine"));
    }
  }

  private void OnFlareSignalDropped(Entity<FlareSignalComponent> ent, ref DroppedEvent args)
  {
    if (!this.IsFlareLit((EntityUid) ent))
      return;
    this.StartTrackingActiveFlare(ent, new EntityUid?(args.User));
  }

  private void OnFlareSignalThrown(Entity<FlareSignalComponent> ent, ref ThrownEvent args)
  {
    if (!this.IsFlareLit((EntityUid) ent))
      return;
    this.StartTrackingActiveFlare(ent, args.User);
  }

  private void OnFlareSignalStopThrow(Entity<FlareSignalComponent> ent, ref StopThrowEvent args)
  {
    if (!this.HasComp<DropshipTargetComponent>((EntityUid) ent))
      return;
    this._physics.SetBodyType((EntityUid) ent, BodyType.Static);
  }

  private void OnFlareSignalContainerGettingInsertedAttempt(
    Entity<FlareSignalComponent> ent,
    ref ContainerGettingInsertedAttemptEvent args)
  {
    if (args.Cancelled || !this.IsFlareLit((EntityUid) ent))
      return;
    args.Cancel();
  }

  private void OnFlareSignalGrenadeContentThrown(
    Entity<FlareSignalComponent> ent,
    ref GrenadeContentThrownEvent args)
  {
    ProjectileComponent comp1;
    if (!this.TryComp<ProjectileComponent>(args.Source, out comp1))
      return;
    int nextId = this.ComputeNextId();
    string userAbbreviation = this.Loc.GetString("rmc-laser-designator-target-abbreviation", ("id", (object) nextId));
    if (comp1.Shooter.HasValue)
      userAbbreviation = this.GetUserAbbreviation(comp1.Shooter.Value, nextId);
    RMCAirShotComponent comp2;
    if (comp1.Weapon.HasValue && this.TryComp<RMCAirShotComponent>(comp1.Weapon, out comp2))
    {
      comp2.LastFlareId = userAbbreviation;
      this.Dirty(comp1.Weapon.Value, (IComponent) comp2);
    }
    this.MakeDropshipTarget((EntityUid) ent, userAbbreviation);
    this._physics.SetBodyType((EntityUid) ent, BodyType.Static);
  }

  private void OnActiveFlareExamined(Entity<DropshipTargetComponent> ent, ref ExaminedEvent args)
  {
    string abbreviation = ent.Comp.Abbreviation;
    if (abbreviation == null)
      return;
    args.PushMarkup(this.Loc.GetString("rmc-laser-designator-signal-flare-examine-id", ("id", (object) abbreviation)));
  }

  private void OnTerminalMapInit(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref MapInitEvent args)
  {
    List<DropshipTerminalWeaponsComponent.TargetEnt> targetEntList = new List<DropshipTerminalWeaponsComponent.TargetEnt>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipTargetComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DropshipTargetComponent>();
    EntityUid uid;
    DropshipTargetComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      targetEntList.Add(new DropshipTerminalWeaponsComponent.TargetEnt(this.GetNetEntity(uid), comp1.Abbreviation));
    targetEntList.Sort((Comparison<DropshipTerminalWeaponsComponent.TargetEnt>) ((a, b) => string.CompareOrdinal(a.Name, b.Name)));
    ent.Comp.Targets = targetEntList;
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
  }

  private void OnTerminalBUIOpened(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref BoundUIOpenedEvent args)
  {
    this.AddPvs(ent, (Entity<ActorComponent>) args.Actor);
  }

  private void OnTerminalBUIClosed(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref BoundUIClosedEvent args)
  {
    this.RemovePvs(ent, (Entity<ActorComponent>) args.Actor);
  }

  private void OnDropshipTargetMapInit(Entity<DropshipTargetComponent> ent, ref MapInitEvent args)
  {
    NetEntity netEntity = this.GetNetEntity((EntityUid) ent);
    Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipTerminalWeaponsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DropshipTerminalWeaponsComponent>();
    EntityUid uid;
    DropshipTerminalWeaponsComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      List<DropshipTerminalWeaponsComponent.TargetEnt> targetEntList = comp1.Targets;
      if (this.HasComp<MedevacStretcherComponent>((EntityUid) ent))
        targetEntList = comp1.Medevacs;
      else if (this.HasComp<RMCActiveFultonComponent>((EntityUid) ent))
        targetEntList = comp1.Fultons;
      targetEntList.Add(new DropshipTerminalWeaponsComponent.TargetEnt(netEntity, ent.Comp.Abbreviation));
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  private void OnDropshipTargetRemove<T>(Entity<DropshipTargetComponent> ent, ref T args)
  {
    NetEntity netEntity = this.GetNetEntity((EntityUid) ent);
    Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipTerminalWeaponsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DropshipTerminalWeaponsComponent>();
    EntityUid uid;
    DropshipTerminalWeaponsComponent comp1;
    EntityUid key2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? nullable = comp1.Target;
      key2 = (EntityUid) ent;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() == key2 ? 1 : 0) : 0) != 0)
      {
        this.RemovePvsActors((Entity<DropshipTerminalWeaponsComponent>) (uid, comp1));
        Entity<DropshipTerminalWeaponsComponent> dropshipWeaponsTerminal = (Entity<DropshipTerminalWeaponsComponent>) (uid, comp1);
        nullable = new EntityUid?();
        EntityUid? newTarget = nullable;
        this.SetTarget(dropshipWeaponsTerminal, newTarget);
      }
      List<DropshipTerminalWeaponsComponent.TargetEnt> list = comp1.Targets;
      if (this.HasComp<MedevacStretcherComponent>((EntityUid) ent))
        list = comp1.Medevacs;
      else if (this.HasComp<RMCActiveFultonComponent>((EntityUid) ent))
        list = comp1.Fultons;
      Span<DropshipTerminalWeaponsComponent.TargetEnt> span = CollectionsMarshal.AsSpan<DropshipTerminalWeaponsComponent.TargetEnt>(list);
      for (int index = 0; index < span.Length; ++index)
      {
        if (!(span[index].Id != netEntity))
        {
          list.RemoveAt(index);
          break;
        }
      }
      this.Dirty(uid, (IComponent) comp1);
    }
    if (this._net.IsClient)
      return;
    EntityUid entityUid2;
    foreach ((key2, entityUid2) in ent.Comp.Eyes)
      this.QueueDel(new EntityUid?(entityUid2));
  }

  private void OnDropshipTargetEyeRemove<T>(Entity<DropshipTargetEyeComponent> ent, ref T args)
  {
    DropshipTargetComponent comp;
    if (this.TerminatingOrDeleted(ent.Comp.Target) || !this.TryComp<DropshipTargetComponent>(ent.Comp.Target, out comp))
      return;
    this._targetsToRemove.Clear();
    foreach ((EntityUid key, EntityUid entityUid) in comp.Eyes)
    {
      if (entityUid == ent.Owner)
        this._targetsToRemove.Add(key);
    }
    foreach (EntityUid key in this._targetsToRemove)
      comp.Eyes.Remove(key);
  }

  private void OnAmmoExamined(Entity<DropshipAmmoComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("DropshipAmmoComponent"))
      args.PushText(this.Loc.GetString("rmc-dropship-ammo-examine", ("rounds", (object) ent.Comp.Rounds)));
  }

  private void OnAmmoInteract(Entity<DropshipAmmoComponent> ent, ref PowerLoaderInteractEvent args)
  {
    if (args.Handled)
      return;
    DropshipWeaponPointComponent comp1;
    BaseContainer container;
    EntityUid? element;
    if (this.TryComp<DropshipWeaponPointComponent>(args.Target, out comp1) && this._container.TryGetContainer(args.Target, comp1.WeaponContainerSlotId, out container) && container.ContainedEntities.TryFirstOrNull<EntityUid>(out element) && ent.Comp.Weapon.Id != this.Prototype(element.Value)?.ID)
    {
      args.Handled = true;
      foreach (EntityUid entityUid in args.Buckled)
        this._popup.PopupClient(this.Loc.GetString("rmc-power-loader-wrong-weapon"), args.Target, new EntityUid?(entityUid), PopupType.SmallCaution);
    }
    else
    {
      DropshipAmmoComponent comp2;
      if (!this.TryComp<DropshipAmmoComponent>(args.Target, out comp2))
        return;
      args.Handled = true;
      if (ent.Comp.AmmoType != comp2.AmmoType)
      {
        foreach (EntityUid entityUid in args.Buckled)
          this._popup.PopupClient(this.Loc.GetString("rmc-power-loader-wrong-ammo"), args.Target, new EntityUid?(entityUid), PopupType.SmallCaution);
      }
      else if (comp2.Rounds == comp2.MaxRounds)
      {
        foreach (EntityUid entityUid in args.Buckled)
          this._popup.PopupClient(this.Loc.GetString("rmc-power-loader-full-ammo", ("ammo", (object) args.Target)), args.Target, new EntityUid?(entityUid), PopupType.SmallCaution);
      }
      else
      {
        int num = Math.Min(ent.Comp.Rounds, comp2.MaxRounds - comp2.Rounds);
        ent.Comp.Rounds -= num;
        comp2.Rounds += num;
        this._appearance.SetData((EntityUid) ent, (Enum) DropshipAmmoVisuals.Fill, (object) ent.Comp.Rounds);
        this._appearance.SetData(args.Target, (Enum) DropshipAmmoVisuals.Fill, (object) comp2.Rounds);
        this.Dirty<DropshipAmmoComponent>(ent);
        this.Dirty(args.Target, (IComponent) comp2);
        if (ent.Comp.Rounds <= 0)
        {
          if (this._net.IsServer)
            this.QueueDel(new EntityUid?(args.Used));
          this._container.TryRemoveFromContainer((Entity<TransformComponent, MetaDataComponent>) args.Used, true);
          this._powerloader.TrySyncHands((Entity<PowerLoaderComponent>) args.PowerLoader);
        }
        foreach (EntityUid entityUid in args.Buckled)
          this._popup.PopupClient(this.Loc.GetString("rmc-power-loader-transfer-ammo", ("rounds", (object) num), ("ammo", (object) args.Target)), args.Target, new EntityUid?(entityUid));
      }
    }
  }

  private void OnWeaponsChangeScreenMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsChangeScreenMsg args)
  {
    if (!Enum.IsDefined<DropshipTerminalWeaponsScreen>(args.Screen))
      return;
    ref DropshipTerminalWeaponsComponent.Screen local = ref (args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo);
    local.State = args.Screen;
    if (args.Screen == DropshipTerminalWeaponsScreen.StrikeWeapon)
      local.Weapon = new NetEntity?();
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsChooseWeaponMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsChooseWeaponMsg args)
  {
    EntityUid? entity;
    if (!this.TryGetEntity(args.Weapon, out entity) || !this._dropship.IsWeaponAttached((Entity<DropshipWeaponComponent>) entity.Value))
    {
      if (!this.HasComp<RMCEquipmentDeployerComponent>(entity))
        return;
      EntityUid parentUid = this.Transform(this.GetEntity(args.Weapon)).ParentUid;
      this.SetScreenUtility<RMCEquipmentDeployerComponent>(ent, args.First, DropshipTerminalWeaponsScreen.EquipmentDeployer, new NetEntity?(this.GetNetEntity(parentUid)));
    }
    else
    {
      ref DropshipTerminalWeaponsComponent.Screen local = ref (args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo);
      local.Weapon = new NetEntity?(args.Weapon);
      if (local.State == DropshipTerminalWeaponsScreen.Equip)
        local.State = DropshipTerminalWeaponsScreen.SelectingWeapon;
      else if (local.State == DropshipTerminalWeaponsScreen.StrikeWeapon)
        local.State = DropshipTerminalWeaponsScreen.Target;
      this.Dirty<DropshipTerminalWeaponsComponent>(ent);
      this.RefreshWeaponsUI(ent);
    }
  }

  private void OnWeaponsChooseMedevacMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsChooseMedevacMsg args)
  {
    this.SetScreenUtility<MedevacComponent>(ent, args.First, DropshipTerminalWeaponsScreen.Medevac);
  }

  private void OnWeaponsChooseFultonMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsChooseFultonMsg args)
  {
    this.SetScreenUtility<RMCFultonComponent>(ent, args.First, DropshipTerminalWeaponsScreen.Fulton);
  }

  private void OnWeaponsChooseParaDropMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsChooseParaDropMsg args)
  {
    this.SetScreenUtility<RMCParaDropComponent>(ent, args.First, DropshipTerminalWeaponsScreen.Paradrop);
  }

  private void OnWeaponsChooseSpotlightMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsChooseSpotlightMsg args)
  {
    this.SetScreenUtility<DropshipSpotlightComponent>(ent, args.First, DropshipTerminalWeaponsScreen.Spotlight, new NetEntity?(args.Slot));
  }

  private void OnWeaponsChooseEquipmentDeployerMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsChooseEquipmentDeployerMsg args)
  {
    this.SetScreenUtility<RMCEquipmentDeployerComponent>(ent, args.First, DropshipTerminalWeaponsScreen.EquipmentDeployer, new NetEntity?(args.Slot));
  }

  private void OnWeaponsFireMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsFireMsg args)
  {
    if (this._net.IsClient)
      return;
    EntityUid actor = args.Actor;
    ref DropshipTerminalWeaponsComponent.Screen local1 = ref (args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo);
    NetEntity? nullable1 = local1.Weapon;
    if (nullable1.HasValue)
    {
      EntityUid? entity;
      DropshipWeaponComponent comp1;
      if (!this.TryGetEntity(nullable1.GetValueOrDefault(), out entity) || !this.TryComp<DropshipWeaponComponent>(entity, out comp1))
      {
        ref DropshipTerminalWeaponsComponent.Screen local2 = ref local1;
        nullable1 = new NetEntity?();
        NetEntity? nullable2 = nullable1;
        local2.Weapon = nullable2;
        this.Dirty<DropshipTerminalWeaponsComponent>(ent);
      }
      else
      {
        Entity<DropshipComponent> dropship = new Entity<DropshipComponent>();
        if (!this.CasDebug)
        {
          if (!this._dropship.TryGetGridDropship(entity.Value, out dropship))
            return;
          FTLComponent comp2;
          if (!this.TryComp<FTLComponent>((EntityUid) dropship, out comp2) || comp2.State != FTLState.Travelling && comp2.State != FTLState.Arriving)
          {
            this._popup.PopupCursor(this.Loc.GetString("rmc-dropship-weapons-fire-not-flying"), actor, PopupType.SmallCaution);
            return;
          }
        }
        EntityUid? nullable3 = ent.Comp.Target;
        if (!nullable3.HasValue)
          return;
        EntityUid valueOrDefault = nullable3.GetValueOrDefault();
        if (!this.IsValidTarget((Entity<DropshipTargetComponent>) valueOrDefault))
        {
          this.RemovePvsActors(ent);
          Entity<DropshipTerminalWeaponsComponent> dropshipWeaponsTerminal = ent;
          nullable3 = new EntityUid?();
          EntityUid? newTarget = nullable3;
          this.SetTarget(dropshipWeaponsTerminal, newTarget);
          this.Dirty<DropshipTerminalWeaponsComponent>(ent);
        }
        else
        {
          EntityCoordinates grid = this._transform.GetMoverCoordinates(valueOrDefault).SnapToGrid((IEntityManager) this.EntityManager, this._mapManager);
          if (!this.CasDebug && !this._area.CanCAS(grid))
            this._popup.PopupCursor(this.Loc.GetString("rmc-laser-designator-not-cas"), actor);
          else if (!this.CasDebug && comp1.Skills != null && !this._skills.HasSkills((Entity<SkillsComponent>) actor, comp1.Skills))
          {
            this._popup.PopupCursor(this.Loc.GetString("rmc-laser-designator-not-skilled"), actor);
          }
          else
          {
            if (!this.CasDebug && !comp1.FireInTransport && !this.HasComp<DropshipInFlyByComponent>((EntityUid) dropship))
              return;
            TimeSpan curTime = this._timing.CurTime;
            TimeSpan timeSpan = curTime;
            TimeSpan? nextFireAt = comp1.NextFireAt;
            if ((nextFireAt.HasValue ? (timeSpan < nextFireAt.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            {
              this._popup.PopupCursor(this.Loc.GetString("rmc-dropship-weapons-fire-cooldown", ("weapon", (object) entity)), actor);
            }
            else
            {
              Entity<DropshipAmmoComponent> ammo;
              if (!this.TryGetWeaponAmmo((Entity<DropshipWeaponComponent>) (entity.Value, comp1), out ammo) || ammo.Comp.Rounds < ammo.Comp.RoundsPerShot)
              {
                this._popup.PopupCursor(this.Loc.GetString("rmc-dropship-weapons-fire-no-ammo", ("weapon", (object) entity)), actor);
              }
              else
              {
                if (ammo.Comp.Rounds < ammo.Comp.RoundsPerShot)
                  return;
                DropshipWeaponShotEvent args1 = new DropshipWeaponShotEvent((float) ammo.Comp.TargetSpread, ammo.Comp.BulletSpread, ammo.Comp.TravelTime, ammo.Comp.RoundsPerShot, ammo.Comp.ShotsPerVolley, ammo.Comp.Damage, ammo.Comp.ArmorPiercing, ammo.Comp.SoundTravelTime, ammo.Comp.SoundCockpit, ammo.Comp.SoundMarker, ammo.Comp.SoundGround, ammo.Comp.SoundImpact, ammo.Comp.SoundWarning, ammo.Comp.MarkerWarning, ammo.Comp.ImpactEffects, ammo.Comp.Explosion, ammo.Comp.Implosion, ammo.Comp.Fire, ammo.Comp.SoundEveryShots);
                this.RaiseLocalEvent<DropshipWeaponShotEvent>((EntityUid) dropship, ref args1);
                ammo.Comp.Rounds -= ammo.Comp.RoundsPerShot;
                this._appearance.SetData((EntityUid) ammo, (Enum) DropshipAmmoVisuals.Fill, (object) ammo.Comp.Rounds);
                this._powerloader.SyncAppearance((Entity<DropshipWeaponPointComponent>) this.Transform(entity.Value).ParentUid);
                this.Dirty<DropshipAmmoComponent>(ammo);
                this._audio.PlayPvs(args1.SoundCockpit, entity.Value);
                comp1.NextFireAt = new TimeSpan?(curTime + comp1.FireDelay);
                this.Dirty(entity.Value, (IComponent) comp1);
                float spread = args1.Spread;
                EntityCoordinates entityCoordinates = grid;
                if ((double) spread != 0.0)
                  entityCoordinates = entityCoordinates.Offset(this._random.NextVector2(-spread, spread + 1f));
                this.AddComp<AmmoInFlightComponent>(this.Spawn((string) null, MapCoordinates.Nullspace, rotation: new Angle()), new AmmoInFlightComponent()
                {
                  Target = entityCoordinates,
                  MarkerAt = curTime + args1.TravelTime,
                  ShotsLeft = args1.RoundsPerShot,
                  ShotsPerVolley = args1.ShotsPerVolley,
                  Damage = args1.Damage,
                  ArmorPiercing = args1.ArmorPiercing,
                  BulletSpread = args1.BulletSpread,
                  SoundTravelTime = args1.SoundTravelTime,
                  SoundMarker = args1.SoundMarker,
                  SoundGround = args1.SoundGround,
                  SoundImpact = args1.SoundImpact,
                  SoundWarning = args1.SoundWarning,
                  MarkerWarning = args1.MarkerWarning,
                  WarningMarkerAt = curTime + TimeSpan.FromSeconds(1L),
                  ImpactEffects = args1.ImpactEffect,
                  Explosion = args1.Explosion,
                  Implosion = args1.Implosion,
                  Fire = args1.Fire,
                  SoundEveryShots = args1.SoundEveryShots
                }, true);
                if (ammo.Comp.DeleteOnEmpty && ammo.Comp.Rounds <= 0)
                  this.QueueDel(new EntityUid?((EntityUid) ammo));
                ISharedAdminLogManager adminLog = this._adminLog;
                LogStringHandler logStringHandler = new LogStringHandler(11, 3);
                logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "ToPrettyString(args.Actor)");
                logStringHandler.AppendLiteral(" fired ");
                logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(entity), "ToPrettyString(weapon)");
                logStringHandler.AppendLiteral(" at ");
                logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) valueOrDefault), "ToPrettyString(target)");
                ref LogStringHandler local3 = ref logStringHandler;
                adminLog.Add(LogType.RMCDropshipWeapon, ref local3);
              }
            }
          }
        }
      }
    }
    else
      this._popup.PopupCursor(this.Loc.GetString("rmc-dropship-weapons-fire-no-weapon"), actor, PopupType.SmallCaution);
  }

  private void OnDropshipWeaponOnSpawnFire(
    Entity<ActivateDropshipWeaponOnSpawnComponent> active,
    ref MapInitEvent args)
  {
    DropshipAmmoComponent comp;
    if (this._net.IsClient || !this.TryComp<DropshipAmmoComponent>((EntityUid) active, out comp))
      return;
    TimeSpan curTime = this._timing.CurTime;
    this.AddComp<AmmoInFlightComponent>(this.Spawn((string) null, MapCoordinates.Nullspace, rotation: new Angle()), new AmmoInFlightComponent()
    {
      Target = this._transform.GetMoverCoordinates((EntityUid) active).SnapToGrid((IEntityManager) this.EntityManager, this._mapManager),
      MarkerAt = curTime + comp.TravelTime,
      ShotsLeft = comp.RoundsPerShot,
      ShotsPerVolley = comp.ShotsPerVolley,
      Damage = comp.Damage,
      ArmorPiercing = comp.ArmorPiercing,
      BulletSpread = comp.BulletSpread,
      SoundTravelTime = comp.SoundTravelTime,
      SoundMarker = comp.SoundMarker,
      SoundGround = comp.SoundGround,
      SoundImpact = comp.SoundImpact,
      SoundWarning = comp.SoundWarning,
      MarkerWarning = comp.MarkerWarning,
      WarningMarkerAt = curTime + TimeSpan.FromSeconds(1L),
      ImpactEffects = comp.ImpactEffects,
      Explosion = comp.Explosion,
      Implosion = comp.Implosion,
      Fire = comp.Fire,
      SoundEveryShots = comp.SoundEveryShots
    }, true);
    this.QueueDel(new EntityUid?((EntityUid) active));
  }

  private void OnWeaponsNightVisionMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsNightVisionMsg args)
  {
    if (this._net.IsClient)
      return;
    ent.Comp.NightVision = args.On;
    Entity<DropshipTerminalWeaponsComponent> terminal = ent;
    EntityUid? target = ent.Comp.Target;
    Entity<DropshipTargetComponent>? targetNullable = target.HasValue ? new Entity<DropshipTargetComponent>?((Entity<DropshipTargetComponent>) target.GetValueOrDefault()) : new Entity<DropshipTargetComponent>?();
    EntityUid? nullable = this.EnsureTargetEye(terminal, targetNullable);
    if (!nullable.HasValue)
      return;
    this._eye.SetDrawLight((Entity<EyeComponent>) nullable.GetValueOrDefault(), !ent.Comp.NightVision);
  }

  private void OnWeaponsExitMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsExitMsg args)
  {
    // ISSUE: explicit reference operation
    (^ref (args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo)).State = DropshipTerminalWeaponsScreen.Main;
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsCancelMsg(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsCancelMsg args)
  {
    ref DropshipTerminalWeaponsComponent.Screen local = ref (args.First ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo);
    DropshipTerminalWeaponsScreen terminalWeaponsScreen;
    switch (local.State)
    {
      case DropshipTerminalWeaponsScreen.Strike:
      case DropshipTerminalWeaponsScreen.StrikeWeapon:
        terminalWeaponsScreen = DropshipTerminalWeaponsScreen.Target;
        break;
      default:
        terminalWeaponsScreen = local.State;
        break;
    }
    local.State = terminalWeaponsScreen;
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsAdjustOffset(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsAdjustOffsetMsg args)
  {
    if (!args.Direction.IsCardinal())
      return;
    Vector2i intVec = DirectionExtensions.ToIntVec(args.Direction);
    Vector2i vector2i = Vector2i.op_Addition(ent.Comp.Offset, intVec);
    Vector2i offsetLimit = ent.Comp.OffsetLimit;
    // ISSUE: explicit constructor call
    ((Vector2i) ref vector2i).\u002Ector(Math.Clamp(vector2i.X, -offsetLimit.X, offsetLimit.X), Math.Clamp(vector2i.Y, -offsetLimit.Y, offsetLimit.Y));
    ent.Comp.Offset = vector2i;
    Entity<DropshipTerminalWeaponsComponent> terminal = ent;
    EntityUid? target = ent.Comp.Target;
    Entity<DropshipTargetComponent>? targetNullable = target.HasValue ? new Entity<DropshipTargetComponent>?((Entity<DropshipTargetComponent>) target.GetValueOrDefault()) : new Entity<DropshipTargetComponent>?();
    EntityUid? nullable = this.EnsureTargetEye(terminal, targetNullable);
    if (nullable.HasValue)
      this._eye.SetOffset(nullable.GetValueOrDefault(), Vector2i.op_Implicit(ent.Comp.Offset));
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsResetOffset(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsResetOffsetMsg args)
  {
    ent.Comp.Offset = Vector2i.Zero;
    Entity<DropshipTerminalWeaponsComponent> terminal = ent;
    EntityUid? target = ent.Comp.Target;
    Entity<DropshipTargetComponent>? targetNullable = target.HasValue ? new Entity<DropshipTargetComponent>?((Entity<DropshipTargetComponent>) target.GetValueOrDefault()) : new Entity<DropshipTargetComponent>?();
    EntityUid? nullable = this.EnsureTargetEye(terminal, targetNullable);
    if (nullable.HasValue)
      this._eye.SetOffset(nullable.GetValueOrDefault(), Vector2i.op_Implicit(ent.Comp.Offset));
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsTargetsPrevious(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsTargetsPreviousMsg args)
  {
    ent.Comp.TargetsPage = Math.Max(0, ent.Comp.TargetsPage - 1);
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsTargetsNext(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsTargetsNextMsg args)
  {
    ent.Comp.TargetsPage = Math.Min(ent.Comp.Targets.Count / 5, ent.Comp.TargetsPage + 1);
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsTargetsSelect(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsTargetsSelectMsg args)
  {
    EntityUid? entity;
    if (!this.TryGetEntity(args.Target, out entity) || !this.HasComp<DropshipTargetComponent>(entity))
      this.RefreshWeaponsUI(ent);
    else
      this.UpdateTarget(ent, entity.Value);
  }

  private void OnWeaponsMedevacPrevious(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsMedevacPreviousMsg args)
  {
    ent.Comp.MedevacsPage = Math.Max(0, ent.Comp.MedevacsPage - 1);
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsMedevacNext(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsMedevacNextMsg args)
  {
    ent.Comp.MedevacsPage = Math.Min(ent.Comp.Medevacs.Count % 5, ent.Comp.MedevacsPage + 1);
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsMedevacSelect(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsMedevacSelectMsg args)
  {
    EntityUid? entity;
    if (!this.TryGetEntity(args.Target, out entity) || !this.HasComp<MedevacStretcherComponent>(entity))
    {
      this.RefreshWeaponsUI(ent);
    }
    else
    {
      this.UpdateTarget(ent, entity.Value);
      this._popup.PopupClient("You move your dropship above the selected stretcher's beacon. You can now manually activate the medevac system to hoist the patient up.", new EntityUid?(args.Actor));
    }
  }

  private void OnWeaponsFultonPrevious(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsFultonPreviousMsg args)
  {
    ent.Comp.FultonsPage = Math.Max(0, ent.Comp.FultonsPage - 1);
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsFultonNext(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsFultonNextMsg args)
  {
    ent.Comp.FultonsPage = Math.Min(ent.Comp.Fultons.Count % 5, ent.Comp.FultonsPage + 1);
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnWeaponsFultonSelect(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropshipTerminalWeaponsFultonSelectMsg args)
  {
    if (this._net.IsClient)
      return;
    EntityUid? entity;
    if (!this.TryGetEntity(args.Target, out entity) || !this.HasComp<RMCActiveFultonComponent>(entity))
    {
      this.RefreshWeaponsUI(ent);
    }
    else
    {
      Entity<DropshipComponent> dropship;
      if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship) || dropship.Comp.AttachmentPoints.Count == 0)
        return;
      foreach (EntityUid attachmentPoint in dropship.Comp.AttachmentPoints)
      {
        DropshipUtilityPointComponent comp1;
        BaseContainer container;
        if (this.TryComp<DropshipUtilityPointComponent>(attachmentPoint, out comp1) && this._container.TryGetContainer(attachmentPoint, comp1.UtilitySlotId, out container) && container.ContainedEntities.Count != 0)
        {
          EntityUid containedEntity = container.ContainedEntities[0];
          if (this.HasComp<RMCFultonComponent>(containedEntity))
          {
            DropshipUtilityComponent comp2;
            string popup;
            if (this.TryComp<DropshipUtilityComponent>(containedEntity, out comp2) && !this._dropshipUtility.IsActivatable((Entity<DropshipUtilityComponent>) (containedEntity, comp2), args.Actor, out popup))
            {
              this._popup.PopupCursor(popup, args.Actor);
            }
            else
            {
              this.RemComp<DropshipTargetComponent>(entity.Value);
              this.RemCompDeferred<RMCActiveFultonComponent>(entity.Value);
              this._transform.PlaceNextTo((Entity<TransformComponent>) entity.Value, (Entity<TransformComponent>) attachmentPoint);
              this.RefreshWeaponsUI(ent);
              return;
            }
          }
        }
      }
      this.RefreshWeaponsUI(ent);
    }
  }

  private void OnWeaponsParaDropSelect(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropShipTerminalWeaponsParaDropTargetSelectMsg args)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    if (!ent.Comp.Target.HasValue)
    {
      if (this._net.IsClient)
        this._popup.PopupCursor(this.Loc.GetString("rmc-dropship-paradrop-lock-no-target"), args.Actor, PopupType.SmallCaution);
      this.RefreshWeaponsUI(ent);
    }
    else
    {
      Entity<DropshipComponent> dropship;
      if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship) || dropship.Comp.AttachmentPoints.Count == 0)
        return;
      FTLComponent comp1;
      if (!this.CasDebug && (!this.TryComp<FTLComponent>((EntityUid) dropship, out comp1) || comp1.State != FTLState.Travelling))
      {
        if (!this._net.IsClient)
          return;
        this._popup.PopupCursor(this.Loc.GetString("rmc-dropship-paradrop-lock-target-not-flying"), args.Actor, PopupType.SmallCaution);
      }
      else if (!this.IsValidTarget((Entity<DropshipTargetComponent>) ent.Comp.Target.Value))
      {
        this.RemovePvsActors(ent);
        this.SetTarget(ent, new EntityUid?());
        this.Dirty<DropshipTerminalWeaponsComponent>(ent);
      }
      else
      {
        EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(ent.Comp.Target.Value);
        if (!this.CasDebug && !this._area.CanCAS(moverCoordinates))
        {
          if (!this._net.IsClient)
            return;
          this._popup.PopupCursor(this.Loc.GetString("rmc-laser-designator-not-cas"), args.Actor);
        }
        else
        {
          if (args.On)
          {
            TransformChildrenEnumerator childEnumerator = this.Transform((EntityUid) dropship).ChildEnumerator;
            EntityUid child;
            while (childEnumerator.MoveNext(out child))
            {
              DoorComponent comp2;
              DoorBoltComponent comp3;
              if (this.TryComp<DoorComponent>(child, out comp2) && comp2.Location == DoorLocation.Aft && this.TryComp<DoorBoltComponent>(child, out comp3) && comp2.State != DoorState.Open)
              {
                this._door.StartOpening(child);
                this._door.TrySetBoltDown((Entity<DoorBoltComponent>) (child, comp3), true);
              }
            }
            ActiveParaDropComponent paraDropComponent = this.EnsureComp<ActiveParaDropComponent>((EntityUid) dropship);
            paraDropComponent.DropTarget = ent.Comp.Target;
            this.Dirty((EntityUid) dropship, (IComponent) paraDropComponent);
          }
          else
            this.RemComp<ActiveParaDropComponent>((EntityUid) dropship);
          this.RefreshWeaponsUI(ent);
        }
      }
    }
  }

  private void OnWeaponsSpotlightSelect(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropShipTerminalWeaponsSpotlightToggleMsg args)
  {
    EntityUid? entity = this.GetEntity(ent.Comp.SelectedSystem);
    Entity<DropshipComponent> dropship;
    DropshipSpotlightComponent comp;
    if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship) || dropship.Comp.AttachmentPoints.Count == 0 || !this.TryComp<DropshipSpotlightComponent>(entity, out comp))
      return;
    EntityUid parentUid = this.Transform(entity.Value).ParentUid;
    this._pointLight.SetEnabled(parentUid, args.On);
    this._appearance.SetData(parentUid, (Enum) DropshipUtilityVisuals.State, args.On ? (object) "spotlights_on" : (object) "spotlights_off");
    comp.Enabled = args.On;
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }

  private void OnEquipmentDeploy(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropShipTerminalWeaponsEquipmentDeployToggleMsg args)
  {
    EntityUid? entity = this.GetEntity(ent.Comp.SelectedSystem);
    Entity<DropshipComponent> dropship;
    BaseContainer container;
    if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship) || dropship.Comp.AttachmentPoints.Count == 0 || !entity.HasValue || !this._equipmentDeployer.TryGetContainer(entity.Value, out container))
      return;
    Vector2 deployOffset = new Vector2();
    float rotationOffset = 0.0f;
    DropshipWeaponPointComponent comp;
    if (this.TryComp<DropshipWeaponPointComponent>(entity, out comp))
      this._equipmentDeployer.TryGetOffset(container.ContainedEntities[0], out deployOffset, out rotationOffset, comp.Location);
    this._equipmentDeployer.TryDeploy(container.ContainedEntities[0], args.Deploy, deployOffset, rotationOffset, user: new EntityUid?(args.Actor));
    this.RefreshWeaponsUI(ent);
  }

  private void OnEquipmentToggleAutoDeploy(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref DropShipTerminalWeaponsEquipmentAutoDeployToggleMsg args)
  {
    EntityUid? entity = this.GetEntity(ent.Comp.SelectedSystem);
    Entity<DropshipComponent> dropship;
    BaseContainer container;
    if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship) || dropship.Comp.AttachmentPoints.Count == 0 || !entity.HasValue || !this._equipmentDeployer.TryGetContainer(entity.Value, out container))
      return;
    this._equipmentDeployer.SetAutoDeploy(container.ContainedEntities[0], args.AutoDeploy);
    this.RefreshWeaponsUI(ent);
  }

  private void OnRefreshNameModifier(
    Entity<ActiveFlareSignalComponent> ent,
    ref RefreshNameModifiersEvent args)
  {
    if (ent.Comp.Abbreviation == null)
      return;
    args.AddModifier((LocId) ent.Comp.Abbreviation);
  }

  private void UpdateTarget(Entity<DropshipTerminalWeaponsComponent> ent, EntityUid target)
  {
    this.RemovePvsActors(ent);
    this.SetTarget(ent, new EntityUid?(target));
    Entity<DropshipTerminalWeaponsComponent> terminal = ent;
    EntityUid? target1 = ent.Comp.Target;
    Entity<DropshipTargetComponent>? targetNullable = target1.HasValue ? new Entity<DropshipTargetComponent>?((Entity<DropshipTargetComponent>) target1.GetValueOrDefault()) : new Entity<DropshipTargetComponent>?();
    EntityUid? nullable = this.EnsureTargetEye(terminal, targetNullable);
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      this._eye.SetOffset(valueOrDefault, Vector2i.op_Implicit(ent.Comp.Offset));
      this._eye.SetDrawLight((Entity<EyeComponent>) valueOrDefault, !ent.Comp.NightVision);
    }
    this.AddPvsActors(ent);
    this.RefreshWeaponsUI(ent);
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
  }

  protected virtual void RefreshWeaponsUI(Entity<DropshipTerminalWeaponsComponent> terminal)
  {
  }

  public bool TryGetWeaponAmmo(
    Entity<DropshipWeaponComponent?> weapon,
    out Entity<DropshipAmmoComponent> ammo)
  {
    ammo = new Entity<DropshipAmmoComponent>();
    BaseContainer container1;
    DropshipWeaponPointComponent comp1;
    BaseContainer container2;
    if (!this.Resolve<DropshipWeaponComponent>((EntityUid) weapon, ref weapon.Comp, false) || !this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) weapon, (TransformComponent) null), out container1) || !this.TryComp<DropshipWeaponPointComponent>(container1.Owner, out comp1) || !this._container.TryGetContainer(container1.Owner, comp1.AmmoContainerSlotId, out container2))
      return false;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container2.ContainedEntities)
    {
      DropshipAmmoComponent comp2;
      if (this.TryComp<DropshipAmmoComponent>(containedEntity, out comp2))
      {
        ammo = (Entity<DropshipAmmoComponent>) (containedEntity, comp2);
        return true;
      }
    }
    return false;
  }

  public int GetWeaponRounds(Entity<DropshipWeaponComponent?> weapon)
  {
    Entity<DropshipAmmoComponent> ammo;
    return this.TryGetWeaponAmmo(weapon, out ammo) ? ammo.Comp.Rounds : 0;
  }

  private bool IsValidTarget(Entity<DropshipTargetComponent?> ent)
  {
    if (!this.Resolve<DropshipTargetComponent>((EntityUid) ent, ref ent.Comp, false))
      return false;
    TransformComponent transformComponent = this.Transform((EntityUid) ent);
    return (this.CasDebug || this.HasComp<RMCPlanetComponent>(transformComponent.GridUid)) && ent.Comp.IsTargetableByWeapons;
  }

  public string GetUserAbbreviation(EntityUid user, int id)
  {
    string userAbbreviation = this.Loc.GetString("rmc-laser-designator-target-abbreviation", (nameof (id), (object) id));
    Entity<SquadTeamComponent> squad;
    if (this._squad.TryGetMemberSquad((Entity<SquadMemberComponent>) user, out squad))
    {
      string str = this.Name((EntityUid) squad);
      if (!string.IsNullOrWhiteSpace(str) && str.Length > 0)
        str = $"{str[0]}";
      userAbbreviation = this.Loc.GetString("rmc-laser-designator-target-abbreviation-squad", ("letter", (object) str), (nameof (id), (object) id));
    }
    return userAbbreviation;
  }

  protected virtual void AddPvs(
    Entity<DropshipTerminalWeaponsComponent> terminal,
    Entity<ActorComponent?> actor)
  {
  }

  protected virtual void RemovePvs(
    Entity<DropshipTerminalWeaponsComponent> terminal,
    Entity<ActorComponent?> actor)
  {
  }

  private void AddPvsActors(Entity<DropshipTerminalWeaponsComponent> terminal)
  {
    foreach (EntityUid actor in this._ui.GetActors((Entity<UserInterfaceComponent>) terminal.Owner, (Enum) DropshipTerminalWeaponsUi.Key))
      this.AddPvs(terminal, (Entity<ActorComponent>) actor);
  }

  private void RemovePvsActors(Entity<DropshipTerminalWeaponsComponent> terminal)
  {
    foreach (EntityUid actor in this._ui.GetActors((Entity<UserInterfaceComponent>) terminal.Owner, (Enum) DropshipTerminalWeaponsUi.Key))
      this.RemovePvs(terminal, (Entity<ActorComponent>) actor);
  }

  private void StartTrackingActiveFlare(Entity<FlareSignalComponent> ent, EntityUid? user)
  {
    ActiveFlareSignalComponent comp;
    if (this.EnsureComp<ActiveFlareSignalComponent>((EntityUid) ent, out comp) || this._net.IsClient)
      return;
    int nextId = this.ComputeNextId();
    comp.Abbreviation = this.Loc.GetString("rmc-laser-designator-target-abbreviation", ("id", (object) nextId));
    if (!user.HasValue)
      return;
    comp.Abbreviation = this.GetUserAbbreviation(user.Value, nextId);
  }

  private bool IsFlareLit(EntityUid flare)
  {
    ExpendableLightComponent comp;
    return this.TryComp<ExpendableLightComponent>(flare, out comp) && comp.Activated;
  }

  private bool TryActivateSignalFlareTarget(Entity<ActiveFlareSignalComponent> ent)
  {
    if (this.HasComp<DropshipTargetComponent>((EntityUid) ent))
      return true;
    if (!this.IsFlareLit((EntityUid) ent) || ent.Comp.Abbreviation == null)
      return false;
    DropshipTargetComponent component = new DropshipTargetComponent()
    {
      Abbreviation = ent.Comp.Abbreviation
    };
    this.AddComp<DropshipTargetComponent>((EntityUid) ent, component, true);
    this.Dirty((EntityUid) ent, (IComponent) component);
    this._name.RefreshNameModifiers((Entity<NameModifierComponent>) ent.Owner);
    this._physics.SetBodyType((EntityUid) ent, BodyType.Static);
    return true;
  }

  public int ComputeNextId() => this._nextId++;

  public void MakeDropshipTarget(EntityUid ent, string abbreviation)
  {
    DropshipTargetComponent component = new DropshipTargetComponent()
    {
      Abbreviation = abbreviation
    };
    this.AddComp<DropshipTargetComponent>(ent, component, true);
    this.Dirty(ent, (IComponent) component);
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveFlareSignalComponent, TransformComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<ActiveFlareSignalComponent, TransformComponent>();
    EntityUid uid1;
    ActiveFlareSignalComponent comp1_1;
    TransformComponent comp2_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1, out comp2_1))
    {
      comp1_1.LastCoordinates.Enqueue(this.GetNetCoordinates(comp2_1.Coordinates));
      this.Dirty(uid1, (IComponent) comp1_1);
      if (comp1_1.LastCoordinates.Count >= 10)
      {
        comp1_1.LastCoordinates.Dequeue();
        bool flag = true;
        foreach (NetCoordinates lastCoordinate in comp1_1.LastCoordinates)
        {
          if (!this._transform.InRange(this.GetCoordinates(lastCoordinate), comp2_1.Coordinates, 0.01f))
          {
            flag = false;
            break;
          }
        }
        if (flag && this.TryActivateSignalFlareTarget((Entity<ActiveFlareSignalComponent>) (uid1, comp1_1)))
          this.RemCompDeferred<ActiveFlareSignalComponent>(uid1);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<AmmoInFlightComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<AmmoInFlightComponent>();
    EntityUid uid2;
    AmmoInFlightComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!this._net.IsClient)
      {
        if (!comp1_2.WarnedSound)
        {
          comp1_2.WarnedSound = true;
          this.Dirty(uid2, (IComponent) comp1_2);
          if (comp1_2.SoundWarning != null)
            this._audio.PlayPvs(comp1_2.SoundWarning, comp1_2.Target);
        }
        TimeSpan timeSpan;
        if (curTime >= comp1_2.WarningMarkerAt && !comp1_2.WarnedMarker)
        {
          comp1_2.WarnedMarker = true;
          this.Dirty(uid2, (IComponent) comp1_2);
          if (comp1_2.MarkerWarning)
          {
            comp1_2.WarningMarker = new EntityUid?(this.Spawn((string) SharedDropshipWeaponSystem.DropshipTargetMarker, comp1_2.Target));
            TimedDespawnComponent despawnComponent = this.EnsureComp<TimedDespawnComponent>(comp1_2.WarningMarker.Value);
            timeSpan = comp1_2.MarkerAt - this._timing.CurTime;
            double totalSeconds = timeSpan.TotalSeconds;
            despawnComponent.Lifetime = (float) totalSeconds;
          }
        }
        if (!(comp1_2.MarkerAt > curTime))
        {
          if (!comp1_2.SpawnedMarker)
          {
            comp1_2.SpawnedMarker = true;
            comp1_2.Marker = new EntityUid?(this.Spawn((string) SharedDropshipWeaponSystem.DropshipTargetMarker, comp1_2.Target));
            this.Dirty(uid2, (IComponent) comp1_2);
            this._audio.PlayPvs(comp1_2.SoundMarker, comp1_2.Marker.Value);
            if (this._net.IsServer)
              this.QueueDel(comp1_2.WarningMarker);
            comp1_2.WarningMarker = new EntityUid?();
          }
          if (!(comp1_2.MarkerAt.Add(TimeSpan.FromSeconds(1L)) > curTime))
          {
            if (comp1_2.Marker.HasValue)
            {
              if (this._net.IsServer)
                this.QueueDel(comp1_2.Marker);
              comp1_2.Marker = new EntityUid?();
              this.Dirty(uid2, (IComponent) comp1_2);
            }
            if (!(comp1_2.NextShot > curTime))
            {
              if (comp1_2.ShotsLeft > 0)
              {
                comp1_2.ShotsLeft -= comp1_2.ShotsPerVolley;
                comp1_2.NextShot = curTime + comp1_2.ShotDelay;
                --comp1_2.SoundShotsLeft;
                this.Dirty(uid2, (IComponent) comp1_2);
                Vector2 offset = Vector2.Zero;
                if (comp1_2.BulletSpread > 0)
                  offset = this._random.NextVector2((float) -comp1_2.BulletSpread, (float) (comp1_2.BulletSpread + 1));
                MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(comp1_2.Target).Offset(offset);
                foreach (EntProtoId impactEffect in comp1_2.ImpactEffects)
                  this.Spawn((string) impactEffect, mapCoordinates, rotation: this._random.NextAngle());
                if (comp1_2.Damage != null)
                {
                  this._damageables.Clear();
                  this._entityLookup.GetEntitiesInRange<DamageableComponent>(mapCoordinates, 0.49f, this._damageables, LookupFlags.Uncontained);
                  foreach (Entity<DamageableComponent> damageable1 in this._damageables)
                  {
                    DamageableSystem damageable2 = this._damageable;
                    EntityUid? uid3 = new EntityUid?((EntityUid) damageable1);
                    DamageSpecifier damage = comp1_2.Damage;
                    DamageableComponent damageable3 = (DamageableComponent) damageable1;
                    int armorPiercing1 = comp1_2.ArmorPiercing;
                    EntityUid? origin = new EntityUid?();
                    EntityUid? tool = new EntityUid?();
                    int armorPiercing2 = armorPiercing1;
                    damageable2.TryChangeDamage(uid3, damage, damageable: damageable3, origin: origin, tool: tool, armorPiercing: armorPiercing2);
                  }
                }
                if (comp1_2.Implosion != null)
                  this._rmcImplosion.Implode(comp1_2.Implosion, mapCoordinates);
                if (comp1_2.Fire != null)
                {
                  Entity<CollideChainComponent> chain = this._onCollide.SpawnChain();
                  int? total = comp1_2.Fire.Total;
                  bool cont;
                  if (total.HasValue)
                  {
                    int valueOrDefault = total.GetValueOrDefault();
                    List<Vector2i> list = new List<Vector2i>();
                    for (int index1 = -comp1_2.Fire.Range; index1 <= comp1_2.Fire.Range; ++index1)
                    {
                      for (int index2 = -comp1_2.Fire.Range; index2 <= comp1_2.Fire.Range; ++index2)
                        list.Add(Vector2i.op_Implicit((index1, index2)));
                    }
                    for (int index = 0; index < valueOrDefault && list.Count != 0; ++index)
                    {
                      Vector2i vector2i = this._random.PickAndTake<Vector2i>((IList<Vector2i>) list);
                      this._rmcFlammable.SpawnFire(comp1_2.Target.Offset(Vector2i.op_Implicit(vector2i)), comp1_2.Fire.Type, (EntityUid) chain, comp1_2.Fire.Range, comp1_2.Fire.Intensity, comp1_2.Fire.Duration, out cont);
                    }
                  }
                  else
                  {
                    this._rmcFlammable.SpawnFireLines(comp1_2.Fire.Type, comp1_2.Target, comp1_2.Fire.CardinalRange, comp1_2.Fire.OrdinalRange, comp1_2.Fire.Intensity, comp1_2.Fire.Duration);
                    for (int x = -comp1_2.Fire.Range; x <= comp1_2.Fire.Range; ++x)
                    {
                      for (int y = -comp1_2.Fire.Range; y <= comp1_2.Fire.Range; ++y)
                        this._rmcFlammable.SpawnFire(comp1_2.Target.Offset(new Vector2((float) x, (float) y)), comp1_2.Fire.Type, (EntityUid) chain, comp1_2.Fire.Range, comp1_2.Fire.Intensity, comp1_2.Fire.Duration, out cont);
                    }
                  }
                }
                if (comp1_2.Explosion != null)
                  this._rmcExplosion.QueueExplosion(mapCoordinates, (string) comp1_2.Explosion.Type, comp1_2.Explosion.Total, comp1_2.Explosion.Slope, comp1_2.Explosion.Max, new EntityUid?(uid2), canCreateVacuum: false);
                if (comp1_2.SoundShotsLeft <= 0)
                {
                  comp1_2.SoundShotsLeft = comp1_2.SoundEveryShots;
                  this._audio.PlayPvs(comp1_2.SoundImpact, comp1_2.Target);
                }
              }
              else
              {
                AmmoInFlightComponent inFlightComponent = comp1_2;
                timeSpan = inFlightComponent.PlayGroundSoundAt.GetValueOrDefault();
                if (!inFlightComponent.PlayGroundSoundAt.HasValue)
                {
                  timeSpan = this._timing.CurTime + comp1_2.SoundTravelTime;
                  inFlightComponent.PlayGroundSoundAt = new TimeSpan?(timeSpan);
                }
                this.Dirty(uid2, (IComponent) comp1_2);
                timeSpan = curTime;
                TimeSpan? playGroundSoundAt = comp1_2.PlayGroundSoundAt;
                if ((playGroundSoundAt.HasValue ? (timeSpan >= playGroundSoundAt.GetValueOrDefault() ? 1 : 0) : 0) != 0)
                {
                  this._audio.PlayPvs(comp1_2.SoundGround, comp1_2.Target);
                  if (this._net.IsServer)
                    this.QueueDel(new EntityUid?(uid2));
                }
              }
            }
          }
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveLaserDesignatorComponent, TransformComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<ActiveLaserDesignatorComponent, TransformComponent>();
    EntityUid uid4;
    ActiveLaserDesignatorComponent comp1_3;
    TransformComponent comp2_2;
    while (entityQueryEnumerator3.MoveNext(out uid4, out comp1_3, out comp2_2))
    {
      if (!this._transform.InRange(comp2_2.Coordinates, comp1_3.Origin, comp1_3.BreakRange))
        this.RemCompDeferred<ActiveLaserDesignatorComponent>(uid4);
    }
  }

  public void TargetUpdated(Entity<DropshipTargetComponent> ent)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipTerminalWeaponsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DropshipTerminalWeaponsComponent>();
    EntityUid uid;
    DropshipTerminalWeaponsComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      Span<DropshipTerminalWeaponsComponent.TargetEnt> span = CollectionsMarshal.AsSpan<DropshipTerminalWeaponsComponent.TargetEnt>(comp1.Medevacs);
      for (int index = 0; index < span.Length; ++index)
      {
        ref DropshipTerminalWeaponsComponent.TargetEnt local = ref span[index];
        if (!(local.Id != this.GetNetEntity(ent.Owner)))
        {
          local = local with
          {
            Name = ent.Comp.Abbreviation
          };
          break;
        }
      }
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  private EntityUid? EnsureTargetEye(
    Entity<DropshipTerminalWeaponsComponent> terminal,
    Entity<DropshipTargetComponent?>? targetNullable)
  {
    if (!targetNullable.HasValue)
      return new EntityUid?();
    Entity<DropshipTargetComponent> entity = targetNullable.Value;
    if (!this.Resolve<DropshipTargetComponent>((EntityUid) entity, ref entity.Comp, false))
      return new EntityUid?();
    EntityUid uid;
    if (!entity.Comp.Eyes.TryGetValue((EntityUid) terminal, out uid))
    {
      if (this._net.IsClient)
        return new EntityUid?();
      uid = this.Spawn((string) null, entity.Owner.ToCoordinates());
      entity.Comp.Eyes[(EntityUid) terminal] = uid;
      this.Dirty<DropshipTargetComponent>(entity);
      EyeComponent eyeComponent = this.EnsureComp<EyeComponent>(uid);
      this._eye.SetDrawFov(uid, false, eyeComponent);
      DropshipTargetEyeComponent targetEyeComponent = this.EnsureComp<DropshipTargetEyeComponent>(uid);
      targetEyeComponent.Target = new EntityUid?((EntityUid) entity);
      this.Dirty(uid, (IComponent) targetEyeComponent);
    }
    return new EntityUid?(uid);
  }

  public bool TryGetTargetEye(
    Entity<DropshipTerminalWeaponsComponent> terminal,
    Entity<DropshipTargetComponent?> target,
    out EntityUid eye)
  {
    if (this.Resolve<DropshipTargetComponent>((EntityUid) target, ref target.Comp, false) && target.Comp.Eyes.TryGetValue((EntityUid) terminal, out eye))
      return true;
    eye = new EntityUid();
    return false;
  }

  public void MakeTarget(EntityUid target, string abbreviation, bool targetableByWeapons)
  {
    DropshipTargetComponent component = new DropshipTargetComponent()
    {
      Abbreviation = abbreviation,
      IsTargetableByWeapons = targetableByWeapons
    };
    this.AddComp<DropshipTargetComponent>(target, component, true);
  }

  private void SetScreenUtility<T>(
    Entity<DropshipTerminalWeaponsComponent> ent,
    bool first,
    DropshipTerminalWeaponsScreen state,
    NetEntity? selected = null)
    where T : IComponent
  {
    Entity<DropshipComponent> dropship;
    if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship) || dropship.Comp.AttachmentPoints.Count == 0)
      return;
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    foreach (EntityUid attachmentPoint in dropship.Comp.AttachmentPoints)
    {
      DropshipElectronicSystemPointComponent comp1;
      BaseContainer container1;
      if (this.TryComp<DropshipElectronicSystemPointComponent>(attachmentPoint, out comp1) && this._container.TryGetContainer(attachmentPoint, comp1.ContainerId, out container1) && container1.ContainedEntities.Count > 0)
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container1.ContainedEntities)
        {
          if (this.HasComp<T>(containedEntity))
          {
            flag2 = true;
            break;
          }
        }
      }
      DropshipWeaponPointComponent comp2;
      BaseContainer container2;
      if (this.TryComp<DropshipWeaponPointComponent>(attachmentPoint, out comp2) && this._container.TryGetContainer(attachmentPoint, comp2.WeaponContainerSlotId, out container2) && container2.ContainedEntities.Count > 0)
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container2.ContainedEntities)
        {
          if (this.HasComp<T>(containedEntity))
          {
            flag3 = true;
            break;
          }
        }
      }
      DropshipUtilityPointComponent comp3;
      BaseContainer container3;
      if (this.TryComp<DropshipUtilityPointComponent>(attachmentPoint, out comp3) && this._container.TryGetContainer(attachmentPoint, comp3.UtilitySlotId, out container3) && container3.ContainedEntities.Count != 0)
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container3.ContainedEntities)
        {
          if (this.HasComp<T>(containedEntity))
          {
            flag1 = true;
            break;
          }
        }
      }
    }
    if (!flag1 && !flag2 && !flag3)
      return;
    // ISSUE: explicit reference operation
    (^ref (first ? ref ent.Comp.ScreenOne : ref ent.Comp.ScreenTwo)).State = state;
    ent.Comp.SelectedSystem = selected;
    this.Dirty<DropshipTerminalWeaponsComponent>(ent);
    this.RefreshWeaponsUI(ent);
  }
}
