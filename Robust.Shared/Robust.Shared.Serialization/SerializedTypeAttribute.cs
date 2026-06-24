using System;

namespace Robust.Shared.Serialization;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SerializedTypeAttribute : Attribute
{
	public string SerializeName { get; }

	public SerializedTypeAttribute(string serializeName)
	{
		SerializeName = serializeName;
	}
}
