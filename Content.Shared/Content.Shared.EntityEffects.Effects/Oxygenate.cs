using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class Oxygenate : EventEntityEffect<Oxygenate>, ISerializationGenerated<Oxygenate>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Factor = 1f;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return null;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Oxygenate target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<Oxygenate> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Oxygenate)definitionCast;
		if (!serialization.TryCustomCopy<Oxygenate>(this, ref target, hookCtx, false, context))
		{
			float FactorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Factor, ref FactorTemp, hookCtx, false, context))
			{
				FactorTemp = Factor;
			}
			target.Factor = FactorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Oxygenate target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<Oxygenate> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Oxygenate cast = (Oxygenate)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Oxygenate cast = (Oxygenate)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Oxygenate Instantiate()
	{
		return new Oxygenate();
	}
}
