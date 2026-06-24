using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.CCVar;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Client.Graphics;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;

namespace Content.Client.Vehicle;

public sealed class GridVehicleMoverSystem : EntitySystem
{
	[Dependency]
	private readonly IConfigurationManager _cfg;

	[Dependency]
	private readonly IPlayerManager _playerManager;

	[Dependency]
	private readonly PhysicsSystem _physics;

	[Dependency]
	private readonly IOverlayManager _overlayManager;

	public static readonly List<Vector2> DebugCollisionPositions = new List<Vector2>();

	private GridVehicleMoverOverlay? _overlay;

	private VehicleHardpointDebugOverlay? _hardpointOverlay;

	private EntityUid? _lastPredictedVehicle;

	public override void Initialize()
	{
		_overlay = new GridVehicleMoverOverlay((IEntityManager)(object)base.EntityManager);
		_overlay.DebugEnabled = _cfg.GetCVar<bool>(RMCCVars.VehicleDebugOverlay);
		_overlay.CollisionsEnabled = _cfg.GetCVar<bool>(RMCCVars.VehicleCollisionOverlay);
		_overlay.MovementEnabled = _cfg.GetCVar<bool>(RMCCVars.VehicleMovementOverlay);
		RefreshSharedDebugFlags();
		_hardpointOverlay = new VehicleHardpointDebugOverlay((IEntityManager)(object)base.EntityManager)
		{
			Enabled = _cfg.GetCVar<bool>(RMCCVars.VehicleHardpointOverlay)
		};
		_cfg.OnValueChanged<bool>(RMCCVars.VehicleDebugOverlay, (Action<bool>)delegate(bool val)
		{
			if (_overlay != null)
			{
				_overlay.DebugEnabled = val;
			}
			RefreshSharedDebugFlags();
			RefreshVehicleDebugOverlay();
		}, true);
		_cfg.OnValueChanged<bool>(RMCCVars.VehicleHardpointOverlay, (Action<bool>)delegate(bool val)
		{
			if (_hardpointOverlay != null)
			{
				_hardpointOverlay.Enabled = val;
			}
		}, true);
		_cfg.OnValueChanged<bool>(RMCCVars.VehicleCollisionOverlay, (Action<bool>)delegate(bool val)
		{
			if (_overlay != null)
			{
				_overlay.CollisionsEnabled = val;
			}
			RefreshSharedDebugFlags();
			RefreshVehicleDebugOverlay();
		}, true);
		_cfg.OnValueChanged<bool>(RMCCVars.VehicleMovementOverlay, (Action<bool>)delegate(bool val)
		{
			if (_overlay != null)
			{
				_overlay.MovementEnabled = val;
			}
			RefreshSharedDebugFlags();
			RefreshVehicleDebugOverlay();
		}, true);
		((EntitySystem)this).SubscribeLocalEvent<GridVehicleMoverComponent, UpdateIsPredictedEvent>((EntityEventRefHandler<GridVehicleMoverComponent, UpdateIsPredictedEvent>)OnUpdateIsPredicted, (Type[])null, (Type[])null);
		RefreshVehicleDebugOverlay();
		_overlayManager.AddOverlay((Overlay)(object)_hardpointOverlay);
	}

	public override void Shutdown()
	{
		if (_overlay != null)
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
		}
		if (_hardpointOverlay != null)
		{
			_overlayManager.RemoveOverlay((Overlay)(object)_hardpointOverlay);
		}
	}

	private void RefreshSharedDebugFlags()
	{
		GridVehicleMoverOverlay overlay = _overlay;
		bool collisionDebugEnabled = ((overlay != null && (overlay.DebugEnabled || overlay.CollisionsEnabled)) ? true : false);
		Content.Shared.Vehicle.GridVehicleMoverSystem.CollisionDebugEnabled = collisionDebugEnabled;
		Content.Shared.Vehicle.GridVehicleMoverSystem.MovementDebugEnabled = _overlay?.MovementEnabled ?? false;
	}

	private void RefreshVehicleDebugOverlay()
	{
		if (_overlay != null)
		{
			bool flag = _overlay.DebugEnabled || _overlay.CollisionsEnabled || _overlay.MovementEnabled;
			bool flag2 = _overlayManager.HasOverlay<GridVehicleMoverOverlay>();
			if (flag && !flag2)
			{
				_overlayManager.AddOverlay((Overlay)(object)_overlay);
			}
			else if (!flag && flag2)
			{
				_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
			}
		}
	}

	private void OnUpdateIsPredicted(Entity<GridVehicleMoverComponent> ent, ref UpdateIsPredictedEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		VehicleComponent vehicleComponent = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(ent.Owner, ref vehicleComponent))
		{
			localEntity = vehicleComponent.Operator;
			EntityUid val = valueOrDefault;
			if (localEntity.HasValue && localEntity.GetValueOrDefault() == val)
			{
				args.IsPredicted = true;
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			VehicleOperatorComponent vehicleOperatorComponent = default(VehicleOperatorComponent);
			if (((EntitySystem)this).TryComp<VehicleOperatorComponent>(valueOrDefault, ref vehicleOperatorComponent))
			{
				localEntity = vehicleOperatorComponent.Vehicle;
				if (localEntity.HasValue)
				{
					EntityUid valueOrDefault2 = localEntity.GetValueOrDefault();
					localEntity = _lastPredictedVehicle;
					EntityUid val = valueOrDefault2;
					if (!localEntity.HasValue || localEntity.GetValueOrDefault() != val)
					{
						localEntity = _lastPredictedVehicle;
						if (localEntity.HasValue)
						{
							EntityUid valueOrDefault3 = localEntity.GetValueOrDefault();
							((SharedPhysicsSystem)_physics).UpdateIsPredicted((EntityUid?)valueOrDefault3, (PhysicsComponent)null);
						}
						_lastPredictedVehicle = valueOrDefault2;
						((SharedPhysicsSystem)_physics).UpdateIsPredicted((EntityUid?)valueOrDefault2, (PhysicsComponent)null);
					}
					return;
				}
			}
			localEntity = _lastPredictedVehicle;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault4 = localEntity.GetValueOrDefault();
				((SharedPhysicsSystem)_physics).UpdateIsPredicted((EntityUid?)valueOrDefault4, (PhysicsComponent)null);
				_lastPredictedVehicle = null;
			}
		}
		else
		{
			localEntity = _lastPredictedVehicle;
			if (localEntity.HasValue)
			{
				EntityUid valueOrDefault5 = localEntity.GetValueOrDefault();
				((SharedPhysicsSystem)_physics).UpdateIsPredicted((EntityUid?)valueOrDefault5, (PhysicsComponent)null);
			}
			_lastPredictedVehicle = null;
		}
	}
}
