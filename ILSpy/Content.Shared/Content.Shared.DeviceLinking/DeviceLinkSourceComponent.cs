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
using Robust.Shared.ViewVariables;

namespace Content.Shared.DeviceLinking;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedDeviceLinkSystem) })]
public sealed class DeviceLinkSourceComponent : Component, ISerializationGenerated<DeviceLinkSourceComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<SourcePortPrototype>> Ports = new HashSet<ProtoId<SourcePortPrototype>>();

	[ViewVariables]
	public Dictionary<ProtoId<SourcePortPrototype>, HashSet<EntityUid>> Outputs = new Dictionary<ProtoId<SourcePortPrototype>, HashSet<EntityUid>>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<SourcePortPrototype>, bool> LastSignals = new Dictionary<ProtoId<SourcePortPrototype>, bool>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype> Source, ProtoId<SinkPortPrototype> Sink)>> LinkedPorts = new Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>();

	[DataField(null, false, 1, false, false, null)]
	public float Range = 30f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DeviceLinkSourceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DeviceLinkSourceComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DeviceLinkSourceComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<ProtoId<SourcePortPrototype>> PortsTemp = null;
			if (Ports == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<SourcePortPrototype>>>(Ports, ref PortsTemp, hookCtx, true, context))
			{
				PortsTemp = serialization.CreateCopy<HashSet<ProtoId<SourcePortPrototype>>>(Ports, hookCtx, context, false);
			}
			target.Ports = PortsTemp;
			Dictionary<ProtoId<SourcePortPrototype>, bool> LastSignalsTemp = null;
			if (LastSignals == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<SourcePortPrototype>, bool>>(LastSignals, ref LastSignalsTemp, hookCtx, true, context))
			{
				LastSignalsTemp = serialization.CreateCopy<Dictionary<ProtoId<SourcePortPrototype>, bool>>(LastSignals, hookCtx, context, false);
			}
			target.LastSignals = LastSignalsTemp;
			Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>> LinkedPortsTemp = null;
			if (LinkedPorts == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>>(LinkedPorts, ref LinkedPortsTemp, hookCtx, true, context))
			{
				LinkedPortsTemp = serialization.CreateCopy<Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>>(LinkedPorts, hookCtx, context, false);
			}
			target.LinkedPorts = LinkedPortsTemp;
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DeviceLinkSourceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeviceLinkSourceComponent cast = (DeviceLinkSourceComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeviceLinkSourceComponent cast = (DeviceLinkSourceComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DeviceLinkSourceComponent def = (DeviceLinkSourceComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DeviceLinkSourceComponent Instantiate()
	{
		return new DeviceLinkSourceComponent();
	}
}
