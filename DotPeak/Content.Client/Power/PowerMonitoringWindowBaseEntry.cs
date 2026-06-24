// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.PowerMonitoringWindowBaseEntry
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Power;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Client.Power;

public abstract class PowerMonitoringWindowBaseEntry : BoxContainer
{
  public NetEntity NetEntity;
  public PowerMonitoringConsoleEntry Entry;
  public PowerMonitoringButton Button;

  public PowerMonitoringWindowBaseEntry(PowerMonitoringConsoleEntry entry)
  {
    this.Entry = entry;
    this.Button = new PowerMonitoringButton();
  }
}
