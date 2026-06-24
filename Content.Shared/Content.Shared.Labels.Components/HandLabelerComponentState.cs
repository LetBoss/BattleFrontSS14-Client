using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Labels.Components;

[Serializable]
[NetSerializable]
public sealed class HandLabelerComponentState(string assignedLabel) : IComponentState
{
	public string AssignedLabel = assignedLabel;

	public int MaxLabelChars;
}
