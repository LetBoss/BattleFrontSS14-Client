using System;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Shuttles.Systems;

public abstract class SharedDockingSystem : EntitySystem
{
	[Dependency]
	protected SharedTransformSystem XformSystem;

	public const float DockingHiglightRange = 4f;

	public const float DockRange = 1.2f;

	public static readonly double AlignmentTolerance = Angle.FromDegrees(15.0).Theta;

	public bool CanShuttleDock(EntityUid? shuttle)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!shuttle.HasValue)
		{
			return false;
		}
		return !((EntitySystem)this).HasComp<PreventPilotComponent>(shuttle.Value);
	}

	public bool CanShuttleUndock(EntityUid? shuttle)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!shuttle.HasValue)
		{
			return false;
		}
		return !((EntitySystem)this).HasComp<PreventPilotComponent>(shuttle.Value);
	}

	public bool CanDock(MapCoordinates mapPosA, Angle worldRotA, MapCoordinates mapPosB, Angle worldRotB)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (mapPosA.MapId != mapPosB.MapId)
		{
			return false;
		}
		if (InRange(mapPosA, mapPosB))
		{
			return InAlignment(mapPosA, worldRotA, mapPosB, worldRotB);
		}
		return false;
	}

	public bool InRange(MapCoordinates mapPosA, MapCoordinates mapPosB)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return (mapPosA.Position - mapPosB.Position).Length() <= 1.2f;
	}

	public bool InAlignment(MapCoordinates mapPosA, Angle worldRotA, MapCoordinates mapPosB, Angle worldRotB)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		Angle worldRotToB = DirectionExtensions.ToWorldAngle(mapPosB.Position - mapPosA.Position);
		Angle worldRotToA = DirectionExtensions.ToWorldAngle(mapPosA.Position - mapPosB.Position);
		Angle val = worldRotA - worldRotToB;
		val = ((Angle)(ref val)).Reduced();
		Angle zero = Angle.Zero;
		Angle val2 = Angle.ShortestDistance(ref val, ref zero);
		val = worldRotB - worldRotToA;
		val = ((Angle)(ref val)).Reduced();
		zero = Angle.Zero;
		Angle bDiff = Angle.ShortestDistance(ref val, ref zero);
		if (Math.Abs(val2.Theta) > AlignmentTolerance)
		{
			return false;
		}
		if (Math.Abs(bDiff.Theta) > AlignmentTolerance)
		{
			return false;
		}
		return true;
	}

	public bool CanDock(NetCoordinates coordinatesOne, Angle angleOne, NetCoordinates coordinatesTwo, Angle angleTwo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordsA = ((EntitySystem)this).GetCoordinates(coordinatesOne);
		EntityCoordinates coordsB = ((EntitySystem)this).GetCoordinates(coordinatesTwo);
		MapCoordinates mapPosA = XformSystem.ToMapCoordinates(coordsA, true);
		MapCoordinates mapPosB = XformSystem.ToMapCoordinates(coordsB, true);
		Angle worldRotA = XformSystem.GetWorldRotation(coordsA.EntityId) + angleOne;
		Angle worldRotB = XformSystem.GetWorldRotation(coordsB.EntityId) + angleTwo;
		return CanDock(mapPosA, worldRotA, mapPosB, worldRotB);
	}
}
