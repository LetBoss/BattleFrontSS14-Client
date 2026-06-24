using System.Net;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Administration.UI.BanPanel;

public sealed class BanPanelEui : BaseEui
{
	private BanPanel BanPanel { get; }

	public BanPanelEui()
	{
		BanPanel = new BanPanel();
		((BaseWindow)BanPanel).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
		BanPanel.BanSubmitted += delegate(string? player, (IPAddress, int)? ip, bool useLastIp, ImmutableTypedHwid? hwid, bool useLastHwid, uint minutes, string reason, NoteSeverity severity, string[]? roles, bool erase)
		{
			SendMessage(new BanPanelEuiStateMsg.CreateBanRequest(player, ip, useLastIp, hwid, useLastHwid, minutes, reason, severity, roles, erase));
		};
		BanPanel.PlayerChanged += delegate(string player)
		{
			SendMessage(new BanPanelEuiStateMsg.GetPlayerInfoRequest(player));
		};
	}

	public override void HandleState(EuiStateBase state)
	{
		if (state is BanPanelEuiState banPanelEuiState)
		{
			BanPanel.UpdateBanFlag(banPanelEuiState.HasBan);
			BanPanel.UpdatePlayerData(banPanelEuiState.PlayerName);
		}
	}

	public override void Opened()
	{
		((BaseWindow)BanPanel).OpenCentered();
	}

	public override void Closed()
	{
		((BaseWindow)BanPanel).Close();
		((Control)BanPanel).Orphan();
	}
}
