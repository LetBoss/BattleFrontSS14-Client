using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Admin;

public abstract class SharedRMCBanSystem : EntitySystem
{
	public virtual bool IsJobBanned(NetUserId user, ProtoId<JobPrototype> job)
	{
		return false;
	}

	public bool IsJobBanned(Entity<ActorComponent?> user, ProtoId<JobPrototype> job)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ActorComponent>(Entity<ActorComponent>.op_Implicit(user), ref user.Comp, false))
		{
			return IsJobBanned(user.Comp.PlayerSession.UserId, job);
		}
		return false;
	}
}
