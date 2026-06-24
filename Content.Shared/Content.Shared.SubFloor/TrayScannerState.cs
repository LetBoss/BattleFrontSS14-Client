using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.SubFloor;

[Serializable]
[NetSerializable]
public sealed class TrayScannerState : ComponentState
{
	public bool Enabled;

	public float Range;

	public TrayScannerState(bool enabled, float range)
	{
		Enabled = enabled;
		Range = range;
	}
}
