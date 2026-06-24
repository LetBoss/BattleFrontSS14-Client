using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin.ChatBans;

[Serializable]
[NetSerializable]
public sealed class RMCAdminChatBanState(string target) : EuiStateBase
{
	public readonly string Target = target;
}
