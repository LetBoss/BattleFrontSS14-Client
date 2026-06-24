// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.AtmosDirectionHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Atmos;

public static class AtmosDirectionHelpers
{
  public static AtmosDirection GetOpposite(this AtmosDirection direction)
  {
    switch (direction)
    {
      case AtmosDirection.North:
        return AtmosDirection.South;
      case AtmosDirection.South:
        return AtmosDirection.North;
      case AtmosDirection.East:
        return AtmosDirection.West;
      case AtmosDirection.NorthEast:
        return AtmosDirection.SouthWest;
      case AtmosDirection.SouthEast:
        return AtmosDirection.NorthWest;
      case AtmosDirection.West:
        return AtmosDirection.East;
      case AtmosDirection.NorthWest:
        return AtmosDirection.SouthEast;
      case AtmosDirection.SouthWest:
        return AtmosDirection.NorthEast;
      default:
        throw new ArgumentOutOfRangeException(nameof (direction));
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int ToOppositeIndex(this int index) => index ^ 1;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static AtmosDirection ToOppositeDir(this int index) => (AtmosDirection) (1 << (index ^ 1));

  public static Direction ToDirection(this AtmosDirection direction)
  {
    switch (direction)
    {
      case AtmosDirection.Invalid:
        return (Direction) -1;
      case AtmosDirection.North:
        return (Direction) 4;
      case AtmosDirection.South:
        return (Direction) 0;
      case AtmosDirection.East:
        return (Direction) 2;
      case AtmosDirection.NorthEast:
        return (Direction) 3;
      case AtmosDirection.SouthEast:
        return (Direction) 1;
      case AtmosDirection.West:
        return (Direction) 6;
      case AtmosDirection.NorthWest:
        return (Direction) 5;
      case AtmosDirection.SouthWest:
        return (Direction) 7;
      default:
        throw new ArgumentOutOfRangeException(nameof (direction));
    }
  }

  public static AtmosDirection ToAtmosDirection(this Direction direction)
  {
    switch (direction - -1)
    {
      case 0:
        return AtmosDirection.Invalid;
      case 1:
        return AtmosDirection.South;
      case 2:
        return AtmosDirection.SouthEast;
      case 3:
        return AtmosDirection.East;
      case 4:
        return AtmosDirection.NorthEast;
      case 5:
        return AtmosDirection.North;
      case 6:
        return AtmosDirection.NorthWest;
      case 7:
        return AtmosDirection.West;
      case 8:
        return AtmosDirection.SouthWest;
      default:
        throw new ArgumentOutOfRangeException(nameof (direction));
    }
  }

  public static Angle ToAngle(this AtmosDirection direction)
  {
    switch (direction)
    {
      case AtmosDirection.North:
        return new Angle(Math.PI);
      case AtmosDirection.South:
        return Angle.Zero;
      case AtmosDirection.East:
        return new Angle(1.5707963705062866);
      case AtmosDirection.NorthEast:
        return new Angle(3.0 * Math.PI / 4.0);
      case AtmosDirection.SouthEast:
        return new Angle(0.78539818525314331);
      case AtmosDirection.West:
        return new Angle(-1.5707963705062866);
      case AtmosDirection.NorthWest:
        return new Angle(-3.0 * Math.PI / 4.0);
      case AtmosDirection.SouthWest:
        return new Angle(-0.78539818525314331);
      default:
        throw new ArgumentOutOfRangeException(nameof (direction), $"It was {direction}.");
    }
  }

  public static AtmosDirection ToAtmosDirectionCardinal(this Angle angle)
  {
    return ((Angle) ref angle).GetCardinalDir().ToAtmosDirection();
  }

  public static AtmosDirection ToAtmosDirection(this Angle angle)
  {
    return ((Angle) ref angle).GetDir().ToAtmosDirection();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int ToIndex(this AtmosDirection direction) => BitOperations.Log2((uint) direction);

  public static AtmosDirection WithFlag(this AtmosDirection direction, AtmosDirection other)
  {
    return direction | other;
  }

  public static AtmosDirection WithoutFlag(this AtmosDirection direction, AtmosDirection other)
  {
    return direction & ~other;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsFlagSet(this AtmosDirection direction, AtmosDirection other)
  {
    return (direction & other) == other;
  }

  public static Vector2i CardinalToIntVec(this AtmosDirection dir)
  {
    switch (dir)
    {
      case AtmosDirection.North:
        return new Vector2i(0, 1);
      case AtmosDirection.South:
        return new Vector2i(0, -1);
      case AtmosDirection.East:
        return new Vector2i(1, 0);
      case AtmosDirection.West:
        return new Vector2i(-1, 0);
      default:
        throw new ArgumentException($"Direction dir {dir} is not a cardinal direction", nameof (dir));
    }
  }

  public static Vector2i Offset(this Vector2i pos, AtmosDirection dir)
  {
    return Vector2i.op_Addition(pos, dir.CardinalToIntVec());
  }
}
