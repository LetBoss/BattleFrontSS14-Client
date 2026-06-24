using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Robust.Shared.GameStates;

public abstract class SharedPvsOverrideSystem : EntitySystem
{
	public virtual void AddGlobalOverride(EntityUid uid)
	{
	}

	public virtual void RemoveGlobalOverride(EntityUid uid)
	{
	}

	public virtual void AddSessionOverride(EntityUid uid, ICommonSession session)
	{
	}

	public virtual void RemoveSessionOverride(EntityUid uid, ICommonSession session)
	{
	}

	public virtual void AddSessionOverrides(EntityUid uid, Filter filter)
	{
	}
}
