// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Maths.RMCDirectionExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared._RMC14.Maths;

public static class RMCDirectionExtensions
{
  public static string GetShorthand(this Direction direction)
  {
    string shorthand;
    switch ((int) direction)
    {
      case 0:
        shorthand = "S";
        break;
      case 1:
        shorthand = "SE";
        break;
      case 2:
        shorthand = "E";
        break;
      case 3:
        shorthand = "NE";
        break;
      case 4:
        shorthand = "N";
        break;
      case 5:
        shorthand = "NW";
        break;
      case 6:
        shorthand = "W";
        break;
      case 7:
        shorthand = "SW";
        break;
      default:
        shorthand = string.Empty;
        break;
    }
    return shorthand;
  }
}
