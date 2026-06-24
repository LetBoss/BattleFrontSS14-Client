// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Admin.Reputation.PubgAdminReputationEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared._PUBG.Admin.Reputation;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._PUBG.Admin.Reputation;

public sealed class PubgAdminReputationEui : BaseEui
{
  private PubgAdminReputationWindow? _window;
  private PubgAdminReputationAdjustWindow? _adjustWindow;
  private PubgAdminReputationState? _state;

  public override void Opened()
  {
    base.Opened();
    this._window = new PubgAdminReputationWindow();
    ((BaseButton) this._window.AdjustButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.OpenAdjustWindow());
    this.Refresh();
    ((BaseWindow) this._window).OpenCentered();
  }

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this._adjustWindow)?.Close();
    ((BaseWindow) this._window)?.Close();
  }

  public override void HandleState(EuiStateBase state)
  {
    base.HandleState(state);
    if (!(state is PubgAdminReputationState adminReputationState))
      return;
    this._state = adminReputationState;
    this.Refresh();
  }

  public override void HandleMessage(EuiMessageBase msg)
  {
    base.HandleMessage(msg);
    if (this._window == null || !(msg is PubgAdminReputationErrorMsg reputationErrorMsg))
      return;
    this._window.ErrorLabel.SetMessage(reputationErrorMsg.Message, new Color?());
  }

  private void OpenAdjustWindow()
  {
    if (this._window == null || this._state == null || !this._state.CanEdit)
      return;
    if (this._adjustWindow != null)
    {
      ((BaseWindow) this._adjustWindow).MoveToFront();
    }
    else
    {
      this._adjustWindow = new PubgAdminReputationAdjustWindow();
      ((BaseButton) this._adjustWindow.ApplyButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ApplyAdjustment());
      ((BaseWindow) this._adjustWindow).OnClose += (Action) (() => this._adjustWindow = (PubgAdminReputationAdjustWindow) null);
      ((BaseWindow) this._adjustWindow).OpenCentered();
    }
  }

  private void ApplyAdjustment()
  {
    if (this._adjustWindow == null)
      return;
    int result;
    if (!int.TryParse(this._adjustWindow.AmountEdit.Text, out result) || result <= 0)
    {
      this._adjustWindow.ErrorLabel.SetMessage(Loc.GetString("pubg-reputation-admin-error-invalid-amount"), new Color?());
    }
    else
    {
      string reason = Rope.Collapse(this._adjustWindow.ReasonEdit.TextRope).Trim();
      if (string.IsNullOrWhiteSpace(reason))
      {
        this._adjustWindow.ErrorLabel.SetMessage(Loc.GetString("pubg-reputation-admin-error-reason-required"), new Color?());
      }
      else
      {
        this.SendMessage((EuiMessageBase) new PubgAdminReputationAdjustMsg(this._adjustWindow.IsIncrease, result, reason));
        ((BaseWindow) this._adjustWindow).Close();
        this._adjustWindow = (PubgAdminReputationAdjustWindow) null;
      }
    }
  }

  private void Refresh()
  {
    if (this._window == null || this._state == null)
      return;
    this._window.ErrorLabel.Text = string.Empty;
    this._window.PlayerLabel.Text = Loc.GetString("pubg-reputation-admin-player", new (string, object)[1]
    {
      ("player", (object) this._state.PlayerName)
    });
    this._window.CurrentValueLabel.Text = Loc.GetString("pubg-reputation-admin-current", new (string, object)[1]
    {
      ("value", (object) this._state.CurrentReputation)
    });
    this._window.CurrentValueLabel.FontColorOverride = new Color?(this._state.CurrentReputation < 80 /*0x50*/ ? Color.IndianRed : Color.LightGreen);
    ((BaseButton) this._window.AdjustButton).Disabled = !this._state.CanEdit;
    ((Control) this._window.HistoryContainer).DisposeAllChildren();
    foreach (PubgReputationHistoryEntry reputationHistoryEntry in this._state.History)
    {
      BoxContainer boxContainer = new BoxContainer()
      {
        Orientation = (BoxContainer.LayoutOrientation) 0,
        SeparationOverride = new int?(4)
      };
      Label label1 = new Label();
      DateTime dateTime = reputationHistoryEntry.CreatedAt;
      dateTime = dateTime.ToLocalTime();
      label1.Text = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
      ((Control) label1).MinWidth = 160f;
      Label label2 = label1;
      Label label3 = new Label();
      label3.Text = reputationHistoryEntry.ChangedByName;
      ((Control) label3).MinWidth = 150f;
      Label label4 = label3;
      string str1;
      if (reputationHistoryEntry.Delta < 0)
        str1 = reputationHistoryEntry.Delta.ToString();
      else
        str1 = $"+{reputationHistoryEntry.Delta}";
      string str2 = str1;
      Label label5 = new Label();
      label5.Text = str2;
      ((Control) label5).MinWidth = 100f;
      label5.FontColorOverride = new Color?(reputationHistoryEntry.Delta >= 0 ? Color.LightGreen : Color.IndianRed);
      Label label6 = label5;
      Label label7 = new Label();
      label7.Text = $"{reputationHistoryEntry.OldValue}->{reputationHistoryEntry.NewValue}";
      ((Control) label7).MinWidth = 120f;
      Label label8 = label7;
      Label label9 = new Label();
      label9.Text = reputationHistoryEntry.Source;
      ((Control) label9).MinWidth = 90f;
      Label label10 = label9;
      Label label11 = new Label();
      label11.Text = reputationHistoryEntry.Reason ?? string.Empty;
      ((Control) label11).HorizontalExpand = true;
      Label label12 = label11;
      ((Control) boxContainer).AddChild((Control) label2);
      ((Control) boxContainer).AddChild((Control) label4);
      ((Control) boxContainer).AddChild((Control) label6);
      ((Control) boxContainer).AddChild((Control) label8);
      ((Control) boxContainer).AddChild((Control) label10);
      ((Control) boxContainer).AddChild((Control) label12);
      ((Control) this._window.HistoryContainer).AddChild((Control) boxContainer);
    }
  }
}
