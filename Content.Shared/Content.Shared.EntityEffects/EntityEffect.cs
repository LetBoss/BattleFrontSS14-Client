using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Database;
using Content.Shared.Localizations;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects;

[ImplicitDataDefinitionForInheritors]
public abstract class EntityEffect : ISerializationGenerated<EntityEffect>, ISerializationGenerated
{
	[DataField("conditions", false, 1, false, false, null)]
	public EntityEffectCondition[]? Conditions;

	[DataField("probability", false, 1, false, false, null)]
	public float Probability = 1f;

	private protected string _id => GetType().Name;

	public virtual string ReagentEffectFormat => "guidebook-reagent-effect-description";

	public virtual LogImpact LogImpact { get; private set; } = LogImpact.Low;

	public virtual bool ShouldLog { get; private set; }

	protected abstract string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys);

	public abstract void Effect(EntityEffectBaseArgs args);

	public string? GuidebookEffectDescription(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		string effect = ReagentEffectGuidebookText(prototype, entSys);
		if (effect == null)
		{
			return null;
		}
		string reagentEffectFormat = ReagentEffectFormat;
		(string, object)[] obj = new(string, object)[4]
		{
			("effect", effect),
			("chance", Probability),
			default((string, object)),
			default((string, object))
		};
		EntityEffectCondition[]? conditions = Conditions;
		obj[2] = ("conditionCount", (conditions != null) ? conditions.Length : 0);
		obj[3] = ("conditions", ContentLocalizationManager.FormatList(Conditions?.Select((EntityEffectCondition x) => x.GuidebookExplanation(prototype)).ToList() ?? new List<string>()));
		return Loc.GetString(reagentEffectFormat, obj);
	}

	public EntityEffect()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<EntityEffect>(this, ref target, hookCtx, false, context))
		{
			EntityEffectCondition[] ConditionsTemp = null;
			if (!serialization.TryCustomCopy<EntityEffectCondition[]>(Conditions, ref ConditionsTemp, hookCtx, true, context))
			{
				ConditionsTemp = serialization.CreateCopy<EntityEffectCondition[]>(Conditions, hookCtx, context, false);
			}
			target.Conditions = ConditionsTemp;
			float ProbabilityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Probability, ref ProbabilityTemp, hookCtx, false, context))
			{
				ProbabilityTemp = Probability;
			}
			target.Probability = ProbabilityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect cast = (EntityEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual EntityEffect Instantiate()
	{
		throw new NotImplementedException();
	}
}
