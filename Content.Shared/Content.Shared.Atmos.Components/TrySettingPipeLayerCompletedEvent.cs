using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public sealed class TrySettingPipeLayerCompletedEvent : SimpleDoAfterEvent, ISerializationGenerated<TrySettingPipeLayerCompletedEvent>, ISerializationGenerated
{
	public AtmosPipeLayer PipeLayer;

	public TrySettingPipeLayerCompletedEvent(AtmosPipeLayer pipeLayer)
	{
		PipeLayer = pipeLayer;
	}

	public TrySettingPipeLayerCompletedEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TrySettingPipeLayerCompletedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (TrySettingPipeLayerCompletedEvent)definitionCast;
		serialization.TryCustomCopy<TrySettingPipeLayerCompletedEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TrySettingPipeLayerCompletedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TrySettingPipeLayerCompletedEvent cast = (TrySettingPipeLayerCompletedEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TrySettingPipeLayerCompletedEvent cast = (TrySettingPipeLayerCompletedEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TrySettingPipeLayerCompletedEvent Instantiate()
	{
		return new TrySettingPipeLayerCompletedEvent();
	}
}
