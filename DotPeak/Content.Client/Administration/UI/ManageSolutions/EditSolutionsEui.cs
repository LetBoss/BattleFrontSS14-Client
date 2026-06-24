// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.ManageSolutions.EditSolutionsEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.UserInterface.CustomControls;
using System;

#nullable enable
namespace Content.Client.Administration.UI.ManageSolutions;

public sealed class EditSolutionsEui : BaseEui
{
  private readonly EditSolutionsWindow _window;

  public EditSolutionsEui()
  {
    this._window = new EditSolutionsWindow();
    ((BaseWindow) this._window).OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new CloseEuiMessage()));
  }

  public override void Opened()
  {
    base.Opened();
    ((BaseWindow) this._window).OpenCentered();
  }

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this._window).Close();
  }

  public override void HandleState(EuiStateBase baseState)
  {
    this._window.SetState((EditSolutionsEuiState) baseState);
  }
}
