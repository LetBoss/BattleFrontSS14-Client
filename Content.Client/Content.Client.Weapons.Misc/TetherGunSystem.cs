using System;
using Content.Shared.Weapons.Misc;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.Weapons.Misc;

public sealed class TetherGunSystem : SharedTetherGunSystem
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private MapSystem _mapSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<TetheredComponent, ComponentStartup>((ComponentEventHandler<TetheredComponent, ComponentStartup>)OnTetheredStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TetheredComponent, ComponentShutdown>((ComponentEventHandler<TetheredComponent, ComponentShutdown>)OnTetheredShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TetherGunComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<TetherGunComponent, AfterAutoHandleStateEvent>)OnAfterState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ForceGunComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<ForceGunComponent, AfterAutoHandleStateEvent>)OnAfterState, (Type[])null, (Type[])null);
		_overlay.AddOverlay((Overlay)(object)new TetherGunOverlay((IEntityManager)(object)((EntitySystem)this).EntityManager));
	}

	private void OnAfterState(EntityUid uid, BaseForceGunComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(component.Tethered, ref item))
		{
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((component.Tethered.Value, item)), component.LineColor);
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<TetherGunOverlay>();
	}

	protected override bool CanTether(EntityUid uid, BaseForceGunComponent component, EntityUid target, EntityUid? user)
	{
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue || !TryGetTetherGun(localEntity.Value, out EntityUid? _, out TetherGunComponent gun) || !gun.TetherEntity.HasValue)
		{
			return;
		}
		ScreenCoordinates mouseScreenPosition = _input.MouseScreenPosition;
		MapCoordinates val = _eyeManager.PixelToMap(mouseScreenPosition);
		if (val.MapId == MapId.Nullspace)
		{
			return;
		}
		EntityUid val3 = default(EntityUid);
		MapGridComponent val4 = default(MapGridComponent);
		EntityCoordinates val2 = ((!_mapManager.TryFindGridAt(val, ref val3, ref val4)) ? TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(((SharedMapSystem)_mapSystem).GetMap(val.MapId)), val) : TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(val3), val));
		TransformComponent val5 = default(TransformComponent);
		if (((EntitySystem)this).TryComp(gun.TetherEntity, ref val5))
		{
			EntityCoordinates coordinates = val5.Coordinates;
			float num = default(float);
			if (((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)((EntitySystem)this).EntityManager, TransformSystem, val2, ref num) && num < 0.1f)
			{
				return;
			}
		}
		((EntitySystem)this).RaisePredictiveEvent<RequestTetherMoveEvent>(new RequestTetherMoveEvent
		{
			Coordinates = ((EntitySystem)this).GetNetCoordinates(val2, (MetaDataComponent)null)
		});
	}

	private void OnTetheredStartup(EntityUid uid, TetheredComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			ForceGunComponent forceGunComponent = default(ForceGunComponent);
			TetherGunComponent tetherGunComponent = default(TetherGunComponent);
			if (((EntitySystem)this).TryComp<ForceGunComponent>(component.Tetherer, ref forceGunComponent))
			{
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, item)), forceGunComponent.LineColor);
			}
			else if (((EntitySystem)this).TryComp<TetherGunComponent>(component.Tetherer, ref tetherGunComponent))
			{
				_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, item)), tetherGunComponent.LineColor);
			}
		}
	}

	private void OnTetheredShutdown(EntityUid uid, TetheredComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, item)), Color.White);
		}
	}
}
