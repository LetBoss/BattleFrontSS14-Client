using System;
using System.Collections.Generic;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Damage;

[Serializable]
[DataDefinition]
[NetSerializable]
[Virtual]
public class DamageModifierSet : ISerializationGenerated<DamageModifierSet>, ISerializationGenerated
{
	[DataField("coefficients", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<float, DamageTypePrototype>))]
	public Dictionary<string, float> Coefficients = new Dictionary<string, float>();

	[DataField("flatReductions", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<float, DamageTypePrototype>))]
	public Dictionary<string, float> FlatReduction = new Dictionary<string, float>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref DamageModifierSet target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<DamageModifierSet>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, float> CoefficientsTemp = null;
			if (Coefficients == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, float>>(Coefficients, ref CoefficientsTemp, hookCtx, true, context))
			{
				CoefficientsTemp = serialization.CreateCopy<Dictionary<string, float>>(Coefficients, hookCtx, context, false);
			}
			target.Coefficients = CoefficientsTemp;
			Dictionary<string, float> FlatReductionTemp = null;
			if (FlatReduction == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, float>>(FlatReduction, ref FlatReductionTemp, hookCtx, true, context))
			{
				FlatReductionTemp = serialization.CreateCopy<Dictionary<string, float>>(FlatReduction, hookCtx, context, false);
			}
			target.FlatReduction = FlatReductionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref DamageModifierSet target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageModifierSet cast = (DamageModifierSet)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual DamageModifierSet Instantiate()
	{
		return new DamageModifierSet();
	}
}
