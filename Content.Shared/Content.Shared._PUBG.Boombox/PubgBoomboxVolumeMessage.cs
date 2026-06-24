using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Boombox;

[Serializable]
[NetSerializable]
public sealed class PubgBoomboxVolumeMessage : BoundUserInterfaceMessage
{
	public float Volume;

	public PubgBoomboxVolumeMessage(float volume)
	{
		Volume = volume;
	}
}
