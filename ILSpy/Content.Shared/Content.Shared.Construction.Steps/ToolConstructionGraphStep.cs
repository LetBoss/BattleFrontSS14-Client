using System;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Tools;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class ToolConstructionGraphStep : ConstructionGraphStep, ISerializationGenerated<ToolConstructionGraphStep>, ISerializationGenerated
{
	[DataField("tool", false, 1, true, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
	public string Tool { get; private set; } = string.Empty;

	[DataField("fuel", false, 1, false, false, null)]
	public float Fuel { get; private set; } = 10f;

	[DataField("examine", false, 1, false, false, null)]
	public string ExamineOverride { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public DuplicateConditions DuplicateConditions { get; private set; }

	public override void DoExamine(ExaminedEvent examinedEvent)
	{
		ToolQualityPrototype quality = default(ToolQualityPrototype);
		if (!string.IsNullOrEmpty(ExamineOverride))
		{
			examinedEvent.PushMarkup(Loc.GetString(ExamineOverride));
		}
		else if (!string.IsNullOrEmpty(Tool) && IoCManager.Resolve<IPrototypeManager>().TryIndex<ToolQualityPrototype>(Tool, ref quality))
		{
			examinedEvent.PushMarkup(Loc.GetString("construction-use-tool-entity", new(string, object)[1] { ("toolName", Loc.GetString(quality.ToolName)) }));
		}
	}

	public override ConstructionGuideEntry GenerateGuideEntry()
	{
		ToolQualityPrototype quality = IoCManager.Resolve<IPrototypeManager>().Index<ToolQualityPrototype>(Tool);
		ConstructionGuideEntry constructionGuideEntry = new ConstructionGuideEntry();
		constructionGuideEntry.Localization = "construction-presenter-tool-step";
		constructionGuideEntry.Arguments = new(string, object)[1] { ("tool", quality.ToolName) };
		constructionGuideEntry.Icon = quality.Icon;
		return constructionGuideEntry;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ToolConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		ConstructionGraphStep definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ToolConstructionGraphStep)definitionCast;
		if (!serialization.TryCustomCopy<ToolConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
			string ToolTemp = null;
			if (Tool == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Tool, ref ToolTemp, hookCtx, false, context))
			{
				ToolTemp = Tool;
			}
			target.Tool = ToolTemp;
			float FuelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Fuel, ref FuelTemp, hookCtx, false, context))
			{
				FuelTemp = Fuel;
			}
			target.Fuel = FuelTemp;
			string ExamineOverrideTemp = null;
			if (ExamineOverride == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ExamineOverride, ref ExamineOverrideTemp, hookCtx, false, context))
			{
				ExamineOverrideTemp = ExamineOverride;
			}
			target.ExamineOverride = ExamineOverrideTemp;
			DuplicateConditions DuplicateConditionsTemp = DuplicateConditions.None;
			if (!serialization.TryCustomCopy<DuplicateConditions>(DuplicateConditions, ref DuplicateConditionsTemp, hookCtx, false, context))
			{
				DuplicateConditionsTemp = DuplicateConditions;
			}
			target.DuplicateConditions = DuplicateConditionsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ToolConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolConstructionGraphStep cast = (ToolConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ToolConstructionGraphStep cast = (ToolConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ToolConstructionGraphStep Instantiate()
	{
		return new ToolConstructionGraphStep();
	}
}
