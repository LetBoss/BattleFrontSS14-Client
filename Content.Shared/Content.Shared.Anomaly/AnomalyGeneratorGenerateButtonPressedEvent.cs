using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Anomaly;

[Serializable]
[NetSerializable]
public sealed class AnomalyGeneratorGenerateButtonPressedEvent : BoundUserInterfaceMessage
{
}
