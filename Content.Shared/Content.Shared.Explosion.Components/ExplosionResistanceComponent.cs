using System;
using System.Collections.Generic;
using Content.Shared.Explosion.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Explosion.Components;

[NetworkedComponent]
[RegisterComponent]
[Access(new Type[] { typeof(SharedExplosionSystem) })]
public sealed class ExplosionResistanceComponent : Component, ISerializationGenerated<ExplosionResistanceComponent>, ISerializationGenerated
{
	[DataField("damageCoefficient", false, 1, false, false, null)]
	public float DamageCoefficient = 1f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Worn = true;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public LocId Examine = LocId.op_Implicit("explosion-resistance-coefficient-value");

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("modifiers", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<float, ExplosionPrototype>))]
	public Dictionary<string, float> Modifiers = new Dictionary<string, float>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExplosionResistanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExplosionResistanceComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ExplosionResistanceComponent>(this, ref target, hookCtx, false, context))
		{
			float DamageCoefficientTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DamageCoefficient, ref DamageCoefficientTemp, hookCtx, false, context))
			{
				DamageCoefficientTemp = DamageCoefficient;
			}
			target.DamageCoefficient = DamageCoefficientTemp;
			bool WornTemp = false;
			if (!serialization.TryCustomCopy<bool>(Worn, ref WornTemp, hookCtx, false, context))
			{
				WornTemp = Worn;
			}
			target.Worn = WornTemp;
			LocId ExamineTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(Examine, ref ExamineTemp, hookCtx, false, context))
			{
				ExamineTemp = serialization.CreateCopy<LocId>(Examine, hookCtx, context, false);
			}
			target.Examine = ExamineTemp;
			Dictionary<string, float> ModifiersTemp = null;
			if (Modifiers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, float>>(Modifiers, ref ModifiersTemp, hookCtx, true, context))
			{
				ModifiersTemp = serialization.CreateCopy<Dictionary<string, float>>(Modifiers, hookCtx, context, false);
			}
			target.Modifiers = ModifiersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExplosionResistanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionResistanceComponent cast = (ExplosionResistanceComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionResistanceComponent cast = (ExplosionResistanceComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExplosionResistanceComponent def = (ExplosionResistanceComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExplosionResistanceComponent Instantiate()
	{
		return new ExplosionResistanceComponent();
	}
}
