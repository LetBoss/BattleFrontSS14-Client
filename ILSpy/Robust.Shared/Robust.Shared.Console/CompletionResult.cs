using System;
using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Console;

public sealed record CompletionResult(CompletionOption[] Options, string? Hint)
{
	public string? Hint { get; set; } = Hint;

	public static readonly CompletionResult Empty = new CompletionResult(Array.Empty<CompletionOption>(), null);

	public static CompletionResult FromHintOptions(IEnumerable<string> options, string? hint)
	{
		return new CompletionResult(ConvertOptions(options), hint);
	}

	public static CompletionResult FromHintOptions(IEnumerable<CompletionOption> options, string? hint)
	{
		return new CompletionResult(options.ToArray(), hint);
	}

	public static CompletionResult FromOptions(IEnumerable<string> options)
	{
		return new CompletionResult(ConvertOptions(options), null);
	}

	public static CompletionResult FromOptions(IEnumerable<CompletionOption> options)
	{
		return new CompletionResult(options.ToArray(), null);
	}

	public static CompletionResult FromHint(string hint)
	{
		return new CompletionResult(Array.Empty<CompletionOption>(), hint);
	}

	private static CompletionOption[] ConvertOptions(IEnumerable<string> stringOpts)
	{
		return stringOpts.Select((string c) => new CompletionOption(c)).ToArray();
	}
}
