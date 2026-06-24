// Decompiled with JetBrains decompiler
// Type: Robust.Shared.EntitySerialization.Systems.MapLoaderSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Events;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.EntitySerialization.Systems;

public sealed class MapLoaderSystem : EntitySystem
{
  [Dependency]
  private readonly IResourceManager _resourceManager;
  [Dependency]
  private readonly SharedMapSystem _mapSystem;
  [Dependency]
  private readonly SharedTransformSystem _xform;
  [Dependency]
  private readonly IDependencyCollection _dependency;
  private Stopwatch _stopwatch = new Stopwatch();
  private Robust.Shared.GameObjects.EntityQuery<MapComponent> _mapQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _gridQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
    this._mapQuery = this.GetEntityQuery<MapComponent>();
    this._gridQuery = this.GetEntityQuery<MapGridComponent>();
  }

  private void Write(ResPath path, MappingDataNode data)
  {
    this.Log.Info($"Saving serialized results to {path}");
    path = path.ToRootedPath();
    YamlDocument yamlDocument = new YamlDocument((YamlNode) data.ToYaml());
    this._resourceManager.UserData.CreateDir(path.Directory);
    using (StreamWriter streamWriter = this._resourceManager.UserData.OpenWriteText(path))
    {
      YamlStream yamlStream = new YamlStream();
      yamlStream.Add(yamlDocument);
      yamlStream.Save((IEmitter) new YamlMappingFix((IEmitter) new Emitter((TextWriter) streamWriter)), false);
    }
  }

  public bool TryReadFile(ResPath file, [NotNullWhen(true)] out MappingDataNode? data)
  {
    ResPath rootedPath = file.ToRootedPath();
    data = (MappingDataNode) null;
    TextReader reader;
    if (!this.TryGetReader(rootedPath, out reader))
      return false;
    this.Log.Info($"Loading file: {rootedPath}");
    return this.TryReadFile(reader, out data);
  }

  private bool TryReadFile(TextReader reader, [NotNullWhen(true)] out MappingDataNode? data)
  {
    data = (MappingDataNode) null;
    this._stopwatch.Restart();
    using (reader)
    {
      DataNodeDocument[] array = DataNodeParser.ParseYamlStream(reader).ToArray<DataNodeDocument>();
      this.Log.Debug($"Loaded yml stream in {this._stopwatch.Elapsed}");
      int length = array.Length;
      if (length >= 1)
      {
        if (length > 1)
        {
          this.Log.Error("Stream too many YAML documents. Map files store exactly one.");
          return false;
        }
        data = (MappingDataNode) array[0].Root;
        return true;
      }
      this.Log.Error("Stream has no YAML documents.");
      return false;
    }
  }

  private bool TryGetReader(ResPath resPath, [NotNullWhen(true)] out TextReader? reader)
  {
    if (this._resourceManager.UserData.Exists(resPath))
    {
      if (this._resourceManager.ContentFileExists(resPath))
        this.Log.Warning("Reading map user data instead of content");
      reader = (TextReader) this._resourceManager.UserData.OpenText(resPath);
      return true;
    }
    Stream fileStream;
    if (this._resourceManager.TryContentFileRead(new ResPath?(resPath), out fileStream))
    {
      reader = (TextReader) new StreamReader(fileStream);
      return true;
    }
    this.Log.Error($"File not found: {resPath}");
    reader = (TextReader) null;
    return false;
  }

  public void Delete(LoadResult result)
  {
    foreach (Entity<MapComponent> map in result.Maps)
      this.Del(new EntityUid?((EntityUid) map));
    foreach (EntityUid orphan in result.Orphans)
      this.Del(new EntityUid?(orphan));
    foreach (EntityUid entity in result.Entities)
      this.Del(new EntityUid?(entity));
  }

  public bool TryLoadGeneric(
    ResPath file,
    [NotNullWhen(true)] out HashSet<Entity<MapComponent>>? maps,
    [NotNullWhen(true)] out HashSet<Entity<MapGridComponent>>? grids,
    MapLoadOptions? options = null)
  {
    grids = (HashSet<Entity<MapGridComponent>>) null;
    maps = (HashSet<Entity<MapComponent>>) null;
    LoadResult result;
    if (!this.TryLoadGeneric(file, out result, options))
      return false;
    maps = result.Maps;
    grids = result.Grids;
    return true;
  }

  public bool TryLoadGeneric(
    Stream file,
    string fileName,
    [NotNullWhen(true)] out LoadResult? result,
    MapLoadOptions? options = null)
  {
    result = (LoadResult) null;
    MappingDataNode data;
    return this.TryReadFile((TextReader) new StreamReader(file), out data) && this.TryLoadGeneric(data, fileName, out result, options);
  }

  public bool TryLoadGeneric(ResPath file, [NotNullWhen(true)] out LoadResult? result, MapLoadOptions? options = null)
  {
    result = (LoadResult) null;
    MappingDataNode data;
    return this.TryReadFile(file, out data) && this.TryLoadGeneric(data, file.ToString(), out result, options);
  }

  private bool TryLoadGeneric(
    MappingDataNode data,
    string fileName,
    [NotNullWhen(true)] out LoadResult? result,
    MapLoadOptions? options = null)
  {
    result = (LoadResult) null;
    this._stopwatch.Restart();
    BeforeEntityReadEvent message = new BeforeEntityReadEvent();
    this.RaiseLocalEvent<BeforeEntityReadEvent>(message);
    MapLoadOptions opts = options ?? MapLoadOptions.Default;
    opts.DeserializationOptions.AssignMapIds = !opts.ForceMapId.HasValue;
    MapId? mergeMap = opts.MergeMap;
    if (mergeMap.HasValue)
    {
      MapId valueOrDefault = mergeMap.GetValueOrDefault();
      if (!this._mapSystem.MapExists(new MapId?(valueOrDefault)))
        throw new Exception($"Target map {valueOrDefault} does not exist");
    }
    if (opts.MergeMap.HasValue && opts.ForceMapId.HasValue)
      throw new Exception("Invalid combination of MapLoadOptions");
    if (this._mapSystem.MapExists(opts.ForceMapId))
      throw new Exception("Target map already exists");
    EntityDeserializer deserializer = new EntityDeserializer(this._dependency, data, opts.DeserializationOptions, message.RenamedPrototypes, message.DeletedPrototypes);
    if (!deserializer.TryProcessData())
    {
      this.Log.Debug("Failed to process entity data in " + fileName);
      return false;
    }
    FileCategory? expectedCategory1 = opts.ExpectedCategory;
    if (expectedCategory1.HasValue)
    {
      FileCategory valueOrDefault = expectedCategory1.GetValueOrDefault();
      if (valueOrDefault != deserializer.Result.Category && deserializer.Result.Category != FileCategory.Unknown)
      {
        this.Log.Error($"Map {fileName} does not contain the expected data. Expected {valueOrDefault} but got {deserializer.Result.Category}");
        this.Delete(deserializer.Result);
        return false;
      }
    }
    try
    {
      deserializer.CreateEntities();
    }
    catch (Exception ex)
    {
      this.Log.Error($"Caught exception while creating entities for map {fileName}: {ex}");
      this.Delete(deserializer.Result);
      throw;
    }
    FileCategory? expectedCategory2 = opts.ExpectedCategory;
    if (expectedCategory2.HasValue)
    {
      FileCategory valueOrDefault = expectedCategory2.GetValueOrDefault();
      if (valueOrDefault != deserializer.Result.Category)
      {
        this.Log.Error($"Map {fileName} does not contain the expected data. Expected {valueOrDefault} but got {deserializer.Result.Category}");
        this.Delete(deserializer.Result);
        return false;
      }
    }
    HashSet<EntityUid> merged = new HashSet<EntityUid>();
    this.MergeMaps(deserializer, opts, merged);
    if (!this.SetMapId(deserializer, opts))
      return false;
    this.ApplyTransform(deserializer, opts);
    try
    {
      deserializer.StartEntities();
    }
    catch (Exception ex)
    {
      this.Log.Error($"Caught exception while starting entities: {ex}");
      this.Delete(deserializer.Result);
      throw;
    }
    mergeMap = opts.MergeMap;
    if (mergeMap.HasValue)
    {
      MapId valueOrDefault = mergeMap.GetValueOrDefault();
      this.MapInitalizeMerged(merged, valueOrDefault);
    }
    result = deserializer.Result;
    this.Log.Debug($"Loaded map in {this._stopwatch.Elapsed}");
    return true;
  }

  public bool TryLoadEntity(
    ResPath path,
    [NotNullWhen(true)] out Entity<TransformComponent>? entity,
    DeserializationOptions? options = null)
  {
    MapLoadOptions mapLoadOptions = new MapLoadOptions()
    {
      DeserializationOptions = options ?? DeserializationOptions.Default,
      ExpectedCategory = new FileCategory?(FileCategory.Entity)
    };
    entity = new Entity<TransformComponent>?();
    LoadResult result;
    if (!this.TryLoadGeneric(path, out result, new MapLoadOptions?(mapLoadOptions)))
      return false;
    if (result.Orphans.Count == 1)
    {
      EntityUid uid = result.Orphans.Single<EntityUid>();
      entity = new Entity<TransformComponent>?((Entity<TransformComponent>) (uid, this.Transform(uid)));
      return true;
    }
    this.Delete(result);
    return false;
  }

  public bool TryLoadGrid(
    MapId map,
    ResPath path,
    [NotNullWhen(true)] out Entity<MapGridComponent>? grid,
    DeserializationOptions? options = null,
    Vector2 offset = default (Vector2),
    Angle rot = default (Angle))
  {
    MapLoadOptions mapLoadOptions = new MapLoadOptions()
    {
      MergeMap = new MapId?(map),
      Offset = offset,
      Rotation = rot,
      DeserializationOptions = options ?? DeserializationOptions.Default,
      ExpectedCategory = new FileCategory?(FileCategory.Grid)
    };
    grid = new Entity<MapGridComponent>?();
    LoadResult result;
    if (!this.TryLoadGeneric(path, out result, new MapLoadOptions?(mapLoadOptions)))
      return false;
    if (result.Grids.Count == 1)
    {
      grid = new Entity<MapGridComponent>?(result.Grids.Single<Entity<MapGridComponent>>());
      return true;
    }
    this.Delete(result);
    return false;
  }

  public bool TryLoadGrid(
    ResPath path,
    [NotNullWhen(true)] out Entity<MapComponent>? map,
    [NotNullWhen(true)] out Entity<MapGridComponent>? grid,
    DeserializationOptions? options = null,
    Vector2 offset = default (Vector2),
    Angle rot = default (Angle))
  {
    DeserializationOptions deserializationOptions = options ?? DeserializationOptions.Default;
    MapId mapId;
    EntityUid map1 = this._mapSystem.CreateMap(out mapId, deserializationOptions.InitializeMaps);
    if (deserializationOptions.PauseMaps)
      this._mapSystem.SetPaused((Entity<MapComponent>) map1, true);
    if (!this.TryLoadGrid(mapId, path, out grid, options, offset, rot))
    {
      this.Del(new EntityUid?(map1));
      map = new Entity<MapComponent>?();
      return false;
    }
    map = new Entity<MapComponent>?(new Entity<MapComponent>(map1, this.Comp<MapComponent>(map1)));
    return true;
  }

  private void ApplyTransform(EntityDeserializer deserializer, MapLoadOptions opts)
  {
    if (Angle.op_Equality(opts.Rotation, Angle.Zero) && opts.Offset == Vector2.Zero || opts.MergeMap.HasValue)
      return;
    Matrix3x2 transform = Matrix3Helpers.CreateTransform(ref opts.Offset, ref opts.Rotation);
    foreach (EntityUid entity in deserializer.Result.Entities)
    {
      TransformComponent xform = this.Transform(entity);
      if (this._mapQuery.HasComp(xform.ParentUid) && !this._gridQuery.HasComponent(xform.ParentUid))
      {
        Angle rot = Angle.op_Addition(xform.LocalRotation, opts.Rotation);
        Vector2 pos = Vector2.Transform(xform.LocalPosition, transform);
        this._xform.SetLocalPositionRotation(entity, pos, rot, xform);
      }
    }
  }

  public bool TryLoadMap(
    ResPath path,
    [NotNullWhen(true)] out Entity<MapComponent>? map,
    [NotNullWhen(true)] out HashSet<Entity<MapGridComponent>>? grids,
    DeserializationOptions? options = null,
    Vector2 offset = default (Vector2),
    Angle rot = default (Angle))
  {
    MapLoadOptions mapLoadOptions = new MapLoadOptions()
    {
      Offset = offset,
      Rotation = rot,
      DeserializationOptions = options ?? DeserializationOptions.Default,
      ExpectedCategory = new FileCategory?(FileCategory.Map)
    };
    map = new Entity<MapComponent>?();
    grids = (HashSet<Entity<MapGridComponent>>) null;
    LoadResult result;
    if (!this.TryLoadGeneric(path, out result, new MapLoadOptions?(mapLoadOptions)))
      return false;
    if (result.Maps.Count == 1)
    {
      map = new Entity<MapComponent>?(result.Maps.First<Entity<MapComponent>>());
      grids = result.Grids;
      return true;
    }
    this.Delete(result);
    return false;
  }

  public bool TryLoadMapWithId(
    MapId mapId,
    ResPath path,
    [NotNullWhen(true)] out Entity<MapComponent>? map,
    [NotNullWhen(true)] out HashSet<Entity<MapGridComponent>>? grids,
    DeserializationOptions? options = null,
    Vector2 offset = default (Vector2),
    Angle rot = default (Angle))
  {
    map = new Entity<MapComponent>?();
    grids = (HashSet<Entity<MapGridComponent>>) null;
    MapLoadOptions mapLoadOptions = new MapLoadOptions()
    {
      Offset = offset,
      Rotation = rot,
      DeserializationOptions = options ?? DeserializationOptions.Default,
      ExpectedCategory = new FileCategory?(FileCategory.Map)
    };
    mapLoadOptions.ForceMapId = !this._mapSystem.MapExists(new MapId?(mapId)) ? new MapId?(mapId) : throw new Exception("Target map already exists");
    LoadResult result;
    EntityUid? uid;
    MapComponent comp;
    if (!this.TryLoadGeneric(path, out result, new MapLoadOptions?(mapLoadOptions)) || !this._mapSystem.TryGetMap(new MapId?(mapId), out uid) || !this.TryComp<MapComponent>(uid, out comp))
      return false;
    map = new Entity<MapComponent>?(new Entity<MapComponent>(uid.Value, comp));
    grids = result.Grids;
    return true;
  }

  public bool TryMergeMap(
    MapId mapId,
    ResPath path,
    [NotNullWhen(true)] out HashSet<Entity<MapGridComponent>>? grids,
    DeserializationOptions? options = null,
    Vector2 offset = default (Vector2),
    Angle rot = default (Angle))
  {
    grids = (HashSet<Entity<MapGridComponent>>) null;
    MapLoadOptions mapLoadOptions = new MapLoadOptions()
    {
      Offset = offset,
      Rotation = rot,
      DeserializationOptions = options ?? DeserializationOptions.Default,
      ExpectedCategory = new FileCategory?(FileCategory.Map)
    };
    mapLoadOptions.MergeMap = this._mapSystem.MapExists(new MapId?(mapId)) ? new MapId?(mapId) : throw new Exception($"Target map {mapId} does not exist");
    LoadResult result;
    EntityUid? uid;
    if (!this.TryLoadGeneric(path, out result, new MapLoadOptions?(mapLoadOptions)) || !this._mapSystem.TryGetMap(new MapId?(mapId), out uid) || !this.TryComp<MapComponent>(uid, out MapComponent _))
      return false;
    grids = result.Grids;
    return true;
  }

  private void MergeMaps(
    EntityDeserializer deserializer,
    MapLoadOptions opts,
    HashSet<EntityUid> merged)
  {
    MapId? mergeMap = opts.MergeMap;
    if (!mergeMap.HasValue)
      return;
    MapId valueOrDefault = mergeMap.GetValueOrDefault();
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(valueOrDefault), out uid))
      throw new Exception($"Target map {valueOrDefault} does not exist");
    deserializer.Result.Category = FileCategory.Unknown;
    Angle rotation = opts.Rotation;
    Matrix3x2 matrix = Matrix3Helpers.CreateTransform(ref opts.Offset, ref rotation);
    Entity<TransformComponent> target = new Entity<TransformComponent>(uid.Value, this.Transform(uid.Value));
    HashSet<EntityUid> maps = new HashSet<EntityUid>();
    HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>();
    foreach (EntityUid entity in deserializer.Result.Entities)
    {
      TransformComponent transformComponent = this.Transform(entity);
      if (this._mapQuery.HasComp(transformComponent.ParentUid))
      {
        if (this._gridQuery.HasComponent(transformComponent.ParentUid) && entityUidSet.Add(transformComponent.ParentUid))
        {
          this.Log.Error("Merging a grid-map onto another map is not supported.");
        }
        else
        {
          maps.Add(transformComponent.ParentUid);
          this.Merge(merged, entity, target, in matrix, rotation);
        }
      }
    }
    deserializer.ToDelete.UnionWith((IEnumerable<EntityUid>) maps);
    deserializer.Result.Maps.RemoveWhere((Predicate<Entity<MapComponent>>) (x => maps.Contains(x.Owner)));
    foreach (EntityUid orphan in deserializer.Result.Orphans)
      this.Merge(merged, orphan, target, in matrix, rotation);
    deserializer.Result.Orphans.Clear();
  }

  private void Merge(
    HashSet<EntityUid> merged,
    EntityUid uid,
    Entity<TransformComponent> target,
    in Matrix3x2 matrix,
    Angle rotation)
  {
    merged.Add(uid);
    TransformComponent transformComponent = this.Transform(uid);
    Angle angle = Angle.op_Addition(transformComponent.LocalRotation, rotation);
    Vector2 position = Vector2.Transform(transformComponent.LocalPosition, matrix);
    EntityCoordinates entityCoordinates = new EntityCoordinates(target.Owner, position);
    this._xform.SetCoordinates((Entity<TransformComponent, MetaDataComponent>) (uid, transformComponent, this.MetaData(uid)), entityCoordinates, new Angle?(angle), newParent: target.Comp);
  }

  private void MapInitalizeMerged(HashSet<EntityUid> merged, MapId targetId)
  {
    EntityUid? uid;
    if (!this._mapSystem.TryGetMap(new MapId?(targetId), out uid))
      throw new Exception($"Target map {targetId} does not exist");
    if (this._mapSystem.IsInitialized((Entity<MapComponent>) uid.Value))
    {
      foreach (EntityUid entity in merged)
        this._mapSystem.RecursiveMapInit(entity);
    }
    bool paused = this._mapSystem.IsPaused((Entity<MapComponent>) uid.Value);
    foreach (EntityUid entity in merged)
      this._mapSystem.RecursiveSetPaused(entity, paused);
  }

  private bool SetMapId(EntityDeserializer deserializer, MapLoadOptions opts)
  {
    MapId? forceMapId = opts.ForceMapId;
    if (!forceMapId.HasValue)
      return true;
    MapId valueOrDefault = forceMapId.GetValueOrDefault();
    if (deserializer.Result.Maps.Count != 1)
    {
      this.Log.Error("The ForceMapId option is only supported when loading a file containing a single map.");
      this.Delete(deserializer.Result);
      return false;
    }
    this._mapSystem.AssignMapId(deserializer.Result.Maps.Single<Entity<MapComponent>>(), new MapId?(valueOrDefault));
    return true;
  }

  public event EntitySerializer.IsSerializableDelegate? OnIsSerializable;

  public (MappingDataNode Node, FileCategory Category) SerializeEntitiesRecursive(
    HashSet<EntityUid> entities,
    SerializationOptions? options = null)
  {
    this._stopwatch.Restart();
    if (!entities.All<EntityUid>(new Func<EntityUid, bool>(((EntitySystem) this).Exists)))
      throw new Exception("Cannot serialize deleted entities");
    this.Log.Info("Serializing entities: " + string.Join(", ", entities.Select<EntityUid, string>((Func<EntityUid, string>) (x => this.ToPrettyString((Entity<MetaDataComponent>) x).ToString()))));
    HashSet<MapId> hashSet = entities.Select<EntityUid, MapId>((Func<EntityUid, MapId>) (x => this.Transform(x).MapID)).ToHashSet<MapId>();
    SerializationOptions? nullable = options;
    SerializationOptions valueOrDefault;
    if (!nullable.HasValue)
      valueOrDefault = SerializationOptions.Default with
      {
        ExpectPreInit = entities.All<EntityUid>((Func<EntityUid, bool>) (x => this.LifeStage(x) < EntityLifeStage.MapInitialized))
      };
    else
      valueOrDefault = nullable.GetValueOrDefault();
    SerializationOptions options1 = valueOrDefault;
    this.RaiseLocalEvent<BeforeSerializationEvent>(new BeforeSerializationEvent(entities, hashSet, options1.Category));
    EntitySerializer entitySerializer = new EntitySerializer(this._dependency, options1);
    entitySerializer.OnIsSerializeable += this.OnIsSerializable;
    entitySerializer.SerializeEntityRecursive(entities);
    MappingDataNode Node = entitySerializer.Write();
    FileCategory category = entitySerializer.GetCategory();
    this.RaiseLocalEvent<AfterSerializationEvent>(new AfterSerializationEvent(entities, Node, category));
    this.Log.Debug($"Serialized {entitySerializer.EntityData.Count} entities in {this._stopwatch.Elapsed}");
    return (Node, category);
  }

  public bool TrySaveEntity(EntityUid entity, ResPath path, SerializationOptions? options = null)
  {
    if (this._mapQuery.HasComp(entity))
    {
      this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) entity)} is a map. Use {"TrySaveMap"}.");
      return false;
    }
    if (this._gridQuery.HasComp(entity))
    {
      this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) entity)} is a grid. Use {"TrySaveGrid"}.");
      return false;
    }
    SerializationOptions serializationOptions = (options ?? SerializationOptions.Default) with
    {
      Category = FileCategory.Entity
    };
    MappingDataNode mappingDataNode;
    FileCategory Category;
    try
    {
      (mappingDataNode, Category) = this.SerializeEntitiesRecursive(new HashSet<EntityUid>()
      {
        entity
      }, new SerializationOptions?(serializationOptions));
    }
    catch (Exception ex)
    {
      this.Log.Error($"Caught exception while trying to serialize entity {this.ToPrettyString((Entity<MetaDataComponent>) entity)}:\n{ex}");
      return false;
    }
    if (Category != FileCategory.Entity)
    {
      this.Log.Error($"Failed to save {this.ToPrettyString((Entity<MetaDataComponent>) entity)} as a singular entity. Output: {Category}");
      return false;
    }
    this.Write(path, mappingDataNode);
    return true;
  }

  public bool TrySaveMap(MapId mapId, ResPath path, SerializationOptions? options = null)
  {
    EntityUid? uid;
    if (this._mapSystem.TryGetMap(new MapId?(mapId), out uid))
      return this.TrySaveMap(uid.Value, path, options);
    this.Log.Error($"Unable to find map {mapId}");
    return false;
  }

  public bool TrySaveMap(EntityUid map, ResPath path, SerializationOptions? options = null)
  {
    if (!this._mapQuery.HasComp(map))
    {
      this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) map)} is not a map.");
      return false;
    }
    SerializationOptions serializationOptions = (options ?? SerializationOptions.Default) with
    {
      Category = FileCategory.Map
    };
    MappingDataNode mappingDataNode;
    FileCategory Category;
    try
    {
      (mappingDataNode, Category) = this.SerializeEntitiesRecursive(new HashSet<EntityUid>()
      {
        map
      }, new SerializationOptions?(serializationOptions));
    }
    catch (Exception ex)
    {
      this.Log.Error($"Caught exception while trying to serialize map {this.ToPrettyString((Entity<MetaDataComponent>) map)}:\n{ex}");
      return false;
    }
    if (Category != FileCategory.Map)
    {
      this.Log.Error($"Failed to save {this.ToPrettyString((Entity<MetaDataComponent>) map)} as a map. Output: {Category}");
      return false;
    }
    this.Write(path, mappingDataNode);
    return true;
  }

  public bool TrySaveGrid(EntityUid grid, ResPath path, SerializationOptions? options = null)
  {
    if (!this._gridQuery.HasComp(grid))
    {
      this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) grid)} is not a grid.");
      return false;
    }
    if (this._mapQuery.HasComp(grid))
    {
      this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) grid)} is a map, not (just) a grid. Use {"TrySaveMap"}");
      return false;
    }
    SerializationOptions serializationOptions = (options ?? SerializationOptions.Default) with
    {
      Category = FileCategory.Grid
    };
    MappingDataNode mappingDataNode;
    FileCategory Category;
    try
    {
      (mappingDataNode, Category) = this.SerializeEntitiesRecursive(new HashSet<EntityUid>()
      {
        grid
      }, new SerializationOptions?(serializationOptions));
    }
    catch (Exception ex)
    {
      this.Log.Error($"Caught exception while trying to serialize grid {this.ToPrettyString((Entity<MetaDataComponent>) grid)}:\n{ex}");
      return false;
    }
    if (Category != FileCategory.Grid)
    {
      this.Log.Error($"Failed to save {this.ToPrettyString((Entity<MetaDataComponent>) grid)} as a grid. Output: {Category}");
      return false;
    }
    this.Write(path, mappingDataNode);
    return true;
  }

  public bool TrySaveGeneric(
    EntityUid uid,
    ResPath path,
    out FileCategory category,
    SerializationOptions? options = null)
  {
    return this.TrySaveGeneric(new HashSet<EntityUid>()
    {
      uid
    }, path, out category, options);
  }

  public bool TrySaveGeneric(
    HashSet<EntityUid> entities,
    ResPath path,
    out FileCategory category,
    SerializationOptions? options = null)
  {
    category = FileCategory.Unknown;
    if (entities.Count == 0)
      return false;
    SerializationOptions serializationOptions = options ?? SerializationOptions.Default;
    MappingDataNode mappingDataNode;
    try
    {
      (mappingDataNode, category) = this.SerializeEntitiesRecursive(entities, new SerializationOptions?(serializationOptions));
    }
    catch (Exception ex)
    {
      this.Log.Error($"Caught exception while trying to serialize entities:\n{ex}");
      return false;
    }
    this.Write(path, mappingDataNode);
    return true;
  }

  public bool TrySaveAllEntities(ResPath path, SerializationOptions? options = null)
  {
    MappingDataNode data;
    if (!this.TrySerializeAllEntities(out data, options))
      return false;
    this.Write(path, data);
    return true;
  }

  public bool TrySerializeAllEntities([NotNullWhen(true)] out MappingDataNode? data, SerializationOptions? options = null)
  {
    data = (MappingDataNode) null;
    SerializationOptions? nullable = options;
    SerializationOptions valueOrDefault;
    if (!nullable.HasValue)
      valueOrDefault = SerializationOptions.Default with
      {
        MissingEntityBehaviour = MissingEntityBehaviour.Error
      };
    else
      valueOrDefault = nullable.GetValueOrDefault();
    SerializationOptions options1 = valueOrDefault with
    {
      Category = FileCategory.Save
    };
    this._stopwatch.Restart();
    this.Log.Info("Serializing all entities");
    HashSet<EntityUid> hashSet1 = this.EntityManager.GetEntities().ToHashSet<EntityUid>();
    HashSet<MapId> hashSet2 = this._mapSystem.Maps.Keys.ToHashSet<MapId>();
    BeforeSerializationEvent message = new BeforeSerializationEvent(hashSet1, hashSet2, FileCategory.Save);
    EntitySerializer entitySerializer = new EntitySerializer(this._dependency, options1);
    Queue<EntityUid> entityUidQueue = new Queue<EntityUid>();
    foreach (EntityUid ent in hashSet1)
    {
      if (!entitySerializer.IsSerializable((Entity<MetaDataComponent>) ent))
        entityUidQueue.Enqueue(ent);
    }
    if (entityUidQueue.Count > 0)
    {
      if (options1.MissingEntityBehaviour == MissingEntityBehaviour.Error)
      {
        options1.MissingEntityBehaviour = MissingEntityBehaviour.Ignore;
        this.Log.Error("Attempted to serialize one or more non-serializable entities");
      }
      EntityUid result;
      while (entityUidQueue.TryDequeue(out result))
      {
        hashSet1.Remove(result);
        foreach (EntityUid child in this.Transform(result)._children)
          entityUidQueue.Enqueue(child);
      }
    }
    try
    {
      this.RaiseLocalEvent<BeforeSerializationEvent>(message);
      entitySerializer.OnIsSerializeable += this.OnIsSerializable;
      entitySerializer.SerializeEntities(hashSet1);
      data = entitySerializer.Write();
      FileCategory category = entitySerializer.GetCategory();
      this.RaiseLocalEvent<AfterSerializationEvent>(new AfterSerializationEvent(hashSet1, data, category));
      this.Log.Debug($"Serialized {entitySerializer.EntityData.Count} entities in {this._stopwatch.Elapsed}");
    }
    catch (Exception ex)
    {
      this.Log.Error($"Caught exception while trying to serialize all entities:\n{ex}");
      return false;
    }
    return true;
  }
}
