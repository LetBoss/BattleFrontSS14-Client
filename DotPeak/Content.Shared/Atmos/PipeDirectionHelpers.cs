// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.PipeDirectionHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System;

#nullable disable
namespace Content.Shared.Atmos;

public static class PipeDirectionHelpers
{
  public const int PipeDirections = 4;
  public const int AllPipeDirections = 6;

  public static bool HasDirection(this PipeDirection pipeDirection, PipeDirection other)
  {
    return (pipeDirection & other) == other;
  }

  public static Angle ToAngle(this PipeDirection pipeDirection)
  {
    return DirectionExtensions.ToAngle(pipeDirection.ToDirection());
  }

  public static PipeDirection ToPipeDirection(this Direction direction)
  {
    switch ((int) direction)
    {
      case 0:
        return PipeDirection.South;
      case 2:
        return PipeDirection.East;
      case 4:
        return PipeDirection.North;
      case 6:
        return PipeDirection.West;
      default:
        throw new ArgumentOutOfRangeException(nameof (direction));
    }
  }

  public static Direction ToDirection(this PipeDirection pipeDirection)
  {
    switch (pipeDirection)
    {
      case PipeDirection.North:
        return (Direction) 4;
      case PipeDirection.South:
        return (Direction) 0;
      case PipeDirection.West:
        return (Direction) 6;
      case PipeDirection.East:
        return (Direction) 2;
      default:
        throw new ArgumentOutOfRangeException(nameof (pipeDirection));
    }
  }

  public static PipeDirection GetOpposite(this PipeDirection pipeDirection)
  {
    switch (pipeDirection)
    {
      case PipeDirection.North:
        return PipeDirection.South;
      case PipeDirection.South:
        return PipeDirection.North;
      case PipeDirection.West:
        return PipeDirection.East;
      case PipeDirection.East:
        return PipeDirection.West;
      default:
        throw new ArgumentOutOfRangeException(nameof (pipeDirection));
    }
  }

  public static PipeShape PipeDirectionToPipeShape(this PipeDirection pipeDirection)
  {
    switch (pipeDirection)
    {
      case PipeDirection.North:
        return PipeShape.Half;
      case PipeDirection.South:
        return PipeShape.Half;
      case PipeDirection.Longitudinal:
        return PipeShape.Straight;
      case PipeDirection.West:
        return PipeShape.Half;
      case PipeDirection.NWBend:
        return PipeShape.Bend;
      case PipeDirection.SWBend:
        return PipeShape.Bend;
      case PipeDirection.TWest:
        return PipeShape.TJunction;
      case PipeDirection.East:
        return PipeShape.Half;
      case PipeDirection.NEBend:
        return PipeShape.Bend;
      case PipeDirection.SEBend:
        return PipeShape.Bend;
      case PipeDirection.TEast:
        return PipeShape.TJunction;
      case PipeDirection.Lateral:
        return PipeShape.Straight;
      case PipeDirection.TNorth:
        return PipeShape.TJunction;
      case PipeDirection.TSouth:
        return PipeShape.TJunction;
      case PipeDirection.Fourway:
        return PipeShape.Fourway;
      default:
        throw new ArgumentOutOfRangeException(nameof (pipeDirection));
    }
  }

  public static PipeDirection RotatePipeDirection(this PipeDirection pipeDirection, double diff)
  {
    PipeDirection pipeDirection1 = PipeDirection.None;
    for (int index = 0; index < 4; ++index)
    {
      PipeDirection pipeDirection2 = (PipeDirection) (1 << index);
      if (pipeDirection.HasFlag((Enum) pipeDirection2))
      {
        Angle angle = Angle.op_Addition(pipeDirection2.ToAngle(), Angle.op_Implicit(diff));
        pipeDirection1 |= ((Angle) ref angle).GetCardinalDir().ToPipeDirection();
      }
    }
    return pipeDirection1;
  }
}
