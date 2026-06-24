// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Admin.NameRestriction.PubgAdminNameRestrictionEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared._PUBG.Admin.NameRestriction;
using Content.Shared.Eui;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client._PUBG.Admin.NameRestriction;

public sealed class PubgAdminNameRestrictionEui : BaseEui
{
  private PubgAdminNameRestrictionWindow? _window;
  private PubgAdminNameRestrictionState? _state;

  public override void Opened()
  {
    base.Opened();
    this._window = new PubgAdminNameRestrictionWindow();
    ((BaseButton) this._window.ApplyButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ApplyStatus());
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
    if (!(state is PubgAdminNameRestrictionState restrictionState))
      return;
    this._state = restrictionState;
    this.Refresh();
  }

  public override void HandleMessage(EuiMessageBase msg)
  {
    base.HandleMessage(msg);
    if (this._window == null || !(msg is PubgAdminNameRestrictionErrorMsg restrictionErrorMsg))
      return;
    this._window.ErrorLabel.Text = restrictionErrorMsg.Message;
  }

  private void ApplyStatus()
  {
    if (this._window == null || this._state == null || !this._state.CanEdit)
      return;
    this.SendMessage((EuiMessageBase) new PubgAdminNameRestrictionSetStatusMsg(this._window.SelectedRestricted));
  }

  private void Refresh()
  {
    if (this._window == null || this._state == null)
      return;
    string str1 = Loc.GetString("pubg-name-restriction-admin-never");
    string str2 = string.IsNullOrWhiteSpace(this._state.ChangedByCkey) ? str1 : this._state.ChangedByCkey;
    ref readonly DateTime? local = ref this._state.ChangedAtUtc;
    string str3;
    if (!local.HasValue)
    {
      str3 = (string) null;
    }
    else
    {
      DateTime dateTime = local.GetValueOrDefault();
      dateTime = dateTime.ToLocalTime();
      str3 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
    if (str3 == null)
      str3 = str1;
    string str4 = str3;
    this._window.ErrorLabel.Text = string.Empty;
    this._window.PlayerValue.Text = this._state.PlayerCkey;
    this._window.LastChangedByValue.Text = str2;
    this._window.LastChangedAtValue.Text = str4;
    this._window.SetSelectedRestricted(this._state.IsRestricted);
    ((BaseButton) this._window.StatusOption).Disabled = !this._state.CanEdit;
    ((BaseButton) this._window.ApplyButton).Disabled = !this._state.CanEdit;
  }
}
