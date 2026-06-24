// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.VehicleTurretVisualSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleTurretVisualSystem : EntitySystem
{
  private const float PixelsPerMeter = 32f;
  [Dependency]
  private readonly SharedContainerSystem _container;
  [Dependency]
  private readonly IEyeManager _eye;
  [Dependency]
  private readonly SharedTransformSystem _transform;

  public virtual void Initialize()
  {
    this.UpdatesAfter.Add(typeof (VehicleTurretSystem));
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VehicleTurretVisualComponent, ComponentInit>(new EntityEventRefHandler<VehicleTurretVisualComponent, ComponentInit>((object) this, __methodptr(OnVisualInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<VehicleTurretVisualComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<VehicleTurretVisualComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnVisualState)), (Type[]) null, (Type[]) null);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    EntityQueryEnumerator<VehicleTurretVisualComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VehicleTurretVisualComponent>();
    EntityUid entityUid;
    VehicleTurretVisualComponent turretVisualComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref turretVisualComponent))
    {
      EntityUid? nullable;
      VehicleTurretComponent turret;
      Vector2 localOffset;
      Angle localRotation;
      if (this.TryGetEntity(turretVisualComponent.Turret, ref nullable) && this.TryComp<VehicleTurretComponent>(nullable, ref turret) && this.TryComputeRenderedTransform(nullable.Value, turret, out EntityUid _, out Angle _, out localOffset, out localRotation))
      {
        TransformComponent transformComponent = this.Transform(entityUid);
        transformComponent.ActivelyLerping = false;
        this._transform.SetLocalRotationNoLerp(entityUid, localRotation, transformComponent);
        this._transform.SetLocalPositionNoLerp(entityUid, localOffset, transformComponent);
      }
    }
  }

  public bool TryGetRenderedPose(
    EntityUid turretUid,
    out EntityCoordinates origin,
    out Angle worldRotation)
  {
    origin = new EntityCoordinates();
    worldRotation = Angle.Zero;
    VehicleTurretComponent turret;
    EntityUid vehicle;
    Angle vehicleRot;
    Vector2 localOffset;
    Angle localRotation;
    if (!this.TryComp<VehicleTurretComponent>(turretUid, ref turret) || !this.TryComputeRenderedTransform(turretUid, turret, out vehicle, out vehicleRot, out localOffset, out localRotation))
      return false;
    origin = this._transform.GetMoverCoordinates(new EntityCoordinates(vehicle, localOffset));
    ref Angle local = ref worldRotation;
    Angle angle1 = Angle.op_Addition(vehicleRot, localRotation);
    Angle angle2 = ((Angle) ref angle1).Reduced();
    local = angle2;
    return true;
  }

  private void OnVisualInit(Entity<VehicleTurretVisualComponent> ent, ref ComponentInit args)
  {
    this.UpdateVisual(ent);
  }

  private void OnVisualState(
    Entity<VehicleTurretVisualComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateVisual(ent);
  }

  private void UpdateVisual(Entity<VehicleTurretVisualComponent> ent)
  {
    SpriteComponent sprite;
    EntityUid? nullable;
    if (!this.TryComp<SpriteComponent>(ent.Owner, ref sprite) || !this.TryGetEntity(ent.Comp.Turret, ref nullable))
      return;
    VehicleTurretComponent vehicleTurretComponent;
    if (this.TryComp<VehicleTurretComponent>(nullable, ref vehicleTurretComponent) && !string.IsNullOrWhiteSpace(vehicleTurretComponent.OverlayState))
    {
      this.SetOverlayDepth(nullable.Value, sprite);
      string overlayState = vehicleTurretComponent.OverlayState;
      if (!string.IsNullOrWhiteSpace(vehicleTurretComponent.OverlayRsi))
        sprite.LayerSetState(0, RSI.StateId.op_Implicit(overlayState), vehicleTurretComponent.OverlayRsi);
      else
        sprite.LayerSetState(0, RSI.StateId.op_Implicit(overlayState));
      sprite.LayerSetVisible(0, true);
    }
    else
    {
      SpriteComponent spriteComponent;
      if (!this.TryComp<SpriteComponent>(nullable, ref spriteComponent) || spriteComponent.BaseRSI == null || !spriteComponent.AllLayers.Any<ISpriteLayer>())
        return;
      this.SetOverlayDepth(nullable.Value, sprite);
      string str = spriteComponent.LayerGetState(0).ToString();
      sprite.LayerSetRSI(0, spriteComponent.BaseRSI);
      sprite.LayerSetState(0, RSI.StateId.op_Implicit(str));
      sprite.LayerSetVisible(0, true);
    }
  }

  private void SetOverlayDepth(EntityUid turretUid, SpriteComponent sprite)
  {
    int num = 7;
    if (this.HasComp<VehicleTurretAttachmentComponent>(turretUid))
      ++num;
    if (sprite.DrawDepth == num)
      return;
    sprite.DrawDepth = num;
  }

  private bool TryComputeRenderedTransform(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    out EntityUid vehicle,
    out Angle vehicleRot,
    out Vector2 localOffset,
    out Angle localRotation)
  {
    vehicle = new EntityUid();
    vehicleRot = Angle.Zero;
    localOffset = Vector2.Zero;
    localRotation = Angle.Zero;
    if (!this.TryGetVehicle(turretUid, out vehicle))
      return false;
    EntityUid anchorUid;
    VehicleTurretComponent anchorTurret;
    this.TryGetAnchorTurret(turretUid, turret, out anchorUid, out anchorTurret);
    vehicleRot = this._transform.GetWorldRotation(vehicle);
    Angle rotation = this._eye.CurrentEye.Rotation;
    Angle vehicleFacingAngle = this.GetVehicleFacingAngle(vehicle, vehicleRot);
    Angle renderFacing1 = this.GetRenderFacing(anchorTurret, anchorTurret, vehicleRot, vehicleFacingAngle, rotation);
    Vector2 offset1 = this.GetPixelOffset(anchorTurret, renderFacing1) / 32f;
    Vector2 vehicleLocalOffset = VehicleTurretVisualSystem.GetVehicleLocalOffset(anchorTurret, offset1, vehicleRot, rotation);
    Angle angle1 = anchorTurret.RotateToCursor ? anchorTurret.WorldRotation : Angle.Zero;
    localOffset = vehicleLocalOffset;
    localRotation = angle1;
    if (EntityUid.op_Equality(anchorUid, turretUid))
      return true;
    Angle renderFacing2 = this.GetRenderFacing(turret, anchorTurret, vehicleRot, vehicleFacingAngle, rotation);
    Vector2 offset2 = this.GetPixelOffset(turret, renderFacing2) / 32f;
    Vector2 vector2;
    if (turret.OffsetRotatesWithTurret)
    {
      if (turret.UseDirectionalOffsets)
      {
        Angle directionalAngle = VehicleTurretVisualSystem.GetDirectionalAngle(VehicleTurretVisualSystem.GetDirectionalDir(renderFacing2));
        Angle angle2 = Angle.op_Subtraction(angle1, directionalAngle);
        vector2 = ((Angle) ref angle2).RotateVec(ref offset2);
      }
      else
        vector2 = ((Angle) ref angle1).RotateVec(ref offset2);
    }
    else
      vector2 = VehicleTurretVisualSystem.GetVehicleLocalOffset(turret, offset2, vehicleRot, rotation);
    localOffset += vector2;
    return true;
  }

  private Vector2 GetPixelOffset(VehicleTurretComponent turret, Angle facing)
  {
    if (!turret.UseDirectionalOffsets)
      return turret.PixelOffset;
    Vector2 pixelOffset = turret.PixelOffset;
    double normalized = facing.Theta % 6.2831854820251465;
    if (normalized < 0.0)
      normalized += 6.2831854820251465;
    Direction directionalDir = VehicleTurretVisualSystem.GetDirectionalDir((float) normalized);
    Vector2 directionalOffset = VehicleTurretVisualSystem.GetDirectionalOffset(turret, directionalDir);
    return pixelOffset + directionalOffset;
  }

  private static Vector2 GetDirectionalOffset(VehicleTurretComponent turret, Direction dir)
  {
    Vector2 directionalOffset;
    switch ((int) dir)
    {
      case 0:
        directionalOffset = turret.PixelOffsetSouth;
        break;
      case 2:
        directionalOffset = turret.PixelOffsetEast;
        break;
      case 4:
        directionalOffset = turret.PixelOffsetNorth;
        break;
      case 6:
        directionalOffset = turret.PixelOffsetWest;
        break;
      default:
        directionalOffset = Vector2.Zero;
        break;
    }
    return directionalOffset;
  }

  private static Direction GetDirectionalDir(Angle facing)
  {
    return VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(facing);
  }

  private static Direction GetDirectionalDir(float normalized)
  {
    return VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(new Angle((double) normalized));
  }

  private static Angle GetDirectionalAngle(Direction dir) => DirectionExtensions.ToAngle(dir);

  private bool TryGetVehicle(EntityUid turretUid, out EntityUid vehicle)
  {
    vehicle = new EntityUid();
    BaseContainer baseContainer;
    EntityUid owner;
    for (EntityUid entityUid = turretUid; this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((entityUid, (TransformComponent) null)), ref baseContainer); entityUid = owner)
    {
      owner = baseContainer.Owner;
      if (this.HasComp<VehicleComponent>(owner))
      {
        vehicle = owner;
        return true;
      }
    }
    return false;
  }

  private void TryGetAnchorTurret(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    out EntityUid anchorUid,
    out VehicleTurretComponent anchorTurret)
  {
    anchorUid = turretUid;
    anchorTurret = turret;
    EntityUid parentUid;
    VehicleTurretComponent parentTurret;
    if (!this.HasComp<VehicleTurretAttachmentComponent>(turretUid) || !this.TryGetParentTurret(turretUid, out parentUid, out parentTurret))
      return;
    anchorUid = parentUid;
    anchorTurret = parentTurret;
  }

  private bool TryGetParentTurret(
    EntityUid turretUid,
    out EntityUid parentUid,
    out VehicleTurretComponent parentTurret)
  {
    parentUid = new EntityUid();
    parentTurret = (VehicleTurretComponent) null;
    BaseContainer baseContainer;
    EntityUid owner;
    for (EntityUid entityUid = turretUid; this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((entityUid, (TransformComponent) null)), ref baseContainer); entityUid = owner)
    {
      owner = baseContainer.Owner;
      VehicleTurretComponent vehicleTurretComponent;
      if (this.TryComp<VehicleTurretComponent>(owner, ref vehicleTurretComponent))
      {
        parentUid = owner;
        parentTurret = vehicleTurretComponent;
        return true;
      }
    }
    return false;
  }

  private Angle GetRenderFacing(
    VehicleTurretComponent turret,
    VehicleTurretComponent anchorTurret,
    Angle vehicleRot,
    Angle baseFacingAngle,
    Angle eyeRot)
  {
    Angle angle = Angle.op_Addition(this.GetOffsetFacing(turret, anchorTurret, vehicleRot, baseFacingAngle), eyeRot);
    return ((Angle) ref angle).Reduced();
  }

  private static Vector2 GetVehicleLocalOffset(
    VehicleTurretComponent turret,
    Vector2 offset,
    Angle vehicleRot,
    Angle eyeRot)
  {
    if (turret.UseDirectionalOffsets)
    {
      Angle angle = Angle.op_UnaryNegation(eyeRot);
      offset = ((Angle) ref angle).RotateVec(ref offset);
    }
    Angle angle1 = Angle.op_UnaryNegation(vehicleRot);
    return ((Angle) ref angle1).RotateVec(ref offset);
  }

  private Angle GetOffsetFacing(
    VehicleTurretComponent turret,
    VehicleTurretComponent anchorTurret,
    Angle vehicleRot,
    Angle baseFacingAngle)
  {
    if (!turret.OffsetRotatesWithTurret)
      return baseFacingAngle;
    Angle angle = Angle.op_Addition(vehicleRot, anchorTurret.WorldRotation);
    return ((Angle) ref angle).Reduced();
  }

  private Angle GetVehicleFacingAngle(EntityUid vehicle, Angle vehicleRot)
  {
    GridVehicleMoverComponent vehicleMoverComponent;
    return this.TryComp<GridVehicleMoverComponent>(vehicle, ref vehicleMoverComponent) && Vector2i.op_Inequality(vehicleMoverComponent.CurrentDirection, Vector2i.Zero) ? DirectionExtensions.ToWorldAngle(new Vector2((float) vehicleMoverComponent.CurrentDirection.X, (float) vehicleMoverComponent.CurrentDirection.Y)) : vehicleRot;
  }
}
