using System;
using Content.Shared._RMC14.Marines.Squads;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Marines.Mutiny;

public abstract class SharedMutinySystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MutineerComponent, GetMarineIconEvent>((EntityEventRefHandler<MutineerComponent, GetMarineIconEvent>)OnGetMarineIcon, (Type[])null, new Type[1] { typeof(SquadSystem) });
		((EntitySystem)this).SubscribeLocalEvent<MutineerLeaderComponent, GetMarineIconEvent>((EntityEventRefHandler<MutineerLeaderComponent, GetMarineIconEvent>)OnGetLeaderIcon, (Type[])null, new Type[1] { typeof(SquadSystem) });
		((EntitySystem)this).SubscribeLocalEvent<MutineerComponent, ComponentAdd>((EntityEventRefHandler<MutineerComponent, ComponentAdd>)MutineerAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MutineerComponent, ComponentRemove>((EntityEventRefHandler<MutineerComponent, ComponentRemove>)MutineerRemoved, (Type[])null, (Type[])null);
	}

	private void OnGetMarineIcon(Entity<MutineerComponent> mutineer, ref GetMarineIconEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		args.Icon = mutineer.Comp.Icon;
	}

	private void OnGetLeaderIcon(Entity<MutineerLeaderComponent> leader, ref GetMarineIconEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		args.Icon = leader.Comp.Icon;
	}

	protected abstract void MutineerAdded(Entity<MutineerComponent> ent, ref ComponentAdd args);

	protected abstract void MutineerRemoved(Entity<MutineerComponent> ent, ref ComponentRemove args);
}
