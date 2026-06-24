using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Silicons.Bots;

[DataDefinition]
public sealed class MedibotTreatment : ISerializationGenerated<MedibotTreatment>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ReagentPrototype> Reagent = ProtoId<ReagentPrototype>.op_Implicit(string.Empty);

	[DataField(null, false, 1, true, false, null)]
	public FixedPoint2 Quantity;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? MinDamage;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? MaxDamage;

	public bool IsValid(FixedPoint2 damage)
	{
		if (MaxDamage.HasValue)
		{
			FixedPoint2 value = damage;
			FixedPoint2? maxDamage = MaxDamage;
			if (!(value < maxDamage))
			{
				return false;
			}
		}
		if (MinDamage.HasValue)
		{
			FixedPoint2 value = damage;
			FixedPoint2? maxDamage = MinDamage;
			return value > maxDamage;
		}
		return true;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MedibotTreatment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<MedibotTreatment>(this, ref target, hookCtx, false, context))
		{
			ProtoId<ReagentPrototype> ReagentTemp = default(ProtoId<ReagentPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(Reagent, ref ReagentTemp, hookCtx, false, context))
			{
				ReagentTemp = serialization.CreateCopy<ProtoId<ReagentPrototype>>(Reagent, hookCtx, context, false);
			}
			target.Reagent = ReagentTemp;
			FixedPoint2 QuantityTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Quantity, ref QuantityTemp, hookCtx, false, context))
			{
				QuantityTemp = serialization.CreateCopy<FixedPoint2>(Quantity, hookCtx, context, false);
			}
			target.Quantity = QuantityTemp;
			FixedPoint2? MinDamageTemp = null;
			if (!serialization.TryCustomCopy<FixedPoint2?>(MinDamage, ref MinDamageTemp, hookCtx, false, context))
			{
				MinDamageTemp = serialization.CreateCopy<FixedPoint2?>(MinDamage, hookCtx, context, false);
			}
			target.MinDamage = MinDamageTemp;
			FixedPoint2? MaxDamageTemp = null;
			if (!serialization.TryCustomCopy<FixedPoint2?>(MaxDamage, ref MaxDamageTemp, hookCtx, false, context))
			{
				MaxDamageTemp = serialization.CreateCopy<FixedPoint2?>(MaxDamage, hookCtx, context, false);
			}
			target.MaxDamage = MaxDamageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MedibotTreatment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MedibotTreatment cast = (MedibotTreatment)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public MedibotTreatment Instantiate()
	{
		return new MedibotTreatment();
	}
}
