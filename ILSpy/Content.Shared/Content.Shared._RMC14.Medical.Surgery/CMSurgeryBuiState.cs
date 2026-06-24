using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Medical.Surgery;

[Serializable]
[NetSerializable]
public sealed class CMSurgeryBuiState(Dictionary<NetEntity, List<EntProtoId>> choices) : BoundUserInterfaceState
{
	public readonly Dictionary<NetEntity, List<EntProtoId>> Choices = choices;
}
