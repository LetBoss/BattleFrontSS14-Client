using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ChemMasterCreatePillsMessage : BoundUserInterfaceMessage
{
	public readonly uint Dosage;

	public readonly uint Number;

	public readonly string Label;

	public ChemMasterCreatePillsMessage(uint dosage, uint number, string label)
	{
		Dosage = dosage;
		Number = number;
		Label = label;
	}
}
