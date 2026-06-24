using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Egg;

[RegisterComponent]
[NetworkedComponent]
public sealed class EggPlantingDistanceComponent : Component, ISerializationGenerated<EggPlantingDistanceComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Distance = 1.5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EggPlantingDistanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EggPlantingDistanceComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<EggPlantingDistanceComponent>(this, ref target, hookCtx, false, context))
		{
			float DistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Distance, ref DistanceTemp, hookCtx, false, context))
			{
				DistanceTemp = Distance;
			}
			target.Distance = DistanceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EggPlantingDistanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EggPlantingDistanceComponent cast = (EggPlantingDistanceComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EggPlantingDistanceComponent cast = (EggPlantingDistanceComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EggPlantingDistanceComponent def = (EggPlantingDistanceComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EggPlantingDistanceComponent Instantiate()
	{
		return new EggPlantingDistanceComponent();
	}
}
