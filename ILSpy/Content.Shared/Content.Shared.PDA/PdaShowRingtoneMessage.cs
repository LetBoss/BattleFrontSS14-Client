using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.PDA;

[Serializable]
[NetSerializable]
public sealed class PdaShowRingtoneMessage : BoundUserInterfaceMessage
{
}
