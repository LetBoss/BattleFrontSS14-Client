using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Projectiles;

[RegisterComponent]
public sealed class RMCAreaDamageComponent : Component, ISerializationGenerated<RMCAreaDamageComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float DamageArea;

	[DataField(null, false, 1, false, false, null)]
	public float FalloffDistance = 0.5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCAreaDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCAreaDamageComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCAreaDamageComponent>(this, ref target, hookCtx, false, context))
		{
			float DamageAreaTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DamageArea, ref DamageAreaTemp, hookCtx, false, context))
			{
				DamageAreaTemp = DamageArea;
			}
			target.DamageArea = DamageAreaTemp;
			float FalloffDistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FalloffDistance, ref FalloffDistanceTemp, hookCtx, false, context))
			{
				FalloffDistanceTemp = FalloffDistance;
			}
			target.FalloffDistance = FalloffDistanceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCAreaDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCAreaDamageComponent cast = (RMCAreaDamageComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCAreaDamageComponent cast = (RMCAreaDamageComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCAreaDamageComponent def = (RMCAreaDamageComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCAreaDamageComponent Instantiate()
	{
		return new RMCAreaDamageComponent();
	}
}
