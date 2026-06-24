using System.Runtime.CompilerServices;

namespace Robust.Shared.Localization;

public sealed record LocValueString : LocValue<string>
{
	public LocValueString(string Value)
		: base(Value)
	{
	}

	public override string Format(LocContext ctx)
	{
		return base.Value;
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	[CompilerGenerated]
	private LocValueString(LocValueString original)
		: base((LocValue<string>)original)
	{
	}

	[CompilerGenerated]
	public void Deconstruct(out string Value)
	{
		Value = base.Value;
	}
}
