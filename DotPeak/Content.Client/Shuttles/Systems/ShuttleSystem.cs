// Decompiled with JetBrains decompiler
// Type: Content.Client.Shuttles.Systems.ShuttleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Events;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Shuttles.UI.MapObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using System;

#nullable enable
namespace Content.Client.Shuttles.Systems;

public sealed class ShuttleSystem : SharedShuttleSystem
{
  [Dependency]
  private IResourceCache _resource;
  [Dependency]
  private IOverlayManager _overlays;
  private bool _enableShuttlePosition;
  private EmergencyShuttleOverlay? _overlay;

  public Texture GetTexture(Entity<ShuttleMapParallaxComponent?> entity)
  {
    return !this.Resolve<ShuttleMapParallaxComponent>(Entity<ShuttleMapParallaxComponent>.op_Implicit(entity), ref entity.Comp, false) ? this._resource.GetTexture(ShuttleMapParallaxComponent.FallbackTexture) : this._resource.GetTexture(entity.Comp.TexturePath);
  }

  public MapCoordinates GetMapCoordinates(IMapObject mapObj)
  {
    switch (mapObj)
    {
      case ShuttleBeaconObject shuttleBeaconObject:
        return this.XformSystem.ToMapCoordinates(this.GetCoordinates(shuttleBeaconObject.Coordinates), true);
      case ShuttleExclusionObject shuttleExclusionObject:
        return this.XformSystem.ToMapCoordinates(this.GetCoordinates(shuttleExclusionObject.Coordinates), true);
      case GridMapObject gridMapObject:
        TransformComponent transformComponent = this.Transform(gridMapObject.Entity);
        return this.HasComp<MapComponent>(gridMapObject.Entity) ? new MapCoordinates(transformComponent.LocalPosition, transformComponent.MapID) : new MapCoordinates(this.Maps.GetGridPosition(Entity<PhysicsComponent, TransformComponent>.op_Implicit((gridMapObject.Entity, (PhysicsComponent) null, transformComponent))), transformComponent.MapID);
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  public override void Initialize()
  {
    base.Initialize();
    this.InitializeEmergency();
    this._overlays.AddOverlay((Overlay) new FtlArrivalOverlay());
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._overlays.RemoveOverlay<FtlArrivalOverlay>();
  }

  public bool EnableShuttlePosition
  {
    get => this._enableShuttlePosition;
    set
    {
      if (this._enableShuttlePosition == value)
        return;
      this._enableShuttlePosition = value;
      IOverlayManager ioverlayManager = IoCManager.Resolve<IOverlayManager>();
      if (this._enableShuttlePosition)
      {
        this._overlay = new EmergencyShuttleOverlay(this.EntityManager.TransformQuery, this.XformSystem);
        ioverlayManager.AddOverlay((Overlay) this._overlay);
        this.RaiseNetworkEvent((EntityEventArgs) new EmergencyShuttleRequestPositionMessage());
      }
      else
      {
        ioverlayManager.RemoveOverlay((Overlay) this._overlay);
        this._overlay = (EmergencyShuttleOverlay) null;
      }
    }
  }

  private void InitializeEmergency()
  {
    this.SubscribeNetworkEvent<EmergencyShuttlePositionMessage>(new EntityEventHandler<EmergencyShuttlePositionMessage>(this.OnShuttlePosMessage), (Type[]) null, (Type[]) null);
  }

  private void OnShuttlePosMessage(EmergencyShuttlePositionMessage ev)
  {
    if (this._overlay == null)
      return;
    this._overlay.StationUid = this.GetEntity(ev.StationUid);
    this._overlay.Position = ev.Position;
  }
}
