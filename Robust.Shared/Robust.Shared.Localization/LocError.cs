using Linguini.Bundle.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Localization;

internal record LocError
{
	public readonly ResPath Path;

	public readonly FluentError Error;

	public LocError(ResPath path, FluentError fluentError)
	{
		Path = path;
		Error = fluentError;
	}

	public override string ToString()
	{
		return $"[{Path.CanonPath}]: {Error}";
	}
}
