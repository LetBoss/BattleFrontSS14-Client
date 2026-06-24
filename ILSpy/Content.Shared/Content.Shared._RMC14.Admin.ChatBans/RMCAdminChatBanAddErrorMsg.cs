using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin.ChatBans;

[Serializable]
[NetSerializable]
public sealed class RMCAdminChatBanAddErrorMsg(string message) : EuiMessageBase
{
	public readonly string Message = message;
}
