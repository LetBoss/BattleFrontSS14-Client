using System;

namespace Robust.Shared.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public sealed class NetSerializableAttribute : Attribute
{
}
