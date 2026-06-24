// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.AlertLevel.RMCAlertLevelSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.ARES;
using Content.Shared._RMC14.Doors;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Ghost;
using Content.Shared.Lock;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.AlertLevel;

public sealed class RMCAlertLevelSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private ARESSystem _ares;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoorSystem _door;
  [Dependency]
  private SharedEntityStorageSystem _entityStorage;
  [Dependency]
  private LockSystem _lock;
  [Dependency]
  private SharedMarineAnnounceSystem _marineAnnounce;
  [Dependency]
  private INetManager _net;
  private Robust.Shared.GameObjects.EntityQuery<GhostComponent> _ghostQuery;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DropshipHijackLandedEvent>(new EntityEventRefHandler<DropshipHijackLandedEvent>(this.OnDropshipHijackLanded));
    this._ghostQuery = this.GetEntityQuery<GhostComponent>();
  }

  private void OnDropshipHijackLanded(ref DropshipHijackLandedEvent ev)
  {
  }

  private bool TryGetAlertLevel(out Entity<RMCAlertLevelComponent> alert)
  {
    EntityUid uid;
    RMCAlertLevelComponent comp1;
    if (this.EntityQueryEnumerator<RMCAlertLevelComponent>().MoveNext(out uid, out comp1))
    {
      alert = (Entity<RMCAlertLevelComponent>) (uid, comp1);
      return true;
    }
    alert = new Entity<RMCAlertLevelComponent>();
    return false;
  }

  private Entity<RMCAlertLevelComponent> EnsureAlertLevel()
  {
    Entity<RMCAlertLevelComponent> alert;
    if (this.TryGetAlertLevel(out alert))
      return alert;
    EntityUid uid = this.Spawn();
    RMCAlertLevelComponent alertLevelComponent = this.EnsureComp<RMCAlertLevelComponent>(uid);
    return (Entity<RMCAlertLevelComponent>) (uid, alertLevelComponent);
  }

  public RMCAlertLevels? Get()
  {
    Entity<RMCAlertLevelComponent> alert;
    return !this.TryGetAlertLevel(out alert) ? new RMCAlertLevels?() : new RMCAlertLevels?(alert.Comp.Level);
  }

  public bool IsRedOrDeltaAlert()
  {
    return this.Get().GetValueOrDefault() == RMCAlertLevels.Red || this.Get().GetValueOrDefault() == RMCAlertLevels.Delta;
  }

  public void Set(RMCAlertLevels level, EntityUid? user, bool playSound = true, bool sendAnnouncement = true)
  {
    Entity<RMCAlertLevelComponent> entity1 = this.EnsureAlertLevel();
    if (entity1.Comp.Level == level)
      return;
    (SoundSpecifier, LocId?, LocId?) valueTuple;
    switch (level)
    {
      case RMCAlertLevels.Green:
        valueTuple = (entity1.Comp.GreenSound, entity1.Comp.GreenMessage, new LocId?());
        break;
      case RMCAlertLevels.Blue:
        if (entity1.Comp.Level < RMCAlertLevels.Blue)
        {
          valueTuple = (entity1.Comp.BlueElevatedSound, entity1.Comp.BlueElevatedMessage, new LocId?());
          break;
        }
        if (entity1.Comp.Level > RMCAlertLevels.Blue)
        {
          valueTuple = (entity1.Comp.BlueLoweredSound, entity1.Comp.BlueLoweredMessage, new LocId?());
          break;
        }
        goto default;
      case RMCAlertLevels.Red:
        if (entity1.Comp.Level < RMCAlertLevels.Red)
        {
          valueTuple = (entity1.Comp.RedElevatedSound, entity1.Comp.RedElevatedMessage, new LocId?());
          break;
        }
        if (entity1.Comp.Level > RMCAlertLevels.Red)
        {
          valueTuple = (entity1.Comp.RedLoweredSound, entity1.Comp.RedLoweredMessage, new LocId?());
          break;
        }
        goto default;
      case RMCAlertLevels.Delta:
        valueTuple = (entity1.Comp.DeltaSound, entity1.Comp.DeltaAnnouncement, entity1.Comp.DeltaAnnouncement);
        break;
      default:
        valueTuple = ((SoundSpecifier) null, new LocId?(), new LocId?());
        break;
    }
    (SoundSpecifier sound, LocId? nullable1, LocId? nullable2) = valueTuple;
    entity1.Comp.Level = level;
    this.Dirty<RMCAlertLevelComponent>(entity1);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(20, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" set alert level to ");
    logStringHandler.AppendFormatted<RMCAlertLevels>(level, nameof (level));
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCAlertLevel, ref local);
    HashSet<EntityUid> almayers = new HashSet<EntityUid>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<AlmayerComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<AlmayerComponent>();
    EntityUid uid1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out AlmayerComponent _))
      almayers.Add(uid1);
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> transformQuery = this.EntityManager.TransformQuery;
    Filter playerFilter = Filter.Empty().AddWhereAttachedEntity((Predicate<EntityUid>) (entity =>
    {
      EntityUid? mapUid = (EntityUid?) transformQuery.CompOrNull(entity)?.MapUid;
      return mapUid.HasValue && almayers.Contains(mapUid.GetValueOrDefault()) || this._ghostQuery.HasComp(entity);
    }));
    if (playSound && this._net.IsServer)
      this._audio.PlayGlobal(sound, playerFilter, true);
    if (sendAnnouncement)
    {
      if (nullable2.HasValue)
      {
        SharedMarineAnnounceSystem marineAnnounce = this._marineAnnounce;
        ILocalizationManager loc = this.Loc;
        LocId? nullable3 = nullable2;
        string valueOrDefault = nullable3.HasValue ? (string) nullable3.GetValueOrDefault() : (string) null;
        string message = loc.GetString(valueOrDefault);
        marineAnnounce.AnnounceToMarines(message);
      }
      else if (nullable1.HasValue)
        this._marineAnnounce.AnnounceRadio((EntityUid) this._ares.EnsureARES(), this.Loc.GetString((string) nullable1.Value), entity1.Comp.RadioChannel);
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCUnlockOnAlertLevelComponent, LockComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCUnlockOnAlertLevelComponent, LockComponent>();
    EntityUid uid2;
    RMCUnlockOnAlertLevelComponent comp1_1;
    LockComponent comp2_1;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_1, out comp2_1))
    {
      if (comp1_1.Level <= level)
      {
        this._lock.Unlock(uid2, new EntityUid?(), comp2_1);
      }
      else
      {
        SharedEntityStorageComponent component = (SharedEntityStorageComponent) null;
        if (this._entityStorage.ResolveStorage(uid2, ref component))
          this._entityStorage.CloseStorage(uid2, component);
        this._lock.Lock(uid2, new EntityUid?(), comp2_1);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCOpenOnAlertLevelComponent, DoorComponent, RMCPodDoorComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<RMCOpenOnAlertLevelComponent, DoorComponent, RMCPodDoorComponent>();
    EntityUid uid3;
    RMCOpenOnAlertLevelComponent comp1_2;
    DoorComponent comp2_2;
    RMCPodDoorComponent comp3;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_2, out comp2_2, out comp3))
    {
      if (!(comp1_2.Id != comp3.Id))
      {
        if (comp1_2.Level <= level)
          this._door.TryOpen(uid3, comp2_2);
        else
          this._door.TryClose(uid3, comp2_2);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCAlertLevelDisplayComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<RMCAlertLevelDisplayComponent>();
    EntityUid uid4;
    while (entityQueryEnumerator4.MoveNext(out uid4, out RMCAlertLevelDisplayComponent _))
      this._appearance.SetData(uid4, (Enum) RMCAlertLevelsVisuals.Alert, (object) level);
    RMCAlertLevelChangedEvent args = new RMCAlertLevelChangedEvent(entity1.Comp.Level);
    this.RaiseLocalEvent<RMCAlertLevelChangedEvent>((EntityUid) entity1, ref args, true);
  }
}
