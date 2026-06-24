using System.Collections.Generic;
using System.Linq;
using Content.Shared.Chemistry.Reaction;
using Robust.Shared.Prototypes;

namespace Content.Client.Chemistry.EntitySystems;

public sealed class ReagentReactionSourceData : ReagentSourceData
{
	public readonly ReactionPrototype ReactionPrototype;

	public override int OutputCount => ReactionPrototype.Products.Count + ReactionPrototype.Reactants.Count<KeyValuePair<string, ReactantPrototype>>((KeyValuePair<string, ReactantPrototype> r) => r.Value.Catalyst);

	public override string IdentifierString => ReactionPrototype.ID;

	public ReagentReactionSourceData(List<ProtoId<MixingCategoryPrototype>> mixingType, ReactionPrototype reactionPrototype)
		: base(mixingType)
	{
		ReactionPrototype = reactionPrototype;
	}
}
