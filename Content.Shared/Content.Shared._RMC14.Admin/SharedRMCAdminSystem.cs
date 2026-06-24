using System;
using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Admin;

public abstract class SharedRMCAdminSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminManager _admin;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GetVerbsEvent<RMCAdminVerb>>((EntityEventHandler<GetVerbsEvent<RMCAdminVerb>>)OnXenoGetVerbs, (Type[])null, (Type[])null);
	}

	private void OnXenoGetVerbs(GetVerbsEvent<RMCAdminVerb> args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		ActorComponent actor = default(ActorComponent);
		if (((EntitySystem)this).TryComp<ActorComponent>(args.User, ref actor))
		{
			ICommonSession player = actor.PlayerSession;
			CanUse(player);
		}
	}

	protected bool CanUse(ICommonSession player)
	{
		return _admin.HasAdminFlag(player, AdminFlags.Debug);
	}

	protected virtual void OpenBui(ICommonSession player, EntityUid target)
	{
	}
}
