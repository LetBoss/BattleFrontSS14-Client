// Decompiled with JetBrains decompiler
// Type: Content.Client.NetworkConfigurator.NetworkConfiguratorLinkOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.NetworkConfigurator.Systems;
using Content.Shared.DeviceNetwork.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.NetworkConfigurator;

public sealed class NetworkConfiguratorLinkOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IRobustRandom _random;
  private readonly DeviceListSystem _deviceListSystem;
  private readonly SharedTransformSystem _transformSystem;
  public Dictionary<EntityUid, Color> Colors = new Dictionary<EntityUid, Color>();
  public EntityUid? Action;

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public NetworkConfiguratorLinkOverlay()
  {
    IoCManager.InjectDependencies<NetworkConfiguratorLinkOverlay>(this);
    this._deviceListSystem = this._entityManager.System<DeviceListSystem>();
    this._transformSystem = this._entityManager.System<SharedTransformSystem>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    EntityQueryEnumerator<NetworkConfiguratorActiveLinkOverlayComponent> entityQueryEnumerator = this._entityManager.EntityQueryEnumerator<NetworkConfiguratorActiveLinkOverlayComponent>();
    EntityUid entityUid;
    NetworkConfiguratorActiveLinkOverlayComponent overlayComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref overlayComponent))
    {
      DeviceListComponent component1;
      if (this._entityManager.Deleted(entityUid) || !this._entityManager.TryGetComponent<DeviceListComponent>(entityUid, ref component1))
      {
        this._entityManager.RemoveComponentDeferred<NetworkConfiguratorActiveLinkOverlayComponent>(entityUid);
      }
      else
      {
        Color color;
        if (!this.Colors.TryGetValue(entityUid, out color))
        {
          // ISSUE: explicit constructor call
          ((Color) ref color).\u002Ector((float) this._random.Next(0, (int) byte.MaxValue), (float) this._random.Next(0, (int) byte.MaxValue), (float) this._random.Next(0, (int) byte.MaxValue), 1f);
          this.Colors.Add(entityUid, color);
        }
        TransformComponent component2 = this._entityManager.GetComponent<TransformComponent>(entityUid);
        if (!MapId.op_Equality(component2.MapID, MapId.Nullspace))
        {
          foreach (EntityUid allDevice in this._deviceListSystem.GetAllDevices(entityUid, component1))
          {
            if (!this._entityManager.Deleted(allDevice))
            {
              TransformComponent component3 = this._entityManager.GetComponent<TransformComponent>(allDevice);
              if (!MapId.op_Equality(component3.MapID, MapId.Nullspace))
                ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).DrawLine(this._transformSystem.GetWorldPosition(component2), this._transformSystem.GetWorldPosition(component3), this.Colors[entityUid]);
            }
          }
        }
      }
    }
  }
}
