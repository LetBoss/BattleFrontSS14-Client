using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Boombox;

[Serializable]
[NetSerializable]
public sealed class PubgBoomboxRangeMessage : BoundUserInterfaceMessage
{
	public float Range;

	public PubgBoomboxRangeMessage(float range)
	{
		Range = range;
	}
}
