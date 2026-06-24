using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Events;

namespace Content.Shared.Physics;

public sealed class SharedPreventCollideSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PreventCollideComponent, PreventCollideEvent>((ComponentEventRefHandler<PreventCollideComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
	}

	private void OnPreventCollide(EntityUid uid, PreventCollideComponent component, ref PreventCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (component.Uid == args.OtherEntity)
		{
			args.Cancelled = true;
		}
	}
}
