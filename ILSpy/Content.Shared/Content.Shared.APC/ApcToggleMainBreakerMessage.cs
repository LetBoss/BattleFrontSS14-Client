using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.APC;

[Serializable]
[NetSerializable]
public sealed class ApcToggleMainBreakerMessage : BoundUserInterfaceMessage
{
}
