// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.BanList.BanListEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.UI.BanList.Bans;
using Content.Client.Administration.UI.BanList.RoleBans;
using Content.Client.Eui;
using Content.Shared.Administration.BanList;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Administration.UI.BanList;

public sealed class BanListEui : BaseEui
{
  [Dependency]
  private IUserInterfaceManager _ui;
  private BanListIdsPopup? _popup;

  public BanListEui()
  {
    this.BanWindow = new BanListWindow();
    ((BaseWindow) this.BanWindow).OnClose += new Action(this.OnClosed);
    this.BanControl = this.BanWindow.BanList;
    this.BanControl.LineIdsClicked += new Action<BanListLine>(this.OnLineIdsClicked<SharedServerBan>);
    this.RoleBanControl = this.BanWindow.RoleBanList;
    this.RoleBanControl.LineIdsClicked += new Action<RoleBanListLine>(this.OnLineIdsClicked<SharedServerRoleBan>);
  }

  private BanListWindow BanWindow { get; }

  private BanListControl BanControl { get; }

  private RoleBanListControl RoleBanControl { get; }

  private void OnClosed()
  {
    if (this._popup != null)
    {
      this._popup.Close();
      ((Control) this._popup).Orphan();
      this._popup = (BanListIdsPopup) null;
    }
    this.SendMessage((EuiMessageBase) new CloseEuiMessage());
  }

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this.BanWindow).Close();
  }

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is BanListEuiState banListEuiState))
      return;
    this.BanWindow.SetTitlePlayer(banListEuiState.BanListPlayerName);
    banListEuiState.Bans.Sort((Comparison<SharedServerBan>) ((a, b) => a.BanTime.CompareTo(b.BanTime)));
    this.BanControl.SetBans(banListEuiState.Bans);
    this.RoleBanControl.SetRoleBans(banListEuiState.RoleBans);
  }

  public override void Opened() => ((BaseWindow) this.BanWindow).OpenCentered();

  private static string FormatDate(DateTimeOffset date) => date.ToString("MM/dd/yyyy h:mm tt");

  public static void SetData<T>(IBanListLine<T> line, SharedServerBan ban) where T : SharedServerBan
  {
    line.Reason.Text = ban.Reason;
    line.BanTime.Text = BanListEui.FormatDate((DateTimeOffset) ban.BanTime);
    line.Expires.Text = !ban.ExpirationTime.HasValue ? Loc.GetString("ban-list-permanent") : BanListEui.FormatDate((DateTimeOffset) ban.ExpirationTime.Value);
    SharedServerUnban unban = ban.Unban;
    if ((object) unban != null)
    {
      string str1 = Loc.GetString("ban-list-unbanned", new (string, object)[1]
      {
        ("date", (object) BanListEui.FormatDate((DateTimeOffset) unban.UnbanTime))
      });
      string str2;
      if (unban.UnbanningAdmin != null)
        str2 = "\n" + Loc.GetString("ban-list-unbanned-by", new (string, object)[1]
        {
          ("unbanner", (object) unban.UnbanningAdmin)
        });
      else
        str2 = string.Empty;
      string str3 = str2;
      Label expires = line.Expires;
      expires.Text = $"{expires.Text}\n{str1}{str3}";
    }
    line.BanningAdmin.Text = ban.BanningAdminName;
  }

  private void OnLineIdsClicked<T>(IBanListLine<T> line) where T : SharedServerBan
  {
    this._popup?.Close();
    this._popup = (BanListIdsPopup) null;
    T ban = line.Ban;
    string empty1;
    if (ban.Id.HasValue)
      empty1 = Loc.GetString("ban-list-id", new (string, object)[1]
      {
        ("id", (object) ban.Id.Value)
      });
    else
      empty1 = string.Empty;
    string id = empty1;
    string empty2;
    if (ban.Address.HasValue)
      empty2 = Loc.GetString("ban-list-ip", new (string, object)[1]
      {
        ("ip", (object) ban.Address.Value.address)
      });
    else
      empty2 = string.Empty;
    string ip = empty2;
    string empty3;
    if (ban.HWId != null)
      empty3 = Loc.GetString("ban-list-hwid", new (string, object)[1]
      {
        ("hwid", (object) ban.HWId)
      });
    else
      empty3 = string.Empty;
    string hwid = empty3;
    string empty4;
    if (ban.UserId.HasValue)
      empty4 = Loc.GetString("ban-list-guid", new (string, object)[1]
      {
        ("guid", (object) ban.UserId.Value.ToString())
      });
    else
      empty4 = string.Empty;
    string guid = empty4;
    this._popup = new BanListIdsPopup(id, ip, hwid, guid);
    this._popup.Open(new UIBox2?(UIBox2.FromDimensions(this._ui.MousePositionScaled.Position, new Vector2(1f, 1f))), new Vector2?(), new Vector2?());
  }
}
