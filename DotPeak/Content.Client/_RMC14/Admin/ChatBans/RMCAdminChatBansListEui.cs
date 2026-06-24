// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Admin.ChatBans.RMCAdminChatBansListEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.UserInterface;
using Content.Client.Eui;
using Content.Shared._RMC14.Admin.ChatBans;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Admin.ChatBans;

public sealed class RMCAdminChatBansListEui : BaseEui
{
  private RMCAdminChatBanListWindow? _window;
  private RMCAdminChatBanListState? _state;

  public override void Opened()
  {
    base.Opened();
    this._window = new RMCAdminChatBanListWindow();
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
    if (!(state is RMCAdminChatBanListState chatBanListState))
      return;
    this._state = chatBanListState;
    this.Refresh();
  }

  private void Refresh()
  {
    if (this._window == null || this._state == null)
      return;
    ((Control) this._window.Container).DisposeAllChildren();
    foreach (ChatBan ban1 in this._state.Bans)
    {
      ChatBan ban = ban1;
      RMCAdminChatBanRow rmcAdminChatBanRow = new RMCAdminChatBanRow();
      rmcAdminChatBanRow.TypeLabel.Text = RMCAdminChatBansListEui.FormatChatType(ban.Type);
      rmcAdminChatBanRow.ReasonLabel.Text = FormattedMessage.EscapeText(ban.Reason);
      rmcAdminChatBanRow.BannedAtLabel.Text = $"{ban.BannedAt:MM/dd/yyyy h:mm tt}";
      Label expiresAtLabel1 = rmcAdminChatBanRow.ExpiresAtLabel;
      string str;
      if (ban.ExpiresAt.HasValue)
        str = $"{ban.ExpiresAt:MM/dd/yyyy h:mm tt}";
      else
        str = "rmc-chat-bans-list-permanent";
      expiresAtLabel1.Text = str;
      if (ban.UnbannedBy != null)
      {
        Label expiresAtLabel2 = rmcAdminChatBanRow.ExpiresAtLabel;
        expiresAtLabel2.Text = $"{expiresAtLabel2.Text}\n{Loc.GetString("ban-list-unbanned", new (string, object)[1]
        {
          ("date", (object) $"{ban.UnbannedAt:MM/dd/yyyy h:mm tt}")
        })}";
        Label expiresAtLabel3 = rmcAdminChatBanRow.ExpiresAtLabel;
        expiresAtLabel3.Text = $"{expiresAtLabel3.Text}\n{Loc.GetString("ban-list-unbanned-by", new (string, object)[1]
        {
          ("unbanner", (object) ban.UnbannedBy)
        })}";
        ((Control) rmcAdminChatBanRow.PardonButton).Visible = false;
      }
      rmcAdminChatBanRow.PardonButton.OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((EuiMessageBase) new RMCAdminChatBanListPardonMsg(ban.Id)));
      ((Control) this._window.Container).AddChild((Control) rmcAdminChatBanRow);
      ((Control) this._window.Container).AddChild((Control) new BlueHorizontalSeparator());
    }
  }

  private static string FormatChatType(ChatType type)
  {
    List<string> values = new List<string>();
    if (type.HasFlag((Enum) ChatType.Dead))
      values.Add(Loc.GetString("rmc-chat-bans-dead"));
    if (type.HasFlag((Enum) ChatType.Looc))
      values.Add(Loc.GetString("rmc-chat-bans-looc"));
    if (type.HasFlag((Enum) ChatType.Ooc))
      values.Add(Loc.GetString("rmc-chat-bans-ooc"));
    if (type.HasFlag((Enum) ChatType.Local))
      values.Add(Loc.GetString("rmc-chat-bans-local"));
    if (type.HasFlag((Enum) ChatType.Whisper))
      values.Add(Loc.GetString("rmc-chat-bans-whisper"));
    if (type.HasFlag((Enum) ChatType.Radio))
      values.Add(Loc.GetString("rmc-chat-bans-radio"));
    if (type.HasFlag((Enum) ChatType.Emote))
      values.Add(Loc.GetString("rmc-chat-bans-emote"));
    if (type.HasFlag((Enum) ChatType.Party))
      values.Add(Loc.GetString("rmc-chat-bans-party"));
    if (type.HasFlag((Enum) ChatType.MiniGame))
      values.Add(Loc.GetString("rmc-chat-bans-minigame"));
    if (type.HasFlag((Enum) ChatType.Lobby))
      values.Add(Loc.GetString("rmc-chat-bans-lobby"));
    if (type.HasFlag((Enum) ChatType.Ahelp))
      values.Add(Loc.GetString("rmc-chat-bans-ahelp"));
    return values.Count <= 0 ? type.ToString() : string.Join(", ", (IEnumerable<string>) values);
  }
}
