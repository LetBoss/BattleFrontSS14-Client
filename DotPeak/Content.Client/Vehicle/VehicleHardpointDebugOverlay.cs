// Decompiled with JetBrains decompiler
// Type: Content.Client.Vehicle.VehicleHardpointDebugOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Vehicle;
using Content.Client.Resources;
using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Vehicle;

public sealed class VehicleHardpointDebugOverlay : Overlay
{
  private const float PixelsPerMeter = 32f;
  private static readonly Vector2 GunLabelWorldOffset = new Vector2(0.18f, -0.18f);
  private static readonly Vector2 TurretLabelWorldOffset = new Vector2(-0.55f, -0.18f);
  private const float LabelLineHeight = 13f;
  private const float LabelPadding = 4f;
  private const float LabelCharWidth = 7f;
  private readonly IEntityManager _ents;
  private readonly IEyeManager _eye;
  private readonly SharedTransformSystem _transform;
  private readonly SharedContainerSystem _container;
  private readonly VehicleTurretMuzzleOffsetSystem _vehicleTurretMuzzle;
  private readonly VehicleTurretVisualSystem _vehicleTurretVisual;
  private readonly Font _font;
  private readonly EntityQuery<GunComponent> _gunQ;
  private readonly EntityQuery<GunFireArcComponent> _fireArcQ;
  private readonly EntityQuery<GridVehicleMoverComponent> _moverQ;
  private readonly EntityQuery<GunMuzzleOffsetComponent> _muzzleQ;
  private readonly EntityQuery<VehicleTurretComponent> _turretQ;
  private readonly EntityQuery<VehicleTurretMuzzleComponent> _turretMuzzleQ;
  private readonly EntityQuery<VehiclePortGunComponent> _portGunQ;
  private readonly List<VehicleHardpointDebugOverlay.DebugLabel> _labels = new List<VehicleHardpointDebugOverlay.DebugLabel>();

  public virtual OverlaySpace Space => (OverlaySpace) 10;

  public bool Enabled { get; set; }

  public VehicleHardpointDebugOverlay(IEntityManager ents)
  {
    this._ents = ents;
    this._eye = IoCManager.Resolve<IEyeManager>();
    IResourceCache cache = IoCManager.Resolve<IResourceCache>();
    this._transform = ents.System<SharedTransformSystem>();
    this._container = ents.System<SharedContainerSystem>();
    this._vehicleTurretMuzzle = ents.System<VehicleTurretMuzzleOffsetSystem>();
    this._vehicleTurretVisual = ents.System<VehicleTurretVisualSystem>();
    this._font = cache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
    this._gunQ = ents.GetEntityQuery<GunComponent>();
    this._fireArcQ = ents.GetEntityQuery<GunFireArcComponent>();
    this._moverQ = ents.GetEntityQuery<GridVehicleMoverComponent>();
    this._muzzleQ = ents.GetEntityQuery<GunMuzzleOffsetComponent>();
    this._turretQ = ents.GetEntityQuery<VehicleTurretComponent>();
    this._turretMuzzleQ = ents.GetEntityQuery<VehicleTurretMuzzleComponent>();
    this._portGunQ = ents.GetEntityQuery<VehiclePortGunComponent>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (!this.Enabled)
      return;
    if (args.Space == 2)
    {
      this.DrawLabels(in args);
    }
    else
    {
      this._labels.Clear();
      DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
      EntityQueryEnumerator<GunMuzzleOffsetComponent> entityQueryEnumerator1 = this._ents.EntityQueryEnumerator<GunMuzzleOffsetComponent>();
      EntityUid uid1;
      GunMuzzleOffsetComponent muzzle;
      while (entityQueryEnumerator1.MoveNext(ref uid1, ref muzzle))
      {
        Vector2 origin;
        Vector2 muzzlePos;
        if ((this._turretQ.HasComp(uid1) || this._portGunQ.HasComp(uid1)) && this.TryGetMuzzlePositions(uid1, muzzle, args.MapId, out origin, out muzzlePos))
        {
          ((DrawingHandleBase) worldHandle).DrawLine(origin, muzzlePos, new Color(0.95f, 0.95f, 0.95f, 0.7f));
          ((DrawingHandleBase) worldHandle).DrawCircle(origin, 0.07f, new Color(0.2f, 0.9f, 1f, 0.9f), true);
          ((DrawingHandleBase) worldHandle).DrawCircle(muzzlePos, 0.1f, new Color(1f, 0.75f, 0.2f, 0.95f), true);
          this.DrawShootArc(uid1, origin, args.MapId, worldHandle);
          this.DrawShootTarget(uid1, origin, args.MapId, worldHandle);
          this.AddGunDebugLabel(uid1, muzzle, origin, muzzlePos);
        }
      }
      EntityQueryEnumerator<VehicleTurretMuzzleComponent> entityQueryEnumerator2 = this._ents.EntityQueryEnumerator<VehicleTurretMuzzleComponent>();
      EntityUid uid2;
      VehicleTurretMuzzleComponent turretMuzzle;
      while (entityQueryEnumerator2.MoveNext(ref uid2, ref turretMuzzle))
      {
        Vector2 turretBasePos;
        Vector2 basePos;
        Vector2 leftPos;
        Vector2 rightPos;
        float leftRadius;
        float rightRadius;
        bool useRightNext;
        if (this._turretQ.HasComp(uid2) && this.TryGetTurretMuzzlePositions(uid2, turretMuzzle, args.MapId, out turretBasePos, out basePos, out leftPos, out rightPos, out leftRadius, out rightRadius, out useRightNext))
        {
          if ((double) leftRadius > 0.0)
            ((DrawingHandleBase) worldHandle).DrawCircle(turretBasePos, leftRadius, new Color(0.25f, 0.85f, 1f, 0.5f), true);
          if ((double) rightRadius > 0.0 && (double) MathF.Abs(rightRadius - leftRadius) > 0.0099999997764825821)
            ((DrawingHandleBase) worldHandle).DrawCircle(turretBasePos, rightRadius, new Color(1f, 0.6f, 0.2f, 0.5f), true);
          Color color1 = useRightNext ? new Color(0.4f, 0.4f, 0.4f, 0.7f) : new Color(0.2f, 0.95f, 0.4f, 0.95f);
          Color color2 = useRightNext ? new Color(0.2f, 0.95f, 0.4f, 0.95f) : new Color(0.4f, 0.4f, 0.4f, 0.7f);
          if ((double) leftRadius > 0.0)
            ((DrawingHandleBase) worldHandle).DrawCircle(leftPos, 0.08f, color1, true);
          if ((double) rightRadius > 0.0)
            ((DrawingHandleBase) worldHandle).DrawCircle(rightPos, 0.08f, color2, true);
          ((DrawingHandleBase) worldHandle).DrawLine(turretBasePos, basePos, new Color(0.4f, 0.9f, 1f, 0.5f));
        }
      }
      EntityQueryEnumerator<VehicleTurretComponent> entityQueryEnumerator3 = this._ents.EntityQueryEnumerator<VehicleTurretComponent>();
      EntityUid entityUid;
      VehicleTurretComponent turret;
      while (entityQueryEnumerator3.MoveNext(ref entityUid, ref turret))
      {
        Vector2 basePos;
        Vector2 anchorPos;
        Vector2 turretPos;
        if (this.TryGetTurretOverlayPositions(entityUid, turret, args.MapId, out basePos, out anchorPos, out turretPos))
        {
          ((DrawingHandleBase) worldHandle).DrawCircle(basePos, 0.06f, new Color(0.7f, 0.7f, 0.7f, 0.8f), true);
          ((DrawingHandleBase) worldHandle).DrawLine(basePos, anchorPos, new Color(0.2f, 0.8f, 0.95f, 0.7f));
          ((DrawingHandleBase) worldHandle).DrawCircle(anchorPos, 0.07f, new Color(0.2f, 0.8f, 0.95f, 0.9f), true);
          if (anchorPos != turretPos)
          {
            ((DrawingHandleBase) worldHandle).DrawLine(anchorPos, turretPos, new Color(1f, 0.7f, 0.2f, 0.8f));
            ((DrawingHandleBase) worldHandle).DrawCircle(turretPos, 0.08f, new Color(1f, 0.7f, 0.2f, 0.95f), true);
          }
          if (!this._muzzleQ.HasComp(entityUid))
            this.AddTurretDebugLabel(entityUid, turret, basePos, anchorPos, turretPos);
        }
      }
    }
  }

  private void DrawLabels(in OverlayDrawArgs args)
  {
    if (args.ViewportControl == null || this._labels.Count == 0)
      return;
    List<(VehicleHardpointDebugOverlay.DebugLabel, Vector2, float, float)> valueTupleList = new List<(VehicleHardpointDebugOverlay.DebugLabel, Vector2, float, float)>(this._labels.Count);
    foreach (VehicleHardpointDebugOverlay.DebugLabel label in this._labels)
    {
      Vector2 screen = args.ViewportControl.WorldToScreen(label.WorldPosition);
      float approxLabelWidth = VehicleHardpointDebugOverlay.GetApproxLabelWidth(label.Lines);
      float num = (float) label.Lines.Count * 13f;
      valueTupleList.Add((label, screen, approxLabelWidth, num));
    }
    valueTupleList.Sort((Comparison<(VehicleHardpointDebugOverlay.DebugLabel, Vector2, float, float)>) ((a, b) =>
    {
      int num = a.Position.Y.CompareTo(b.Position.Y);
      return num == 0 ? a.Position.X.CompareTo(b.Position.X) : num;
    }));
    List<Box2> box2List = new List<Box2>(valueTupleList.Count);
    foreach ((VehicleHardpointDebugOverlay.DebugLabel, Vector2, float, float) valueTuple in valueTupleList)
    {
      Vector2 position = valueTuple.Item2;
      Box2 labelRect = VehicleHardpointDebugOverlay.GetLabelRect(position, valueTuple.Item3, valueTuple.Item4);
      int num = 0;
      while (num++ < 16 /*0x10*/)
      {
        Box2? nullable = new Box2?();
        foreach (Box2 box2 in box2List)
        {
          if (((Box2) ref labelRect).Intersects(ref box2))
          {
            nullable = new Box2?(box2);
            break;
          }
        }
        if (nullable.HasValue)
        {
          position.Y = nullable.Value.Bottom + 4f;
          labelRect = VehicleHardpointDebugOverlay.GetLabelRect(position, valueTuple.Item3, valueTuple.Item4);
        }
        else
          break;
      }
      box2List.Add(labelRect);
      Vector2 vector2 = position;
      foreach (VehicleHardpointDebugOverlay.DebugLine line in valueTuple.Item1.Lines)
      {
        ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2 + Vector2.One, line.Text, Color.Black);
        ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2, line.Text, line.Color);
        vector2.Y += 13f;
      }
    }
  }

  private static float GetApproxLabelWidth(List<VehicleHardpointDebugOverlay.DebugLine> lines)
  {
    int num = 0;
    foreach (VehicleHardpointDebugOverlay.DebugLine line in lines)
    {
      if (line.Text.Length > num)
        num = line.Text.Length;
    }
    return (float) num * 7f;
  }

  private static Box2 GetLabelRect(Vector2 position, float width, float height)
  {
    return Box2.FromDimensions(position - new Vector2(4f, 4f), new Vector2(width + 8f, height + 8f));
  }

  private void AddGunDebugLabel(
    EntityUid uid,
    GunMuzzleOffsetComponent muzzle,
    Vector2 origin,
    Vector2 muzzlePos)
  {
    List<VehicleHardpointDebugOverlay.DebugLine> Lines = new List<VehicleHardpointDebugOverlay.DebugLine>();
    Angle rotation = this._eye.CurrentEye.Rotation;
    VehicleTurretComponent turret;
    int num = this._turretQ.TryComp(uid, ref turret) ? 1 : 0;
    bool flag = this._portGunQ.HasComp(uid);
    string str1 = num != 0 ? "turret-gun" : (flag ? "port-gun" : "gun");
    Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"{str1} {uid.Id}", Color.White));
    EntityUid baseUid = uid;
    Angle angle1 = this._transform.GetWorldRotation(uid);
    Angle angle2 = angle1;
    if (num != 0 && turret != null)
    {
      Angle worldRotation1;
      if (this._vehicleTurretVisual.TryGetRenderedPose(uid, out EntityCoordinates _, out worldRotation1))
        angle2 = worldRotation1;
      EntityUid vehicle;
      if (this.TryGetVehicle(uid, out vehicle))
      {
        VehicleTurretComponent anchorTurret;
        this.TryGetAnchorTurret(uid, turret, out EntityUid _, out anchorTurret);
        Angle worldRotation2 = this._transform.GetWorldRotation(vehicle);
        Angle vehicleFacingAngle = this.GetVehicleFacingAngle(vehicle, worldRotation2);
        Angle renderFacing = VehicleHardpointDebugOverlay.GetRenderFacing(turret, anchorTurret, worldRotation2, vehicleFacingAngle, rotation);
        Direction dir = turret.UseDirectionalOffsets ? VehicleHardpointDebugOverlay.GetDirectionalDir(renderFacing) : (Direction) (object) -1;
        Vector2 vector = turret.UseDirectionalOffsets ? VehicleHardpointDebugOverlay.GetDirectionalOffset(turret, dir) : turret.PixelOffset;
        Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"veh {vehicle.Id} face {((Angle) ref vehicleFacingAngle).GetCardinalDir()} eye {VehicleHardpointDebugOverlay.FmtDeg(rotation)} phys {VehicleHardpointDebugOverlay.FmtDeg(angle1)} vis {VehicleHardpointDebugOverlay.FmtDeg(angle2)}", new Color(0.8f, 0.8f, 0.8f, 1f)));
        Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"render {(turret.UseDirectionalOffsets ? dir.ToString() : "-")} {VehicleHardpointDebugOverlay.FmtVec(vector)} gun ", new Color(0.55f, 0.9f, 1f, 1f)));
        List<VehicleHardpointDebugOverlay.DebugLine> debugLineList1 = Lines;
        int index = debugLineList1.Count - 1;
        List<VehicleHardpointDebugOverlay.DebugLine> debugLineList2 = Lines;
        VehicleHardpointDebugOverlay.DebugLine debugLine = new VehicleHardpointDebugOverlay.DebugLine($"{debugLineList2[debugLineList2.Count - 1].Text}{(muzzle.UseDirectionalOffsets ? this.GetBaseDirection(baseUid, angle1).ToString() : "-")} {VehicleHardpointDebugOverlay.FmtVec(muzzle.UseDirectionalOffsets ? VehicleHardpointDebugOverlay.GetDirectionalGunOffset(muzzle, this.GetBaseDirection(baseUid, angle1)) : muzzle.Offset)}", new Color(1f, 0.75f, 0.3f, 1f));
        debugLineList1[index] = debugLine;
      }
    }
    else
    {
      BaseContainer baseContainer;
      if (muzzle.UseContainerOwner && this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null)), ref baseContainer))
        baseUid = baseContainer.Owner;
      angle1 = this.GetBaseRotation(baseUid, muzzle.AngleOffset);
      Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"base {baseUid.Id} rot {VehicleHardpointDebugOverlay.FmtDeg(angle1)}", new Color(0.8f, 0.8f, 0.8f, 1f)));
    }
    Vector2 vector1 = muzzle.Offset;
    Vector2 vector2_1 = muzzle.Offset;
    string str2 = "-";
    if (muzzle.UseDirectionalOffsets)
    {
      Direction baseDirection = this.GetBaseDirection(baseUid, angle1);
      str2 = baseDirection.ToString();
      vector1 = VehicleHardpointDebugOverlay.GetDirectionalGunOffset(muzzle, baseDirection);
      vector2_1 = vector1;
    }
    Vector2 vector2 = !muzzle.UseDirectionalOffsets || muzzle.RotateDirectionalOffsets ? ((Angle) ref angle1).RotateVec(ref vector2_1) : vector2_1;
    Angle angle3 = angle1;
    Vector2? nullable = new Vector2?();
    GunComponent gunComponent;
    if (this._gunQ.TryComp(uid, ref gunComponent))
    {
      EntityCoordinates? shootCoordinates = gunComponent.ShootCoordinates;
      if (shootCoordinates.HasValue)
      {
        MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(shootCoordinates.GetValueOrDefault(), true);
        nullable = new Vector2?(mapCoordinates.Position);
        if (muzzle.UseAimDirection)
        {
          Vector2 vector2_2 = mapCoordinates.Position - origin;
          if ((double) vector2_2.LengthSquared() > 9.9999997473787516E-05)
            angle3 = Angle.op_Addition(DirectionExtensions.ToWorldAngle(vector2_2), muzzle.AngleOffset);
        }
      }
    }
    Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"origin {VehicleHardpointDebugOverlay.FmtPos(origin)} muzzle {VehicleHardpointDebugOverlay.FmtPos(muzzlePos)}", new Color(0.3f, 0.9f, 1f, 1f)));
    Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"dir {str2} pick {VehicleHardpointDebugOverlay.FmtVec(vector1)} world {VehicleHardpointDebugOverlay.FmtVec(vector2)}", new Color(1f, 0.82f, 0.25f, 1f)));
    if (muzzle.MuzzleOffset != Vector2.Zero || muzzle.UseAimDirection)
      Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"muzzle {VehicleHardpointDebugOverlay.FmtVec(muzzle.MuzzleOffset)} rot {VehicleHardpointDebugOverlay.FmtDeg(angle3)} aim={VehicleHardpointDebugOverlay.FmtBool(muzzle.UseAimDirection)}", new Color(0.72f, 1f, 0.76f, 1f)));
    if (nullable.HasValue)
    {
      Vector2 valueOrDefault = nullable.GetValueOrDefault();
      Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"target {VehicleHardpointDebugOverlay.FmtPos(valueOrDefault)} dist {(valueOrDefault - origin).Length():0.00}", new Color(0.95f, 0.4f, 0.95f, 1f)));
    }
    VehicleTurretMuzzleComponent muzzle1;
    if (this._turretMuzzleQ.TryComp(uid, ref muzzle1))
    {
      Direction baseDirection = this.GetBaseDirection(uid, angle1);
      Vector2 directionalTurretOffset1 = VehicleHardpointDebugOverlay.GetDirectionalTurretOffset(muzzle1, baseDirection, false);
      Vector2 directionalTurretOffset2 = VehicleHardpointDebugOverlay.GetDirectionalTurretOffset(muzzle1, baseDirection, true);
      Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"alt {(muzzle1.UseRightNext ? "R" : "L")} L {VehicleHardpointDebugOverlay.FmtVec(directionalTurretOffset1)} R {VehicleHardpointDebugOverlay.FmtVec(directionalTurretOffset2)}", new Color(1f, 0.65f, 0.25f, 1f)));
    }
    this._labels.Add(new VehicleHardpointDebugOverlay.DebugLabel(muzzlePos + VehicleHardpointDebugOverlay.GunLabelWorldOffset, Lines));
  }

  private void AddTurretDebugLabel(
    EntityUid uid,
    VehicleTurretComponent turret,
    Vector2 basePos,
    Vector2 anchorPos,
    Vector2 turretPos)
  {
    List<VehicleHardpointDebugOverlay.DebugLine> debugLineList = new List<VehicleHardpointDebugOverlay.DebugLine>();
    debugLineList.Add(new VehicleHardpointDebugOverlay.DebugLine($"turret {uid.Id}", Color.White));
    List<VehicleHardpointDebugOverlay.DebugLine> Lines = debugLineList;
    EntityUid vehicle;
    if (this.TryGetVehicle(uid, out vehicle))
    {
      EntityUid anchorUid;
      VehicleTurretComponent anchorTurret;
      this.TryGetAnchorTurret(uid, turret, out anchorUid, out anchorTurret);
      Angle worldRotation1 = this._transform.GetWorldRotation(vehicle);
      Angle rotation = this._eye.CurrentEye.Rotation;
      Angle vehicleFacingAngle = this.GetVehicleFacingAngle(vehicle, worldRotation1);
      Angle renderFacing = VehicleHardpointDebugOverlay.GetRenderFacing(turret, anchorTurret, worldRotation1, vehicleFacingAngle, rotation);
      Direction dir = turret.UseDirectionalOffsets ? VehicleHardpointDebugOverlay.GetDirectionalDir(renderFacing) : (Direction) (object) -1;
      Vector2 vector = turret.UseDirectionalOffsets ? VehicleHardpointDebugOverlay.GetDirectionalOffset(turret, dir) : turret.PixelOffset;
      Angle angle = this._transform.GetWorldRotation(uid);
      Angle worldRotation2;
      if (this._vehicleTurretVisual.TryGetRenderedPose(uid, out EntityCoordinates _, out worldRotation2))
        angle = worldRotation2;
      Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"veh {vehicle.Id} face {((Angle) ref vehicleFacingAngle).GetCardinalDir()} eye {VehicleHardpointDebugOverlay.FmtDeg(rotation)} render {VehicleHardpointDebugOverlay.FmtDeg(angle)}", new Color(0.8f, 0.8f, 0.8f, 1f)));
      Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"anchor {anchorUid.Id} world {VehicleHardpointDebugOverlay.FmtDeg(turret.WorldRotation)} target {VehicleHardpointDebugOverlay.FmtDeg(turret.TargetRotation)}", new Color(0.55f, 0.9f, 1f, 1f)));
      Lines.Add(new VehicleHardpointDebugOverlay.DebugLine($"pix {(turret.UseDirectionalOffsets ? dir.ToString() : "-")} {VehicleHardpointDebugOverlay.FmtVec(vector)} base {VehicleHardpointDebugOverlay.FmtPos(basePos)} pos {VehicleHardpointDebugOverlay.FmtPos(turretPos)}", new Color(0.75f, 0.95f, 0.65f, 1f)));
    }
    this._labels.Add(new VehicleHardpointDebugOverlay.DebugLabel(turretPos + VehicleHardpointDebugOverlay.TurretLabelWorldOffset, Lines));
  }

  private static string FmtBool(bool value) => !value ? "N" : "Y";

  private static string FmtDeg(Angle angle) => $"{((Angle) ref angle).Degrees:0.0}";

  private static string FmtVec(Vector2 vector) => $"<{vector.X:0.00},{vector.Y:0.00}>";

  private static string FmtPos(Vector2 vector) => $"{vector.X:0.00},{vector.Y:0.00}";

  private bool TryGetMuzzlePositions(
    EntityUid uid,
    GunMuzzleOffsetComponent muzzle,
    MapId mapId,
    out Vector2 origin,
    out Vector2 muzzlePos)
  {
    origin = new Vector2();
    muzzlePos = new Vector2();
    EntityCoordinates origin1;
    if (this._turretQ.HasComp(uid) && this._vehicleTurretMuzzle.TryGetGunOrigin(uid, new EntityCoordinates?(), out origin1))
    {
      MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(origin1, true);
      if (MapId.op_Inequality(mapCoordinates.MapId, mapId))
        return false;
      origin = mapCoordinates.Position;
      muzzlePos = origin;
      return true;
    }
    EntityUid baseUid = uid;
    BaseContainer baseContainer;
    if (muzzle.UseContainerOwner && this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null)), ref baseContainer))
      baseUid = baseContainer.Owner;
    TransformComponent transformComponent;
    if (!this._ents.TryGetComponent<TransformComponent>(baseUid, ref transformComponent) || MapId.op_Inequality(transformComponent.MapID, mapId))
      return false;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(baseUid);
    Angle baseRotation = this.GetBaseRotation(baseUid, muzzle.AngleOffset);
    (Vector2 Offset, bool Rotate) offset1 = this.GetOffset(muzzle, baseUid, baseRotation);
    Vector2 offset2 = offset1.Offset;
    EntityCoordinates entityCoordinates1 = offset1.Rotate ? ((EntityCoordinates) ref moverCoordinates).Offset(((Angle) ref baseRotation).RotateVec(ref offset2)) : ((EntityCoordinates) ref moverCoordinates).Offset(offset2);
    origin = this._transform.ToMapCoordinates(entityCoordinates1, true).Position;
    EntityCoordinates entityCoordinates2 = entityCoordinates1;
    Angle angle = baseRotation;
    if (muzzle.MuzzleOffset != Vector2.Zero)
    {
      GunComponent gunComponent;
      if (muzzle.UseAimDirection && this._gunQ.TryComp(uid, ref gunComponent))
      {
        EntityCoordinates? shootCoordinates = gunComponent.ShootCoordinates;
        if (shootCoordinates.HasValue)
        {
          EntityCoordinates valueOrDefault = shootCoordinates.GetValueOrDefault();
          MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(entityCoordinates1, true);
          Vector2 vector2 = this._transform.ToMapCoordinates(valueOrDefault, true).Position - mapCoordinates.Position;
          if ((double) vector2.LengthSquared() > 9.9999997473787516E-05)
            angle = Angle.op_Addition(DirectionExtensions.ToWorldAngle(vector2), muzzle.AngleOffset);
        }
      }
      entityCoordinates2 = ((EntityCoordinates) ref entityCoordinates1).Offset(((Angle) ref angle).RotateVec(ref muzzle.MuzzleOffset));
    }
    muzzlePos = this._transform.ToMapCoordinates(entityCoordinates2, true).Position;
    return true;
  }

  private bool TryGetTurretMuzzlePositions(
    EntityUid uid,
    VehicleTurretMuzzleComponent turretMuzzle,
    MapId mapId,
    out Vector2 turretBasePos,
    out Vector2 basePos,
    out Vector2 leftPos,
    out Vector2 rightPos,
    out float leftRadius,
    out float rightRadius,
    out bool useRightNext)
  {
    turretBasePos = new Vector2();
    basePos = new Vector2();
    leftPos = new Vector2();
    rightPos = new Vector2();
    leftRadius = 0.0f;
    rightRadius = 0.0f;
    useRightNext = turretMuzzle.UseRightNext;
    TransformComponent transformComponent;
    if (!this._ents.TryGetComponent<TransformComponent>(uid, ref transformComponent) || MapId.op_Inequality(transformComponent.MapID, mapId))
      return false;
    Angle baseRotation = this._transform.GetWorldRotation(uid);
    EntityCoordinates origin;
    Angle worldRotation;
    if (this._vehicleTurretVisual.TryGetRenderedPose(uid, out origin, out worldRotation))
    {
      MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(origin, true);
      if (MapId.op_Inequality(mapCoordinates.MapId, mapId))
        return false;
      turretBasePos = mapCoordinates.Position;
      baseRotation = worldRotation;
    }
    else
      turretBasePos = this._transform.ToMapCoordinates(this._transform.GetMoverCoordinates(uid), true).Position;
    EntityCoordinates originCoords;
    if (!this.TryGetGunOriginCoordinates(uid, mapId, out originCoords))
      return false;
    basePos = this._transform.ToMapCoordinates(originCoords, true).Position;
    leftPos = basePos + VehicleHardpointDebugOverlay.GetTurretMuzzleWorldOffset(turretMuzzle, baseRotation, false);
    rightPos = basePos + VehicleHardpointDebugOverlay.GetTurretMuzzleWorldOffset(turretMuzzle, baseRotation, true);
    leftRadius = (leftPos - turretBasePos).Length();
    rightRadius = (rightPos - turretBasePos).Length();
    return true;
  }

  private bool TryGetGunOriginCoordinates(
    EntityUid uid,
    MapId mapId,
    out EntityCoordinates originCoords)
  {
    originCoords = new EntityCoordinates();
    EntityCoordinates origin;
    if (this._turretQ.HasComp(uid) && this._vehicleTurretMuzzle.TryGetGunOrigin(uid, new EntityCoordinates?(), out origin))
    {
      if (MapId.op_Inequality(this._transform.ToMapCoordinates(origin, true).MapId, mapId))
        return false;
      originCoords = origin;
      return true;
    }
    EntityUid baseUid = uid;
    GunMuzzleOffsetComponent muzzle;
    BaseContainer baseContainer;
    if (this._muzzleQ.TryComp(uid, ref muzzle) && muzzle.UseContainerOwner && this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null)), ref baseContainer))
      baseUid = baseContainer.Owner;
    TransformComponent transformComponent;
    if (!this._ents.TryGetComponent<TransformComponent>(baseUid, ref transformComponent) || MapId.op_Inequality(transformComponent.MapID, mapId))
      return false;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(baseUid);
    if (muzzle == null)
    {
      originCoords = moverCoordinates;
      return true;
    }
    Angle baseRotation = this.GetBaseRotation(baseUid, muzzle.AngleOffset);
    (Vector2 Offset, bool Rotate) offset1 = this.GetOffset(muzzle, baseUid, baseRotation);
    Vector2 offset2 = offset1.Offset;
    EntityCoordinates entityCoordinates = offset1.Rotate ? ((EntityCoordinates) ref moverCoordinates).Offset(((Angle) ref baseRotation).RotateVec(ref offset2)) : ((EntityCoordinates) ref moverCoordinates).Offset(offset2);
    if (muzzle.MuzzleOffset != Vector2.Zero)
    {
      Angle angle = baseRotation;
      GunComponent gunComponent;
      if (muzzle.UseAimDirection && this._gunQ.TryComp(uid, ref gunComponent))
      {
        EntityCoordinates? shootCoordinates = gunComponent.ShootCoordinates;
        if (shootCoordinates.HasValue)
        {
          EntityCoordinates valueOrDefault = shootCoordinates.GetValueOrDefault();
          MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(entityCoordinates, true);
          Vector2 vector2 = this._transform.ToMapCoordinates(valueOrDefault, true).Position - mapCoordinates.Position;
          if ((double) vector2.LengthSquared() > 9.9999997473787516E-05)
            angle = Angle.op_Addition(DirectionExtensions.ToWorldAngle(vector2), muzzle.AngleOffset);
        }
      }
      entityCoordinates = ((EntityCoordinates) ref entityCoordinates).Offset(((Angle) ref angle).RotateVec(ref muzzle.MuzzleOffset));
    }
    originCoords = entityCoordinates;
    return true;
  }

  private bool TryGetTurretOverlayPositions(
    EntityUid turretUid,
    VehicleTurretComponent turret,
    MapId mapId,
    out Vector2 basePos,
    out Vector2 anchorPos,
    out Vector2 turretPos)
  {
    basePos = new Vector2();
    anchorPos = new Vector2();
    turretPos = new Vector2();
    EntityUid vehicle;
    if (!this.TryGetVehicle(turretUid, out vehicle))
      return false;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(vehicle);
    MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(moverCoordinates, true);
    if (MapId.op_Inequality(mapCoordinates1.MapId, mapId))
      return false;
    EntityUid anchorUid;
    VehicleTurretComponent anchorTurret;
    this.TryGetAnchorTurret(turretUid, turret, out anchorUid, out anchorTurret);
    Angle worldRotation = this._transform.GetWorldRotation(vehicle);
    Angle rotation = this._eye.CurrentEye.Rotation;
    Angle vehicleFacingAngle = this.GetVehicleFacingAngle(vehicle, worldRotation);
    Angle renderFacing1 = VehicleHardpointDebugOverlay.GetRenderFacing(anchorTurret, anchorTurret, worldRotation, vehicleFacingAngle, rotation);
    Vector2 offset = this.GetPixelOffset(anchorTurret, renderFacing1) / 32f;
    Vector2 vehicleLocalOffset = VehicleHardpointDebugOverlay.GetVehicleLocalOffset(anchorTurret, offset, worldRotation, rotation);
    EntityCoordinates entityCoordinates = ((EntityCoordinates) ref moverCoordinates).Offset(vehicleLocalOffset);
    basePos = mapCoordinates1.Position;
    anchorPos = this._transform.ToMapCoordinates(entityCoordinates, true).Position;
    if (EntityUid.op_Equality(anchorUid, turretUid))
    {
      turretPos = anchorPos;
      return true;
    }
    Angle angle1 = anchorTurret.RotateToCursor ? anchorTurret.WorldRotation : Angle.Zero;
    Angle renderFacing2 = VehicleHardpointDebugOverlay.GetRenderFacing(turret, anchorTurret, worldRotation, vehicleFacingAngle, rotation);
    Vector2 vector2_1 = this.GetPixelOffset(turret, renderFacing2) / 32f;
    Vector2 vector2_2;
    Vector2 vector2_3;
    if (turret.OffsetRotatesWithTurret)
    {
      if (turret.UseDirectionalOffsets)
      {
        Angle directionalAngle = VehicleHardpointDebugOverlay.GetDirectionalAngle(VehicleHardpointDebugOverlay.GetDirectionalDir(renderFacing2));
        Angle angle2 = Angle.op_UnaryNegation(directionalAngle);
        vector2_2 = ((Angle) ref angle2).RotateVec(ref vector2_1);
        Angle angle3 = Angle.op_Subtraction(angle1, directionalAngle);
        vector2_3 = ((Angle) ref angle3).RotateVec(ref vector2_1);
      }
      else
      {
        vector2_2 = vector2_1;
        vector2_3 = ((Angle) ref angle1).RotateVec(ref vector2_2);
      }
    }
    else
    {
      Angle angle4 = Angle.op_UnaryNegation(worldRotation);
      Vector2 vector2_4 = ((Angle) ref angle4).RotateVec(ref vector2_1);
      angle4 = Angle.op_UnaryNegation(angle1);
      vector2_2 = ((Angle) ref angle4).RotateVec(ref vector2_4);
    }
    MapCoordinates mapCoordinates2 = !turret.OffsetRotatesWithTurret ? this._transform.ToMapCoordinates(((EntityCoordinates) ref moverCoordinates).Offset(vehicleLocalOffset + vector2_2), true) : this._transform.ToMapCoordinates(new EntityCoordinates(anchorUid, vector2_2), true);
    if (MapId.op_Inequality(mapCoordinates2.MapId, mapId))
      return false;
    turretPos = mapCoordinates2.Position;
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
    Direction directionalDir = VehicleHardpointDebugOverlay.GetDirectionalDir((float) normalized);
    Vector2 directionalOffset = VehicleHardpointDebugOverlay.GetDirectionalOffset(turret, directionalDir);
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

  private static Vector2 GetDirectionalGunOffset(GunMuzzleOffsetComponent muzzle, Direction dir)
  {
    Vector2 directionalGunOffset;
    switch ((int) dir)
    {
      case 0:
        directionalGunOffset = muzzle.OffsetSouth;
        break;
      case 2:
        directionalGunOffset = muzzle.OffsetEast;
        break;
      case 4:
        directionalGunOffset = muzzle.OffsetNorth;
        break;
      case 6:
        directionalGunOffset = muzzle.OffsetWest;
        break;
      default:
        directionalGunOffset = muzzle.Offset;
        break;
    }
    return directionalGunOffset;
  }

  private static Vector2 GetDirectionalTurretOffset(
    VehicleTurretMuzzleComponent muzzle,
    Direction dir,
    bool useRight)
  {
    if (!muzzle.UseDirectionalOffsets)
      return !useRight ? muzzle.OffsetLeft : muzzle.OffsetRight;
    Vector2 directionalTurretOffset;
    switch ((int) dir)
    {
      case 0:
        directionalTurretOffset = useRight ? muzzle.OffsetRightSouth : muzzle.OffsetLeftSouth;
        break;
      case 2:
        directionalTurretOffset = useRight ? muzzle.OffsetRightEast : muzzle.OffsetLeftEast;
        break;
      case 4:
        directionalTurretOffset = useRight ? muzzle.OffsetRightNorth : muzzle.OffsetLeftNorth;
        break;
      case 6:
        directionalTurretOffset = useRight ? muzzle.OffsetRightWest : muzzle.OffsetLeftWest;
        break;
      default:
        directionalTurretOffset = useRight ? muzzle.OffsetRight : muzzle.OffsetLeft;
        break;
    }
    return directionalTurretOffset;
  }

  private static Vector2 GetTurretMuzzleWorldOffset(
    VehicleTurretMuzzleComponent muzzle,
    Angle baseRotation,
    bool useRight)
  {
    Vector2 directionalTurretOffset = VehicleHardpointDebugOverlay.GetDirectionalTurretOffset(muzzle, VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(baseRotation), useRight);
    return ((Angle) ref baseRotation).RotateVec(ref directionalTurretOffset);
  }

  private void DrawShootArc(EntityUid uid, Vector2 origin, MapId mapId, DrawingHandleWorld handle)
  {
    GunFireArcComponent fireArcComponent;
    BaseContainer baseContainer;
    TransformComponent transformComponent;
    if (!this._fireArcQ.TryComp(uid, ref fireArcComponent) || !this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null)), ref baseContainer) || !this._ents.TryGetComponent<TransformComponent>(baseContainer.Owner, ref transformComponent) || MapId.op_Inequality(transformComponent.MapID, mapId))
      return;
    Angle baseRotation = this.GetBaseRotation(baseContainer.Owner, fireArcComponent.AngleOffset);
    Angle angle1 = Angle.FromDegrees(((Angle) ref fireArcComponent.Arc).Degrees / 2.0);
    Angle angle2 = Angle.op_Addition(baseRotation, angle1);
    Angle angle3 = Angle.op_Subtraction(baseRotation, angle1);
    ((DrawingHandleBase) handle).DrawLine(origin, origin + ((Angle) ref baseRotation).ToWorldVec() * 3.5f, new Color(0.2f, 0.9f, 0.3f, 0.8f));
    ((DrawingHandleBase) handle).DrawLine(origin, origin + ((Angle) ref angle2).ToWorldVec() * 3.5f, new Color(0.95f, 0.45f, 0.2f, 0.8f));
    ((DrawingHandleBase) handle).DrawLine(origin, origin + ((Angle) ref angle3).ToWorldVec() * 3.5f, new Color(0.95f, 0.45f, 0.2f, 0.8f));
  }

  private void DrawShootTarget(
    EntityUid uid,
    Vector2 origin,
    MapId mapId,
    DrawingHandleWorld handle)
  {
    GunComponent gunComponent;
    if (!this._gunQ.TryComp(uid, ref gunComponent) || !gunComponent.ShootCoordinates.HasValue)
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(gunComponent.ShootCoordinates.Value, true);
    if (MapId.op_Inequality(mapCoordinates.MapId, mapId))
      return;
    ((DrawingHandleBase) handle).DrawLine(origin, mapCoordinates.Position, new Color(0.9f, 0.2f, 0.9f, 0.7f));
    ((DrawingHandleBase) handle).DrawCircle(mapCoordinates.Position, 0.08f, new Color(0.9f, 0.2f, 0.9f, 0.8f), true);
  }

  private Angle GetBaseRotation(EntityUid baseUid, Angle angleOffset)
  {
    Angle angle = this._transform.GetWorldRotation(baseUid);
    GridVehicleMoverComponent vehicleMoverComponent;
    if (this._moverQ.TryComp(baseUid, ref vehicleMoverComponent) && Vector2i.op_Inequality(vehicleMoverComponent.CurrentDirection, Vector2i.Zero))
      angle = DirectionExtensions.ToWorldAngle(new Vector2((float) vehicleMoverComponent.CurrentDirection.X, (float) vehicleMoverComponent.CurrentDirection.Y));
    return Angle.op_Addition(angle, angleOffset);
  }

  private Angle GetVehicleFacingAngle(EntityUid vehicle, Angle vehicleRot)
  {
    GridVehicleMoverComponent vehicleMoverComponent;
    return this._moverQ.TryComp(vehicle, ref vehicleMoverComponent) && Vector2i.op_Inequality(vehicleMoverComponent.CurrentDirection, Vector2i.Zero) ? DirectionExtensions.ToWorldAngle(new Vector2((float) vehicleMoverComponent.CurrentDirection.X, (float) vehicleMoverComponent.CurrentDirection.Y)) : vehicleRot;
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

  private static Angle GetRenderFacing(
    VehicleTurretComponent turret,
    VehicleTurretComponent anchorTurret,
    Angle vehicleRot,
    Angle baseFacingAngle,
    Angle eyeRot)
  {
    Angle angle = Angle.op_Addition(VehicleHardpointDebugOverlay.GetOffsetFacing(turret, anchorTurret, vehicleRot, baseFacingAngle), eyeRot);
    return ((Angle) ref angle).Reduced();
  }

  private static Angle GetOffsetFacing(
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

  private (Vector2 Offset, bool Rotate) GetOffset(
    GunMuzzleOffsetComponent muzzle,
    EntityUid baseUid,
    Angle baseRotation)
  {
    if (!muzzle.UseDirectionalOffsets)
      return (muzzle.Offset, true);
    Vector2 vector2;
    switch ((int) this.GetBaseDirection(baseUid, baseRotation))
    {
      case 0:
        vector2 = muzzle.OffsetSouth;
        break;
      case 2:
        vector2 = muzzle.OffsetEast;
        break;
      case 4:
        vector2 = muzzle.OffsetNorth;
        break;
      case 6:
        vector2 = muzzle.OffsetWest;
        break;
      default:
        vector2 = muzzle.Offset;
        break;
    }
    return (vector2, muzzle.RotateDirectionalOffsets);
  }

  private Direction GetBaseDirection(EntityUid baseUid, Angle baseRotation)
  {
    GridVehicleMoverComponent vehicleMoverComponent;
    return this._moverQ.TryComp(baseUid, ref vehicleMoverComponent) && Vector2i.op_Inequality(vehicleMoverComponent.CurrentDirection, Vector2i.Zero) ? DirectionExtensions.AsDirection(vehicleMoverComponent.CurrentDirection) : ((Angle) ref baseRotation).GetCardinalDir();
  }

  private bool TryGetVehicle(EntityUid turretUid, out EntityUid vehicle)
  {
    vehicle = new EntityUid();
    BaseContainer baseContainer;
    EntityUid owner;
    for (EntityUid entityUid = turretUid; this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((entityUid, (TransformComponent) null)), ref baseContainer); entityUid = owner)
    {
      owner = baseContainer.Owner;
      if (this._ents.HasComponent<VehicleComponent>(owner))
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
    if (!this._ents.HasComponent<VehicleTurretAttachmentComponent>(turretUid) || !this.TryGetParentTurret(turretUid, out parentUid, out parentTurret))
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
      if (this._turretQ.TryComp(owner, ref vehicleTurretComponent))
      {
        parentUid = owner;
        parentTurret = vehicleTurretComponent;
        return true;
      }
    }
    return false;
  }

  private readonly record struct DebugLine(string Text, Color Color);

  private readonly record struct DebugLabel(
    Vector2 WorldPosition,
    List<VehicleHardpointDebugOverlay.DebugLine> Lines)
  ;
}
