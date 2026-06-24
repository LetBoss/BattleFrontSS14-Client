using Robust.Shared.GameObjects;

namespace Content.Shared.DragDrop;

[ByRefEvent]
public record struct CanDragEvent
{
	public bool Handled;
}
