// Decompiled with JetBrains decompiler
// Type: Content.Client.SensorMonitoring.SensorMonitoringConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Computer;
using Content.Shared.SensorMonitoring;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.SensorMonitoring;

public sealed class SensorMonitoringConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : 
  ComputerBoundUserInterface<SensorMonitoringWindow, SensorMonitoringConsoleBoundInterfaceState>(owner, uiKey)
{
}
