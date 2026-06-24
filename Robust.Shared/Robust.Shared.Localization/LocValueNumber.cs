using System.Runtime.CompilerServices;

namespace Robust.Shared.Localization;

public sealed record LocValueNumber : LocValue<double>
{
	public LocValueNumber(double Value)
		: base(Value)
	{
	}

	public override string Format(LocContext ctx)
	{
		return base.Value.ToString(ctx.Culture);
	}

	[CompilerGenerated]
	public void Deconstruct(out double Value)
	{
		Value = base.Value;
	}
}
