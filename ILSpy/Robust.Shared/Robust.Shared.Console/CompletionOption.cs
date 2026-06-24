using System;

namespace Robust.Shared.Console;

public record struct CompletionOption(string Value, string? Hint = null, CompletionOptionFlags Flags = (CompletionOptionFlags)0) : IComparable<CompletionOption>
{
	public int CompareTo(CompletionOption other)
	{
		return string.Compare(Value, other.Value, StringComparison.CurrentCultureIgnoreCase);
	}
}
