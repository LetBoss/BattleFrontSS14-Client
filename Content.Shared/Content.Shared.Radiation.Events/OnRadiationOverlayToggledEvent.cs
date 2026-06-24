using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Events;

[Serializable]
[NetSerializable]
public sealed class OnRadiationOverlayToggledEvent : EntityEventArgs
{
	public readonly bool IsEnabled;

	public OnRadiationOverlayToggledEvent(bool isEnabled)
	{
		IsEnabled = isEnabled;
	}
}
