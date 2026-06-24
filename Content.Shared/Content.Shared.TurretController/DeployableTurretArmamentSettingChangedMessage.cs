using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.TurretController;

[Serializable]
[NetSerializable]
public sealed class DeployableTurretArmamentSettingChangedMessage : BoundUserInterfaceMessage
{
	public int ArmamentState;

	public DeployableTurretArmamentSettingChangedMessage(int armamentState)
	{
		ArmamentState = armamentState;
	}
}
