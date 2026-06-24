using System.Globalization;
using Linguini.Bundle;

namespace Robust.Shared.Localization;

public readonly struct LocContext
{
	internal readonly FluentBundle Bundle;

	public CultureInfo Culture => Bundle.Culture;

	internal LocContext(FluentBundle bundle)
	{
		Bundle = bundle;
	}
}
