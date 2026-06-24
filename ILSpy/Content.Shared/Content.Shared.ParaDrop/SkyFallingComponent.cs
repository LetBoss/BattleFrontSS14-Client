using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.ParaDrop;

[RegisterComponent]
[NetworkedComponent]
public sealed class SkyFallingComponent : Component, ISerializationGenerated<SkyFallingComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float RemainingTime = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 OriginalScale;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 AnimationScale = new Vector2(0.01f, 0.01f);

	[DataField(null, false, 1, false, false, null)]
	public EntityCoordinates? TargetCoordinates;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SkyFallingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SkyFallingComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SkyFallingComponent>(this, ref target, hookCtx, false, context))
		{
			float RemainingTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RemainingTime, ref RemainingTimeTemp, hookCtx, false, context))
			{
				RemainingTimeTemp = RemainingTime;
			}
			target.RemainingTime = RemainingTimeTemp;
			Vector2 OriginalScaleTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(OriginalScale, ref OriginalScaleTemp, hookCtx, false, context))
			{
				OriginalScaleTemp = serialization.CreateCopy<Vector2>(OriginalScale, hookCtx, context, false);
			}
			target.OriginalScale = OriginalScaleTemp;
			Vector2 AnimationScaleTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(AnimationScale, ref AnimationScaleTemp, hookCtx, false, context))
			{
				AnimationScaleTemp = serialization.CreateCopy<Vector2>(AnimationScale, hookCtx, context, false);
			}
			target.AnimationScale = AnimationScaleTemp;
			EntityCoordinates? TargetCoordinatesTemp = null;
			if (!serialization.TryCustomCopy<EntityCoordinates?>(TargetCoordinates, ref TargetCoordinatesTemp, hookCtx, false, context))
			{
				TargetCoordinatesTemp = serialization.CreateCopy<EntityCoordinates?>(TargetCoordinates, hookCtx, context, false);
			}
			target.TargetCoordinates = TargetCoordinatesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SkyFallingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SkyFallingComponent cast = (SkyFallingComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SkyFallingComponent cast = (SkyFallingComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SkyFallingComponent def = (SkyFallingComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SkyFallingComponent Instantiate()
	{
		return new SkyFallingComponent();
	}
}
