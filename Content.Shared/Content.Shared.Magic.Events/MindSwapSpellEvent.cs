using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Magic.Events;

public sealed class MindSwapSpellEvent : EntityTargetActionEvent, ISerializationGenerated<MindSwapSpellEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan PerformerStunDuration = TimeSpan.FromSeconds(10L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan TargetStunDuration = TimeSpan.FromSeconds(10L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MindSwapSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MindSwapSpellEvent)definitionCast;
		if (!serialization.TryCustomCopy<MindSwapSpellEvent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan PerformerStunDurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(PerformerStunDuration, ref PerformerStunDurationTemp, hookCtx, false, context))
			{
				PerformerStunDurationTemp = serialization.CreateCopy<TimeSpan>(PerformerStunDuration, hookCtx, context, false);
			}
			target.PerformerStunDuration = PerformerStunDurationTemp;
			TimeSpan TargetStunDurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(TargetStunDuration, ref TargetStunDurationTemp, hookCtx, false, context))
			{
				TargetStunDurationTemp = serialization.CreateCopy<TimeSpan>(TargetStunDuration, hookCtx, context, false);
			}
			target.TargetStunDuration = TargetStunDurationTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MindSwapSpellEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MindSwapSpellEvent cast = (MindSwapSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MindSwapSpellEvent cast = (MindSwapSpellEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MindSwapSpellEvent Instantiate()
	{
		return new MindSwapSpellEvent();
	}
}
