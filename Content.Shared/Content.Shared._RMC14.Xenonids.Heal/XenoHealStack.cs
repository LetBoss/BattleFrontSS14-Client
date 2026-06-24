using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._RMC14.Xenonids.Heal;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class XenoHealStack : ISerializationGenerated<XenoHealStack>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 HealAmount;

	[DataField(null, false, 1, false, false, null)]
	public int Charges;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan TimeBetweenHeals;

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan NextHealAt;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoHealStack target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<XenoHealStack>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 HealAmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(HealAmount, ref HealAmountTemp, hookCtx, false, context))
			{
				HealAmountTemp = serialization.CreateCopy<FixedPoint2>(HealAmount, hookCtx, context, false);
			}
			target.HealAmount = HealAmountTemp;
			int ChargesTemp = 0;
			if (!serialization.TryCustomCopy<int>(Charges, ref ChargesTemp, hookCtx, false, context))
			{
				ChargesTemp = Charges;
			}
			target.Charges = ChargesTemp;
			TimeSpan TimeBetweenHealsTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(TimeBetweenHeals, ref TimeBetweenHealsTemp, hookCtx, false, context))
			{
				TimeBetweenHealsTemp = serialization.CreateCopy<TimeSpan>(TimeBetweenHeals, hookCtx, context, false);
			}
			target.TimeBetweenHeals = TimeBetweenHealsTemp;
			TimeSpan NextHealAtTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(NextHealAt, ref NextHealAtTemp, hookCtx, false, context))
			{
				NextHealAtTemp = serialization.CreateCopy<TimeSpan>(NextHealAt, hookCtx, context, false);
			}
			target.NextHealAt = NextHealAtTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoHealStack target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoHealStack cast = (XenoHealStack)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public XenoHealStack Instantiate()
	{
		return new XenoHealStack();
	}
}
