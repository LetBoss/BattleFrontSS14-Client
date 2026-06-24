using System;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Flash.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(DamagedByFlashingSystem) })]
public sealed class DamagedByFlashingComponent : Component, ISerializationGenerated<DamagedByFlashingComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public DamageSpecifier FlashDamage = new DamageSpecifier();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamagedByFlashingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DamagedByFlashingComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<DamagedByFlashingComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		DamageSpecifier FlashDamageTemp = null;
		if (FlashDamage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageSpecifier>(FlashDamage, ref FlashDamageTemp, hookCtx, false, context))
		{
			if (FlashDamage == null)
			{
				FlashDamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(FlashDamage, ref FlashDamageTemp, hookCtx, context, true);
			}
		}
		target.FlashDamage = FlashDamageTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamagedByFlashingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamagedByFlashingComponent cast = (DamagedByFlashingComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamagedByFlashingComponent cast = (DamagedByFlashingComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamagedByFlashingComponent def = (DamagedByFlashingComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DamagedByFlashingComponent Instantiate()
	{
		return new DamagedByFlashingComponent();
	}
}
