// Decompiled with JetBrains decompiler
// Type: Content.Client.Medical.CrewMonitoring.CrewMonitoringButton
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

#nullable disable
namespace Content.Client.Medical.CrewMonitoring;

public sealed class CrewMonitoringButton : Button
{
  public int IndexInTable;
  public NetEntity SuitSensorUid;
  public EntityCoordinates? Coordinates;
}
