using Robust.Shared.GameObjects;

namespace Content.Shared.Stacks;

public sealed class StackCountChangedEvent : EntityEventArgs
{
	public int OldCount;

	public int NewCount;

	public StackCountChangedEvent(int oldCount, int newCount)
	{
		OldCount = oldCount;
		NewCount = newCount;
	}
}
