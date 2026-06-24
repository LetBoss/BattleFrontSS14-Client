using System;
using Content.Shared.Eui;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin;

[Serializable]
[NetSerializable]
public sealed class RMCAdminCreateSquadMsg(EntProtoId squadId) : EuiMessageBase
{
	public readonly EntProtoId SquadId = squadId;
}
