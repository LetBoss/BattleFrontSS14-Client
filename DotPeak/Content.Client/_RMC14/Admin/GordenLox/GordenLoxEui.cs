// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Admin.GordenLox.GordenLoxEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Admin.GordenLox;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._RMC14.Admin.GordenLox;

public sealed class GordenLoxEui : BaseEui
{
  private GordenLoxWindow? _window;
  private GordenLoxState? _state;

  public override void Opened()
  {
    base.Opened();
    this._window = new GordenLoxWindow();
    this.Refresh();
    ((BaseWindow) this._window).OpenCentered();
  }

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this._window)?.Close();
  }

  public override void HandleState(EuiStateBase state)
  {
    base.HandleState(state);
    if (!(state is GordenLoxState gordenLoxState))
      return;
    this._state = gordenLoxState;
    this.Refresh();
  }

  private void Refresh()
  {
    if (this._window == null || this._state == null)
      return;
    this._window.CountLabel.Text = Loc.GetString("pubg-gordenlox-admin-total", new (string, object)[1]
    {
      ("count", (object) this._state.Alerts.Count)
    });
    ((Control) this._window.HistoryContainer).DisposeAllChildren();
    if (this._state.Alerts.Count == 0)
    {
      ((Control) this._window.HistoryContainer).AddChild((Control) new Label()
      {
        Text = Loc.GetString("pubg-gordenlox-admin-empty"),
        FontColorOverride = new Color?(Color.Gray)
      });
    }
    else
    {
      for (int index = this._state.Alerts.Count - 1; index >= 0; --index)
      {
        GordenLoxAlertEntry alert = this._state.Alerts[index];
        BoxContainer boxContainer1 = new BoxContainer()
        {
          Orientation = (BoxContainer.LayoutOrientation) 0,
          SeparationOverride = new int?(8)
        };
        BoxContainer boxContainer2 = boxContainer1;
        Label label1 = new Label();
        DateTime dateTime = alert.CreatedAt;
        dateTime = dateTime.ToLocalTime();
        label1.Text = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        ((Control) label1).MinWidth = 170f;
        ((Control) boxContainer2).AddChild((Control) label1);
        BoxContainer boxContainer3 = boxContainer1;
        Label label2 = new Label();
        label2.Text = alert.Message;
        ((Control) label2).HorizontalExpand = true;
        ((Control) boxContainer3).AddChild((Control) label2);
        ConfirmButton confirmButton1 = new ConfirmButton();
        confirmButton1.Text = Loc.GetString("pubg-gordenlox-admin-delete");
        confirmButton1.ConfirmationText = Loc.GetString("pubg-gordenlox-admin-delete-confirm");
        ((Control) confirmButton1).MinWidth = 120f;
        ((Control) confirmButton1).StyleClasses.Add("OpenBoth");
        ConfirmButton confirmButton2 = confirmButton1;
        confirmButton2.OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new GordenLoxDeleteAlertMsg(alert.Id)));
        ((Control) boxContainer1).AddChild((Control) confirmButton2);
        ((Control) this._window.HistoryContainer).AddChild((Control) boxContainer1);
      }
    }
  }
}
