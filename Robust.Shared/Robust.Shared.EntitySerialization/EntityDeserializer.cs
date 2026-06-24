using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Robust.Shared.EntitySerialization.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
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

namespace Robust.Shared.EntitySerialization;

public sealed class EntityDeserializer : ISerializationContext, ITypeSerializer<EntityUid, ValueDataNode>, ITypeReader<EntityUid, ValueDataNode>, ITypeValidator<EntityUid, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<EntityUid, ValueDataNode>, ITypeWriter<EntityUid>, BaseSerializerInterfaces.ITypeInterface<EntityUid>, ITypeSerializer<NetEntity, ValueDataNode>, ITypeReader<NetEntity, ValueDataNode>, ITypeValidator<NetEntity, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<NetEntity, ValueDataNode>, ITypeWriter<NetEntity>, BaseSerializerInterfaces.ITypeInterface<NetEntity>, ITypeSerializer<MapId, ValueDataNode>, ITypeReader<MapId, ValueDataNode>, ITypeValidator<MapId, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<MapId, ValueDataNode>, ITypeWriter<MapId>, BaseSerializerInterfaces.ITypeInterface<MapId>
{
	public record struct EntData(int YamlId, MappingDataNode Node, Dictionary<string, MappingDataNode>? Components, HashSet<string>? MissingComponents, bool PostInit, bool Paused, bool ToDelete);

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

	public readonly Dictionary<EntityUid, EntData> Entities = new Dictionary<EntityUid, EntData>();

	public readonly Dictionary<int, EntData> YamlEntities = new Dictionary<int, EntData>();

	public readonly Dictionary<string, List<EntData>> Prototypes = new Dictionary<string, List<EntData>>();

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

	public EntData? CurrentReadingEntity;

	public string? CurrentComponent;

	private readonly EntityQuery<MapComponent> _mapQuery;

	private readonly EntityQuery<MapGridComponent> _gridQuery;

	private readonly EntityQuery<TransformComponent> _xformQuery;

	private readonly EntityQuery<MetaDataComponent> _metaQuery;

	public SerializationManager.SerializerProvider SerializerProvider { get; } = new SerializationManager.SerializerProvider();

	public bool WritingReadingPrototypes { get; private set; }

	public EntityDeserializer(IDependencyCollection deps, MappingDataNode data, DeserializationOptions options, Dictionary<string, string>? renamedPrototypes = null, HashSet<string>? deletedPrototypes = null)
	{
		deps.InjectDependencies(this);
		_log = _logMan.GetSawmill("entity_deserializer");
		_log.Level = LogLevel.Info;
		SerializerProvider.RegisterSerializer(this);
		Data = data;
		Options = options;
		RenamedPrototypes = renamedPrototypes ?? new Dictionary<string, string>();
		DeletedPrototypes = deletedPrototypes ?? new HashSet<string>();
		_mapQuery = EntMan.GetEntityQuery<MapComponent>();
		_gridQuery = EntMan.GetEntityQuery<MapGridComponent>();
		_xformQuery = EntMan.GetEntityQuery<TransformComponent>();
		_metaQuery = EntMan.GetEntityQuery<MetaDataComponent>();
	}

	public bool TryProcessData()
	{
		ReadMetadata();
		if (Result.Version < 3)
		{
			_log.Error($"Cannot handle this map file version, found v{Result.Version} and require at least v{3}");
			return false;
		}
		if (Result.Version > 7)
		{
			_log.Error($"Cannot handle this map file version, found v{Result.Version} but require at most v{7}");
			return false;
		}
		if (!ValidatePrototypes())
		{
			return false;
		}
		ReadEntities();
		ReadTileMap();
		ReadMapsAndGrids();
		return true;
	}

	public void CreateEntities()
	{
		AllocateEntities();
		if (Options.AssignMapIds)
		{
			AllocateMapIds();
		}
		LoadEntities();
		GetRootEntities();
		RemoveEmptyChunks();
		StoreGridTileMap();
		if (Options.AssignMapIds)
		{
			AssignMapIds();
		}
		CheckCategory();
	}

	public void StartEntities()
	{
		AdoptGrids();
		ValidateMapIds();
		BuildEntityHierarchy();
		StartEntitiesInternal();
		SetMapInitLifestage();
		SetPaused();
		GetRootNodes();
		PauseMaps();
		InitializeMaps();
		ProcessDeletions();
		if (!Options.AssignMapIds)
		{
			return;
		}
		foreach (int mapYamlId in MapYamlIds)
		{
			if (AllocatedMapIds.TryGetValue(mapYamlId, out var _))
			{
				EntMan.EntityExists(UidMap[mapYamlId]);
			}
		}
	}

	private void ReadMetadata()
	{
		MappingDataNode mappingDataNode = Data.Get<MappingDataNode>("meta");
		Result.Version = mappingDataNode.Get<ValueDataNode>("format").AsInt();
		if (mappingDataNode.TryGet("engineVersion", out ValueDataNode node))
		{
			Result.EngineVersion = node.Value;
		}
		if (mappingDataNode.TryGet("forkId", out ValueDataNode node2))
		{
			Result.ForkId = node2.Value;
		}
		if (mappingDataNode.TryGet("forkVersion", out ValueDataNode node3))
		{
			Result.ForkVersion = node3.Value;
		}
		if (mappingDataNode.TryGet("time", out ValueDataNode node4) && DateTime.TryParse(node4.Value, out var result))
		{
			Result.Time = result;
		}
		if (mappingDataNode.TryGet("category", out ValueDataNode node5) && Enum.TryParse<FileCategory>(node5.Value, out var result2))
		{
			Result.Category = result2;
		}
	}

	private bool ValidatePrototypes()
	{
		_stopwatch.Restart();
		bool flag = false;
		string key = ((Result.Version >= 4) ? "proto" : "type");
		foreach (MappingDataNode item in Data.Get<SequenceDataNode>("entities").Cast<MappingDataNode>())
		{
			if (!item.TryGet(key, out ValueDataNode node))
			{
				continue;
			}
			string text = node.Value;
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (RenamedPrototypes.TryGetValue(text, out string value))
				{
					text = value;
				}
				if (DeletedPrototypes.Contains(text))
				{
					_log.Warning("Map contains an obsolete/removed prototype: {0}. This may cause unexpected errors.", text);
				}
				else if (!_proto.HasIndex<EntityPrototype>(text))
				{
					_log.Error("Missing prototype for map: {0}", text);
					flag = true;
				}
			}
		}
		_log.Debug($"Verified entities in {_stopwatch.Elapsed}");
		if (!flag)
		{
			return true;
		}
		_log.Error("Found missing prototypes in map file. Missing prototypes have been dumped to logs.");
		return false;
	}

	private void ReadEntities()
	{
		if (Result.Version == 3)
		{
			ReadEntitiesV3();
			return;
		}
		if (Result.Version < 7)
		{
			ReadEntitiesFallback();
			return;
		}
		foreach (MappingDataNode item3 in Data.Get<SequenceDataNode>("entities").Cast<MappingDataNode>())
		{
			EntProtoId? id = null;
			bool toDelete = false;
			if (item3.TryGet("proto", out ValueDataNode node) && !string.IsNullOrWhiteSpace(node.Value))
			{
				if (!DeletedPrototypes.Contains(node.Value))
				{
					id = ((!RenamedPrototypes.TryGetValue(node.Value, out string value)) ? ((EntProtoId?)node.Value) : ((EntProtoId?)value));
				}
				else
				{
					toDelete = true;
					if (_proto.HasIndex<EntityPrototype>(node.Value))
					{
						id = node.Value;
					}
				}
			}
			SequenceDataNode source = (SequenceDataNode)item3["entities"];
			_proto.TryIndex(id, out EntityPrototype prototype);
			List<EntData> orNew = Prototypes.GetOrNew(prototype?.ID ?? string.Empty);
			foreach (MappingDataNode item4 in source.Cast<MappingDataNode>())
			{
				int num = item4.Get<ValueDataNode>("uid").AsInt();
				ValueDataNode node2;
				bool flag = item4.TryGet("mapInit", out node2) && node2.AsBool();
				ValueDataNode node3;
				bool paused = (item4.TryGet("paused", out node3) ? node3.AsBool() : (!flag));
				(Dictionary<string, MappingDataNode>? Comps, HashSet<string>? Missing) components = GetComponents(item4);
				Dictionary<string, MappingDataNode> item = components.Comps;
				HashSet<string> item2 = components.Missing;
				EntData entData = new EntData(num, item4, item, item2, flag, paused, toDelete);
				orNew.Add(entData);
				YamlEntities.Add(num, entData);
			}
		}
	}

	private void ReadEntitiesV3()
	{
		ValueDataNode node;
		bool flag = Data.Get<MappingDataNode>("meta").TryGet("postmapinit", out node) && !node.AsBool();
		foreach (MappingDataNode item3 in Data.Get<SequenceDataNode>("entities").Cast<MappingDataNode>())
		{
			int num = item3.Get<ValueDataNode>("uid").AsInt();
			EntProtoId? id = null;
			bool toDelete = false;
			if (item3.TryGet("type", out ValueDataNode node2))
			{
				if (DeletedPrototypes.Contains(node2.Value))
				{
					toDelete = true;
					if (_proto.HasIndex<EntityPrototype>(node2.Value))
					{
						id = node2.Value;
					}
				}
				else
				{
					id = ((!RenamedPrototypes.TryGetValue(node2.Value, out string value)) ? ((EntProtoId?)node2.Value) : ((EntProtoId?)value));
				}
			}
			_proto.TryIndex(id, out EntityPrototype prototype);
			List<EntData> orNew = Prototypes.GetOrNew(prototype?.ID ?? string.Empty);
			(Dictionary<string, MappingDataNode>? Comps, HashSet<string>? Missing) components = GetComponents(item3);
			Dictionary<string, MappingDataNode> item = components.Comps;
			HashSet<string> item2 = components.Missing;
			EntData entData = new EntData(num, item3, item, item2, !flag, flag, toDelete);
			orNew.Add(entData);
			YamlEntities.Add(num, entData);
		}
	}

	private void ReadEntitiesFallback()
	{
		ValueDataNode node;
		bool flag = Data.Get<MappingDataNode>("meta").TryGet("postmapinit", out node) && !node.AsBool();
		foreach (MappingDataNode item3 in Data.Get<SequenceDataNode>("entities").Cast<MappingDataNode>())
		{
			EntProtoId? id = null;
			bool toDelete = false;
			if (item3.TryGet("proto", out ValueDataNode node2) && !string.IsNullOrWhiteSpace(node2.Value))
			{
				if (!DeletedPrototypes.Contains(node2.Value))
				{
					id = ((!RenamedPrototypes.TryGetValue(node2.Value, out string value)) ? ((EntProtoId?)node2.Value) : ((EntProtoId?)value));
				}
				else
				{
					toDelete = true;
					if (_proto.HasIndex<EntityPrototype>(node2.Value))
					{
						id = node2.Value;
					}
				}
			}
			SequenceDataNode source = (SequenceDataNode)item3["entities"];
			_proto.TryIndex(id, out EntityPrototype prototype);
			List<EntData> orNew = Prototypes.GetOrNew(prototype?.ID ?? string.Empty);
			foreach (MappingDataNode item4 in source.Cast<MappingDataNode>())
			{
				int num = item4.Get<ValueDataNode>("uid").AsInt();
				(Dictionary<string, MappingDataNode>? Comps, HashSet<string>? Missing) components = GetComponents(item4);
				Dictionary<string, MappingDataNode> item = components.Comps;
				HashSet<string> item2 = components.Missing;
				EntData entData = new EntData(num, item4, item, item2, !flag, flag, toDelete);
				orNew.Add(entData);
				YamlEntities.Add(num, entData);
			}
		}
	}

	private (Dictionary<string, MappingDataNode>? Comps, HashSet<string>? Missing) GetComponents(MappingDataNode node)
	{
		Dictionary<string, MappingDataNode> dictionary = null;
		HashSet<string> hashSet = null;
		if (node.TryGet("components", out SequenceDataNode node2))
		{
			dictionary = new Dictionary<string, MappingDataNode>(node2.Count);
			foreach (MappingDataNode item in node2.Cast<MappingDataNode>())
			{
				string value = ((ValueDataNode)item["type"]).Value;
				item.Remove("type");
				dictionary.Add(value, item);
			}
		}
		if (node.TryGet("missingComponents", out SequenceDataNode node3))
		{
			hashSet = new HashSet<string>(node3.Count);
			foreach (DataNode item2 in node3)
			{
				hashSet.Add(((ValueDataNode)item2).Value);
			}
		}
		node.Remove("components");
		node.Remove("missingComponents");
		return (Comps: dictionary, Missing: hashSet);
	}

	private void ReadTileMap()
	{
		_stopwatch.Restart();
		MappingDataNode mappingDataNode = Data.Get<MappingDataNode>("tilemap");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (TileAliasPrototype item in _proto.EnumeratePrototypes<TileAliasPrototype>())
		{
			dictionary.Add(item.ID, item.Target);
		}
		foreach (KeyValuePair<string, DataNode> child in mappingDataNode.Children)
		{
			child.Deconstruct(out var key, out var value);
			string s = key;
			DataNode dataNode = value;
			int key2 = int.Parse(s, CultureInfo.InvariantCulture);
			string text = ((ValueDataNode)dataNode).Value;
			if (dictionary.TryGetValue(text, out var value2))
			{
				text = value2;
			}
			TileMap.Add(key2, text);
		}
		_log.Debug($"Read tilemap in {_stopwatch.Elapsed}");
	}

	private void AllocateEntities()
	{
		_stopwatch.Restart();
		foreach (KeyValuePair<string, List<EntData>> prototype2 in Prototypes)
		{
			prototype2.Deconstruct(out var key, out var value);
			string text = key;
			List<EntData> list = value;
			EntityPrototype prototype = ((text == string.Empty) ? null : _proto.Index<EntityPrototype>(text));
			foreach (EntData item in list)
			{
				EntityUid entityUid = EntMan.AllocEntity(prototype);
				Result.Entities.Add(entityUid);
				UidMap.Add(item.YamlId, entityUid);
				Entities.Add(entityUid, item);
				if (item.PostInit)
				{
					PostMapInit.Add(entityUid);
				}
				if (item.Paused)
				{
					Paused.Add(entityUid);
				}
				if (item.ToDelete)
				{
					ToDelete.Add(entityUid);
				}
				if (Options.StoreYamlUids)
				{
					EntMan.AddComponent<YamlUidComponent>(entityUid).Uid = item.YamlId;
				}
			}
		}
		_log.Debug($"Allocated {Entities.Count} entities in {_stopwatch.Elapsed}");
	}

	private void AllocateMapIds()
	{
		if (Result.Version < 7)
		{
			return;
		}
		foreach (int mapYamlId in MapYamlIds)
		{
			EntityUid ent = UidMap[mapYamlId];
			AllocatedMapIds[mapYamlId] = _map.AllocateMapId(ent);
		}
	}

	private void ReadMapsAndGrids()
	{
		if (Result.Version >= 7)
		{
			ReadYamlIdList(Data, "maps", MapYamlIds);
			ReadYamlIdList(Data, "grids", GridYamlIds);
			ReadYamlIdList(Data, "orphans", OrphanYamlIds);
			ReadYamlIdList(Data, "nullspace", NullspaceYamlIds);
		}
	}

	private void ReadYamlIdList(MappingDataNode data, string key, List<int> list)
	{
		SequenceDataNode sequenceDataNode = data.Get<SequenceDataNode>(key);
		list.EnsureCapacity(sequenceDataNode.Count);
		foreach (ValueDataNode item2 in sequenceDataNode)
		{
			int item = item2.AsInt();
			list.Add(item);
		}
	}

	private void LoadEntities()
	{
		_stopwatch.Restart();
		foreach (var (entityUid2, value) in Entities)
		{
			try
			{
				CurrentReadingEntity = value;
				LoadEntity(entityUid2, _metaQuery.Comp(entityUid2), value.Components, value.MissingComponents);
			}
			catch (Exception value2)
			{
				ToDelete.Add(entityUid2);
				_log.Error($"Encountered error while loading entity. Yaml uid: {value.YamlId}. Loaded loaded entity: {EntMan.ToPrettyString(entityUid2)}. Error:\n{value2}.");
			}
		}
		CurrentReadingEntity = null;
		_log.Debug($"Loaded {Entities.Count} entities in {_stopwatch.Elapsed}");
	}

	private void LoadEntity(EntityUid uid, MetaDataComponent meta, Dictionary<string, MappingDataNode>? comps, HashSet<string>? missingComps)
	{
		EntityPrototype entityPrototype = meta.EntityPrototype;
		_components.Clear();
		string key;
		MappingDataNode value;
		if (comps != null)
		{
			_components.EnsureCapacity(comps.Count);
			foreach (KeyValuePair<string, MappingDataNode> comp in comps)
			{
				comp.Deconstruct(out key, out value);
				string text = key;
				MappingDataNode mappingDataNode = value;
				if (!_factory.TryGetRegistration(text, out ComponentRegistration _))
				{
					if (!_factory.IsIgnored(text))
					{
						_log.Error($"Encountered unregistered component ({text}) while loading entity {EntMan.ToPrettyString(uid)}");
					}
				}
				else
				{
					MappingDataNode value2 = mappingDataNode;
					if (entityPrototype != null && entityPrototype.Components.TryGetValue(text, out EntityPrototype.ComponentRegistryEntry value3))
					{
						value2 = _seriMan.CombineMappings(mappingDataNode, value3.Mapping);
					}
					_components.Add(text, value2);
				}
			}
		}
		if (entityPrototype != null)
		{
			foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> component5 in entityPrototype.Components)
			{
				component5.Deconstruct(out key, out var value4);
				string text2 = key;
				EntityPrototype.ComponentRegistryEntry componentRegistryEntry = value4;
				if ((missingComps != null && missingComps.Contains(text2)) || _components.ContainsKey(text2))
				{
					continue;
				}
				CurrentComponent = text2;
				ComponentRegistration registration2 = _factory.GetRegistration(text2);
				if (!EntMan.TryGetComponent(uid, registration2.Idx, out IComponent component))
				{
					IComponent component2 = _factory.GetComponent(registration2);
					EntMan.AddComponent(uid, component2);
					component = component2;
				}
				_seriMan.CopyTo(componentRegistryEntry.Component, ref component, this, skipHook: false, notNullableOverride: true);
				if (!componentRegistryEntry.Component.NetSyncEnabled)
				{
					ushort? netID = registration2.NetID;
					if (netID.HasValue)
					{
						ushort valueOrDefault = netID.GetValueOrDefault();
						meta.NetComponents.Remove(valueOrDefault);
					}
				}
			}
		}
		foreach (KeyValuePair<string, MappingDataNode> component6 in _components)
		{
			component6.Deconstruct(out key, out value);
			string text3 = key;
			MappingDataNode node = value;
			CurrentComponent = text3;
			ComponentRegistration registration3 = _factory.GetRegistration(text3);
			if (!EntMan.TryGetComponent(uid, registration3.Idx, out IComponent component3))
			{
				IComponent component4 = (IComponent)_seriMan.Read(registration3.Type, node, this);
				if (component4 is ISerializationHooks)
				{
					component3 = _factory.GetComponent(registration3);
					EntMan.AddComponent(uid, component3);
					_seriMan.CopyTo(component4, ref component3, this, skipHook: false, notNullableOverride: true);
				}
				else
				{
					_deps.InjectDependencies(component4);
					EntMan.AddComponent(uid, component4);
				}
			}
			else
			{
				IComponent source = (IComponent)_seriMan.Read(registration3.Type, node, this);
				_seriMan.CopyTo(source, ref component3, this, skipHook: false, notNullableOverride: true);
			}
		}
		_components.Clear();
		CurrentComponent = null;
		if (missingComps != null && missingComps.Count > 0)
		{
			meta.LastComponentRemoved = Timing.CurTick;
		}
	}

	private void GetRootEntities()
	{
		if (Result.Version < 7)
		{
			GetRootEntitiesFallback();
			return;
		}
		foreach (int mapYamlId in MapYamlIds)
		{
			if (UidMap.TryGetValue(mapYamlId, out var value) && _mapQuery.TryComp(value, out MapComponent component))
			{
				Result.Maps.Add((Owner: value, Comp: component));
				EntMan.EnsureComponent<LoadedMapComponent>(value);
				continue;
			}
			_log.Error($"Missing map entity: {EntMan.ToPrettyString(value)}. YamlId: {mapYamlId}");
		}
		foreach (int gridYamlId in GridYamlIds)
		{
			if (UidMap.TryGetValue(gridYamlId, out var value2) && _gridQuery.TryComp(value2, out MapGridComponent component2))
			{
				Result.Grids.Add((Owner: value2, Comp: component2));
				continue;
			}
			_log.Error($"Missing grid entity: {EntMan.ToPrettyString(value2)}. YamlId: {gridYamlId}");
		}
		foreach (int orphanYamlId in OrphanYamlIds)
		{
			if (!UidMap.TryGetValue(orphanYamlId, out var value3))
			{
				_log.Error($"Missing orphan entity with YamlId: {orphanYamlId}");
			}
			else if (_mapQuery.HasComponent(value3) || _xformQuery.Comp(value3).ParentUid.IsValid())
			{
				_log.Error($"Entity {EntMan.ToPrettyString(value3)} was incorrectly labelled as an orphan? YamlId: {orphanYamlId}");
			}
			else
			{
				Result.Orphans.Add(value3);
			}
		}
		foreach (int nullspaceYamlId in NullspaceYamlIds)
		{
			if (!UidMap.TryGetValue(nullspaceYamlId, out var value4))
			{
				_log.Error($"Missing nullspace entity with YamlId: {nullspaceYamlId}");
			}
			else if (_mapQuery.HasComponent(value4) || _xformQuery.Comp(value4).ParentUid.IsValid())
			{
				_log.Error($"Entity {EntMan.ToPrettyString(value4)} was incorrectly labelled as a null-space entity?");
			}
			else
			{
				Result.NullspaceEntities.Add(value4);
			}
		}
	}

	public void AssignMapIds()
	{
		foreach (Entity<MapComponent> map in Result.Maps)
		{
			_map.AssignMapId(map);
		}
	}

	private void GetRootEntitiesFallback()
	{
		foreach (EntityUid entity in Result.Entities)
		{
			if (_gridQuery.TryComp(entity, out MapGridComponent component))
			{
				Result.Grids.Add((Owner: entity, Comp: component));
				if (_xformQuery.Comp(entity).ParentUid == EntityUid.Invalid && !_mapQuery.HasComp(entity))
				{
					Result.Orphans.Add(entity);
				}
			}
			if (_mapQuery.TryComp(entity, out MapComponent component2))
			{
				Result.Maps.Add((Owner: entity, Comp: component2));
				EntMan.EnsureComponent<LoadedMapComponent>(entity);
			}
		}
	}

	private void RemoveEmptyChunks()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid key in Entities.Keys)
		{
			if (!_gridQuery.TryGetComponent(key, out MapGridComponent component))
			{
				continue;
			}
			foreach (var (val2, mapChunk2) in component.Chunks)
			{
				if (mapChunk2.FilledTiles <= 0)
				{
					_log.Warning($"Encountered empty chunk while deserializing map. Grid: {EntMan.ToPrettyString(key)}. Chunk index: {val2}");
					component.Chunks.Remove(val2);
				}
			}
		}
	}

	private void StoreGridTileMap()
	{
		foreach (Entity<MapGridComponent> grid in Result.Grids)
		{
			EntMan.EnsureComponent<MapSaveTileMapComponent>(grid).TileMap = TileMap.ShallowClone();
		}
	}

	private void BuildEntityHierarchy()
	{
		_stopwatch.Restart();
		HashSet<EntityUid> processed = new HashSet<EntityUid>(Result.Entities.Count);
		foreach (EntityUid entity in Result.Entities)
		{
			BuildEntityHierarchy(entity, processed);
		}
		_log.Debug($"Built entity hierarchy for {Result.Entities.Count} entities in {_stopwatch.Elapsed}");
	}

	private void CheckCategory()
	{
		if (Result.Version < 7)
		{
			InferCategory();
			return;
		}
		switch (Result.Category)
		{
		case FileCategory.Map:
			if (Result.Maps.Count != 1 || Result.Orphans.Count != 0)
			{
				_log.Error($"Expected file to contain a single map, but instead found {Result.Maps.Count} maps and {Result.Orphans.Count} orphans");
			}
			break;
		case FileCategory.Grid:
			if (Result.Maps.Count != 0 || Result.Grids.Count != 1 || Result.Orphans.Count != 1 || !(Result.Orphans.First() == Result.Grids.First().Owner))
			{
				_log.Error($"Expected file to contain a single grid, but instead found {Result.Grids.Count} grids and {Result.Orphans.Count} orphans");
			}
			break;
		case FileCategory.Entity:
			if (Result.Maps.Count != 0 || Result.Grids.Count != 0 || Result.Orphans.Count != 1)
			{
				_log.Error($"Expected file to contain a orphaned entity, but instead found {Result.Orphans.Count} orphans");
			}
			break;
		case FileCategory.Save:
			break;
		}
	}

	private void InferCategory()
	{
		if (Result.Category == FileCategory.Unknown)
		{
			if (Result.Maps.Count == 1)
			{
				Result.Category = FileCategory.Map;
			}
			else if (Result.Maps.Count == 0 && Result.Grids.Count == 1)
			{
				Result.Category = FileCategory.Grid;
			}
		}
	}

	private void AdoptGrids()
	{
		foreach (Entity<MapGridComponent> grid in Result.Grids)
		{
			if (_mapQuery.HasComponent(grid.Owner))
			{
				continue;
			}
			TransformComponent transformComponent = _xformQuery.Comp(grid.Owner);
			if (!transformComponent.ParentUid.IsValid())
			{
				if (Options.LogOrphanedGrids)
				{
					_log.Error("Encountered an orphaned grid. Automatically creating a map for the grid.");
				}
				Entity<MapComponent, MetaDataComponent> entity = _map.CreateUninitializedMap();
				_map.AssignMapId(entity);
				Result.Entities.Add(entity);
				Result.Maps.Add(entity);
				Result.Orphans.Remove(grid.Owner);
				transformComponent._parent = entity.Owner;
			}
		}
	}

	private void ValidateMapIds()
	{
		foreach (Entity<MapComponent> map in Result.Maps)
		{
			if (map.Comp.MapId == MapId.Nullspace || !_map.TryGetMap(map.Comp.MapId, out var uid) || uid != map.Owner)
			{
				throw new Exception($"Map entity {EntMan.ToPrettyString(map)} has not been assigned a map id");
			}
		}
	}

	private void PauseMaps()
	{
		if (!Options.PauseMaps)
		{
			return;
		}
		foreach (Entity<MapComponent> map in Result.Maps)
		{
			_map.SetPaused(map, paused: true);
		}
	}

	private void BuildEntityHierarchy(EntityUid uid, HashSet<EntityUid> processed)
	{
		if (processed.Add(uid) && _xformQuery.TryComp(uid, out TransformComponent component))
		{
			EntityUid parentUid = component.ParentUid;
			if (parentUid != EntityUid.Invalid)
			{
				BuildEntityHierarchy(parentUid, processed);
			}
			if (Result.Entities.Contains(uid))
			{
				SortedEntities.Add(uid);
			}
		}
	}

	private void StartEntitiesInternal()
	{
		_stopwatch.Restart();
		foreach (EntityUid sortedEntity in SortedEntities)
		{
			StartupEntity(sortedEntity, _metaQuery.GetComponent(sortedEntity));
		}
		_log.Debug($"Started up {Result.Entities.Count} entities in {_stopwatch.Elapsed}");
	}

	private void StartupEntity(EntityUid uid, MetaDataComponent metadata)
	{
		ResetNetTicks(uid, metadata);
		EntMan.InitializeEntity(uid, metadata);
		EntMan.StartEntity(uid);
	}

	private void ResetNetTicks(EntityUid uid, MetaDataComponent metadata)
	{
		if (!Entities.TryGetValue(uid, out var value))
		{
			return;
		}
		EntityPrototype entityPrototype = metadata.EntityPrototype;
		if (entityPrototype == null || value.Components == null)
		{
			return;
		}
		foreach (IComponent value2 in metadata.NetComponents.Values)
		{
			string componentName = _factory.GetComponentName(value2.GetType());
			if (!value.Components.ContainsKey(componentName))
			{
				value2.ClearTicks();
			}
			else if (entityPrototype.Components.ContainsKey(componentName))
			{
				value2.ClearCreationTick();
			}
		}
	}

	private void SetMapInitLifestage()
	{
		if (PostMapInit.Count == 0)
		{
			return;
		}
		_stopwatch.Restart();
		foreach (EntityUid item in PostMapInit)
		{
			if (_metaQuery.TryComp(item, out MetaDataComponent component))
			{
				EntMan.SetLifeStage(component, EntityLifeStage.MapInitialized);
			}
		}
		_log.Debug($"Finished flagging mapinit in {_stopwatch.Elapsed}");
	}

	private void SetPaused()
	{
		if (Paused.Count == 0)
		{
			return;
		}
		_stopwatch.Restart();
		TimeSpan curTime = Timing.CurTime;
		EntityPausedEvent args = default(EntityPausedEvent);
		foreach (EntityUid item in Paused)
		{
			if (_metaQuery.TryComp(item, out MetaDataComponent component))
			{
				component.PauseTime = curTime;
				EntMan.EventBus.RaiseLocalEvent(item, ref args);
			}
		}
		_log.Debug($"Finished setting PauseTime in {_stopwatch.Elapsed}");
	}

	private void InitializeMaps()
	{
		if (!Options.InitializeMaps)
		{
			if (Options.PauseMaps)
			{
				return;
			}
			{
				foreach (Entity<MapComponent> map in Result.Maps)
				{
					if ((int)_metaQuery.Comp(map.Owner).EntityLifeStage < 3)
					{
						_map.SetPaused(map, paused: true);
					}
				}
				return;
			}
		}
		foreach (Entity<MapComponent> map2 in Result.Maps)
		{
			if (!map2.Comp.MapInitialized)
			{
				_map.InitializeMap(map2, !Options.PauseMaps);
			}
		}
	}

	private void ProcessDeletions()
	{
		foreach (EntityUid item in ToDelete)
		{
			EntMan.DeleteEntity(item);
			Result.Entities.Remove(item);
		}
	}

	private void GetRootNodes()
	{
		Result.RootNodes.UnionWith(Result.Orphans);
		Result.RootNodes.UnionWith(Result.NullspaceEntities);
		foreach (Entity<MapComponent> map in Result.Maps)
		{
			Result.RootNodes.Add(map.Owner);
		}
	}

	ValidationNode ITypeValidator<EntityUid, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		if (node.Value == "invalid")
		{
			return new ValidatedValueNode(node);
		}
		if (!int.TryParse(node.Value, out var _))
		{
			return new ErrorNode(node, "Invalid EntityUid");
		}
		return new ValidatedValueNode(node);
	}

	DataNode ITypeWriter<EntityUid>.Write(ISerializationManager serializationManager, EntityUid value, IDependencyCollection dependencies, bool alwaysWrite, ISerializationContext? context)
	{
		if (!value.IsValid())
		{
			return new ValueDataNode("invalid");
		}
		return new ValueDataNode(value.Id.ToString(CultureInfo.InvariantCulture));
	}

	EntityUid ITypeReader<EntityUid, ValueDataNode>.Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<EntityUid>? _)
	{
		EntData? currentReadingEntity;
		string message;
		if (node.Value == "invalid")
		{
			if (CurrentComponent == "Transform")
			{
				return EntityUid.Invalid;
			}
			if (!Options.LogInvalidEntities)
			{
				return EntityUid.Invalid;
			}
			currentReadingEntity = CurrentReadingEntity;
			object obj;
			if (currentReadingEntity.HasValue)
			{
				EntData valueOrDefault = currentReadingEntity.GetValueOrDefault();
				obj = $"Encountered invalid EntityUid reference while reading entity {valueOrDefault.YamlId}, component: {CurrentComponent}";
			}
			else
			{
				obj = "Encountered invalid EntityUid reference";
			}
			message = (string)obj;
			_log.Error(message);
			return EntityUid.Invalid;
		}
		if (int.TryParse(node.Value, out var result) && UidMap.TryGetValue(result, out var value))
		{
			return value;
		}
		currentReadingEntity = CurrentReadingEntity;
		object obj2;
		if (currentReadingEntity.HasValue)
		{
			EntData valueOrDefault2 = currentReadingEntity.GetValueOrDefault();
			obj2 = $"Encountered unknown entity yaml uid while reading entity {valueOrDefault2.YamlId}, component: {CurrentComponent}";
		}
		else
		{
			obj2 = "Encountered unknown entity yaml uid";
		}
		message = (string)obj2;
		_log.Error(message);
		return EntityUid.Invalid;
	}

	ValidationNode ITypeValidator<NetEntity, ValueDataNode>.Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		if (node.Value == "invalid")
		{
			return new ValidatedValueNode(node);
		}
		if (!int.TryParse(node.Value, out var _))
		{
			return new ErrorNode(node, "Invalid NetEntity");
		}
		return new ValidatedValueNode(node);
	}

	NetEntity ITypeReader<NetEntity, ValueDataNode>.Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<NetEntity>? instanceProvider)
	{
		EntityUid entityUid = serializationManager.Read<EntityUid>(node, context);
		if (EntMan.TryGetNetEntity(entityUid, out var netEntity))
		{
			return netEntity.Value;
		}
		_log.Error($"Failed to get NetEntity entity {EntMan.ToPrettyString(entityUid)}");
		return NetEntity.Invalid;
	}

	DataNode ITypeWriter<NetEntity>.Write(ISerializationManager serializationManager, NetEntity value, IDependencyCollection dependencies, bool alwaysWrite, ISerializationContext? context)
	{
		if (!value.IsValid())
		{
			return new ValueDataNode("invalid");
		}
		return new ValueDataNode(value.Id.ToString(CultureInfo.InvariantCulture));
	}

	ValidationNode ITypeValidator<MapId, ValueDataNode>.Validate(ISerializationManager seri, ValueDataNode node, IDependencyCollection deps, ISerializationContext? context)
	{
		return seri.ValidateNode<EntityUid>(node, context);
	}

	MapId ITypeReader<MapId, ValueDataNode>.Read(ISerializationManager seri, ValueDataNode node, IDependencyCollection deps, SerializationHookContext hookCtx, ISerializationContext? ctx, ISerializationManager.InstantiationDelegate<MapId>? instanceProvider)
	{
		if (!Options.AssignMapIds || Result.Version < 7)
		{
			_log.Error("Cannot deserialize map ids without pre-allocated ids");
			return MapId.Nullspace;
		}
		if (int.TryParse(node.Value, out var result) && AllocatedMapIds.TryGetValue(result, out var value))
		{
			return value;
		}
		EntData? currentReadingEntity = CurrentReadingEntity;
		object obj;
		if (currentReadingEntity.HasValue)
		{
			EntData valueOrDefault = currentReadingEntity.GetValueOrDefault();
			obj = $"Encountered unknown yaml map id while reading entity {valueOrDefault.YamlId}, component: {CurrentComponent}";
		}
		else
		{
			obj = "Encountered unknown yaml map id";
		}
		string message = (string)obj;
		_log.Error(message);
		return MapId.Nullspace;
	}

	DataNode ITypeWriter<MapId>.Write(ISerializationManager seri, MapId value, IDependencyCollection deps, bool alwaysWrite, ISerializationContext? ctx)
	{
		return seri.WriteValue(_map.GetMapOrInvalid(value), alwaysWrite, ctx);
	}
}
