using System;
using System.Collections.Generic;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps;

public sealed class MultipleTagsConstructionGraphStep : ArbitraryInsertConstructionGraphStep, ISerializationGenerated<MultipleTagsConstructionGraphStep>, ISerializationGenerated
{
	[DataField("allTags", false, 1, false, false, null)]
	private List<ProtoId<TagPrototype>>? _allTags;

	[DataField("anyTags", false, 1, false, false, null)]
	private List<ProtoId<TagPrototype>>? _anyTags;

	private static bool IsNullOrEmpty<T>(ICollection<T>? list)
	{
		if (list != null)
		{
			return list.Count == 0;
		}
		return true;
	}

	public override bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (IsNullOrEmpty(_allTags) && IsNullOrEmpty(_anyTags))
		{
			return false;
		}
		TagSystem tagSystem = entityManager.EntitySysManager.GetEntitySystem<TagSystem>();
		if (_allTags != null && !tagSystem.HasAllTags(uid, _allTags))
		{
			return false;
		}
		if (_anyTags != null && !tagSystem.HasAnyTag(uid, _anyTags))
		{
			return false;
		}
		return true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MultipleTagsConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArbitraryInsertConstructionGraphStep definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MultipleTagsConstructionGraphStep)definitionCast;
		if (!serialization.TryCustomCopy<MultipleTagsConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<TagPrototype>> _allTagsTemp = null;
			if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(_allTags, ref _allTagsTemp, hookCtx, true, context))
			{
				_allTagsTemp = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(_allTags, hookCtx, context, false);
			}
			target._allTags = _allTagsTemp;
			List<ProtoId<TagPrototype>> _anyTagsTemp = null;
			if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(_anyTags, ref _anyTagsTemp, hookCtx, true, context))
			{
				_anyTagsTemp = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(_anyTags, hookCtx, context, false);
			}
			target._anyTags = _anyTagsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MultipleTagsConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ArbitraryInsertConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MultipleTagsConstructionGraphStep cast = (MultipleTagsConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MultipleTagsConstructionGraphStep cast = (MultipleTagsConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MultipleTagsConstructionGraphStep Instantiate()
	{
		return new MultipleTagsConstructionGraphStep();
	}
}
