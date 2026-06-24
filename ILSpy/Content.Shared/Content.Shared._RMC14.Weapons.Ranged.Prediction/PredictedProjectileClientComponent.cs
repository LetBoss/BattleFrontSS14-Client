using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Weapons.Ranged.Prediction;

[RegisterComponent]
public sealed class PredictedProjectileClientComponent : Component, ISerializationGenerated<PredictedProjectileClientComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Hit;

	[DataField(null, false, 1, false, false, null)]
	public EntityCoordinates? Coordinates;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PredictedProjectileClientComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PredictedProjectileClientComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PredictedProjectileClientComponent>(this, ref target, hookCtx, false, context))
		{
			bool HitTemp = false;
			if (!serialization.TryCustomCopy<bool>(Hit, ref HitTemp, hookCtx, false, context))
			{
				HitTemp = Hit;
			}
			target.Hit = HitTemp;
			EntityCoordinates? CoordinatesTemp = null;
			if (!serialization.TryCustomCopy<EntityCoordinates?>(Coordinates, ref CoordinatesTemp, hookCtx, false, context))
			{
				CoordinatesTemp = serialization.CreateCopy<EntityCoordinates?>(Coordinates, hookCtx, context, false);
			}
			target.Coordinates = CoordinatesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PredictedProjectileClientComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PredictedProjectileClientComponent cast = (PredictedProjectileClientComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PredictedProjectileClientComponent cast = (PredictedProjectileClientComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PredictedProjectileClientComponent def = (PredictedProjectileClientComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PredictedProjectileClientComponent Instantiate()
	{
		return new PredictedProjectileClientComponent();
	}
}
