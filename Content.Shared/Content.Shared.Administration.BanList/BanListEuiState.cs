using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.BanList;

[Serializable]
[NetSerializable]
public sealed class BanListEuiState : EuiStateBase
{
	public string BanListPlayerName { get; }

	public List<SharedServerBan> Bans { get; }

	public List<SharedServerRoleBan> RoleBans { get; }

	public BanListEuiState(string banListPlayerName, List<SharedServerBan> bans, List<SharedServerRoleBan> roleBans)
	{
		BanListPlayerName = banListPlayerName;
		Bans = bans;
		RoleBans = roleBans;
	}
}
