using System;

namespace Content.Shared.Administration;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AnyCommandAttribute : Attribute
{
}
