using System;
using Content.Shared.Polymorph;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.EntityEffects.Effects;

public sealed class Polymorph : EventEntityEffect<Polymorph>, ISerializationGenerated<Polymorph>, ISerializationGenerated
{
	[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<PolymorphPrototype>))]
	public string PolymorphPrototype { get; set; }

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		return Loc.GetString("reagent-effect-guidebook-make-polymorph", new(string, object)[2]
		{
			("chance", Probability),
			("entityname", prototype.Index<EntityPrototype>(EntProtoId.op_Implicit(prototype.Index<PolymorphPrototype>(PolymorphPrototype).Configuration.Entity)).Name)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Polymorph target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EventEntityEffect<Polymorph> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Polymorph)definitionCast;
		if (!serialization.TryCustomCopy<Polymorph>(this, ref target, hookCtx, false, context))
		{
			string PolymorphPrototypeTemp = null;
			if (PolymorphPrototype == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(PolymorphPrototype, ref PolymorphPrototypeTemp, hookCtx, false, context))
			{
				PolymorphPrototypeTemp = PolymorphPrototype;
			}
			target.PolymorphPrototype = PolymorphPrototypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Polymorph target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<Polymorph> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Polymorph cast = (Polymorph)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Polymorph cast = (Polymorph)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Polymorph Instantiate()
	{
		return new Polymorph();
	}
}
