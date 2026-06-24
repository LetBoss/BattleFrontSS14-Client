using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Body.Components;
using Content.Shared.CardboardBox;
using Content.Shared.CardboardBox.Components;
using Content.Shared.Examine;
using Content.Shared.Movement.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.CardboardBox;

public sealed class CardboardBoxSystem : SharedCardboardBoxSystem
{
	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private TransformSystem _transform;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private SpriteSystem _sprite;

	private EntityQuery<BodyComponent> _bodyQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_bodyQuery = ((EntitySystem)this).GetEntityQuery<BodyComponent>();
		((EntitySystem)this).SubscribeNetworkEvent<PlayBoxEffectMessage>((EntityEventHandler<PlayBoxEffectMessage>)OnBoxEffect, (Type[])null, (Type[])null);
	}

	private void OnBoxEffect(PlayBoxEffectMessage msg)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(msg.Source);
		CardboardBoxComponent cardboardBoxComponent = default(CardboardBoxComponent);
		if (!((EntitySystem)this).TryComp<CardboardBoxComponent>(entity, ref cardboardBoxComponent))
		{
			return;
		}
		EntityQuery<TransformComponent> entityQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		TransformComponent val = default(TransformComponent);
		if (!entityQuery.TryGetComponent(entity, ref val))
		{
			return;
		}
		MapCoordinates mapCoordinates = ((SharedTransformSystem)_transform).GetMapCoordinates(entity, val);
		List<EntityUid> list = new List<EntityUid>();
		EntityUid entity2 = ((EntitySystem)this).GetEntity(msg.Mover);
		HashSet<Entity<MobMoverComponent>> hashSet = new HashSet<Entity<MobMoverComponent>>();
		_entityLookup.GetEntitiesInRange<MobMoverComponent>(val.Coordinates, cardboardBoxComponent.Distance, hashSet, (LookupFlags)110);
		foreach (Entity<MobMoverComponent> item2 in hashSet)
		{
			EntityUid owner = item2.Owner;
			if (!(owner == entity2))
			{
				list.Add(owner);
			}
		}
		TransformComponent val3 = default(TransformComponent);
		SpriteComponent item = default(SpriteComponent);
		foreach (EntityUid item3 in list)
		{
			MapCoordinates mapCoordinates2 = ((SharedTransformSystem)_transform).GetMapCoordinates(item3, (TransformComponent)null);
			if (_examine.InRangeUnOccluded(mapCoordinates, mapCoordinates2, cardboardBoxComponent.Distance, null) && _bodyQuery.HasComp(item3))
			{
				EntityUid val2 = ((EntitySystem)this).Spawn(cardboardBoxComponent.Effect, mapCoordinates2, (ComponentRegistry)null, default(Angle));
				if (entityQuery.TryGetComponent(val2, ref val3) && ((EntitySystem)this).TryComp<SpriteComponent>(val2, ref item))
				{
					_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((val2, item)), new Vector2(0f, 1f));
					((SharedTransformSystem)_transform).SetParent(val2, val3, item3, (TransformComponent)null);
				}
			}
		}
	}
}
