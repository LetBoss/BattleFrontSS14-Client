using System;
using System.Collections.Generic;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MagicMirror;

[Serializable]
[NetSerializable]
public sealed class MagicMirrorUiState : BoundUserInterfaceState
{
	public NetEntity Target;

	public string Species;

	public List<Marking> Hair;

	public int HairSlotTotal;

	public List<Marking> FacialHair;

	public int FacialHairSlotTotal;

	public MagicMirrorUiState(string species, List<Marking> hair, int hairSlotTotal, List<Marking> facialHair, int facialHairSlotTotal)
	{
		Species = species;
		Hair = hair;
		HairSlotTotal = hairSlotTotal;
		FacialHair = facialHair;
		FacialHairSlotTotal = facialHairSlotTotal;
	}
}
