using System;
using System.Numerics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[DataDefinition]
public sealed class VehicleEntryPoint : ISerializationGenerated<VehicleEntryPoint>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public Vector2 Offset;

	[DataField(null, false, 1, false, false, null)]
	public float Radius = 0.6f;

	[DataField(null, false, 1, false, false, null)]
	public Vector2? InteriorCoords;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleEntryPoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<VehicleEntryPoint>(this, ref target, hookCtx, false, context))
		{
			Vector2 OffsetTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Offset, ref OffsetTemp, hookCtx, false, context))
			{
				OffsetTemp = serialization.CreateCopy<Vector2>(Offset, hookCtx, context, false);
			}
			target.Offset = OffsetTemp;
			float RadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Radius, ref RadiusTemp, hookCtx, false, context))
			{
				RadiusTemp = Radius;
			}
			target.Radius = RadiusTemp;
			Vector2? InteriorCoordsTemp = null;
			if (!serialization.TryCustomCopy<Vector2?>(InteriorCoords, ref InteriorCoordsTemp, hookCtx, false, context))
			{
				InteriorCoordsTemp = serialization.CreateCopy<Vector2?>(InteriorCoords, hookCtx, context, false);
			}
			target.InteriorCoords = InteriorCoordsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleEntryPoint target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleEntryPoint cast = (VehicleEntryPoint)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public VehicleEntryPoint Instantiate()
	{
		return new VehicleEntryPoint();
	}
}
