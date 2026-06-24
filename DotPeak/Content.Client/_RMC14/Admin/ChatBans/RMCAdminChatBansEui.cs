// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Admin.ChatBans.RMCAdminChatBansEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared._RMC14.Admin.ChatBans;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._RMC14.Admin.ChatBans;

public sealed class RMCAdminChatBansEui : BaseEui
{
  private RMCAdminChatBanWindow? _window;
  private string? _prefilledTarget;

  public override void Opened()
  {
    base.Opened();
    this._window = new RMCAdminChatBanWindow();
    if (!string.IsNullOrWhiteSpace(this._prefilledTarget))
      this._window.PlayerEdit.Text = this._prefilledTarget;
    this._window.ReasonEdit.Placeholder = (Rope.Node) new Rope.Leaf(Loc.GetString("rmc-chat-bans-reason-placeholder"));
    ((BaseButton) this._window.SubmitButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      ChatType type = ChatType.None;
      if (((BaseButton) this._window.DeadButton).Pressed)
        type |= ChatType.Dead;
      if (((BaseButton) this._window.LoocButton).Pressed)
        type |= ChatType.Looc;
      if (((BaseButton) this._window.OocButton).Pressed)
        type |= ChatType.Ooc;
      if (((BaseButton) this._window.LocalButton).Pressed)
        type |= ChatType.Local;
      if (((BaseButton) this._window.WhisperButton).Pressed)
        type |= ChatType.Whisper;
      if (((BaseButton) this._window.RadioButton).Pressed)
        type |= ChatType.Radio;
      if (((BaseButton) this._window.EmoteButton).Pressed)
        type |= ChatType.Emote;
      if (((BaseButton) this._window.PartyButton).Pressed)
        type |= ChatType.Party;
      if (((BaseButton) this._window.MiniGameButton).Pressed)
        type |= ChatType.MiniGame;
      if (((BaseButton) this._window.LobbyButton).Pressed)
        type |= ChatType.Lobby;
      if (((BaseButton) this._window.AhelpButton).Pressed)
        type |= ChatType.Ahelp;
      double result;
      if (!double.TryParse(this._window.TimeEdit.Text, out result))
        result = 0.0;
      double num = result * (double) this._window.Multiplier;
      TimeSpan timeSpan = TimeSpan.FromMinutes(num);
      string reason = Rope.Collapse(this._window.ReasonEdit.TextRope);
      this.SendMessage((EuiMessageBase) new RMCAdminChatBanAddMsg(this._window.PlayerEdit.Text, type, num == 0.0 ? new TimeSpan?() : new TimeSpan?(timeSpan), reason));
    });
    ((BaseWindow) this._window).OpenCentered();
  }

  public override void HandleState(EuiStateBase state)
  {
    base.HandleState(state);
    if (!(state is RMCAdminChatBanState adminChatBanState))
      return;
    this._prefilledTarget = adminChatBanState.Target;
    if (this._window == null || string.IsNullOrWhiteSpace(adminChatBanState.Target))
      return;
    this._window.PlayerEdit.Text = adminChatBanState.Target;
  }

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this._window)?.Close();
  }

  public override void HandleMessage(EuiMessageBase msg)
  {
    base.HandleMessage(msg);
    if (this._window == null)
      return;
    this._window.ErrorLabel.Text = !(msg is RMCAdminChatBanAddErrorMsg chatBanAddErrorMsg) ? this._window.ErrorLabel.Text : chatBanAddErrorMsg.Message;
  }
}
