using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Construction.Components;

[Serializable]
[NetSerializable]
public sealed class FlatpackCreatorStartPackBuiMessage : BoundUserInterfaceMessage
{
}
