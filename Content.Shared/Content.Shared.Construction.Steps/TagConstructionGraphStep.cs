using System;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class TagConstructionGraphStep : ArbitraryInsertConstructionGraphStep, ISerializationGenerated<TagConstructionGraphStep>, ISerializationGenerated
{
	[DataField("tag", false, 1, false, false, null)]
	private string? _tag;

	public override bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		TagSystem tagSystem = entityManager.EntitySysManager.GetEntitySystem<TagSystem>();
		if (!string.IsNullOrEmpty(_tag))
		{
			return tagSystem.HasTag(uid, ProtoId<TagPrototype>.op_Implicit(_tag));
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TagConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArbitraryInsertConstructionGraphStep definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TagConstructionGraphStep)definitionCast;
		if (!serialization.TryCustomCopy<TagConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
			string _tagTemp = null;
			if (!serialization.TryCustomCopy<string>(_tag, ref _tagTemp, hookCtx, false, context))
			{
				_tagTemp = _tag;
			}
			target._tag = _tagTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TagConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ArbitraryInsertConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TagConstructionGraphStep cast = (TagConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TagConstructionGraphStep cast = (TagConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TagConstructionGraphStep Instantiate()
	{
		return new TagConstructionGraphStep();
	}
}
