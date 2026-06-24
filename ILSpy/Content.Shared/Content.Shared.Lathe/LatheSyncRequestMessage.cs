using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe;

[Serializable]
[NetSerializable]
public sealed class LatheSyncRequestMessage : BoundUserInterfaceMessage
{
}
