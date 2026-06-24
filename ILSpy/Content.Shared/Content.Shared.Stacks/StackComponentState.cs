using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Stacks;

[Serializable]
[NetSerializable]
public sealed class StackComponentState : ComponentState
{
	public bool Lingering;

	public int Count { get; }

	public int? MaxCount { get; }

	public StackComponentState(int count, int? maxCount, bool lingering)
	{
		Count = count;
		MaxCount = maxCount;
		Lingering = lingering;
	}
}
