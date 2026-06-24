using System.Collections.Generic;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Prototypes;

namespace Content.Client.Chemistry.EntitySystems;

public sealed class ReagentEntitySourceData : ReagentSourceData
{
	public readonly EntityPrototype SourceEntProto;

	public readonly Solution Solution;

	public override int OutputCount => Solution.Contents.Count;

	public override string IdentifierString => SourceEntProto.Name;

	public ReagentEntitySourceData(List<ProtoId<MixingCategoryPrototype>> mixingType, EntityPrototype sourceEntProto, Solution solution)
		: base(mixingType)
	{
		SourceEntProto = sourceEntProto;
		Solution = solution;
	}
}
