// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Evacuation.SharedEvacuationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.HyperSleep;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared.Audio;
using Content.Shared.CCVar;
using Content.Shared.Coordinates;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Prying.Components;
using Content.Shared.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.EntitySerialization;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

#nullable enable
namespace Content.Shared._RMC14.Evacuation;

public abstract class SharedEvacuationSystem : EntitySystem
{
  [Dependency]
  private SharedAmbientSoundSystem _ambientSound;
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
  private SharedDoorSystem _door;
  [Dependency]
  private SharedHyperSleepChamberSystem _hyperSleep;
  [Dependency]
  private MapLoaderSystem _mapLoader;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedRMCExplosionSystem _rmcExplosion;
  [Dependency]
  private SharedRMCPowerSystem _rmcPower;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedXenoAnnounceSystem _xenoAnnounce;
  private Robust.Shared.GameObjects.EntityQuery<AreaComponent> _areaQuery;
  private Robust.Shared.GameObjects.EntityQuery<DoorComponent> _doorQuery;
  private Robust.Shared.GameObjects.EntityQuery<MobStateComponent> _mobStateQuery;
  private MapId? _map;
  private int _index;

  public override void Initialize()
  {
    this._areaQuery = this.GetEntityQuery<AreaComponent>();
    this._doorQuery = this.GetEntityQuery<DoorComponent>();
    this._mobStateQuery = this.GetEntityQuery<MobStateComponent>();
    this.SubscribeLocalEvent<DropshipHijackLandedEvent>(new EntityEventRefHandler<DropshipHijackLandedEvent>(this.OnDropshipHijackLanded), after: new Type[1]
    {
      typeof (SharedRMCPowerSystem)
    });
    this.SubscribeLocalEvent<EvacuationEnabledEvent>(new EntityEventRefHandler<EvacuationEnabledEvent>(this.OnEvacuationEnabled));
    this.SubscribeLocalEvent<EvacuationDisabledEvent>(new EntityEventRefHandler<EvacuationDisabledEvent>(this.OnEvacuationDisabled));
    this.SubscribeLocalEvent<EvacuationProgressEvent>(new EntityEventRefHandler<EvacuationProgressEvent>(this.OnEvacuationProgress));
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<GridSpawnerComponent, MapInitEvent>(new EntityEventRefHandler<GridSpawnerComponent, MapInitEvent>(this.OnGridSpawnerMapInit));
    this.SubscribeLocalEvent<EvacuationDoorComponent, BeforeDoorOpenedEvent>(new EntityEventRefHandler<EvacuationDoorComponent, BeforeDoorOpenedEvent>(this.OnEvacuationDoorBeforeOpened));
    this.SubscribeLocalEvent<EvacuationDoorComponent, BeforeDoorClosedEvent>(new EntityEventRefHandler<EvacuationDoorComponent, BeforeDoorClosedEvent>(this.OnEvacuationDoorBeforeClosed));
    this.SubscribeLocalEvent<EvacuationDoorComponent, BeforePryEvent>(new EntityEventRefHandler<EvacuationDoorComponent, BeforePryEvent>(this.OnEvacuationDoorBeforePry));
    this.SubscribeLocalEvent<EvacuationComputerComponent, ExaminedEvent>(new EntityEventRefHandler<EvacuationComputerComponent, ExaminedEvent>(this.OnEvacuationComputerExamined));
    this.SubscribeLocalEvent<EvacuationComputerComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<EvacuationComputerComponent, ActivatableUIOpenAttemptEvent>(this.OnEvacuationComputerUIOpenAttempt));
    this.SubscribeLocalEvent<LifeboatComputerComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<LifeboatComputerComponent, ActivatableUIOpenAttemptEvent>(this.OnLifeboatComputerUIOpenAttempt));
    this.SubscribeLocalEvent<EvacuationPumpComponent, ExaminedEvent>(new EntityEventRefHandler<EvacuationPumpComponent, ExaminedEvent>(this.OnEvacuationPumpExamined));
    this.Subs.BuiEvents<EvacuationComputerComponent>((object) EvacuationComputerUi.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<EvacuationComputerComponent>) (subs => subs.Event<EvacuationComputerLaunchBuiMsg>(new EntityEventRefHandler<EvacuationComputerComponent, EvacuationComputerLaunchBuiMsg>(this.OnEvacuationComputerLaunch))));
    this.Subs.BuiEvents<LifeboatComputerComponent>((object) LifeboatComputerUi.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<LifeboatComputerComponent>) (subs => subs.Event<LifeboatComputerLaunchBuiMsg>(new EntityEventRefHandler<LifeboatComputerComponent, LifeboatComputerLaunchBuiMsg>(this.OnLifeboatComputerLaunch))));
  }

  private void OnDropshipHijackLanded(ref DropshipHijackLandedEvent ev)
  {
    this.EnsureComp<EvacuationProgressComponent>(ev.Map).DropShipCrashed = true;
    Robust.Shared.GameObjects.EntityQueryEnumerator<EvacuationDoorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<EvacuationDoorComponent>();
    EntityUid uid;
    EvacuationDoorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Locked = false;
      this.Dirty(uid, (IComponent) comp1);
    }
    this._config.SetCVar<bool>(CCVars.GameDisallowLateJoins, true);
  }

  private void OnEvacuationEnabled(ref EvacuationEnabledEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<LifeboatComputerComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<LifeboatComputerComponent>();
    EntityUid uid1;
    LifeboatComputerComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      comp1_1.Enabled = true;
      this.Dirty(uid1, (IComponent) comp1_1);
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<EvacuationComputerComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<EvacuationComputerComponent>();
    EntityUid uid2;
    EvacuationComputerComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (comp1_2.Mode == EvacuationComputerMode.Disabled)
      {
        comp1_2.Mode = EvacuationComputerMode.Ready;
        this.Dirty(uid2, (IComponent) comp1_2);
      }
    }
  }

  private void OnEvacuationDisabled(ref EvacuationDisabledEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<LifeboatComputerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<LifeboatComputerComponent>();
    EntityUid uid;
    LifeboatComputerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      comp1.Enabled = false;
      this.Dirty(uid, (IComponent) comp1);
    }
  }

  private void OnEvacuationProgress(ref EvacuationProgressEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<EvacuationComputerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<EvacuationComputerComponent>();
    EntityUid uid;
    EvacuationComputerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Mode == EvacuationComputerMode.Disabled)
      {
        comp1.Mode = EvacuationComputerMode.Ready;
        this.Dirty(uid, (IComponent) comp1);
      }
    }
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
  {
    this._map = new MapId?();
    this._index = 0;
  }

  private void OnGridSpawnerMapInit(Entity<GridSpawnerComponent> ent, ref MapInitEvent args)
  {
    ResPath? spawn = ent.Comp.Spawn;
    if (!spawn.HasValue)
      return;
    ResPath valueOrDefault = spawn.GetValueOrDefault();
    if (this._net.IsClient || !this._config.GetCVar<bool>(CCVars.GridFill))
      return;
    if (!this._map.HasValue)
    {
      MapId mapId;
      this._mapSystem.CreateMap(out mapId);
      this._map = new MapId?(mapId);
    }
    Vector2 vector2_1 = new Vector2((float) (this._index * 50), (float) (this._index * 50));
    ++this._index;
    if (!this._mapSystem.MapExists(this._map))
      return;
    MapLoaderSystem mapLoader = this._mapLoader;
    MapId map = this._map.Value;
    ResPath path = valueOrDefault;
    Entity<MapGridComponent>? nullable;
    ref Entity<MapGridComponent>? local = ref nullable;
    Vector2 vector2_2 = vector2_1;
    DeserializationOptions? options = new DeserializationOptions?();
    Vector2 offset = vector2_2;
    Angle rot = new Angle();
    if (!mapLoader.TryLoadGrid(map, path, out local, options, offset, rot))
      return;
    Entity<MapGridComponent> entity = nullable.Value;
    TransformComponent xform = this.Transform((EntityUid) ent);
    MapCoordinates coordinates = this._transform.GetMapCoordinates((EntityUid) ent, xform);
    coordinates = coordinates.Offset(ent.Comp.Offset);
    this._transform.SetMapCoordinates((EntityUid) entity, coordinates);
    PhysicsComponent comp1;
    FixturesComponent comp2;
    if (!this.TryComp<PhysicsComponent>((EntityUid) entity, out comp1) || !this.TryComp<FixturesComponent>((EntityUid) entity, out comp2))
      return;
    this._physics.SetBodyType((EntityUid) entity, BodyType.Static, comp2, comp1);
    this._physics.SetBodyStatus((EntityUid) entity, comp1, BodyStatus.OnGround);
    this._physics.SetFixedRotation((EntityUid) entity, true, manager: comp2, body: comp1);
  }

  private void OnEvacuationDoorBeforeOpened(
    Entity<EvacuationDoorComponent> ent,
    ref BeforeDoorOpenedEvent args)
  {
    if (args.Cancelled || !ent.Comp.Locked)
      return;
    args.Cancel();
  }

  private void OnEvacuationDoorBeforeClosed(
    Entity<EvacuationDoorComponent> ent,
    ref BeforeDoorClosedEvent args)
  {
    if (!ent.Comp.Locked)
      return;
    args.PerformCollisionCheck = false;
  }

  private void OnEvacuationDoorBeforePry(
    Entity<EvacuationDoorComponent> ent,
    ref BeforePryEvent args)
  {
    if (!ent.Comp.Locked)
      return;
    args.Cancelled = true;
  }

  private void OnEvacuationComputerExamined(
    Entity<EvacuationComputerComponent> ent,
    ref ExaminedEvent args)
  {
    int? maxMobs = ent.Comp.MaxMobs;
    if (!maxMobs.HasValue)
      return;
    int valueOrDefault = maxMobs.GetValueOrDefault();
    using (args.PushGroup("EvacuationComputerComponent"))
      args.PushMarkup($"[color=red]This pod is only rated for a maximum of {valueOrDefault} occupants! Any more may cause it to crash and burn.[/color]");
  }

  private void OnEvacuationComputerUIOpenAttempt(
    Entity<EvacuationComputerComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled || ent.Comp.Mode == EvacuationComputerMode.Ready)
      return;
    args.Cancel();
    string message;
    switch (ent.Comp.Mode)
    {
      case EvacuationComputerMode.Disabled:
        message = "Evacuation has not started.";
        break;
      case EvacuationComputerMode.Ready:
        message = "";
        break;
      case EvacuationComputerMode.Travelling:
        message = "The escape pod has already been launched!";
        break;
      case EvacuationComputerMode.Crashed:
        message = "This escape pod has crashed!";
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    this._popup.PopupClient(message, (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  private void OnEvacuationPumpExamined(Entity<EvacuationPumpComponent> ent, ref ExaminedEvent args)
  {
    if (!this.IsEvacuationInProgress())
      return;
    using (args.PushGroup("EvacuationPumpComponent"))
    {
      int evacuationProgress = this.GetEvacuationProgress();
      if (evacuationProgress < 25)
        args.PushMarkup("It looks like it barely has any fuel yet.");
      else if (evacuationProgress < 50)
        args.PushMarkup("It looks like it has accumulated some fuel.");
      else if (evacuationProgress < 75)
        args.PushMarkup("It looks like the fuel tank is a little over half full.");
      else if (evacuationProgress < 100)
        args.PushMarkup("It looks like the fuel tank is almost full.");
      else
        args.PushMarkup("It looks like the fuel tank is full.");
    }
  }

  private void OnLifeboatComputerUIOpenAttempt(
    Entity<LifeboatComputerComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled || ent.Comp.Enabled)
      return;
    args.Cancel();
    this._popup.PopupClient("Evacuation has not been authorized.", (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  private void OnEvacuationComputerLaunch(
    Entity<EvacuationComputerComponent> ent,
    ref EvacuationComputerLaunchBuiMsg args)
  {
    EntityUid actor = args.Actor;
    if (ent.Comp.Mode != EvacuationComputerMode.Ready)
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to activate evacuation computer {this.ToPrettyString(new EntityUid?((EntityUid) ent))} that is not ready. Mode: {ent.Comp.Mode}");
    }
    else
    {
      EntityUid? gridUid = this.Transform((EntityUid) ent).GridUid;
      if (gridUid.HasValue)
      {
        EntityUid valueOrDefault1 = gridUid.GetValueOrDefault();
        TransformComponent transformComponent = this.Transform(valueOrDefault1);
        int? maxMobs = ent.Comp.MaxMobs;
        if (maxMobs.HasValue)
        {
          int valueOrDefault2 = maxMobs.GetValueOrDefault();
          HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>();
          TransformChildrenEnumerator childEnumerator = transformComponent.ChildEnumerator;
          EntityUid child;
          while (childEnumerator.MoveNext(out child))
          {
            if (this._mobStateQuery.HasComp(child))
            {
              entityUidSet.Add(child);
            }
            else
            {
              ContainerManagerComponent comp;
              if (this.TryComp<ContainerManagerComponent>(child, out comp))
              {
                foreach (BaseContainer allContainer in this._container.GetAllContainers(child, comp))
                {
                  foreach (EntityUid entityUid in allContainer.ContainedEntities.Where<EntityUid>(new Func<EntityUid, bool>(this._mobStateQuery.HasComp)).ToList<EntityUid>())
                    entityUidSet.Add(entityUid);
                }
              }
            }
            DoorComponent component;
            if (this._doorQuery.TryComp(child, out component))
            {
              EvacuationDoorComponent evacuationDoorComponent = this.EnsureComp<EvacuationDoorComponent>(child);
              evacuationDoorComponent.Locked = true;
              this.Dirty(child, (IComponent) evacuationDoorComponent);
              this._door.TryClose(child, component);
            }
          }
          if (entityUidSet.Count > valueOrDefault2)
          {
            this._popup.PopupPredicted("The evacuation pod is overloaded with this many people inside!", (EntityUid) ent, new EntityUid?(), PopupType.LargeCaution);
            ent.Comp.Mode = EvacuationComputerMode.Crashed;
            this.Dirty<EvacuationComputerComponent>(ent);
            TimeSpan curTime = this._timing.CurTime;
            DetonatingEvacuationComputerComponent computerComponent = this.EnsureComp<DetonatingEvacuationComputerComponent>((EntityUid) ent);
            computerComponent.DetonateAt = curTime + ent.Comp.DetonateDelay;
            computerComponent.EjectAt = curTime + ent.Comp.EjectDelay;
          }
        }
        this._audio.PlayPredicted(ent.Comp.WarmupSound, (EntityUid) ent, new EntityUid?(actor));
        if (ent.Comp.Mode == EvacuationComputerMode.Crashed)
          return;
        ent.Comp.Mode = EvacuationComputerMode.Travelling;
        this.Dirty<EvacuationComputerComponent>(ent);
        float crashLandChance = this.IsEvacuationComplete() ? 0.0f : ent.Comp.EarlyCrashChance;
        this.LaunchEvacuationFTL(valueOrDefault1, crashLandChance, ent.Comp.LaunchSound);
      }
      else
        this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to activate evacuation computer {this.ToPrettyString(new EntityUid?((EntityUid) ent))} not on grid");
    }
  }

  private void OnLifeboatComputerLaunch(
    Entity<LifeboatComputerComponent> ent,
    ref LifeboatComputerLaunchBuiMsg args)
  {
    EntityUid actor = args.Actor;
    if (!ent.Comp.Enabled)
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to activate lifeboat computer {this.ToPrettyString(new EntityUid?((EntityUid) ent))} that is not ready.");
    }
    else
    {
      EntityUid? gridUid = this.Transform((EntityUid) ent).GridUid;
      if (!gridUid.HasValue)
        return;
      EntityUid valueOrDefault = gridUid.GetValueOrDefault();
      ent.Comp.Enabled = false;
      this.Dirty<LifeboatComputerComponent>(ent);
      float crashLandChance = this.IsEvacuationComplete() ? 0.0f : ent.Comp.EarlyCrashChance;
      this.LaunchEvacuationFTL(valueOrDefault, crashLandChance, (SoundSpecifier) null);
    }
  }

  protected virtual void LaunchEvacuationFTL(
    EntityUid grid,
    float crashLandChance,
    SoundSpecifier? launchSound)
  {
  }

  private void SetPumpAppearance(EvacuationPumpVisuals visual)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<EvacuationPumpComponent> entityQueryEnumerator = this.EntityQueryEnumerator<EvacuationPumpComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out EvacuationPumpComponent _))
      this._appearance.SetData(uid, (Enum) EvacuationPumpLayers.Layer, (object) visual);
  }

  private void SetPumpAmbience()
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<EvacuationPumpComponent> entityQueryEnumerator = this.EntityQueryEnumerator<EvacuationPumpComponent>();
    EntityUid uid;
    EvacuationPumpComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      this._ambientSound.SetSound(uid, comp1.ActiveSound);
  }

  private IEnumerable<EntityUid> GetEvacuationAreas(EntityCoordinates coordinates)
  {
    Entity<AreaGridComponent>? areaGrid;
    if (this._area.TryGetAllAreas(coordinates, out areaGrid))
    {
      foreach (EntityUid uid in areaGrid.Value.Comp.AreaEntities.Values)
      {
        AreaComponent component;
        if (this._areaQuery.TryComp(uid, out component) && component.HijackEvacuationArea)
          yield return uid;
      }
    }
  }

  private bool IsAreaPumpPowered(EntityUid area)
  {
    return this._rmcPower.IsAreaPowered((Entity<RMCAreaPowerComponent>) area, RMCPowerChannel.Equipment);
  }

  public void ToggleEvacuation(
    SoundSpecifier? startSound,
    SoundSpecifier? cancelSound,
    EntityUid? map)
  {
    EvacuationProgressComponent progressComponent = this.EnsureComp<EvacuationProgressComponent>(map.Value);
    progressComponent.Enabled = !progressComponent.Enabled;
    this.Dirty(map.Value, (IComponent) progressComponent);
    if (progressComponent.Enabled)
    {
      this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), "Attention. Emergency. All personnel must evacuate immediately.", startSound);
      EvacuationEnabledEvent args = new EvacuationEnabledEvent();
      this.RaiseLocalEvent<EvacuationEnabledEvent>(map.Value, ref args, true);
    }
    else
    {
      this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), "Evacuation has been cancelled.", cancelSound);
      EvacuationDisabledEvent args = new EvacuationDisabledEvent();
      this.RaiseLocalEvent<EvacuationDisabledEvent>(map.Value, ref args, true);
    }
  }

  public bool IsEvacuationInProgress()
  {
    return this.EntityQueryEnumerator<EvacuationProgressComponent>().MoveNext(out EvacuationProgressComponent _);
  }

  public bool IsEvacuationEnabled()
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<EvacuationProgressComponent> entityQueryEnumerator = this.EntityQueryEnumerator<EvacuationProgressComponent>();
    EvacuationProgressComponent comp1;
    while (entityQueryEnumerator.MoveNext(out comp1))
    {
      if (comp1.Enabled)
        return true;
    }
    return false;
  }

  public int GetEvacuationProgress()
  {
    EvacuationProgressComponent comp1;
    return this.EntityQueryEnumerator<EvacuationProgressComponent>().MoveNext(out comp1) ? (int) comp1.Progress : 0;
  }

  public bool IsEvacuationComplete() => this.GetEvacuationProgress() >= 100;

  private void ProcessEvacuation()
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<EvacuationProgressComponent> entityQueryEnumerator = this.EntityQueryEnumerator<EvacuationProgressComponent>();
    EntityUid uid;
    EvacuationProgressComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1) && comp1.DropShipCrashed)
    {
      if (!comp1.StartAnnounced)
      {
        comp1.StartAnnounced = true;
        this.SetPumpAppearance(EvacuationPumpVisuals.Empty);
        this.SetPumpAmbience();
        StringBuilder stringBuilder = new StringBuilder();
        foreach (EntityUid evacuationArea in this.GetEvacuationAreas(uid.ToCoordinates()))
        {
          bool flag = this.IsAreaPumpPowered(evacuationArea);
          string str = $"[{this.Name(evacuationArea)}] - [{(flag ? "Online" : "Offline")}]";
          stringBuilder.AppendLine(str);
        }
        stringBuilder.Append("Due to low orbit, extra fuel is required for non-surface evacuations.\nMaintain fueling functionality for optimal evacuation conditions.");
        this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), stringBuilder.ToString());
      }
      if (!(comp1.NextUpdate > curTime))
      {
        comp1.NextUpdate = curTime + comp1.UpdateEvery;
        this.Dirty(uid, (IComponent) comp1);
        double num1 = 0.0;
        double num2 = 1.0;
        foreach (EntityUid evacuationArea in this.GetEvacuationAreas(uid.ToCoordinates()))
        {
          AreaComponent component;
          if (this._areaQuery.TryComp(evacuationArea, out component) && component.HijackEvacuationArea)
          {
            bool flag1 = this.IsAreaPumpPowered(evacuationArea);
            bool flag2;
            if (comp1.LastPower.TryGetValue(evacuationArea, out flag2) && flag2 != flag1)
              this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), $"{this.Name(evacuationArea)} - [{(flag1 ? "Online" : "Offline")}]");
            comp1.LastPower[evacuationArea] = flag1;
            if (flag1)
            {
              switch (component.HijackEvacuationType)
              {
                case AreaHijackEvacuationType.Add:
                  num1 += component.HijackEvacuationWeight;
                  continue;
                case AreaHijackEvacuationType.Multiply:
                  num2 += component.HijackEvacuationWeight;
                  continue;
                default:
                  continue;
              }
            }
          }
        }
        comp1.Progress = Math.Min(comp1.Required, comp1.Progress + num1 * num2);
        if (comp1.Progress >= (double) comp1.NextAnnounce)
        {
          int nextAnnounce = comp1.NextAnnounce;
          comp1.NextAnnounce = nextAnnounce + comp1.AnnounceEvery;
          string str1 = string.Join(", ", comp1.LastPower.Where<KeyValuePair<EntityUid, bool>>((Func<KeyValuePair<EntityUid, bool>, bool>) (kvp => kvp.Value)).Select<KeyValuePair<EntityUid, bool>, string>((Func<KeyValuePair<EntityUid, bool>, string>) (kvp => this.Name(kvp.Key))));
          string offAreas = string.Join(", ", comp1.LastPower.Where<KeyValuePair<EntityUid, bool>>((Func<KeyValuePair<EntityUid, bool>, bool>) (kvp => !kvp.Value)).Select<KeyValuePair<EntityUid, bool>, string>((Func<KeyValuePair<EntityUid, bool>, string>) (kvp => this.Name(kvp.Key))));
          if (comp1.Progress >= comp1.Required)
          {
            this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), "Emergency fuel replenishment is at 100 percent. Safe utilization of lifeboats and pods is now possible.");
            this._xenoAnnounce.AnnounceAll(new EntityUid(), "The talls have completed their goals!");
            this.SetPumpAppearance(EvacuationPumpVisuals.Full);
            EvacuationProgressEvent args = new EvacuationProgressEvent(100);
            this.RaiseLocalEvent<EvacuationProgressEvent>(uid, ref args, true);
          }
          else if (comp1.Progress >= comp1.Required * 0.75)
          {
            this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), MarinePercentageString(75));
            string message = "The talls are three quarters of the way towards their goals.";
            if (str1.Length > 0)
              message = $"{message} Disable the following areas: {str1}";
            this._xenoAnnounce.AnnounceAll(new EntityUid(), message);
            this.SetPumpAppearance(EvacuationPumpVisuals.SeventyFive);
            EvacuationProgressEvent args = new EvacuationProgressEvent(75);
            this.RaiseLocalEvent<EvacuationProgressEvent>(uid, ref args, true);
          }
          else if (comp1.Progress >= comp1.Required * 0.5)
          {
            this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), MarinePercentageString(50));
            string message = "The talls are half way towards their goals.";
            if (str1.Length > 0)
              message = $"{message} Disable the following areas: {str1}";
            this._xenoAnnounce.AnnounceAll(new EntityUid(), message);
            this.SetPumpAppearance(EvacuationPumpVisuals.Fifty);
            EvacuationProgressEvent args = new EvacuationProgressEvent(50);
            this.RaiseLocalEvent<EvacuationProgressEvent>(uid, ref args, true);
          }
          else if (comp1.Progress >= comp1.Required * 0.25)
          {
            string str2 = "Emergency fuel replenishment is at 25 percent. Lifeboat emergency early launch is now available.";
            this._marineAnnounce.AnnounceARESStaging(new EntityUid?(), offAreas.Length != 0 ? $"{str2} To increase speed, restore power to the following areas: {offAreas}" : str2 + " All fueling areas operational.");
            string message = "The talls are a quarter of the way towards their goals.";
            if (str1.Length > 0)
              message = $"{message} Disable the following areas: {str1}";
            this._xenoAnnounce.AnnounceAll(new EntityUid(), message);
            this.SetPumpAppearance(EvacuationPumpVisuals.TwentyFive);
            EvacuationProgressEvent args = new EvacuationProgressEvent(25);
            this.RaiseLocalEvent<EvacuationProgressEvent>(uid, ref args, true);
          }

          string MarinePercentageString(int percentage)
          {
            string str = $"Emergency fuel replenishment is at {percentage} percent.";
            return offAreas.Length != 0 ? $"{str}To increase speed, restore power to the following areas: {offAreas}" : str + " All fueling areas operational.";
          }
        }
      }
    }
  }

  private void ProcessExplodingPods()
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<DetonatingEvacuationComputerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DetonatingEvacuationComputerComponent>();
    EntityUid uid;
    DetonatingEvacuationComputerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? gridUid = this.Transform(uid).GridUid;
      if (gridUid.HasValue)
      {
        EntityUid valueOrDefault = gridUid.GetValueOrDefault();
        if (this.TryComp<MapGridComponent>(valueOrDefault, out MapGridComponent _))
        {
          TransformComponent transformComponent = this.Transform(valueOrDefault);
          if (!comp1.Detonated && curTime >= comp1.DetonateAt)
          {
            comp1.Detonated = true;
            this.Dirty(uid, (IComponent) comp1);
            this._rmcExplosion.QueueExplosion(this._transform.ToMapCoordinates(transformComponent.Coordinates), "RMC", 40f, 5f, 25f, new EntityUid?(uid), canCreateVacuum: false);
          }
          if (!comp1.Ejected && curTime >= comp1.EjectAt)
          {
            comp1.Ejected = true;
            this.Dirty(uid, (IComponent) comp1);
            TransformChildrenEnumerator childEnumerator = transformComponent.ChildEnumerator;
            EntityUid child;
            while (childEnumerator.MoveNext(out child))
            {
              this._hyperSleep.EjectChamber((Entity<HyperSleepChamberComponent>) child);
              DoorComponent component;
              if (this._doorQuery.TryComp(child, out component))
              {
                EvacuationDoorComponent evacuationDoorComponent = this.EnsureComp<EvacuationDoorComponent>(child);
                evacuationDoorComponent.Locked = false;
                this.Dirty(child, (IComponent) evacuationDoorComponent);
                this._door.SetState(child, DoorState.Emagging, component);
              }
            }
          }
          if (comp1.Detonated && comp1.Ejected)
            this.RemCompDeferred<DetonatingEvacuationComputerComponent>(uid);
        }
      }
    }
  }

  public override void Update(float frameTime)
  {
    this.ProcessEvacuation();
    this.ProcessExplodingPods();
  }
}
