using System.Collections.Generic;
using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Prototypes;

namespace Content.Client.Chemistry.EntitySystems;

public abstract class ReagentSourceData
{
	public readonly IReadOnlyList<ProtoId<MixingCategoryPrototype>> MixingType;

	public abstract int OutputCount { get; }

	public abstract string IdentifierString { get; }

	protected ReagentSourceData(List<ProtoId<MixingCategoryPrototype>> mixingType)
	{
		MixingType = mixingType;
	}
}
