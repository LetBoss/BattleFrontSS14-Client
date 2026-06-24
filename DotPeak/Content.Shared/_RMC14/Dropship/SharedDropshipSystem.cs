// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.SharedDropshipSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Evacuation;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Thunderdome;
using Content.Shared._RMC14.Tracker;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Maturing;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Dropship;

public abstract class SharedDropshipSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedGameTicker _gameTicker;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private IGameTiming _timing;
  private TimeSpan _dropshipInitialDelay;
  private TimeSpan _hijackInitialDelay;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DropshipComponent, MapInitEvent>(new EntityEventRefHandler<DropshipComponent, MapInitEvent>(this.OnDropshipMapInit));
    this.SubscribeLocalEvent<DropshipNavigationComputerComponent, MapInitEvent>(new EntityEventRefHandler<DropshipNavigationComputerComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<DropshipNavigationComputerComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<DropshipNavigationComputerComponent, ActivatableUIOpenAttemptEvent>(this.OnUIOpenAttempt));
    this.SubscribeLocalEvent<DropshipNavigationComputerComponent, AfterActivatableUIOpenEvent>(new EntityEventRefHandler<DropshipNavigationComputerComponent, AfterActivatableUIOpenEvent>(this.OnNavigationOpen));
    this.SubscribeLocalEvent<DropshipTerminalComponent, ActivateInWorldEvent>(new EntityEventRefHandler<DropshipTerminalComponent, ActivateInWorldEvent>(this.OnDropshipTerminalActivateInWorld));
    this.SubscribeLocalEvent<DropshipWeaponPointComponent, MapInitEvent>(new EntityEventRefHandler<DropshipWeaponPointComponent, MapInitEvent>(this.OnAttachmentPointMapInit<DropshipWeaponPointComponent, MapInitEvent>));
    this.SubscribeLocalEvent<DropshipWeaponPointComponent, EntityTerminatingEvent>(new EntityEventRefHandler<DropshipWeaponPointComponent, EntityTerminatingEvent>(this.OnAttachmentPointRemove<DropshipWeaponPointComponent, EntityTerminatingEvent>));
    this.SubscribeLocalEvent<DropshipWeaponPointComponent, ExaminedEvent>(new EntityEventRefHandler<DropshipWeaponPointComponent, ExaminedEvent>(this.OnAttachmentExamined));
    this.SubscribeLocalEvent<DropshipWeaponPointComponent, InteractHandEvent>(new EntityEventRefHandler<DropshipWeaponPointComponent, InteractHandEvent>(this.OnInteract));
    this.SubscribeLocalEvent<DropshipUtilityPointComponent, MapInitEvent>(new EntityEventRefHandler<DropshipUtilityPointComponent, MapInitEvent>(this.OnAttachmentPointMapInit<DropshipUtilityPointComponent, MapInitEvent>));
    this.SubscribeLocalEvent<DropshipUtilityPointComponent, EntityTerminatingEvent>(new EntityEventRefHandler<DropshipUtilityPointComponent, EntityTerminatingEvent>(this.OnAttachmentPointRemove<DropshipUtilityPointComponent, EntityTerminatingEvent>));
    this.SubscribeLocalEvent<DropshipEnginePointComponent, MapInitEvent>(new EntityEventRefHandler<DropshipEnginePointComponent, MapInitEvent>(this.OnAttachmentPointMapInit<DropshipEnginePointComponent, MapInitEvent>));
    this.SubscribeLocalEvent<DropshipEnginePointComponent, EntityTerminatingEvent>(new EntityEventRefHandler<DropshipEnginePointComponent, EntityTerminatingEvent>(this.OnAttachmentPointRemove<DropshipEnginePointComponent, EntityTerminatingEvent>));
    this.SubscribeLocalEvent<DropshipEnginePointComponent, ExaminedEvent>(new EntityEventRefHandler<DropshipEnginePointComponent, ExaminedEvent>(this.OnEngineExamined));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, MapInitEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, MapInitEvent>(this.OnAttachmentPointMapInit<DropshipElectronicSystemPointComponent, MapInitEvent>));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, EntityTerminatingEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, EntityTerminatingEvent>(this.OnAttachmentPointRemove<DropshipElectronicSystemPointComponent, EntityTerminatingEvent>));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, ExaminedEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, ExaminedEvent>(this.OnElectronicSystemExamined));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, InteractHandEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, InteractHandEvent>(this.OnInteract));
    this.Subs.BuiEvents<DropshipNavigationComputerComponent>((object) DropshipNavigationUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<DropshipNavigationComputerComponent>) (subs =>
    {
      subs.Event<DropshipNavigationLaunchMsg>(new EntityEventRefHandler<DropshipNavigationComputerComponent, DropshipNavigationLaunchMsg>(this.OnDropshipNavigationLaunchMsg));
      subs.Event<DropshipNavigationCancelMsg>(new EntityEventRefHandler<DropshipNavigationComputerComponent, DropshipNavigationCancelMsg>(this.OnDropshipNavigationCancelMsg));
    }));
    this.Subs.BuiEvents<DropshipNavigationComputerComponent>((object) DropshipHijackerUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<DropshipNavigationComputerComponent>) (subs => subs.Event<DropshipHijackerDestinationChosenBuiMsg>(new EntityEventRefHandler<DropshipNavigationComputerComponent, DropshipHijackerDestinationChosenBuiMsg>(this.OnHijackerDestinationChosenMsg))));
    this.Subs.CVar<float>(this._config, RMCCVars.RMCDropshipInitialDelayMinutes, (Action<float>) (v => this._dropshipInitialDelay = TimeSpan.FromMinutes((double) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCDropshipHijackInitialDelayMinutes, (Action<int>) (v => this._hijackInitialDelay = TimeSpan.FromMinutes((long) v)), true);
  }

  private void OnDropshipMapInit(Entity<DropshipComponent> ent, ref MapInitEvent args)
  {
    TransformChildrenEnumerator childEnumerator = this.Transform((EntityUid) ent).ChildEnumerator;
    EntityUid child;
    while (childEnumerator.MoveNext(out child))
    {
      if (!this.TerminatingOrDeleted(child) && (this.HasComp<DropshipWeaponPointComponent>(child) || this.HasComp<DropshipEnginePointComponent>(child) || this.HasComp<DropshipUtilityPointComponent>(child) || this.HasComp<DropshipElectronicSystemPointComponent>(child)))
        ent.Comp.AttachmentPoints.Add(child);
    }
    DropshipMapInitEvent args1 = new DropshipMapInitEvent();
    this.RaiseLocalEvent<DropshipMapInitEvent>((EntityUid) ent, ref args1);
  }

  private void OnMapInit(Entity<DropshipNavigationComputerComponent> ent, ref MapInitEvent args)
  {
    EntityUid parentUid = this.Transform((EntityUid) ent).ParentUid;
    if (!parentUid.Valid || !this.IsShuttle(parentUid))
      return;
    this.EnsureComp<DropshipComponent>(parentUid);
  }

  private void OnUIOpenAttempt(
    Entity<DropshipNavigationComputerComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    DropshipComponent comp;
    if (this.TryComp<DropshipComponent>(this.Transform((EntityUid) ent).ParentUid, out comp) && comp.Crashed)
    {
      args.Cancel();
    }
    else
    {
      if (this.TryDropshipLaunchPopup((EntityUid) ent, args.User, true))
        return;
      args.Cancel();
    }
  }

  private void OnNavigationOpen(
    Entity<DropshipNavigationComputerComponent> ent,
    ref AfterActivatableUIOpenEvent args)
  {
    this.RefreshUI(ent);
  }

  private void OnDropshipTerminalActivateInWorld(
    Entity<DropshipTerminalComponent> ent,
    ref ActivateInWorldEvent args)
  {
    if (this._net.IsClient)
      return;
    EntityUid user = args.User;
    if (!this.HasComp<XenoComponent>(user))
      this._popup.PopupEntity("This terminal doesn't seem to work yet... Maybe you should ask High Command?", user, user, PopupType.MediumCaution);
    else if (!this.HasComp<DropshipHijackerComponent>(user))
    {
      this._popup.PopupEntity("You stare cluelessly at the " + this.Name(ent.Owner), user, user);
    }
    else
    {
      if (!this.TryDropshipLaunchPopup((EntityUid) ent, user, false) || !this.TryDropshipHijackPopup((EntityUid) ent, (Entity<DropshipHijackerComponent>) user, false))
        return;
      TransformComponent transformComponent = this.Transform(user);
      Entity<DropshipDestinationComponent, TransformComponent>? nullable1 = new Entity<DropshipDestinationComponent, TransformComponent>?();
      Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipDestinationComponent, TransformComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<DropshipDestinationComponent, TransformComponent>();
      EntityUid uid1;
      DropshipDestinationComponent comp1_1;
      TransformComponent comp2_1;
      while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1, out comp2_1))
      {
        if (!(comp2_1.MapID != transformComponent.MapID))
        {
          if (!nullable1.HasValue)
          {
            nullable1 = new Entity<DropshipDestinationComponent, TransformComponent>?((Entity<DropshipDestinationComponent, TransformComponent>) (uid1, comp1_1, comp2_1));
          }
          else
          {
            EntityCoordinates coordinates = transformComponent.Coordinates;
            float distance1;
            if (coordinates.TryDistance((IEntityManager) this.EntityManager, comp2_1.Coordinates, out distance1))
            {
              coordinates = transformComponent.Coordinates;
              float distance2;
              if (coordinates.TryDistance((IEntityManager) this.EntityManager, nullable1.Value.Comp2.Coordinates, out distance2) && (double) distance1 < (double) distance2)
                nullable1 = new Entity<DropshipDestinationComponent, TransformComponent>?((Entity<DropshipDestinationComponent, TransformComponent>) (uid1, comp1_1, comp2_1));
            }
          }
        }
      }
      if (!nullable1.HasValue)
        this._popup.PopupEntity("There are no dropship destinations near you!", user, user, PopupType.MediumCaution);
      else if (nullable1.Value.Comp1.Ship.HasValue)
      {
        this._popup.PopupEntity("There's already a dropship coming here!", user, user, PopupType.MediumCaution);
      }
      else
      {
        if (this.Count<PrimaryLandingZoneComponent>() > 0)
        {
          Entity<DropshipDestinationComponent, TransformComponent>? nullable2 = nullable1;
          if (!this.HasComp<PrimaryLandingZoneComponent>(nullable2.HasValue ? new EntityUid?((EntityUid) nullable2.GetValueOrDefault()) : new EntityUid?()))
          {
            this._popup.PopupEntity("The shuttle isn't responding to prompts, it looks like this isn't the primary shuttle.", user, user, PopupType.MediumCaution);
            return;
          }
        }
        Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipComponent, TransformComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<DropshipComponent, TransformComponent>();
label_23:
        EntityUid uid2;
        DropshipComponent comp1_2;
        TransformComponent comp2_2;
        while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2, out comp2_2))
        {
          if (!comp1_2.Crashed && !this.IsInFTL(uid2) && !this.HasComp<ThunderdomeMapComponent>(comp2_2.MapUid))
          {
            Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipNavigationComputerComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<DropshipNavigationComputerComponent>();
            EntityUid uid3;
            DropshipNavigationComputerComponent comp1_3;
            EntityUid? gridUid;
            EntityUid entityUid;
            do
            {
              do
              {
                if (!entityQueryEnumerator3.MoveNext(out uid3, out comp1_3))
                  goto label_23;
              }
              while (!comp1_3.Hijackable);
              gridUid = this.Transform(uid3).GridUid;
              entityUid = uid2;
            }
            while ((gridUid.HasValue ? (gridUid.GetValueOrDefault() == entityUid ? 1 : 0) : 0) == 0 || !this.FlyTo((Entity<DropshipNavigationComputerComponent>) (uid3, comp1_3), (EntityUid) nullable1.Value, new EntityUid?(user)));
            this._popup.PopupEntity("You call down one of the dropships to your location", user, user, PopupType.LargeCaution);
            return;
          }
        }
        this._popup.PopupEntity("There are no available dropships! Wait a moment.", user, user, PopupType.LargeCaution);
      }
    }
  }

  private void OnAttachmentPointMapInit<TComp, TEvent>(Entity<TComp> ent, ref TEvent args) where TComp : IComponent?
  {
    Entity<DropshipComponent> dropship;
    if (this._net.IsClient || !this.TryGetGridDropship((EntityUid) ent, out dropship))
      return;
    dropship.Comp.AttachmentPoints.Add((EntityUid) ent);
    this.Dirty<DropshipComponent>(dropship);
  }

  private void OnAttachmentPointRemove<TComp, TEvent>(Entity<TComp> ent, ref TEvent args) where TComp : IComponent?
  {
    Entity<DropshipComponent> dropship;
    if (!this.TryGetGridDropship((EntityUid) ent, out dropship))
      return;
    dropship.Comp.AttachmentPoints.Remove((EntityUid) ent);
    this.Dirty<DropshipComponent>(dropship);
  }

  private void OnAttachmentExamined(
    Entity<DropshipWeaponPointComponent> ent,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("DropshipWeaponPointComponent"))
    {
      EntityUid contained1;
      if (this.TryGetAttachmentContained((EntityUid) ent, ent.Comp.WeaponContainerSlotId, out contained1))
        args.PushText(this.Loc.GetString("rmc-dropship-attached", ("attachment", (object) contained1)));
      EntityUid contained2;
      if (!this.TryGetAttachmentContained((EntityUid) ent, ent.Comp.AmmoContainerSlotId, out contained2))
        return;
      args.PushText(this.Loc.GetString("rmc-dropship-weapons-point-ammo", ("ammo", (object) contained2)));
      DropshipAmmoComponent comp;
      if (!this.TryComp<DropshipAmmoComponent>(contained2, out comp))
        return;
      args.PushText(this.Loc.GetString("rmc-dropship-weapons-rounds-left", ("current", (object) comp.Rounds), ("max", (object) comp.MaxRounds)));
    }
  }

  private void OnEngineExamined(Entity<DropshipEnginePointComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("DropshipWeaponPointComponent"))
    {
      EntityUid contained;
      if (!this.TryGetAttachmentContained((EntityUid) ent, ent.Comp.ContainerId, out contained))
        return;
      args.PushText(this.Loc.GetString("rmc-dropship-attached", ("attachment", (object) contained)));
    }
  }

  private void OnElectronicSystemExamined(
    Entity<DropshipElectronicSystemPointComponent> ent,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("DropshipWeaponPointComponent"))
    {
      EntityUid contained;
      if (!this.TryGetAttachmentContained((EntityUid) ent, ent.Comp.ContainerId, out contained))
        return;
      args.PushText(this.Loc.GetString("rmc-dropship-attached", ("attachment", (object) contained)));
    }
  }

  private void OnDropshipNavigationLaunchMsg(
    Entity<DropshipNavigationComputerComponent> ent,
    ref DropshipNavigationLaunchMsg args)
  {
    EntityUid actor = args.Actor;
    EntityUid? entity;
    if (!this.TryGetEntity(args.Target, out entity))
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to launch to invalid dropship destination {args.Target}");
    else if (!this.HasComp<DropshipDestinationComponent>(entity))
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) args.Actor)} tried to launch to invalid dropship destination {this.ToPrettyString(entity)}");
    else
      this.FlyTo(ent, entity.Value, new EntityUid?(actor));
  }

  private void OnDropshipNavigationCancelMsg(
    Entity<DropshipNavigationComputerComponent> ent,
    ref DropshipNavigationCancelMsg args)
  {
    EntityUid? grid = this._transform.GetGrid((Entity<TransformComponent>) (ent.Owner, this.Transform(ent.Owner)));
    FTLComponent comp1;
    DropshipComponent comp2;
    if (!this.TryComp<FTLComponent>(grid, out comp1) || !this.TryComp<DropshipComponent>(grid, out comp2))
      return;
    EntityUid? destination = comp2.Destination;
    EntityUid? departureLocation = comp2.DepartureLocation;
    if ((destination.HasValue == departureLocation.HasValue ? (destination.HasValue ? (destination.GetValueOrDefault() != departureLocation.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0 || this._timing.CurTime + comp2.CancelFlightTime >= comp1.StateTime.End)
      return;
    comp1.StateTime.End = this._timing.CurTime + comp2.CancelFlightTime;
    this.Dirty(grid.Value, (IComponent) comp2);
    this.RefreshUI();
  }

  private void OnHijackerDestinationChosenMsg(
    Entity<DropshipNavigationComputerComponent> ent,
    ref DropshipHijackerDestinationChosenBuiMsg args)
  {
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) DropshipHijackerUiKey.Key, new EntityUid?(args.Actor));
    EntityUid? entity;
    if (!this.TryGetEntity(args.Destination, out entity))
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) args.Actor)} tried to hijack to invalid destination");
    else if (!this.HasComp<DropshipHijackDestinationComponent>(entity))
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) args.Actor)} tried to hijack to invalid destination {this.ToPrettyString(entity)}");
    }
    else
    {
      TransformComponent comp;
      if (!this.FlyTo(ent, entity.Value, new EntityUid?(args.Actor), true) || !this.TryComp((EntityUid) ent, out comp) || !comp.ParentUid.Valid)
        return;
      DropshipComponent dropshipComponent = this.EnsureComp<DropshipComponent>(comp.ParentUid);
      dropshipComponent.Crashed = true;
      this.Dirty(comp.ParentUid, (IComponent) dropshipComponent);
      DropshipHijackStartEvent message = new DropshipHijackStartEvent(new EntityUid?(comp.ParentUid));
      this.RaiseLocalEvent<DropshipHijackStartEvent>(ref message);
    }
  }

  private void OnInteract(Entity<DropshipWeaponPointComponent> ent, ref InteractHandEvent args)
  {
    this.RelayInteractToContained(this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.WeaponContainerSlotId), ref args);
  }

  private void OnInteract(
    Entity<DropshipElectronicSystemPointComponent> ent,
    ref InteractHandEvent args)
  {
    this.RelayInteractToContained(this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.ContainerId), ref args);
  }

  private void RelayInteractToContained(ContainerSlot slot, ref InteractHandEvent args)
  {
    EntityUid? containedEntity = slot.ContainedEntity;
    if (!this.HasComp<RMCEquipmentDeployerComponent>(containedEntity))
      return;
    InteractHandEvent args1 = new InteractHandEvent(args.User, args.Target);
    this.RaiseLocalEvent<InteractHandEvent>(containedEntity.Value, args1);
    args.Handled = args1.Handled;
  }

  public virtual bool FlyTo(
    Entity<DropshipNavigationComputerComponent> computer,
    EntityUid destination,
    EntityUid? user,
    bool hijack = false,
    float? startupTime = null,
    float? hyperspaceTime = null,
    bool offset = false)
  {
    return false;
  }

  protected virtual void RefreshUI()
  {
  }

  protected virtual void RefreshUI(
    Entity<DropshipNavigationComputerComponent> computer)
  {
  }

  protected virtual bool IsShuttle(EntityUid dropship) => false;

  protected virtual bool IsInFTL(EntityUid dropship) => false;

  private bool TryDropshipLaunchPopup(EntityUid computer, EntityUid user, bool predicted)
  {
    TimeSpan timeSpan = this._gameTicker.RoundDuration();
    if (!(timeSpan < this._dropshipInitialDelay))
      return true;
    string message = this.Loc.GetString("rmc-dropship-pre-flight-fueling", ("minutes", (object) Math.Max(1, (int) (this._dropshipInitialDelay - timeSpan).TotalMinutes)));
    if (predicted)
      this._popup.PopupClient(message, computer, new EntityUid?(user), PopupType.MediumCaution);
    else
      this._popup.PopupEntity(message, computer, user, PopupType.MediumCaution);
    return false;
  }

  protected bool TryDropshipHijackPopup(
    EntityUid computer,
    Entity<DropshipHijackerComponent?> user,
    bool predicted)
  {
    TimeSpan timeSpan = this._gameTicker.RoundDuration();
    if (this.HasComp<DropshipHijackerComponent>((EntityUid) user) && timeSpan < this._hijackInitialDelay)
    {
      string message = this.Loc.GetString("rmc-dropship-pre-hijack", ("minutes", (object) Math.Max(1, (int) (this._hijackInitialDelay - timeSpan).TotalMinutes)));
      if (predicted)
        this._popup.PopupClient(message, computer, new EntityUid?((EntityUid) user), PopupType.MediumCaution);
      else
        this._popup.PopupEntity(message, computer, (EntityUid) user, PopupType.MediumCaution);
      return false;
    }
    EntityUid? map = this._transform.GetMap((Entity<TransformComponent>) user.Owner);
    EvacuationProgressComponent comp;
    if ((!this.HasComp<XenoMaturingComponent>((EntityUid) user) || this.HasComp<RMCPlanetComponent>(map)) && (!this.TryComp<EvacuationProgressComponent>(map, out comp) || !comp.DropShipCrashed))
      return true;
    string message1 = this.Loc.GetString("rmc-dropship-invalid-hijack");
    if (predicted)
      this._popup.PopupClient(message1, computer, new EntityUid?((EntityUid) user), PopupType.MediumCaution);
    else
      this._popup.PopupEntity(message1, computer, (EntityUid) user, PopupType.MediumCaution);
    return false;
  }

  public bool TryDesignatePrimaryLZ(EntityUid actor, EntityUid lz)
  {
    if (!this.HasComp<DropshipDestinationComponent>(lz))
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to designate as primary LZ entity {this.ToPrettyString((Entity<MetaDataComponent>) lz)} with no {"DropshipDestinationComponent"}!");
      return false;
    }
    if (this.Count<PrimaryLandingZoneComponent>() > 0)
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to designate as primary LZ entity {this.ToPrettyString((Entity<MetaDataComponent>) lz)} when one already exists!");
      return false;
    }
    if (!this.HasComp<RMCPlanetComponent>(this._transform.GetGrid((Entity<TransformComponent>) lz)) && !this.HasComp<RMCPlanetComponent>(this._transform.GetMap((Entity<TransformComponent>) lz)))
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to designate entity {this.ToPrettyString((Entity<MetaDataComponent>) lz)} on the warship as primary LZ!");
      return false;
    }
    if (this.GetPrimaryLZCandidates().All<Entity<MetaDataComponent>>((Func<Entity<MetaDataComponent>, bool>) (candidate => candidate.Owner != lz)))
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to designate invalid primary LZ entity {this.ToPrettyString((Entity<MetaDataComponent>) lz)}!");
      return false;
    }
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(36, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) actor), "player", "ToPrettyString(actor)");
    logStringHandler.AppendLiteral(" designated ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) lz), nameof (lz), "ToPrettyString(lz)");
    logStringHandler.AppendLiteral(" as primary landing zone");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCPrimaryLZ, ref local);
    this.EnsureComp<PrimaryLandingZoneComponent>(lz);
    this.EnsureComp<RMCTrackableComponent>(lz);
    this.RefreshUI();
    string message = this.Loc.GetString("rmc-announcement-ares-lz-designated", ("name", (object) this.Name(lz)));
    this._marineAnnounce.AnnounceARESStaging(new EntityUid?(actor), message);
    return true;
  }

  public IEnumerable<Entity<MetaDataComponent>> GetPrimaryLZCandidates()
  {
    SharedDropshipSystem sharedDropshipSystem = this;
    if (sharedDropshipSystem.Count<PrimaryLandingZoneComponent>() == 0)
    {
      Robust.Shared.GameObjects.EntityQueryEnumerator<DropshipDestinationComponent, MetaDataComponent, TransformComponent> landingZoneQuery = sharedDropshipSystem.EntityQueryEnumerator<DropshipDestinationComponent, MetaDataComponent, TransformComponent>();
      EntityUid uid;
      MetaDataComponent comp2;
      TransformComponent comp3;
      while (landingZoneQuery.MoveNext(out uid, out DropshipDestinationComponent _, out comp2, out comp3))
      {
        if (sharedDropshipSystem.HasComp<RMCPlanetComponent>(comp3.ParentUid) || sharedDropshipSystem.HasComp<RMCPlanetComponent>(comp3.MapUid))
          yield return (Entity<MetaDataComponent>) (uid, comp2);
      }
    }
  }

  public bool TryGetGridDropship(EntityUid ent, out Entity<DropshipComponent> dropship)
  {
    TransformComponent comp1;
    if (this.TryComp(ent, out comp1))
    {
      EntityUid? gridUid = comp1.GridUid;
      if (gridUid.HasValue)
      {
        EntityUid valueOrDefault = gridUid.GetValueOrDefault();
        DropshipComponent comp2;
        if (!this.TerminatingOrDeleted(valueOrDefault) && this.TryComp<DropshipComponent>(comp1.GridUid, out comp2))
        {
          dropship = (Entity<DropshipComponent>) (valueOrDefault, comp2);
          return true;
        }
      }
    }
    dropship = new Entity<DropshipComponent>();
    return false;
  }

  public bool IsWeaponAttached(Entity<DropshipWeaponComponent?> weapon)
  {
    Entity<DropshipComponent> dropship;
    BaseContainer container;
    return this.Resolve<DropshipWeaponComponent>((EntityUid) weapon, ref weapon.Comp, false) && this.TryGetGridDropship((EntityUid) weapon, out dropship) && this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) weapon, (TransformComponent) null), out container) && dropship.Comp.AttachmentPoints.Contains(container.Owner);
  }

  public bool TryGetAttachmentContained(
    EntityUid point,
    string containerId,
    out EntityUid contained)
  {
    contained = new EntityUid();
    BaseContainer container;
    if (!this._container.TryGetContainer(point, containerId, out container) || container.ContainedEntities.Count == 0)
      return false;
    contained = container.ContainedEntities[0];
    return true;
  }

  public bool IsInFlight(Entity<DropshipComponent?> dropship)
  {
    if (!this.Resolve<DropshipComponent>((EntityUid) dropship, ref dropship.Comp, false))
      return false;
    return dropship.Comp.State == FTLState.Travelling || dropship.Comp.State == FTLState.Arriving;
  }

  public bool IsOnDropship(EntityUid entity)
  {
    return this.HasComp<DropshipComponent>(this._transform.GetGrid((Entity<TransformComponent>) entity));
  }

  public bool IsOnDropship(EntityCoordinates coordinates)
  {
    return this.HasComp<DropshipComponent>(this._transform.GetGrid(coordinates));
  }
}
