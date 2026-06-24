using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ChemMasterSetModeMessage : BoundUserInterfaceMessage
{
	public readonly ChemMasterMode ChemMasterMode;

	public ChemMasterSetModeMessage(ChemMasterMode mode)
	{
		ChemMasterMode = mode;
	}
}
