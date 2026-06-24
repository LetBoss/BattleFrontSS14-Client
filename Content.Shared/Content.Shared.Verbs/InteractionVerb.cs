using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class InteractionVerb : Verb
{
	public new static string DefaultTextStyleClass = "InteractionVerb";

	public override int TypePriority => 4;

	public override bool DefaultDoContactInteraction => true;

	public InteractionVerb()
	{
		TextStyleClass = DefaultTextStyleClass;
	}
}
