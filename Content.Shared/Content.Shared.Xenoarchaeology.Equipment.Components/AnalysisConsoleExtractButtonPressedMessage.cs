using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Xenoarchaeology.Equipment.Components;

[Serializable]
[NetSerializable]
public sealed class AnalysisConsoleExtractButtonPressedMessage : BoundUserInterfaceMessage
{
}
