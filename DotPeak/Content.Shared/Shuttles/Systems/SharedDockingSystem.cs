// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.Systems.SharedDockingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared.Shuttles.Systems;

public abstract class SharedDockingSystem : EntitySystem
{
  [Dependency]
  protected SharedTransformSystem XformSystem;
  public const float DockingHiglightRange = 4f;
  public const float DockRange = 1.2f;
  public static readonly double AlignmentTolerance = Angle.FromDegrees(15.0).Theta;

  public bool CanShuttleDock(EntityUid? shuttle)
  {
    return shuttle.HasValue && !this.HasComp<PreventPilotComponent>(shuttle.Value);
  }

  public bool CanShuttleUndock(EntityUid? shuttle)
  {
    return shuttle.HasValue && !this.HasComp<PreventPilotComponent>(shuttle.Value);
  }

  public bool CanDock(
    MapCoordinates mapPosA,
    Angle worldRotA,
    MapCoordinates mapPosB,
    Angle worldRotB)
  {
    return !(mapPosA.MapId != mapPosB.MapId) && this.InRange(mapPosA, mapPosB) && this.InAlignment(mapPosA, worldRotA, mapPosB, worldRotB);
  }

  public bool InRange(MapCoordinates mapPosA, MapCoordinates mapPosB)
  {
    return (double) (mapPosA.Position - mapPosB.Position).Length() <= 1.2000000476837158;
  }

  public bool InAlignment(
    MapCoordinates mapPosA,
    Angle worldRotA,
    MapCoordinates mapPosB,
    Angle worldRotB)
  {
    Angle worldAngle1 = DirectionExtensions.ToWorldAngle(mapPosB.Position - mapPosA.Position);
    Angle worldAngle2 = DirectionExtensions.ToWorldAngle(mapPosA.Position - mapPosB.Position);
    Angle angle1 = Angle.op_Subtraction(worldRotA, worldAngle1);
    angle1 = ((Angle) ref angle1).Reduced();
    ref Angle local1 = ref angle1;
    Angle zero1 = Angle.Zero;
    ref Angle local2 = ref zero1;
    Angle angle2 = Angle.ShortestDistance(ref local1, ref local2);
    angle1 = Angle.op_Subtraction(worldRotB, worldAngle2);
    angle1 = ((Angle) ref angle1).Reduced();
    ref Angle local3 = ref angle1;
    Angle zero2 = Angle.Zero;
    ref Angle local4 = ref zero2;
    Angle angle3 = Angle.ShortestDistance(ref local3, ref local4);
    return Math.Abs(angle2.Theta) <= SharedDockingSystem.AlignmentTolerance && Math.Abs(angle3.Theta) <= SharedDockingSystem.AlignmentTolerance;
  }

  public bool CanDock(
    NetCoordinates coordinatesOne,
    Angle angleOne,
    NetCoordinates coordinatesTwo,
    Angle angleTwo)
  {
    EntityCoordinates coordinates1 = this.GetCoordinates(coordinatesOne);
    EntityCoordinates coordinates2 = this.GetCoordinates(coordinatesTwo);
    MapCoordinates mapCoordinates1 = this.XformSystem.ToMapCoordinates(coordinates1);
    MapCoordinates mapCoordinates2 = this.XformSystem.ToMapCoordinates(coordinates2);
    Angle worldRotA = Angle.op_Addition(this.XformSystem.GetWorldRotation(coordinates1.EntityId), angleOne);
    Angle worldRotB = Angle.op_Addition(this.XformSystem.GetWorldRotation(coordinates2.EntityId), angleTwo);
    return this.CanDock(mapCoordinates1, worldRotA, mapCoordinates2, worldRotB);
  }
}
