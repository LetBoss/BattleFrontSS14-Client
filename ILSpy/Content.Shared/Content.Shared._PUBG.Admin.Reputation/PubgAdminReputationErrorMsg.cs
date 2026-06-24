using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Admin.Reputation;

[Serializable]
[NetSerializable]
public sealed class PubgAdminReputationErrorMsg(string message) : EuiMessageBase
{
	public readonly string Message = message;
}
