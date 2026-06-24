// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Lobby.RMCLobbyUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.JoinXeno;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client._RMC14.Lobby;

public sealed class RMCLobbyUIController : UIController
{
  private JoinXenoWindow? _joinXenoWindow;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BurrowedLarvaChangedEvent>(new EntityEventRefHandler<BurrowedLarvaChangedEvent>((object) this, __methodptr(OnBurrowedLarvaChanged)), (Type[]) null, (Type[]) null);
  }

  private void OnBurrowedLarvaChanged(ref BurrowedLarvaChangedEvent ev)
  {
    JoinXenoWindow joinXenoWindow = this._joinXenoWindow;
    if (joinXenoWindow == null || !((BaseWindow) joinXenoWindow).IsOpen)
      return;
    this.RefreshWindow(ev.Larva);
  }

  public void OpenJoinXenoWindow()
  {
    this.RefreshWindow(this.EntityManager.System<JoinXenoSystem>().ClientBurrowedLarva);
  }

  private void RefreshWindow(int larva)
  {
    JoinXenoSystem system = this.EntityManager.System<JoinXenoSystem>();
    if (this._joinXenoWindow == null || ((Control) this._joinXenoWindow).Disposed)
    {
      this._joinXenoWindow = new JoinXenoWindow();
      ((BaseWindow) this._joinXenoWindow).OnClose += (Action) (() => this._joinXenoWindow = (JoinXenoWindow) null);
      ((BaseButton) this._joinXenoWindow.LarvaButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        system.ClientJoinLarva();
        ((BaseWindow) this._joinXenoWindow).Close();
      });
      ((BaseWindow) this._joinXenoWindow).OpenCentered();
    }
    if (larva == 0)
    {
      this._joinXenoWindow.Label.Text = Loc.GetString("rmc-lobby-no-burrowed-larva");
      ((Control) this._joinXenoWindow.Buttons).Visible = false;
    }
    else
    {
      this._joinXenoWindow.Label.Text = Loc.GetString("rmc-lobby-burrowed-larva-available");
      ((Control) this._joinXenoWindow.Buttons).Visible = true;
    }
    system.RequestBurrowedLarvaStatus();
  }
}
