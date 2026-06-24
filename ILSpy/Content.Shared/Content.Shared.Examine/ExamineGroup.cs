using System;
using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Examine;

[DataDefinition]
public sealed class ExamineGroup : ISerializationGenerated<ExamineGroup>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? Title;

	[DataField(null, false, 1, false, false, null)]
	public List<ExamineEntry> Entries = new List<ExamineEntry>();

	[DataField(null, false, 1, false, false, null)]
	public List<string> Components = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier Icon = (SpriteSpecifier)new Texture(new ResPath("/Textures/Interface/examine-star.png"));

	[DataField(null, false, 1, false, false, null)]
	public LocId ContextText = LocId.op_Implicit("verb-examine-group-other");

	[DataField(null, false, 1, false, false, null)]
	public string HoverMessage = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExamineGroup target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ExamineGroup>(this, ref target, hookCtx, false, context))
		{
			string TitleTemp = null;
			if (!serialization.TryCustomCopy<string>(Title, ref TitleTemp, hookCtx, false, context))
			{
				TitleTemp = Title;
			}
			target.Title = TitleTemp;
			List<ExamineEntry> EntriesTemp = null;
			if (Entries == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ExamineEntry>>(Entries, ref EntriesTemp, hookCtx, true, context))
			{
				EntriesTemp = serialization.CreateCopy<List<ExamineEntry>>(Entries, hookCtx, context, false);
			}
			target.Entries = EntriesTemp;
			List<string> ComponentsTemp = null;
			if (Components == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(Components, ref ComponentsTemp, hookCtx, true, context))
			{
				ComponentsTemp = serialization.CreateCopy<List<string>>(Components, hookCtx, context, false);
			}
			target.Components = ComponentsTemp;
			SpriteSpecifier IconTemp = null;
			if (Icon == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SpriteSpecifier>(Icon, ref IconTemp, hookCtx, true, context))
			{
				IconTemp = serialization.CreateCopy<SpriteSpecifier>(Icon, hookCtx, context, false);
			}
			target.Icon = IconTemp;
			LocId ContextTextTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(ContextText, ref ContextTextTemp, hookCtx, false, context))
			{
				ContextTextTemp = serialization.CreateCopy<LocId>(ContextText, hookCtx, context, false);
			}
			target.ContextText = ContextTextTemp;
			string HoverMessageTemp = null;
			if (HoverMessage == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(HoverMessage, ref HoverMessageTemp, hookCtx, false, context))
			{
				HoverMessageTemp = HoverMessage;
			}
			target.HoverMessage = HoverMessageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExamineGroup target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExamineGroup cast = (ExamineGroup)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ExamineGroup Instantiate()
	{
		return new ExamineGroup();
	}
}
