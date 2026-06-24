using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin;

[Serializable]
[NetSerializable]
public sealed class RMCAdminFactionMsg(RMCAdminFactionMsgType type, string left, string right) : EuiMessageBase
{
	public readonly RMCAdminFactionMsgType Type = type;

	public readonly string Left = left;

	public readonly string Right = right;
}
