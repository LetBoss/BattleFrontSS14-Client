using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Events;

[Serializable]
[NetSerializable]
public sealed class CargoPalletAppraiseMessage : BoundUserInterfaceMessage
{
}
