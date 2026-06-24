using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components;

[Serializable]
[NetSerializable]
public sealed class MicrowaveEjectMessage : BoundUserInterfaceMessage
{
}
