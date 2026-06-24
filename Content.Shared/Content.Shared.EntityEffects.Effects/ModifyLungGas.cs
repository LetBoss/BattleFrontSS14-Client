using System;
using System.Collections.Generic;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ModifyLungGas : EventEntityEffect<ModifyLungGas>, ISerializationGenerated<ModifyLungGas>, ISerializationGenerated
{
	[DataField("ratios", false, 1, true, false, null)]
	public Dictionary<Gas, float> Ratios;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return null;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ModifyLungGas target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EventEntityEffect<ModifyLungGas> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ModifyLungGas)definitionCast;
		if (!serialization.TryCustomCopy<ModifyLungGas>(this, ref target, hookCtx, false, context))
		{
			Dictionary<Gas, float> RatiosTemp = null;
			if (Ratios == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<Gas, float>>(Ratios, ref RatiosTemp, hookCtx, true, context))
			{
				RatiosTemp = serialization.CreateCopy<Dictionary<Gas, float>>(Ratios, hookCtx, context, false);
			}
			target.Ratios = RatiosTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ModifyLungGas target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<ModifyLungGas> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ModifyLungGas cast = (ModifyLungGas)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ModifyLungGas cast = (ModifyLungGas)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ModifyLungGas Instantiate()
	{
		return new ModifyLungGas();
	}
}
