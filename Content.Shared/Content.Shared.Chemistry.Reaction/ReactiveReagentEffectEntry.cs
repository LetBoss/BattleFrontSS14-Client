using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Chemistry.Reaction;

[DataDefinition]
public sealed class ReactiveReagentEffectEntry : ISerializationGenerated<ReactiveReagentEffectEntry>, ISerializationGenerated
{
	[DataField("methods", false, 1, false, false, null)]
	public HashSet<ReactionMethod> Methods;

	[DataField("reagents", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<ReagentPrototype>))]
	public HashSet<string>? Reagents;

	[DataField("effects", false, 1, true, false, null)]
	public List<EntityEffect> Effects;

	[DataField("groups", true, 1, false, true, typeof(PrototypeIdDictionarySerializer<HashSet<ReactionMethod>, ReactiveGroupPrototype>))]
	public Dictionary<string, HashSet<ReactionMethod>>? ReactiveGroups { get; private set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReactiveReagentEffectEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
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
			HashSet<string> ReagentsTemp = null;
			if (!serialization.TryCustomCopy<HashSet<string>>(Reagents, ref ReagentsTemp, hookCtx, true, context))
			{
				ReagentsTemp = serialization.CreateCopy<HashSet<string>>(Reagents, hookCtx, context, false);
			}
			target.Reagents = ReagentsTemp;
			List<EntityEffect> EffectsTemp = null;
			if (Effects == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntityEffect>>(Effects, ref EffectsTemp, hookCtx, true, context))
			{
				EffectsTemp = serialization.CreateCopy<List<EntityEffect>>(Effects, hookCtx, context, false);
			}
			target.Effects = EffectsTemp;
			Dictionary<string, HashSet<ReactionMethod>> ReactiveGroupsTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<string, HashSet<ReactionMethod>>>(ReactiveGroups, ref ReactiveGroupsTemp, hookCtx, true, context))
			{
				ReactiveGroupsTemp = serialization.CreateCopy<Dictionary<string, HashSet<ReactionMethod>>>(ReactiveGroups, hookCtx, context, false);
			}
			target.ReactiveGroups = ReactiveGroupsTemp;
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
