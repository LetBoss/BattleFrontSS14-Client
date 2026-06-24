// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.Consoles.AtmosMonitoringConsoleLine
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Maths;
using System.Numerics;

#nullable disable
namespace Content.Client.Atmos.Consoles;

public struct AtmosMonitoringConsoleLine(Vector2 origin, Vector2 terminus, Color color)
{
  public readonly Vector2 Origin = origin;
  public readonly Vector2 Terminus = terminus;
  public readonly Color Color = color;
}
