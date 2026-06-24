using System;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Popups;

[Serializable]
[NetSerializable]
public sealed class PopupCoordinatesEvent : PopupEvent
{
	public NetCoordinates Coordinates { get; }

	public PopupCoordinatesEvent(string message, PopupType type, NetCoordinates coordinates)
		: base(message, type)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Coordinates = coordinates;
	}
}
