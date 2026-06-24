using System;
using Robust.Shared.Prototypes;

namespace Robust.Shared.Serialization.Manager.Attributes;

[Obsolete("Use a static readonly ProtoId<T> instead")]
[AttributeUsage(AttributeTargets.Field)]
public sealed class ValidatePrototypeIdAttribute<T> : Attribute where T : IPrototype
{
}
