using System;
using Content.Shared.Examine;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Construction.Steps;

public abstract class ArbitraryInsertConstructionGraphStep : EntityInsertConstructionGraphStep, ISerializationGenerated<ArbitraryInsertConstructionGraphStep>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string Name { get; private set; } = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? Icon { get; private set; }

	public override void DoExamine(ExaminedEvent examinedEvent)
	{
		if (!string.IsNullOrEmpty(Name))
		{
			string stepName = Loc.GetString(Name);
			examinedEvent.PushMarkup(Loc.GetString("construction-insert-arbitrary-entity", new(string, object)[1] { ("stepName", stepName) }));
		}
	}

	public override ConstructionGuideEntry GenerateGuideEntry()
	{
		string stepName = Loc.GetString(Name);
		ConstructionGuideEntry constructionGuideEntry = new ConstructionGuideEntry();
		constructionGuideEntry.Localization = "construction-presenter-arbitrary-step";
		constructionGuideEntry.Arguments = new(string, object)[1] { ("name", stepName) };
		constructionGuideEntry.Icon = Icon;
		return constructionGuideEntry;
	}

	public ArbitraryInsertConstructionGraphStep()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref ArbitraryInsertConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityInsertConstructionGraphStep definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ArbitraryInsertConstructionGraphStep)definitionCast;
		if (!serialization.TryCustomCopy<ArbitraryInsertConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			SpriteSpecifier IconTemp = null;
			if (!serialization.TryCustomCopy<SpriteSpecifier>(Icon, ref IconTemp, hookCtx, true, context))
			{
				IconTemp = serialization.CreateCopy<SpriteSpecifier>(Icon, hookCtx, context, false);
			}
			target.Icon = IconTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref ArbitraryInsertConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityInsertConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArbitraryInsertConstructionGraphStep cast = (ArbitraryInsertConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ArbitraryInsertConstructionGraphStep cast = (ArbitraryInsertConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ArbitraryInsertConstructionGraphStep Instantiate()
	{
		throw new NotImplementedException();
	}
}
