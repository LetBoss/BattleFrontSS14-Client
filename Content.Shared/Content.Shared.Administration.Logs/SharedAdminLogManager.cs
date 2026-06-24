using Content.Shared.Database;
using Robust.Shared.Analyzers;

namespace Content.Shared.Administration.Logs;

[Virtual]
public class SharedAdminLogManager : ISharedAdminLogManager
{
	public virtual void Add(LogType type, LogImpact impact, ref LogStringHandler handler)
	{
	}

	public virtual void Add(LogType type, ref LogStringHandler handler)
	{
	}
}
