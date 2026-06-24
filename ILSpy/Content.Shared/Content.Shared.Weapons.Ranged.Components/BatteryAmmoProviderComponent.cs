using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components;

public abstract class BatteryAmmoProviderComponent : AmmoProviderComponent, ISerializationGenerated<BatteryAmmoProviderComponent>, ISerializationGenerated
{
	[DataField("fireCost", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float FireCost = 100f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int Shots;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int Capacity;

	public BatteryAmmoProviderComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref BatteryAmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmmoProviderComponent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BatteryAmmoProviderComponent)definitionCast;
		if (!serialization.TryCustomCopy<BatteryAmmoProviderComponent>(this, ref target, hookCtx, false, context))
		{
			float FireCostTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FireCost, ref FireCostTemp, hookCtx, false, context))
			{
				FireCostTemp = FireCost;
			}
			target.FireCost = FireCostTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref BatteryAmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref AmmoProviderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BatteryAmmoProviderComponent cast = (BatteryAmmoProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BatteryAmmoProviderComponent cast = (BatteryAmmoProviderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BatteryAmmoProviderComponent def = (BatteryAmmoProviderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BatteryAmmoProviderComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
