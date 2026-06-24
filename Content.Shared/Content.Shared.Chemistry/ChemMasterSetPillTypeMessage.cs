using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ChemMasterSetPillTypeMessage : BoundUserInterfaceMessage
{
	public readonly uint PillType;

	public ChemMasterSetPillTypeMessage(uint pillType)
	{
		PillType = pillType;
	}
}
