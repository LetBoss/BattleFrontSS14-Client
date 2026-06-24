using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[Access(new Type[] { typeof(RMCVehicleDamageProfileSystem) })]
public sealed class RMCVehicleDamageProfileComponent : Component, ISerializationGenerated<RMCVehicleDamageProfileComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<RMCVehicleDamageScaleRule> Rules = new List<RMCVehicleDamageScaleRule>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCVehicleDamageProfileComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCVehicleDamageProfileComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCVehicleDamageProfileComponent>(this, ref target, hookCtx, false, context))
		{
			List<RMCVehicleDamageScaleRule> RulesTemp = null;
			if (Rules == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<RMCVehicleDamageScaleRule>>(Rules, ref RulesTemp, hookCtx, true, context))
			{
				RulesTemp = serialization.CreateCopy<List<RMCVehicleDamageScaleRule>>(Rules, hookCtx, context, false);
			}
			target.Rules = RulesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCVehicleDamageProfileComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleDamageProfileComponent cast = (RMCVehicleDamageProfileComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleDamageProfileComponent cast = (RMCVehicleDamageProfileComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCVehicleDamageProfileComponent def = (RMCVehicleDamageProfileComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCVehicleDamageProfileComponent Instantiate()
	{
		return new RMCVehicleDamageProfileComponent();
	}
}
