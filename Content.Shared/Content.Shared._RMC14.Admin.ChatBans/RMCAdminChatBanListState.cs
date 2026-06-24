using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin.ChatBans;

[Serializable]
[NetSerializable]
public sealed class RMCAdminChatBanListState(List<ChatBan> bans) : EuiStateBase
{
	public readonly List<ChatBan> Bans = bans;
}
