using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Tools.Components;

[Serializable]
[NetSerializable]
public enum ToolOpenableVisualState : byte
{
	Open,
	Closed
}
