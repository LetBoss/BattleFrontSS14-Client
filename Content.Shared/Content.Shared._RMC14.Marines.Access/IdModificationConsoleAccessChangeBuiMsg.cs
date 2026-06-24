using System;
using Content.Shared.Access;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Marines.Access;

[Serializable]
[NetSerializable]
public sealed class IdModificationConsoleAccessChangeBuiMsg(ProtoId<AccessLevelPrototype> access, bool add) : BoundUserInterfaceMessage
{
	public readonly ProtoId<AccessLevelPrototype> Access = access;

	public readonly bool Add = add;
}
