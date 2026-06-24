using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ReagentDispenserClearContainerSolutionMessage : BoundUserInterfaceMessage
{
}
