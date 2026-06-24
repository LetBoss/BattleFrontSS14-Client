// Decompiled with JetBrains decompiler
// Type: Content.Client.Gateway.UI.GatewayBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Gateway;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Gateway.UI;

public sealed class GatewayBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private GatewayWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<GatewayWindow>((BoundUserInterface) this);
    this._window.SetEntity(this.EntMan.GetNetEntity(this.Owner, (MetaDataComponent) null));
    this._window.OpenPortal += (Action<NetEntity>) (destination => this.SendMessage((BoundUserInterfaceMessage) new GatewayOpenPortalMessage(destination)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is GatewayBoundUserInterfaceState state1))
      return;
    this._window?.UpdateState(state1);
  }
}
