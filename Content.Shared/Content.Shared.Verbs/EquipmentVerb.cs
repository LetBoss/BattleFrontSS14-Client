using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs;

[Serializable]
[NetSerializable]
public sealed class EquipmentVerb : Verb
{
	public override int TypePriority => 5;
}
