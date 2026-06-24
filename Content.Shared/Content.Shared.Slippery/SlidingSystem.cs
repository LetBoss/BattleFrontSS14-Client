using System;
using Content.Shared.Standing;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Events;

namespace Content.Shared.Slippery;

public sealed class SlidingSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SlidingComponent, StoodEvent>((ComponentEventRefHandler<SlidingComponent, StoodEvent>)OnStand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlidingComponent, StartCollideEvent>((ComponentEventRefHandler<SlidingComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SlidingComponent, EndCollideEvent>((ComponentEventRefHandler<SlidingComponent, EndCollideEvent>)OnEndCollide, (Type[])null, (Type[])null);
	}

	private void OnStand(EntityUid uid, SlidingComponent component, ref StoodEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemComp<SlidingComponent>(uid);
	}

	private void OnStartCollide(EntityUid uid, SlidingComponent component, ref StartCollideEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		SlipperyComponent slippery = default(SlipperyComponent);
		if (((EntitySystem)this).TryComp<SlipperyComponent>(args.OtherEntity, ref slippery) && slippery.SlipData.SuperSlippery)
		{
			component.CollidingEntities.Add(args.OtherEntity);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	private void OnEndCollide(EntityUid uid, SlidingComponent component, ref EndCollideEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (component.CollidingEntities.Remove(args.OtherEntity))
		{
			if (component.CollidingEntities.Count == 0)
			{
				((EntitySystem)this).RemComp<SlidingComponent>(uid);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}
}
