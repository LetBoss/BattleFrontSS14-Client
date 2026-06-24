using System;
using System.Runtime.CompilerServices;

namespace Robust.Shared.Localization;

public sealed record LocValueDateTime : LocValue<DateTime>
{
	public LocValueDateTime(DateTime Value)
		: base(Value)
	{
	}

	public override string Format(LocContext ctx)
	{
		return base.Value.ToString(ctx.Culture);
	}

	[CompilerGenerated]
	public void Deconstruct(out DateTime Value)
	{
		Value = base.Value;
	}
}
