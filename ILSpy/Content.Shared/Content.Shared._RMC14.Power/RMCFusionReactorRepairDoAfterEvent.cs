using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Power;

[Serializable]
[NetSerializable]
public sealed class RMCFusionReactorRepairDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<RMCFusionReactorRepairDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public RMCFusionReactorState State;

	public RMCFusionReactorRepairDoAfterEvent(RMCFusionReactorState state)
	{
		State = state;
	}

	public RMCFusionReactorRepairDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCFusionReactorRepairDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCFusionReactorRepairDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<RMCFusionReactorRepairDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			RMCFusionReactorState StateTemp = RMCFusionReactorState.Working;
			if (!serialization.TryCustomCopy<RMCFusionReactorState>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target.State = StateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCFusionReactorRepairDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCFusionReactorRepairDoAfterEvent cast = (RMCFusionReactorRepairDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCFusionReactorRepairDoAfterEvent cast = (RMCFusionReactorRepairDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCFusionReactorRepairDoAfterEvent Instantiate()
	{
		return new RMCFusionReactorRepairDoAfterEvent();
	}
}
