using System;

namespace Robust.Shared.Serialization.Manager.Attributes;

public abstract class DataFieldBaseAttribute : Attribute
{
	public readonly int Priority;

	public readonly Type? CustomTypeSerializer;

	public readonly bool ReadOnly;

	public readonly bool ServerOnly;

	protected DataFieldBaseAttribute(bool readOnly = false, int priority = 1, bool serverOnly = false, Type? customTypeSerializer = null)
	{
		ReadOnly = readOnly;
		Priority = priority;
		ServerOnly = serverOnly;
		CustomTypeSerializer = customTypeSerializer;
	}
}
