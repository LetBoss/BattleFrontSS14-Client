using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.Borgs.Components;

[Serializable]
[NetSerializable]
public sealed class BorgSelectTypeMessage(ProtoId<BorgTypePrototype> prototype) : BoundUserInterfaceMessage
{
	public ProtoId<BorgTypePrototype> Prototype = prototype;
}
