using System;
using Content.Shared.Access;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Marines.Access;

[Serializable]
[NetSerializable]
public sealed class IdModificationConsoleJobChangeBuiMsg(ProtoId<AccessGroupPrototype> accessGroup) : BoundUserInterfaceMessage
{
	public readonly ProtoId<AccessGroupPrototype> AccessGroup = accessGroup;
}
