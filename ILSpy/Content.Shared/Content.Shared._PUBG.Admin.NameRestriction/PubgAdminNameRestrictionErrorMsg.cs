using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Admin.NameRestriction;

[Serializable]
[NetSerializable]
public sealed class PubgAdminNameRestrictionErrorMsg(string message) : EuiMessageBase
{
	public readonly string Message = message;
}
