using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Atmos.Visualizers;

[RegisterComponent]
public sealed class PortableScrubberVisualsComponent : Component, ISerializationGenerated<PortableScrubberVisualsComponent>, ISerializationGenerated
{
	[DataField("idleState", false, 1, true, false, null)]
	public string IdleState;

	[DataField("runningState", false, 1, true, false, null)]
	public string RunningState;

	[DataField("readyState", false, 1, true, false, null)]
	public string ReadyState;

	[DataField("fullState", false, 1, true, false, null)]
	public string FullState;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PortableScrubberVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (PortableScrubberVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<PortableScrubberVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string idleState = null;
			if (IdleState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(IdleState, ref idleState, hookCtx, false, context))
			{
				idleState = IdleState;
			}
			target.IdleState = idleState;
			string runningState = null;
			if (RunningState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(RunningState, ref runningState, hookCtx, false, context))
			{
				runningState = RunningState;
			}
			target.RunningState = runningState;
			string readyState = null;
			if (ReadyState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ReadyState, ref readyState, hookCtx, false, context))
			{
				readyState = ReadyState;
			}
			target.ReadyState = readyState;
			string fullState = null;
			if (FullState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FullState, ref fullState, hookCtx, false, context))
			{
				fullState = FullState;
			}
			target.FullState = fullState;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PortableScrubberVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PortableScrubberVisualsComponent target2 = (PortableScrubberVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PortableScrubberVisualsComponent target2 = (PortableScrubberVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PortableScrubberVisualsComponent target2 = (PortableScrubberVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PortableScrubberVisualsComponent Instantiate()
	{
		return new PortableScrubberVisualsComponent();
	}
}
