// Decompiled with JetBrains decompiler
// Type: Robust.Shared.EntitySerialization.EntitySerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;
using Robust.Shared.EntitySerialization.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.EntitySerialization;

public sealed class EntitySerializer : 
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
  public const int MapFormatVersion = 7;
  [Robust.Shared.IoC.Dependency]
  public readonly EntityManager EntMan;
  [Robust.Shared.IoC.Dependency]
  public readonly IGameTiming Timing;
  [Robust.Shared.IoC.Dependency]
  private readonly IComponentFactory _factory;
  [Robust.Shared.IoC.Dependency]
  private readonly ISerializationManager _serialization;
  [Robust.Shared.IoC.Dependency]
  private readonly ITileDefinitionManager _tileDef;
  [Robust.Shared.IoC.Dependency]
  private readonly IConfigurationManager _conf;
  [Robust.Shared.IoC.Dependency]
  private readonly ILogManager _logMan;
  [Robust.Shared.IoC.Dependency]
  private readonly SharedMapSystem _map;
  private readonly ISawmill _log;
  public readonly Dictionary<EntityUid, int> YamlUidMap = new Dictionary<EntityUid, int>();
  public readonly HashSet<int> YamlIds = new HashSet<int>();
  public readonly ValueDataNode InvalidNode = new ValueDataNode("invalid");
  private readonly Dictionary<int, int> _tileMap = new Dictionary<int, int>();
  private readonly HashSet<int> _yamlTileIds = new HashSet<int>();
  public readonly HashSet<EntityUid> Truncated = new HashSet<EntityUid>();
  public readonly SerializationOptions Options;
  public readonly Dictionary<string, Dictionary<string, MappingDataNode>> PrototypeCache = new Dictionary<string, Dictionary<string, MappingDataNode>>();
  public readonly Dictionary<int, (EntityUid Uid, MappingDataNode Node)> EntityData = new Dictionary<int, (EntityUid, MappingDataNode)>();
  public readonly Dictionary<string, List<int>> Prototypes = new Dictionary<string, List<int>>();
  public HashSet<EntityUid> ErroringEntities = new HashSet<EntityUid>();
  public readonly List<int> Maps = new List<int>();
  public readonly List<int> Nullspace = new List<int>();
  public readonly List<int> Grids = new List<int>();
  public readonly List<int> Orphans = new List<int>();
  private readonly string _metaName;
  private readonly string _xformName;
  private readonly MappingDataNode _emptyMetaNode;
  private readonly MappingDataNode _emptyXformNode;
  private int _nextYamlUid = 1;
  private int _nextYamlTileId;
  private readonly List<EntityUid> _autoInclude = new List<EntityUid>();
  private readonly EntityQuery<YamlUidComponent> _yamlQuery;
  private readonly EntityQuery<MapGridComponent> _gridQuery;
  private readonly EntityQuery<MapComponent> _mapQuery;
  private readonly EntityQuery<MetaDataComponent> _metaQuery;
  private readonly EntityQuery<TransformComponent> _xformQuery;

  public SerializationManager.SerializerProvider SerializerProvider { get; } = new SerializationManager.SerializerProvider();

  public string? CurrentComponent { get; private set; }

  public Entity<MetaDataComponent>? CurrentEntity { get; private set; }

  public int CurrentEntityYamlUid { get; private set; }

  public bool WritingReadingPrototypes { get; private set; }

  public EntityUid Truncate { get; private set; }

  public event EntitySerializer.IsSerializableDelegate? OnIsSerializeable;

  public EntitySerializer(IDependencyCollection dependency, SerializationOptions options)
  {
    dependency.InjectDependencies((object) this);
    this._log = this._logMan.GetSawmill("entity_serializer");
    this.SerializerProvider.RegisterSerializer((object) this);
    this._metaName = this._factory.GetComponentName<MetaDataComponent>();
    this._xformName = this._factory.GetComponentName<TransformComponent>();
    this._emptyMetaNode = this._serialization.WriteValueAs<MappingDataNode>(typeof (MetaDataComponent), (object) new MetaDataComponent(), true, (ISerializationContext) this);
    this.CurrentComponent = this._xformName;
    this._emptyXformNode = this._serialization.WriteValueAs<MappingDataNode>(typeof (TransformComponent), (object) new TransformComponent(), true, (ISerializationContext) this);
    this.CurrentComponent = (string) null;
    this._yamlQuery = this.EntMan.GetEntityQuery<YamlUidComponent>();
    this._gridQuery = this.EntMan.GetEntityQuery<MapGridComponent>();
    this._mapQuery = this.EntMan.GetEntityQuery<MapComponent>();
    this._metaQuery = this.EntMan.GetEntityQuery<MetaDataComponent>();
    this._xformQuery = this.EntMan.GetEntityQuery<TransformComponent>();
    this.Options = options;
  }

  public bool IsSerializable(Entity<MetaDataComponent?> ent)
  {
    if (ent.Comp == null && !this.EntMan.TryGetComponent<MetaDataComponent>(ent.Owner, out ent.Comp))
      return false;
    Robust.Shared.Prototypes.EntityPrototype entityPrototype = ent.Comp.EntityPrototype;
    if ((entityPrototype != null ? (!entityPrototype.MapSavable ? 1 : 0) : 0) != 0)
      return false;
    bool serializable = true;
    EntitySerializer.IsSerializableDelegate onIsSerializeable = this.OnIsSerializeable;
    if (onIsSerializeable != null)
      onIsSerializeable(ent, ref serializable);
    return serializable;
  }

  public void SerializeEntity(EntityUid uid)
  {
    if (!this.IsSerializable((Entity<MetaDataComponent>) uid))
      throw new Exception($"{this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)} is not serializable");
    this.ReserveYamlId(uid);
    this.SerializeEntityInternal(uid);
    if (this._autoInclude.Count == 0)
      return;
    this.ProcessAutoInclude();
  }

  public void SerializeEntities(HashSet<EntityUid> entities)
  {
    foreach (EntityUid entity in entities)
    {
      if (!this.IsSerializable((Entity<MetaDataComponent>) entity))
        throw new Exception($"{this.EntMan.ToPrettyString((Entity<MetaDataComponent>) entity)} is not serializable");
    }
    this.ReserveYamlIds(entities);
    this.SerializeEntitiesInternal(entities);
  }

  public void SerializeEntityRecursive(EntityUid root)
  {
    if (!this.IsSerializable((Entity<MetaDataComponent>) root))
      throw new Exception($"{this.EntMan.ToPrettyString((Entity<MetaDataComponent>) root)} is not serializable");
    this.Truncate = this._xformQuery.GetComponent(root).ParentUid;
    this.Truncated.Add(this.Truncate);
    this.InitializeTileMap(root);
    HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>();
    this.RecursivelyIncludeChildren(root, entityUidSet);
    this.ReserveYamlIds(entityUidSet);
    this.SerializeEntitiesInternal(entityUidSet);
    this.Truncate = EntityUid.Invalid;
  }

  public void SerializeEntityRecursive(HashSet<EntityUid> roots)
  {
    if (roots.Count == 0)
      return;
    this.InitializeTileMap(roots.First<EntityUid>());
    HashSet<EntityUid> entities1 = new HashSet<EntityUid>();
    List<(EntityUid, HashSet<EntityUid>)> valueTupleList = new List<(EntityUid, HashSet<EntityUid>)>();
    foreach (EntityUid root in roots)
    {
      if (!this.IsSerializable((Entity<MetaDataComponent>) root))
        throw new Exception($"{this.EntMan.ToPrettyString((Entity<MetaDataComponent>) root)} is not serializable");
      HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>();
      this.RecursivelyIncludeChildren(root, entityUidSet);
      valueTupleList.Add((root, entityUidSet));
      entities1.UnionWith((IEnumerable<EntityUid>) entityUidSet);
    }
    this.ReserveYamlIds(entities1);
    foreach ((EntityUid uid, HashSet<EntityUid> entities2) in valueTupleList)
    {
      this.Truncate = this._xformQuery.GetComponent(uid).ParentUid;
      this.Truncated.Add(this.Truncate);
      this.SerializeEntitiesInternal(entities2);
      this.Truncate = EntityUid.Invalid;
    }
  }

  private void InitializeTileMap(EntityUid root)
  {
    Dictionary<int, string> map;
    if (!this.FindSavedTileMap(root, out map))
      return;
    foreach ((int key, string name) in map)
    {
      ITileDefinition definition;
      if (this._tileDef.TryGetDefinition(name, out definition))
      {
        this._tileMap.TryAdd((int) definition.TileId, key);
        this._yamlTileIds.Add(key);
      }
    }
  }

  private bool FindSavedTileMap(EntityUid root, [NotNullWhen(true)] out Dictionary<int, string>? map)
  {
    MapSaveTileMapComponent component1;
    if (this.EntMan.TryGetComponent<MapSaveTileMapComponent>(root, out component1))
    {
      map = component1.TileMap;
      return true;
    }
    map = (Dictionary<int, string>) null;
    if (!this._mapQuery.HasComponent(root))
      return false;
    foreach (EntityUid child in this._xformQuery.GetComponent(root)._children)
    {
      MapSaveTileMapComponent component2;
      if (this.EntMan.TryGetComponent<MapSaveTileMapComponent>(child, out component2))
      {
        map = component2.TileMap;
        return true;
      }
    }
    return false;
  }

  private void ProcessAutoInclude()
  {
    HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>();
    switch (this.Options.MissingEntityBehaviour)
    {
      case MissingEntityBehaviour.IncludeNullspace:
      case MissingEntityBehaviour.AutoInclude:
        HashSet<EntityUid> ents = new HashSet<EntityUid>();
        foreach (EntityUid uid in this._autoInclude)
          this.GetRootNode(uid, ents);
        using (HashSet<EntityUid>.Enumerator enumerator = ents.GetEnumerator())
        {
          while (enumerator.MoveNext())
            this.RecursivelyIncludeChildren(enumerator.Current, entityUidSet);
          break;
        }
      case MissingEntityBehaviour.PartialInclude:
        using (List<EntityUid>.Enumerator enumerator = this._autoInclude.GetEnumerator())
        {
          while (enumerator.MoveNext())
            this.RecursivelyIncludeParents(enumerator.Current, entityUidSet);
          break;
        }
      default:
        throw new ArgumentOutOfRangeException();
    }
    this._autoInclude.Clear();
    this.SerializeEntitiesInternal(entityUidSet);
  }

  private void RecursivelyIncludeChildren(EntityUid uid, HashSet<EntityUid> ents)
  {
    if (!this.IsSerializable((Entity<MetaDataComponent>) uid))
      return;
    ents.Add(uid);
    foreach (EntityUid child in this._xformQuery.GetComponent(uid)._children)
      this.RecursivelyIncludeChildren(child, ents);
  }

  private void GetRootNode(EntityUid uid, HashSet<EntityUid> ents)
  {
    TransformComponent transformComponent = this.IsSerializable((Entity<MetaDataComponent>) uid) ? this._xformQuery.GetComponent(uid) : throw new NotSupportedException($"Attempted to auto-include an unserializable entity: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    while (transformComponent.ParentUid.IsValid() && transformComponent.ParentUid != this.Truncate)
    {
      uid = transformComponent.ParentUid;
      transformComponent = this._xformQuery.GetComponent(uid);
      if (!this.IsSerializable((Entity<MetaDataComponent>) uid))
        throw new NotSupportedException($"Encountered an un-serializable parent entity: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    }
    ents.Add(uid);
  }

  private void RecursivelyIncludeParents(EntityUid uid, HashSet<EntityUid> ents)
  {
    for (; uid.IsValid() && uid != this.Truncate && ents.Add(uid); uid = this._xformQuery.GetComponent(uid).ParentUid)
    {
      if (!this.IsSerializable((Entity<MetaDataComponent>) uid))
        throw new NotSupportedException($"Encountered an un-serializable parent entity: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    }
  }

  private void SerializeEntitiesInternal(HashSet<EntityUid> entities)
  {
    foreach (EntityUid entity in entities)
      this.SerializeEntityInternal(entity);
    if (this._autoInclude.Count == 0)
      return;
    this.ProcessAutoInclude();
  }

  private void SerializeEntityInternal(EntityUid uid)
  {
    int yamlUid = this.GetYamlUid(uid);
    if (this.EntityData.ContainsKey(yamlUid))
      return;
    MetaDataComponent component1 = this._metaQuery.GetComponent(uid);
    string key1 = component1.EntityPrototype?.ID ?? string.Empty;
    EntityLifeStage entityLifeStage = component1.EntityLifeStage;
    if (entityLifeStage > EntityLifeStage.Initializing)
    {
      if (entityLifeStage >= EntityLifeStage.Terminating)
        this._log.Error($"Encountered terminating or deleted entity: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    }
    else
      this._log.Error($"Encountered an uninitialized entity: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    this.CurrentEntityYamlUid = yamlUid;
    this.CurrentEntity = new Entity<MetaDataComponent>?((Entity<MetaDataComponent>) (uid, component1));
    this.Prototypes.GetOrNew<string, List<int>>(key1).Add(yamlUid);
    TransformComponent component2 = this._xformQuery.GetComponent(uid);
    if (this._mapQuery.HasComp(uid))
      this.Maps.Add(yamlUid);
    else if (component2.ParentUid == EntityUid.Invalid)
      this.Nullspace.Add(yamlUid);
    if (this._gridQuery.HasComp(uid))
      this.Grids.Add(yamlUid);
    MappingDataNode mapping1 = new MappingDataNode();
    mapping1.Add(nameof (uid), yamlUid.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    MappingDataNode mapping2 = mapping1;
    this.EntityData[yamlUid] = (uid, mapping2);
    Dictionary<string, MappingDataNode> protoCache = this.GetProtoCache(component1.EntityPrototype);
    if (component1.EntityLifeStage == EntityLifeStage.MapInitialized)
    {
      if (this.Options.ExpectPreInit)
        this._log.Error($"Expected all entities to be pre-mapinit, but encountered post-init entity: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
      mapping2.Add("mapInit", "true");
      if (component1.EntityPaused)
        mapping2.Add("paused", "true");
    }
    else if (!component1.EntityPaused)
      mapping2.Add("paused", "false");
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    if (component2.NoLocalRotation && Angle.op_Inequality(component2.LocalRotation, Angle.op_Implicit(0.0f)))
    {
      this._log.Error($"Encountered a no-rotation entity with non-zero local rotation: {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
      component2._localRotation = Angle.op_Implicit(0.0f);
    }
    try
    {
      this.SerializeComponents(uid, protoCache, sequenceDataNode);
    }
    catch (Exception ex)
    {
      if (this.Options.EntityExceptionBehaviour == EntityExceptionBehaviour.Rethrow)
      {
        this._log.Error($"Caught exception while serializing component {this.CurrentComponent} of entity {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}");
        throw;
      }
      this._log.Error($"Caught exception while serializing component {this.CurrentComponent} of entity {this.EntMan.ToPrettyString((Entity<MetaDataComponent>) uid)}:\n{ex}");
      this.CurrentEntityYamlUid = 0;
      this.CurrentEntity = new Entity<MetaDataComponent>?();
      this.CurrentComponent = (string) null;
      this.RemoveErroringEntity(uid);
      return;
    }
    this.CurrentComponent = (string) null;
    if (sequenceDataNode.Count != 0)
      mapping2.Add("components", (DataNode) sequenceDataNode);
    if (component1.EntityPrototype == null)
    {
      this.CurrentEntityYamlUid = 0;
      this.CurrentEntity = new Entity<MetaDataComponent>?();
    }
    else
    {
      SequenceDataNode node = (SequenceDataNode) null;
      foreach ((string key2, Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry componentRegistryEntry) in (Dictionary<string, Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry>) component1.EntityPrototype.Components)
      {
        if (!this.EntMan.TryGetComponent(uid, componentRegistryEntry.Component.GetType(), out IComponent _))
        {
          if (node == null)
            node = new SequenceDataNode();
          node.Add((DataNode) new ValueDataNode(key2));
        }
      }
      if (node != null)
        mapping2.Add("missingComponents", (DataNode) node);
      this.CurrentEntityYamlUid = 0;
      this.CurrentEntity = new Entity<MetaDataComponent>?();
    }
  }

  private void RemoveErroringEntity(EntityUid uid)
  {
    if (this.Options.EntityExceptionBehaviour == EntityExceptionBehaviour.IgnoreEntityAndChildren)
    {
      foreach (EntityUid child in this._xformQuery.GetComponent(uid)._children)
        this.RemoveErroringEntity(child);
    }
    this.ErroringEntities.Add(uid);
    int key;
    if (!this.YamlUidMap.TryGetValue(uid, out key))
      return;
    this.Nullspace.Remove(key);
    this.Orphans.Remove(key);
    this.Maps.Remove(key);
    this.Grids.Remove(key);
    this.EntityData.Remove(key);
    MetaDataComponent component;
    List<int> intList;
    if (!this._metaQuery.TryGetComponent(uid, out component) || component.EntityPrototype == null || !this.Prototypes.TryGetValue(component.EntityPrototype.ID, out intList))
      return;
    intList.Remove(key);
  }

  private void SerializeComponents(
    EntityUid uid,
    Dictionary<string, MappingDataNode>? cache,
    SequenceDataNode components)
  {
    foreach (IComponent component in (IEnumerable<IComponent>) this.EntMan.GetComponentsInternal(uid))
    {
      Type type = component.GetType();
      ComponentRegistration registration = this._factory.GetRegistration(type);
      if (!registration.Unsaved)
      {
        this.CurrentComponent = registration.Name;
        MappingDataNode node1 = (MappingDataNode) null;
        MappingDataNode node2;
        if (cache != null && cache.TryGetValue(registration.Name, out node1))
        {
          node2 = this._serialization.WriteValueAs<MappingDataNode>(type, (object) component, true, (ISerializationContext) this).Except(node1);
          if (node2 == null)
            continue;
        }
        else
          node2 = this._serialization.WriteValueAs<MappingDataNode>(type, (object) component, context: (ISerializationContext) this);
        if (node2.Children.Count != 0 || node1 == null)
        {
          node2.InsertAt(0, "type", (DataNode) new ValueDataNode(registration.Name));
          components.Add((DataNode) node2);
        }
      }
    }
  }

  private Dictionary<string, MappingDataNode>? GetProtoCache(Robust.Shared.Prototypes.EntityPrototype? proto)
  {
    if (proto == null)
      return (Dictionary<string, MappingDataNode>) null;
    Dictionary<string, MappingDataNode> protoCache;
    if (this.PrototypeCache.TryGetValue(proto.ID, out protoCache))
      return protoCache;
    this.PrototypeCache[proto.ID] = protoCache = new Dictionary<string, MappingDataNode>(proto.Components.Count);
    this.WritingReadingPrototypes = true;
    foreach ((string key, Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry componentRegistryEntry) in (Dictionary<string, Robust.Shared.Prototypes.EntityPrototype.ComponentRegistryEntry>) proto.Components)
    {
      this.CurrentComponent = key;
      protoCache.Add(key, this._serialization.WriteValueAs<MappingDataNode>(componentRegistryEntry.Component.GetType(), (object) componentRegistryEntry.Component, true, (ISerializationContext) this));
    }
    this.CurrentComponent = (string) null;
    this.WritingReadingPrototypes = false;
    protoCache.TryAdd(this._metaName, this._emptyMetaNode);
    protoCache.TryAdd(this._xformName, this._emptyXformNode);
    return protoCache;
  }

  public MappingDataNode Write()
  {
    return new MappingDataNode()
    {
      {
        "meta",
        (DataNode) this.WriteMetadata()
      },
      {
        "maps",
        (DataNode) this.WriteIds(this.Maps)
      },
      {
        "grids",
        (DataNode) this.WriteIds(this.Grids)
      },
      {
        "orphans",
        (DataNode) this.WriteIds(this.Orphans)
      },
      {
        "nullspace",
        (DataNode) this.WriteIds(this.Nullspace)
      },
      {
        "tilemap",
        (DataNode) this.WriteTileMap()
      },
      {
        "entities",
        (DataNode) this.WriteEntitySection()
      }
    };
  }

  public MappingDataNode WriteMetadata()
  {
    MappingDataNode mapping = new MappingDataNode();
    mapping.Add("format", 7.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    mapping.Add("category", this.GetCategory().ToString());
    mapping.Add("engineVersion", this._conf.GetCVar<string>(CVars.BuildEngineVersion));
    mapping.Add("forkId", this._conf.GetCVar<string>(CVars.BuildForkId));
    mapping.Add("forkVersion", this._conf.GetCVar<string>(CVars.BuildVersion));
    mapping.Add("time", DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    mapping.Add("entityCount", this.EntityData.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    return mapping;
  }

  public SequenceDataNode WriteIds(List<int> ids)
  {
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    foreach (int id in ids)
      sequenceDataNode.Add((DataNode) new ValueDataNode(id.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
    return sequenceDataNode;
  }

  public MappingDataNode WriteTileMap()
  {
    MappingDataNode mapping = new MappingDataNode();
    foreach ((int num1, int num2) in (IEnumerable<KeyValuePair<int, int>>) this._tileMap.OrderBy<KeyValuePair<int, int>, int>((Func<KeyValuePair<int, int>, int>) (x => x.Key)))
    {
      ITileDefinition definition;
      if (!this._tileDef.TryGetDefinition(num1, out definition))
        throw new Exception($"Attempting to serialize a tile {num1} with no valid tile definition.");
      string key = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      mapping.Add(key, definition.ID);
    }
    return mapping;
  }

  public SequenceDataNode WriteEntitySection()
  {
    if (this.Options.EntityExceptionBehaviour != EntityExceptionBehaviour.IgnoreEntity && this.Options.EntityExceptionBehaviour != EntityExceptionBehaviour.IgnoreEntityAndChildren && (this.YamlIds.Count != this.YamlUidMap.Count || this.YamlIds.Count != this.EntityData.Count))
      throw new Exception("Entity count mismatch");
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    List<string> list = this.Prototypes.Keys.ToList<string>();
    list.Sort((IComparer<string>) StringComparer.InvariantCulture);
    foreach (string key1 in list)
    {
      SequenceDataNode node1 = new SequenceDataNode();
      MappingDataNode mapping = new MappingDataNode();
      mapping.Add("proto", key1);
      mapping.Add("entities", (DataNode) node1);
      MappingDataNode node2 = mapping;
      sequenceDataNode.Add((DataNode) node2);
      List<int> prototype = this.Prototypes[key1];
      prototype.Sort();
      foreach (int key2 in prototype)
      {
        MappingDataNode node3 = this.EntityData[key2].Node;
        node1.Add((DataNode) node3);
      }
    }
    return sequenceDataNode;
  }

  public FileCategory GetCategory()
  {
    switch (this.Options.Category)
    {
      case FileCategory.Entity:
        return this.Maps.Count > 0 || this.Grids.Count > 0 || this.Orphans.Count != 1 ? FileCategory.Unknown : FileCategory.Entity;
      case FileCategory.Grid:
        return this.Maps.Count > 0 || this.Grids.Count != 1 ? FileCategory.Unknown : FileCategory.Grid;
      case FileCategory.Map:
        return this.Maps.Count != 1 ? FileCategory.Unknown : FileCategory.Map;
      case FileCategory.Save:
        return FileCategory.Save;
      default:
        if (this.Maps.Count == 1)
        {
          if (this.Orphans.Count == 0)
            return FileCategory.Map;
        }
        else if (this.Grids.Count == 1)
        {
          if (this.Orphans.Count == 1 && this.Grids[0] == this.Orphans[0])
            return FileCategory.Grid;
        }
        else if (this.Orphans.Count == 1)
          return FileCategory.Entity;
        return FileCategory.Unknown;
    }
  }

  public int GetYamlUid(EntityUid uid)
  {
    int num;
    return this.YamlUidMap.TryGetValue(uid, out num) ? num : this.AllocateYamlUid(uid);
  }

  private int AllocateYamlUid(EntityUid uid)
  {
    if (this.Truncated.Contains(uid))
      this._log.Error("Including a previously truncated entity within the serialization process? Something probably wrong");
    while (!this.YamlIds.Add(this._nextYamlUid))
      ++this._nextYamlUid;
    this.YamlUidMap.Add(uid, this._nextYamlUid);
    return this._nextYamlUid++;
  }

  public int GetYamlTileId(int tileId)
  {
    int num;
    return this._tileMap.TryGetValue(tileId, out num) ? num : this.AllocateYamlTileId(tileId);
  }

  private int AllocateYamlTileId(int tileId)
  {
    while (!this._yamlTileIds.Add(this._nextYamlTileId))
      ++this._nextYamlTileId;
    this._tileMap[tileId] = this._nextYamlTileId;
    return this._nextYamlTileId++;
  }

  public void ReserveYamlIds(HashSet<EntityUid> entities)
  {
    List<EntityUid> entityUidList = new List<EntityUid>();
    foreach (EntityUid entity in entities)
    {
      if (!this.YamlUidMap.ContainsKey(entity))
      {
        YamlUidComponent component;
        if (this._yamlQuery.TryGetComponent(entity, out component) && component.Uid > 0 && this.YamlIds.Add(component.Uid))
        {
          if (this.Truncated.Contains(entity))
            this._log.Error("Including a previously truncated entity within the serialization process? Something probably wrong");
          this.YamlUidMap.Add(entity, component.Uid);
        }
        else
          entityUidList.Add(entity);
      }
    }
    foreach (EntityUid uid in entityUidList)
      this.AllocateYamlUid(uid);
  }

  public void ReserveYamlId(EntityUid uid)
  {
    if (this.YamlUidMap.ContainsKey(uid))
      return;
    YamlUidComponent component;
    if (this._yamlQuery.TryGetComponent(uid, out component) && component.Uid > 0 && this.YamlIds.Add(component.Uid))
    {
      if (this.Truncated.Contains(uid))
        this._log.Error("Including a previously truncated entity within the serialization process? Something probably wrong");
      this.YamlUidMap.Add(uid, component.Uid);
    }
    else
      this.AllocateYamlUid(uid);
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

  public DataNode Write(
    ISerializationManager serializationManager,
    EntityUid value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    int num;
    if (this.YamlUidMap.TryGetValue(value, out num))
      return (DataNode) new ValueDataNode(num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    if (this.CurrentComponent == this._xformName)
    {
      if (value == EntityUid.Invalid)
        return (DataNode) this.InvalidNode;
      this.Orphans.Add(this.CurrentEntityYamlUid);
      if (this.Options.ErrorOnOrphan && this.CurrentEntity.HasValue && value != this.Truncate && !this.ErroringEntities.Contains(value))
      {
        ISawmill log = this._log;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 2);
        interpolatedStringHandler.AppendLiteral("Serializing entity ");
        ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
        EntityManager entMan = this.EntMan;
        Entity<MetaDataComponent>? currentEntity = this.CurrentEntity;
        EntityUid? uid = currentEntity.HasValue ? new EntityUid?((EntityUid) currentEntity.GetValueOrDefault()) : new EntityUid?();
        EntityStringRepresentation? prettyString = entMan.ToPrettyString(uid, (MetaDataComponent) null);
        local.AppendFormatted<EntityStringRepresentation?>(prettyString);
        interpolatedStringHandler.AppendLiteral(" without including its parent ");
        interpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntMan.ToPrettyString((Entity<MetaDataComponent>) value));
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        log.Error(stringAndClear);
      }
      return (DataNode) this.InvalidNode;
    }
    if (this.ErroringEntities.Contains(value))
      return (DataNode) this.InvalidNode;
    if (value == EntityUid.Invalid)
    {
      if (this.Options.MissingEntityBehaviour != MissingEntityBehaviour.Ignore)
        this._log.Error("Encountered an invalid entityUid reference.");
      return (DataNode) this.InvalidNode;
    }
    Entity<MetaDataComponent>? currentEntity1;
    if (value == this.Truncate)
    {
      ISawmill log = this._log;
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(63 /*0x3F*/, 3);
      ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
      EntityManager entMan = this.EntMan;
      currentEntity1 = this.CurrentEntity;
      EntityUid? uid = currentEntity1.HasValue ? new EntityUid?((EntityUid) currentEntity1.GetValueOrDefault()) : new EntityUid?();
      EntityStringRepresentation? prettyString = entMan.ToPrettyString(uid, (MetaDataComponent) null);
      local.AppendFormatted<EntityStringRepresentation?>(prettyString);
      interpolatedStringHandler.AppendLiteral(":");
      interpolatedStringHandler.AppendFormatted(this.CurrentComponent);
      interpolatedStringHandler.AppendLiteral(" is attempting to serialize references to a truncated entity ");
      interpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntMan.ToPrettyString((Entity<MetaDataComponent>) this.Truncate));
      interpolatedStringHandler.AppendLiteral(".");
      string stringAndClear = interpolatedStringHandler.ToStringAndClear();
      log.Error(stringAndClear);
    }
    switch (this.Options.MissingEntityBehaviour)
    {
      case MissingEntityBehaviour.Error:
        ISawmill log1 = this._log;
        string stringAndClear1;
        if (!this.EntMan.Deleted(value))
        {
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 2);
          interpolatedStringHandler.AppendLiteral("Encountered a reference to a missing entity: ");
          interpolatedStringHandler.AppendFormatted<EntityUid>(value);
          interpolatedStringHandler.AppendLiteral(" while serializing ");
          ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
          EntityManager entMan = this.EntMan;
          currentEntity1 = this.CurrentEntity;
          EntityUid? uid = currentEntity1.HasValue ? new EntityUid?((EntityUid) currentEntity1.GetValueOrDefault()) : new EntityUid?();
          EntityStringRepresentation? prettyString = entMan.ToPrettyString(uid, (MetaDataComponent) null);
          local.AppendFormatted<EntityStringRepresentation?>(prettyString);
          interpolatedStringHandler.AppendLiteral(".");
          stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
        }
        else
        {
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(64 /*0x40*/, 2);
          interpolatedStringHandler.AppendLiteral("Encountered a reference to a deleted entity ");
          interpolatedStringHandler.AppendFormatted<EntityUid>(value);
          interpolatedStringHandler.AppendLiteral(" while serializing ");
          ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
          EntityManager entMan = this.EntMan;
          currentEntity1 = this.CurrentEntity;
          EntityUid? uid = currentEntity1.HasValue ? new EntityUid?((EntityUid) currentEntity1.GetValueOrDefault()) : new EntityUid?();
          EntityStringRepresentation? prettyString = entMan.ToPrettyString(uid, (MetaDataComponent) null);
          local.AppendFormatted<EntityStringRepresentation?>(prettyString);
          interpolatedStringHandler.AppendLiteral(".");
          stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
        }
        log1.Error(stringAndClear1);
        return (DataNode) this.InvalidNode;
      case MissingEntityBehaviour.Ignore:
        return (DataNode) this.InvalidNode;
      case MissingEntityBehaviour.IncludeNullspace:
        TransformComponent component;
        if (!this.EntMan.TryGetComponent<TransformComponent>(value, out component) || component.ParentUid != EntityUid.Invalid || this._gridQuery.HasComp(value) || this._mapQuery.HasComp(value))
          goto case MissingEntityBehaviour.Error;
        goto case MissingEntityBehaviour.PartialInclude;
      case MissingEntityBehaviour.PartialInclude:
      case MissingEntityBehaviour.AutoInclude:
        LogLevel? logAutoInclude = this.Options.LogAutoInclude;
        if (logAutoInclude.HasValue)
        {
          LogLevel valueOrDefault = logAutoInclude.GetValueOrDefault();
          ISawmill log2 = this._log;
          int level = (int) valueOrDefault;
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
          interpolatedStringHandler.AppendLiteral("Auto-including entity ");
          interpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntMan.ToPrettyString((Entity<MetaDataComponent>) value));
          interpolatedStringHandler.AppendLiteral(" referenced by ");
          ref DefaultInterpolatedStringHandler local = ref interpolatedStringHandler;
          EntityManager entMan = this.EntMan;
          currentEntity1 = this.CurrentEntity;
          EntityUid? uid = currentEntity1.HasValue ? new EntityUid?((EntityUid) currentEntity1.GetValueOrDefault()) : new EntityUid?();
          EntityStringRepresentation? prettyString = entMan.ToPrettyString(uid, (MetaDataComponent) null);
          local.AppendFormatted<EntityStringRepresentation?>(prettyString);
          string stringAndClear2 = interpolatedStringHandler.ToStringAndClear();
          log2.Log((LogLevel) level, stringAndClear2);
        }
        this._autoInclude.Add(value);
        return (DataNode) new ValueDataNode(this.GetYamlUid(value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  EntityUid ITypeReader<EntityUid, ValueDataNode>.Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<EntityUid>? _)
  {
    return !(node.Value == "invalid") ? EntityUid.Parse(node.Value.AsSpan()) : EntityUid.Invalid;
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    if (node.Value == "invalid")
      return (ValidationNode) new ValidatedValueNode((DataNode) node);
    return !int.TryParse(node.Value, out int _) ? (ValidationNode) new ErrorNode((DataNode) node, "Invalid NetEntity") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public NetEntity Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<NetEntity>? instanceProvider = null)
  {
    return !(node.Value == "invalid") ? NetEntity.Parse(node.Value.AsSpan()) : NetEntity.Invalid;
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    NetEntity value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    EntityUid entity = this.EntMan.GetEntity(value);
    return serializationManager.WriteValue<EntityUid>(entity, alwaysWrite, context);
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
    MapComponent component;
    return !this.EntMan.TryGetComponent<MapComponent>(seri.Read<EntityUid>((DataNode) node, ctx), out component) ? MapId.Nullspace : component.MapId;
  }

  DataNode ITypeWriter<MapId>.Write(
    ISerializationManager seri,
    MapId value,
    IDependencyCollection deps,
    bool alwaysWrite,
    ISerializationContext? ctx)
  {
    EntityUid? uid;
    if (this._map.TryGetMap(new MapId?(value), out uid))
      return seri.WriteValue<EntityUid?>(uid, alwaysWrite, ctx);
    this._log.Error($"Attempted to serialize invalid map id {value} while serializing component '{this.CurrentComponent}' on entity '{this.EntMan.ToPrettyString(uid, (MetaDataComponent) null)}'");
    return (DataNode) new ValueDataNode("invalid");
  }

  public delegate void IsSerializableDelegate(Entity<MetaDataComponent> ent, ref bool serializable);
}
