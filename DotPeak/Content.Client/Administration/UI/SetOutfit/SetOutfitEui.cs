// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.SetOutfit.SetOutfitEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Administration.UI.SetOutfit;

public sealed class SetOutfitEui : BaseEui
{
  private readonly SetOutfitMenu _window;
  private IEntityManager _entManager;

  public SetOutfitEui()
  {
    this._entManager = IoCManager.Resolve<IEntityManager>();
    this._window = new SetOutfitMenu();
    ((BaseWindow) this._window).OnClose += new Action(this.OnClosed);
  }

  private void OnClosed() => this.SendMessage((EuiMessageBase) new CloseEuiMessage());

  public override void Opened() => ((BaseWindow) this._window).OpenCentered();

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this._window).Close();
  }

  public override void HandleState(EuiStateBase state)
  {
    this._window.TargetEntityId = new NetEntity?(((SetOutfitEuiState) state).TargetNetEntity);
  }
}
