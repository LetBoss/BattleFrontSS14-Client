using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Anomaly;

[Serializable]
[NetSerializable]
public enum AnomalyGeneratorVisuals : byte
{
	Generating
}
