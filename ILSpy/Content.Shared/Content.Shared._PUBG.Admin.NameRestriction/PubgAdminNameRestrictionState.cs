using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Admin.NameRestriction;

[Serializable]
[NetSerializable]
public sealed class PubgAdminNameRestrictionState(string playerCkey, bool isRestricted, bool canEdit, string? changedByCkey, DateTime? changedAtUtc) : EuiStateBase
{
	public readonly string PlayerCkey = playerCkey;

	public readonly bool IsRestricted = isRestricted;

	public readonly bool CanEdit = canEdit;

	public readonly string? ChangedByCkey = changedByCkey;

	public readonly DateTime? ChangedAtUtc = changedAtUtc;
}
