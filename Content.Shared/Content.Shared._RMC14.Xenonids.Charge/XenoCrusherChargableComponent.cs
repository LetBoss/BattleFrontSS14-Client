using System;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Charge;

[RegisterComponent]
[NetworkedComponent]
public sealed class XenoCrusherChargableComponent : Component, ISerializationGenerated<XenoCrusherChargableComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool InstantDestroy;

	[DataField(null, false, 1, false, false, null)]
	public bool PassOnDestroy;

	[DataField(null, false, 1, false, false, null)]
	public DamageSpecifier? SetDamage;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? DestroyDamage;

	[DataField(null, false, 1, false, false, null)]
	public float? ThrowRange;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoCrusherChargableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoCrusherChargableComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<XenoCrusherChargableComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		bool InstantDestroyTemp = false;
		if (!serialization.TryCustomCopy<bool>(InstantDestroy, ref InstantDestroyTemp, hookCtx, false, context))
		{
			InstantDestroyTemp = InstantDestroy;
		}
		target.InstantDestroy = InstantDestroyTemp;
		bool PassOnDestroyTemp = false;
		if (!serialization.TryCustomCopy<bool>(PassOnDestroy, ref PassOnDestroyTemp, hookCtx, false, context))
		{
			PassOnDestroyTemp = PassOnDestroy;
		}
		target.PassOnDestroy = PassOnDestroyTemp;
		DamageSpecifier SetDamageTemp = null;
		if (!serialization.TryCustomCopy<DamageSpecifier>(SetDamage, ref SetDamageTemp, hookCtx, false, context))
		{
			if (SetDamage == null)
			{
				SetDamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(SetDamage, ref SetDamageTemp, hookCtx, context, false);
			}
		}
		target.SetDamage = SetDamageTemp;
		FixedPoint2? DestroyDamageTemp = null;
		if (!serialization.TryCustomCopy<FixedPoint2?>(DestroyDamage, ref DestroyDamageTemp, hookCtx, false, context))
		{
			DestroyDamageTemp = serialization.CreateCopy<FixedPoint2?>(DestroyDamage, hookCtx, context, false);
		}
		target.DestroyDamage = DestroyDamageTemp;
		float? ThrowRangeTemp = null;
		if (!serialization.TryCustomCopy<float?>(ThrowRange, ref ThrowRangeTemp, hookCtx, false, context))
		{
			ThrowRangeTemp = ThrowRange;
		}
		target.ThrowRange = ThrowRangeTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoCrusherChargableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoCrusherChargableComponent cast = (XenoCrusherChargableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoCrusherChargableComponent cast = (XenoCrusherChargableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoCrusherChargableComponent def = (XenoCrusherChargableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoCrusherChargableComponent Instantiate()
	{
		return new XenoCrusherChargableComponent();
	}
}
