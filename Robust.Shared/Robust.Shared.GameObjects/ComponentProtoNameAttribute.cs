using System;

namespace Robust.Shared.GameObjects;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ComponentProtoNameAttribute(string prototypeName) : Attribute
{
	public string PrototypeName { get; } = prototypeName;
}
