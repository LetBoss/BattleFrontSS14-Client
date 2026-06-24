using System;
using System.Collections.Generic;
using Content.Shared.Destructible.Thresholds;
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
public sealed class IngredientsWithTags : FoodMetamorphRule, ISerializationGenerated<IngredientsWithTags>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<TagPrototype>> Tags = new List<ProtoId<TagPrototype>>();

	[DataField(null, false, 1, true, false, null)]
	public MinMax Count = new MinMax();

	[DataField(null, false, 1, false, false, null)]
	public bool NeedAll = true;

	public override bool Check(IPrototypeManager protoMan, EntityManager entMan, EntityUid food, List<FoodSequenceVisualLayer> ingredients)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		FoodSequenceElementPrototype protoIndexed = default(FoodSequenceElementPrototype);
		foreach (FoodSequenceVisualLayer ingredient2 in ingredients)
		{
			if (!protoMan.TryIndex<FoodSequenceElementPrototype>(ingredient2.Proto, ref protoIndexed))
			{
				continue;
			}
			bool allowed = false;
			if (NeedAll)
			{
				allowed = true;
				foreach (ProtoId<TagPrototype> tag in Tags)
				{
					if (!protoIndexed.Tags.Contains(tag))
					{
						allowed = false;
						break;
					}
				}
			}
			else
			{
				allowed = false;
				foreach (ProtoId<TagPrototype> tag2 in Tags)
				{
					if (protoIndexed.Tags.Contains(tag2))
					{
						allowed = true;
						break;
					}
				}
			}
			if (allowed)
			{
				count++;
			}
		}
		if (count >= Count.Min)
		{
			return count <= Count.Max;
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IngredientsWithTags target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		FoodMetamorphRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (IngredientsWithTags)definitionCast;
		if (!serialization.TryCustomCopy<IngredientsWithTags>(this, ref target, hookCtx, false, context))
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
			MinMax CountTemp = default(MinMax);
			if (!serialization.TryCustomCopy<MinMax>(Count, ref CountTemp, hookCtx, false, context))
			{
				serialization.CopyTo<MinMax>(Count, ref CountTemp, hookCtx, context, false);
			}
			target.Count = CountTemp;
			bool NeedAllTemp = false;
			if (!serialization.TryCustomCopy<bool>(NeedAll, ref NeedAllTemp, hookCtx, false, context))
			{
				NeedAllTemp = NeedAll;
			}
			target.NeedAll = NeedAllTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IngredientsWithTags target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref FoodMetamorphRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IngredientsWithTags cast = (IngredientsWithTags)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IngredientsWithTags cast = (IngredientsWithTags)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override IngredientsWithTags Instantiate()
	{
		return new IngredientsWithTags();
	}
}
