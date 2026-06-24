using System;

namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class AutoNetworkedFieldAttribute : Attribute
{
}
