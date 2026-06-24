using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class UtilityVerb : Verb
{
	public override int TypePriority => 3;

	public override bool DefaultDoContactInteraction => true;

	public UtilityVerb()
	{
		TextStyleClass = InteractionVerb.DefaultTextStyleClass;
	}
}
