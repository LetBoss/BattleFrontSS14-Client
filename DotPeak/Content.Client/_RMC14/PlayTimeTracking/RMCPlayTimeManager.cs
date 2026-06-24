// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.PlayTimeTracking.RMCPlayTimeManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.PlayTimeTracking;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.PlayTimeTracking;

public sealed class RMCPlayTimeManager : IPostInjectInit
{
  [Dependency]
  private INetManager _net;
  private readonly HashSet<string> _excluded = new HashSet<string>();

  public event Action? Updated;

  private void OnExcludedTimers(RMCExcludedTimersMsg message)
  {
    this._excluded.Clear();
    this._excluded.UnionWith((IEnumerable<string>) message.Trackers);
    Action updated = this.Updated;
    if (updated == null)
      return;
    updated();
  }

  public bool IsExcluded(string tracker) => this._excluded.Contains(tracker);

  void IPostInjectInit.PostInject()
  {
    // ISSUE: method pointer
    this._net.RegisterNetMessage<RMCExcludedTimersMsg>(new ProcessMessage<RMCExcludedTimersMsg>((object) this, __methodptr(OnExcludedTimers)), (NetMessageAccept) 3);
  }
}
