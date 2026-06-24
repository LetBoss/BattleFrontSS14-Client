// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Map.RMCDirectionExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System;

#nullable disable
namespace Content.Shared._RMC14.Map;

public static class RMCDirectionExtensions
{
  public static (Direction First, Direction Second) GetPerpendiculars(this Direction direction)
  {
    switch ((int) direction)
    {
      case 0:
      case 4:
        return ((Direction) 6, (Direction) 2);
      case 1:
      case 5:
        return ((Direction) 7, (Direction) 3);
      case 2:
      case 6:
        return ((Direction) 4, (Direction) 0);
      case 3:
      case 7:
        return ((Direction) 5, (Direction) 1);
      default:
        throw new ArgumentOutOfRangeException(nameof (direction), (object) direction, (string) null);
    }
  }

  public static bool IsCardinal(this Direction direction)
  {
    bool flag;
    switch ((int) direction)
    {
      case 0:
      case 2:
      case 4:
      case 6:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }
}
