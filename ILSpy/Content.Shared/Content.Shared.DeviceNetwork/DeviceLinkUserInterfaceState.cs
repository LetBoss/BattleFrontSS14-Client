using System;
using System.Collections.Generic;
using Content.Shared.DeviceLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork;

[Serializable]
[NetSerializable]
public sealed class DeviceLinkUserInterfaceState : BoundUserInterfaceState
{
	public readonly ProtoId<SourcePortPrototype>[] Sources;

	public readonly ProtoId<SinkPortPrototype>[] Sinks;

	public readonly HashSet<(ProtoId<SourcePortPrototype> source, ProtoId<SinkPortPrototype> sink)> Links;

	public readonly List<(string source, string sink)>? Defaults;

	public readonly string SourceAddress;

	public readonly string SinkAddress;

	public DeviceLinkUserInterfaceState(ProtoId<SourcePortPrototype>[] sources, ProtoId<SinkPortPrototype>[] sinks, HashSet<(ProtoId<SourcePortPrototype> source, ProtoId<SinkPortPrototype> sink)> links, string sourceAddress, string sinkAddress, List<(string source, string sink)>? defaults = null)
	{
		Links = links;
		SourceAddress = sourceAddress;
		SinkAddress = sinkAddress;
		Defaults = defaults;
		Sources = sources;
		Sinks = sinks;
	}
}
