using System;
using System.Numerics;
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

namespace Content.Client.Administration.UI.BanList;

public sealed class BanListEui : BaseEui
{
	[Dependency]
	private IUserInterfaceManager _ui;

	private BanListIdsPopup? _popup;

	private BanListWindow BanWindow { get; }

	private BanListControl BanControl { get; }

	private RoleBanListControl RoleBanControl { get; }

	public BanListEui()
	{
		BanWindow = new BanListWindow();
		((BaseWindow)BanWindow).OnClose += OnClosed;
		BanControl = BanWindow.BanList;
		BanControl.LineIdsClicked += OnLineIdsClicked;
		RoleBanControl = BanWindow.RoleBanList;
		RoleBanControl.LineIdsClicked += OnLineIdsClicked;
	}

	private void OnClosed()
	{
		if (_popup != null)
		{
			((Popup)_popup).Close();
			((Control)_popup).Orphan();
			_popup = null;
		}
		SendMessage(new CloseEuiMessage());
	}

	public override void Closed()
	{
		base.Closed();
		((BaseWindow)BanWindow).Close();
	}

	public override void HandleState(EuiStateBase state)
	{
		if (state is BanListEuiState banListEuiState)
		{
			BanWindow.SetTitlePlayer(banListEuiState.BanListPlayerName);
			banListEuiState.Bans.Sort((SharedServerBan a, SharedServerBan b) => a.BanTime.CompareTo(b.BanTime));
			BanControl.SetBans(banListEuiState.Bans);
			RoleBanControl.SetRoleBans(banListEuiState.RoleBans);
		}
	}

	public override void Opened()
	{
		((BaseWindow)BanWindow).OpenCentered();
	}

	private static string FormatDate(DateTimeOffset date)
	{
		return date.ToString("MM/dd/yyyy h:mm tt");
	}

	public static void SetData<T>(IBanListLine<T> line, SharedServerBan ban) where T : SharedServerBan
	{
		line.Reason.Text = ban.Reason;
		line.BanTime.Text = FormatDate(ban.BanTime);
		line.Expires.Text = ((!ban.ExpirationTime.HasValue) ? Loc.GetString("ban-list-permanent") : FormatDate(ban.ExpirationTime.Value));
		SharedServerUnban unban = ban.Unban;
		if ((object)unban != null)
		{
			string text = Loc.GetString("ban-list-unbanned", new(string, object)[1] { ("date", FormatDate(unban.UnbanTime)) });
			string text2 = ((unban.UnbanningAdmin == null) ? string.Empty : ("\n" + Loc.GetString("ban-list-unbanned-by", new(string, object)[1] { ("unbanner", unban.UnbanningAdmin) })));
			Label expires = line.Expires;
			expires.Text = expires.Text + "\n" + text + text2;
		}
		line.BanningAdmin.Text = ban.BanningAdminName;
	}

	private void OnLineIdsClicked<T>(IBanListLine<T> line) where T : SharedServerBan
	{
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		BanListIdsPopup? popup = _popup;
		if (popup != null)
		{
			((Popup)popup).Close();
		}
		_popup = null;
		T ban = line.Ban;
		string id = ((!ban.Id.HasValue) ? string.Empty : Loc.GetString("ban-list-id", new(string, object)[1] { ("id", ban.Id.Value) }));
		string ip = ((!ban.Address.HasValue) ? string.Empty : Loc.GetString("ban-list-ip", new(string, object)[1] { ("ip", ban.Address.Value.address) }));
		string hwid = ((ban.HWId == null) ? string.Empty : Loc.GetString("ban-list-hwid", new(string, object)[1] { ("hwid", ban.HWId) }));
		string guid = ((!ban.UserId.HasValue) ? string.Empty : Loc.GetString("ban-list-guid", new(string, object)[1] { ("guid", ((object)ban.UserId.Value/*cast due to constrained. prefix*/).ToString()) }));
		_popup = new BanListIdsPopup(id, ip, hwid, guid);
		UIBox2 value = UIBox2.FromDimensions(_ui.MousePositionScaled.Position, new Vector2(1f, 1f));
		((Popup)_popup).Open((UIBox2?)value, (Vector2?)null, (Vector2?)null);
	}
}
