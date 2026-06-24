using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ChemMasterOutputToBottleMessage : BoundUserInterfaceMessage
{
	public readonly uint Dosage;

	public readonly string Label;

	public ChemMasterOutputToBottleMessage(uint dosage, string label)
	{
		Dosage = dosage;
		Label = label;
	}
}
