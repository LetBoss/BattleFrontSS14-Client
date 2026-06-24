using System;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Prototypes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[MeansDataDefinition]
[MeansDataRecord]
public sealed class PrototypeRecordAttribute : PrototypeAttribute
{
	public PrototypeRecordAttribute(string type, int loadPriority = 1)
		: base(type, loadPriority)
	{
	}
}
