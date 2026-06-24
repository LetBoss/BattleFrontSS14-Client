using System;
using System.Collections.Generic;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.Prototypes;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Nutrition.FoodMetamorphRules;

[Serializable]
[NetSerializable]
public sealed class LastElementHasTags : FoodMetamorphRule, ISerializationGenerated<LastElementHasTags>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<TagPrototype>> Tags = new List<ProtoId<TagPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public bool NeedAll = true;

	public override bool Check(IPrototypeManager protoMan, EntityManager entMan, EntityUid food, List<FoodSequenceVisualLayer> ingredients)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		FoodSequenceElementPrototype protoIndexed = default(FoodSequenceElementPrototype);
		if (!protoMan.TryIndex<FoodSequenceElementPrototype>(ingredients[ingredients.Count - 1].Proto, ref protoIndexed))
		{
			return false;
		}
		foreach (ProtoId<TagPrototype> tag in Tags)
		{
			bool containsTag = protoIndexed.Tags.Contains(tag);
			if (NeedAll && !containsTag)
			{
				return false;
			}
			if (!NeedAll && containsTag)
			{
				return true;
			}
		}
		return NeedAll;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LastElementHasTags target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		FoodMetamorphRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LastElementHasTags)definitionCast;
		if (!serialization.TryCustomCopy<LastElementHasTags>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<TagPrototype>> TagsTemp = null;
			if (Tags == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(Tags, ref TagsTemp, hookCtx, true, context))
			{
				TagsTemp = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(Tags, hookCtx, context, false);
			}
			target.Tags = TagsTemp;
			bool NeedAllTemp = false;
			if (!serialization.TryCustomCopy<bool>(NeedAll, ref NeedAllTemp, hookCtx, false, context))
			{
				NeedAllTemp = NeedAll;
			}
			target.NeedAll = NeedAllTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LastElementHasTags target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref FoodMetamorphRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LastElementHasTags cast = (LastElementHasTags)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LastElementHasTags cast = (LastElementHasTags)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LastElementHasTags Instantiate()
	{
		return new LastElementHasTags();
	}
}
