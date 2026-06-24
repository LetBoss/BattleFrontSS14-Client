using System;
using System.Collections.Generic;
using Content.Shared.Access;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Electronics;

[Serializable]
[NetSerializable]
public sealed class DoorElectronicsConfigurationState : BoundUserInterfaceState
{
	public List<ProtoId<AccessLevelPrototype>> AccessList;

	public DoorElectronicsConfigurationState(List<ProtoId<AccessLevelPrototype>> accessList)
	{
		AccessList = accessList;
	}
}
