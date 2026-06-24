using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class BanPanelEuiState : EuiStateBase
{
	public string PlayerName { get; set; }

	public bool HasBan { get; set; }

	public BanPanelEuiState(string playerName, bool hasBan)
	{
		PlayerName = playerName;
		HasBan = hasBan;
	}
}
