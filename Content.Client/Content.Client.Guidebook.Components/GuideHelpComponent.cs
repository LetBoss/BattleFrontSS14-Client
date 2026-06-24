using System;
using System.Collections.Generic;
using Content.Shared.Guidebook;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.Guidebook.Components;

[RegisterComponent]
[Access(new Type[] { typeof(GuidebookSystem) })]
public sealed class GuideHelpComponent : Component, ISerializationGenerated<GuideHelpComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<ProtoId<GuideEntryPrototype>> Guides = new List<ProtoId<GuideEntryPrototype>>();

	[DataField("includeChildren", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool IncludeChildren = true;

	[DataField("openOnActivation", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool OpenOnActivation;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GuideHelpComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (GuideHelpComponent)(object)val;
		if (!serialization.TryCustomCopy<GuideHelpComponent>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<GuideEntryPrototype>> guides = null;
			if (Guides == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<GuideEntryPrototype>>>(Guides, ref guides, hookCtx, true, context))
			{
				guides = serialization.CreateCopy<List<ProtoId<GuideEntryPrototype>>>(Guides, hookCtx, context, false);
			}
			target.Guides = guides;
			bool includeChildren = false;
			if (!serialization.TryCustomCopy<bool>(IncludeChildren, ref includeChildren, hookCtx, false, context))
			{
				includeChildren = IncludeChildren;
			}
			target.IncludeChildren = includeChildren;
			bool openOnActivation = false;
			if (!serialization.TryCustomCopy<bool>(OpenOnActivation, ref openOnActivation, hookCtx, false, context))
			{
				openOnActivation = OpenOnActivation;
			}
			target.OpenOnActivation = openOnActivation;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GuideHelpComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GuideHelpComponent target2 = (GuideHelpComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GuideHelpComponent target2 = (GuideHelpComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GuideHelpComponent target2 = (GuideHelpComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GuideHelpComponent Instantiate()
	{
		return new GuideHelpComponent();
	}
}
