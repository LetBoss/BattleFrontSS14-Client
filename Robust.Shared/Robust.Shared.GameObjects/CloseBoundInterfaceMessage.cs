using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public sealed class CloseBoundInterfaceMessage : BoundUserInterfaceMessage
{
}
