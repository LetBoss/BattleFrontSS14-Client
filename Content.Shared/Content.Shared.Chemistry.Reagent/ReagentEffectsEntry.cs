using System;
using System.Linq;
using System.Text.Json.Serialization;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Chemistry.Reagent;

[DataDefinition]
public sealed class ReagentEffectsEntry : ISerializationGenerated<ReagentEffectsEntry>, ISerializationGenerated
{
	[JsonPropertyName("rate")]
	[DataField("metabolismRate", false, 1, false, false, null)]
	public FixedPoint2 MetabolismRate = FixedPoint2.New(0.5f);

	[JsonPropertyName("effects")]
	[DataField("effects", false, 1, true, false, null)]
	public EntityEffect[] Effects;

	public ReagentEffectsGuideEntry MakeGuideEntry(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return new ReagentEffectsGuideEntry(MetabolismRate, (from x in Effects
			select x.GuidebookEffectDescription(prototype, entSys) into x
			where x != null
			select (x)).ToArray());
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReagentEffectsEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ReagentEffectsEntry>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 MetabolismRateTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(MetabolismRate, ref MetabolismRateTemp, hookCtx, false, context))
			{
				MetabolismRateTemp = serialization.CreateCopy<FixedPoint2>(MetabolismRate, hookCtx, context, false);
			}
			target.MetabolismRate = MetabolismRateTemp;
			EntityEffect[] EffectsTemp = null;
			if (Effects == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<EntityEffect[]>(Effects, ref EffectsTemp, hookCtx, true, context))
			{
				EffectsTemp = serialization.CreateCopy<EntityEffect[]>(Effects, hookCtx, context, false);
			}
			target.Effects = EffectsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReagentEffectsEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentEffectsEntry cast = (ReagentEffectsEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ReagentEffectsEntry Instantiate()
	{
		return new ReagentEffectsEntry();
	}
}
