using System;
using System.Collections.Generic;
using Content.Client.Gameplay;
using Content.Shared.Sprite;
using Robust.Client.GameObjects;
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

namespace Content.Client.Sprite;

public sealed class SpriteFadeSystem : EntitySystem
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
	private EntityLookupSystem _lookup;

	private List<(MapCoordinates Point, bool ExcludeBoundingBox)> _points = new List<(MapCoordinates, bool)>();

	private readonly HashSet<FadingSpriteComponent> _comps = new HashSet<FadingSpriteComponent>();

	private readonly HashSet<Entity<SpriteFadeComponent>> _fadeEntities = new HashSet<Entity<SpriteFadeComponent>>();

	private EntityQuery<SpriteComponent> _spriteQuery;

	private EntityQuery<SpriteFadeComponent> _fadeQuery;

	private EntityQuery<FadingSpriteComponent> _fadingQuery;

	private EntityQuery<FixturesComponent> _fixturesQuery;

	private const float TargetAlpha = 0.4f;

	private const float ChangeRate = 1f;

	private const float MouseFadeRadius = 3f;

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
		_fadeQuery = ((EntitySystem)this).GetEntityQuery<SpriteFadeComponent>();
		_fadingQuery = ((EntitySystem)this).GetEntityQuery<FadingSpriteComponent>();
		_fixturesQuery = ((EntitySystem)this).GetEntityQuery<FixturesComponent>();
		((EntitySystem)this).SubscribeLocalEvent<FadingSpriteComponent, ComponentShutdown>((ComponentEventHandler<FadingSpriteComponent, ComponentShutdown>)OnFadingShutdown, (Type[])null, (Type[])null);
	}

	private void OnFadingShutdown(EntityUid uid, FadingSpriteComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if ((int)((EntitySystem)this).MetaData(uid).EntityLifeStage < 4 && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			SpriteSystem sprite = _sprite;
			Entity<SpriteComponent> val2 = Entity<SpriteComponent>.op_Implicit((uid, val));
			Color color = val.Color;
			sprite.SetColor(val2, ((Color)(ref color)).WithAlpha(component.OriginalAlpha));
		}
	}

	private void FadeIn(float change)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		_points.Clear();
		MapCoordinates? val = null;
		Control currentlyHovered = _uiManager.CurrentlyHovered;
		IViewportControl val2 = (IViewportControl)(object)((currentlyHovered is IViewportControl) ? currentlyHovered : null);
		if (val2 != null)
		{
			ScreenCoordinates mouseScreenPosition = _inputManager.MouseScreenPosition;
			if (((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
			{
				val = val2.PixelToMap(_inputManager.MouseScreenPosition.Position);
			}
		}
		TransformComponent val3 = default(TransformComponent);
		if (((EntitySystem)this).TryComp(localEntity, ref val3))
		{
			_points.Add((_transform.GetMapCoordinates(((ISharedPlayerManager)_playerManager).LocalEntity.Value, val3), false));
		}
		SpriteComponent val4 = default(SpriteComponent);
		if (!(_stateManager.CurrentState is GameplayState gameplayState) || !_spriteQuery.TryGetComponent(localEntity, ref val4))
		{
			return;
		}
		_fadeEntities.Clear();
		if (val.HasValue)
		{
			_lookup.GetEntitiesInRange<SpriteFadeComponent>(val.Value, 3f, _fadeEntities, (LookupFlags)110);
		}
		EntityUid val6 = default(EntityUid);
		SpriteComponent val8 = default(SpriteComponent);
		FixturesComponent val9 = default(FixturesComponent);
		FadingSpriteComponent fadingSpriteComponent = default(FadingSpriteComponent);
		Color color;
		foreach (var (val5, flag) in _points)
		{
			foreach (EntityUid clickableEntity in gameplayState.GetClickableEntities(val5, excludeFaded: false))
			{
				val6 = clickableEntity;
				EntityUid? val7 = localEntity;
				if ((val7.HasValue && val6 == val7.GetValueOrDefault()) || !_fadeQuery.HasComponent(clickableEntity) || !_spriteQuery.TryGetComponent(clickableEntity, ref val8) || val8.DrawDepth < val4.DrawDepth)
				{
					continue;
				}
				if (flag && _fixturesQuery.TryComp(clickableEntity, ref val9))
				{
					Transform physicsTransform = _physics.GetPhysicsTransform(clickableEntity, (TransformComponent)null);
					bool flag2 = false;
					foreach (Fixture value in val9.Fixtures.Values)
					{
						if (value.Hard && _fixtures.TestPoint<IPhysShape>(value.Shape, physicsTransform, val5.Position))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						continue;
					}
				}
				if (!_fadingQuery.TryComp(clickableEntity, ref fadingSpriteComponent))
				{
					fadingSpriteComponent = ((EntitySystem)this).AddComp<FadingSpriteComponent>(clickableEntity);
					fadingSpriteComponent.OriginalAlpha = val8.Color.A;
				}
				_comps.Add(fadingSpriteComponent);
				float num = Math.Max(val8.Color.A - change, 0.4f);
				color = val8.Color;
				if (!color.A.Equals(num))
				{
					SpriteSystem sprite = _sprite;
					Entity<SpriteComponent> val10 = Entity<SpriteComponent>.op_Implicit((clickableEntity, val8));
					color = val8.Color;
					sprite.SetColor(val10, ((Color)(ref color)).WithAlpha(num));
				}
			}
		}
		SpriteFadeComponent spriteFadeComponent = default(SpriteFadeComponent);
		SpriteComponent val12 = default(SpriteComponent);
		FadingSpriteComponent fadingSpriteComponent2 = default(FadingSpriteComponent);
		foreach (Entity<SpriteFadeComponent> fadeEntity in _fadeEntities)
		{
			fadeEntity.Deconstruct(ref val6, ref spriteFadeComponent);
			EntityUid val11 = val6;
			val6 = val11;
			EntityUid? val7 = localEntity;
			if ((!val7.HasValue || !(val6 == val7.GetValueOrDefault())) && _spriteQuery.TryGetComponent(val11, ref val12) && val12.DrawDepth >= val4.DrawDepth)
			{
				if (!_fadingQuery.TryComp(val11, ref fadingSpriteComponent2))
				{
					fadingSpriteComponent2 = ((EntitySystem)this).AddComp<FadingSpriteComponent>(val11);
					fadingSpriteComponent2.OriginalAlpha = val12.Color.A;
				}
				_comps.Add(fadingSpriteComponent2);
				float num2 = Math.Max(val12.Color.A - change, 0.4f);
				color = val12.Color;
				if (!color.A.Equals(num2))
				{
					SpriteSystem sprite2 = _sprite;
					Entity<SpriteComponent> val13 = Entity<SpriteComponent>.op_Implicit((val11, val12));
					color = val12.Color;
					sprite2.SetColor(val13, ((Color)(ref color)).WithAlpha(num2));
				}
			}
		}
	}

	private void FadeOut(float change)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<FadingSpriteComponent> val = ((EntitySystem)this).AllEntityQuery<FadingSpriteComponent>();
		EntityUid val2 = default(EntityUid);
		FadingSpriteComponent fadingSpriteComponent = default(FadingSpriteComponent);
		SpriteComponent val3 = default(SpriteComponent);
		while (val.MoveNext(ref val2, ref fadingSpriteComponent))
		{
			if (!_comps.Contains(fadingSpriteComponent) && _spriteQuery.TryGetComponent(val2, ref val3))
			{
				float num = Math.Min(val3.Color.A + change, fadingSpriteComponent.OriginalAlpha);
				if (!num.Equals(val3.Color.A))
				{
					SpriteSystem sprite = _sprite;
					Entity<SpriteComponent> val4 = Entity<SpriteComponent>.op_Implicit((val2, val3));
					Color color = val3.Color;
					sprite.SetColor(val4, ((Color)(ref color)).WithAlpha(num));
				}
				else
				{
					((EntitySystem)this).RemCompDeferred<FadingSpriteComponent>(val2);
				}
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		((EntitySystem)this).FrameUpdate(frameTime);
		float change = 1f * frameTime;
		FadeIn(change);
		FadeOut(change);
		_comps.Clear();
	}
}
