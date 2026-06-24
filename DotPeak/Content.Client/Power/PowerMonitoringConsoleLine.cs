// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.PowerMonitoringConsoleLine
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System.Numerics;

#nullable disable
namespace Content.Client.Power;

public struct PowerMonitoringConsoleLine(
  Vector2 origin,
  Vector2 terminus,
  PowerMonitoringConsoleLineGroup group)
{
  public readonly Vector2 Origin = origin;
  public readonly Vector2 Terminus = terminus;
  public readonly PowerMonitoringConsoleLineGroup Group = group;
}
