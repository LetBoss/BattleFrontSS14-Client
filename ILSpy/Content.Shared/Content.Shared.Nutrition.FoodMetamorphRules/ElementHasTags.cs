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
public sealed class ElementHasTags : FoodMetamorphRule, ISerializationGenerated<ElementHasTags>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public int ElementNumber;

	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<TagPrototype>> Tags = new List<ProtoId<TagPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public bool NeedAll = true;

	public override bool Check(IPrototypeManager protoMan, EntityManager entMan, EntityUid food, List<FoodSequenceVisualLayer> ingredients)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (ingredients.Count < ElementNumber + 1)
		{
			return false;
		}
		FoodSequenceElementPrototype protoIndexed = default(FoodSequenceElementPrototype);
		if (!protoMan.TryIndex<FoodSequenceElementPrototype>(ingredients[ElementNumber].Proto, ref protoIndexed))
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
	public void InternalCopy(ref ElementHasTags target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		FoodMetamorphRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ElementHasTags)definitionCast;
		if (!serialization.TryCustomCopy<ElementHasTags>(this, ref target, hookCtx, false, context))
		{
			int ElementNumberTemp = 0;
			if (!serialization.TryCustomCopy<int>(ElementNumber, ref ElementNumberTemp, hookCtx, false, context))
			{
				ElementNumberTemp = ElementNumber;
			}
			target.ElementNumber = ElementNumberTemp;
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
	public void Copy(ref ElementHasTags target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref FoodMetamorphRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ElementHasTags cast = (ElementHasTags)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ElementHasTags cast = (ElementHasTags)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ElementHasTags Instantiate()
	{
		return new ElementHasTags();
	}
}
