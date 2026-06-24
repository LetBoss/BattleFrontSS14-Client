using System;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.FixedPoint;
using Content.Shared.Standing;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Evasion;

public sealed class EvasionSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<EvasionComponent, MapInitEvent>((EntityEventRefHandler<EvasionComponent, MapInitEvent>)CallRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvasionComponent, XenoRestEvent>((EntityEventRefHandler<EvasionComponent, XenoRestEvent>)CallRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvasionComponent, DownedEvent>((EntityEventRefHandler<EvasionComponent, DownedEvent>)CallRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvasionComponent, StoodEvent>((EntityEventRefHandler<EvasionComponent, StoodEvent>)CallRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCSizeComponent, EvasionRefreshModifiersEvent>((EntityEventRefHandler<RMCSizeComponent, EvasionRefreshModifiersEvent>)OnSizeRefreshEvasion, (Type[])null, (Type[])null);
	}

	public void RefreshEvasionModifiers(EntityUid entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		EvasionComponent evasionComponent = default(EvasionComponent);
		if (((EntitySystem)this).TryComp<EvasionComponent>(entity, ref evasionComponent))
		{
			RefreshEvasionModifiers(Entity<EvasionComponent>.op_Implicit((entity, evasionComponent)));
		}
	}

	public void RefreshEvasionModifiers(Entity<EvasionComponent> entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EvasionRefreshModifiersEvent ev = new EvasionRefreshModifiersEvent(entity, entity.Comp.Evasion, entity.Comp.EvasionFriendly);
		((EntitySystem)this).RaiseLocalEvent<EvasionRefreshModifiersEvent>(entity.Owner, ref ev, false);
		entity.Comp.ModifiedEvasion = ev.Evasion;
		entity.Comp.ModifiedEvasionFriendly = ev.EvasionFriendly;
		((EntitySystem)this).Dirty<EvasionComponent>(entity, (MetaDataComponent)null);
	}

	private void CallRefresh<T>(Entity<EvasionComponent> entity, ref T args) where T : notnull
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RefreshEvasionModifiers(entity);
	}

	private void OnSizeRefreshEvasion(Entity<RMCSizeComponent> size, ref EvasionRefreshModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!(size.Owner != args.Entity.Owner))
		{
			if ((int)size.Comp.Size <= 0)
			{
				args.Evasion += (FixedPoint2)10;
			}
			if ((int)size.Comp.Size >= 5)
			{
				args.Evasion += (FixedPoint2)(-10);
			}
		}
	}
}
