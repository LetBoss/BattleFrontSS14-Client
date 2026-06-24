using System;
using System.Numerics;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleTurretSystem : EntitySystem
{
	private const float PixelsPerMeter = 32f;

	private const float FireAlignmentToleranceDegrees = 2f;

	[Dependency]
	private readonly SharedContainerSystem _container;

	[Dependency]
	private readonly INetManager _net;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	[Dependency]
	private readonly IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).UpdatesAfter.Add(typeof(GridVehicleMoverSystem));
		((EntitySystem)this).SubscribeLocalEvent<VehicleTurretComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<VehicleTurretComponent, EntInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleTurretComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<VehicleTurretComponent, EntRemovedFromContainerMessage>)OnRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleTurretComponent, ComponentShutdown>((EntityEventRefHandler<VehicleTurretComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleTurretComponent, AttemptShootEvent>((EntityEventRefHandler<VehicleTurretComponent, AttemptShootEvent>)OnAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<VehicleTurretRotateEvent>((EntitySessionEventHandler<VehicleTurretRotateEvent>)OnRotateEvent, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient && _timing.ApplyingState)
		{
			return;
		}
		EntityQueryEnumerator<VehicleTurretComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VehicleTurretComponent>();
		EntityUid uid = default(EntityUid);
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		while (query.MoveNext(ref uid, ref turret))
		{
			if (ShouldUpdateTransforms(turret))
			{
				if (!TryGetVehicle(uid, out var vehicle))
				{
					CleanupVisual(turret);
				}
				else
				{
					UpdateTurretRotation(uid, turret, vehicle, frameTime);
				}
			}
		}
		query = ((EntitySystem)this).EntityQueryEnumerator<VehicleTurretComponent>();
		EntityUid uid2 = default(EntityUid);
		VehicleTurretComponent turret2 = default(VehicleTurretComponent);
		while (query.MoveNext(ref uid2, ref turret2))
		{
			if (ShouldUpdateTransforms(turret2))
			{
				if (!TryGetVehicle(uid2, out var vehicle2))
				{
					CleanupVisual(turret2);
					continue;
				}
				TryGetAnchorTurret(uid2, turret2, out EntityUid anchorUid, out VehicleTurretComponent anchorTurret);
				EnsureVisual(uid2, turret2, vehicle2);
				InitializeRotation(anchorUid, anchorTurret, vehicle2);
				UpdateTurretTransforms(uid2, turret2, vehicle2, anchorUid, anchorTurret);
			}
		}
	}

	private void OnInserted(Entity<VehicleTurretComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if ((!_net.IsClient || !_timing.ApplyingState) && ShouldUpdateTransforms(ent.Comp) && TryGetVehicle(ent.Owner, out var vehicle))
		{
			UpdateTurretRotation(ent.Owner, ent.Comp, vehicle, 0f);
			TryGetAnchorTurret(ent.Owner, ent.Comp, out EntityUid anchorUid, out VehicleTurretComponent anchorTurret);
			EnsureVisual(ent.Owner, ent.Comp, vehicle);
			InitializeRotation(anchorUid, anchorTurret, vehicle);
			UpdateTurretTransforms(ent.Owner, ent.Comp, vehicle, anchorUid, anchorTurret);
		}
	}

	private void OnRemoved(Entity<VehicleTurretComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CleanupVisual(ent.Comp);
	}

	private void OnShutdown(Entity<VehicleTurretComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CleanupVisual(ent.Comp);
	}

	private void OnRotateEvent(VehicleTurretRotateEvent args, EntitySessionEventArgs session)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		if ((_net.IsClient && !_timing.IsFirstTimePredicted) || (_net.IsClient && _timing.ApplyingState))
		{
			return;
		}
		EntityUid turretUid = ((EntitySystem)this).GetEntity(args.Turret);
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		if (!((EntitySystem)this).TryComp<VehicleTurretComponent>(turretUid, ref turret) || !TryGetVehicle(turretUid, out var vehicle))
		{
			return;
		}
		if (!_net.IsClient)
		{
			EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref session)).SenderSession.AttachedEntity;
			if (!attachedEntity.HasValue)
			{
				return;
			}
			EntityUid user = attachedEntity.GetValueOrDefault();
			VehicleViewToggleComponent viewToggle = default(VehicleViewToggleComponent);
			VehicleWeaponsOperatorComponent operatorComp = default(VehicleWeaponsOperatorComponent);
			if ((((EntitySystem)this).TryComp<VehicleViewToggleComponent>(user, ref viewToggle) && !viewToggle.IsOutside) || !((EntitySystem)this).TryComp<VehicleWeaponsOperatorComponent>(user, ref operatorComp))
			{
				return;
			}
			attachedEntity = operatorComp.Vehicle;
			EntityUid val = vehicle;
			if (!attachedEntity.HasValue || attachedEntity.GetValueOrDefault() != val)
			{
				return;
			}
			attachedEntity = operatorComp.SelectedWeapon;
			val = turretUid;
			if (!attachedEntity.HasValue || attachedEntity.GetValueOrDefault() != val)
			{
				return;
			}
		}
		if (!TryResolveRotationTarget(turretUid, turret, out EntityUid targetUid, out VehicleTurretComponent targetTurret) || !targetTurret.RotateToCursor || !TryGetTurretOrigin(targetUid, targetTurret, out var originCoords))
		{
			return;
		}
		EntityCoordinates targetCoords = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		MapCoordinates originMap = _transform.ToMapCoordinates(originCoords, true);
		Vector2 direction = _transform.ToMapCoordinates(targetCoords, true).Position - originMap.Position;
		if (!(direction.LengthSquared() <= 0.0001f))
		{
			Angle vehicleRot = _transform.GetWorldRotation(vehicle);
			Angle val3;
			if (!targetTurret.StabilizedRotation)
			{
				Angle val2 = DirectionExtensions.ToWorldAngle(direction) - vehicleRot;
				val3 = ((Angle)(ref val2)).Reduced();
			}
			else
			{
				val3 = DirectionExtensions.ToWorldAngle(direction);
			}
			Angle desiredRotation = val3;
			SetTargetRotation(targetUid, targetTurret, vehicle, desiredRotation, allowReverseDelay: true);
		}
	}

	private void EnsureVisual(EntityUid turretUid, VehicleTurretComponent turret, EntityUid vehicle)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || !turret.ShowOverlay)
		{
			return;
		}
		EntityUid? visualEntity = turret.VisualEntity;
		if (visualEntity.HasValue)
		{
			EntityUid existing = visualEntity.GetValueOrDefault();
			if (((EntitySystem)this).Exists(existing))
			{
				return;
			}
		}
		EntityUid visual = ((EntitySystem)this).Spawn("VehicleTurretVisual", ((EntitySystem)this).Transform(vehicle).Coordinates);
		VehicleTurretVisualComponent visualComp = ((EntitySystem)this).EnsureComp<VehicleTurretVisualComponent>(visual);
		visualComp.Turret = ((EntitySystem)this).GetNetEntity(turretUid, (MetaDataComponent)null);
		((EntitySystem)this).Dirty(visual, (IComponent)(object)visualComp, (MetaDataComponent)null);
		turret.VisualEntity = visual;
	}

	private void CleanupVisual(VehicleTurretComponent turret)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid? visualEntity = turret.VisualEntity;
		if (visualEntity.HasValue)
		{
			EntityUid visual = visualEntity.GetValueOrDefault();
			if (((EntitySystem)this).Exists(visual) && !((EntitySystem)this).TerminatingOrDeleted(visual, (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(visual))
			{
				((EntitySystem)this).Del((EntityUid?)visual);
			}
			turret.VisualEntity = null;
		}
	}

	private void UpdateTurretTransforms(EntityUid turretUid, VehicleTurretComponent turret, EntityUid vehicle, EntityUid anchorUid, VehicleTurretComponent anchorTurret)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		Angle vehicleRot = _transform.GetWorldRotation(vehicle);
		Angle baseFacingAngle = GetVehicleFacingAngle(vehicle, vehicleRot);
		Angle anchorFacingAngle = GetOffsetFacing(anchorTurret, anchorTurret, vehicleRot, baseFacingAngle);
		Angle val = -vehicleRot;
		Vector2 vector = GetPixelOffset(anchorTurret, anchorFacingAngle) / 32f;
		Vector2 anchorLocalOffset = ((Angle)(ref val)).RotateVec(ref vector);
		Angle localRot = Angle.Zero;
		if (anchorTurret.RotateToCursor)
		{
			localRot = anchorTurret.WorldRotation;
		}
		EntityCoordinates turretCoords = default(EntityCoordinates);
		Angle turretLocalRot;
		EntityCoordinates visualCoords = default(EntityCoordinates);
		Angle visualLocalRot;
		if (anchorUid == turretUid)
		{
			((EntityCoordinates)(ref turretCoords))._002Ector(vehicle, anchorLocalOffset);
			turretLocalRot = localRot;
			visualCoords = turretCoords;
			visualLocalRot = localRot;
		}
		else
		{
			Angle turretFacingAngle = GetOffsetFacing(turret, anchorTurret, vehicleRot, baseFacingAngle);
			Vector2 worldOffset = GetPixelOffset(turret, turretFacingAngle) / 32f;
			Vector2 relativeAnchorOffset;
			Vector2 turretLocalOffset;
			if (turret.OffsetRotatesWithTurret)
			{
				if (turret.UseDirectionalOffsets)
				{
					Direction dir = GetDirectionalDir(turretFacingAngle);
					Vector2 directionalOffset = (turret.PixelOffset + GetDirectionalOffset(turret, dir)) / 32f;
					Angle snappedAngle = GetDirectionalAngle(dir);
					val = -snappedAngle;
					relativeAnchorOffset = ((Angle)(ref val)).RotateVec(ref directionalOffset);
					val = localRot - snappedAngle;
					turretLocalOffset = ((Angle)(ref val)).RotateVec(ref directionalOffset);
				}
				else
				{
					relativeAnchorOffset = worldOffset;
					turretLocalOffset = ((Angle)(ref localRot)).RotateVec(ref relativeAnchorOffset);
				}
			}
			else
			{
				val = -vehicleRot;
				turretLocalOffset = ((Angle)(ref val)).RotateVec(ref worldOffset);
				val = -localRot;
				relativeAnchorOffset = ((Angle)(ref val)).RotateVec(ref turretLocalOffset);
			}
			((EntityCoordinates)(ref turretCoords))._002Ector(anchorUid, relativeAnchorOffset);
			turretLocalRot = Angle.Zero;
			((EntityCoordinates)(ref visualCoords))._002Ector(vehicle, anchorLocalOffset + turretLocalOffset);
			visualLocalRot = localRot;
		}
		TransformComponent turretXform = ((EntitySystem)this).Transform(turretUid);
		_transform.SetCoordinates(turretUid, turretXform, turretCoords, (Angle?)null, true, (TransformComponent)null, (TransformComponent)null);
		_transform.SetLocalRotation(turretUid, turretLocalRot, turretXform);
		EntityUid? visualEntity = turret.VisualEntity;
		if (visualEntity.HasValue)
		{
			EntityUid visual = visualEntity.GetValueOrDefault();
			if (((EntitySystem)this).Exists(visual))
			{
				TransformComponent visualXform = ((EntitySystem)this).Transform(visual);
				_transform.SetCoordinates(visual, visualXform, visualCoords, (Angle?)null, true, (TransformComponent)null, (TransformComponent)null);
				_transform.SetLocalRotation(visual, visualLocalRot, visualXform);
			}
		}
	}

	private void TryGetAnchorTurret(EntityUid turretUid, VehicleTurretComponent turret, out EntityUid anchorUid, out VehicleTurretComponent anchorTurret)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		anchorUid = turretUid;
		anchorTurret = turret;
		if (((EntitySystem)this).HasComp<VehicleTurretAttachmentComponent>(turretUid) && TryGetParentTurret(turretUid, out EntityUid parentUid, out VehicleTurretComponent parentTurret))
		{
			anchorUid = parentUid;
			anchorTurret = parentTurret;
		}
	}

	public bool TryGetTurretOrigin(EntityUid turretUid, VehicleTurretComponent turret, out EntityCoordinates origin)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		origin = default(EntityCoordinates);
		if (!TryGetVehicle(turretUid, out var _))
		{
			return false;
		}
		origin = _transform.GetMoverCoordinates(turretUid);
		return true;
	}

	private Vector2 GetPixelOffset(VehicleTurretComponent turret, Angle facing)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!turret.UseDirectionalOffsets)
		{
			return turret.PixelOffset;
		}
		Vector2 pixelOffset = turret.PixelOffset;
		double normalized = facing.Theta % 6.2831854820251465;
		if (normalized < 0.0)
		{
			normalized += 6.2831854820251465;
		}
		Direction dir = GetDirectionalDir((float)normalized);
		return pixelOffset + GetDirectionalOffset(turret, dir);
	}

	private static Vector2 GetDirectionalOffset(VehicleTurretComponent turret, Direction dir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected I4, but got Unknown
		return (int)dir switch
		{
			0 => turret.PixelOffsetSouth, 
			2 => turret.PixelOffsetEast, 
			4 => turret.PixelOffsetNorth, 
			6 => turret.PixelOffsetWest, 
			_ => Vector2.Zero, 
		};
	}

	private static Direction GetDirectionalDir(Angle facing)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(facing);
	}

	private static Direction GetDirectionalDir(float normalized)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return VehicleTurretDirectionHelpers.GetRenderAlignedCardinalDir(new Angle((double)normalized));
	}

	private static Angle GetDirectionalAngle(Direction dir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return DirectionExtensions.ToAngle(dir);
	}

	private Angle GetVehicleFacingAngle(EntityUid vehicle, Angle vehicleRot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		GridVehicleMoverComponent mover = default(GridVehicleMoverComponent);
		if (((EntitySystem)this).TryComp<GridVehicleMoverComponent>(vehicle, ref mover) && mover.CurrentDirection != Vector2i.Zero)
		{
			return DirectionExtensions.ToWorldAngle(new Vector2(mover.CurrentDirection.X, mover.CurrentDirection.Y));
		}
		return vehicleRot;
	}

	private Angle GetOffsetFacing(VehicleTurretComponent turret, VehicleTurretComponent anchorTurret, Angle vehicleRot, Angle baseFacingAngle)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!turret.OffsetRotatesWithTurret)
		{
			return baseFacingAngle;
		}
		Angle val = vehicleRot + anchorTurret.WorldRotation;
		return ((Angle)(ref val)).Reduced();
	}

	private bool TryGetVehicle(EntityUid turretUid, out EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		vehicle = default(EntityUid);
		EntityUid current = turretUid;
		BaseContainer container = default(BaseContainer);
		while (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(current, null)), ref container))
		{
			EntityUid owner = container.Owner;
			if (((EntitySystem)this).HasComp<VehicleComponent>(owner))
			{
				vehicle = owner;
				return true;
			}
			current = owner;
		}
		return false;
	}

	public bool TryResolveRotationTarget(EntityUid turretUid, out EntityUid targetUid, out VehicleTurretComponent targetTurret)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		targetUid = default(EntityUid);
		targetTurret = null;
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		if (!((EntitySystem)this).TryComp<VehicleTurretComponent>(turretUid, ref turret))
		{
			return false;
		}
		return TryResolveRotationTarget(turretUid, turret, out targetUid, out targetTurret);
	}

	private bool TryResolveRotationTarget(EntityUid turretUid, VehicleTurretComponent turret, out EntityUid targetUid, out VehicleTurretComponent targetTurret)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		targetUid = turretUid;
		targetTurret = turret;
		if (!((EntitySystem)this).HasComp<VehicleTurretAttachmentComponent>(turretUid))
		{
			return true;
		}
		if (!TryGetParentTurret(turretUid, out EntityUid parentUid, out VehicleTurretComponent parentTurret))
		{
			return true;
		}
		targetUid = parentUid;
		targetTurret = parentTurret;
		return true;
	}

	private bool TryGetParentTurret(EntityUid turretUid, out EntityUid parentUid, out VehicleTurretComponent parentTurret)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		parentUid = default(EntityUid);
		parentTurret = null;
		EntityUid current = turretUid;
		BaseContainer container = default(BaseContainer);
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		while (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(current, null)), ref container))
		{
			EntityUid owner = container.Owner;
			if (((EntitySystem)this).TryComp<VehicleTurretComponent>(owner, ref turret))
			{
				parentUid = owner;
				parentTurret = turret;
				return true;
			}
			current = owner;
		}
		return false;
	}

	public bool TryAimAtTarget(EntityUid turretUid, EntityUid target, out EntityCoordinates targetCoords)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		targetCoords = default(EntityCoordinates);
		if (!TryResolveRotationTarget(turretUid, out EntityUid targetUid, out VehicleTurretComponent targetTurret))
		{
			return false;
		}
		if (!targetTurret.RotateToCursor)
		{
			return false;
		}
		if (!TryGetVehicle(targetUid, out var vehicle))
		{
			return false;
		}
		if (!TryGetTurretOrigin(targetUid, targetTurret, out var originCoords))
		{
			return false;
		}
		targetCoords = ((EntitySystem)this).Transform(target).Coordinates;
		MapCoordinates originMap = _transform.ToMapCoordinates(originCoords, true);
		Vector2 direction = _transform.ToMapCoordinates(targetCoords, true).Position - originMap.Position;
		if (direction.LengthSquared() <= 0.0001f)
		{
			return false;
		}
		Angle vehicleRot = _transform.GetWorldRotation(vehicle);
		Angle val;
		Angle targetRotation;
		if (!targetTurret.StabilizedRotation)
		{
			val = DirectionExtensions.ToWorldAngle(direction) - vehicleRot;
			targetRotation = ((Angle)(ref val)).Reduced();
		}
		else
		{
			targetRotation = DirectionExtensions.ToWorldAngle(direction);
		}
		Angle desiredRotation = (targetTurret.TargetRotation = targetRotation);
		if (targetTurret.RotationSpeed <= 0f)
		{
			Angle val2;
			if (!targetTurret.StabilizedRotation)
			{
				val2 = desiredRotation;
			}
			else
			{
				val = desiredRotation - vehicleRot;
				val2 = ((Angle)(ref val)).Reduced();
			}
			Angle desiredLocal = val2;
			targetTurret.WorldRotation = desiredLocal;
		}
		((EntitySystem)this).Dirty(targetUid, (IComponent)(object)targetTurret, (MetaDataComponent)null);
		UpdateTurretTransforms(targetUid, targetTurret, vehicle, targetUid, targetTurret);
		return true;
	}

	public bool TrySetTargetRotationWorld(EntityUid turretUid, Angle worldRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (!TryResolveRotationTarget(turretUid, out EntityUid targetUid, out VehicleTurretComponent targetTurret))
		{
			return false;
		}
		if (!TryGetVehicle(targetUid, out var vehicle))
		{
			return false;
		}
		Angle vehicleRot = _transform.GetWorldRotation(vehicle);
		Angle val;
		Angle targetRotation;
		if (!targetTurret.StabilizedRotation)
		{
			val = worldRotation - vehicleRot;
			targetRotation = ((Angle)(ref val)).Reduced();
		}
		else
		{
			targetRotation = worldRotation;
		}
		Angle desiredRotation = (targetTurret.TargetRotation = targetRotation);
		if (targetTurret.RotationSpeed <= 0f)
		{
			Angle val2;
			if (!targetTurret.StabilizedRotation)
			{
				val2 = desiredRotation;
			}
			else
			{
				val = desiredRotation - vehicleRot;
				val2 = ((Angle)(ref val)).Reduced();
			}
			Angle desiredLocal = val2;
			targetTurret.WorldRotation = desiredLocal;
		}
		((EntitySystem)this).Dirty(targetUid, (IComponent)(object)targetTurret, (MetaDataComponent)null);
		UpdateTurretTransforms(targetUid, targetTurret, vehicle, targetUid, targetTurret);
		return true;
	}

	private void InitializeRotation(EntityUid turretUid, VehicleTurretComponent turret, EntityUid vehicle)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		if ((!turret.RotateToCursor && !turret.ShowOverlay) || turret.WorldRotation != Angle.Zero)
		{
			if (turret.TargetRotation == Angle.Zero && turret.WorldRotation != Angle.Zero)
			{
				Angle vehicleRot = _transform.GetWorldRotation(vehicle);
				Angle targetRotation;
				if (!turret.StabilizedRotation)
				{
					targetRotation = turret.WorldRotation;
				}
				else
				{
					Angle val = turret.WorldRotation + vehicleRot;
					targetRotation = ((Angle)(ref val)).Reduced();
				}
				turret.TargetRotation = targetRotation;
				((EntitySystem)this).Dirty(turretUid, (IComponent)(object)turret, (MetaDataComponent)null);
			}
		}
		else
		{
			Angle baseWorld = _transform.GetWorldRotation(vehicle);
			turret.WorldRotation = Angle.Zero;
			turret.TargetRotation = (turret.StabilizedRotation ? baseWorld : Angle.Zero);
			((EntitySystem)this).Dirty(turretUid, (IComponent)(object)turret, (MetaDataComponent)null);
		}
	}

	private void UpdateTurretRotation(EntityUid turretUid, VehicleTurretComponent turret, EntityUid vehicle, float frameTime)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (!turret.RotateToCursor)
		{
			return;
		}
		ApplyPendingTargetRotation(turretUid, turret, vehicle);
		Angle vehicleRot = _transform.GetWorldRotation(vehicle);
		Angle val;
		if (turret.TargetRotation == Angle.Zero && turret.WorldRotation != Angle.Zero)
		{
			Angle targetRotation;
			if (!turret.StabilizedRotation)
			{
				targetRotation = turret.WorldRotation;
			}
			else
			{
				val = turret.WorldRotation + vehicleRot;
				targetRotation = ((Angle)(ref val)).Reduced();
			}
			turret.TargetRotation = targetRotation;
			((EntitySystem)this).Dirty(turretUid, (IComponent)(object)turret, (MetaDataComponent)null);
			return;
		}
		Angle val2;
		if (!turret.StabilizedRotation)
		{
			val2 = turret.TargetRotation;
		}
		else
		{
			val = turret.TargetRotation - vehicleRot;
			val2 = ((Angle)(ref val)).Reduced();
		}
		Angle target = val2;
		if (turret.RotationSpeed <= 0f)
		{
			if (turret.WorldRotation != target)
			{
				turret.WorldRotation = target;
				((EntitySystem)this).Dirty(turretUid, (IComponent)(object)turret, (MetaDataComponent)null);
			}
			return;
		}
		Angle delta = Angle.ShortestDistance(ref turret.WorldRotation, ref target);
		float maxStep = MathHelper.DegreesToRadians(turret.RotationSpeed) * frameTime;
		if (Math.Abs(delta.Theta) <= (double)maxStep)
		{
			if (turret.WorldRotation != target)
			{
				turret.WorldRotation = target;
				((EntitySystem)this).Dirty(turretUid, (IComponent)(object)turret, (MetaDataComponent)null);
			}
			return;
		}
		float step = (float)Math.Sign(delta.Theta) * maxStep;
		val = turret.WorldRotation + Angle.op_Implicit(step);
		Angle next = ((Angle)(ref val)).Reduced();
		if (next != turret.WorldRotation)
		{
			turret.WorldRotation = next;
			((EntitySystem)this).Dirty(turretUid, (IComponent)(object)turret, (MetaDataComponent)null);
		}
	}

	private void OnAttemptShoot(Entity<VehicleTurretComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		if ((_net.IsClient && !_timing.IsFirstTimePredicted) || args.Cancelled)
		{
			return;
		}
		if (!CanOperatorUseTurret(ent.Owner, args.User))
		{
			args.Cancelled = true;
			args.ResetCooldown = true;
		}
		else
		{
			if (!TryResolveRotationTarget(ent.Owner, ent.Comp, out EntityUid targetUid, out VehicleTurretComponent targetTurret) || !targetTurret.RotateToCursor)
			{
				return;
			}
			float alignmentTolerance = MathHelper.DegreesToRadians(MathF.Max(2f + ent.Comp.FireWhileRotatingGraceDegrees, 0f));
			if (!TryGetVehicle(targetUid, out var vehicle))
			{
				return;
			}
			Angle vehicleRot = _transform.GetWorldRotation(vehicle);
			Angle val;
			if (args.ToCoordinates.HasValue && TryGetTurretOrigin(targetUid, targetTurret, out var originCoords))
			{
				MapCoordinates originMap = _transform.ToMapCoordinates(originCoords, true);
				Vector2 direction = _transform.ToMapCoordinates(args.ToCoordinates.Value, true).Position - originMap.Position;
				if (direction.LengthSquared() > 0.0001f)
				{
					Angle desiredWorldRotation = DirectionExtensions.ToWorldAngle(direction);
					val = targetTurret.WorldRotation + vehicleRot;
					Angle currentWorldRotation = ((Angle)(ref val)).Reduced();
					if (Math.Abs(Angle.ShortestDistance(ref currentWorldRotation, ref desiredWorldRotation).Theta) > (double)alignmentTolerance)
					{
						args.Cancelled = true;
						args.ResetCooldown = true;
						return;
					}
				}
			}
			val = targetTurret.WorldRotation + vehicleRot;
			Angle worldRotation = ((Angle)(ref val)).Reduced();
			Angle val2;
			if (!targetTurret.StabilizedRotation)
			{
				val = targetTurret.TargetRotation + vehicleRot;
				val2 = ((Angle)(ref val)).Reduced();
			}
			else
			{
				val2 = targetTurret.TargetRotation;
			}
			Angle targetWorldRotation = val2;
			if (Math.Abs(Angle.ShortestDistance(ref worldRotation, ref targetWorldRotation).Theta) <= (double)alignmentTolerance)
			{
				ApplyShotDirectionConstraint(ent.Comp, targetTurret, targetUid, vehicle, ref args);
				return;
			}
			args.Cancelled = true;
			args.ResetCooldown = true;
		}
	}

	private void ApplyShotDirectionConstraint(VehicleTurretComponent sourceTurret, VehicleTurretComponent rotationTurret, EntityUid rotationTurretUid, EntityUid vehicle, ref AttemptShootEvent args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		float maxCurveDegrees = MathF.Max(0f, sourceTurret.MaxShotCurvatureDegrees);
		if (!sourceTurret.UseBarrelDirectionForShots && maxCurveDegrees <= 0f)
		{
			return;
		}
		MapCoordinates originMap = _transform.ToMapCoordinates(args.FromCoordinates, true);
		EntityCoordinates? toCoordinates = args.ToCoordinates;
		if (!toCoordinates.HasValue)
		{
			return;
		}
		EntityCoordinates currentTarget = toCoordinates.GetValueOrDefault();
		MapCoordinates targetMap = _transform.ToMapCoordinates(currentTarget, true);
		if (targetMap.MapId != originMap.MapId)
		{
			return;
		}
		float distance = (targetMap.Position - originMap.Position).Length();
		if (!(distance <= 0.0001f))
		{
			Angle vehicleRot = _transform.GetWorldRotation(vehicle);
			Angle val = rotationTurret.WorldRotation + vehicleRot;
			Angle barrelWorldRotation = ((Angle)(ref val)).Reduced();
			Angle shotWorldRotation = barrelWorldRotation;
			if (!sourceTurret.UseBarrelDirectionForShots && maxCurveDegrees > 0f)
			{
				Angle desiredWorldRotation = DirectionExtensions.ToWorldAngle(targetMap.Position - originMap.Position);
				float maxCurveRadians = MathHelper.DegreesToRadians(maxCurveDegrees);
				float clamped = MathHelper.Clamp((float)Angle.ShortestDistance(ref barrelWorldRotation, ref desiredWorldRotation).Theta, 0f - maxCurveRadians, maxCurveRadians);
				val = barrelWorldRotation + Angle.op_Implicit(clamped);
				shotWorldRotation = ((Angle)(ref val)).Reduced();
			}
			MapCoordinates adjustedMap = default(MapCoordinates);
			((MapCoordinates)(ref adjustedMap))._002Ector(originMap.Position + ((Angle)(ref shotWorldRotation)).ToWorldVec() * distance, originMap.MapId);
			EntityCoordinates adjustedTarget = _transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(rotationTurretUid), adjustedMap);
			args = args with
			{
				ToCoordinates = adjustedTarget
			};
		}
	}

	private void ApplyPendingTargetRotation(EntityUid turretUid, VehicleTurretComponent turret, EntityUid vehicle)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Angle? pendingTargetRotation = turret.PendingTargetRotation;
		if (pendingTargetRotation.HasValue)
		{
			Angle pending = pendingTargetRotation.GetValueOrDefault();
			if (!(_timing.CurTime < turret.PendingTargetApplyAt))
			{
				turret.PendingTargetRotation = null;
				turret.PendingTargetApplyAt = TimeSpan.Zero;
				int sign = turret.PendingDirectionSign;
				turret.PendingDirectionSign = 0;
				ApplyTargetRotation(turretUid, turret, vehicle, pending, sign);
			}
		}
	}

	private void SetTargetRotation(EntityUid turretUid, VehicleTurretComponent turret, EntityUid vehicle, Angle desiredRotation, bool allowReverseDelay)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Angle delta = Angle.ShortestDistance(ref turret.TargetRotation, ref desiredRotation);
		float deadzone = MathHelper.DegreesToRadians(MathF.Max(0f, turret.RotationInputDeadzoneDegrees));
		if (Math.Abs(delta.Theta) <= (double)deadzone)
		{
			return;
		}
		int directionSign = Math.Sign(delta.Theta);
		if (allowReverseDelay && turret.ReverseDirectionDelay > 0f && directionSign != 0 && turret.LastAppliedDirectionSign != 0 && directionSign != turret.LastAppliedDirectionSign)
		{
			if (!turret.PendingTargetRotation.HasValue || turret.PendingDirectionSign != directionSign)
			{
				turret.PendingTargetApplyAt = _timing.CurTime + TimeSpan.FromSeconds(turret.ReverseDirectionDelay);
			}
			turret.PendingTargetRotation = desiredRotation;
			turret.PendingDirectionSign = directionSign;
		}
		else
		{
			turret.PendingTargetRotation = null;
			turret.PendingTargetApplyAt = TimeSpan.Zero;
			turret.PendingDirectionSign = 0;
			ApplyTargetRotation(turretUid, turret, vehicle, desiredRotation, directionSign);
		}
	}

	private void ApplyTargetRotation(EntityUid turretUid, VehicleTurretComponent turret, EntityUid vehicle, Angle desiredRotation, int directionSign)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		bool changed = false;
		if (turret.TargetRotation != desiredRotation)
		{
			turret.TargetRotation = desiredRotation;
			changed = true;
		}
		if (directionSign != 0)
		{
			turret.LastAppliedDirectionSign = directionSign;
		}
		if (turret.RotationSpeed <= 0f)
		{
			Angle vehicleRot = _transform.GetWorldRotation(vehicle);
			Angle val;
			if (!turret.StabilizedRotation)
			{
				val = desiredRotation;
			}
			else
			{
				Angle val2 = desiredRotation - vehicleRot;
				val = ((Angle)(ref val2)).Reduced();
			}
			Angle desiredLocal = val;
			if (turret.WorldRotation != desiredLocal)
			{
				turret.WorldRotation = desiredLocal;
				changed = true;
			}
		}
		if (changed)
		{
			((EntitySystem)this).Dirty(turretUid, (IComponent)(object)turret, (MetaDataComponent)null);
		}
	}

	private bool CanOperatorUseTurret(EntityUid turretUid, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetVehicle(turretUid, out var vehicle))
		{
			return true;
		}
		VehicleWeaponsOperatorComponent operatorComp = default(VehicleWeaponsOperatorComponent);
		if (((EntitySystem)this).TryComp<VehicleWeaponsOperatorComponent>(user, ref operatorComp))
		{
			EntityUid? vehicle2 = operatorComp.Vehicle;
			EntityUid val = vehicle;
			if (vehicle2.HasValue && !(vehicle2.GetValueOrDefault() != val))
			{
				vehicle2 = operatorComp.SelectedWeapon;
				val = turretUid;
				if (!vehicle2.HasValue || vehicle2.GetValueOrDefault() != val)
				{
					return false;
				}
				VehicleViewToggleComponent viewToggle = default(VehicleViewToggleComponent);
				if (((EntitySystem)this).TryComp<VehicleViewToggleComponent>(user, ref viewToggle) && !viewToggle.IsOutside)
				{
					return false;
				}
				return true;
			}
		}
		return true;
	}

	private static bool ShouldUpdateTransforms(VehicleTurretComponent turret)
	{
		if (turret.RotateToCursor || turret.ShowOverlay || turret.UseDirectionalOffsets)
		{
			return true;
		}
		if (!(turret.PixelOffset != Vector2.Zero) && !(turret.PixelOffsetNorth != Vector2.Zero) && !(turret.PixelOffsetEast != Vector2.Zero) && !(turret.PixelOffsetSouth != Vector2.Zero))
		{
			return turret.PixelOffsetWest != Vector2.Zero;
		}
		return true;
	}

	public void SetOverlayState(EntityUid uid, string state, VehicleTurretComponent? turret = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<VehicleTurretComponent>(uid, ref turret, false) && !(turret.OverlayState == state))
		{
			turret.OverlayState = state;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)turret, (MetaDataComponent)null);
			if (TryGetVehicle(uid, out var vehicle))
			{
				HardpointSlotsChangedEvent ev = new HardpointSlotsChangedEvent(vehicle);
				((EntitySystem)this).RaiseLocalEvent<HardpointSlotsChangedEvent>(vehicle, ev, true);
			}
		}
	}
}
