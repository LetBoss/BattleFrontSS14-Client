using System;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Climbing.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class BonkableComponent : Component, ISerializationGenerated<BonkableComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan BonkTime = TimeSpan.FromSeconds(2L);

	[DataField(null, false, 1, false, false, null)]
	public DamageSpecifier? BonkDamage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BonkableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BonkableComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<BonkableComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		TimeSpan BonkTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(BonkTime, ref BonkTimeTemp, hookCtx, false, context))
		{
			BonkTimeTemp = serialization.CreateCopy<TimeSpan>(BonkTime, hookCtx, context, false);
		}
		target.BonkTime = BonkTimeTemp;
		DamageSpecifier BonkDamageTemp = null;
		if (!serialization.TryCustomCopy<DamageSpecifier>(BonkDamage, ref BonkDamageTemp, hookCtx, false, context))
		{
			if (BonkDamage == null)
			{
				BonkDamageTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageSpecifier>(BonkDamage, ref BonkDamageTemp, hookCtx, context, false);
			}
		}
		target.BonkDamage = BonkDamageTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BonkableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BonkableComponent cast = (BonkableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BonkableComponent cast = (BonkableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BonkableComponent def = (BonkableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BonkableComponent Instantiate()
	{
		return new BonkableComponent();
	}
}
