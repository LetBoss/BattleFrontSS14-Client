using Robust.Shared.GameObjects;

namespace Robust.Shared.GameStates;

[ByRefEvent]
[ComponentEvent]
public readonly struct ComponentHandleState
{
	public IComponentState? Current { get; }

	public IComponentState? Next { get; }

	public ComponentHandleState(IComponentState? current, IComponentState? next)
	{
		Current = current;
		Next = next;
	}
}
