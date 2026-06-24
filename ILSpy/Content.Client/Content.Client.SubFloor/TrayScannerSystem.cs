using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Vents;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.SubFloor;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.SubFloor;

public sealed class TrayScannerSystem : SharedTrayScannerSystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private AnimationPlayerSystem _animation;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private TrayScanRevealSystem _trayScanReveal;

	private const string TRayAnimationKey = "trays";

	private const double AnimationLength = 0.3;

	public const LookupFlags Flags = (LookupFlags)13;

	public override void Update(float frameTime)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Expected O, but got Unknown
		//IL_04d6: Expected O, but got Unknown
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Expected O, but got Unknown
		//IL_03b1: Expected O, but got Unknown
		((EntitySystem)this).Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityQuery<TransformComponent> entityQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		TransformComponent val = default(TransformComponent);
		if (!entityQuery.TryGetComponent(localEntity, ref val))
		{
			return;
		}
		Vector2 worldPosition = _transform.GetWorldPosition(val, entityQuery);
		MapId mapID = val.MapID;
		float num = 0f;
		EntityQuery<TrayScannerComponent> entityQuery2 = ((EntitySystem)this).GetEntityQuery<TrayScannerComponent>();
		EntityQuery<RMCTrayCrawlerComponent> entityQuery3 = ((EntitySystem)this).GetEntityQuery<RMCTrayCrawlerComponent>();
		bool flag = false;
		if (_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(localEntity.Value), out var containerSlotEnumerator))
		{
			ContainerSlot container;
			TrayScannerComponent trayScannerComponent = default(TrayScannerComponent);
			while (containerSlotEnumerator.MoveNext(out container))
			{
				foreach (EntityUid containedEntity in ((BaseContainer)container).ContainedEntities)
				{
					if (entityQuery2.TryGetComponent(containedEntity, ref trayScannerComponent) && trayScannerComponent.Enabled)
					{
						flag = true;
						num = MathF.Max(num, trayScannerComponent.Range);
					}
				}
			}
		}
		RMCTrayCrawlerComponent rMCTrayCrawlerComponent = default(RMCTrayCrawlerComponent);
		if (entityQuery3.TryGetComponent(localEntity.Value, ref rMCTrayCrawlerComponent) && rMCTrayCrawlerComponent.Enabled)
		{
			num = MathF.Max(rMCTrayCrawlerComponent.Range, num);
			flag = true;
		}
		else
		{
			TrayScannerComponent trayScannerComponent2 = default(TrayScannerComponent);
			foreach (string item2 in _hands.EnumerateHands(Entity<HandsComponent>.op_Implicit(localEntity.Value)))
			{
				if (_hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit(localEntity.Value), item2, out var held) && entityQuery2.TryGetComponent(held, ref trayScannerComponent2) && trayScannerComponent2.Enabled)
				{
					num = MathF.Max(trayScannerComponent2.Range, num);
					flag = true;
					break;
				}
			}
		}
		HashSet<Entity<SubFloorHideComponent>> hashSet = new HashSet<Entity<SubFloorHideComponent>>();
		if (flag)
		{
			_lookup.GetEntitiesInRange<SubFloorHideComponent>(mapID, worldPosition, num, hashSet, (LookupFlags)13);
			EntityUid val2 = default(EntityUid);
			SubFloorHideComponent subFloorHideComponent = default(SubFloorHideComponent);
			foreach (Entity<SubFloorHideComponent> item3 in hashSet)
			{
				item3.Deconstruct(ref val2, ref subFloorHideComponent);
				EntityUid val3 = val2;
				if (subFloorHideComponent.IsUnderCover || _trayScanReveal.IsUnderRevealingEntity(val3))
				{
					((EntitySystem)this).EnsureComp<TrayRevealedComponent>(val3);
				}
			}
		}
		AllEntityQueryEnumerator<TrayRevealedComponent, SpriteComponent> val4 = ((EntitySystem)this).AllEntityQuery<TrayRevealedComponent, SpriteComponent>();
		EntityQuery<SubFloorHideComponent> entityQuery4 = ((EntitySystem)this).GetEntityQuery<SubFloorHideComponent>();
		EntityUid val5 = default(EntityUid);
		TrayRevealedComponent trayRevealedComponent = default(TrayRevealedComponent);
		SpriteComponent val6 = default(SpriteComponent);
		SubFloorHideComponent item = default(SubFloorHideComponent);
		bool flag2 = default(bool);
		while (val4.MoveNext(ref val5, ref trayRevealedComponent, ref val6))
		{
			Color color;
			if (entityQuery4.TryGetComponent(val5, ref item) && hashSet.Contains(Entity<SubFloorHideComponent>.op_Implicit((val5, item))))
			{
				if ((!_appearance.TryGetData<bool>(val5, (Enum)SubFloorVisuals.ScannerRevealed, ref flag2, (AppearanceComponent)null) || !flag2) && val6.Color.A > 0.8f)
				{
					SpriteSystem sprite = _sprite;
					Entity<SpriteComponent> val7 = Entity<SpriteComponent>.op_Implicit((val5, val6));
					color = val6.Color;
					sprite.SetColor(val7, ((Color)(ref color)).WithAlpha(0f));
				}
				SetRevealed(val5, value: true);
				if (!(val6.Color.A >= 0.8f) && !_animation.HasRunningAnimation(val5, "trays"))
				{
					AnimationPlayerSystem animation = _animation;
					EntityUid val8 = val5;
					Animation val9 = new Animation
					{
						Length = TimeSpan.FromSeconds(0.3)
					};
					List<AnimationTrack> animationTracks = val9.AnimationTracks;
					AnimationTrackComponentProperty val10 = new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Color"
					};
					List<KeyFrame> keyFrames = ((AnimationTrackProperty)val10).KeyFrames;
					color = val6.Color;
					keyFrames.Add(new KeyFrame((object)((Color)(ref color)).WithAlpha(0f), 0f, (Func<float, float>)null));
					List<KeyFrame> keyFrames2 = ((AnimationTrackProperty)val10).KeyFrames;
					color = val6.Color;
					keyFrames2.Add(new KeyFrame((object)((Color)(ref color)).WithAlpha(0.8f), 0.3f, (Func<float, float>)null));
					animationTracks.Add((AnimationTrack)val10);
					animation.Play(val8, val9, "trays");
				}
			}
			else if (val6.Color.A <= 0f)
			{
				SetRevealed(val5, value: false);
				((EntitySystem)this).RemCompDeferred<TrayRevealedComponent>(val5);
				SpriteSystem sprite2 = _sprite;
				Entity<SpriteComponent> val11 = Entity<SpriteComponent>.op_Implicit((val5, val6));
				color = val6.Color;
				sprite2.SetColor(val11, ((Color)(ref color)).WithAlpha(1f));
			}
			else
			{
				SetRevealed(val5, value: true);
				if (!_animation.HasRunningAnimation(val5, "trays"))
				{
					AnimationPlayerSystem animation2 = _animation;
					EntityUid val12 = val5;
					Animation val13 = new Animation
					{
						Length = TimeSpan.FromSeconds(0.3)
					};
					List<AnimationTrack> animationTracks2 = val13.AnimationTracks;
					AnimationTrackComponentProperty val14 = new AnimationTrackComponentProperty
					{
						ComponentType = typeof(SpriteComponent),
						Property = "Color",
						KeyFrames = 
						{
							new KeyFrame((object)val6.Color, 0f, (Func<float, float>)null)
						}
					};
					List<KeyFrame> keyFrames3 = ((AnimationTrackProperty)val14).KeyFrames;
					color = val6.Color;
					keyFrames3.Add(new KeyFrame((object)((Color)(ref color)).WithAlpha(0f), 0.3f, (Func<float, float>)null));
					animationTracks2.Add((AnimationTrack)val14);
					animation2.Play(val12, val13, "trays");
				}
			}
		}
	}

	private void SetRevealed(EntityUid uid, bool value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)SubFloorVisuals.ScannerRevealed, (object)value, (AppearanceComponent)null);
	}
}
