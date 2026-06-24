using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen;

[Serializable]
[NetSerializable]
public sealed class ReagentGrinderToggleAutoModeMessage : BoundUserInterfaceMessage
{
}
