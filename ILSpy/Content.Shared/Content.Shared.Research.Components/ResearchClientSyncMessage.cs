using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components;

[Serializable]
[NetSerializable]
public sealed class ResearchClientSyncMessage : BoundUserInterfaceMessage
{
}
