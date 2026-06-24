using Robust.Shared.GameObjects;

namespace Content.Shared.Mind.Components;

public sealed class MindRemovedMessage : MindEvent
{
	public MindRemovedMessage(Entity<MindComponent> mind, Entity<MindContainerComponent> container)
		: base(mind, container)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
