using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.CombatMode;
using Content.Shared._RMC14.Vehicle;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleTurretInputSystem : EntitySystem
{
	private const float AimUpdateInterval = 0.1f;

	private static readonly Angle AimEpsilon = Angle.FromDegrees(1.0);

	[Dependency]
	private readonly CombatModeSystem _combat;

	[Dependency]
	private readonly IEyeManager _eye;

	[Dependency]
	private readonly IInputManager _input;

	[Dependency]
	private readonly IPlayerManager _player;

	[Dependency]
	private readonly VehicleTurretSystem _turrets;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	[Dependency]
	private readonly IGameTiming _timing;

	private readonly Dictionary<EntityUid, (Angle Angle, TimeSpan Time)> _lastAims = new Dictionary<EntityUid, (Angle, TimeSpan)>();

	private readonly Dictionary<EntityUid, MapCoordinates> _lastAimCoordinates = new Dictionary<EntityUid, MapCoordinates>();

	public override void Update(float frameTime)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		VehicleWeaponsOperatorComponent vehicleWeaponsOperatorComponent = default(VehicleWeaponsOperatorComponent);
		if (!_combat.IsInCombatMode(valueOrDefault) || !((EntitySystem)this).TryComp<VehicleWeaponsOperatorComponent>(valueOrDefault, ref vehicleWeaponsOperatorComponent))
		{
			return;
		}
		localEntity = vehicleWeaponsOperatorComponent.Vehicle;
		if (!localEntity.HasValue)
		{
			return;
		}
		localEntity = vehicleWeaponsOperatorComponent.SelectedWeapon;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault2 = localEntity.GetValueOrDefault();
		VehicleTurretComponent vehicleTurretComponent = default(VehicleTurretComponent);
		if (!((EntitySystem)this).TryComp<VehicleTurretComponent>(valueOrDefault2, ref vehicleTurretComponent) || !_turrets.TryResolveRotationTarget(valueOrDefault2, out EntityUid targetUid, out VehicleTurretComponent targetTurret) || !targetTurret.RotateToCursor || !_turrets.TryGetTurretOrigin(targetUid, targetTurret, out var origin))
		{
			return;
		}
		MapCoordinates val = _eye.PixelToMap(_input.MouseScreenPosition);
		if (val.MapId == MapId.Nullspace)
		{
			return;
		}
		_lastAimCoordinates[valueOrDefault2] = val;
		MapCoordinates val2 = _transform.ToMapCoordinates(origin, true);
		Vector2 vector = val.Position - val2.Position;
		if (vector.LengthSquared() <= 0.0001f)
		{
			return;
		}
		Angle item = DirectionExtensions.ToWorldAngle(vector);
		if (_lastAims.TryGetValue(targetUid, out (Angle, TimeSpan) value) && (_timing.CurTime - value.Item2).TotalSeconds < 0.10000000149011612)
		{
			Angle val3 = Angle.ShortestDistance(ref item, ref value.Item1);
			if (Math.Abs(((Angle)(ref val3)).Degrees) < ((Angle)(ref AimEpsilon)).Degrees)
			{
				return;
			}
		}
		_lastAims[targetUid] = (item, _timing.CurTime);
		EntityCoordinates val4 = _transform.ToCoordinates(val);
		((EntitySystem)this).RaisePredictiveEvent<VehicleTurretRotateEvent>(new VehicleTurretRotateEvent
		{
			Turret = ((EntitySystem)this).GetNetEntity(valueOrDefault2, (MetaDataComponent)null),
			Coordinates = ((EntitySystem)this).GetNetCoordinates(val4, (MetaDataComponent)null)
		});
	}

	public bool TryGetLastAimCoordinates(EntityUid turretUid, out MapCoordinates coordinates)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _lastAimCoordinates.TryGetValue(turretUid, out coordinates);
	}
}
