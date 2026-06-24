// Decompiled with JetBrains decompiler
// Type: Content.Client.Eui.EuiManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Eui;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Reflection;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Eui;

public sealed class EuiManager
{
  [Dependency]
  private IClientNetManager _net;
  [Dependency]
  private IReflectionManager _refl;
  [Dependency]
  private IDynamicTypeFactory _dtf;
  private readonly Dictionary<uint, EuiManager.EuiData> _openUis = new Dictionary<uint, EuiManager.EuiData>();

  public void Initialize()
  {
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MsgEuiCtl>(new ProcessMessage<MsgEuiCtl>((object) this, __methodptr(RxMsgCtl)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MsgEuiState>(new ProcessMessage<MsgEuiState>((object) this, __methodptr(RxMsgState)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MsgEuiMessage>(new ProcessMessage<MsgEuiMessage>((object) this, __methodptr(RxMsgMessage)), (NetMessageAccept) 3);
    ((INetManager) this._net).Disconnect += new EventHandler<NetDisconnectedArgs>(this.NetOnDisconnect);
  }

  private void NetOnDisconnect(object? sender, NetDisconnectedArgs e)
  {
    foreach (KeyValuePair<uint, EuiManager.EuiData> openUi in this._openUis)
      openUi.Value.Eui.Closed();
    this._openUis.Clear();
  }

  private void RxMsgMessage(MsgEuiMessage message)
  {
    this._openUis[message.Id].Eui.HandleMessage(message.Message);
  }

  private void RxMsgState(MsgEuiState message)
  {
    this._openUis[message.Id].Eui.HandleState(message.State);
  }

  private void RxMsgCtl(MsgEuiCtl message)
  {
    EuiManager.EuiData euiData;
    if (this._openUis.TryGetValue(message.Id, out euiData))
    {
      euiData.Eui.Closed();
      this._openUis.Remove(message.Id);
    }
    if (message.Type != MsgEuiCtl.CtlType.Open)
      return;
    BaseEui instance = DynamicTypeFactoryExt.CreateInstance<BaseEui>(this._dtf, this._refl.LooseGetType(message.OpenType), false, true);
    instance.Initialize(this, message.Id);
    instance.Opened();
    this._openUis.Add(message.Id, new EuiManager.EuiData(instance));
  }

  private sealed class EuiData
  {
    public readonly BaseEui Eui;

    public EuiData(BaseEui eui) => this.Eui = eui;
  }
}
