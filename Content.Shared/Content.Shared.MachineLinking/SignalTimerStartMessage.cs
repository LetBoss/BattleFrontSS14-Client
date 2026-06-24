using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking;

[Serializable]
[NetSerializable]
public sealed class SignalTimerStartMessage : BoundUserInterfaceMessage
{
}
