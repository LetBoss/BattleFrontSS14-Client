using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class ActivationVerb : Verb
{
	public new static string DefaultTextStyleClass = "ActivationVerb";

	public override int TypePriority => 1;

	public override bool DefaultDoContactInteraction => true;

	public ActivationVerb()
	{
		TextStyleClass = DefaultTextStyleClass;
	}
}
