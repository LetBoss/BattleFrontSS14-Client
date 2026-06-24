// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Camera.SharedRMCCameraSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Camera;

public abstract class SharedRMCCameraSystem : EntitySystem
{
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  private readonly HashSet<EntProtoId> _refresh = new HashSet<EntProtoId>();
  private readonly Dictionary<string, int> _cameraNames = new Dictionary<string, int>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<RMCCameraComponent, MapInitEvent>(new EntityEventRefHandler<RMCCameraComponent, MapInitEvent>(this.OnCameraMapInit), after: new Type[1]
    {
      typeof (AreaSystem)
    });
    this.SubscribeLocalEvent<RMCCameraComponent, ComponentRemove>(new EntityEventRefHandler<RMCCameraComponent, ComponentRemove>(this.OnCameraRemove));
    this.SubscribeLocalEvent<RMCCameraComponent, EntityTerminatingEvent>(new EntityEventRefHandler<RMCCameraComponent, EntityTerminatingEvent>(this.OnCameraTerminating));
    this.SubscribeLocalEvent<RMCCameraComputerComponent, MapInitEvent>(new EntityEventRefHandler<RMCCameraComputerComponent, MapInitEvent>(this.OnComputerMapInit), after: new Type[1]
    {
      typeof (AreaSystem)
    });
    this.SubscribeLocalEvent<RMCCameraWatcherComponent, ComponentRemove>(new EntityEventRefHandler<RMCCameraWatcherComponent, ComponentRemove>(this.OnWatcherRemove));
    this.SubscribeLocalEvent<RMCCameraWatcherComponent, EntityTerminatingEvent>(new EntityEventRefHandler<RMCCameraWatcherComponent, EntityTerminatingEvent>(this.OnWatcherTerminating));
    this.Subs.BuiEvents<RMCCameraComputerComponent>((object) RMCCameraUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<RMCCameraComputerComponent>) (subs =>
    {
      subs.Event<BoundUIOpenedEvent>(new EntityEventRefHandler<RMCCameraComputerComponent, BoundUIOpenedEvent>(this.OnComputerBuiOpened));
      subs.Event<BoundUIClosedEvent>(new EntityEventRefHandler<RMCCameraComputerComponent, BoundUIClosedEvent>(this.OnComputerBuiClosed));
      subs.Event<RMCCameraWatchBuiMsg>(new EntityEventRefHandler<RMCCameraComputerComponent, RMCCameraWatchBuiMsg>(this.OnComputerWatchBuiMsg));
      subs.Event<RMCCameraPreviousBuiMsg>(new EntityEventRefHandler<RMCCameraComputerComponent, RMCCameraPreviousBuiMsg>(this.OnComputerPreviousBuiMsg));
      subs.Event<RMCCameraNextBuiMsg>(new EntityEventRefHandler<RMCCameraComputerComponent, RMCCameraNextBuiMsg>(this.OnComputerNextBuiMsg));
    }));
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev) => this._cameraNames.Clear();

  private void OnCameraMapInit(Entity<RMCCameraComponent> ent, ref MapInitEvent args)
  {
    EntProtoId? id = ent.Comp.Id;
    if (id.HasValue)
      this._refresh.Add(id.GetValueOrDefault());
    if (ent.Comp.Rename)
    {
      EntityPrototype areaPrototype;
      if (!this._area.TryGetArea((EntityUid) ent, out Entity<AreaComponent>? _, out areaPrototype))
        return;
      string name = areaPrototype.Name;
      int valueOrDefault = this._cameraNames.GetValueOrDefault<string, int>(name);
      int num;
      this._metaData.SetEntityName((EntityUid) ent, $"{name} #{num = valueOrDefault + 1}");
      this._cameraNames[name] = num;
    }
    else
    {
      string key = this.Name((EntityUid) ent);
      int valueOrDefault = this._cameraNames.GetValueOrDefault<string, int>(key);
      this._cameraNames[key] = valueOrDefault;
    }
  }

  private void OnCameraRemove(Entity<RMCCameraComponent> ent, ref ComponentRemove args)
  {
    this.OnCameraRemoved(ent);
  }

  private void OnCameraTerminating(Entity<RMCCameraComponent> ent, ref EntityTerminatingEvent args)
  {
    this.OnCameraRemoved(ent);
  }

  private void OnComputerMapInit(Entity<RMCCameraComputerComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.CameraIds.Clear();
    ent.Comp.CameraNames.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCCameraComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCCameraComponent>();
    EntityUid uid;
    RMCCameraComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      foreach (EntProtoId protoId in ent.Comp.ProtoIds)
      {
        EntProtoId? id = comp1.Id;
        EntProtoId entProtoId = protoId;
        if ((id.HasValue ? (id.GetValueOrDefault() != entProtoId ? 1 : 0) : 1) == 0)
        {
          ent.Comp.CameraIds.Add(this.GetNetEntity(uid));
          ent.Comp.CameraNames.Add(this.Name(uid));
        }
      }
    }
    this.Dirty<RMCCameraComputerComponent>(ent);
  }

  private void OnWatcherRemove(Entity<RMCCameraWatcherComponent> ent, ref ComponentRemove args)
  {
    this.OnWatcherRemoved(ent);
  }

  private void OnWatcherTerminating(
    Entity<RMCCameraWatcherComponent> ent,
    ref EntityTerminatingEvent args)
  {
    this.OnWatcherRemoved(ent);
  }

  private void OnComputerBuiOpened(
    Entity<RMCCameraComputerComponent> ent,
    ref BoundUIOpenedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    EntityUid actor = args.Actor;
    ent.Comp.Watchers.Add(actor);
    this.Dirty<RMCCameraComputerComponent>(ent);
    RMCCameraWatcherComponent watcherComponent = this.EnsureComp<RMCCameraWatcherComponent>(actor);
    watcherComponent.Computer = new EntityUid?();
    this.Dirty(actor, (IComponent) watcherComponent);
    this.Refresh(ent, new EntityUid?());
  }

  private void OnComputerBuiClosed(
    Entity<RMCCameraComputerComponent> ent,
    ref BoundUIClosedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    EntityUid actor = args.Actor;
    ent.Comp.Watchers.Remove(actor);
    this.Dirty<RMCCameraComputerComponent>(ent);
    this.RemCompDeferred<RMCCameraWatcherComponent>(actor);
  }

  private void OnComputerWatchBuiMsg(
    Entity<RMCCameraComputerComponent> ent,
    ref RMCCameraWatchBuiMsg args)
  {
    EntityUid? entity;
    if (this._timing.ApplyingState || !this.TryGetEntity(args.Camera, out entity) || !ent.Comp.CameraIds.Contains(args.Camera))
      return;
    EntityUid? currentCamera = ent.Comp.CurrentCamera;
    ent.Comp.CurrentCamera = entity;
    this.Refresh(ent, currentCamera);
  }

  private void OnComputerPreviousBuiMsg(
    Entity<RMCCameraComputerComponent> ent,
    ref RMCCameraPreviousBuiMsg args)
  {
    EntityUid? currentCamera = ent.Comp.CurrentCamera;
    int index = 0;
    NetEntity? netEntity;
    if (currentCamera.HasValue && this.TryGetNetEntity(currentCamera, out netEntity))
    {
      index = ent.Comp.CameraIds.IndexOf(netEntity.Value) - 1;
      if (index < 0 || index >= ent.Comp.CameraIds.Count)
        index = ent.Comp.CameraIds.Count - 1;
    }
    EntityUid? entity;
    if (index >= 0 && index < ent.Comp.CameraIds.Count && this.TryGetEntity(ent.Comp.CameraIds[index], out entity))
      ent.Comp.CurrentCamera = entity;
    this.Refresh(ent, currentCamera);
  }

  private void OnComputerNextBuiMsg(
    Entity<RMCCameraComputerComponent> ent,
    ref RMCCameraNextBuiMsg args)
  {
    EntityUid? currentCamera = ent.Comp.CurrentCamera;
    int index = 0;
    NetEntity? netEntity;
    if (currentCamera.HasValue && this.TryGetNetEntity(currentCamera, out netEntity))
    {
      index = ent.Comp.CameraIds.IndexOf(netEntity.Value) + 1;
      if (index < 0 || index >= ent.Comp.CameraIds.Count)
        index = 0;
    }
    EntityUid? entity;
    if (index >= 0 && index < ent.Comp.CameraIds.Count && this.TryGetEntity(ent.Comp.CameraIds[index], out entity))
      ent.Comp.CurrentCamera = entity;
    this.Refresh(ent, currentCamera);
  }

  protected virtual void Refresh(Entity<RMCCameraComputerComponent> ent, EntityUid? old)
  {
    this.Dirty<RMCCameraComputerComponent>(ent);
  }

  protected virtual void OnWatcherRemoved(Entity<RMCCameraWatcherComponent> watcher)
  {
    RMCCameraComputerComponent comp;
    if (!this.TryComp<RMCCameraComputerComponent>(watcher.Comp.Computer, out comp))
      return;
    comp.Watchers.Remove((EntityUid) watcher);
    this.Dirty(watcher.Comp.Computer.Value, (IComponent) comp);
  }

  public bool GetComputerCameraName(
    Entity<RMCCameraComputerComponent> computer,
    EntityUid camera,
    [NotNullWhen(true)] out string? name)
  {
    int index = computer.Comp.CameraIds.IndexOf(this.GetNetEntity(camera));
    if (index < 0)
    {
      name = (string) null;
      return false;
    }
    name = computer.Comp.CameraNames[index];
    return true;
  }

  private void OnCameraRemoved(Entity<RMCCameraComponent> camera)
  {
    NetEntity netEntity = this.GetNetEntity((EntityUid) camera);
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCCameraComputerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCCameraComputerComponent>();
    EntityUid uid;
    RMCCameraComputerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      foreach (EntProtoId protoId in comp1.ProtoIds)
      {
        EntProtoId? id = camera.Comp.Id;
        if ((id.HasValue ? (protoId != id.GetValueOrDefault() ? 1 : 0) : 1) == 0 && !this.TerminatingOrDeleted(uid))
        {
          int index = comp1.CameraIds.IndexOf(netEntity);
          if (index >= 0)
          {
            comp1.CameraIds.RemoveAt(index);
            comp1.CameraNames.RemoveAt(index);
          }
          EntityUid? currentCamera = comp1.CurrentCamera;
          EntityUid entityUid = (EntityUid) camera;
          if ((currentCamera.HasValue ? (currentCamera.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
            comp1.CurrentCamera = new EntityUid?();
          this.Dirty(uid, (IComponent) comp1);
        }
      }
    }
  }

  public void AddProtoId(RMCCameraComputerComponent computer, EntProtoId protoId)
  {
    computer.ProtoIds.Add(protoId);
  }

  public void RemoveProtoId(RMCCameraComputerComponent computer, EntProtoId protoId)
  {
    computer.ProtoIds.Remove(protoId);
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCCameraComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCCameraComponent>();
    EntityUid uid;
    RMCCameraComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntProtoId? id = comp1.Id;
      EntProtoId entProtoId = protoId;
      if ((id.HasValue ? (id.GetValueOrDefault() != entProtoId ? 1 : 0) : 1) == 0)
      {
        computer.CameraIds.Remove(this.GetNetEntity(uid));
        computer.CameraNames.Remove(this.Name(uid));
      }
    }
  }

  public void RefreshCameras(EntProtoId protoId) => this._refresh.Add(protoId);

  public override void Update(float frameTime)
  {
    if (this._refresh.Count == 0)
      return;
    if (this._net.IsClient)
    {
      this._refresh.Clear();
    }
    else
    {
      HashSet<Entity<RMCCameraComputerComponent>> entitySet = new HashSet<Entity<RMCCameraComputerComponent>>();
      foreach (EntProtoId entProtoId1 in this._refresh)
      {
        entitySet.Clear();
        Robust.Shared.GameObjects.EntityQueryEnumerator<RMCCameraComputerComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RMCCameraComputerComponent>();
        EntityUid uid1;
        RMCCameraComputerComponent comp1_1;
        while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
        {
          foreach (EntProtoId protoId in comp1_1.ProtoIds)
          {
            if (protoId == entProtoId1)
              entitySet.Add((Entity<RMCCameraComputerComponent>) (uid1, comp1_1));
          }
        }
        if (entitySet.Count != 0)
        {
          List<NetEntity> netEntityList = new List<NetEntity>();
          List<string> stringList = new List<string>();
          Robust.Shared.GameObjects.EntityQueryEnumerator<RMCCameraComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCCameraComponent>();
          EntityUid uid2;
          RMCCameraComponent comp1_2;
          while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
          {
            EntProtoId? id = comp1_2.Id;
            EntProtoId entProtoId2 = entProtoId1;
            if ((id.HasValue ? (id.GetValueOrDefault() != entProtoId2 ? 1 : 0) : 1) == 0)
            {
              netEntityList.Add(this.GetNetEntity(uid2));
              stringList.Add(this.Name(uid2));
            }
          }
          foreach (Entity<RMCCameraComputerComponent> ent in entitySet)
          {
            foreach (NetEntity netEntity in netEntityList)
            {
              if (!ent.Comp.CameraIds.Contains(netEntity))
                ent.Comp.CameraIds.Add(netEntity);
            }
            foreach (string str in stringList)
            {
              if (!ent.Comp.CameraNames.Contains(str))
                ent.Comp.CameraNames.Add(str);
            }
            this.Dirty<RMCCameraComputerComponent>(ent);
          }
        }
      }
      this._refresh.Clear();
    }
  }
}
