using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class AlternativeVerb : Verb
{
	public new static string DefaultTextStyleClass = "AlternativeVerb";

	public override int TypePriority => 2;

	public override bool DefaultDoContactInteraction => true;

	public AlternativeVerb()
	{
		TextStyleClass = DefaultTextStyleClass;
	}
}
