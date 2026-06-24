using System;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
public sealed class NanoTaskUiMessageEvent : CartridgeMessageEvent
{
	public readonly INanoTaskUiMessagePayload Payload;

	public NanoTaskUiMessageEvent(INanoTaskUiMessagePayload payload)
	{
		Payload = payload;
	}
}
