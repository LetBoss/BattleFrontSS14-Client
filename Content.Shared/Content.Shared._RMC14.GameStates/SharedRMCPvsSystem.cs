using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.GameStates;

public abstract class SharedRMCPvsSystem : EntitySystem
{
	[Dependency]
	private ISharedPlayerManager _player;

	public virtual void AddGlobalOverride(EntityUid ent)
	{
	}

	public virtual void RemoveGlobalOverride(EntityUid ent)
	{
	}

	public virtual void AddForceSend(EntityUid ent)
	{
	}

	public virtual void AddSessionOverride(EntityUid ent, ICommonSession session)
	{
	}

	public virtual void AddSessionOverride(EntityUid ent, NetUserId sessionId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession session = default(ICommonSession);
		if (_player.TryGetSessionById((NetUserId?)sessionId, ref session))
		{
			AddSessionOverride(ent, session);
		}
	}
}
