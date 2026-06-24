using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
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
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

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

	private EntityQuery<MapComponent> _mapQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	public event EntitySerializer.IsSerializableDelegate? OnIsSerializable;

	public override void Initialize()
	{
		base.Initialize();
		_gridQuery = GetEntityQuery<MapGridComponent>();
		_mapQuery = GetEntityQuery<MapComponent>();
		_gridQuery = GetEntityQuery<MapGridComponent>();
	}

	private void Write(ResPath path, MappingDataNode data)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		base.Log.Info($"Saving serialized results to {path}");
		path = path.ToRootedPath();
		YamlDocument val = new YamlDocument((YamlNode)(object)data.ToYaml());
		_resourceManager.UserData.CreateDir(path.Directory);
		using StreamWriter streamWriter = _resourceManager.UserData.OpenWriteText(path);
		YamlStream val2 = new YamlStream();
		val2.Add(val);
		val2.Save((IEmitter)(object)new YamlMappingFix((IEmitter)new Emitter((TextWriter)streamWriter)), false);
	}

	public bool TryReadFile(ResPath file, [NotNullWhen(true)] out MappingDataNode? data)
	{
		ResPath resPath = file.ToRootedPath();
		data = null;
		if (!TryGetReader(resPath, out TextReader reader))
		{
			return false;
		}
		base.Log.Info($"Loading file: {resPath}");
		return TryReadFile(reader, out data);
	}

	private bool TryReadFile(TextReader reader, [NotNullWhen(true)] out MappingDataNode? data)
	{
		data = null;
		_stopwatch.Restart();
		using (reader)
		{
			DataNodeDocument[] array = DataNodeParser.ParseYamlStream(reader).ToArray();
			base.Log.Debug($"Loaded yml stream in {_stopwatch.Elapsed}");
			int num = array.Length;
			if (num >= 1)
			{
				if (num > 1)
				{
					base.Log.Error("Stream too many YAML documents. Map files store exactly one.");
					return false;
				}
				data = (MappingDataNode)array[0].Root;
				return true;
			}
			base.Log.Error("Stream has no YAML documents.");
			return false;
		}
	}

	private bool TryGetReader(ResPath resPath, [NotNullWhen(true)] out TextReader? reader)
	{
		if (_resourceManager.UserData.Exists(resPath))
		{
			if (_resourceManager.ContentFileExists(resPath))
			{
				base.Log.Warning("Reading map user data instead of content");
			}
			reader = _resourceManager.UserData.OpenText(resPath);
			return true;
		}
		if (_resourceManager.TryContentFileRead(resPath, out Stream fileStream))
		{
			reader = new StreamReader(fileStream);
			return true;
		}
		base.Log.Error($"File not found: {resPath}");
		reader = null;
		return false;
	}

	public void Delete(LoadResult result)
	{
		foreach (Entity<MapComponent> map in result.Maps)
		{
			Del(map);
		}
		foreach (EntityUid orphan in result.Orphans)
		{
			Del(orphan);
		}
		foreach (EntityUid entity in result.Entities)
		{
			Del(entity);
		}
	}

	public bool TryLoadGeneric(ResPath file, [NotNullWhen(true)] out HashSet<Entity<MapComponent>>? maps, [NotNullWhen(true)] out HashSet<Entity<MapGridComponent>>? grids, MapLoadOptions? options = null)
	{
		grids = null;
		maps = null;
		if (!TryLoadGeneric(file, out LoadResult result, options))
		{
			return false;
		}
		maps = result.Maps;
		grids = result.Grids;
		return true;
	}

	public bool TryLoadGeneric(Stream file, string fileName, [NotNullWhen(true)] out LoadResult? result, MapLoadOptions? options = null)
	{
		result = null;
		if (!TryReadFile(new StreamReader(file), out MappingDataNode data))
		{
			return false;
		}
		return TryLoadGeneric(data, fileName, out result, options);
	}

	public bool TryLoadGeneric(ResPath file, [NotNullWhen(true)] out LoadResult? result, MapLoadOptions? options = null)
	{
		result = null;
		if (!TryReadFile(file, out MappingDataNode data))
		{
			return false;
		}
		return TryLoadGeneric(data, file.ToString(), out result, options);
	}

	private bool TryLoadGeneric(MappingDataNode data, string fileName, [NotNullWhen(true)] out LoadResult? result, MapLoadOptions? options = null)
	{
		result = null;
		_stopwatch.Restart();
		BeforeEntityReadEvent beforeEntityReadEvent = new BeforeEntityReadEvent();
		RaiseLocalEvent(beforeEntityReadEvent);
		MapLoadOptions opts = options ?? MapLoadOptions.Default;
		opts.DeserializationOptions.AssignMapIds = !opts.ForceMapId.HasValue;
		MapId? mergeMap = opts.MergeMap;
		if (mergeMap.HasValue)
		{
			MapId valueOrDefault = mergeMap.GetValueOrDefault();
			if (!_mapSystem.MapExists(valueOrDefault))
			{
				throw new Exception($"Target map {valueOrDefault} does not exist");
			}
		}
		if (opts.MergeMap.HasValue && opts.ForceMapId.HasValue)
		{
			throw new Exception("Invalid combination of MapLoadOptions");
		}
		if (_mapSystem.MapExists(opts.ForceMapId))
		{
			throw new Exception("Target map already exists");
		}
		EntityDeserializer entityDeserializer = new EntityDeserializer(_dependency, data, opts.DeserializationOptions, beforeEntityReadEvent.RenamedPrototypes, beforeEntityReadEvent.DeletedPrototypes);
		if (!entityDeserializer.TryProcessData())
		{
			base.Log.Debug("Failed to process entity data in " + fileName);
			return false;
		}
		FileCategory? expectedCategory = opts.ExpectedCategory;
		if (expectedCategory.HasValue)
		{
			FileCategory valueOrDefault2 = expectedCategory.GetValueOrDefault();
			if (valueOrDefault2 != entityDeserializer.Result.Category && entityDeserializer.Result.Category != FileCategory.Unknown)
			{
				base.Log.Error($"Map {fileName} does not contain the expected data. Expected {valueOrDefault2} but got {entityDeserializer.Result.Category}");
				Delete(entityDeserializer.Result);
				return false;
			}
		}
		try
		{
			entityDeserializer.CreateEntities();
		}
		catch (Exception value)
		{
			base.Log.Error($"Caught exception while creating entities for map {fileName}: {value}");
			Delete(entityDeserializer.Result);
			throw;
		}
		expectedCategory = opts.ExpectedCategory;
		if (expectedCategory.HasValue)
		{
			FileCategory valueOrDefault3 = expectedCategory.GetValueOrDefault();
			if (valueOrDefault3 != entityDeserializer.Result.Category)
			{
				base.Log.Error($"Map {fileName} does not contain the expected data. Expected {valueOrDefault3} but got {entityDeserializer.Result.Category}");
				Delete(entityDeserializer.Result);
				return false;
			}
		}
		HashSet<EntityUid> merged = new HashSet<EntityUid>();
		MergeMaps(entityDeserializer, opts, merged);
		if (!SetMapId(entityDeserializer, opts))
		{
			return false;
		}
		ApplyTransform(entityDeserializer, opts);
		try
		{
			entityDeserializer.StartEntities();
		}
		catch (Exception value2)
		{
			base.Log.Error($"Caught exception while starting entities: {value2}");
			Delete(entityDeserializer.Result);
			throw;
		}
		mergeMap = opts.MergeMap;
		if (mergeMap.HasValue)
		{
			MapId valueOrDefault4 = mergeMap.GetValueOrDefault();
			MapInitalizeMerged(merged, valueOrDefault4);
		}
		result = entityDeserializer.Result;
		base.Log.Debug($"Loaded map in {_stopwatch.Elapsed}");
		return true;
	}

	public bool TryLoadEntity(ResPath path, [NotNullWhen(true)] out Entity<TransformComponent>? entity, DeserializationOptions? options = null)
	{
		MapLoadOptions mapLoadOptions = new MapLoadOptions();
		mapLoadOptions.DeserializationOptions = options ?? DeserializationOptions.Default;
		mapLoadOptions.ExpectedCategory = FileCategory.Entity;
		MapLoadOptions value = mapLoadOptions;
		entity = null;
		if (!TryLoadGeneric(path, out LoadResult result, value))
		{
			return false;
		}
		if (result.Orphans.Count == 1)
		{
			EntityUid entityUid = result.Orphans.Single();
			entity = (Owner: entityUid, Comp: Transform(entityUid));
			return true;
		}
		Delete(result);
		return false;
	}

	public bool TryLoadGrid(MapId map, ResPath path, [NotNullWhen(true)] out Entity<MapGridComponent>? grid, DeserializationOptions? options = null, Vector2 offset = default(Vector2), Angle rot = default(Angle))
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		MapLoadOptions mapLoadOptions = new MapLoadOptions();
		mapLoadOptions.MergeMap = map;
		mapLoadOptions.Offset = offset;
		mapLoadOptions.Rotation = rot;
		mapLoadOptions.DeserializationOptions = options ?? DeserializationOptions.Default;
		mapLoadOptions.ExpectedCategory = FileCategory.Grid;
		MapLoadOptions value = mapLoadOptions;
		grid = null;
		if (!TryLoadGeneric(path, out LoadResult result, value))
		{
			return false;
		}
		if (result.Grids.Count == 1)
		{
			grid = result.Grids.Single();
			return true;
		}
		Delete(result);
		return false;
	}

	public bool TryLoadGrid(ResPath path, [NotNullWhen(true)] out Entity<MapComponent>? map, [NotNullWhen(true)] out Entity<MapGridComponent>? grid, DeserializationOptions? options = null, Vector2 offset = default(Vector2), Angle rot = default(Angle))
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		DeserializationOptions deserializationOptions = options ?? DeserializationOptions.Default;
		MapId mapId;
		EntityUid entityUid = _mapSystem.CreateMap(out mapId, deserializationOptions.InitializeMaps);
		if (deserializationOptions.PauseMaps)
		{
			_mapSystem.SetPaused(entityUid, paused: true);
		}
		if (!TryLoadGrid(mapId, path, out grid, options, offset, rot))
		{
			Del(entityUid);
			map = null;
			return false;
		}
		map = new Entity<MapComponent>(entityUid, Comp<MapComponent>(entityUid));
		return true;
	}

	private void ApplyTransform(EntityDeserializer deserializer, MapLoadOptions opts)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if ((opts.Rotation == Angle.Zero && opts.Offset == Vector2.Zero) || opts.MergeMap.HasValue)
		{
			return;
		}
		Matrix3x2 matrix = Matrix3Helpers.CreateTransform(ref opts.Offset, ref opts.Rotation);
		foreach (EntityUid entity in deserializer.Result.Entities)
		{
			TransformComponent transformComponent = Transform(entity);
			if (_mapQuery.HasComp(transformComponent.ParentUid) && !_gridQuery.HasComponent(transformComponent.ParentUid))
			{
				Angle rot = transformComponent.LocalRotation + opts.Rotation;
				Vector2 pos = Vector2.Transform(transformComponent.LocalPosition, matrix);
				_xform.SetLocalPositionRotation(entity, pos, rot, transformComponent);
			}
		}
	}

	public bool TryLoadMap(ResPath path, [NotNullWhen(true)] out Entity<MapComponent>? map, [NotNullWhen(true)] out HashSet<Entity<MapGridComponent>>? grids, DeserializationOptions? options = null, Vector2 offset = default(Vector2), Angle rot = default(Angle))
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		MapLoadOptions mapLoadOptions = new MapLoadOptions();
		mapLoadOptions.Offset = offset;
		mapLoadOptions.Rotation = rot;
		mapLoadOptions.DeserializationOptions = options ?? DeserializationOptions.Default;
		mapLoadOptions.ExpectedCategory = FileCategory.Map;
		MapLoadOptions value = mapLoadOptions;
		map = null;
		grids = null;
		if (!TryLoadGeneric(path, out LoadResult result, value))
		{
			return false;
		}
		if (result.Maps.Count == 1)
		{
			map = result.Maps.First();
			grids = result.Grids;
			return true;
		}
		Delete(result);
		return false;
	}

	public bool TryLoadMapWithId(MapId mapId, ResPath path, [NotNullWhen(true)] out Entity<MapComponent>? map, [NotNullWhen(true)] out HashSet<Entity<MapGridComponent>>? grids, DeserializationOptions? options = null, Vector2 offset = default(Vector2), Angle rot = default(Angle))
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		map = null;
		grids = null;
		MapLoadOptions mapLoadOptions = new MapLoadOptions();
		mapLoadOptions.Offset = offset;
		mapLoadOptions.Rotation = rot;
		mapLoadOptions.DeserializationOptions = options ?? DeserializationOptions.Default;
		mapLoadOptions.ExpectedCategory = FileCategory.Map;
		MapLoadOptions value = mapLoadOptions;
		if (_mapSystem.MapExists(mapId))
		{
			throw new Exception("Target map already exists");
		}
		value.ForceMapId = mapId;
		if (!TryLoadGeneric(path, out LoadResult result, value))
		{
			return false;
		}
		if (!_mapSystem.TryGetMap(mapId, out var uid) || !TryComp(uid, out MapComponent comp))
		{
			return false;
		}
		map = new Entity<MapComponent>(uid.Value, comp);
		grids = result.Grids;
		return true;
	}

	public bool TryMergeMap(MapId mapId, ResPath path, [NotNullWhen(true)] out HashSet<Entity<MapGridComponent>>? grids, DeserializationOptions? options = null, Vector2 offset = default(Vector2), Angle rot = default(Angle))
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		grids = null;
		MapLoadOptions mapLoadOptions = new MapLoadOptions();
		mapLoadOptions.Offset = offset;
		mapLoadOptions.Rotation = rot;
		mapLoadOptions.DeserializationOptions = options ?? DeserializationOptions.Default;
		mapLoadOptions.ExpectedCategory = FileCategory.Map;
		MapLoadOptions value = mapLoadOptions;
		if (!_mapSystem.MapExists(mapId))
		{
			throw new Exception($"Target map {mapId} does not exist");
		}
		value.MergeMap = mapId;
		if (!TryLoadGeneric(path, out LoadResult result, value))
		{
			return false;
		}
		if (!_mapSystem.TryGetMap(mapId, out var uid) || !TryComp(uid, out MapComponent _))
		{
			return false;
		}
		grids = result.Grids;
		return true;
	}

	private void MergeMaps(EntityDeserializer deserializer, MapLoadOptions opts, HashSet<EntityUid> merged)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		MapId? mergeMap = opts.MergeMap;
		if (!mergeMap.HasValue)
		{
			return;
		}
		MapId valueOrDefault = mergeMap.GetValueOrDefault();
		if (!_mapSystem.TryGetMap(valueOrDefault, out var uid))
		{
			throw new Exception($"Target map {valueOrDefault} does not exist");
		}
		deserializer.Result.Category = FileCategory.Unknown;
		Angle rotation = opts.Rotation;
		Matrix3x2 matrix = Matrix3Helpers.CreateTransform(ref opts.Offset, ref rotation);
		Entity<TransformComponent> target = new Entity<TransformComponent>(uid.Value, Transform(uid.Value));
		HashSet<EntityUid> maps = new HashSet<EntityUid>();
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		foreach (EntityUid entity in deserializer.Result.Entities)
		{
			TransformComponent transformComponent = Transform(entity);
			if (_mapQuery.HasComp(transformComponent.ParentUid))
			{
				if (_gridQuery.HasComponent(transformComponent.ParentUid) && hashSet.Add(transformComponent.ParentUid))
				{
					base.Log.Error("Merging a grid-map onto another map is not supported.");
					continue;
				}
				maps.Add(transformComponent.ParentUid);
				Merge(merged, entity, target, in matrix, rotation);
			}
		}
		deserializer.ToDelete.UnionWith(maps);
		deserializer.Result.Maps.RemoveWhere((Entity<MapComponent> x) => maps.Contains(x.Owner));
		foreach (EntityUid orphan in deserializer.Result.Orphans)
		{
			Merge(merged, orphan, target, in matrix, rotation);
		}
		deserializer.Result.Orphans.Clear();
	}

	private void Merge(HashSet<EntityUid> merged, EntityUid uid, Entity<TransformComponent> target, in Matrix3x2 matrix, Angle rotation)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		merged.Add(uid);
		TransformComponent transformComponent = Transform(uid);
		Angle value = transformComponent.LocalRotation + rotation;
		Vector2 position = Vector2.Transform(transformComponent.LocalPosition, matrix);
		EntityCoordinates value2 = new EntityCoordinates(target.Owner, position);
		_xform.SetCoordinates((Owner: uid, Comp1: transformComponent, Comp2: MetaData(uid)), value2, value, unanchor: true, target.Comp);
	}

	private void MapInitalizeMerged(HashSet<EntityUid> merged, MapId targetId)
	{
		if (!_mapSystem.TryGetMap(targetId, out var uid))
		{
			throw new Exception($"Target map {targetId} does not exist");
		}
		if (_mapSystem.IsInitialized(uid.Value))
		{
			foreach (EntityUid item in merged)
			{
				_mapSystem.RecursiveMapInit(item);
			}
		}
		bool paused = _mapSystem.IsPaused(uid.Value);
		foreach (EntityUid item2 in merged)
		{
			_mapSystem.RecursiveSetPaused(item2, paused);
		}
	}

	private bool SetMapId(EntityDeserializer deserializer, MapLoadOptions opts)
	{
		MapId? forceMapId = opts.ForceMapId;
		if (forceMapId.HasValue)
		{
			MapId valueOrDefault = forceMapId.GetValueOrDefault();
			if (deserializer.Result.Maps.Count != 1)
			{
				base.Log.Error("The ForceMapId option is only supported when loading a file containing a single map.");
				Delete(deserializer.Result);
				return false;
			}
			Entity<MapComponent> map = deserializer.Result.Maps.Single();
			_mapSystem.AssignMapId(map, valueOrDefault);
			return true;
		}
		return true;
	}

	public (MappingDataNode Node, FileCategory Category) SerializeEntitiesRecursive(HashSet<EntityUid> entities, SerializationOptions? options = null)
	{
		_stopwatch.Restart();
		if (!entities.All(base.Exists))
		{
			throw new Exception("Cannot serialize deleted entities");
		}
		base.Log.Info("Serializing entities: " + string.Join(", ", entities.Select((EntityUid x) => ToPrettyString(x).ToString())));
		HashSet<MapId> mapIds = entities.Select((EntityUid x) => Transform(x).MapID).ToHashSet();
		SerializationOptions options2 = options ?? SerializationOptions.Default with
		{
			ExpectPreInit = entities.All((EntityUid x) => (int)LifeStage(x) < 3)
		};
		BeforeSerializationEvent message = new BeforeSerializationEvent(entities, mapIds, options2.Category);
		RaiseLocalEvent(message);
		EntitySerializer entitySerializer = new EntitySerializer(_dependency, options2);
		entitySerializer.OnIsSerializeable += this.OnIsSerializable;
		entitySerializer.SerializeEntityRecursive(entities);
		MappingDataNode mappingDataNode = entitySerializer.Write();
		FileCategory category = entitySerializer.GetCategory();
		AfterSerializationEvent message2 = new AfterSerializationEvent(entities, mappingDataNode, category);
		RaiseLocalEvent(message2);
		base.Log.Debug($"Serialized {entitySerializer.EntityData.Count} entities in {_stopwatch.Elapsed}");
		return (Node: mappingDataNode, Category: category);
	}

	public bool TrySaveEntity(EntityUid entity, ResPath path, SerializationOptions? options = null)
	{
		if (_mapQuery.HasComp(entity))
		{
			base.Log.Error($"{ToPrettyString(entity)} is a map. Use {"TrySaveMap"}.");
			return false;
		}
		if (_gridQuery.HasComp(entity))
		{
			base.Log.Error($"{ToPrettyString(entity)} is a grid. Use {"TrySaveGrid"}.");
			return false;
		}
		SerializationOptions obj = options ?? SerializationOptions.Default;
		SerializationOptions value = obj with
		{
			Category = FileCategory.Entity
		};
		MappingDataNode data;
		FileCategory fileCategory;
		try
		{
			(data, fileCategory) = SerializeEntitiesRecursive(new HashSet<EntityUid> { entity }, value);
		}
		catch (Exception value2)
		{
			base.Log.Error($"Caught exception while trying to serialize entity {ToPrettyString(entity)}:\n{value2}");
			return false;
		}
		if (fileCategory != FileCategory.Entity)
		{
			base.Log.Error($"Failed to save {ToPrettyString(entity)} as a singular entity. Output: {fileCategory}");
			return false;
		}
		Write(path, data);
		return true;
	}

	public bool TrySaveMap(MapId mapId, ResPath path, SerializationOptions? options = null)
	{
		if (_mapSystem.TryGetMap(mapId, out var uid))
		{
			return TrySaveMap(uid.Value, path, options);
		}
		base.Log.Error($"Unable to find map {mapId}");
		return false;
	}

	public bool TrySaveMap(EntityUid map, ResPath path, SerializationOptions? options = null)
	{
		if (!_mapQuery.HasComp(map))
		{
			base.Log.Error($"{ToPrettyString(map)} is not a map.");
			return false;
		}
		SerializationOptions obj = options ?? SerializationOptions.Default;
		SerializationOptions value = obj with
		{
			Category = FileCategory.Map
		};
		MappingDataNode data;
		FileCategory fileCategory;
		try
		{
			(data, fileCategory) = SerializeEntitiesRecursive(new HashSet<EntityUid> { map }, value);
		}
		catch (Exception value2)
		{
			base.Log.Error($"Caught exception while trying to serialize map {ToPrettyString(map)}:\n{value2}");
			return false;
		}
		if (fileCategory != FileCategory.Map)
		{
			base.Log.Error($"Failed to save {ToPrettyString(map)} as a map. Output: {fileCategory}");
			return false;
		}
		Write(path, data);
		return true;
	}

	public bool TrySaveGrid(EntityUid grid, ResPath path, SerializationOptions? options = null)
	{
		if (!_gridQuery.HasComp(grid))
		{
			base.Log.Error($"{ToPrettyString(grid)} is not a grid.");
			return false;
		}
		if (_mapQuery.HasComp(grid))
		{
			base.Log.Error($"{ToPrettyString(grid)} is a map, not (just) a grid. Use {"TrySaveMap"}");
			return false;
		}
		SerializationOptions obj = options ?? SerializationOptions.Default;
		SerializationOptions value = obj with
		{
			Category = FileCategory.Grid
		};
		MappingDataNode data;
		FileCategory fileCategory;
		try
		{
			(data, fileCategory) = SerializeEntitiesRecursive(new HashSet<EntityUid> { grid }, value);
		}
		catch (Exception value2)
		{
			base.Log.Error($"Caught exception while trying to serialize grid {ToPrettyString(grid)}:\n{value2}");
			return false;
		}
		if (fileCategory != FileCategory.Grid)
		{
			base.Log.Error($"Failed to save {ToPrettyString(grid)} as a grid. Output: {fileCategory}");
			return false;
		}
		Write(path, data);
		return true;
	}

	public bool TrySaveGeneric(EntityUid uid, ResPath path, out FileCategory category, SerializationOptions? options = null)
	{
		return TrySaveGeneric(new HashSet<EntityUid> { uid }, path, out category, options);
	}

	public bool TrySaveGeneric(HashSet<EntityUid> entities, ResPath path, out FileCategory category, SerializationOptions? options = null)
	{
		category = FileCategory.Unknown;
		if (entities.Count == 0)
		{
			return false;
		}
		SerializationOptions value = options ?? SerializationOptions.Default;
		MappingDataNode data;
		try
		{
			(data, category) = SerializeEntitiesRecursive(entities, value);
		}
		catch (Exception value2)
		{
			base.Log.Error($"Caught exception while trying to serialize entities:\n{value2}");
			return false;
		}
		Write(path, data);
		return true;
	}

	public bool TrySaveAllEntities(ResPath path, SerializationOptions? options = null)
	{
		if (!TrySerializeAllEntities(out MappingDataNode data, options))
		{
			return false;
		}
		Write(path, data);
		return true;
	}

	public bool TrySerializeAllEntities([NotNullWhen(true)] out MappingDataNode? data, SerializationOptions? options = null)
	{
		data = null;
		SerializationOptions obj = options ?? SerializationOptions.Default with
		{
			MissingEntityBehaviour = MissingEntityBehaviour.Error
		};
		SerializationOptions options2 = obj with
		{
			Category = FileCategory.Save
		};
		_stopwatch.Restart();
		base.Log.Info("Serializing all entities");
		HashSet<EntityUid> hashSet = EntityManager.GetEntities().ToHashSet();
		HashSet<MapId> mapIds = _mapSystem.Maps.Keys.ToHashSet();
		BeforeSerializationEvent message = new BeforeSerializationEvent(hashSet, mapIds, FileCategory.Save);
		EntitySerializer entitySerializer = new EntitySerializer(_dependency, options2);
		Queue<EntityUid> queue = new Queue<EntityUid>();
		foreach (EntityUid item in hashSet)
		{
			if (!entitySerializer.IsSerializable(item))
			{
				queue.Enqueue(item);
			}
		}
		if (queue.Count > 0)
		{
			if (options2.MissingEntityBehaviour == MissingEntityBehaviour.Error)
			{
				options2.MissingEntityBehaviour = MissingEntityBehaviour.Ignore;
				base.Log.Error("Attempted to serialize one or more non-serializable entities");
			}
			EntityUid result;
			while (queue.TryDequeue(out result))
			{
				hashSet.Remove(result);
				foreach (EntityUid child in Transform(result)._children)
				{
					queue.Enqueue(child);
				}
			}
		}
		try
		{
			RaiseLocalEvent(message);
			entitySerializer.OnIsSerializeable += this.OnIsSerializable;
			entitySerializer.SerializeEntities(hashSet);
			data = entitySerializer.Write();
			FileCategory category = entitySerializer.GetCategory();
			AfterSerializationEvent message2 = new AfterSerializationEvent(hashSet, data, category);
			RaiseLocalEvent(message2);
			base.Log.Debug($"Serialized {entitySerializer.EntityData.Count} entities in {_stopwatch.Elapsed}");
		}
		catch (Exception value)
		{
			base.Log.Error($"Caught exception while trying to serialize all entities:\n{value}");
			return false;
		}
		return true;
	}
}
