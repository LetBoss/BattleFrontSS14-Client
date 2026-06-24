using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin.ChatBans;

[Serializable]
[NetSerializable]
public sealed class RMCAdminChatBanListPardonMsg(int id) : EuiMessageBase
{
	public readonly int Id = id;
}
