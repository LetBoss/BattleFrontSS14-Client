using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class RMCVehicleInteriorAudioRelayComponent : Component, ISerializationGenerated<RMCVehicleInteriorAudioRelayComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float ExteriorRange = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float InteriorVolumeOffset = -3f;

	[DataField(null, false, 1, false, false, null)]
	public float InteriorMaxDistance = 18f;

	[DataField(null, false, 1, false, false, null)]
	public float InteriorReferenceDistance = 4f;

	[DataField(null, false, 1, false, false, null)]
	public bool InteriorNoOcclusion = true;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 InsideOffset = Vector2.Zero;

	[DataField(null, false, 1, false, false, null)]
	public float InsideScale = 0.35f;

	[DataField(null, false, 1, false, false, null)]
	public float InsideClamp = 2f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCVehicleInteriorAudioRelayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCVehicleInteriorAudioRelayComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCVehicleInteriorAudioRelayComponent>(this, ref target, hookCtx, false, context))
		{
			float ExteriorRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ExteriorRange, ref ExteriorRangeTemp, hookCtx, false, context))
			{
				ExteriorRangeTemp = ExteriorRange;
			}
			target.ExteriorRange = ExteriorRangeTemp;
			float InteriorVolumeOffsetTemp = 0f;
			if (!serialization.TryCustomCopy<float>(InteriorVolumeOffset, ref InteriorVolumeOffsetTemp, hookCtx, false, context))
			{
				InteriorVolumeOffsetTemp = InteriorVolumeOffset;
			}
			target.InteriorVolumeOffset = InteriorVolumeOffsetTemp;
			float InteriorMaxDistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(InteriorMaxDistance, ref InteriorMaxDistanceTemp, hookCtx, false, context))
			{
				InteriorMaxDistanceTemp = InteriorMaxDistance;
			}
			target.InteriorMaxDistance = InteriorMaxDistanceTemp;
			float InteriorReferenceDistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(InteriorReferenceDistance, ref InteriorReferenceDistanceTemp, hookCtx, false, context))
			{
				InteriorReferenceDistanceTemp = InteriorReferenceDistance;
			}
			target.InteriorReferenceDistance = InteriorReferenceDistanceTemp;
			bool InteriorNoOcclusionTemp = false;
			if (!serialization.TryCustomCopy<bool>(InteriorNoOcclusion, ref InteriorNoOcclusionTemp, hookCtx, false, context))
			{
				InteriorNoOcclusionTemp = InteriorNoOcclusion;
			}
			target.InteriorNoOcclusion = InteriorNoOcclusionTemp;
			Vector2 InsideOffsetTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(InsideOffset, ref InsideOffsetTemp, hookCtx, false, context))
			{
				InsideOffsetTemp = serialization.CreateCopy<Vector2>(InsideOffset, hookCtx, context, false);
			}
			target.InsideOffset = InsideOffsetTemp;
			float InsideScaleTemp = 0f;
			if (!serialization.TryCustomCopy<float>(InsideScale, ref InsideScaleTemp, hookCtx, false, context))
			{
				InsideScaleTemp = InsideScale;
			}
			target.InsideScale = InsideScaleTemp;
			float InsideClampTemp = 0f;
			if (!serialization.TryCustomCopy<float>(InsideClamp, ref InsideClampTemp, hookCtx, false, context))
			{
				InsideClampTemp = InsideClamp;
			}
			target.InsideClamp = InsideClampTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCVehicleInteriorAudioRelayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleInteriorAudioRelayComponent cast = (RMCVehicleInteriorAudioRelayComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleInteriorAudioRelayComponent cast = (RMCVehicleInteriorAudioRelayComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleInteriorAudioRelayComponent def = (RMCVehicleInteriorAudioRelayComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCVehicleInteriorAudioRelayComponent Instantiate()
	{
		return new RMCVehicleInteriorAudioRelayComponent();
	}
}
