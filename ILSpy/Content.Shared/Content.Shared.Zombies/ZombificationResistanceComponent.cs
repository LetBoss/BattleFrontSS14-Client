using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Zombies;

[NetworkedComponent]
[RegisterComponent]
public sealed class ZombificationResistanceComponent : Component, ISerializationGenerated<ZombificationResistanceComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float ZombificationResistanceCoefficient = 1f;

	[DataField(null, false, 1, false, false, null)]
	public LocId Examine = LocId.op_Implicit("zombification-resistance-coefficient-value");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ZombificationResistanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ZombificationResistanceComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ZombificationResistanceComponent>(this, ref target, hookCtx, false, context))
		{
			float ZombificationResistanceCoefficientTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ZombificationResistanceCoefficient, ref ZombificationResistanceCoefficientTemp, hookCtx, false, context))
			{
				ZombificationResistanceCoefficientTemp = ZombificationResistanceCoefficient;
			}
			target.ZombificationResistanceCoefficient = ZombificationResistanceCoefficientTemp;
			LocId ExamineTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(Examine, ref ExamineTemp, hookCtx, false, context))
			{
				ExamineTemp = serialization.CreateCopy<LocId>(Examine, hookCtx, context, false);
			}
			target.Examine = ExamineTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ZombificationResistanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ZombificationResistanceComponent cast = (ZombificationResistanceComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ZombificationResistanceComponent cast = (ZombificationResistanceComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ZombificationResistanceComponent def = (ZombificationResistanceComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ZombificationResistanceComponent Instantiate()
	{
		return new ZombificationResistanceComponent();
	}
}
