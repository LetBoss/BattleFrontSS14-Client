using System;

namespace Robust.Shared.Prototypes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EntityCategoryAttribute(params string[] categories) : Attribute
{
	public readonly string[] Categories = categories;
}
