using System;

namespace Robust.Shared.Console;

[Flags]
public enum CompletionOptionFlags
{
	PartialCompletion = 1,
	NoQuote = 2,
	NoEscape = 4
}
