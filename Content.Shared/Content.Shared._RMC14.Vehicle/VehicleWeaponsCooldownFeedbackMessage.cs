using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleWeaponsCooldownFeedbackMessage : BoundUserInterfaceMessage
{
	public readonly float RemainingSeconds;

	public VehicleWeaponsCooldownFeedbackMessage(float remainingSeconds)
	{
		RemainingSeconds = remainingSeconds;
	}
}
