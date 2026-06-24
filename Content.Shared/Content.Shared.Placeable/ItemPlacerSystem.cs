using System;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;

namespace Content.Shared.Placeable;

public sealed class ItemPlacerSystem : EntitySystem
{
	[Dependency]
	private CollisionWakeSystem _wake;

	[Dependency]
	private PlaceableSurfaceSystem _placeableSurface;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemPlacerComponent, StartCollideEvent>((ComponentEventRefHandler<ItemPlacerComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemPlacerComponent, EndCollideEvent>((ComponentEventRefHandler<ItemPlacerComponent, EndCollideEvent>)OnEndCollide, (Type[])null, (Type[])null);
	}

	private void OnStartCollide(EntityUid uid, ItemPlacerComponent comp, ref StartCollideEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (!_whitelistSystem.IsWhitelistFail(comp.Whitelist, args.OtherEntity))
		{
			CollisionWakeComponent wakeComp = default(CollisionWakeComponent);
			if (((EntitySystem)this).TryComp<CollisionWakeComponent>(args.OtherEntity, ref wakeComp))
			{
				_wake.SetEnabled(args.OtherEntity, false, wakeComp);
			}
			int count = comp.PlacedEntities.Count;
			if (comp.MaxEntities == 0 || count < comp.MaxEntities)
			{
				comp.PlacedEntities.Add(args.OtherEntity);
				ItemPlacedEvent ev = new ItemPlacedEvent(args.OtherEntity);
				((EntitySystem)this).RaiseLocalEvent<ItemPlacedEvent>(uid, ref ev, false);
			}
			if (comp.MaxEntities != 0 && count >= comp.MaxEntities - 1)
			{
				_placeableSurface.SetPlaceable(uid, isPlaceable: false);
			}
		}
	}

	private void OnEndCollide(EntityUid uid, ItemPlacerComponent comp, ref EndCollideEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		CollisionWakeComponent wakeComp = default(CollisionWakeComponent);
		if (((EntitySystem)this).TryComp<CollisionWakeComponent>(args.OtherEntity, ref wakeComp))
		{
			_wake.SetEnabled(args.OtherEntity, true, wakeComp);
		}
		comp.PlacedEntities.Remove(args.OtherEntity);
		ItemRemovedEvent ev = new ItemRemovedEvent(args.OtherEntity);
		((EntitySystem)this).RaiseLocalEvent<ItemRemovedEvent>(uid, ref ev, false);
		_placeableSurface.SetPlaceable(uid, isPlaceable: true);
	}
}
