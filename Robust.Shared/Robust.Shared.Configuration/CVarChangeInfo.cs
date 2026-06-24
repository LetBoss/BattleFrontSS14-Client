using Robust.Shared.Timing;

namespace Robust.Shared.Configuration;

public readonly struct CVarChangeInfo
{
	public readonly string Name;

	public readonly GameTick TickChanged;

	public readonly object NewValue;

	public readonly object OldValue;

	internal CVarChangeInfo(string name, GameTick tickChanged, object newValue, object oldValue)
	{
		Name = name;
		TickChanged = tickChanged;
		NewValue = newValue;
		OldValue = oldValue;
	}
}
