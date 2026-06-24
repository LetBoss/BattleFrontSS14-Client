using System;

namespace Robust.Shared.Prototypes;

public static class PrototypeUtility
{
	public const string PrototypeNameEnding = "Prototype";

	public static string CalculatePrototypeName(string type)
	{
		ReadOnlySpan<char> readOnlySpan = type.AsSpan();
		if (type.EndsWith("Prototype"))
		{
			return $"{char.ToLowerInvariant(readOnlySpan[0])}{readOnlySpan.Slice(1, readOnlySpan.Length - "Prototype".Length - 1).ToString()}";
		}
		return $"{char.ToLowerInvariant(readOnlySpan[0])}{readOnlySpan.Slice(1).ToString()}";
	}
}
