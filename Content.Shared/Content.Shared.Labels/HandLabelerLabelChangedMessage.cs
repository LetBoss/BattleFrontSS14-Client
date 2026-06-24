using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Labels;

[Serializable]
[NetSerializable]
public sealed class HandLabelerLabelChangedMessage(string label) : BoundUserInterfaceMessage
{
	public string Label { get; } = label;
}
