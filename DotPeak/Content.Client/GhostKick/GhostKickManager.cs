// Decompiled with JetBrains decompiler
// Type: Content.Client.GhostKick.GhostKickManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.GhostKick;
using Robust.Client;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Client.GhostKick;

public sealed class GhostKickManager
{
  private bool _fakeLossEnabled;
  [Dependency]
  private IBaseClient _baseClient;
  [Dependency]
  private IClientNetManager _netManager;
  [Dependency]
  private IConfigurationManager _cfg;

  public void Initialize()
  {
    // ISSUE: method pointer
    ((INetManager) this._netManager).RegisterNetMessage<MsgGhostKick>(new ProcessMessage<MsgGhostKick>((object) this, __methodptr(RxCallback)), (NetMessageAccept) 3);
    this._baseClient.RunLevelChanged += new EventHandler<RunLevelChangedEventArgs>(this.BaseClientOnRunLevelChanged);
  }

  private void BaseClientOnRunLevelChanged(object? sender, RunLevelChangedEventArgs e)
  {
    if (!this._fakeLossEnabled || e.OldLevel != 4)
      return;
    this._cfg.SetCVar<float>(CVars.NetFakeLoss, 0.0f, false);
    this._fakeLossEnabled = false;
  }

  private void RxCallback(MsgGhostKick message)
  {
    this._fakeLossEnabled = true;
    this._cfg.SetCVar<float>(CVars.NetFakeLoss, 1f, false);
  }
}
