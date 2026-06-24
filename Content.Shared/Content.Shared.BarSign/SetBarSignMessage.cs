using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.BarSign;

[Serializable]
[NetSerializable]
public sealed class SetBarSignMessage(ProtoId<BarSignPrototype> sign) : BoundUserInterfaceMessage
{
	public ProtoId<BarSignPrototype> Sign = sign;
}
