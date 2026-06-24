using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Timing;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(UseDelaySystem) })]
public sealed class UseDelayComponent : Component, ISerializationGenerated<UseDelayComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, UseDelayInfo> Delays = new Dictionary<string, UseDelayInfo>();

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Delay = TimeSpan.FromSeconds(1L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref UseDelayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (UseDelayComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<UseDelayComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, UseDelayInfo> DelaysTemp = null;
			if (Delays == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, UseDelayInfo>>(Delays, ref DelaysTemp, hookCtx, true, context))
			{
				DelaysTemp = serialization.CreateCopy<Dictionary<string, UseDelayInfo>>(Delays, hookCtx, context, false);
			}
			target.Delays = DelaysTemp;
			TimeSpan DelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = serialization.CreateCopy<TimeSpan>(Delay, hookCtx, context, false);
			}
			target.Delay = DelayTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref UseDelayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UseDelayComponent cast = (UseDelayComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UseDelayComponent cast = (UseDelayComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UseDelayComponent def = (UseDelayComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override UseDelayComponent Instantiate()
	{
		return new UseDelayComponent();
	}
}
