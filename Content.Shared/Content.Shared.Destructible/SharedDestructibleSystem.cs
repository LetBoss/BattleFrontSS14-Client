using Robust.Shared.GameObjects;

namespace Content.Shared.Destructible;

public abstract class SharedDestructibleSystem : EntitySystem
{
	public bool DestroyEntity(EntityUid owner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		DestructionAttemptEvent ev = new DestructionAttemptEvent();
		((EntitySystem)this).RaiseLocalEvent<DestructionAttemptEvent>(owner, ev, false);
		if (((CancellableEntityEventArgs)ev).Cancelled)
		{
			return false;
		}
		DestructionEventArgs eventArgs = new DestructionEventArgs();
		((EntitySystem)this).RaiseLocalEvent<DestructionEventArgs>(owner, eventArgs, false);
		((EntitySystem)this).QueueDel((EntityUid?)owner);
		return true;
	}

	public void BreakEntity(EntityUid owner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		BreakageEventArgs eventArgs = new BreakageEventArgs();
		((EntitySystem)this).RaiseLocalEvent<BreakageEventArgs>(owner, eventArgs, false);
	}
}
