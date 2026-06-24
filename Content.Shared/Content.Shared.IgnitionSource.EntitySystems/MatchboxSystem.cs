using System;
using Content.Shared.IgnitionSource.Components;
using Content.Shared.Interaction;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.IgnitionSource.EntitySystems;

public sealed class MatchboxSystem : EntitySystem
{
	[Dependency]
	private MatchstickSystem _match;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MatchboxComponent, InteractUsingEvent>((EntityEventRefHandler<MatchboxComponent, InteractUsingEvent>)OnInteractUsing, new Type[1] { typeof(SharedStorageSystem) }, (Type[])null);
	}

	private void OnInteractUsing(Entity<MatchboxComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		MatchstickComponent matchstick = default(MatchstickComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<MatchstickComponent>(args.Used, ref matchstick))
		{
			((HandledEntityEventArgs)args).Handled = _match.TryIgnite(Entity<MatchstickComponent>.op_Implicit((args.Used, matchstick)), args.User);
		}
	}
}
