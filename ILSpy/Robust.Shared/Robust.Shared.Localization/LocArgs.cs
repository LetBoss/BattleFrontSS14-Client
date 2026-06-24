using System.Collections.Generic;

namespace Robust.Shared.Localization;

public readonly struct LocArgs(IReadOnlyList<ILocValue> args, IReadOnlyDictionary<string, ILocValue> options)
{
	public IReadOnlyList<ILocValue> Args { get; } = args;

	public IReadOnlyDictionary<string, ILocValue> Options { get; } = options;
}
