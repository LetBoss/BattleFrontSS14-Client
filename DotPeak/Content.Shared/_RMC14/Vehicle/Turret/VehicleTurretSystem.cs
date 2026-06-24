// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleTurretSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleTurretSystem : EntitySystem
{
  private const float PixelsPerMeter = 32f;
  private const float FireAlignmentToleranceDegrees = 2f;
  [Dependency]
  private readonly SharedContainerSystem _container;
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  [Dependency]
  private readonly IGameTiming _timing;

  public override void Initialize()
  {
    this.UpdatesAfter.Add(typeof (GridVehicleMoverSystem));
    this.SubscribeLocalEvent<VehicleTurretComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<VehicleTurretComponent, EntInsertedIntoContainerMessage>(this.OnInserted));
    this.SubscribeLocalEvent<VehicleTurretComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<VehicleTurretComponent, EntRemovedFromContainerMessage>(this.OnRemoved));
    this.SubscribeLocalEvent<VehicleTurretComponent, ComponentShutdown>(new EntityEventRefHandler<VehicleTurretComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<VehicleTurretComponent, AttemptShootEvent>(new EntityEventRefHandler<VehicleTurretComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeNetworkEvent<VehicleTurretRotateEvent>(new EntitySessionEventHandler<VehicleTurretRotateEvent>(this.OnRotateEvent));
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient && this._timing.ApplyingState)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleTurretComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<VehicleTurretComponent>();
    EntityUid uid1;
    VehicleTurretComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (VehicleTurretSystem.ShouldUpdateTransforms(comp1_1))
      {
        EntityUid vehicle;
        if (!this.TryGetVehicle(uid1, out vehicle))
          this.CleanupVisual(comp1_1);
        else
          this.UpdateTurretRotation(uid1, comp1_1, vehicle, frameTime);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleTurretComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<VehicleTurretComponent>();
    EntityUid uid2;
    VehicleTurretComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (VehicleTurretSystem.ShouldUpdateTransforms(comp1_2))
      {
        EntityUid vehicle;
        if (!this.TryGetVehicle(uid2, out vehicle))
        {
          this.CleanupVisual(comp1_2);
        }
        else
        {
          EntityUid anchorUid;
          VehicleTurretComponent anchorTurret;
          this.TryGetAnchorTurret(uid2, comp1_2, out anchorUid, out anchorTurret);
          this.EnsureVisual(uid2, comp1_2, vehicle);
          this.InitializeRotation(anchorUid, anchorTurret, vehicle);
          this.UpdateTurretTransforms(uid2, comp1_2, vehicle, anchorUid, anchorTurret);
        }
      }
    }
  }

  private void OnInserted(
    Entity<VehicleTurretComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    EntityUid vehicle;
    if (this._net.IsClient && this._timing.ApplyingState || !VehicleTurretSystem.ShouldUpdateTransforms(ent.Comp) || !this.TryGetVehicle(ent.Owner, out vehicle))
      return;
    this.UpdateTurretRotation(ent.Owner, ent.Comp, vehicle, 0.0f);
    EntityUid anchorUid;
    VehicleTurretComponent anchorTurret;
    this.TryGetAnchorTurret(ent.Owner, ent.Comp, out anchorUid, out anchorTurret);
    this.EnsureVisual(ent.Owner, ent.Comp, vehicle);
    this.InitializeRotation(anchorUid, anchorTurret, vehicle);
    this.UpdateTurretTransforms(ent.Owner, ent.Comp, vehicle, anchorUid, anchorTurret);
  }

  private void OnRemoved(
    Entity<VehicleTurretComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    this.CleanupVisual(ent.Comp);
  }

  private void OnShutdown(Entity<VehicleTurretComponent> ent, ref ComponentShutdown args)
  {
    this.CleanupVisual(ent.Comp);
  }

  private void OnRotateEvent(VehicleTurretRotateEvent args, EntitySessionEventArgs session)
  {
    if (this._net.IsClient && !this._timing.IsFirstTimePredicted || this._net.IsClient && this._timing.ApplyingState)
      return;
    EntityUid entity = this.GetEntity(args.Turret);
    VehicleTurretComponent comp1;
    EntityUid vehicle;
    if (!this.TryComp<VehicleTurretComponent>(entity, out comp1) || !this.TryGetVehicle(entity, out vehicle))
      return;
    if (!this._net.IsClient)
    {
      EntityUid? nullable = session.SenderSession.AttachedEntity;
      if (!nullable.HasValue)
        return;
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      VehicleViewToggleComponent comp2;
      VehicleWeaponsOperatorComponent comp3;
      if (this.TryComp<VehicleViewToggleComponent>(valueOrDefault, out comp2) && !comp2.IsOutside || !this.TryComp<VehicleWeaponsOperatorComponent>(valueOrDefault, out comp3))
        return;
      nullable = comp3.Vehicle;
      EntityUid entityUid1 = vehicle;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid1 ? 1 : 0) : 1) != 0)
        return;
      nullable = comp3.SelectedWeapon;
      EntityUid entityUid2 = entity;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid2 ? 1 : 0) : 1) != 0)
        return;
    }
    EntityUid targetUid;
    VehicleTurretComponent targetTurret;
    EntityCoordinates origin;
    if (!this.TryResolveRotationTarget(entity, comp1, out targetUid, out targetTurret) || !targetTurret.RotateToCursor || !this.TryGetTurretOrigin(targetUid, targetTurret, out origin))
      return;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(origin);
    Vector2 vector2 = this._transform.ToMapCoordinates(coordinates).Position - mapCoordinates.Position;
    if ((double) vector2.LengthSquared() <= 9.9999997473787516E-05)
      return;
    Angle worldRotation = this._transform.GetWorldRotation(vehicle);
    Angle angle1;
    if (!targetTurret.StabilizedRotation)
    {
      Angle angle2 = Angle.op_Subtraction(DirectionExtensions.ToWorldAngle(vector2), worldRotation);
      angle1 = ((Angle) ref angle2).Reduced();
    }
    else
      angle1 = DirectionExtensions.ToWorldAngle(vector2);
    Angle desiredRotation = angle1;
    this.SetTargetRotation(targetUid, targetTurret, vehicle, desiredRotation, true);
  }

  private void EnsureVisual(EntityUid turretUid, VehicleTurretComponent turret, EntityUid vehicle)
  {
    if (this._net.IsClient || !turret.ShowOverlay)
      return;
    EntityUid? visualEntity = turret.VisualEntity;
    if (visualEntity.HasValue && this.Exists(visualEntity.GetValueOrDefault()))
      return;
    EntityUid uid = this.Spawn("VehicleTurretVisual", this.Transform(vehicle).Coordinates);
    VehicleTurretVisualComponent turretVisualComponent = this.EnsureComp<VehicleTurretVisualComponent>(uid);
    turretVisualComponent.Turret = this.GetNetEntity(turretUid);
    this.Dirty(uid, (IComponent) turretVisualComponent);
    turret.VisualEntity = new EntityUid?(uid);
  }

  private void CleanupVisual(VehicleTurretComponent turret)
  {
    if (this._net.IsClient)
      return;
    EntityUid? visualEntity = turret.VisualEntity;
    if (!visualEntity.HasValue)
      return;
    EntityUid valueOrDefault = visualEntity.GetValueOrDefault();
    if (this.Exists(valueOrDefault) && !this.TerminatingOrDeleted(valueOrDefault) && !this.EntityManager.IsQueuedForDeletion(valueOrDefault))
      this.Del(new EntityUid?(valueOrDefault));
    turret.VisualEntity = new EntityUid?();
  }

  private void UpdateTurretTransforms(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    EntityUid vehicle,
    EntityUid anchorUid,
    VehicleTurretComponent anchorTurret)
  {
    Angle worldRotation = this._transform.GetWorldRotation(vehicle);
    Angle vehicleFacingAngle = this.GetVehicleFacingAngle(vehicle, worldRotation);
    Angle offsetFacing1 = this.GetOffsetFacing(anchorTurret, anchorTurret, worldRotation, vehicleFacingAngle);
    Angle angle1 = Angle.op_UnaryNegation(worldRotation);
    ref Angle local1 = ref angle1;
    Vector2 vector2_1 = this.GetPixelOffset(anchorTurret, offsetFacing1) / 32f;
    ref Vector2 local2 = ref vector2_1;
    Vector2 position1 = ((Angle) ref local1).RotateVec(ref local2);
    Angle angle2 = Angle.Zero;
    if (anchorTurret.RotateToCursor)
      angle2 = anchorTurret.WorldRotation;
    EntityCoordinates entityCoordinates1;
    Angle angle3;
    EntityCoordinates entityCoordinates2;
    Angle angle4;
    if (anchorUid == turretUid)
    {
      entityCoordinates1 = new EntityCoordinates(vehicle, position1);
      angle3 = angle2;
      entityCoordinates2 = entityCoordinates1;
      angle4 = angle2;
    }
    else
    {
      Angle offsetFacing2 = this.GetOffsetFacing(turret, anchorTurret, worldRotation, vehicleFacingAngle);
      Vector2 vector2_2 = this.GetPixelOffset(turret, offsetFacing2) / 32f;
      Vector2 position2;
      Vector2 vector2_3;
      if (turret.OffsetRotatesWithTurret)
      {
        if (turret.UseDirectionalOffsets)
        {
          Direction directionalDir = VehicleTurretSystem.GetDirectionalDir(offsetFacing2);
          Vector2 vector2_4 = (turret.PixelOffset + VehicleTurretSystem.GetDirectionalOffset(turret, directionalDir)) / 32f;
          Angle directionalAngle = VehicleTurretSystem.GetDirectionalAngle(directionalDir);
          Angle angle5 = Angle.op_UnaryNegation(directionalAngle);
          position2 = ((Angle) ref angle5).RotateVec(ref vector2_4);
          Angle angle6 = Angle.op_Subtraction(angle2, directionalAngle);
          vector2_3 = ((Angle) ref angle6).RotateVec(ref vector2_4);
        }
        else
        {
          position2 = vector2_2;
          vector2_3 = ((Angle) ref angle2).RotateVec(ref position2);
        }
      }
      else
      {
        Angle angle7 = Angle.op_UnaryNegation(worldRotation);
        vector2_3 = ((Angle) ref angle7).RotateVec(ref vector2_2);
        Angle angle8 = Angle.op_UnaryNegation(angle2);
        position2 = ((Angle) ref angle8).RotateVec(ref vector2_3);
      }
      entityCoordinates1 = new EntityCoordinates(anchorUid, position2);
      angle3 = Angle.Zero;
      entityCoordinates2 = new EntityCoordinates(vehicle, position1 + vector2_3);
      angle4 = angle2;
    }
    TransformComponent xform1 = this.Transform(turretUid);
    this._transform.SetCoordinates(turretUid, xform1, entityCoordinates1);
    this._transform.SetLocalRotation(turretUid, angle3, xform1);
    EntityUid? visualEntity = turret.VisualEntity;
    if (!visualEntity.HasValue)
      return;
    EntityUid valueOrDefault = visualEntity.GetValueOrDefault();
    if (!this.Exists(valueOrDefault))
      return;
    TransformComponent xform2 = this.Transform(valueOrDefault);
    this._transform.SetCoordinates(valueOrDefault, xform2, entityCoordinates2);
    this._transform.SetLocalRotation(valueOrDefault, angle4, xform2);
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

  public bool TryGetTurretOrigin(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    out EntityCoordinates origin)
  {
    origin = new EntityCoordinates();
    if (!this.TryGetVehicle(turretUid, out EntityUid _))
      return false;
    origin = this._transform.GetMoverCoordinates(turretUid);
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
    Direction directionalDir = VehicleTurretSystem.GetDirectionalDir((float) normalized);
    Vector2 directionalOffset = VehicleTurretSystem.GetDirectionalOffset(turret, directionalDir);
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

  private Angle GetVehicleFacingAngle(EntityUid vehicle, Angle vehicleRot)
  {
    GridVehicleMoverComponent comp;
    return this.TryComp<GridVehicleMoverComponent>(vehicle, out comp) && Vector2i.op_Inequality(comp.CurrentDirection, Vector2i.Zero) ? DirectionExtensions.ToWorldAngle(new Vector2((float) comp.CurrentDirection.X, (float) comp.CurrentDirection.Y)) : vehicleRot;
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

  private bool TryGetVehicle(EntityUid turretUid, out EntityUid vehicle)
  {
    vehicle = new EntityUid();
    BaseContainer container;
    EntityUid owner;
    for (EntityUid entityUid = turretUid; this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (entityUid, (TransformComponent) null), out container); entityUid = owner)
    {
      owner = container.Owner;
      if (this.HasComp<VehicleComponent>(owner))
      {
        vehicle = owner;
        return true;
      }
    }
    return false;
  }

  public bool TryResolveRotationTarget(
    EntityUid turretUid,
    out EntityUid targetUid,
    out VehicleTurretComponent targetTurret)
  {
    targetUid = new EntityUid();
    targetTurret = (VehicleTurretComponent) null;
    VehicleTurretComponent comp;
    return this.TryComp<VehicleTurretComponent>(turretUid, out comp) && this.TryResolveRotationTarget(turretUid, comp, out targetUid, out targetTurret);
  }

  private bool TryResolveRotationTarget(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    out EntityUid targetUid,
    out VehicleTurretComponent targetTurret)
  {
    targetUid = turretUid;
    targetTurret = turret;
    EntityUid parentUid;
    VehicleTurretComponent parentTurret;
    if (!this.HasComp<VehicleTurretAttachmentComponent>(turretUid) || !this.TryGetParentTurret(turretUid, out parentUid, out parentTurret))
      return true;
    targetUid = parentUid;
    targetTurret = parentTurret;
    return true;
  }

  private bool TryGetParentTurret(
    EntityUid turretUid,
    out EntityUid parentUid,
    out VehicleTurretComponent parentTurret)
  {
    parentUid = new EntityUid();
    parentTurret = (VehicleTurretComponent) null;
    BaseContainer container;
    EntityUid owner;
    for (EntityUid entityUid = turretUid; this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (entityUid, (TransformComponent) null), out container); entityUid = owner)
    {
      owner = container.Owner;
      VehicleTurretComponent comp;
      if (this.TryComp<VehicleTurretComponent>(owner, out comp))
      {
        parentUid = owner;
        parentTurret = comp;
        return true;
      }
    }
    return false;
  }

  public bool TryAimAtTarget(
    EntityUid turretUid,
    EntityUid target,
    out EntityCoordinates targetCoords)
  {
    targetCoords = new EntityCoordinates();
    EntityUid targetUid;
    VehicleTurretComponent targetTurret;
    EntityUid vehicle;
    EntityCoordinates origin;
    if (!this.TryResolveRotationTarget(turretUid, out targetUid, out targetTurret) || !targetTurret.RotateToCursor || !this.TryGetVehicle(targetUid, out vehicle) || !this.TryGetTurretOrigin(targetUid, targetTurret, out origin))
      return false;
    targetCoords = this.Transform(target).Coordinates;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(origin);
    Vector2 vector2 = this._transform.ToMapCoordinates(targetCoords).Position - mapCoordinates.Position;
    if ((double) vector2.LengthSquared() <= 9.9999997473787516E-05)
      return false;
    Angle worldRotation = this._transform.GetWorldRotation(vehicle);
    Angle angle1;
    if (!targetTurret.StabilizedRotation)
    {
      Angle angle2 = Angle.op_Subtraction(DirectionExtensions.ToWorldAngle(vector2), worldRotation);
      angle1 = ((Angle) ref angle2).Reduced();
    }
    else
      angle1 = DirectionExtensions.ToWorldAngle(vector2);
    Angle angle3 = angle1;
    targetTurret.TargetRotation = angle3;
    if ((double) targetTurret.RotationSpeed <= 0.0)
    {
      Angle angle4;
      if (!targetTurret.StabilizedRotation)
      {
        angle4 = angle3;
      }
      else
      {
        Angle angle5 = Angle.op_Subtraction(angle3, worldRotation);
        angle4 = ((Angle) ref angle5).Reduced();
      }
      Angle angle6 = angle4;
      targetTurret.WorldRotation = angle6;
    }
    this.Dirty(targetUid, (IComponent) targetTurret);
    this.UpdateTurretTransforms(targetUid, targetTurret, vehicle, targetUid, targetTurret);
    return true;
  }

  public bool TrySetTargetRotationWorld(EntityUid turretUid, Angle worldRotation)
  {
    EntityUid targetUid;
    VehicleTurretComponent targetTurret;
    EntityUid vehicle;
    if (!this.TryResolveRotationTarget(turretUid, out targetUid, out targetTurret) || !this.TryGetVehicle(targetUid, out vehicle))
      return false;
    Angle worldRotation1 = this._transform.GetWorldRotation(vehicle);
    Angle angle1;
    if (!targetTurret.StabilizedRotation)
    {
      Angle angle2 = Angle.op_Subtraction(worldRotation, worldRotation1);
      angle1 = ((Angle) ref angle2).Reduced();
    }
    else
      angle1 = worldRotation;
    Angle angle3 = angle1;
    targetTurret.TargetRotation = angle3;
    if ((double) targetTurret.RotationSpeed <= 0.0)
    {
      Angle angle4;
      if (!targetTurret.StabilizedRotation)
      {
        angle4 = angle3;
      }
      else
      {
        Angle angle5 = Angle.op_Subtraction(angle3, worldRotation1);
        angle4 = ((Angle) ref angle5).Reduced();
      }
      Angle angle6 = angle4;
      targetTurret.WorldRotation = angle6;
    }
    this.Dirty(targetUid, (IComponent) targetTurret);
    this.UpdateTurretTransforms(targetUid, targetTurret, vehicle, targetUid, targetTurret);
    return true;
  }

  private void InitializeRotation(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    EntityUid vehicle)
  {
    if (this._net.IsClient)
      return;
    if (!turret.RotateToCursor && !turret.ShowOverlay || Angle.op_Inequality(turret.WorldRotation, Angle.Zero))
    {
      if (!Angle.op_Equality(turret.TargetRotation, Angle.Zero) || !Angle.op_Inequality(turret.WorldRotation, Angle.Zero))
        return;
      Angle worldRotation = this._transform.GetWorldRotation(vehicle);
      VehicleTurretComponent vehicleTurretComponent = turret;
      Angle angle1;
      if (!turret.StabilizedRotation)
      {
        angle1 = turret.WorldRotation;
      }
      else
      {
        Angle angle2 = Angle.op_Addition(turret.WorldRotation, worldRotation);
        angle1 = ((Angle) ref angle2).Reduced();
      }
      vehicleTurretComponent.TargetRotation = angle1;
      this.Dirty(turretUid, (IComponent) turret);
    }
    else
    {
      Angle worldRotation = this._transform.GetWorldRotation(vehicle);
      turret.WorldRotation = Angle.Zero;
      turret.TargetRotation = turret.StabilizedRotation ? worldRotation : Angle.Zero;
      this.Dirty(turretUid, (IComponent) turret);
    }
  }

  private void UpdateTurretRotation(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    EntityUid vehicle,
    float frameTime)
  {
    if (!turret.RotateToCursor)
      return;
    this.ApplyPendingTargetRotation(turretUid, turret, vehicle);
    Angle worldRotation = this._transform.GetWorldRotation(vehicle);
    if (Angle.op_Equality(turret.TargetRotation, Angle.Zero) && Angle.op_Inequality(turret.WorldRotation, Angle.Zero))
    {
      VehicleTurretComponent vehicleTurretComponent = turret;
      Angle angle1;
      if (!turret.StabilizedRotation)
      {
        angle1 = turret.WorldRotation;
      }
      else
      {
        Angle angle2 = Angle.op_Addition(turret.WorldRotation, worldRotation);
        angle1 = ((Angle) ref angle2).Reduced();
      }
      vehicleTurretComponent.TargetRotation = angle1;
      this.Dirty(turretUid, (IComponent) turret);
    }
    else
    {
      Angle angle3;
      Angle angle4;
      if (!turret.StabilizedRotation)
      {
        angle4 = turret.TargetRotation;
      }
      else
      {
        angle3 = Angle.op_Subtraction(turret.TargetRotation, worldRotation);
        angle4 = ((Angle) ref angle3).Reduced();
      }
      Angle angle5 = angle4;
      if ((double) turret.RotationSpeed <= 0.0)
      {
        if (!Angle.op_Inequality(turret.WorldRotation, angle5))
          return;
        turret.WorldRotation = angle5;
        this.Dirty(turretUid, (IComponent) turret);
      }
      else
      {
        Angle angle6 = Angle.ShortestDistance(ref turret.WorldRotation, ref angle5);
        float num1 = MathHelper.DegreesToRadians(turret.RotationSpeed) * frameTime;
        if (Math.Abs(angle6.Theta) <= (double) num1)
        {
          if (!Angle.op_Inequality(turret.WorldRotation, angle5))
            return;
          turret.WorldRotation = angle5;
          this.Dirty(turretUid, (IComponent) turret);
        }
        else
        {
          float num2 = (float) Math.Sign(angle6.Theta) * num1;
          angle3 = Angle.op_Addition(turret.WorldRotation, Angle.op_Implicit(num2));
          Angle angle7 = ((Angle) ref angle3).Reduced();
          if (!Angle.op_Inequality(angle7, turret.WorldRotation))
            return;
          turret.WorldRotation = angle7;
          this.Dirty(turretUid, (IComponent) turret);
        }
      }
    }
  }

  private void OnAttemptShoot(Entity<VehicleTurretComponent> ent, ref AttemptShootEvent args)
  {
    if (this._net.IsClient && !this._timing.IsFirstTimePredicted || args.Cancelled)
      return;
    if (!this.CanOperatorUseTurret(ent.Owner, args.User))
    {
      args.Cancelled = true;
      args.ResetCooldown = true;
    }
    else
    {
      EntityUid targetUid;
      VehicleTurretComponent targetTurret;
      if (!this.TryResolveRotationTarget(ent.Owner, ent.Comp, out targetUid, out targetTurret) || !targetTurret.RotateToCursor)
        return;
      float radians = MathHelper.DegreesToRadians(MathF.Max(2f + ent.Comp.FireWhileRotatingGraceDegrees, 0.0f));
      EntityUid vehicle;
      if (!this.TryGetVehicle(targetUid, out vehicle))
        return;
      Angle worldRotation = this._transform.GetWorldRotation(vehicle);
      EntityCoordinates? toCoordinates = args.ToCoordinates;
      EntityCoordinates origin;
      Angle angle1;
      if (toCoordinates.HasValue && this.TryGetTurretOrigin(targetUid, targetTurret, out origin))
      {
        MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(origin);
        SharedTransformSystem transform = this._transform;
        toCoordinates = args.ToCoordinates;
        EntityCoordinates coordinates = toCoordinates.Value;
        Vector2 vector2 = transform.ToMapCoordinates(coordinates).Position - mapCoordinates.Position;
        if ((double) vector2.LengthSquared() > 9.9999997473787516E-05)
        {
          Angle worldAngle = DirectionExtensions.ToWorldAngle(vector2);
          angle1 = Angle.op_Addition(targetTurret.WorldRotation, worldRotation);
          Angle angle2 = ((Angle) ref angle1).Reduced();
          if (Math.Abs(Angle.ShortestDistance(ref angle2, ref worldAngle).Theta) > (double) radians)
          {
            args.Cancelled = true;
            args.ResetCooldown = true;
            return;
          }
        }
      }
      angle1 = Angle.op_Addition(targetTurret.WorldRotation, worldRotation);
      Angle angle3 = ((Angle) ref angle1).Reduced();
      Angle angle4;
      if (!targetTurret.StabilizedRotation)
      {
        angle1 = Angle.op_Addition(targetTurret.TargetRotation, worldRotation);
        angle4 = ((Angle) ref angle1).Reduced();
      }
      else
        angle4 = targetTurret.TargetRotation;
      Angle angle5 = angle4;
      if (Math.Abs(Angle.ShortestDistance(ref angle3, ref angle5).Theta) <= (double) radians)
      {
        this.ApplyShotDirectionConstraint(ent.Comp, targetTurret, targetUid, vehicle, ref args);
      }
      else
      {
        args.Cancelled = true;
        args.ResetCooldown = true;
      }
    }
  }

  private void ApplyShotDirectionConstraint(
    VehicleTurretComponent sourceTurret,
    VehicleTurretComponent rotationTurret,
    EntityUid rotationTurretUid,
    EntityUid vehicle,
    ref AttemptShootEvent args)
  {
    float num1 = MathF.Max(0.0f, sourceTurret.MaxShotCurvatureDegrees);
    if (!sourceTurret.UseBarrelDirectionForShots && (double) num1 <= 0.0)
      return;
    MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(args.FromCoordinates);
    EntityCoordinates? toCoordinates = args.ToCoordinates;
    if (!toCoordinates.HasValue)
      return;
    MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(toCoordinates.GetValueOrDefault());
    if (mapCoordinates2.MapId != mapCoordinates1.MapId)
      return;
    float num2 = (mapCoordinates2.Position - mapCoordinates1.Position).Length();
    if ((double) num2 <= 9.9999997473787516E-05)
      return;
    Angle worldRotation = this._transform.GetWorldRotation(vehicle);
    Angle angle1 = Angle.op_Addition(rotationTurret.WorldRotation, worldRotation);
    Angle angle2 = ((Angle) ref angle1).Reduced();
    Angle angle3 = angle2;
    if (!sourceTurret.UseBarrelDirectionForShots && (double) num1 > 0.0)
    {
      Angle worldAngle = DirectionExtensions.ToWorldAngle(mapCoordinates2.Position - mapCoordinates1.Position);
      float radians = MathHelper.DegreesToRadians(num1);
      float num3 = MathHelper.Clamp((float) Angle.ShortestDistance(ref angle2, ref worldAngle).Theta, -radians, radians);
      angle1 = Angle.op_Addition(angle2, Angle.op_Implicit(num3));
      angle3 = ((Angle) ref angle1).Reduced();
    }
    MapCoordinates coordinates1 = new MapCoordinates(mapCoordinates1.Position + ((Angle) ref angle3).ToWorldVec() * num2, mapCoordinates1.MapId);
    EntityCoordinates coordinates2 = this._transform.ToCoordinates((Entity<TransformComponent>) rotationTurretUid, coordinates1);
    args = args with
    {
      ToCoordinates = new EntityCoordinates?(coordinates2)
    };
  }

  private void ApplyPendingTargetRotation(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    EntityUid vehicle)
  {
    Angle? pendingTargetRotation = turret.PendingTargetRotation;
    if (!pendingTargetRotation.HasValue)
      return;
    Angle valueOrDefault = pendingTargetRotation.GetValueOrDefault();
    if (this._timing.CurTime < turret.PendingTargetApplyAt)
      return;
    turret.PendingTargetRotation = new Angle?();
    turret.PendingTargetApplyAt = TimeSpan.Zero;
    int pendingDirectionSign = turret.PendingDirectionSign;
    turret.PendingDirectionSign = 0;
    this.ApplyTargetRotation(turretUid, turret, vehicle, valueOrDefault, pendingDirectionSign);
  }

  private void SetTargetRotation(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    EntityUid vehicle,
    Angle desiredRotation,
    bool allowReverseDelay)
  {
    Angle angle = Angle.ShortestDistance(ref turret.TargetRotation, ref desiredRotation);
    float radians = MathHelper.DegreesToRadians(MathF.Max(0.0f, turret.RotationInputDeadzoneDegrees));
    if (Math.Abs(angle.Theta) <= (double) radians)
      return;
    int directionSign = Math.Sign(angle.Theta);
    if (allowReverseDelay && (double) turret.ReverseDirectionDelay > 0.0 && directionSign != 0 && turret.LastAppliedDirectionSign != 0 && directionSign != turret.LastAppliedDirectionSign)
    {
      if (!turret.PendingTargetRotation.HasValue || turret.PendingDirectionSign != directionSign)
        turret.PendingTargetApplyAt = this._timing.CurTime + TimeSpan.FromSeconds((double) turret.ReverseDirectionDelay);
      turret.PendingTargetRotation = new Angle?(desiredRotation);
      turret.PendingDirectionSign = directionSign;
    }
    else
    {
      turret.PendingTargetRotation = new Angle?();
      turret.PendingTargetApplyAt = TimeSpan.Zero;
      turret.PendingDirectionSign = 0;
      this.ApplyTargetRotation(turretUid, turret, vehicle, desiredRotation, directionSign);
    }
  }

  private void ApplyTargetRotation(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    EntityUid vehicle,
    Angle desiredRotation,
    int directionSign)
  {
    bool flag = false;
    if (Angle.op_Inequality(turret.TargetRotation, desiredRotation))
    {
      turret.TargetRotation = desiredRotation;
      flag = true;
    }
    if (directionSign != 0)
      turret.LastAppliedDirectionSign = directionSign;
    if ((double) turret.RotationSpeed <= 0.0)
    {
      Angle worldRotation = this._transform.GetWorldRotation(vehicle);
      Angle angle1;
      if (!turret.StabilizedRotation)
      {
        angle1 = desiredRotation;
      }
      else
      {
        Angle angle2 = Angle.op_Subtraction(desiredRotation, worldRotation);
        angle1 = ((Angle) ref angle2).Reduced();
      }
      Angle angle3 = angle1;
      if (Angle.op_Inequality(turret.WorldRotation, angle3))
      {
        turret.WorldRotation = angle3;
        flag = true;
      }
    }
    if (!flag)
      return;
    this.Dirty(turretUid, (IComponent) turret);
  }

  private bool CanOperatorUseTurret(EntityUid turretUid, EntityUid user)
  {
    EntityUid vehicle;
    VehicleWeaponsOperatorComponent comp1;
    if (!this.TryGetVehicle(turretUid, out vehicle) || !this.TryComp<VehicleWeaponsOperatorComponent>(user, out comp1))
      return true;
    EntityUid? nullable = comp1.Vehicle;
    EntityUid entityUid1 = vehicle;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid1 ? 1 : 0) : 1) != 0)
      return true;
    nullable = comp1.SelectedWeapon;
    EntityUid entityUid2 = turretUid;
    VehicleViewToggleComponent comp2;
    return (nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid2 ? 1 : 0) : 1) == 0 && (!this.TryComp<VehicleViewToggleComponent>(user, out comp2) || comp2.IsOutside);
  }

  private static bool ShouldUpdateTransforms(VehicleTurretComponent turret)
  {
    return turret.RotateToCursor || turret.ShowOverlay || turret.UseDirectionalOffsets || turret.PixelOffset != Vector2.Zero || turret.PixelOffsetNorth != Vector2.Zero || turret.PixelOffsetEast != Vector2.Zero || turret.PixelOffsetSouth != Vector2.Zero || turret.PixelOffsetWest != Vector2.Zero;
  }

  public void SetOverlayState(EntityUid uid, string state, VehicleTurretComponent? turret = null)
  {
    if (!this.Resolve<VehicleTurretComponent>(uid, ref turret, false) || turret.OverlayState == state)
      return;
    turret.OverlayState = state;
    this.Dirty(uid, (IComponent) turret);
    EntityUid vehicle;
    if (!this.TryGetVehicle(uid, out vehicle))
      return;
    HardpointSlotsChangedEvent args = new HardpointSlotsChangedEvent(vehicle);
    this.RaiseLocalEvent<HardpointSlotsChangedEvent>(vehicle, args, true);
  }
}
