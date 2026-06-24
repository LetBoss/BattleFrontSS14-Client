using System;
using System.Collections.Generic;
using Content.Shared.Access;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.TurretController;

[Serializable]
[NetSerializable]
public sealed class DeployableTurretExemptAccessLevelChangedMessage : BoundUserInterfaceMessage
{
	public HashSet<ProtoId<AccessLevelPrototype>> AccessLevels;

	public bool Enabled;

	public DeployableTurretExemptAccessLevelChangedMessage(HashSet<ProtoId<AccessLevelPrototype>> accessLevels, bool enabled)
	{
		AccessLevels = accessLevels;
		Enabled = enabled;
	}
}
