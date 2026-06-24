// Decompiled with JetBrains decompiler
// Type: Content.Client.Eui.BaseEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Eui;
using Robust.Shared.IoC;
using Robust.Shared.Network;

#nullable enable
namespace Content.Client.Eui;

public abstract class BaseEui
{
  [Dependency]
  private IClientNetManager _netManager;

  public EuiManager Manager { get; private set; }

  public uint Id { get; private set; }

  protected BaseEui() => IoCManager.InjectDependencies<BaseEui>(this);

  internal void Initialize(EuiManager mgr, uint id)
  {
    this.Manager = mgr;
    this.Id = id;
  }

  public virtual void Opened()
  {
  }

  public virtual void Closed()
  {
  }

  public virtual void HandleState(EuiStateBase state)
  {
  }

  public virtual void HandleMessage(EuiMessageBase msg)
  {
  }

  protected void SendMessage(EuiMessageBase msg)
  {
    ((INetManager) this._netManager).ClientSendMessage((NetMessage) new MsgEuiMessage()
    {
      Id = this.Id,
      Message = msg
    });
  }
}
