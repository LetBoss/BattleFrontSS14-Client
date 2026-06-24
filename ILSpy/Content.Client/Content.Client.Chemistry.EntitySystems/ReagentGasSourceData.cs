using System.Collections.Generic;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Client.Chemistry.EntitySystems;

public sealed class ReagentGasSourceData : ReagentSourceData
{
	public readonly GasPrototype GasPrototype;

	public override int OutputCount => 1;

	public override string IdentifierString => Loc.GetString(GasPrototype.Name);

	public ReagentGasSourceData(List<ProtoId<MixingCategoryPrototype>> mixingType, GasPrototype gasPrototype)
		: base(mixingType)
	{
		GasPrototype = gasPrototype;
	}
}
