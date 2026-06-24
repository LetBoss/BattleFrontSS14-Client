using System;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Prototypes;

public sealed class ParentDataFieldAttribute : DataFieldAttribute
{
	public const string Name = "parent";

	public ParentDataFieldAttribute(Type prototypeIdSerializer, int priority = 1)
		: base("parent", readOnly: false, priority, required: false, serverOnly: false, prototypeIdSerializer)
	{
	}
}
