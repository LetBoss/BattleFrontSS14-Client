using System;
using System.Collections.Generic;
using Content.Client.Gameplay;
using Content.Shared._RMC14.Sprite;
using Content.Shared.Ghost;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;

namespace Content.Client._RMC14.Sprite;

public sealed class RMCSpriteFadeSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IStateManager _stateManager;

	[Dependency]
	private FixtureSystem _fixtures;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private IUserInterfaceManager _uiManager;

	[Dependency]
	private IInputManager _inputManager;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private IEyeManager _eyeManager;

	private List<(MapCoordinates Point, bool ExcludeBoundingBox)> _points = new List<(MapCoordinates, bool)>();

	private readonly HashSet<RMCFadingSpriteComponent> _comps = new HashSet<RMCFadingSpriteComponent>();

	private EntityQuery<SpriteComponent> _spriteQuery;

	private EntityQuery<RMCSpriteFadeComponent> _fadeQuery;

	private EntityQuery<RMCFadingSpriteComponent> _fadingQuery;

	private EntityQuery<FixturesComponent> _fixturesQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_spriteQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
		_fadeQuery = ((EntitySystem)this).GetEntityQuery<RMCSpriteFadeComponent>();
		_fadingQuery = ((EntitySystem)this).GetEntityQuery<RMCFadingSpriteComponent>();
		_fixturesQuery = ((EntitySystem)this).GetEntityQuery<FixturesComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RMCFadingSpriteComponent, ComponentRemove>((EntityEventRefHandler<RMCFadingSpriteComponent, ComponentRemove>)OnFadingRemove, (Type[])null, (Type[])null);
	}

	private void OnFadingRemove(Entity<RMCFadingSpriteComponent> entity, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if ((int)((EntitySystem)this).MetaData(Entity<RMCFadingSpriteComponent>.op_Implicit(entity)).EntityLifeStage >= 4 || !((EntitySystem)this).TryComp<SpriteComponent>(Entity<RMCFadingSpriteComponent>.op_Implicit(entity), ref val))
		{
			return;
		}
		SpriteSystem sprite = _sprite;
		Entity<SpriteComponent> val2 = Entity<SpriteComponent>.op_Implicit((Entity<RMCFadingSpriteComponent>.op_Implicit(entity), val));
		Color color = val.Color;
		sprite.SetColor(val2, ((Color)(ref color)).WithAlpha(entity.Comp.OriginalAlpha));
		int num3 = default(int);
		foreach (var (text2, num2) in entity.Comp.OriginalLayerAlphas)
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<RMCFadingSpriteComponent>.op_Implicit(entity), val)), text2, ref num3, true))
			{
				ISpriteLayer val3 = val[num3];
				SpriteSystem sprite2 = _sprite;
				Entity<SpriteComponent> val4 = Entity<SpriteComponent>.op_Implicit((Entity<RMCFadingSpriteComponent>.op_Implicit(entity), val));
				int num4 = num3;
				color = val3.Color;
				sprite2.LayerSetColor(val4, num4, ((Color)(ref color)).WithAlpha(num2));
			}
		}
	}

	private void FadeIn(float frameTime)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		_points.Clear();
		Control currentlyHovered = _uiManager.CurrentlyHovered;
		IViewportControl val = (IViewportControl)(object)((currentlyHovered is IViewportControl) ? currentlyHovered : null);
		if (val != null)
		{
			ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
			if (((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
			{
				_points.Add((val.PixelToMap(_inputManager.MouseScreenPosition.Position), true));
			}
		}
		TransformComponent val2 = default(TransformComponent);
		if (((EntitySystem)this).TryComp(localEntity, ref val2))
		{
			_points.Add((_transform.GetMapCoordinates(((ISharedPlayerManager)_playerManager).LocalEntity.Value, val2), false));
		}
		SpriteComponent val3 = default(SpriteComponent);
		if (!(_stateManager.CurrentState is GameplayState gameplayState) || !_spriteQuery.TryGetComponent(localEntity, ref val3))
		{
			return;
		}
		bool flag = localEntity.HasValue && ((EntitySystem)this).HasComp<GhostComponent>(localEntity.Value);
		SpriteComponent val7 = default(SpriteComponent);
		FixturesComponent val8 = default(FixturesComponent);
		RMCFadingSpriteComponent rMCFadingSpriteComponent = default(RMCFadingSpriteComponent);
		int num2 = default(int);
		foreach (var (val4, flag2) in _points)
		{
			foreach (EntityUid clickableEntity in gameplayState.GetClickableEntities(val4, _eyeManager.CurrentEye, excludeFaded: false, ignoreInteractionTransparency: true))
			{
				EntityUid val5 = clickableEntity;
				EntityUid? val6 = localEntity;
				if ((val6.HasValue && val5 == val6.GetValueOrDefault()) || !_fadeQuery.HasComponent(clickableEntity) || !_spriteQuery.TryGetComponent(clickableEntity, ref val7) || (!flag && val7.DrawDepth < val3.DrawDepth))
				{
					continue;
				}
				if (flag2 && _fixturesQuery.TryComp(clickableEntity, ref val8))
				{
					Transform physicsTransform = _physics.GetPhysicsTransform(clickableEntity, (TransformComponent)null);
					bool flag3 = false;
					foreach (Fixture value in val8.Fixtures.Values)
					{
						if (value.Hard && _fixtures.TestPoint<IPhysShape>(value.Shape, physicsTransform, val4.Position))
						{
							flag3 = true;
							break;
						}
					}
					if (flag3)
					{
						continue;
					}
				}
				RMCSpriteFadeComponent component = _fadeQuery.GetComponent(clickableEntity);
				if (flag2 && !component.ReactToMouse)
				{
					continue;
				}
				if (!_fadingQuery.TryComp(clickableEntity, ref rMCFadingSpriteComponent))
				{
					rMCFadingSpriteComponent = ((EntitySystem)this).AddComp<RMCFadingSpriteComponent>(clickableEntity);
					rMCFadingSpriteComponent.OriginalAlpha = val7.Color.A;
				}
				_comps.Add(rMCFadingSpriteComponent);
				float targetAlpha = component.TargetAlpha;
				float num = component.ChangeRate * frameTime;
				Color color;
				if (component.FadeLayers.Count > 0)
				{
					foreach (string fadeLayer in component.FadeLayers)
					{
						if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((clickableEntity, val7)), fadeLayer, ref num2, true))
						{
							ISpriteLayer val9 = val7[num2];
							if (!rMCFadingSpriteComponent.OriginalLayerAlphas.ContainsKey(fadeLayer))
							{
								rMCFadingSpriteComponent.OriginalLayerAlphas[fadeLayer] = val9.Color.A;
							}
							float num3 = Math.Max(val9.Color.A - num, targetAlpha);
							color = val9.Color;
							if (!color.A.Equals(num3))
							{
								SpriteSystem sprite = _sprite;
								Entity<SpriteComponent> val10 = Entity<SpriteComponent>.op_Implicit((clickableEntity, val7));
								int num4 = num2;
								color = val9.Color;
								sprite.LayerSetColor(val10, num4, ((Color)(ref color)).WithAlpha(num3));
							}
						}
					}
				}
				else
				{
					float num5 = Math.Max(val7.Color.A - num, targetAlpha);
					color = val7.Color;
					if (!color.A.Equals(num5))
					{
						SpriteSystem sprite2 = _sprite;
						Entity<SpriteComponent> val11 = Entity<SpriteComponent>.op_Implicit((clickableEntity, val7));
						color = val7.Color;
						sprite2.SetColor(val11, ((Color)(ref color)).WithAlpha(num5));
					}
				}
			}
		}
	}

	private void FadeOut(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<RMCFadingSpriteComponent> val = ((EntitySystem)this).AllEntityQuery<RMCFadingSpriteComponent>();
		EntityUid val2 = default(EntityUid);
		RMCFadingSpriteComponent rMCFadingSpriteComponent = default(RMCFadingSpriteComponent);
		SpriteComponent val3 = default(SpriteComponent);
		RMCSpriteFadeComponent rMCSpriteFadeComponent = default(RMCSpriteFadeComponent);
		int num3 = default(int);
		while (val.MoveNext(ref val2, ref rMCFadingSpriteComponent))
		{
			if (_comps.Contains(rMCFadingSpriteComponent) || !_spriteQuery.TryGetComponent(val2, ref val3) || !_fadeQuery.TryComp(val2, ref rMCSpriteFadeComponent))
			{
				continue;
			}
			float num = rMCSpriteFadeComponent.ChangeRate * frameTime;
			Color color;
			if (rMCSpriteFadeComponent.FadeLayers.Count > 0)
			{
				bool flag = true;
				foreach (var (text2, val4) in rMCFadingSpriteComponent.OriginalLayerAlphas)
				{
					if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((val2, val3)), text2, ref num3, true))
					{
						ISpriteLayer val5 = val3[num3];
						float num4 = Math.Min(val5.Color.A + num, val4);
						if (!num4.Equals(val5.Color.A))
						{
							SpriteSystem sprite = _sprite;
							Entity<SpriteComponent> val6 = Entity<SpriteComponent>.op_Implicit((val2, val3));
							int num5 = num3;
							color = val5.Color;
							sprite.LayerSetColor(val6, num5, ((Color)(ref color)).WithAlpha(num4));
							flag = false;
						}
					}
				}
				if (flag)
				{
					((EntitySystem)this).RemCompDeferred<RMCFadingSpriteComponent>(val2);
				}
			}
			else
			{
				float num6 = Math.Min(val3.Color.A + num, rMCFadingSpriteComponent.OriginalAlpha);
				if (!num6.Equals(val3.Color.A))
				{
					SpriteSystem sprite2 = _sprite;
					Entity<SpriteComponent> val7 = Entity<SpriteComponent>.op_Implicit((val2, val3));
					color = val3.Color;
					sprite2.SetColor(val7, ((Color)(ref color)).WithAlpha(num6));
				}
				else
				{
					((EntitySystem)this).RemCompDeferred<RMCFadingSpriteComponent>(val2);
				}
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		((EntitySystem)this).FrameUpdate(frameTime);
		FadeIn(frameTime);
		FadeOut(frameTime);
		_comps.Clear();
	}
}
