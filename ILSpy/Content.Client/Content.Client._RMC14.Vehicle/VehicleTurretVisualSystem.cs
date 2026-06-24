using System;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleTurretVisualSystem : EntitySystem
{
	private const float PixelsPerMeter = 32f;

	[Dependency]
	private readonly SharedContainerSystem _container;

	[Dependency]
	private readonly IEyeManager _eye;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).UpdatesAfter.Add(typeof(VehicleTurretSystem));
		((EntitySystem)this).SubscribeLocalEvent<VehicleTurretVisualComponent, ComponentInit>((EntityEventRefHandler<VehicleTurretVisualComponent, ComponentInit>)OnVisualInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleTurretVisualComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<VehicleTurretVisualComponent, AfterAutoHandleStateEvent>)OnVisualState, (Type[])null, (Type[])null);
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<VehicleTurretVisualComponent> val = ((EntitySystem)this).EntityQueryEnumerator<VehicleTurretVisualComponent>();
		EntityUid val2 = default(EntityUid);
		VehicleTurretVisualComponent vehicleTurretVisualComponent = default(VehicleTurretVisualComponent);
		EntityUid? val3 = default(EntityUid?);
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		while (val.MoveNext(ref val2, ref vehicleTurretVisualComponent))
		{
			if (((EntitySystem)this).TryGetEntity(vehicleTurretVisualComponent.Turret, ref val3) && ((EntitySystem)this).TryComp<VehicleTurretComponent>(val3, ref turret) && TryComputeRenderedTransform(val3.Value, turret, out var _, out var _, out var localOffset, out var localRotation))
			{
				TransformComponent val4 = ((EntitySystem)this).Transform(val2);
				val4.ActivelyLerping = false;
				_transform.SetLocalRotationNoLerp(val2, localRotation, val4);
				_transform.SetLocalPositionNoLerp(val2, localOffset, val4);
			}
		}
	}

	public bool TryGetRenderedPose(EntityUid turretUid, out EntityCoordinates origin, out Angle worldRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		origin = default(EntityCoordinates);
		worldRotation = Angle.Zero;
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		if (!((EntitySystem)this).TryComp<VehicleTurretComponent>(turretUid, ref turret))
		{
			return false;
		}
		if (!TryComputeRenderedTransform(turretUid, turret, out var vehicle, out var vehicleRot, out var localOffset, out var localRotation))
		{
			return false;
		}
		origin = _transform.GetMoverCoordinates(new EntityCoordinates(vehicle, localOffset));
		Angle val = vehicleRot + localRotation;
		worldRotation = ((Angle)(ref val)).Reduced();
		return true;
	}

	private void OnVisualInit(Entity<VehicleTurretVisualComponent> ent, ref ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisual(ent);
	}

	private void OnVisualState(Entity<VehicleTurretVisualComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisual(ent);
	}

	private void UpdateVisual(Entity<VehicleTurretVisualComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		EntityUid? val2 = default(EntityUid?);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(ent.Owner, ref val) || !((EntitySystem)this).TryGetEntity(ent.Comp.Turret, ref val2))
		{
			return;
		}
		VehicleTurretComponent vehicleTurretComponent = default(VehicleTurretComponent);
		SpriteComponent val3 = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<VehicleTurretComponent>(val2, ref vehicleTurretComponent) && !string.IsNullOrWhiteSpace(vehicleTurretComponent.OverlayState))
		{
			SetOverlayDepth(val2.Value, val);
			string overlayState = vehicleTurretComponent.OverlayState;
			if (!string.IsNullOrWhiteSpace(vehicleTurretComponent.OverlayRsi))
			{
				val.LayerSetState(0, StateId.op_Implicit(overlayState), vehicleTurretComponent.OverlayRsi);
			}
			else
			{
				val.LayerSetState(0, StateId.op_Implicit(overlayState));
			}
			val.LayerSetVisible(0, true);
		}
		else if (((EntitySystem)this).TryComp<SpriteComponent>(val2, ref val3) && val3.BaseRSI != null && val3.AllLayers.Any())
		{
			SetOverlayDepth(val2.Value, val);
			string text = ((object)val3.LayerGetState(0)/*cast due to constrained. prefix*/).ToString();
			val.LayerSetRSI(0, val3.BaseRSI);
			val.LayerSetState(0, StateId.op_Implicit(text));
			val.LayerSetVisible(0, true);
		}
	}

	private void SetOverlayDepth(EntityUid turretUid, SpriteComponent sprite)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		int num = 7;
		if (((EntitySystem)this).HasComp<VehicleTurretAttachmentComponent>(turretUid))
		{
			num++;
		}
		if (sprite.DrawDepth != num)
		{
			sprite.DrawDepth = num;
		}
	}

	private bool TryComputeRenderedTransform(EntityUid turretUid, VehicleTurretComponent turret, out EntityUid vehicle, out Angle vehicleRot, out Vector2 localOffset, out Angle localRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		vehicle = default(EntityUid);
		vehicleRot = Angle.Zero;
		localOffset = Vector2.Zero;
		localRotation = Angle.Zero;
		if (!TryGetVehicle(turretUid, out vehicle))
		{
			return false;
		}
		TryGetAnchorTurret(turretUid, turret, out EntityUid anchorUid, out VehicleTurretComponent anchorTurret);
		vehicleRot = _transform.GetWorldRotation(vehicle);
		Angle rotation = _eye.CurrentEye.Rotation;
		Angle vehicleFacingAngle = GetVehicleFacingAngle(vehicle, vehicleRot);
		Angle renderFacing = GetRenderFacing(anchorTurret, anchorTurret, vehicleRot, vehicleFacingAngle, rotation);
		Vector2 offset = GetPixelOffset(anchorTurret, renderFacing) / 32f;
		Vector2 vehicleLocalOffset = GetVehicleLocalOffset(anchorTurret, offset, vehicleRot, rotation);
		Angle val = (anchorTurret.RotateToCursor ? anchorTurret.WorldRotation : Angle.Zero);
		localOffset = vehicleLocalOffset;
		localRotation = val;
		if (anchorUid == turretUid)
		{
			return true;
		}
		Angle renderFacing2 = GetRenderFacing(turret, anchorTurret, vehicleRot, vehicleFacingAngle, rotation);
		Vector2 offset2 = GetPixelOffset(turret, renderFacing2) / 32f;
		Vector2 vector;
		if (turret.OffsetRotatesWithTurret)
		{
			if (turret.UseDirectionalOffsets)
			{
				Angle directionalAngle = GetDirectionalAngle(GetDirectionalDir(renderFacing2));
				Angle val2 = val - directionalAngle;
				vector = ((Angle)(ref val2)).RotateVec(ref offset2);
			}
			else
			{
				vector = ((Angle)(ref val)).RotateVec(ref offset2);
			}
		}
		else
		{
			vector = GetVehicleLocalOffset(turret, offset2, vehicleRot, rotation);
		}
		localOffset += vector;
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
		double num = facing.Theta % 6.2831854820251465;
		if (num < 0.0)
		{
			num += 6.2831854820251465;
		}
		Direction directionalDir = GetDirectionalDir((float)num);
		return pixelOffset + GetDirectionalOffset(turret, directionalDir);
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
		EntityUid item = turretUid;
		BaseContainer val = default(BaseContainer);
		while (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(item, null)), ref val))
		{
			EntityUid owner = val.Owner;
			if (((EntitySystem)this).HasComp<VehicleComponent>(owner))
			{
				vehicle = owner;
				return true;
			}
			item = owner;
		}
		return false;
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
		EntityUid item = turretUid;
		BaseContainer val = default(BaseContainer);
		VehicleTurretComponent vehicleTurretComponent = default(VehicleTurretComponent);
		while (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(item, null)), ref val))
		{
			EntityUid owner = val.Owner;
			if (((EntitySystem)this).TryComp<VehicleTurretComponent>(owner, ref vehicleTurretComponent))
			{
				parentUid = owner;
				parentTurret = vehicleTurretComponent;
				return true;
			}
			item = owner;
		}
		return false;
	}

	private Angle GetRenderFacing(VehicleTurretComponent turret, VehicleTurretComponent anchorTurret, Angle vehicleRot, Angle baseFacingAngle, Angle eyeRot)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Angle val = GetOffsetFacing(turret, anchorTurret, vehicleRot, baseFacingAngle) + eyeRot;
		return ((Angle)(ref val)).Reduced();
	}

	private static Vector2 GetVehicleLocalOffset(VehicleTurretComponent turret, Vector2 offset, Angle vehicleRot, Angle eyeRot)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Angle val;
		if (turret.UseDirectionalOffsets)
		{
			val = -eyeRot;
			offset = ((Angle)(ref val)).RotateVec(ref offset);
		}
		val = -vehicleRot;
		return ((Angle)(ref val)).RotateVec(ref offset);
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

	private Angle GetVehicleFacingAngle(EntityUid vehicle, Angle vehicleRot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		GridVehicleMoverComponent gridVehicleMoverComponent = default(GridVehicleMoverComponent);
		if (((EntitySystem)this).TryComp<GridVehicleMoverComponent>(vehicle, ref gridVehicleMoverComponent) && gridVehicleMoverComponent.CurrentDirection != Vector2i.Zero)
		{
			return DirectionExtensions.ToWorldAngle(new Vector2(gridVehicleMoverComponent.CurrentDirection.X, gridVehicleMoverComponent.CurrentDirection.Y));
		}
		return vehicleRot;
	}
}
