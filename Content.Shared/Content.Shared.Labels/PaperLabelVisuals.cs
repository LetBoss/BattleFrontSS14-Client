using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Labels;

[Serializable]
[NetSerializable]
public enum PaperLabelVisuals : byte
{
	Layer,
	HasLabel,
	LabelType
}
