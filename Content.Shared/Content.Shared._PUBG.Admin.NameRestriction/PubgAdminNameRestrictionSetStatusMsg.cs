using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Admin.NameRestriction;

[Serializable]
[NetSerializable]
public sealed class PubgAdminNameRestrictionSetStatusMsg(bool isRestricted) : EuiMessageBase
{
	public readonly bool IsRestricted = isRestricted;
}
