using System;
using System.Collections.Generic;
using Content.Shared.EntityEffects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Chemistry.Reagent;

[DataDefinition]
public sealed class ReactiveReagentEffectEntry : ISerializationGenerated<ReactiveReagentEffectEntry>, ISerializationGenerated
{
	[DataField("methods", false, 1, true, false, null)]
	public HashSet<ReactionMethod> Methods;

	[DataField("effects", false, 1, true, false, null)]
	public EntityEffect[] Effects;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReactiveReagentEffectEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ReactiveReagentEffectEntry>(this, ref target, hookCtx, false, context))
		{
			HashSet<ReactionMethod> MethodsTemp = null;
			if (Methods == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ReactionMethod>>(Methods, ref MethodsTemp, hookCtx, true, context))
			{
				MethodsTemp = serialization.CreateCopy<HashSet<ReactionMethod>>(Methods, hookCtx, context, false);
			}
			target.Methods = MethodsTemp;
			EntityEffect[] EffectsTemp = null;
			if (Effects == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<EntityEffect[]>(Effects, ref EffectsTemp, hookCtx, true, context))
			{
				EffectsTemp = serialization.CreateCopy<EntityEffect[]>(Effects, hookCtx, context, false);
			}
			target.Effects = EffectsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReactiveReagentEffectEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactiveReagentEffectEntry cast = (ReactiveReagentEffectEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ReactiveReagentEffectEntry Instantiate()
	{
		return new ReactiveReagentEffectEntry();
	}
}
