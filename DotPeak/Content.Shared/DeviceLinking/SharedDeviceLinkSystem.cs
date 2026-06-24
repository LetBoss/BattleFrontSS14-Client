// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceLinking.SharedDeviceLinkSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
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

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeviceLinkSourceComponent, ComponentStartup>(new EntityEventRefHandler<DeviceLinkSourceComponent, ComponentStartup>((object) this, __methodptr(OnSourceStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeviceLinkSourceComponent, ComponentRemove>(new EntityEventRefHandler<DeviceLinkSourceComponent, ComponentRemove>((object) this, __methodptr(OnSourceRemoved)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeviceLinkSinkComponent, ComponentRemove>(new EntityEventRefHandler<DeviceLinkSinkComponent, ComponentRemove>((object) this, __methodptr(OnSinkRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnSourceStartup(Entity<DeviceLinkSourceComponent> source, ref ComponentStartup args)
  {
    List<EntityUid> entityUidList = new List<EntityUid>();
    List<(string, string)> valueTupleList1 = new List<(string, string)>();
    foreach ((EntityUid key, HashSet<(ProtoId<SourcePortPrototype> Source, ProtoId<SinkPortPrototype> Sink)> tupleSet) in source.Comp.LinkedPorts)
    {
      DeviceLinkSinkComponent linkSinkComponent;
      if (!this.TryComp<DeviceLinkSinkComponent>(key, ref linkSinkComponent))
      {
        entityUidList.Add(key);
      }
      else
      {
        foreach ((ProtoId<SourcePortPrototype> Source, ProtoId<SinkPortPrototype> Sink) tuple1 in tupleSet)
        {
          if (linkSinkComponent.Ports.Contains(tuple1.Sink) && source.Comp.Ports.Contains(tuple1.Source))
          {
            Extensions.GetOrNew<ProtoId<SourcePortPrototype>, HashSet<EntityUid>>(source.Comp.Outputs, tuple1.Source).Add(key);
          }
          else
          {
            List<(string, string)> valueTupleList2 = valueTupleList1;
            (ProtoId<SourcePortPrototype> Source, ProtoId<SinkPortPrototype> Sink) tuple2 = tuple1;
            (string, string) valueTuple = (ProtoId<SourcePortPrototype>.op_Implicit(tuple2.Source), ProtoId<SinkPortPrototype>.op_Implicit(tuple2.Sink));
            valueTupleList2.Add(valueTuple);
          }
        }
        foreach ((string, string) valueTuple1 in valueTupleList1)
        {
          this.Log.Warning($"Device source {this.ToPrettyString(new EntityUid?(Entity<DeviceLinkSourceComponent>.op_Implicit(source)), (MetaDataComponent) null)} contains invalid links to entity {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(key))}: {valueTuple1.Item1}->{valueTuple1.Item2}");
          HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)> valueTupleSet = tupleSet;
          (string, string) valueTuple2 = valueTuple1;
          (ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>) valueTuple3 = (ProtoId<SourcePortPrototype>.op_Implicit(valueTuple2.Item1), ProtoId<SinkPortPrototype>.op_Implicit(valueTuple2.Item2));
          valueTupleSet.Remove(valueTuple3);
        }
        if (tupleSet.Count == 0)
        {
          entityUidList.Add(key);
        }
        else
        {
          valueTupleList1.Clear();
          linkSinkComponent.LinkedSources.Add(source.Owner);
        }
      }
    }
    foreach (EntityUid key in entityUidList)
    {
      source.Comp.LinkedPorts.Remove(key);
      this.Log.Warning($"Device source {this.ToPrettyString(new EntityUid?(Entity<DeviceLinkSourceComponent>.op_Implicit(source)), (MetaDataComponent) null)} contains invalid sink: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(key))}");
    }
  }

  private void OnSourceRemoved(Entity<DeviceLinkSourceComponent> source, ref ComponentRemove args)
  {
    EntityQuery<DeviceLinkSinkComponent> entityQuery = this.GetEntityQuery<DeviceLinkSinkComponent>();
    foreach (EntityUid key in source.Comp.LinkedPorts.Keys)
    {
      DeviceLinkSinkComponent sinkComponent;
      if (entityQuery.TryGetComponent(key, ref sinkComponent))
        this.RemoveSinkFromSourceInternal(Entity<DeviceLinkSourceComponent>.op_Implicit(source), key, Entity<DeviceLinkSourceComponent>.op_Implicit(source), sinkComponent);
      else
        this.Log.Error($"Device source {this.ToPrettyString(new EntityUid?(Entity<DeviceLinkSourceComponent>.op_Implicit(source)), (MetaDataComponent) null)} links to invalid entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(key))}");
    }
  }

  private void OnSinkRemoved(Entity<DeviceLinkSinkComponent> sink, ref ComponentRemove args)
  {
    foreach (EntityUid linkedSource in sink.Comp.LinkedSources)
    {
      DeviceLinkSourceComponent sourceComponent;
      if (this.TryComp<DeviceLinkSourceComponent>(linkedSource, ref sourceComponent))
        this.RemoveSinkFromSourceInternal(linkedSource, Entity<DeviceLinkSinkComponent>.op_Implicit(sink), sourceComponent, Entity<DeviceLinkSinkComponent>.op_Implicit(sink));
      else
        this.Log.Error($"Device sink {this.ToPrettyString(new EntityUid?(Entity<DeviceLinkSinkComponent>.op_Implicit(sink)), (MetaDataComponent) null)} source list contains invalid entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(linkedSource))}");
    }
  }

  public void EnsureSourcePorts(EntityUid uid, params ProtoId<SourcePortPrototype>[] ports)
  {
    if (ports.Length == 0)
      return;
    DeviceLinkSourceComponent linkSourceComponent = this.EnsureComp<DeviceLinkSourceComponent>(uid);
    foreach (ProtoId<SourcePortPrototype> port in ports)
    {
      if (!this._prototypeManager.HasIndex<SourcePortPrototype>(port))
        this.Log.Error($"Attempted to add invalid port {port} to {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
      else
        linkSourceComponent.Ports.Add(port);
    }
  }

  public void EnsureSinkPorts(EntityUid uid, params ProtoId<SinkPortPrototype>[] ports)
  {
    if (ports.Length == 0)
      return;
    DeviceLinkSinkComponent linkSinkComponent = this.EnsureComp<DeviceLinkSinkComponent>(uid);
    foreach (ProtoId<SinkPortPrototype> port in ports)
    {
      if (!this._prototypeManager.HasIndex<SinkPortPrototype>(port))
        this.Log.Error($"Attempted to add invalid port {port} to {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
      else
        linkSinkComponent.Ports.Add(port);
    }
  }

  public ProtoId<SourcePortPrototype>[] GetSourcePortIds(Entity<DeviceLinkSourceComponent> source)
  {
    return source.Comp.Ports.ToArray<ProtoId<SourcePortPrototype>>();
  }

  public List<SourcePortPrototype> GetSourcePorts(
    EntityUid sourceUid,
    DeviceLinkSourceComponent? sourceComponent = null)
  {
    if (!this.Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true))
      return new List<SourcePortPrototype>();
    List<SourcePortPrototype> sourcePorts = new List<SourcePortPrototype>();
    foreach (ProtoId<SourcePortPrototype> port in sourceComponent.Ports)
      sourcePorts.Add(this._prototypeManager.Index<SourcePortPrototype>(port));
    return sourcePorts;
  }

  public ProtoId<SinkPortPrototype>[] GetSinkPortIds(Entity<DeviceLinkSinkComponent> source)
  {
    return source.Comp.Ports.ToArray<ProtoId<SinkPortPrototype>>();
  }

  public List<SinkPortPrototype> GetSinkPorts(
    EntityUid sinkUid,
    DeviceLinkSinkComponent? sinkComponent = null)
  {
    if (!this.Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
      return new List<SinkPortPrototype>();
    List<SinkPortPrototype> sinkPorts = new List<SinkPortPrototype>();
    foreach (ProtoId<SinkPortPrototype> port in sinkComponent.Ports)
      sinkPorts.Add(this._prototypeManager.Index<SinkPortPrototype>(port));
    return sinkPorts;
  }

  public string PortName<TPort>(string port) where TPort : DevicePortPrototype, IPrototype
  {
    TPort port1;
    return !this._prototypeManager.TryIndex<TPort>(port, ref port1) ? port : this.Loc.GetString(port1.Name);
  }

  public HashSet<(ProtoId<SourcePortPrototype> source, ProtoId<SinkPortPrototype> sink)> GetLinks(
    EntityUid sourceUid,
    EntityUid sinkUid,
    DeviceLinkSourceComponent? sourceComponent = null)
  {
    HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)> valueTupleSet;
    return !this.Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true) || !sourceComponent.LinkedPorts.TryGetValue(sinkUid, out valueTupleSet) ? new HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>() : valueTupleSet;
  }

  public List<(string source, string sink)> GetDefaults(List<SourcePortPrototype> sources)
  {
    List<(string, string)> defaults = new List<(string, string)>();
    foreach (SourcePortPrototype source in sources)
    {
      if (source.DefaultLinks == null)
        return new List<(string, string)>();
      foreach (string defaultLink in source.DefaultLinks)
        defaults.Add((source.ID, defaultLink));
    }
    return defaults;
  }

  public void LinkDefaults(
    EntityUid? userId,
    EntityUid sourceUid,
    EntityUid sinkUid,
    DeviceLinkSourceComponent? sourceComponent = null,
    DeviceLinkSinkComponent? sinkComponent = null)
  {
    if (!this.Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true) || !this.Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
      return;
    if (userId.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(34, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(userId.Value)), "actor", "ToPrettyString(userId.Value)");
      logStringHandler.AppendLiteral(" is linking defaults between ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), "source", "ToPrettyString(sourceUid)");
      logStringHandler.AppendLiteral(" and ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), "sink", "ToPrettyString(sinkUid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.DeviceLinking, LogImpact.Low, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(30, 2);
      logStringHandler.AppendLiteral("linking defaults between ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), "source", "ToPrettyString(sourceUid)");
      logStringHandler.AppendLiteral(" and ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), "sink", "ToPrettyString(sinkUid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.DeviceLinking, LogImpact.Low, ref local);
    }
    List<(string, string)> defaults = this.GetDefaults(this.GetSourcePorts(sourceUid, sourceComponent));
    this.SaveLinks(userId, sourceUid, sinkUid, defaults, sourceComponent, sinkComponent);
    if (!userId.HasValue)
      return;
    this._popupSystem.PopupCursor(this.Loc.GetString("signal-linking-verb-success", ("machine", (object) sourceUid)), userId.Value);
  }

  public void SaveLinks(
    EntityUid? userId,
    EntityUid sourceUid,
    EntityUid sinkUid,
    List<(string source, string sink)> links,
    DeviceLinkSourceComponent? sourceComponent = null,
    DeviceLinkSinkComponent? sinkComponent = null)
  {
    if (!this.Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true) || !this.Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
      return;
    if (!this.InRange(sourceUid, sinkUid, sourceComponent.Range))
    {
      if (!userId.HasValue)
        return;
      this._popupSystem.PopupCursor(this.Loc.GetString("signal-linker-component-out-of-range"), userId.Value);
    }
    else
    {
      this.RemoveSinkFromSource(sourceUid, sinkUid, sourceComponent);
      foreach ((string source, string sink) in links)
      {
        if (sourceComponent.Ports.Contains(ProtoId<SourcePortPrototype>.op_Implicit(source)) && sinkComponent.Ports.Contains(ProtoId<SinkPortPrototype>.op_Implicit(sink)) && this.CanLink(userId, sourceUid, sinkUid, source, sink, false, sourceComponent))
        {
          Extensions.GetOrNew<ProtoId<SourcePortPrototype>, HashSet<EntityUid>>(sourceComponent.Outputs, ProtoId<SourcePortPrototype>.op_Implicit(source)).Add(sinkUid);
          Extensions.GetOrNew<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>(sourceComponent.LinkedPorts, sinkUid).Add((ProtoId<SourcePortPrototype>.op_Implicit(source), ProtoId<SinkPortPrototype>.op_Implicit(sink)));
          this.SendNewLinkEvent(userId, sourceUid, source, sinkUid, sink);
        }
      }
      if (links.Count <= 0)
        return;
      sinkComponent.LinkedSources.Add(sourceUid);
    }
  }

  public void RemoveAllFromSink(EntityUid sinkUid, DeviceLinkSinkComponent? sinkComponent = null)
  {
    if (!this.Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
      return;
    foreach (EntityUid linkedSource in sinkComponent.LinkedSources)
      this.RemoveSinkFromSource(linkedSource, sinkUid, sinkComponent: sinkComponent);
  }

  public void RemoveSinkFromSource(
    EntityUid sourceUid,
    EntityUid sinkUid,
    DeviceLinkSourceComponent? sourceComponent = null,
    DeviceLinkSinkComponent? sinkComponent = null)
  {
    if (this.Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, false) && this.Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, false))
    {
      this.RemoveSinkFromSourceInternal(sourceUid, sinkUid, sourceComponent, sinkComponent);
    }
    else
    {
      if (sourceComponent == null && sinkComponent == null)
        return;
      if (sourceComponent == null)
      {
        this.Log.Error($"Attempted to remove link between {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid))} and {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid))}, but the source component was missing.");
        sinkComponent.LinkedSources.Remove(sourceUid);
      }
      else
      {
        this.Log.Error($"Attempted to remove link between {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid))} and {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid))}, but the sink component was missing.");
        sourceComponent.LinkedPorts.Remove(sinkUid);
      }
    }
  }

  private void RemoveSinkFromSourceInternal(
    EntityUid sourceUid,
    EntityUid sinkUid,
    DeviceLinkSourceComponent sourceComponent,
    DeviceLinkSinkComponent sinkComponent)
  {
    HashSet<(ProtoId<SourcePortPrototype> Source, ProtoId<SinkPortPrototype> Sink)> tupleSet;
    if (sourceComponent.LinkedPorts.TryGetValue(sinkUid, out tupleSet))
    {
      foreach ((ProtoId<SourcePortPrototype> Source, ProtoId<SinkPortPrototype> Sink) in tupleSet)
      {
        this.RaiseLocalEvent<PortDisconnectedEvent>(sourceUid, new PortDisconnectedEvent(ProtoId<SourcePortPrototype>.op_Implicit(Source)), false);
        this.RaiseLocalEvent<PortDisconnectedEvent>(sinkUid, new PortDisconnectedEvent(ProtoId<SinkPortPrototype>.op_Implicit(Sink)), false);
      }
    }
    sinkComponent.LinkedSources.Remove(sourceUid);
    sourceComponent.LinkedPorts.Remove(sinkUid);
    foreach (HashSet<EntityUid> entityUidSet in sourceComponent.Outputs.Values)
      entityUidSet.Remove(sinkUid);
  }

  public bool ToggleLink(
    EntityUid? userId,
    EntityUid sourceUid,
    EntityUid sinkUid,
    string source,
    string sink,
    DeviceLinkSourceComponent? sourceComponent = null,
    DeviceLinkSinkComponent? sinkComponent = null)
  {
    if (!this.Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true) || !this.Resolve<DeviceLinkSinkComponent>(sinkUid, ref sinkComponent, true))
      return false;
    HashSet<EntityUid> orNew1 = Extensions.GetOrNew<ProtoId<SourcePortPrototype>, HashSet<EntityUid>>(sourceComponent.Outputs, ProtoId<SourcePortPrototype>.op_Implicit(source));
    HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)> orNew2 = Extensions.GetOrNew<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>(sourceComponent.LinkedPorts, sinkUid);
    if (orNew2.Contains((ProtoId<SourcePortPrototype>.op_Implicit(source), ProtoId<SinkPortPrototype>.op_Implicit(sink))))
    {
      if (userId.HasValue)
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(17, 5);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(userId.Value)), "actor", "ToPrettyString(userId.Value)");
        logStringHandler.AppendLiteral(" unlinked ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), nameof (source), "ToPrettyString(sourceUid)");
        logStringHandler.AppendLiteral(" ");
        logStringHandler.AppendFormatted(source);
        logStringHandler.AppendLiteral(" and ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), nameof (sink), "ToPrettyString(sinkUid)");
        logStringHandler.AppendLiteral(" ");
        logStringHandler.AppendFormatted(sink);
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.DeviceLinking, LogImpact.Low, ref local);
      }
      else
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(16 /*0x10*/, 4);
        logStringHandler.AppendLiteral("unlinked ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), nameof (source), "ToPrettyString(sourceUid)");
        logStringHandler.AppendLiteral(" ");
        logStringHandler.AppendFormatted(source);
        logStringHandler.AppendLiteral(" and ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), nameof (sink), "ToPrettyString(sinkUid)");
        logStringHandler.AppendLiteral(" ");
        logStringHandler.AppendFormatted(sink);
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.DeviceLinking, LogImpact.Low, ref local);
      }
      this.RaiseLocalEvent<PortDisconnectedEvent>(sourceUid, new PortDisconnectedEvent(source), false);
      this.RaiseLocalEvent<PortDisconnectedEvent>(sinkUid, new PortDisconnectedEvent(sink), false);
      orNew1.Remove(sinkUid);
      orNew2.Remove((ProtoId<SourcePortPrototype>.op_Implicit(source), ProtoId<SinkPortPrototype>.op_Implicit(sink)));
      if (orNew2.Count != 0)
        return true;
      sourceComponent.LinkedPorts.Remove(sinkUid);
      sinkComponent.LinkedSources.Remove(sourceUid);
      this.CreateLinkPopup(userId, sourceUid, source, sinkUid, sink, true);
    }
    else
    {
      if (!sourceComponent.Ports.Contains(ProtoId<SourcePortPrototype>.op_Implicit(source)) || !sinkComponent.Ports.Contains(ProtoId<SinkPortPrototype>.op_Implicit(sink)) || !this.CanLink(userId, sourceUid, sinkUid, source, sink, sourceComponent: sourceComponent))
        return false;
      orNew1.Add(sinkUid);
      orNew2.Add((ProtoId<SourcePortPrototype>.op_Implicit(source), ProtoId<SinkPortPrototype>.op_Implicit(sink)));
      sinkComponent.LinkedSources.Add(sourceUid);
      this.SendNewLinkEvent(userId, sourceUid, source, sinkUid, sink);
      this.CreateLinkPopup(userId, sourceUid, source, sinkUid, sink, false);
    }
    return true;
  }

  private bool CanLink(
    EntityUid? userId,
    EntityUid sourceUid,
    EntityUid sinkUid,
    string source,
    string sink,
    bool checkRange = true,
    DeviceLinkSourceComponent? sourceComponent = null)
  {
    if (!this.Resolve<DeviceLinkSourceComponent>(sourceUid, ref sourceComponent, true))
      return false;
    if (checkRange && !this.InRange(sourceUid, sinkUid, sourceComponent.Range))
    {
      if (userId.HasValue)
        this._popupSystem.PopupCursor(this.Loc.GetString("signal-linker-component-out-of-range"), userId.Value);
      return false;
    }
    LinkAttemptEvent linkAttemptEvent = new LinkAttemptEvent(userId, sourceUid, source, sinkUid, sink);
    this.RaiseLocalEvent<LinkAttemptEvent>(sourceUid, linkAttemptEvent, true);
    if (linkAttemptEvent.Cancelled && userId.HasValue)
    {
      this._popupSystem.PopupCursor(this.Loc.GetString("signal-linker-component-connection-refused", ("machine", (object) source)), userId.Value);
      return false;
    }
    this.RaiseLocalEvent<LinkAttemptEvent>(sinkUid, linkAttemptEvent, true);
    if (!linkAttemptEvent.Cancelled || !userId.HasValue)
      return !linkAttemptEvent.Cancelled;
    this._popupSystem.PopupCursor(this.Loc.GetString("signal-linker-component-connection-refused", ("machine", (object) source)), userId.Value);
    return false;
  }

  private bool InRange(EntityUid sourceUid, EntityUid sinkUid, float range)
  {
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(sourceUid, (TransformComponent) null);
    return ((MapCoordinates) ref mapCoordinates).InRange(this._transform.GetMapCoordinates(sinkUid, (TransformComponent) null), range);
  }

  private void SendNewLinkEvent(
    EntityUid? user,
    EntityUid sourceUid,
    string source,
    EntityUid sinkUid,
    string sink)
  {
    if (user.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(15, 5);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "actor", "ToPrettyString(user.Value)");
      logStringHandler.AppendLiteral(" linked ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), nameof (source), "ToPrettyString(sourceUid)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted(source);
      logStringHandler.AppendLiteral(" and ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), nameof (sink), "ToPrettyString(sinkUid)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted(sink);
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.DeviceLinking, LogImpact.Low, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(14, 4);
      logStringHandler.AppendLiteral("linked ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sourceUid)), nameof (source), "ToPrettyString(sourceUid)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted(source);
      logStringHandler.AppendLiteral(" and ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(sinkUid)), nameof (sink), "ToPrettyString(sinkUid)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted(sink);
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.DeviceLinking, LogImpact.Low, ref local);
    }
    NewLinkEvent newLinkEvent = new NewLinkEvent(user, sourceUid, source, sinkUid, sink);
    this.RaiseLocalEvent<NewLinkEvent>(sourceUid, newLinkEvent, false);
    this.RaiseLocalEvent<NewLinkEvent>(sinkUid, newLinkEvent, false);
  }

  private void CreateLinkPopup(
    EntityUid? userId,
    EntityUid sourceUid,
    string source,
    EntityUid sinkUid,
    string sink,
    bool removed)
  {
    if (!userId.HasValue)
      return;
    this._popupSystem.PopupCursor(this.Loc.GetString(removed ? "signal-linker-component-unlinked-port" : "signal-linker-component-linked-port", new (string, object)[4]
    {
      ("machine1", (object) sourceUid),
      ("port1", (object) this.PortName<SourcePortPrototype>(source)),
      ("machine2", (object) sinkUid),
      ("port2", (object) this.PortName<SinkPortPrototype>(sink))
    }), userId.Value, PopupType.Medium);
  }

  public virtual void InvokePort(
    EntityUid uid,
    string port,
    NetworkPayload? data = null,
    DeviceLinkSourceComponent? sourceComponent = null)
  {
  }

  public int GetEffectiveInvokeCounter(DeviceLinkSinkComponent sink)
  {
    GameTick curTick = this._gameTiming.CurTick;
    if (GameTick.op_LessThan(curTick, sink.InvokeCounterTick))
      return 0;
    uint num = curTick.Value - sink.InvokeCounterTick.Value;
    return (long) num >= (long) sink.InvokeCounter ? 0 : Math.Max(0, sink.InvokeCounter - (int) num);
  }

  protected void SetInvokeCounter(DeviceLinkSinkComponent sink, int value)
  {
    sink.InvokeCounterTick = this._gameTiming.CurTick;
    sink.InvokeCounter = value;
  }
}
