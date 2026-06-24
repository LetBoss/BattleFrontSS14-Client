using System;

namespace Robust.Shared.Serialization.Manager.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class AlwaysPushInheritanceAttribute : Attribute
{
}
