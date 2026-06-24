using System;
using Content.Shared.Examine;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class TemperatureConstructionGraphStep : ConstructionGraphStep, ISerializationGenerated<TemperatureConstructionGraphStep>, ISerializationGenerated
{
	[DataField("minTemperature", false, 1, false, false, null)]
	public float? MinTemperature;

	[DataField("maxTemperature", false, 1, false, false, null)]
	public float? MaxTemperature;

	public override void DoExamine(ExaminedEvent examinedEvent)
	{
		float guideTemperature = (MinTemperature.HasValue ? MinTemperature.Value : (MaxTemperature.HasValue ? MaxTemperature.Value : 0f));
		examinedEvent.PushMarkup(Loc.GetString("construction-temperature-default", new(string, object)[1] { ("temperature", guideTemperature) }));
	}

	public override ConstructionGuideEntry GenerateGuideEntry()
	{
		float guideTemperature = (MinTemperature.HasValue ? MinTemperature.Value : (MaxTemperature.HasValue ? MaxTemperature.Value : 0f));
		ConstructionGuideEntry constructionGuideEntry = new ConstructionGuideEntry();
		constructionGuideEntry.Localization = "construction-presenter-temperature-step";
		constructionGuideEntry.Arguments = new(string, object)[1] { ("temperature", guideTemperature) };
		return constructionGuideEntry;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TemperatureConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConstructionGraphStep definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TemperatureConstructionGraphStep)definitionCast;
		if (!serialization.TryCustomCopy<TemperatureConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
			float? MinTemperatureTemp = null;
			if (!serialization.TryCustomCopy<float?>(MinTemperature, ref MinTemperatureTemp, hookCtx, false, context))
			{
				MinTemperatureTemp = MinTemperature;
			}
			target.MinTemperature = MinTemperatureTemp;
			float? MaxTemperatureTemp = null;
			if (!serialization.TryCustomCopy<float?>(MaxTemperature, ref MaxTemperatureTemp, hookCtx, false, context))
			{
				MaxTemperatureTemp = MaxTemperature;
			}
			target.MaxTemperature = MaxTemperatureTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TemperatureConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TemperatureConstructionGraphStep cast = (TemperatureConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TemperatureConstructionGraphStep cast = (TemperatureConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TemperatureConstructionGraphStep Instantiate()
	{
		return new TemperatureConstructionGraphStep();
	}
}
