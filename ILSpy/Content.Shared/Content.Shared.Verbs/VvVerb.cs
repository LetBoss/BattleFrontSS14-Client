using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class VvVerb : Verb
{
	public override int TypePriority => int.MaxValue;
}
