using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Stamina;

[RegisterComponent]
[NetworkedComponent]
public sealed class RMCStaminaDamageOnCollideComponent : Component, ISerializationGenerated<RMCStaminaDamageOnCollideComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public double Damage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCStaminaDamageOnCollideComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCStaminaDamageOnCollideComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCStaminaDamageOnCollideComponent>(this, ref target, hookCtx, false, context))
		{
			double DamageTemp = 0.0;
			if (!serialization.TryCustomCopy<double>(Damage, ref DamageTemp, hookCtx, false, context))
			{
				DamageTemp = Damage;
			}
			target.Damage = DamageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCStaminaDamageOnCollideComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCStaminaDamageOnCollideComponent cast = (RMCStaminaDamageOnCollideComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCStaminaDamageOnCollideComponent cast = (RMCStaminaDamageOnCollideComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCStaminaDamageOnCollideComponent def = (RMCStaminaDamageOnCollideComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCStaminaDamageOnCollideComponent Instantiate()
	{
		return new RMCStaminaDamageOnCollideComponent();
	}
}
