using System;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Acid;

public sealed class XenoCorrosiveAcidEvent : EntityTargetActionEvent, ISerializationGenerated<XenoCorrosiveAcidEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId AcidId = EntProtoId.op_Implicit("XenoAcidNormal");

	[DataField(null, false, 1, false, false, null)]
	public XenoAcidStrength Strength = XenoAcidStrength.Normal;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 PlasmaCost = 100;

	[DataField(null, false, 1, false, false, null)]
	public int EnergyCost;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Time = TimeSpan.FromSeconds(225L);

	[DataField(null, false, 1, false, false, null)]
	public float Dps = 8f;

	[DataField(null, false, 1, false, false, null)]
	public float ExpendableLightDps = 2.5f;

	[DataField(null, false, 1, false, false, null)]
	public float ApplyTimeMultiplier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoCorrosiveAcidEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoCorrosiveAcidEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoCorrosiveAcidEvent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId AcidIdTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(AcidId, ref AcidIdTemp, hookCtx, false, context))
			{
				AcidIdTemp = serialization.CreateCopy<EntProtoId>(AcidId, hookCtx, context, false);
			}
			target.AcidId = AcidIdTemp;
			XenoAcidStrength StrengthTemp = (XenoAcidStrength)0;
			if (!serialization.TryCustomCopy<XenoAcidStrength>(Strength, ref StrengthTemp, hookCtx, false, context))
			{
				StrengthTemp = Strength;
			}
			target.Strength = StrengthTemp;
			FixedPoint2 PlasmaCostTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(PlasmaCost, ref PlasmaCostTemp, hookCtx, false, context))
			{
				PlasmaCostTemp = serialization.CreateCopy<FixedPoint2>(PlasmaCost, hookCtx, context, false);
			}
			target.PlasmaCost = PlasmaCostTemp;
			int EnergyCostTemp = 0;
			if (!serialization.TryCustomCopy<int>(EnergyCost, ref EnergyCostTemp, hookCtx, false, context))
			{
				EnergyCostTemp = EnergyCost;
			}
			target.EnergyCost = EnergyCostTemp;
			TimeSpan TimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Time, ref TimeTemp, hookCtx, false, context))
			{
				TimeTemp = serialization.CreateCopy<TimeSpan>(Time, hookCtx, context, false);
			}
			target.Time = TimeTemp;
			float DpsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Dps, ref DpsTemp, hookCtx, false, context))
			{
				DpsTemp = Dps;
			}
			target.Dps = DpsTemp;
			float ExpendableLightDpsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ExpendableLightDps, ref ExpendableLightDpsTemp, hookCtx, false, context))
			{
				ExpendableLightDpsTemp = ExpendableLightDps;
			}
			target.ExpendableLightDps = ExpendableLightDpsTemp;
			float ApplyTimeMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ApplyTimeMultiplier, ref ApplyTimeMultiplierTemp, hookCtx, false, context))
			{
				ApplyTimeMultiplierTemp = ApplyTimeMultiplier;
			}
			target.ApplyTimeMultiplier = ApplyTimeMultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoCorrosiveAcidEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoCorrosiveAcidEvent cast = (XenoCorrosiveAcidEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoCorrosiveAcidEvent cast = (XenoCorrosiveAcidEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoCorrosiveAcidEvent Instantiate()
	{
		return new XenoCorrosiveAcidEvent();
	}
}
