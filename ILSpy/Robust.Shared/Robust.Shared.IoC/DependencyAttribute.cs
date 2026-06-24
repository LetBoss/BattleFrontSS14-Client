using System;

namespace Robust.Shared.IoC;

[AttributeUsage(AttributeTargets.Field)]
public sealed class DependencyAttribute : Attribute
{
}
