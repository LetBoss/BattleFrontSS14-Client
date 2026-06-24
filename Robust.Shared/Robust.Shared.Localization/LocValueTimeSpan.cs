using System;
using System.Runtime.CompilerServices;

namespace Robust.Shared.Localization;

public sealed record LocValueTimeSpan : LocValue<TimeSpan>
{
	public LocValueTimeSpan(TimeSpan Value)
		: base(Value)
	{
	}

	public override string Format(LocContext ctx)
	{
		return base.Value.ToString("g", ctx.Culture);
	}

	[CompilerGenerated]
	public void Deconstruct(out TimeSpan Value)
	{
		Value = base.Value;
	}
}
