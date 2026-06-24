using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Boombox;

[Serializable]
[NetSerializable]
public sealed class PubgBoomboxSeekMessage : BoundUserInterfaceMessage
{
	public float Seconds;

	public PubgBoomboxSeekMessage(float seconds)
	{
		Seconds = seconds;
	}
}
