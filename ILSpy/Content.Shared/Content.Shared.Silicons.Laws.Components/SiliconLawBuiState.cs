using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.Laws.Components;

[Serializable]
[NetSerializable]
public sealed class SiliconLawBuiState : BoundUserInterfaceState
{
	public List<SiliconLaw> Laws;

	public HashSet<string>? RadioChannels;

	public SiliconLawBuiState(List<SiliconLaw> laws, HashSet<string>? radioChannels)
	{
		Laws = laws;
		RadioChannels = radioChannels;
	}
}
