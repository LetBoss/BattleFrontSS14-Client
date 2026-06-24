using System;
using Content.Shared.Tag;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class HasTag : EntityEffectCondition, ISerializationGenerated<HasTag>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, typeof(PrototypeIdSerializer<TagPrototype>))]
	public string Tag;

	[DataField(null, false, 1, false, false, null)]
	public bool Invert;

	public override bool Condition(EntityEffectBaseArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		TagComponent tag = default(TagComponent);
		if (args.EntityManager.TryGetComponent<TagComponent>(args.TargetEntity, ref tag))
		{
			return args.EntityManager.System<TagSystem>().HasTag(tag, ProtoId<TagPrototype>.op_Implicit(Tag)) ^ Invert;
		}
		return false;
	}

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		return Loc.GetString("reagent-effect-condition-guidebook-has-tag", new(string, object)[2]
		{
			("tag", Tag),
			("invert", Invert)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HasTag target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityEffectCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HasTag)definitionCast;
		if (!serialization.TryCustomCopy<HasTag>(this, ref target, hookCtx, false, context))
		{
			string TagTemp = null;
			if (Tag == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Tag, ref TagTemp, hookCtx, false, context))
			{
				TagTemp = Tag;
			}
			target.Tag = TagTemp;
			bool InvertTemp = false;
			if (!serialization.TryCustomCopy<bool>(Invert, ref InvertTemp, hookCtx, false, context))
			{
				InvertTemp = Invert;
			}
			target.Invert = InvertTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HasTag target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HasTag cast = (HasTag)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HasTag cast = (HasTag)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HasTag Instantiate()
	{
		return new HasTag();
	}
}
