using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Magic.Events;

public sealed class SmiteSpellEvent : EntityTargetActionEvent, ISerializationGenerated<SmiteSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool DeleteNonBrainParts = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SmiteSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SmiteSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<SmiteSpellEvent>(this, ref target, hookCtx, false, context))
		{
			bool DeleteNonBrainPartsTemp = false;
			if (!serialization.TryCustomCopy<bool>(DeleteNonBrainParts, ref DeleteNonBrainPartsTemp, hookCtx, false, context))
			{
				DeleteNonBrainPartsTemp = DeleteNonBrainParts;
			}
			target.DeleteNonBrainParts = DeleteNonBrainPartsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SmiteSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmiteSpellEvent cast = (SmiteSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SmiteSpellEvent cast = (SmiteSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SmiteSpellEvent Instantiate()
	{
		return new SmiteSpellEvent();
	}
}
