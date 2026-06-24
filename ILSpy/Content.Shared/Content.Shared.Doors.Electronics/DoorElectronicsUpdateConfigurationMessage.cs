using System;
using System.Collections.Generic;
using Content.Shared.Access;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Electronics;

[Serializable]
[NetSerializable]
public sealed class DoorElectronicsUpdateConfigurationMessage : BoundUserInterfaceMessage
{
	public List<ProtoId<AccessLevelPrototype>> AccessList;

	public DoorElectronicsUpdateConfigurationMessage(List<ProtoId<AccessLevelPrototype>> accessList)
	{
		AccessList = accessList;
	}
}
