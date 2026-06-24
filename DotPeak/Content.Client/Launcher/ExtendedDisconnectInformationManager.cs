// Decompiled with JetBrains decompiler
// Type: Content.Client.Launcher.ExtendedDisconnectInformationManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Client.Launcher;

public sealed class ExtendedDisconnectInformationManager
{
  [Dependency]
  private IClientNetManager _clientNetManager;
  private NetDisconnectedArgs? _lastNetDisconnectedArgs;

  public NetDisconnectedArgs? LastNetDisconnectedArgs
  {
    get => this._lastNetDisconnectedArgs;
    private set
    {
      this._lastNetDisconnectedArgs = value;
      Action<NetDisconnectedArgs> disconnectedArgsChanged = this.LastNetDisconnectedArgsChanged;
      if (disconnectedArgsChanged == null)
        return;
      disconnectedArgsChanged(value);
    }
  }

  public event Action<NetDisconnectedArgs?>? LastNetDisconnectedArgsChanged;

  public void Initialize()
  {
    ((INetManager) this._clientNetManager).Disconnect += new EventHandler<NetDisconnectedArgs>(this.OnNetDisconnect);
  }

  private void OnNetDisconnect(object? sender, NetDisconnectedArgs args)
  {
    this.LastNetDisconnectedArgs = args;
  }
}
