// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tracker.TrackerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Tracker;

public sealed class TrackerSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  public static readonly short CenterSeverity = 1;
  private static readonly Dictionary<Direction, short> AlertSeverity = new Dictionary<Direction, short>()
  {
    {
      (Direction) -1,
      (short) 0
    },
    {
      (Direction) 0,
      (short) 2
    },
    {
      (Direction) 1,
      (short) 3
    },
    {
      (Direction) 2,
      (short) 4
    },
    {
      (Direction) 3,
      (short) 5
    },
    {
      (Direction) 4,
      (short) 6
    },
    {
      (Direction) 5,
      (short) 7
    },
    {
      (Direction) 6,
      (short) 8
    },
    {
      (Direction) 7,
      (short) 9
    }
  };
  private Robust.Shared.GameObjects.EntityQuery<InputMoverComponent> _inputMoverQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._inputMoverQuery = this.GetEntityQuery<InputMoverComponent>();
  }

  public short GetAlertSeverity(EntityUid ent, MapCoordinates tracked)
  {
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(ent);
    if (mapCoordinates.MapId != tracked.MapId)
      return TrackerSystem.CenterSeverity;
    Vector2 vector2 = tracked.Position - mapCoordinates.Position;
    if ((double) vector2.Length() < 1.0)
      return TrackerSystem.CenterSeverity;
    InputMoverComponent component;
    if (this._inputMoverQuery.TryComp(ent, out component) && Angle.op_Inequality(component.RelativeRotation, Angle.Zero))
    {
      Angle angle = Angle.op_UnaryNegation(component.RelativeRotation);
      vector2 = ((Angle) ref angle).RotateVec(ref vector2);
    }
    Angle worldAngle = DirectionExtensions.ToWorldAngle(vector2);
    Direction dir = ((Angle) ref worldAngle).GetDir();
    return TrackerSystem.AlertSeverity.GetValueOrDefault<Direction, short>(dir, TrackerSystem.CenterSeverity);
  }
}
