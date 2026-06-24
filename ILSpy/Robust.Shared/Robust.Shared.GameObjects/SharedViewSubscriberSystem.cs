using Robust.Shared.Player;

namespace Robust.Shared.GameObjects;

public abstract class SharedViewSubscriberSystem : EntitySystem
{
	public virtual void AddViewSubscriber(EntityUid uid, ICommonSession session)
	{
	}

	public virtual void RemoveViewSubscriber(EntityUid uid, ICommonSession session)
	{
	}
}
