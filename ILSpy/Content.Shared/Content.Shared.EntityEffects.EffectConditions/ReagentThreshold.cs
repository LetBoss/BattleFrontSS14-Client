using System;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class ReagentThreshold : EntityEffectCondition, ISerializationGenerated<ReagentThreshold>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 Min = FixedPoint2.Zero;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 Max = FixedPoint2.MaxValue;

	[DataField(null, false, 1, false, false, null)]
	public string? Reagent;

	public override bool Condition(EntityEffectBaseArgs args)
	{
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			string reagent = Reagent ?? reagentArgs.Reagent?.ID;
			if (reagent == null)
			{
				return true;
			}
			FixedPoint2 quant = FixedPoint2.Zero;
			if (reagentArgs.Source != null)
			{
				quant = reagentArgs.Source.GetTotalPrototypeQuantity(reagent);
			}
			if (quant >= Min)
			{
				return quant <= Max;
			}
			return false;
		}
		throw new NotImplementedException();
	}

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		ReagentPrototype reagentProto = null;
		if (Reagent != null && IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().TryIndex(ProtoId<ReagentPrototype>.op_Implicit(Reagent), out Reagent reagent))
		{
			reagentProto = reagent;
		}
		return Loc.GetString("reagent-effect-condition-guidebook-reagent-threshold", new(string, object)[3]
		{
			("reagent", reagentProto?.LocalizedName ?? Loc.GetString("reagent-effect-condition-guidebook-this-reagent")),
			("max", (Max == FixedPoint2.MaxValue) ? 2.1474836E+09f : Max.Float()),
			("min", Min.Float())
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReagentThreshold target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffectCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ReagentThreshold)definitionCast;
		if (!serialization.TryCustomCopy<ReagentThreshold>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 MinTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Min, ref MinTemp, hookCtx, false, context))
			{
				MinTemp = serialization.CreateCopy<FixedPoint2>(Min, hookCtx, context, false);
			}
			target.Min = MinTemp;
			FixedPoint2 MaxTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Max, ref MaxTemp, hookCtx, false, context))
			{
				MaxTemp = serialization.CreateCopy<FixedPoint2>(Max, hookCtx, context, false);
			}
			target.Max = MaxTemp;
			string ReagentTemp = null;
			if (!serialization.TryCustomCopy<string>(Reagent, ref ReagentTemp, hookCtx, false, context))
			{
				ReagentTemp = Reagent;
			}
			target.Reagent = ReagentTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReagentThreshold target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentThreshold cast = (ReagentThreshold)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentThreshold cast = (ReagentThreshold)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ReagentThreshold Instantiate()
	{
		return new ReagentThreshold();
	}
}
