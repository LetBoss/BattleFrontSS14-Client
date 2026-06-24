using System;

namespace Robust.Shared.GameObjects;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
internal sealed class ComponentEventAttribute : Attribute
{
	public bool Exclusive = true;
}
