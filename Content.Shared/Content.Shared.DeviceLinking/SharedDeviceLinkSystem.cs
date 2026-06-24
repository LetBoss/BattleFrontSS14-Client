using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.DeviceNetwork;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.DeviceLinking;

public abstract class SharedDeviceLinkSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private IGameTiming _gameTiming;

	public const string InvokedPort = "link_port";

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DeviceLinkSourceComponent, ComponentStartup>((EntityEventRefHandler<DeviceLinkSourceComponent, ComponentStartup>)OnSourceStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeviceLinkSourceComponent, ComponentRemove>((EntityEventRefHandler<DeviceLinkSourceComponent, ComponentRemove>)OnSourceRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeviceLinkSinkComponent, ComponentRemove>((EntityEventRefHandler<DeviceLinkSinkComponent, ComponentRemove>)OnSinkRemoved, (Type[])null, (Type[])null);
	}

	private void OnSourceStartup(Entity<DeviceLinkSourceComponent> source, ref ComponentStartup args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		List<EntityUid> invalidSinks = new List<EntityUid>();
		List<(string, string)> invalidLinks = new List<(string, string)>();
		DeviceLinkSinkComponent sinkComponent = default(DeviceLinkSinkComponent);
		foreach (var (sink, links) in source.Comp.LinkedPorts)
		{
			if (!((EntitySystem)this).TryComp<DeviceLinkSinkComponent>(sink, ref sinkComponent))
			{
				invalidSinks.Add(sink);
				continue;
			}
			foreach (var link in links)
			{
				if (sinkComponent.Ports.Contains(link.Item2) && source.Comp.Ports.Contains(link.Item1))
				{
					Extensions.GetOrNew<ProtoId<SourcePortPrototype>, HashSet<EntityUid>>(source.Comp.Outputs, link.Item1).Add(sink);
					continue;
				}
				(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>) tuple = link;
				invalidLinks.Add((ProtoId<SourcePortPrototype>.op_Implicit(tuple.Item1), ProtoId<SinkPortPrototype>.op_Implicit(tuple.Item2)));
			}
			foreach (var link2 in invalidLinks)
			{
				((EntitySystem)this).Log.Warning($"Device source {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DeviceLinkSourceComponent>.op_Implicit(source), (MetaDataComponent)null)} contains invalid links to entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sink))}: {link2.Item1}->{link2.Item2}");
				(string, string) tuple2 = link2;
				links.Remove((ProtoId<SourcePortPrototype>.op_Implicit(tuple2.Item1), ProtoId<SinkPortPrototype>.op_Implicit(tuple2.Item2)));
			}
			if (links.Count == 0)
			{
				invalidSinks.Add(sink);
				continue;
			}
			invalidLinks.Clear();
			sinkComponent.LinkedSources.Add(source.Owner);
		}
		foreach (EntityUid sink2 in invalidSinks)
		{
			source.Comp.LinkedPorts.Remove(sink2);
			((EntitySystem)this).Log.Warning($"Device source {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DeviceLinkSourceComponent>.op_Implicit(source), (MetaDataComponent)null)} contains invalid sink: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sink2))}");
		}
	}

	private void OnSourceRemoved(Entity<DeviceLinkSourceComponent> source, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery<DeviceLinkSinkComponent> query = ((EntitySystem)this).GetEntityQuery<DeviceLinkSinkComponent>();
		DeviceLinkSinkComponent sink = default(DeviceLinkSinkComponent);
		foreach (EntityUid sinkUid in source.Comp.LinkedPorts.Keys)
		{
			if (query.TryGetComponent(sinkUid, ref sink))
			{
				RemoveSinkFromSourceInternal(Entity<DeviceLinkSourceComponent>.op_Implicit(source), sinkUid, Entity<DeviceLinkSourceComponent>.op_Implicit(source), sink);
				continue;
			}
			((EntitySystem)this).Log.Error($"Device source {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DeviceLinkSourceComponent>.op_Implicit(source), (MetaDataComponent)null)} links to invalid entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid))}");
		}
	}

	private void OnSinkRemoved(Entity<DeviceLinkSinkComponent> sink, ref ComponentRemove args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		DeviceLinkSourceComponent source = default(DeviceLinkSourceComponent);
		foreach (EntityUid sourceUid in sink.Comp.LinkedSources)
		{
			if (((EntitySystem)this).TryComp<DeviceLinkSourceComponent>(sourceUid, ref source))
			{
				RemoveSinkFromSourceInternal(sourceUid, Entity<DeviceLinkSinkComponent>.op_Implicit(sink), source, Entity<DeviceLinkSinkComponent>.op_Implicit(sink));
				continue;
			}
			((EntitySystem)this).Log.Error($"Device sink {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DeviceLinkSinkComponent>.op_Implicit(sink), (MetaDataComponent)null)} source list contains invalid entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid))}");
		}
	}

	public void EnsureSourcePorts(EntityUid uid, params ProtoId<SourcePortPrototype>[] ports)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (ports.Length == 0)
		{
			return;
		}
		DeviceLinkSourceComponent comp = ((EntitySystem)this).EnsureComp<DeviceLinkSourceComponent>(uid);
		foreach (ProtoId<SourcePortPrototype> port in ports)
		{
			if (!_prototypeManager.HasIndex<SourcePortPrototype>(port))
			{
				((EntitySystem)this).Log.Error($"Attempted to add invalid port {port} to {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			}
			else
			{
				comp.Ports.Add(port);
			}
		}
	}

	public void EnsureSinkPorts(EntityUid uid, params ProtoId<SinkPortPrototype>[] ports)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (ports.Length == 0)
		{
			return;
		}
		DeviceLinkSinkComponent comp = ((EntitySystem)this).EnsureComp<DeviceLinkSinkComponent>(uid);
		foreach (ProtoId<SinkPortPrototype> port in ports)
		{
			if (!_prototypeManager.HasIndex<SinkPortPrototype>(port))
			{
				((EntitySystem)this).Log.Error($"Attempted to add invalid port {port} to {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			}
			else
			{
				comp.Ports.Add(port);
			}
		}
	}

	public ProtoId<SourcePortPrototype>[] GetSourcePortIds(Entity<DeviceLinkSourceComponent> source)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return source.Comp.Ports.ToArray();
	}

	public List<SourcePortPrototype> GetSourcePorts(EntityUid sourceUid, DeviceLinkSourceComponent? sourceComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true))
		{
			return new List<SourcePortPrototype>();
		}
		List<SourcePortPrototype> sourcePorts = new List<SourcePortPrototype>();
		foreach (ProtoId<SourcePortPrototype> port in sourceComponent.Ports)
		{
			sourcePorts.Add(_prototypeManager.Index<SourcePortPrototype>(port));
		}
		return sourcePorts;
	}

	public ProtoId<SinkPortPrototype>[] GetSinkPortIds(Entity<DeviceLinkSinkComponent> source)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return source.Comp.Ports.ToArray();
	}

	public List<SinkPortPrototype> GetSinkPorts(EntityUid sinkUid, DeviceLinkSinkComponent? sinkComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
		{
			return new List<SinkPortPrototype>();
		}
		List<SinkPortPrototype> sinkPorts = new List<SinkPortPrototype>();
		foreach (ProtoId<SinkPortPrototype> port in sinkComponent.Ports)
		{
			sinkPorts.Add(_prototypeManager.Index<SinkPortPrototype>(port));
		}
		return sinkPorts;
	}

	public string PortName<TPort>(string port) where TPort : DevicePortPrototype, IPrototype
	{
		TPort proto = default(TPort);
		if (!_prototypeManager.TryIndex<TPort>(port, ref proto))
		{
			return port;
		}
		return base.Loc.GetString(proto.Name);
	}

	public HashSet<(ProtoId<SourcePortPrototype> source, ProtoId<SinkPortPrototype> sink)> GetLinks(EntityUid sourceUid, EntityUid sinkUid, DeviceLinkSourceComponent? sourceComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true) || !sourceComponent.LinkedPorts.TryGetValue(sinkUid, out HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)> links))
		{
			return new HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>();
		}
		return links;
	}

	public List<(string source, string sink)> GetDefaults(List<SourcePortPrototype> sources)
	{
		List<(string, string)> defaults = new List<(string, string)>();
		foreach (SourcePortPrototype source in sources)
		{
			if (source.DefaultLinks == null)
			{
				return new List<(string, string)>();
			}
			foreach (string defaultLink in source.DefaultLinks)
			{
				defaults.Add((source.ID, defaultLink));
			}
		}
		return defaults;
	}

	public void LinkDefaults(EntityUid? userId, EntityUid sourceUid, EntityUid sinkUid, DeviceLinkSourceComponent? sourceComponent = null, DeviceLinkSinkComponent? sinkComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true) && ((EntitySystem)this).Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
		{
			if (userId.HasValue)
			{
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(34, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(userId.Value)), "actor", "ToPrettyString(userId.Value)");
				handler.AppendLiteral(" is linking defaults between ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), "source", "ToPrettyString(sourceUid)");
				handler.AppendLiteral(" and ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), "sink", "ToPrettyString(sinkUid)");
				adminLogger.Add(LogType.DeviceLinking, LogImpact.Low, ref handler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = _adminLogger;
				LogStringHandler handler2 = new LogStringHandler(30, 2);
				handler2.AppendLiteral("linking defaults between ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), "source", "ToPrettyString(sourceUid)");
				handler2.AppendLiteral(" and ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), "sink", "ToPrettyString(sinkUid)");
				adminLogger2.Add(LogType.DeviceLinking, LogImpact.Low, ref handler2);
			}
			List<SourcePortPrototype> sourcePorts = GetSourcePorts(sourceUid, sourceComponent);
			List<(string, string)> defaults = GetDefaults(sourcePorts);
			SaveLinks(userId, sourceUid, sinkUid, defaults, sourceComponent, sinkComponent);
			if (userId.HasValue)
			{
				_popupSystem.PopupCursor(base.Loc.GetString("signal-linking-verb-success", (ValueTuple<string, object>)("machine", sourceUid)), userId.Value);
			}
		}
	}

	public void SaveLinks(EntityUid? userId, EntityUid sourceUid, EntityUid sinkUid, List<(string source, string sink)> links, DeviceLinkSourceComponent? sourceComponent = null, DeviceLinkSinkComponent? sinkComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true) || !((EntitySystem)this).Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
		{
			return;
		}
		if (!InRange(sourceUid, sinkUid, sourceComponent.Range))
		{
			if (userId.HasValue)
			{
				_popupSystem.PopupCursor(base.Loc.GetString("signal-linker-component-out-of-range"), userId.Value);
			}
			return;
		}
		RemoveSinkFromSource(sourceUid, sinkUid, sourceComponent);
		foreach (var (source, sink) in links)
		{
			if (sourceComponent.Ports.Contains(ProtoId<SourcePortPrototype>.op_Implicit(source)) && sinkComponent.Ports.Contains(ProtoId<SinkPortPrototype>.op_Implicit(sink)) && CanLink(userId, sourceUid, sinkUid, source, sink, checkRange: false, sourceComponent))
			{
				Extensions.GetOrNew<ProtoId<SourcePortPrototype>, HashSet<EntityUid>>(sourceComponent.Outputs, ProtoId<SourcePortPrototype>.op_Implicit(source)).Add(sinkUid);
				Extensions.GetOrNew<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>(sourceComponent.LinkedPorts, sinkUid).Add((ProtoId<SourcePortPrototype>.op_Implicit(source), ProtoId<SinkPortPrototype>.op_Implicit(sink)));
				SendNewLinkEvent(userId, sourceUid, source, sinkUid, sink);
			}
		}
		if (links.Count > 0)
		{
			sinkComponent.LinkedSources.Add(sourceUid);
		}
	}

	public void RemoveAllFromSink(EntityUid sinkUid, DeviceLinkSinkComponent? sinkComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
		{
			return;
		}
		foreach (EntityUid sourceUid in sinkComponent.LinkedSources)
		{
			RemoveSinkFromSource(sourceUid, sinkUid, null, sinkComponent);
		}
	}

	public void RemoveSinkFromSource(EntityUid sourceUid, EntityUid sinkUid, DeviceLinkSourceComponent? sourceComponent = null, DeviceLinkSinkComponent? sinkComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, false) && ((EntitySystem)this).Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, false))
		{
			RemoveSinkFromSourceInternal(sourceUid, sinkUid, sourceComponent, sinkComponent);
		}
		else if (sourceComponent != null || sinkComponent != null)
		{
			if (sourceComponent == null)
			{
				((EntitySystem)this).Log.Error($"Attempted to remove link between {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid))} and {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid))}, but the source component was missing.");
				sinkComponent.LinkedSources.Remove(sourceUid);
			}
			else
			{
				((EntitySystem)this).Log.Error($"Attempted to remove link between {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid))} and {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid))}, but the sink component was missing.");
				sourceComponent.LinkedPorts.Remove(sinkUid);
			}
		}
	}

	private void RemoveSinkFromSourceInternal(EntityUid sourceUid, EntityUid sinkUid, DeviceLinkSourceComponent sourceComponent, DeviceLinkSinkComponent sinkComponent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (sourceComponent.LinkedPorts.TryGetValue(sinkUid, out HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)> ports))
		{
			foreach (var (sourcePort, sinkPort) in ports)
			{
				((EntitySystem)this).RaiseLocalEvent<PortDisconnectedEvent>(sourceUid, new PortDisconnectedEvent(ProtoId<SourcePortPrototype>.op_Implicit(sourcePort)), false);
				((EntitySystem)this).RaiseLocalEvent<PortDisconnectedEvent>(sinkUid, new PortDisconnectedEvent(ProtoId<SinkPortPrototype>.op_Implicit(sinkPort)), false);
			}
		}
		sinkComponent.LinkedSources.Remove(sourceUid);
		sourceComponent.LinkedPorts.Remove(sinkUid);
		foreach (HashSet<EntityUid> value in sourceComponent.Outputs.Values)
		{
			value.Remove(sinkUid);
		}
	}

	public bool ToggleLink(EntityUid? userId, EntityUid sourceUid, EntityUid sinkUid, string source, string sink, DeviceLinkSourceComponent? sourceComponent = null, DeviceLinkSinkComponent? sinkComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true) || !((EntitySystem)this).Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
		{
			return false;
		}
		HashSet<EntityUid> outputs = Extensions.GetOrNew<ProtoId<SourcePortPrototype>, HashSet<EntityUid>>(sourceComponent.Outputs, ProtoId<SourcePortPrototype>.op_Implicit(source));
		HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)> linkedPorts = Extensions.GetOrNew<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>(sourceComponent.LinkedPorts, sinkUid);
		if (linkedPorts.Contains((ProtoId<SourcePortPrototype>.op_Implicit(source), ProtoId<SinkPortPrototype>.op_Implicit(sink))))
		{
			if (userId.HasValue)
			{
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(17, 5);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(userId.Value)), "actor", "ToPrettyString(userId.Value)");
				handler.AppendLiteral(" unlinked ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), "source", "ToPrettyString(sourceUid)");
				handler.AppendLiteral(" ");
				handler.AppendFormatted(source);
				handler.AppendLiteral(" and ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), "sink", "ToPrettyString(sinkUid)");
				handler.AppendLiteral(" ");
				handler.AppendFormatted(sink);
				adminLogger.Add(LogType.DeviceLinking, LogImpact.Low, ref handler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = _adminLogger;
				LogStringHandler handler2 = new LogStringHandler(16, 4);
				handler2.AppendLiteral("unlinked ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), "source", "ToPrettyString(sourceUid)");
				handler2.AppendLiteral(" ");
				handler2.AppendFormatted(source);
				handler2.AppendLiteral(" and ");
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), "sink", "ToPrettyString(sinkUid)");
				handler2.AppendLiteral(" ");
				handler2.AppendFormatted(sink);
				adminLogger2.Add(LogType.DeviceLinking, LogImpact.Low, ref handler2);
			}
			((EntitySystem)this).RaiseLocalEvent<PortDisconnectedEvent>(sourceUid, new PortDisconnectedEvent(source), false);
			((EntitySystem)this).RaiseLocalEvent<PortDisconnectedEvent>(sinkUid, new PortDisconnectedEvent(sink), false);
			outputs.Remove(sinkUid);
			linkedPorts.Remove((ProtoId<SourcePortPrototype>.op_Implicit(source), ProtoId<SinkPortPrototype>.op_Implicit(sink)));
			if (linkedPorts.Count != 0)
			{
				return true;
			}
			sourceComponent.LinkedPorts.Remove(sinkUid);
			sinkComponent.LinkedSources.Remove(sourceUid);
			CreateLinkPopup(userId, sourceUid, source, sinkUid, sink, removed: true);
		}
		else
		{
			if (!sourceComponent.Ports.Contains(ProtoId<SourcePortPrototype>.op_Implicit(source)) || !sinkComponent.Ports.Contains(ProtoId<SinkPortPrototype>.op_Implicit(sink)))
			{
				return false;
			}
			if (!CanLink(userId, sourceUid, sinkUid, source, sink, checkRange: true, sourceComponent))
			{
				return false;
			}
			outputs.Add(sinkUid);
			linkedPorts.Add((ProtoId<SourcePortPrototype>.op_Implicit(source), ProtoId<SinkPortPrototype>.op_Implicit(sink)));
			sinkComponent.LinkedSources.Add(sourceUid);
			SendNewLinkEvent(userId, sourceUid, source, sinkUid, sink);
			CreateLinkPopup(userId, sourceUid, source, sinkUid, sink, removed: false);
		}
		return true;
	}

	private bool CanLink(EntityUid? userId, EntityUid sourceUid, EntityUid sinkUid, string source, string sink, bool checkRange = true, DeviceLinkSourceComponent? sourceComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true))
		{
			return false;
		}
		if (checkRange && !InRange(sourceUid, sinkUid, sourceComponent.Range))
		{
			if (userId.HasValue)
			{
				_popupSystem.PopupCursor(base.Loc.GetString("signal-linker-component-out-of-range"), userId.Value);
			}
			return false;
		}
		LinkAttemptEvent linkAttemptEvent = new LinkAttemptEvent(userId, sourceUid, source, sinkUid, sink);
		((EntitySystem)this).RaiseLocalEvent<LinkAttemptEvent>(sourceUid, linkAttemptEvent, true);
		if (((CancellableEntityEventArgs)linkAttemptEvent).Cancelled && userId.HasValue)
		{
			_popupSystem.PopupCursor(base.Loc.GetString("signal-linker-component-connection-refused", (ValueTuple<string, object>)("machine", source)), userId.Value);
			return false;
		}
		((EntitySystem)this).RaiseLocalEvent<LinkAttemptEvent>(sinkUid, linkAttemptEvent, true);
		if (((CancellableEntityEventArgs)linkAttemptEvent).Cancelled && userId.HasValue)
		{
			_popupSystem.PopupCursor(base.Loc.GetString("signal-linker-component-connection-refused", (ValueTuple<string, object>)("machine", source)), userId.Value);
			return false;
		}
		return !((CancellableEntityEventArgs)linkAttemptEvent).Cancelled;
	}

	private bool InRange(EntityUid sourceUid, EntityUid sinkUid, float range)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates mapCoordinates = _transform.GetMapCoordinates(sourceUid, (TransformComponent)null);
		return ((MapCoordinates)(ref mapCoordinates)).InRange(_transform.GetMapCoordinates(sinkUid, (TransformComponent)null), range);
	}

	private void SendNewLinkEvent(EntityUid? user, EntityUid sourceUid, string source, EntityUid sinkUid, string sink)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		if (user.HasValue)
		{
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(15, 5);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "actor", "ToPrettyString(user.Value)");
			handler.AppendLiteral(" linked ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), "source", "ToPrettyString(sourceUid)");
			handler.AppendLiteral(" ");
			handler.AppendFormatted(source);
			handler.AppendLiteral(" and ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), "sink", "ToPrettyString(sinkUid)");
			handler.AppendLiteral(" ");
			handler.AppendFormatted(sink);
			adminLogger.Add(LogType.DeviceLinking, LogImpact.Low, ref handler);
		}
		else
		{
			ISharedAdminLogManager adminLogger2 = _adminLogger;
			LogStringHandler handler2 = new LogStringHandler(14, 4);
			handler2.AppendLiteral("linked ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), "source", "ToPrettyString(sourceUid)");
			handler2.AppendLiteral(" ");
			handler2.AppendFormatted(source);
			handler2.AppendLiteral(" and ");
			handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), "sink", "ToPrettyString(sinkUid)");
			handler2.AppendLiteral(" ");
			handler2.AppendFormatted(sink);
			adminLogger2.Add(LogType.DeviceLinking, LogImpact.Low, ref handler2);
		}
		NewLinkEvent newLinkEvent = new NewLinkEvent(user, sourceUid, source, sinkUid, sink);
		((EntitySystem)this).RaiseLocalEvent<NewLinkEvent>(sourceUid, newLinkEvent, false);
		((EntitySystem)this).RaiseLocalEvent<NewLinkEvent>(sinkUid, newLinkEvent, false);
	}

	private void CreateLinkPopup(EntityUid? userId, EntityUid sourceUid, string source, EntityUid sinkUid, string sink, bool removed)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (userId.HasValue)
		{
			string locString = (removed ? "signal-linker-component-unlinked-port" : "signal-linker-component-linked-port");
			_popupSystem.PopupCursor(base.Loc.GetString(locString, new(string, object)[4]
			{
				("machine1", sourceUid),
				("port1", PortName<SourcePortPrototype>(source)),
				("machine2", sinkUid),
				("port2", PortName<SinkPortPrototype>(sink))
			}), userId.Value, PopupType.Medium);
		}
	}

	public virtual void InvokePort(EntityUid uid, string port, NetworkPayload? data = null, DeviceLinkSourceComponent? sourceComponent = null)
	{
	}

	public int GetEffectiveInvokeCounter(DeviceLinkSinkComponent sink)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		GameTick curTick = _gameTiming.CurTick;
		if (curTick < sink.InvokeCounterTick)
		{
			return 0;
		}
		uint tickDelta = curTick.Value - sink.InvokeCounterTick.Value;
		if (tickDelta >= sink.InvokeCounter)
		{
			return 0;
		}
		return Math.Max(0, sink.InvokeCounter - (int)tickDelta);
	}

	protected void SetInvokeCounter(DeviceLinkSinkComponent sink, int value)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		sink.InvokeCounterTick = _gameTiming.CurTick;
		sink.InvokeCounter = value;
	}
}
