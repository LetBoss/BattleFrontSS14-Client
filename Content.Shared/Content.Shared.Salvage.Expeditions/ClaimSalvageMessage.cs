using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Salvage.Expeditions;

[Serializable]
[NetSerializable]
public sealed class ClaimSalvageMessage : BoundUserInterfaceMessage
{
	public ushort Index;
}
