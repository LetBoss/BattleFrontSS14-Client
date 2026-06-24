using Robust.Shared.GameObjects;

namespace Content.Shared.Mind.Components;

public abstract class MindEvent : EntityEventArgs
{
	public readonly Entity<MindComponent> Mind;

	public readonly Entity<MindContainerComponent> Container;

	public MindEvent(Entity<MindComponent> mind, Entity<MindContainerComponent> container)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Mind = mind;
		Container = container;
	}
}
