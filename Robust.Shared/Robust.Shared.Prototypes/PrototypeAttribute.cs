using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Prototypes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[MeansDataDefinition]
[Virtual]
public class PrototypeAttribute : Attribute
{
	public readonly int LoadPriority;

	public string? Type { get; internal set; }

	public PrototypeAttribute(string? type = null, int loadPriority = 1)
	{
		Type = type;
		LoadPriority = loadPriority;
	}

	public PrototypeAttribute(int loadPriority)
	{
		Type = null;
		LoadPriority = loadPriority;
	}
}
