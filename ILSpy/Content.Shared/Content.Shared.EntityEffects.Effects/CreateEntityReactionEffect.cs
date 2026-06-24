using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.EntityEffects.Effects;

[DataDefinition]
public sealed class CreateEntityReactionEffect : EventEntityEffect<CreateEntityReactionEffect>, ISerializationGenerated<CreateEntityReactionEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string Entity;

	[DataField(null, false, 1, false, false, null)]
	public uint Number = 1u;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-create-entity-reaction-effect", new(string, object)[3]
		{
			("chance", Probability),
			("entname", IoCManager.Resolve<IPrototypeManager>().Index<EntityPrototype>(Entity).Name),
			("amount", Number)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CreateEntityReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EventEntityEffect<CreateEntityReactionEffect> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CreateEntityReactionEffect)definitionCast;
		if (!serialization.TryCustomCopy<CreateEntityReactionEffect>(this, ref target, hookCtx, false, context))
		{
			string EntityTemp = null;
			if (Entity == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Entity, ref EntityTemp, hookCtx, false, context))
			{
				EntityTemp = Entity;
			}
			target.Entity = EntityTemp;
			uint NumberTemp = 0u;
			if (!serialization.TryCustomCopy<uint>(Number, ref NumberTemp, hookCtx, false, context))
			{
				NumberTemp = Number;
			}
			target.Number = NumberTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CreateEntityReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<CreateEntityReactionEffect> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CreateEntityReactionEffect cast = (CreateEntityReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CreateEntityReactionEffect cast = (CreateEntityReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CreateEntityReactionEffect Instantiate()
	{
		return new CreateEntityReactionEffect();
	}
}
