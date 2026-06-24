// Decompiled with JetBrains decompiler
// Type: Content.Client.DebugMon.DebugMonitorManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Shared.CCVar;
using Robust.Client;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.DebugMon;

internal sealed class DebugMonitorManager
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IClientAdminManager _admin;
  [Dependency]
  private IUserInterfaceManager _userInterface;
  [Dependency]
  private IBaseClient _baseClient;

  public void FrameUpdate()
  {
    if (this._baseClient.RunLevel != 4 || this._admin.IsActive() || !this._cfg.GetCVar<bool>(CCVars.DebugCoordinatesAdminOnly))
      return;
    this._userInterface.DebugMonitors.SetMonitor((DebugMonitor) 1, false);
  }
}
