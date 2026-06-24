using System;

namespace Robust.Shared.Serialization.Manager.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
[MeansDataDefinition]
[MeansDataRecord]
public sealed class DataRecordAttribute : Attribute
{
}
