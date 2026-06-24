// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.ElectronicSystem.SharedDropshipElectronicSystemSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Camera;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.PowerLoader;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Dropship.ElectronicSystem;

public abstract class SharedDropshipElectronicSystemSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDropshipSystem _dropship;
  [Dependency]
  private SharedRMCCameraSystem _rmcCamera;
  private const int MinSpread = 0;
  private const int MinBulletSpread = 1;
  private const float MinTravelTime = 1f;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DropshipComponent, DropshipWeaponShotEvent>(new EntityEventRefHandler<DropshipComponent, DropshipWeaponShotEvent>(this.OnDropshipWeaponShot));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, DropShipAttachmentInsertedEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, DropShipAttachmentInsertedEvent>(this.OnDropShipAttachmentInserted));
    this.SubscribeLocalEvent<DropshipElectronicSystemPointComponent, DropShipAttachmentDetachedEvent>(new EntityEventRefHandler<DropshipElectronicSystemPointComponent, DropShipAttachmentDetachedEvent>(this.OnDropShipAttachmentDetached));
  }

  private void OnDropshipWeaponShot(Entity<DropshipComponent> ent, ref DropshipWeaponShotEvent args)
  {
    foreach (EntityUid attachmentPoint in ent.Comp.AttachmentPoints)
    {
      DropshipElectronicSystemPointComponent comp1;
      BaseContainer container;
      if (this.TryComp<DropshipElectronicSystemPointComponent>(attachmentPoint, out comp1) && this._container.TryGetContainer(attachmentPoint, comp1.ContainerId, out container))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
        {
          DropshipTargetingSystemComponent comp2;
          if (this.TryComp<DropshipTargetingSystemComponent>(containedEntity, out comp2))
          {
            args.Spread = Math.Max(0.0f, args.Spread + (float) comp2.SpreadModifier);
            args.BulletSpread = Math.Max(1, args.BulletSpread + comp2.BulletSpreadModifier);
            args.TravelTime = TimeSpan.FromSeconds(Math.Max(1.0, args.TravelTime.TotalSeconds + comp2.TravelingTimeModifier.TotalSeconds));
          }
        }
      }
    }
  }

  protected virtual void OnDropShipAttachmentInserted(
    Entity<DropshipElectronicSystemPointComponent> ent,
    ref DropShipAttachmentInsertedEvent args)
  {
    Entity<DropshipComponent> dropship;
    CameraSignalGranterComponent comp;
    if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship) || !this.TryComp<CameraSignalGranterComponent>(args.Inserted, out comp))
      return;
    this.ModifyCameraSignals((Entity<CameraSignalGranterComponent>) (args.Inserted, comp), dropship);
  }

  protected virtual void OnDropShipAttachmentDetached(
    Entity<DropshipElectronicSystemPointComponent> ent,
    ref DropShipAttachmentDetachedEvent args)
  {
    Entity<DropshipComponent> dropship;
    if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship))
      return;
    CameraSignalGranterComponent comp1;
    if (this.TryComp<CameraSignalGranterComponent>(args.Detached, out comp1))
      this.ModifyCameraSignals((Entity<CameraSignalGranterComponent>) (args.Detached, comp1), dropship, true);
    DropshipSpotlightComponent comp2;
    if (!this.TryComp<DropshipSpotlightComponent>(args.Detached, out comp2))
      return;
    comp2.Enabled = false;
    this.Dirty(args.Detached, (IComponent) comp2);
  }

  private void ModifyCameraSignals(
    Entity<CameraSignalGranterComponent> ent,
    Entity<DropshipComponent> dropship,
    bool remove = false)
  {
    TransformChildrenEnumerator childEnumerator = this.Transform((EntityUid) dropship).ChildEnumerator;
    EntityUid child;
    while (childEnumerator.MoveNext(out child))
    {
      RMCCameraComputerComponent comp;
      if (this.TryComp<RMCCameraComputerComponent>(child, out comp))
      {
        foreach (EntProtoId protoId in ent.Comp.ProtoIds)
        {
          if (remove)
            this._rmcCamera.RemoveProtoId(comp, protoId);
          else
            this._rmcCamera.AddProtoId(comp, protoId);
          this._rmcCamera.RefreshCameras(protoId);
        }
        this.Dirty(child, (IComponent) comp);
      }
    }
  }
}
