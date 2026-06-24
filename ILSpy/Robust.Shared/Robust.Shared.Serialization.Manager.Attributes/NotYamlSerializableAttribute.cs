using System;

namespace Robust.Shared.Serialization.Manager.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
internal sealed class NotYamlSerializableAttribute : Attribute
{
}
