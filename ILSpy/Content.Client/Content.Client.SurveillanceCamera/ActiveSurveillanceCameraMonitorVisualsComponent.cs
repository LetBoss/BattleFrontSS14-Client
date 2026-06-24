using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.SurveillanceCamera;

[RegisterComponent]
public sealed class ActiveSurveillanceCameraMonitorVisualsComponent : Component, ISerializationGenerated<ActiveSurveillanceCameraMonitorVisualsComponent>, ISerializationGenerated
{
	public float TimeLeft = 10f;

	public Action? OnFinish;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActiveSurveillanceCameraMonitorVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (ActiveSurveillanceCameraMonitorVisualsComponent)(object)val;
		serialization.TryCustomCopy<ActiveSurveillanceCameraMonitorVisualsComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActiveSurveillanceCameraMonitorVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveSurveillanceCameraMonitorVisualsComponent target2 = (ActiveSurveillanceCameraMonitorVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveSurveillanceCameraMonitorVisualsComponent target2 = (ActiveSurveillanceCameraMonitorVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveSurveillanceCameraMonitorVisualsComponent target2 = (ActiveSurveillanceCameraMonitorVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActiveSurveillanceCameraMonitorVisualsComponent Instantiate()
	{
		return new ActiveSurveillanceCameraMonitorVisualsComponent();
	}
}
