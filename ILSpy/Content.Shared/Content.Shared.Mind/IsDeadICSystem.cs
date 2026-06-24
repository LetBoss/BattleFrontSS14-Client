using System;
using Content.Shared.Mind.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mind;

public sealed class IsDeadICSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<IsDeadICComponent, GetCharactedDeadIcEvent>((ComponentEventRefHandler<IsDeadICComponent, GetCharactedDeadIcEvent>)OnGetDeadIC, (Type[])null, (Type[])null);
	}

	private void OnGetDeadIC(EntityUid uid, IsDeadICComponent component, ref GetCharactedDeadIcEvent args)
	{
		args.Dead = true;
	}
}
