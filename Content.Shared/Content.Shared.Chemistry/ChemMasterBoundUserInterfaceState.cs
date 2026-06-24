using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry;

[Serializable]
[NetSerializable]
public sealed class ChemMasterBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly ContainerInfo? InputContainerInfo;

	public readonly ContainerInfo? OutputContainerInfo;

	public readonly IReadOnlyList<ReagentQuantity> BufferReagents;

	public readonly ChemMasterMode Mode;

	public readonly ChemMasterSortingType SortingType;

	public readonly FixedPoint2? BufferCurrentVolume;

	public readonly uint SelectedPillType;

	public readonly uint PillDosageLimit;

	public readonly bool UpdateLabel;

	public ChemMasterBoundUserInterfaceState(ChemMasterMode mode, ChemMasterSortingType sortingType, ContainerInfo? inputContainerInfo, ContainerInfo? outputContainerInfo, IReadOnlyList<ReagentQuantity> bufferReagents, FixedPoint2 bufferCurrentVolume, uint selectedPillType, uint pillDosageLimit, bool updateLabel)
	{
		InputContainerInfo = inputContainerInfo;
		OutputContainerInfo = outputContainerInfo;
		BufferReagents = bufferReagents;
		Mode = mode;
		SortingType = sortingType;
		BufferCurrentVolume = bufferCurrentVolume;
		SelectedPillType = selectedPillType;
		PillDosageLimit = pillDosageLimit;
		UpdateLabel = updateLabel;
	}
}
