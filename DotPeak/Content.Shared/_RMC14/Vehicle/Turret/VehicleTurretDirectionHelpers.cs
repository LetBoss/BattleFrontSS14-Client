// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleTurretDirectionHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System;

#nullable disable
namespace Content.Shared._RMC14.Vehicle;

public static class VehicleTurretDirectionHelpers
{
  private const double SpriteDirectionBiasRadians = -0.05;

  public static Direction GetRenderAlignedCardinalDir(Angle facing)
  {
    Angle angle1 = ((Angle) ref facing).Reduced();
    Angle angle2 = ((Angle) ref angle1).FlipPositive();
    Direction alignedCardinalDir;
    switch ((int) Math.Round((angle2.Theta + (Math.Floor(angle2.Theta / 1.5707963705062866) % 2.0 - 0.5) * -0.05) / 1.5707963705062866) % 4)
    {
      case 0:
        alignedCardinalDir = (Direction) 0;
        break;
      case 1:
        alignedCardinalDir = (Direction) 2;
        break;
      case 2:
        alignedCardinalDir = (Direction) 4;
        break;
      default:
        alignedCardinalDir = (Direction) 6;
        break;
    }
    return alignedCardinalDir;
  }
}
