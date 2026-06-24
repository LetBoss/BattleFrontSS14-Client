using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Admin.Reputation;

[Serializable]
[NetSerializable]
public sealed class PubgAdminReputationAdjustMsg(bool increase, int amount, string reason) : EuiMessageBase
{
	public readonly bool Increase = increase;

	public readonly int Amount = amount;

	public readonly string Reason = reason;
}
