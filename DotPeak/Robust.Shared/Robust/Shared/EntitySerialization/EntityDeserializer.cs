// Decompiled with JetBrains decompiler
// Type: Robust.Shared.EntitySerialization.EntityDeserializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.EntitySerialization.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable enable
namespace Robust.Shared.EntitySerialization;

public sealed class EntityDeserializer : 
  ISerializationContext,
  ITypeSerializer<EntityUid, ValueDataNode>,
  ITypeReader<EntityUid, ValueDataNode>,
  ITypeValidator<EntityUid, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<EntityUid, ValueDataNode>,
  ITypeWriter<EntityUid>,
  BaseSerializerInterfaces.ITypeInterface<EntityUid>,
  ITypeSerializer<NetEntity, ValueDataNode>,
  ITypeReader<NetEntity, ValueDataNode>,
  ITypeValidator<NetEntity, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<NetEntity, ValueDataNode>,
  ITypeWriter<NetEntity>,
  BaseSerializerInterfaces.ITypeInterface<NetEntity>,
  ITypeSerializer<MapId, ValueDataNode>,
  ITypeReader<MapId, ValueDataNode>,
  ITypeValidator<MapId, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<MapId, ValueDataNode>,
  ITypeWriter<MapId>,
  BaseSerializerInterfaces.ITypeInterface<MapId>
{
  public const int OldestSupportedVersion = 3;
  public const int NewestSupportedVersion = 7;
  [Dependency]
  public readonly EntityManager EntMan;
  [Dependency]
  public readonly IGameTiming Timing;
  [Dependency]
  private readonly ISerializationManager _seriMan;
  [Dependency]
  private readonly IComponentFactory _factory;
  [Dependency]
  private readonly IPrototypeManager _proto;
  [Dependency]
  private readonly SharedMapSystem _map;
  [Dependency]
  private readonly ILogManager _logMan;
  [Dependency]
  private readonly IDependencyCollection _deps;
  private readonly ISawmill _log;
  private Stopwatch _stopwatch = new Stopwatch();
  public readonly DeserializationOptions Options;
  public readonly MappingDataNode Data;
  public readonly Dictionary<EntityUid, EntityDeserializer.EntData> Entities = new Dictionary<EntityUid, EntityDeserializer.EntData>();
  public readonly Dictionary<int, EntityDeserializer.EntData> YamlEntities = new Dictionary<int, EntityDeserializer.EntData>();
  public readonly Dictionary<string, List<EntityDeserializer.EntData>> Prototypes = new Dictionary<string, List<EntityDeserializer.EntData>>();
  public readonly LoadResult Result = new LoadResult();
  public readonly Dictionary<int, string> TileMap = new Dictionary<int, string>();
  public readonly Dictionary<int, EntityUid> UidMap = new Dictionary<int, EntityUid>();
  public readonly Dictionary<int, MapId> AllocatedMapIds = new Dictionary<int, MapId>();
  public readonly List<int> MapYamlIds = new List<int>();
  public readonly List<int> GridYamlIds = new List<int>();
  public readonly List<int> OrphanYamlIds = new List<int>();
  public readonly List<int> NullspaceYamlIds = new List<int>();
  public readonly Dictionary<string, string> RenamedPrototypes;
  public readonly HashSet<string> DeletedPrototypes;
  public readonly HashSet<EntityUid> PostMapInit = new HashSet<EntityUid>();
  public readonly HashSet<EntityUid> Paused = new HashSet<EntityUid>();
  public readonly HashSet<EntityUid> ToDelete = new HashSet<EntityUid>();
  public readonly List<EntityUid> SortedEntities = new List<EntityUid>();
  private readonly Dictionary<string, MappingDataNode> _components = new Dictionary<string, MappingDataNode>();
  public EntityDeserializer.EntData? CurrentReadingEntity;
  public string? CurrentComponent;
  private readonly EntityQuery<MapComponent> _mapQuery;
  private readonly EntityQuery<MapGridComponent> _gridQuery;
  private readonly EntityQuery<TransformComponent> _xformQuery;
  private readonly EntityQuery<MetaDataComponent> _metaQuery;

  public SerializationManager.SerializerProvider SerializerProvider { get; } = new SerializationManager.SerializerProvider();

  public EntityDeserializer(
    IDependencyCollection deps,
    MappingDataNode data,
    DeserializationOptions options,
    Dictionary<string, string>? renamedPrototypes = null,
    HashSet<string>? deletedPrototypes = null)
  {
    deps.InjectDependencies((object) this);
    this._log = this._logMan.GetSawmill("entity_deserializer");
    this._log.Level = new LogLevel?(LogLevel.Info);
    this.SerializerProvider.RegisterSerializer((object) this);
    this.Data = data;
    this.Options = options;
    this.RenamedPrototypes = renamedPrototypes ?? new Dictionary<string, string>();
    this.DeletedPrototypes = deletedPrototypes ?? new HashSet<string>();
    this._mapQuery = this.EntMan.GetEntityQuery<MapComponent>();
    this._gridQuery = this.EntMan.GetEntityQuery<MapGridComponent>();
    this._xformQuery = this.EntMan.GetEntityQuery<TransformComponent>();
    this._metaQuery = this.EntMan.GetEntityQuery<MetaDataComponent>();
  }

  public bool WritingReadingPrototypes { get; private set; }

  public bool TryProcessData()
  {
    this.ReadMetadata();
    if (this.Result.Version < 3)
    {
      this._log.Error($"Cannot handle this map file version, found v{this.Result.Version} and require at least v{3}");
      return false;
    }
    if (this.Result.Version > 7)
    {
      this._log.Error($"Cannot handle this map file version, found v{this.Result.Version} but require at most v{7}");
      return false;
    }
    if (!this.ValidatePrototypes())
      return false;
    this.ReadEntities();
    this.ReadTileMap();
    this.ReadMapsAndGrids();
    return true;
  }

  public void CreateEntities()
  {
    this.AllocateEntities();
    if (this.Options.AssignMapIds)
      this.AllocateMapIds();
    this.LoadEntities();
    this.GetRootEntities();
    this.RemoveEmptyChunks();
    this.StoreGridTileMap();
    if (this.Options.AssignMapIds)
      this.AssignMapIds();
    this.CheckCategory();
  }

  public void StartEntities()
  {
    this.AdoptGrids();
    this.ValidateMapIds();
    this.BuildEntityHierarchy();
    this.StartEntitiesInternal();
    this.SetMapInitLifestage();
    this.SetPaused();
    this.GetRootNodes();
    this.PauseMaps();
    this.InitializeMaps();
    this.ProcessDeletions();
    if (!this.Options.AssignMapIds)
      return;
    foreach (int mapYamlId in this.MapYamlIds)
    {
      if (this.AllocatedMapIds.TryGetValue(mapYamlId, out MapId _))
        this.EntMan.EntityExists(this.UidMap[mapYamlId]);
    }
  }

  private void ReadMetadata()
  {
    MappingDataNode mappingDataNode = this.Data.Get<MappingDataNode>("meta");
    this.Result.Version = mappingDataNode.Get<ValueDataNode>("format").AsInt();
    ValueDataNode node1;
    if (mappingDataNode.TryGet<ValueDataNode>("engineVersion", out node1))
      this.Result.EngineVersion = node1.Value;
    ValueDataNode node2;
    if (mappingDataNode.TryGet<ValueDataNode>("forkId", out node2))
      this.Result.ForkId = node2.Value;
    ValueDataNode node3;
    if (mappingDataNode.TryGet<ValueDataNode>("forkVersion", out node3))
      this.Result.ForkVersion = node3.Value;
    ValueDataNode node4;
    DateTime result1;
    if (mappingDataNode.TryGet<ValueDataNode>("time", out node4) && DateTime.TryParse(node4.Value, out result1))
      this.Result.Time = new DateTime?(result1);
    ValueDataNode node5;
    FileCategory result2;
    if (!mappingDataNode.TryGet<ValueDataNode>("category", out node5) || !Enum.TryParse<FileCategory>(node5.Value, out result2))
      return;
    this.Result.Category = result2;
  }

  private bool ValidatePrototypes()
  {
    this._stopwatch.Restart();
    bool flag = false;
    string key = this.Result.Version >= 4 ? "proto" : "type";
    foreach (MappingDataNode mappingDataNode in this.Data.Get<SequenceDataNode>("entities").Cast<MappingDataNode>())
    {
      ValueDataNode node;
      if (mappingDataNode.TryGet<ValueDataNode>(key, out node))
      {
        string str1 = node.Value;
        if (!string.IsNullOrWhiteSpace(str1))
        {
          string str2;
          if (this.RenamedPrototypes.TryGetValue(str1, out str2))
            str1 = str2;
          if (this.DeletedPrototypes.Contains(str1))
            this._log.Warning("Map contains an obsolete/removed prototype: {0}. This may cause unexpected errors.", (object) str1);
          else if (!this._proto.HasIndex<Robust.Shared.Prototypes.EntityPrototype>(str1))
          {
            this._log.Error("Missing prototype for map: {0}", (object) str1);
            flag = true;
          }
        }
      }
    }
    this._log.Debug($"Verified entities in {this._stopwatch.Elapsed}");
    if (!flag)
      return true;
    this._log.Error("Found missing prototypes in map file. Missing prototypes have been dumped to logs.");
    return false;
  }

  private void ReadEntities()
  {
    if (this.Result.Version == 3)
      this.ReadEntitiesV3();
    else if (this.Result.Version < 7)
    {
      this.ReadEntitiesFallback();
    }
    else
    {
      foreach (MappingDataNode mappingDataNode1 in this.Data.Get<SequenceDataNode>("entities").Cast<MappingDataNode>())
      {
        EntProtoId? id = new EntProtoId?();
        bool ToDelete = false;
        ValueDataNode node1;
        if (mappingDataNode1.TryGet<ValueDataNode>("proto", out node1) && !string.IsNullOrWhiteSpace(node1.Value))
        {
          if (this.DeletedPrototypes.Contains(node1.Value))
          {
            ToDelete = true;
            if (this._proto.HasIndex<Robust.Shared.Prototypes.EntityPrototype>(node1.Value))
              id = (EntProtoId?) node1.Value;
          }
          else
          {
            string str;
            id = !this.RenamedPrototypes.TryGetValue(node1.Value, out str) ? (EntProtoId?) node1.Value : (EntProtoId?) str;
          }
        }
        SequenceDataNode source = (SequenceDataNode) mappingDataNode1["entities"];
        Robust.Shared.Prototypes.EntityPrototype prototype;
        this._proto.TryIndex(id, out prototype);
        List<EntityDeserializer.EntData> orNew = this.Prototypes.GetOrNew<string, List<EntityDeserializer.EntData>>(prototype?.ID ?? string.Empty);
        foreach (MappingDataNode mappingDataNode2 in source.Cast<MappingDataNode>())
        {
          int num = mappingDataNode2.Get<ValueDataNode>("uid").AsInt();
          ValueDataNode node2;
          bool PostInit = mappingDataNode2.TryGet<ValueDataNode>("mapInit", out node2) && node2.AsBool();
          ValueDataNode node3;
          bool Paused = mappingDataNode2.TryGet<ValueDataNode>("paused", out node3) ? node3.AsBool() : !PostInit;
          (Dictionary<string, MappingDataNode> dictionary, HashSet<string> stringSet) = this.GetComponents(mappingDataNode2);
          EntityDeserializer.EntData entData = new EntityDeserializer.EntData(num, mappingDataNode2, dictionary, stringSet, PostInit, Paused, ToDelete);
          orNew.Add(entData);
          this.YamlEntities.Add(num, entData);
        }
      }
    }
  }

  private void ReadEntitiesV3()
  {
    ValueDataNode node1;
    bool Paused = this.Data.Get<MappingDataNode>("meta").TryGet<ValueDataNode>("postmapinit", out node1) && !node1.AsBool();
    foreach (MappingDataNode mappingDataNode in this.Data.Get<SequenceDataNode>("entities").Cast<MappingDataNode>())
    {
      int num = mappingDataNode.Get<ValueDataNode>("uid").AsInt();
      EntProtoId? id = new EntProtoId?();
      bool ToDelete = false;
      ValueDataNode node2;
      if (mappingDataNode.TryGet<ValueDataNode>("type", out node2))
      {
        if (this.DeletedPrototypes.Contains(node2.Value))
        {
          ToDelete = true;
          if (this._proto.HasIndex<Robust.Shared.Prototypes.EntityPrototype>(node2.Value))
            id = (EntProtoId?) node2.Value;
        }
        else
        {
          string str;
          id = !this.RenamedPrototypes.TryGetValue(node2.Value, out str) ? (EntProtoId?) node2.Value : (EntProtoId?) str;
        }
      }
      Robust.Shared.Prototypes.EntityPrototype prototype;
      this._proto.TryIndex(id, out prototype);
      List<EntityDeserializer.EntData> orNew = this.Prototypes.GetOrNew<string, List<EntityDeserializer.EntData>>(prototype?.ID ?? string.Empty);
      (Dictionary<string, MappingDataNode> dictionary, HashSet<string> stringSet) = this.GetComponents(mappingDataNode);
      EntityDeserializer.EntData entData1 = new EntityDeserializer.EntData(num, mappingDataNode, dictionary, stringSet, !Paused, Paused, ToDelete);
      EntityDeserializer.EntData entData2 = entData1;
      orNew.Add(entData2);
      this.YamlEntities.Add(num, entData1);
    }
  }

  private void ReadEntitiesFallback()
  {
    ValueDataNode node1;
    bool Paused = this.Data.Get<MappingDataNode>("meta").TryGet<ValueDataNode>("postmapinit", out node1) && !node1.AsBool();
    foreach (MappingDataNode mappingDataNode1 in this.Data.Get<SequenceDataNode>("entities").Cast<MappingDataNode>())
    {
      EntProtoId? id = new EntProtoId?();
      bool ToDelete = false;
      ValueDataNode node2;
      if (mappingDataNode1.TryGet<ValueDataNode>("proto", out node2) && !string.IsNullOrWhiteSpace(node2.Value))
      {
        if (this.DeletedPrototypes.Contains(node2.Value))
        {
          ToDelete = true;
          if (this._proto.HasIndex<Robust.Shared.Prototypes.EntityPrototype>(node2.Value))
            id = (EntProtoId?) node2.Value;
        }
        else
        {
          string str;
          id = !this.RenamedPrototypes.TryGetValue(node2.Value, out str) ? (EntProtoId?) node2.Value : (EntProtoId?) str;
        }
      }
      SequenceDataNode source = (SequenceDataNode) mappingDataNode1["entities"];
      Robust.Shared.Prototypes.EntityPrototype prototype;
      this._proto.TryIndex(id, out prototype);
      List<EntityDeserializer.EntData> orNew = this.Prototypes.GetOrNew<string, List<EntityDeserializer.EntData>>(prototype?.ID ?? string.Empty);
      foreach (MappingDataNode mappingDataNode2 in source.Cast<MappingDataNode>())
      {
        int num = mappingDataNode2.Get<ValueDataNode>("uid").AsInt();
        (Dictionary<string, MappingDataNode> dictionary, HashSet<string> stringSet) = this.GetComponents(mappingDataNode2);
        EntityDeserializer.EntData entData = new EntityDeserializer.EntData(num, mappingDataNode2, dictionary, stringSet, !Paused, Paused, ToDelete);
        orNew.Add(entData);
        this.YamlEntities.Add(num, entData);
      }
    }
  }

  private (Dictionary<string, MappingDataNode>? Comps, HashSet<string>? Missing) GetComponents(
    MappingDataNode node)
  {
    Dictionary<string, MappingDataNode> dictionary = (Dictionary<string, MappingDataNode>) null;
    HashSet<string> stringSet = (HashSet<string>) null;
    SequenceDataNode node1;
    if (node.TryGet<SequenceDataNode>("components", out node1))
    {
      dictionary = new Dictionary<string, MappingDataNode>(node1.Count);
      foreach (MappingDataNode mappingDataNode in node1.Cast<MappingDataNode>())
      {
        string key = ((ValueDataNode) mappingDataNode["type"]).Value;
        mappingDataNode.Remove("type");
        dictionary.Add(key, mappingDataNode);
      }
    }
    SequenceDataNode node2;
    if (node.TryGet<SequenceDataNode>("missingComponents", out node2))
    {
      stringSet = new HashSet<string>(node2.Count);
      foreach (DataNode dataNode in node2)
        stringSet.Add(((ValueDataNode) dataNode).Value);
    }
    node.Remove("components");
    node.Remove("missingComponents");
    return (dictionary, stringSet);
  }

  private void ReadTileMap()
  {
    this._stopwatch.Restart();
    MappingDataNode mappingDataNode = this.Data.Get<MappingDataNode>("tilemap");
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (TileAliasPrototype enumeratePrototype in this._proto.EnumeratePrototypes<TileAliasPrototype>())
      dictionary.Add(enumeratePrototype.ID, enumeratePrototype.Target);
    foreach ((string str1, DataNode dataNode) in (IEnumerable<KeyValuePair<string, DataNode>>) mappingDataNode.Children)
    {
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      int key1 = int.Parse(str1, (IFormatProvider) invariantCulture);
      string key2 = ((ValueDataNode) dataNode).Value;
      string str2;
      if (dictionary.TryGetValue(key2, out str2))
        key2 = str2;
      this.TileMap.Add(key1, key2);
    }
    this._log.Debug($"Read tilemap in {this._stopwatch.Elapsed}");
  }

  private void AllocateEntities()
  {
    this._stopwatch.Restart();
    foreach ((string str, List<EntityDeserializer.EntData> entDataList) in this.Prototypes)
    {
      Robust.Shared.Prototypes.EntityPrototype prototype = str == string.Empty ? (Robust.Shared.Prototypes.EntityPrototype) null : this._proto.Index<Robust.Shared.Prototypes.EntityPrototype>(str);
      foreach (EntityDeserializer.EntData entData in entDataList)
      {
        EntityUid entityUid = this.EntMan.AllocEntity(prototype);
        this.Result.Entities.Add(entityUid);
        this.UidMap.Add(entData.YamlId, entityUid);
        this.Entities.Add(entityUid, entData);
        if (entData.PostInit)
          this.PostMapInit.Add(entityUid);
        if (entData.Paused)
          this.Paused.Add(entityUid);
        if (entData.ToDelete)
          this.ToDelete.Add(entityUid);
        if (this.Options.StoreYamlUids)
          this.EntMan.AddComponent<YamlUidComponent>(entityUid).Uid = entData.YamlId;
      }
    }
    this._log.Debug($"Allocated {this.Entities.Count} entities in {this._stopwatch.Elapsed}");
  }

  private void AllocateMapIds()
  {
    if (this.Result.Version < 7)
      return;
    foreach (int mapYamlId in this.MapYamlIds)
    {
      EntityUid uid = this.UidMap[mapYamlId];
      this.AllocatedMapIds[mapYamlId] = this._map.AllocateMapId(uid);
    }
  }

  private void ReadMapsAndGrids()
  {
    if (this.Result.Version < 7)
      return;
    this.ReadYamlIdList(this.Data, "maps", this.MapYamlIds);
    this.ReadYamlIdList(this.Data, "grids", this.GridYamlIds);
    this.ReadYamlIdList(this.Data, "orphans", this.OrphanYamlIds);
    this.ReadYamlIdList(this.Data, "nullspace", this.NullspaceYamlIds);
  }

  private void ReadYamlIdList(MappingDataNode data, string key, List<int> list)
  {
    SequenceDataNode sequenceDataNode = data.Get<SequenceDataNode>(key);
    list.EnsureCapacity(sequenceDataNode.Count);
    foreach (ValueDataNode valueDataNode in sequenceDataNode)
    {
      int num = valueDataNode.AsInt();
      list.Add(num);
    }
  }

  private void LoadEntities()
  {
    this._stopwatch.Restart();
    foreach ((EntityUid entityUid, EntityDeserializer.EntData entData) in this.Entities)
    {
      try
      {
        this.CurrentReadingEntity = new EntityDeserializer.EntData?(entData);
        this.LoadEntity(entityUid, this._metaQuery.Comp(entityUid), entData.Components, entData.MissingComponents);
      }
      catch (Exception ex)
      {
        this.ToDelete.Add(entityUid);
        this._log.Error($"Encountered error while loading entity. Yaml uid: {entData.YamlId}. Loaded loaded entity: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) entityUid)}. Error:\n{ex}.");
      }
    }
    this.CurrentReadingEntity = new EntityDeserializer.EntData?();
    this._log.Debug($"Loaded {this.Entities.Count} entities in {this._stopwatch.Elapsed}");
  }

  private void LoadEntity(
    EntityUid uid,
    MetaDataComponent meta,
    Dictionary<string, MappingDataNode>? comps,
    HashSet<string>? missingComps)
  {
    Robust.Shared.Prototypes.EntityPrototype entityPrototype = meta.EntityPrototype;
    this._components.Clear();
    string key4;
    MappingDataNode mappingDataNode4;
    if (comps != null)
    {
      this._components.EnsureCapacity(comps.Count);
      foreach ((key4, mappingDataNode4) in comps)
      {
        string str = key4;
        MappingDataNode child = mappingDataNode4;
        if (!this._factory.TryGetRegistration(str, out ComponentRegistration _))
        {
          if (!this._factory.IsIgnored(str))
            this._log.Error($"Encountered unregistered component ({str}) while loading entity {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
        }
        else
        {
          MappingDataNode mappingDataNode3 = child;
          Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry componentRegistryEntry;
          if (entityPrototype != null && entityPrototype.Components.TryGetValue(str, out componentRegistryEntry))
            mappingDataNode3 = this._seriMan.CombineMappings(child, componentRegistryEntry.Mapping);
          this._components.Add(str, mappingDataNode3);
        }
      }
    }
    if (entityPrototype != null)
    {
      Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry componentRegistryEntry2;
      foreach ((key4, componentRegistryEntry2) in (Dictionary<string, Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry>) entityPrototype.Components)
      {
        string str = key4;
        Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry componentRegistryEntry3 = componentRegistryEntry2;
        if ((missingComps == null || !missingComps.Contains(str)) && !this._components.ContainsKey(str))
        {
          this.CurrentComponent = str;
          ComponentRegistration registration = this._factory.GetRegistration(str);
          IComponent component1;
          if (!this.EntMan.TryGetComponent(uid, registration.Idx, out component1))
          {
            IComponent component2 = this._factory.GetComponent(registration);
            this.EntMan.AddComponent<IComponent>(uid, component2, false, (MetaDataComponent) null);
            component1 = component2;
          }
          this._seriMan.CopyTo<IComponent>(componentRegistryEntry3.Component, ref component1, (ISerializationContext) this, notNullableOverride: true);
          if (!componentRegistryEntry3.Component.NetSyncEnabled)
          {
            ushort? netId = registration.NetID;
            if (netId.HasValue)
            {
              ushort valueOrDefault = netId.GetValueOrDefault();
              meta.NetComponents.Remove(valueOrDefault);
            }
          }
        }
      }
    }
    foreach ((key4, mappingDataNode4) in this._components)
    {
      string componentName = key4;
      MappingDataNode node = mappingDataNode4;
      this.CurrentComponent = componentName;
      ComponentRegistration registration = this._factory.GetRegistration(componentName);
      IComponent component3;
      if (!this.EntMan.TryGetComponent(uid, registration.Idx, out component3))
      {
        IComponent component4 = (IComponent) this._seriMan.Read(registration.Type, (DataNode) node, (ISerializationContext) this);
        if (component4 is ISerializationHooks)
        {
          component3 = this._factory.GetComponent(registration);
          this.EntMan.AddComponent<IComponent>(uid, component3, false, (MetaDataComponent) null);
          this._seriMan.CopyTo<IComponent>(component4, ref component3, (ISerializationContext) this, notNullableOverride: true);
        }
        else
        {
          this._deps.InjectDependencies((object) component4);
          this.EntMan.AddComponent<IComponent>(uid, component4, false, (MetaDataComponent) null);
        }
      }
      else
        this._seriMan.CopyTo<IComponent>((IComponent) this._seriMan.Read(registration.Type, (DataNode) node, (ISerializationContext) this), ref component3, (ISerializationContext) this, notNullableOverride: true);
    }
    this._components.Clear();
    this.CurrentComponent = (string) null;
    if (missingComps == null || missingComps.Count <= 0)
      return;
    meta.LastComponentRemoved = this.Timing.CurTick;
  }

  private void GetRootEntities()
  {
    if (this.Result.Version < 7)
    {
      this.GetRootEntitiesFallback();
    }
    else
    {
      foreach (int mapYamlId in this.MapYamlIds)
      {
        EntityUid uid;
        MapComponent component;
        if (this.UidMap.TryGetValue(mapYamlId, out uid) && this._mapQuery.TryComp(uid, out component))
        {
          this.Result.Maps.Add((Entity<MapComponent>) (uid, component));
          this.EntMan.EnsureComponent<LoadedMapComponent>(uid);
        }
        else
          this._log.Error($"Missing map entity: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}. YamlId: {mapYamlId}");
      }
      foreach (int gridYamlId in this.GridYamlIds)
      {
        EntityUid uid;
        MapGridComponent component;
        if (this.UidMap.TryGetValue(gridYamlId, out uid) && this._gridQuery.TryComp(uid, out component))
          this.Result.Grids.Add((Entity<MapGridComponent>) (uid, component));
        else
          this._log.Error($"Missing grid entity: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}. YamlId: {gridYamlId}");
      }
      foreach (int orphanYamlId in this.OrphanYamlIds)
      {
        EntityUid uid;
        if (!this.UidMap.TryGetValue(orphanYamlId, out uid))
          this._log.Error($"Missing orphan entity with YamlId: {orphanYamlId}");
        else if (this._mapQuery.HasComponent(uid) || this._xformQuery.Comp(uid).ParentUid.IsValid())
          this._log.Error($"Entity {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)} was incorrectly labelled as an orphan? YamlId: {orphanYamlId}");
        else
          this.Result.Orphans.Add(uid);
      }
      foreach (int nullspaceYamlId in this.NullspaceYamlIds)
      {
        EntityUid uid;
        if (!this.UidMap.TryGetValue(nullspaceYamlId, out uid))
          this._log.Error($"Missing nullspace entity with YamlId: {nullspaceYamlId}");
        else if (this._mapQuery.HasComponent(uid) || this._xformQuery.Comp(uid).ParentUid.IsValid())
          this._log.Error($"Entity {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)} was incorrectly labelled as a null-space entity?");
        else
          this.Result.NullspaceEntities.Add(uid);
      }
    }
  }

  public void AssignMapIds()
  {
    foreach (Entity<MapComponent> map in this.Result.Maps)
      this._map.AssignMapId(map);
  }

  private void GetRootEntitiesFallback()
  {
    foreach (EntityUid entity in this.Result.Entities)
    {
      MapGridComponent component1;
      if (this._gridQuery.TryComp(entity, out component1))
      {
        this.Result.Grids.Add((Entity<MapGridComponent>) (entity, component1));
        if (this._xformQuery.Comp(entity).ParentUid == EntityUid.Invalid && !this._mapQuery.HasComp(entity))
          this.Result.Orphans.Add(entity);
      }
      MapComponent component2;
      if (this._mapQuery.TryComp(entity, out component2))
      {
        this.Result.Maps.Add((Entity<MapComponent>) (entity, component2));
        this.EntMan.EnsureComponent<LoadedMapComponent>(entity);
      }
    }
  }

  private void RemoveEmptyChunks()
  {
    foreach (EntityUid key1 in this.Entities.Keys)
    {
      MapGridComponent component;
      if (this._gridQuery.TryGetComponent(key1, out component))
      {
        foreach ((Vector2i key2, MapChunk mapChunk) in component.Chunks)
        {
          if (mapChunk.FilledTiles <= 0)
          {
            this._log.Warning($"Encountered empty chunk while deserializing map. Grid: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) key1)}. Chunk index: {key2}");
            component.Chunks.Remove(key2);
          }
        }
      }
    }
  }

  private void StoreGridTileMap()
  {
    foreach (Entity<MapGridComponent> grid in this.Result.Grids)
      this.EntMan.EnsureComponent<MapSaveTileMapComponent>((EntityUid) grid).TileMap = this.TileMap.ShallowClone<int, string>();
  }

  private void BuildEntityHierarchy()
  {
    this._stopwatch.Restart();
    HashSet<EntityUid> processed = new HashSet<EntityUid>(this.Result.Entities.Count);
    foreach (EntityUid entity in this.Result.Entities)
      this.BuildEntityHierarchy(entity, processed);
    this._log.Debug($"Built entity hierarchy for {this.Result.Entities.Count} entities in {this._stopwatch.Elapsed}");
  }

  private void CheckCategory()
  {
    if (this.Result.Version < 7)
    {
      this.InferCategory();
    }
    else
    {
      switch (this.Result.Category)
      {
        case FileCategory.Entity:
          if (this.Result.Maps.Count == 0 && this.Result.Grids.Count == 0 && this.Result.Orphans.Count == 1)
            break;
          this._log.Error($"Expected file to contain a orphaned entity, but instead found {this.Result.Orphans.Count} orphans");
          break;
        case FileCategory.Grid:
          if (this.Result.Maps.Count == 0 && this.Result.Grids.Count == 1 && this.Result.Orphans.Count == 1 && this.Result.Orphans.First<EntityUid>() == this.Result.Grids.First<Entity<MapGridComponent>>().Owner)
            break;
          this._log.Error($"Expected file to contain a single grid, but instead found {this.Result.Grids.Count} grids and {this.Result.Orphans.Count} orphans");
          break;
        case FileCategory.Map:
          if (this.Result.Maps.Count == 1 && this.Result.Orphans.Count == 0)
            break;
          this._log.Error($"Expected file to contain a single map, but instead found {this.Result.Maps.Count} maps and {this.Result.Orphans.Count} orphans");
          break;
      }
    }
  }

  private void InferCategory()
  {
    if (this.Result.Category != FileCategory.Unknown)
      return;
    if (this.Result.Maps.Count == 1)
    {
      this.Result.Category = FileCategory.Map;
    }
    else
    {
      if (this.Result.Maps.Count != 0 || this.Result.Grids.Count != 1)
        return;
      this.Result.Category = FileCategory.Grid;
    }
  }

  private void AdoptGrids()
  {
    foreach (Entity<MapGridComponent> grid in this.Result.Grids)
    {
      if (!this._mapQuery.HasComponent(grid.Owner))
      {
        TransformComponent transformComponent = this._xformQuery.Comp(grid.Owner);
        if (!transformComponent.ParentUid.IsValid())
        {
          if (this.Options.LogOrphanedGrids)
            this._log.Error("Encountered an orphaned grid. Automatically creating a map for the grid.");
          Entity<MapComponent, MetaDataComponent> uninitializedMap = this._map.CreateUninitializedMap();
          this._map.AssignMapId((Entity<MapComponent>) uninitializedMap);
          this.Result.Entities.Add((EntityUid) uninitializedMap);
          this.Result.Maps.Add((Entity<MapComponent>) uninitializedMap);
          this.Result.Orphans.Remove(grid.Owner);
          transformComponent._parent = uninitializedMap.Owner;
        }
      }
    }
  }

  private void ValidateMapIds()
  {
    foreach (Entity<MapComponent> map in this.Result.Maps)
    {
      EntityUid? uid;
      if (!(map.Comp.MapId == MapId.Nullspace) && this._map.TryGetMap(new MapId?(map.Comp.MapId), out uid))
      {
        EntityUid? nullable = uid;
        EntityUid owner = map.Owner;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() != owner ? 1 : 0) : 1) == 0)
          continue;
      }
      throw new Exception($"Map entity {this.EntMan.ToPrettyString(new EntityUid?((EntityUid) map), (MetaDataComponent) null)} has not been assigned a map id");
    }
  }

  private void PauseMaps()
  {
    if (!this.Options.PauseMaps)
      return;
    foreach (Entity<MapComponent> map in this.Result.Maps)
      this._map.SetPaused(map, true);
  }

  private void BuildEntityHierarchy(EntityUid uid, HashSet<EntityUid> processed)
  {
    TransformComponent component;
    if (!processed.Add(uid) || !this._xformQuery.TryComp(uid, out component))
      return;
    EntityUid parentUid = component.ParentUid;
    if (parentUid != EntityUid.Invalid)
      this.BuildEntityHierarchy(parentUid, processed);
    if (!this.Result.Entities.Contains(uid))
      return;
    this.SortedEntities.Add(uid);
  }

  private void StartEntitiesInternal()
  {
    this._stopwatch.Restart();
    foreach (EntityUid sortedEntity in this.SortedEntities)
      this.StartupEntity(sortedEntity, this._metaQuery.GetComponent(sortedEntity));
    this._log.Debug($"Started up {this.Result.Entities.Count} entities in {this._stopwatch.Elapsed}");
  }

  private void StartupEntity(EntityUid uid, MetaDataComponent metadata)
  {
    this.ResetNetTicks(uid, metadata);
    this.EntMan.InitializeEntity(uid, metadata);
    this.EntMan.StartEntity(uid);
  }

  private void ResetNetTicks(EntityUid uid, MetaDataComponent metadata)
  {
    EntityDeserializer.EntData entData;
    if (!this.Entities.TryGetValue(uid, out entData))
      return;
    Robust.Shared.Prototypes.EntityPrototype entityPrototype = metadata.EntityPrototype;
    if (entityPrototype == null || entData.Components == null)
      return;
    foreach (IComponent component in metadata.NetComponents.Values)
    {
      string componentName = this._factory.GetComponentName(component.GetType());
      if (!entData.Components.ContainsKey(componentName))
        component.ClearTicks();
      else if (entityPrototype.Components.ContainsKey(componentName))
        component.ClearCreationTick();
    }
  }

  private void SetMapInitLifestage()
  {
    if (this.PostMapInit.Count == 0)
      return;
    this._stopwatch.Restart();
    foreach (EntityUid uid in this.PostMapInit)
    {
      MetaDataComponent component;
      if (this._metaQuery.TryComp(uid, out component))
        this.EntMan.SetLifeStage(component, EntityLifeStage.MapInitialized);
    }
    this._log.Debug($"Finished flagging mapinit in {this._stopwatch.Elapsed}");
  }

  private void SetPaused()
  {
    if (this.Paused.Count == 0)
      return;
    this._stopwatch.Restart();
    TimeSpan curTime = this.Timing.CurTime;
    EntityPausedEvent args = new EntityPausedEvent();
    foreach (EntityUid uid in this.Paused)
    {
      MetaDataComponent component;
      if (this._metaQuery.TryComp(uid, out component))
      {
        component.PauseTime = new TimeSpan?(curTime);
        this.EntMan.EventBus.RaiseLocalEvent<EntityPausedEvent>(uid, ref args);
      }
    }
    this._log.Debug($"Finished setting PauseTime in {this._stopwatch.Elapsed}");
  }

  private void InitializeMaps()
  {
    if (!this.Options.InitializeMaps)
    {
      if (this.Options.PauseMaps)
        return;
      foreach (Entity<MapComponent> map in this.Result.Maps)
      {
        if (this._metaQuery.Comp(map.Owner).EntityLifeStage < EntityLifeStage.MapInitialized)
          this._map.SetPaused(map, true);
      }
    }
    else
    {
      foreach (Entity<MapComponent> map in this.Result.Maps)
      {
        if (!map.Comp.MapInitialized)
          this._map.InitializeMap(map, !this.Options.PauseMaps);
      }
    }
  }

  private void ProcessDeletions()
  {
    foreach (EntityUid entityUid in this.ToDelete)
    {
      this.EntMan.DeleteEntity(new EntityUid?(entityUid));
      this.Result.Entities.Remove(entityUid);
    }
  }

  private void GetRootNodes()
  {
    this.Result.RootNodes.UnionWith((IEnumerable<EntityUid>) this.Result.Orphans);
    this.Result.RootNodes.UnionWith((IEnumerable<EntityUid>) this.Result.NullspaceEntities);
    foreach (Entity<MapComponent> map in this.Result.Maps)
      this.Result.RootNodes.Add(map.Owner);
  }

  ValidationNode ITypeValidator<EntityUid, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    if (node.Value == "invalid")
      return (ValidationNode) new ValidatedValueNode((DataNode) node);
    return !int.TryParse(node.Value, out int _) ? (ValidationNode) new ErrorNode((DataNode) node, "Invalid EntityUid") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  DataNode ITypeWriter<EntityUid>.Write(
    ISerializationManager serializationManager,
    EntityUid value,
    IDependencyCollection dependencies,
    bool alwaysWrite,
    ISerializationContext? context)
  {
    return !value.IsValid() ? (DataNode) new ValueDataNode("invalid") : (DataNode) new ValueDataNode(value.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  EntityUid ITypeReader<EntityUid, ValueDataNode>.Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<EntityUid>? _)
  {
    if (node.Value == "invalid")
    {
      if (this.CurrentComponent == "Transform" || !this.Options.LogInvalidEntities)
        return EntityUid.Invalid;
      EntityDeserializer.EntData? currentReadingEntity = this.CurrentReadingEntity;
      string message;
      if (currentReadingEntity.HasValue)
        message = $"Encountered invalid EntityUid reference while reading entity {currentReadingEntity.GetValueOrDefault().YamlId}, component: {this.CurrentComponent}";
      else
        message = "Encountered invalid EntityUid reference";
      this._log.Error(message);
      return EntityUid.Invalid;
    }
    int result;
    EntityUid entityUid;
    if (int.TryParse(node.Value, out result) && this.UidMap.TryGetValue(result, out entityUid))
      return entityUid;
    EntityDeserializer.EntData? currentReadingEntity1 = this.CurrentReadingEntity;
    string message1;
    if (currentReadingEntity1.HasValue)
      message1 = $"Encountered unknown entity yaml uid while reading entity {currentReadingEntity1.GetValueOrDefault().YamlId}, component: {this.CurrentComponent}";
    else
      message1 = "Encountered unknown entity yaml uid";
    this._log.Error(message1);
    return EntityUid.Invalid;
  }

  ValidationNode ITypeValidator<NetEntity, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    if (node.Value == "invalid")
      return (ValidationNode) new ValidatedValueNode((DataNode) node);
    return !int.TryParse(node.Value, out int _) ? (ValidationNode) new ErrorNode((DataNode) node, "Invalid NetEntity") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  NetEntity ITypeReader<NetEntity, ValueDataNode>.Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<NetEntity>? instanceProvider)
  {
    EntityUid uid = serializationManager.Read<EntityUid>((DataNode) node, context);
    NetEntity? netEntity;
    if (this.EntMan.TryGetNetEntity(uid, out netEntity, (MetaDataComponent) null))
      return netEntity.Value;
    this._log.Error($"Failed to get NetEntity entity {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    return NetEntity.Invalid;
  }

  DataNode ITypeWriter<NetEntity>.Write(
    ISerializationManager serializationManager,
    NetEntity value,
    IDependencyCollection dependencies,
    bool alwaysWrite,
    ISerializationContext? context)
  {
    return !value.IsValid() ? (DataNode) new ValueDataNode("invalid") : (DataNode) new ValueDataNode(value.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  ValidationNode ITypeValidator<MapId, ValueDataNode>.Validate(
    ISerializationManager seri,
    ValueDataNode node,
    IDependencyCollection deps,
    ISerializationContext? context)
  {
    return seri.ValidateNode<EntityUid>((DataNode) node, context);
  }

  MapId ITypeReader<MapId, ValueDataNode>.Read(
    ISerializationManager seri,
    ValueDataNode node,
    IDependencyCollection deps,
    SerializationHookContext hookCtx,
    ISerializationContext? ctx,
    ISerializationManager.InstantiationDelegate<MapId>? instanceProvider)
  {
    if (!this.Options.AssignMapIds || this.Result.Version < 7)
    {
      this._log.Error("Cannot deserialize map ids without pre-allocated ids");
      return MapId.Nullspace;
    }
    int result;
    MapId mapId;
    if (int.TryParse(node.Value, out result) && this.AllocatedMapIds.TryGetValue(result, out mapId))
      return mapId;
    EntityDeserializer.EntData? currentReadingEntity = this.CurrentReadingEntity;
    string message;
    if (currentReadingEntity.HasValue)
      message = $"Encountered unknown yaml map id while reading entity {currentReadingEntity.GetValueOrDefault().YamlId}, component: {this.CurrentComponent}";
    else
      message = "Encountered unknown yaml map id";
    this._log.Error(message);
    return MapId.Nullspace;
  }

  DataNode ITypeWriter<MapId>.Write(
    ISerializationManager seri,
    MapId value,
    IDependencyCollection deps,
    bool alwaysWrite,
    ISerializationContext? ctx)
  {
    return seri.WriteValue<EntityUid>(this._map.GetMapOrInvalid(new MapId?(value)), alwaysWrite, ctx);
  }

  public record struct EntData(
    int YamlId,
    MappingDataNode Node,
    Dictionary<string, MappingDataNode>? Components,
    HashSet<string>? MissingComponents,
    bool PostInit,
    bool Paused,
    bool ToDelete)
  ;
}
