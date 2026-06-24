using System;
using Content.Shared._RMC14.GameStates;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Mind;

public sealed class RMCMindSystem : EntitySystem
{
	[Dependency]
	private SharedRMCPvsSystem _rmcPvs;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MindContainerComponent, PlayerAttachedEvent>((EntityEventRefHandler<MindContainerComponent, PlayerAttachedEvent>)OnMindContainerPlayedAttached, (Type[])null, (Type[])null);
	}

	private void OnMindContainerPlayedAttached(Entity<MindContainerComponent> ent, ref PlayerAttachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		MindComponent mind = default(MindComponent);
		if (!((EntitySystem)this).TryComp<MindComponent>(ent.Comp.Mind, ref mind))
		{
			return;
		}
		foreach (EntityUid role in mind.MindRoles)
		{
			_rmcPvs.AddSessionOverride(role, args.Player);
		}
	}
}
