using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Store;

[Serializable]
[NetSerializable]
public sealed class StoreRequestRefundMessage : BoundUserInterfaceMessage
{
}
