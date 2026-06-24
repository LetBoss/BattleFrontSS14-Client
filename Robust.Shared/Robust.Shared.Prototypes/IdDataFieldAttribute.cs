using System;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Prototypes;

public sealed class IdDataFieldAttribute : DataFieldAttribute
{
	public const string Name = "id";

	public IdDataFieldAttribute(int priority = 1, Type? customTypeSerializer = null)
		: base("id", readOnly: false, priority, required: true, serverOnly: false, customTypeSerializer)
	{
	}
}
