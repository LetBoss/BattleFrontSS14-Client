using System;
using Content.Shared.Body.Components;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class Internals : EntityEffectCondition, ISerializationGenerated<Internals>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool UsingInternals = true;

	public override bool Condition(EntityEffectBaseArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		InternalsComponent internalsComp = default(InternalsComponent);
		if (!args.EntityManager.TryGetComponent<InternalsComponent>(args.TargetEntity, ref internalsComp))
		{
			return !UsingInternals;
		}
		bool internalsState = internalsComp.GasTankEntity.HasValue;
		return UsingInternals == internalsState;
	}

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		return Loc.GetString("reagent-effect-condition-guidebook-internals", new(string, object)[1] { ("usingInternals", UsingInternals) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Internals target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffectCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Internals)definitionCast;
		if (!serialization.TryCustomCopy<Internals>(this, ref target, hookCtx, false, context))
		{
			bool UsingInternalsTemp = false;
			if (!serialization.TryCustomCopy<bool>(UsingInternals, ref UsingInternalsTemp, hookCtx, false, context))
			{
				UsingInternalsTemp = UsingInternals;
			}
			target.UsingInternals = UsingInternalsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Internals target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Internals cast = (Internals)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Internals cast = (Internals)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Internals Instantiate()
	{
		return new Internals();
	}
}
