using System;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Temperature;
using Robust.Shared.GameObjects;

namespace Content.Shared.IgnitionSource;

public abstract class SharedIgnitionSourceSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<IgnitionSourceComponent, IsHotEvent>((EntityEventRefHandler<IgnitionSourceComponent, IsHotEvent>)OnIsHot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemToggleHotComponent, ItemToggledEvent>((EntityEventRefHandler<ItemToggleHotComponent, ItemToggledEvent>)OnItemToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IgnitionSourceComponent, IgnitionEvent>((EntityEventRefHandler<IgnitionSourceComponent, IgnitionEvent>)OnIgnitionEvent, (Type[])null, (Type[])null);
	}

	private void OnIsHot(Entity<IgnitionSourceComponent> ent, ref IsHotEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		args.IsHot |= ent.Comp.Ignited;
	}

	private void OnItemToggle(Entity<ItemToggleHotComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SetIgnited(Entity<IgnitionSourceComponent>.op_Implicit(ent.Owner), args.Activated);
	}

	private void OnIgnitionEvent(Entity<IgnitionSourceComponent> ent, ref IgnitionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		SetIgnited(Entity<IgnitionSourceComponent>.op_Implicit((ent.Owner, ent.Comp)), args.Ignite);
	}

	public void SetIgnited(Entity<IgnitionSourceComponent?> ent, bool ignited = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<IgnitionSourceComponent>(Entity<IgnitionSourceComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			ent.Comp.Ignited = ignited;
			((EntitySystem)this).Dirty(Entity<IgnitionSourceComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}
}
