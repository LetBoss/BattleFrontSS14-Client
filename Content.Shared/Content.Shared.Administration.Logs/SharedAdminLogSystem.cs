using Content.Shared.Database;
using Robust.Shared.GameObjects;

namespace Content.Shared.Administration.Logs;

public abstract class SharedAdminLogSystem : EntitySystem
{
	public virtual void Add(LogType type, LogImpact impact, ref LogStringHandler handler)
	{
	}

	public virtual void Add(LogType type, ref LogStringHandler handler)
	{
	}
}
