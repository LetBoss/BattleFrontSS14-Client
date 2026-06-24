using System;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.Heal;

public sealed class XenoApplySalveActionEvent : EntityTargetActionEvent, ISerializationGenerated<XenoApplySalveActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Range = 2f;

	[DataField(null, false, 1, false, false, null)]
	public float PlasmaCostModifier = 2f;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 DamageTakenModifier = 0.75f;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 StandardHealAmount = 100f;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 SmallHealAmount = 15f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan TimeBetweenHeals = TimeSpan.FromSeconds(1L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan TotalHealDuration = TimeSpan.FromSeconds(5L);

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId HealEffect = EntProtoId.op_Implicit("RMCEffectHealHealer");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier HealSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Xeno/alien_drool1.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoApplySalveActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoApplySalveActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoApplySalveActionEvent>(this, ref target, hookCtx, false, context))
		{
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
			float PlasmaCostModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PlasmaCostModifier, ref PlasmaCostModifierTemp, hookCtx, false, context))
			{
				PlasmaCostModifierTemp = PlasmaCostModifier;
			}
			target.PlasmaCostModifier = PlasmaCostModifierTemp;
			FixedPoint2 DamageTakenModifierTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(DamageTakenModifier, ref DamageTakenModifierTemp, hookCtx, false, context))
			{
				DamageTakenModifierTemp = serialization.CreateCopy<FixedPoint2>(DamageTakenModifier, hookCtx, context, false);
			}
			target.DamageTakenModifier = DamageTakenModifierTemp;
			FixedPoint2 StandardHealAmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(StandardHealAmount, ref StandardHealAmountTemp, hookCtx, false, context))
			{
				StandardHealAmountTemp = serialization.CreateCopy<FixedPoint2>(StandardHealAmount, hookCtx, context, false);
			}
			target.StandardHealAmount = StandardHealAmountTemp;
			FixedPoint2 SmallHealAmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(SmallHealAmount, ref SmallHealAmountTemp, hookCtx, false, context))
			{
				SmallHealAmountTemp = serialization.CreateCopy<FixedPoint2>(SmallHealAmount, hookCtx, context, false);
			}
			target.SmallHealAmount = SmallHealAmountTemp;
			TimeSpan TimeBetweenHealsTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(TimeBetweenHeals, ref TimeBetweenHealsTemp, hookCtx, false, context))
			{
				TimeBetweenHealsTemp = serialization.CreateCopy<TimeSpan>(TimeBetweenHeals, hookCtx, context, false);
			}
			target.TimeBetweenHeals = TimeBetweenHealsTemp;
			TimeSpan TotalHealDurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(TotalHealDuration, ref TotalHealDurationTemp, hookCtx, false, context))
			{
				TotalHealDurationTemp = serialization.CreateCopy<TimeSpan>(TotalHealDuration, hookCtx, context, false);
			}
			target.TotalHealDuration = TotalHealDurationTemp;
			EntProtoId HealEffectTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(HealEffect, ref HealEffectTemp, hookCtx, false, context))
			{
				HealEffectTemp = serialization.CreateCopy<EntProtoId>(HealEffect, hookCtx, context, false);
			}
			target.HealEffect = HealEffectTemp;
			SoundSpecifier HealSoundTemp = null;
			if (HealSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(HealSound, ref HealSoundTemp, hookCtx, true, context))
			{
				HealSoundTemp = serialization.CreateCopy<SoundSpecifier>(HealSound, hookCtx, context, false);
			}
			target.HealSound = HealSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoApplySalveActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoApplySalveActionEvent cast = (XenoApplySalveActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoApplySalveActionEvent cast = (XenoApplySalveActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoApplySalveActionEvent Instantiate()
	{
		return new XenoApplySalveActionEvent();
	}
}
