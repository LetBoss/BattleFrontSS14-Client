using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Chemistry.Reaction;

[RegisterComponent]
public sealed class ReactiveComponent : Component, ISerializationGenerated<ReactiveComponent>, ISerializationGenerated
{
	[DataField("groups", true, 1, false, true, typeof(PrototypeIdDictionarySerializer<HashSet<ReactionMethod>, ReactiveGroupPrototype>))]
	public Dictionary<string, HashSet<ReactionMethod>>? ReactiveGroups;

	[DataField("reactions", true, 1, false, true, null)]
	public List<ReactiveReagentEffectEntry>? Reactions;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReactiveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ReactiveComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ReactiveComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, HashSet<ReactionMethod>> ReactiveGroupsTemp = null;
			if (!serialization.TryCustomCopy<Dictionary<string, HashSet<ReactionMethod>>>(ReactiveGroups, ref ReactiveGroupsTemp, hookCtx, true, context))
			{
				ReactiveGroupsTemp = serialization.CreateCopy<Dictionary<string, HashSet<ReactionMethod>>>(ReactiveGroups, hookCtx, context, false);
			}
			target.ReactiveGroups = ReactiveGroupsTemp;
			List<ReactiveReagentEffectEntry> ReactionsTemp = null;
			if (!serialization.TryCustomCopy<List<ReactiveReagentEffectEntry>>(Reactions, ref ReactionsTemp, hookCtx, true, context))
			{
				ReactionsTemp = serialization.CreateCopy<List<ReactiveReagentEffectEntry>>(Reactions, hookCtx, context, false);
			}
			target.Reactions = ReactionsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReactiveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactiveComponent cast = (ReactiveComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactiveComponent cast = (ReactiveComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactiveComponent def = (ReactiveComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ReactiveComponent Instantiate()
	{
		return new ReactiveComponent();
	}
}
