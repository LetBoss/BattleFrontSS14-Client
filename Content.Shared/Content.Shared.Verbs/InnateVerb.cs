using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class InnateVerb : Verb
{
	public override int TypePriority => 3;

	public InnateVerb()
	{
		TextStyleClass = InteractionVerb.DefaultTextStyleClass;
	}
}
