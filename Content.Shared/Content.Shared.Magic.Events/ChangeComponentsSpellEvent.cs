using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Magic.Events;

public sealed class ChangeComponentsSpellEvent : EntityTargetActionEvent, ISerializationGenerated<ChangeComponentsSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public ComponentRegistry ToAdd = new ComponentRegistry();

	[DataField(null, false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public HashSet<string> ToRemove = new HashSet<string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChangeComponentsSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ChangeComponentsSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<ChangeComponentsSpellEvent>(this, ref target, hookCtx, false, context))
		{
			ComponentRegistry ToAddTemp = null;
			if (ToAdd == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<ComponentRegistry>(ToAdd, ref ToAddTemp, hookCtx, false, context))
			{
				ToAddTemp = serialization.CreateCopy<ComponentRegistry>(ToAdd, hookCtx, context, false);
			}
			target.ToAdd = ToAddTemp;
			HashSet<string> ToRemoveTemp = null;
			if (ToRemove == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<string>>(ToRemove, ref ToRemoveTemp, hookCtx, true, context))
			{
				ToRemoveTemp = serialization.CreateCopy<HashSet<string>>(ToRemove, hookCtx, context, false);
			}
			target.ToRemove = ToRemoveTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChangeComponentsSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChangeComponentsSpellEvent cast = (ChangeComponentsSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChangeComponentsSpellEvent cast = (ChangeComponentsSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ChangeComponentsSpellEvent Instantiate()
	{
		return new ChangeComponentsSpellEvent();
	}
}
