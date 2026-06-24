using System;
using Content.Shared.Body.Prototypes;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class OrganType : EventEntityEffectCondition<OrganType>, ISerializationGenerated<OrganType>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, typeof(PrototypeIdSerializer<MetabolizerTypePrototype>))]
	public string Type;

	[DataField(null, false, 1, false, false, null)]
	public bool ShouldHave = true;

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		return Loc.GetString("reagent-effect-condition-guidebook-organ-type", new(string, object)[2]
		{
			("name", prototype.Index<MetabolizerTypePrototype>(Type).LocalizedName),
			("shouldhave", ShouldHave)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref OrganType target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EventEntityEffectCondition<OrganType> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (OrganType)definitionCast;
		if (!serialization.TryCustomCopy<OrganType>(this, ref target, hookCtx, false, context))
		{
			string TypeTemp = null;
			if (Type == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = Type;
			}
			target.Type = TypeTemp;
			bool ShouldHaveTemp = false;
			if (!serialization.TryCustomCopy<bool>(ShouldHave, ref ShouldHaveTemp, hookCtx, false, context))
			{
				ShouldHaveTemp = ShouldHave;
			}
			target.ShouldHave = ShouldHaveTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref OrganType target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffectCondition<OrganType> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OrganType cast = (OrganType)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		OrganType cast = (OrganType)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override OrganType Instantiate()
	{
		return new OrganType();
	}
}
