using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.DeviceLinking;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedDeviceLinkSystem) })]
public sealed class DeviceLinkSinkComponent : Component, ISerializationGenerated<DeviceLinkSinkComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<SinkPortPrototype>> Ports = new HashSet<ProtoId<SinkPortPrototype>>();

	[ViewVariables]
	public HashSet<EntityUid> LinkedSources = new HashSet<EntityUid>();

	[Access(/*Could not decode attribute arguments.*/)]
	public GameTick InvokeCounterTick;

	[DataField(null, false, 1, false, false, null)]
	[Access(/*Could not decode attribute arguments.*/)]
	public int InvokeCounter;

	[DataField(null, false, 1, false, false, null)]
	public int InvokeLimit = 10;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DeviceLinkSinkComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DeviceLinkSinkComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DeviceLinkSinkComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<ProtoId<SinkPortPrototype>> PortsTemp = null;
			if (Ports == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<SinkPortPrototype>>>(Ports, ref PortsTemp, hookCtx, true, context))
			{
				PortsTemp = serialization.CreateCopy<HashSet<ProtoId<SinkPortPrototype>>>(Ports, hookCtx, context, false);
			}
			target.Ports = PortsTemp;
			int InvokeCounterTemp = 0;
			if (!serialization.TryCustomCopy<int>(InvokeCounter, ref InvokeCounterTemp, hookCtx, false, context))
			{
				InvokeCounterTemp = InvokeCounter;
			}
			target.InvokeCounter = InvokeCounterTemp;
			int InvokeLimitTemp = 0;
			if (!serialization.TryCustomCopy<int>(InvokeLimit, ref InvokeLimitTemp, hookCtx, false, context))
			{
				InvokeLimitTemp = InvokeLimit;
			}
			target.InvokeLimit = InvokeLimitTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DeviceLinkSinkComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeviceLinkSinkComponent cast = (DeviceLinkSinkComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeviceLinkSinkComponent cast = (DeviceLinkSinkComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeviceLinkSinkComponent def = (DeviceLinkSinkComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DeviceLinkSinkComponent Instantiate()
	{
		return new DeviceLinkSinkComponent();
	}
}
