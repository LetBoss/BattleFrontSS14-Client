using System;

namespace Robust.Shared.Serialization.Manager.Definition;

public static class DataDefinitionUtility
{
	public static string AutoGenerateTag(string name)
	{
		ReadOnlySpan<char> readOnlySpan = name.AsSpan();
		return $"{char.ToLowerInvariant(readOnlySpan[0])}{readOnlySpan.Slice(1).ToString()}";
	}
}
