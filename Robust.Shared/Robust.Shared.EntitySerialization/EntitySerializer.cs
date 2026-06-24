using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Robust.Shared.Configuration;
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

namespace Robust.Shared.EntitySerialization;

public sealed class EntitySerializer : ISerializationContext, ITypeSerializer<EntityUid, ValueDataNode>, ITypeReader<EntityUid, ValueDataNode>, ITypeValidator<EntityUid, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<EntityUid, ValueDataNode>, ITypeWriter<EntityUid>, BaseSerializerInterfaces.ITypeInterface<EntityUid>, ITypeSerializer<NetEntity, ValueDataNode>, ITypeReader<NetEntity, ValueDataNode>, ITypeValidator<NetEntity, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<NetEntity, ValueDataNode>, ITypeWriter<NetEntity>, BaseSerializerInterfaces.ITypeInterface<NetEntity>, ITypeSerializer<MapId, ValueDataNode>, ITypeReader<MapId, ValueDataNode>, ITypeValidator<MapId, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<MapId, ValueDataNode>, ITypeWriter<MapId>, BaseSerializerInterfaces.ITypeInterface<MapId>
{
	public delegate void IsSerializableDelegate(Entity<MetaDataComponent> ent, ref bool serializable);

	public const int MapFormatVersion = 7;

	[Dependency]
	public readonly EntityManager EntMan;

	[Dependency]
	public readonly IGameTiming Timing;

	[Dependency]
	private readonly IComponentFactory _factory;

	[Dependency]
	private readonly ISerializationManager _serialization;

	[Dependency]
	private readonly ITileDefinitionManager _tileDef;

	[Dependency]
	private readonly IConfigurationManager _conf;

	[Dependency]
	private readonly ILogManager _logMan;

	[Dependency]
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

	public event IsSerializableDelegate? OnIsSerializeable;

	public EntitySerializer(IDependencyCollection dependency, SerializationOptions options)
	{
		dependency.InjectDependencies(this);
		_log = _logMan.GetSawmill("entity_serializer");
		SerializerProvider.RegisterSerializer(this);
		_metaName = _factory.GetComponentName<MetaDataComponent>();
		_xformName = _factory.GetComponentName<TransformComponent>();
		_emptyMetaNode = _serialization.WriteValueAs<MappingDataNode>(typeof(MetaDataComponent), new MetaDataComponent(), alwaysWrite: true, this);
		CurrentComponent = _xformName;
		_emptyXformNode = _serialization.WriteValueAs<MappingDataNode>(typeof(TransformComponent), new TransformComponent(), alwaysWrite: true, this);
		CurrentComponent = null;
		_yamlQuery = EntMan.GetEntityQuery<YamlUidComponent>();
		_gridQuery = EntMan.GetEntityQuery<MapGridComponent>();
		_mapQuery = EntMan.GetEntityQuery<MapComponent>();
		_metaQuery = EntMan.GetEntityQuery<MetaDataComponent>();
		_xformQuery = EntMan.GetEntityQuery<TransformComponent>();
		Options = options;
	}

	public bool IsSerializable(Entity<MetaDataComponent?> ent)
	{
		if (ent.Comp == null && !EntMan.TryGetComponent<MetaDataComponent>(ent.Owner, out ent.Comp))
		{
			return false;
		}
		EntityPrototype? entityPrototype = ent.Comp.EntityPrototype;
		if (entityPrototype != null && !entityPrototype.MapSavable)
		{
			return false;
		}
		bool serializable = true;
		this.OnIsSerializeable?.Invoke(ent, ref serializable);
		return serializable;
	}

	public void SerializeEntity(EntityUid uid)
	{
		if (!IsSerializable(uid))
		{
			throw new Exception($"{EntMan.ToPrettyString(uid)} is not serializable");
		}
		ReserveYamlId(uid);
		SerializeEntityInternal(uid);
		if (_autoInclude.Count != 0)
		{
			ProcessAutoInclude();
		}
	}

	public void SerializeEntities(HashSet<EntityUid> entities)
	{
		foreach (EntityUid entity in entities)
		{
			if (!IsSerializable(entity))
			{
				throw new Exception($"{EntMan.ToPrettyString(entity)} is not serializable");
			}
		}
		ReserveYamlIds(entities);
		SerializeEntitiesInternal(entities);
	}

	public void SerializeEntityRecursive(EntityUid root)
	{
		if (!IsSerializable(root))
		{
			throw new Exception($"{EntMan.ToPrettyString(root)} is not serializable");
		}
		Truncate = _xformQuery.GetComponent(root).ParentUid;
		Truncated.Add(Truncate);
		InitializeTileMap(root);
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		RecursivelyIncludeChildren(root, hashSet);
		ReserveYamlIds(hashSet);
		SerializeEntitiesInternal(hashSet);
		Truncate = EntityUid.Invalid;
	}

	public void SerializeEntityRecursive(HashSet<EntityUid> roots)
	{
		if (roots.Count == 0)
		{
			return;
		}
		InitializeTileMap(roots.First());
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		List<(EntityUid, HashSet<EntityUid>)> list = new List<(EntityUid, HashSet<EntityUid>)>();
		foreach (EntityUid root in roots)
		{
			if (!IsSerializable(root))
			{
				throw new Exception($"{EntMan.ToPrettyString(root)} is not serializable");
			}
			HashSet<EntityUid> hashSet2 = new HashSet<EntityUid>();
			RecursivelyIncludeChildren(root, hashSet2);
			list.Add((root, hashSet2));
			hashSet.UnionWith(hashSet2);
		}
		ReserveYamlIds(hashSet);
		foreach (var item3 in list)
		{
			EntityUid item = item3.Item1;
			HashSet<EntityUid> item2 = item3.Item2;
			Truncate = _xformQuery.GetComponent(item).ParentUid;
			Truncated.Add(Truncate);
			SerializeEntitiesInternal(item2);
			Truncate = EntityUid.Invalid;
		}
	}

	private void InitializeTileMap(EntityUid root)
	{
		if (!FindSavedTileMap(root, out Dictionary<int, string> map))
		{
			return;
		}
		foreach (var (num2, name) in map)
		{
			if (_tileDef.TryGetDefinition(name, out ITileDefinition definition))
			{
				_tileMap.TryAdd(definition.TileId, num2);
				_yamlTileIds.Add(num2);
			}
		}
	}

	private bool FindSavedTileMap(EntityUid root, [NotNullWhen(true)] out Dictionary<int, string>? map)
	{
		if (EntMan.TryGetComponent<MapSaveTileMapComponent>(root, out MapSaveTileMapComponent component))
		{
			map = component.TileMap;
			return true;
		}
		map = null;
		if (!_mapQuery.HasComponent(root))
		{
			return false;
		}
		foreach (EntityUid child in _xformQuery.GetComponent(root)._children)
		{
			if (EntMan.TryGetComponent<MapSaveTileMapComponent>(child, out MapSaveTileMapComponent component2))
			{
				map = component2.TileMap;
				return true;
			}
		}
		return false;
	}

	private void ProcessAutoInclude()
	{
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		switch (Options.MissingEntityBehaviour)
		{
		case MissingEntityBehaviour.PartialInclude:
			foreach (EntityUid item in _autoInclude)
			{
				RecursivelyIncludeParents(item, hashSet);
			}
			break;
		case MissingEntityBehaviour.IncludeNullspace:
		case MissingEntityBehaviour.AutoInclude:
		{
			HashSet<EntityUid> hashSet2 = new HashSet<EntityUid>();
			foreach (EntityUid item2 in _autoInclude)
			{
				GetRootNode(item2, hashSet2);
			}
			foreach (EntityUid item3 in hashSet2)
			{
				RecursivelyIncludeChildren(item3, hashSet);
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		_autoInclude.Clear();
		SerializeEntitiesInternal(hashSet);
	}

	private void RecursivelyIncludeChildren(EntityUid uid, HashSet<EntityUid> ents)
	{
		if (!IsSerializable(uid))
		{
			return;
		}
		ents.Add(uid);
		foreach (EntityUid child in _xformQuery.GetComponent(uid)._children)
		{
			RecursivelyIncludeChildren(child, ents);
		}
	}

	private void GetRootNode(EntityUid uid, HashSet<EntityUid> ents)
	{
		if (!IsSerializable(uid))
		{
			throw new NotSupportedException($"Attempted to auto-include an unserializable entity: {EntMan.ToPrettyString(uid)}");
		}
		TransformComponent component = _xformQuery.GetComponent(uid);
		while (component.ParentUid.IsValid() && component.ParentUid != Truncate)
		{
			uid = component.ParentUid;
			component = _xformQuery.GetComponent(uid);
			if (!IsSerializable(uid))
			{
				throw new NotSupportedException($"Encountered an un-serializable parent entity: {EntMan.ToPrettyString(uid)}");
			}
		}
		ents.Add(uid);
	}

	private void RecursivelyIncludeParents(EntityUid uid, HashSet<EntityUid> ents)
	{
		while (uid.IsValid() && uid != Truncate && ents.Add(uid))
		{
			if (!IsSerializable(uid))
			{
				throw new NotSupportedException($"Encountered an un-serializable parent entity: {EntMan.ToPrettyString(uid)}");
			}
			uid = _xformQuery.GetComponent(uid).ParentUid;
		}
	}

	private void SerializeEntitiesInternal(HashSet<EntityUid> entities)
	{
		foreach (EntityUid entity in entities)
		{
			SerializeEntityInternal(entity);
		}
		if (_autoInclude.Count != 0)
		{
			ProcessAutoInclude();
		}
	}

	private void SerializeEntityInternal(EntityUid uid)
	{
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		int yamlUid = GetYamlUid(uid);
		if (EntityData.ContainsKey(yamlUid))
		{
			return;
		}
		MetaDataComponent component = _metaQuery.GetComponent(uid);
		string key = component.EntityPrototype?.ID ?? string.Empty;
		EntityLifeStage entityLifeStage = component.EntityLifeStage;
		if ((int)entityLifeStage > 1)
		{
			if ((int)entityLifeStage >= 4)
			{
				_log.Error($"Encountered terminating or deleted entity: {EntMan.ToPrettyString(uid)}");
			}
		}
		else
		{
			_log.Error($"Encountered an uninitialized entity: {EntMan.ToPrettyString(uid)}");
		}
		CurrentEntityYamlUid = yamlUid;
		CurrentEntity = (Owner: uid, Comp: component);
		Prototypes.GetOrNew(key).Add(yamlUid);
		TransformComponent component2 = _xformQuery.GetComponent(uid);
		if (_mapQuery.HasComp(uid))
		{
			Maps.Add(yamlUid);
		}
		else if (component2.ParentUid == EntityUid.Invalid)
		{
			Nullspace.Add(yamlUid);
		}
		if (_gridQuery.HasComp(uid))
		{
			Grids.Add(yamlUid);
		}
		MappingDataNode mappingDataNode = new MappingDataNode { 
		{
			"uid",
			yamlUid.ToString(CultureInfo.InvariantCulture)
		} };
		EntityData[yamlUid] = (uid, mappingDataNode);
		Dictionary<string, MappingDataNode> protoCache = GetProtoCache(component.EntityPrototype);
		if (component.EntityLifeStage == EntityLifeStage.MapInitialized)
		{
			if (Options.ExpectPreInit)
			{
				_log.Error($"Expected all entities to be pre-mapinit, but encountered post-init entity: {EntMan.ToPrettyString(uid)}");
			}
			mappingDataNode.Add("mapInit", "true");
			if (component.EntityPaused)
			{
				mappingDataNode.Add("paused", "true");
			}
		}
		else if (!component.EntityPaused)
		{
			mappingDataNode.Add("paused", "false");
		}
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		if (component2.NoLocalRotation && component2.LocalRotation != Angle.op_Implicit(0f))
		{
			_log.Error($"Encountered a no-rotation entity with non-zero local rotation: {EntMan.ToPrettyString(uid)}");
			component2._localRotation = Angle.op_Implicit(0f);
		}
		try
		{
			SerializeComponents(uid, protoCache, sequenceDataNode);
		}
		catch (Exception value)
		{
			if (Options.EntityExceptionBehaviour == EntityExceptionBehaviour.Rethrow)
			{
				_log.Error($"Caught exception while serializing component {CurrentComponent} of entity {EntMan.ToPrettyString(uid)}");
				throw;
			}
			_log.Error($"Caught exception while serializing component {CurrentComponent} of entity {EntMan.ToPrettyString(uid)}:\n{value}");
			CurrentEntityYamlUid = 0;
			CurrentEntity = null;
			CurrentComponent = null;
			RemoveErroringEntity(uid);
			return;
		}
		CurrentComponent = null;
		if (sequenceDataNode.Count != 0)
		{
			mappingDataNode.Add("components", sequenceDataNode);
		}
		if (component.EntityPrototype == null)
		{
			CurrentEntityYamlUid = 0;
			CurrentEntity = null;
			return;
		}
		SequenceDataNode sequenceDataNode2 = null;
		foreach (var (value2, componentRegistryEntry2) in component.EntityPrototype.Components)
		{
			if (!EntMan.TryGetComponent(uid, componentRegistryEntry2.Component.GetType(), out IComponent _))
			{
				if (sequenceDataNode2 == null)
				{
					sequenceDataNode2 = new SequenceDataNode();
				}
				sequenceDataNode2.Add(new ValueDataNode(value2));
			}
		}
		if (sequenceDataNode2 != null)
		{
			mappingDataNode.Add("missingComponents", sequenceDataNode2);
		}
		CurrentEntityYamlUid = 0;
		CurrentEntity = null;
	}

	private void RemoveErroringEntity(EntityUid uid)
	{
		if (Options.EntityExceptionBehaviour == EntityExceptionBehaviour.IgnoreEntityAndChildren)
		{
			foreach (EntityUid child in _xformQuery.GetComponent(uid)._children)
			{
				RemoveErroringEntity(child);
			}
		}
		ErroringEntities.Add(uid);
		if (YamlUidMap.TryGetValue(uid, out var value))
		{
			Nullspace.Remove(value);
			Orphans.Remove(value);
			Maps.Remove(value);
			Grids.Remove(value);
			EntityData.Remove(value);
			if (_metaQuery.TryGetComponent(uid, out MetaDataComponent component) && component.EntityPrototype != null && Prototypes.TryGetValue(component.EntityPrototype.ID, out List<int> value2))
			{
				value2.Remove(value);
			}
		}
	}

	private void SerializeComponents(EntityUid uid, Dictionary<string, MappingDataNode>? cache, SequenceDataNode components)
	{
		foreach (IComponent item in EntMan.GetComponentsInternal(uid))
		{
			Type type = item.GetType();
			ComponentRegistration registration = _factory.GetRegistration(type);
			if (registration.Unsaved)
			{
				continue;
			}
			CurrentComponent = registration.Name;
			MappingDataNode value = null;
			MappingDataNode mappingDataNode;
			if (cache != null && cache.TryGetValue(registration.Name, out value))
			{
				mappingDataNode = _serialization.WriteValueAs<MappingDataNode>(type, item, alwaysWrite: true, this);
				mappingDataNode = mappingDataNode.Except(value);
				if (mappingDataNode == null)
				{
					continue;
				}
			}
			else
			{
				mappingDataNode = _serialization.WriteValueAs<MappingDataNode>(type, item, alwaysWrite: false, this);
			}
			if (mappingDataNode.Children.Count != 0 || value == null)
			{
				mappingDataNode.InsertAt(0, "type", new ValueDataNode(registration.Name));
				components.Add(mappingDataNode);
			}
		}
	}

	private Dictionary<string, MappingDataNode>? GetProtoCache(EntityPrototype? proto)
	{
		if (proto == null)
		{
			return null;
		}
		if (PrototypeCache.TryGetValue(proto.ID, out Dictionary<string, MappingDataNode> value))
		{
			return value;
		}
		value = (PrototypeCache[proto.ID] = new Dictionary<string, MappingDataNode>(proto.Components.Count));
		WritingReadingPrototypes = true;
		foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> component in proto.Components)
		{
			component.Deconstruct(out var key, out var value2);
			string text = key;
			EntityPrototype.ComponentRegistryEntry componentRegistryEntry = value2;
			CurrentComponent = text;
			value.Add(text, _serialization.WriteValueAs<MappingDataNode>(componentRegistryEntry.Component.GetType(), componentRegistryEntry.Component, alwaysWrite: true, this));
		}
		CurrentComponent = null;
		WritingReadingPrototypes = false;
		value.TryAdd(_metaName, _emptyMetaNode);
		value.TryAdd(_xformName, _emptyXformNode);
		return value;
	}

	public MappingDataNode Write()
	{
		return new MappingDataNode
		{
			{
				"meta",
				WriteMetadata()
			},
			{
				"maps",
				WriteIds(Maps)
			},
			{
				"grids",
				WriteIds(Grids)
			},
			{
				"orphans",
				WriteIds(Orphans)
			},
			{
				"nullspace",
				WriteIds(Nullspace)
			},
			{
				"tilemap",
				WriteTileMap()
			},
			{
				"entities",
				WriteEntitySection()
			}
		};
	}

	public MappingDataNode WriteMetadata()
	{
		return new MappingDataNode
		{
			{
				"format",
				7.ToString(CultureInfo.InvariantCulture)
			},
			{
				"category",
				GetCategory().ToString()
			},
			{
				"engineVersion",
				_conf.GetCVar(CVars.BuildEngineVersion)
			},
			{
				"forkId",
				_conf.GetCVar(CVars.BuildForkId)
			},
			{
				"forkVersion",
				_conf.GetCVar(CVars.BuildVersion)
			},
			{
				"time",
				DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
			},
			{
				"entityCount",
				EntityData.Count.ToString(CultureInfo.InvariantCulture)
			}
		};
	}

	public SequenceDataNode WriteIds(List<int> ids)
	{
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		foreach (int id in ids)
		{
			sequenceDataNode.Add(new ValueDataNode(id.ToString(CultureInfo.InvariantCulture)));
		}
		return sequenceDataNode;
	}

	public MappingDataNode WriteTileMap()
	{
		MappingDataNode mappingDataNode = new MappingDataNode();
		foreach (var (num3, num4) in _tileMap.OrderBy((KeyValuePair<int, int> x) => x.Key))
		{
			if (!_tileDef.TryGetDefinition(num3, out ITileDefinition definition))
			{
				throw new Exception($"Attempting to serialize a tile {num3} with no valid tile definition.");
			}
			string key = num4.ToString(CultureInfo.InvariantCulture);
			mappingDataNode.Add(key, definition.ID);
		}
		return mappingDataNode;
	}

	public SequenceDataNode WriteEntitySection()
	{
		if (Options.EntityExceptionBehaviour != EntityExceptionBehaviour.IgnoreEntity && Options.EntityExceptionBehaviour != EntityExceptionBehaviour.IgnoreEntityAndChildren && (YamlIds.Count != YamlUidMap.Count || YamlIds.Count != EntityData.Count))
		{
			throw new Exception("Entity count mismatch");
		}
		SequenceDataNode sequenceDataNode = new SequenceDataNode();
		List<string> list = Prototypes.Keys.ToList();
		list.Sort(StringComparer.InvariantCulture);
		foreach (string item2 in list)
		{
			SequenceDataNode sequenceDataNode2 = new SequenceDataNode();
			MappingDataNode node = new MappingDataNode
			{
				{ "proto", item2 },
				{ "entities", sequenceDataNode2 }
			};
			sequenceDataNode.Add(node);
			List<int> list2 = Prototypes[item2];
			list2.Sort();
			foreach (int item3 in list2)
			{
				MappingDataNode item = EntityData[item3].Node;
				sequenceDataNode2.Add(item);
			}
		}
		return sequenceDataNode;
	}

	public FileCategory GetCategory()
	{
		switch (Options.Category)
		{
		case FileCategory.Save:
			return FileCategory.Save;
		case FileCategory.Map:
			if (Maps.Count != 1)
			{
				return FileCategory.Unknown;
			}
			return FileCategory.Map;
		case FileCategory.Grid:
			if (Maps.Count > 0 || Grids.Count != 1)
			{
				return FileCategory.Unknown;
			}
			return FileCategory.Grid;
		case FileCategory.Entity:
			if (Maps.Count > 0 || Grids.Count > 0 || Orphans.Count != 1)
			{
				return FileCategory.Unknown;
			}
			return FileCategory.Entity;
		default:
			if (Maps.Count == 1)
			{
				if (Orphans.Count == 0)
				{
					return FileCategory.Map;
				}
			}
			else if (Grids.Count == 1)
			{
				if (Orphans.Count == 1 && Grids[0] == Orphans[0])
				{
					return FileCategory.Grid;
				}
			}
			else if (Orphans.Count == 1)
			{
				return FileCategory.Entity;
			}
			return FileCategory.Unknown;
		}
	}

	public int GetYamlUid(EntityUid uid)
	{
		if (YamlUidMap.TryGetValue(uid, out var value))
		{
			return value;
		}
		return AllocateYamlUid(uid);
	}

	private int AllocateYamlUid(EntityUid uid)
	{
		if (Truncated.Contains(uid))
		{
			_log.Error("Including a previously truncated entity within the serialization process? Something probably wrong");
		}
		while (!YamlIds.Add(_nextYamlUid))
		{
			_nextYamlUid++;
		}
		YamlUidMap.Add(uid, _nextYamlUid);
		return _nextYamlUid++;
	}

	public int GetYamlTileId(int tileId)
	{
		if (_tileMap.TryGetValue(tileId, out var value))
		{
			return value;
		}
		return AllocateYamlTileId(tileId);
	}

	private int AllocateYamlTileId(int tileId)
	{
		while (!_yamlTileIds.Add(_nextYamlTileId))
		{
			_nextYamlTileId++;
		}
		_tileMap[tileId] = _nextYamlTileId;
		return _nextYamlTileId++;
	}

	public void ReserveYamlIds(HashSet<EntityUid> entities)
	{
		List<EntityUid> list = new List<EntityUid>();
		foreach (EntityUid entity in entities)
		{
			if (YamlUidMap.ContainsKey(entity))
			{
				continue;
			}
			if (_yamlQuery.TryGetComponent(entity, out YamlUidComponent component) && component.Uid > 0 && YamlIds.Add(component.Uid))
			{
				if (Truncated.Contains(entity))
				{
					_log.Error("Including a previously truncated entity within the serialization process? Something probably wrong");
				}
				YamlUidMap.Add(entity, component.Uid);
			}
			else
			{
				list.Add(entity);
			}
		}
		foreach (EntityUid item in list)
		{
			AllocateYamlUid(item);
		}
	}

	public void ReserveYamlId(EntityUid uid)
	{
		if (YamlUidMap.ContainsKey(uid))
		{
			return;
		}
		if (_yamlQuery.TryGetComponent(uid, out YamlUidComponent component) && component.Uid > 0 && YamlIds.Add(component.Uid))
		{
			if (Truncated.Contains(uid))
			{
				_log.Error("Including a previously truncated entity within the serialization process? Something probably wrong");
			}
			YamlUidMap.Add(uid, component.Uid);
		}
		else
		{
			AllocateYamlUid(uid);
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

	public DataNode Write(ISerializationManager serializationManager, EntityUid value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		if (YamlUidMap.TryGetValue(value, out var value2))
		{
			return new ValueDataNode(value2.ToString(CultureInfo.InvariantCulture));
		}
		if (CurrentComponent == _xformName)
		{
			if (value == EntityUid.Invalid)
			{
				return InvalidNode;
			}
			Orphans.Add(CurrentEntityYamlUid);
			if (Options.ErrorOnOrphan && CurrentEntity.HasValue && value != Truncate && !ErroringEntities.Contains(value))
			{
				_log.Error($"Serializing entity {EntMan.ToPrettyString(CurrentEntity)} without including its parent {EntMan.ToPrettyString(value)}");
			}
			return InvalidNode;
		}
		if (ErroringEntities.Contains(value))
		{
			return InvalidNode;
		}
		if (value == EntityUid.Invalid)
		{
			if (Options.MissingEntityBehaviour != MissingEntityBehaviour.Ignore)
			{
				_log.Error("Encountered an invalid entityUid reference.");
			}
			return InvalidNode;
		}
		if (value == Truncate)
		{
			_log.Error($"{EntMan.ToPrettyString(CurrentEntity)}:{CurrentComponent} is attempting to serialize references to a truncated entity {EntMan.ToPrettyString(Truncate)}.");
		}
		switch (Options.MissingEntityBehaviour)
		{
		case MissingEntityBehaviour.Error:
			_log.Error(EntMan.Deleted(value) ? $"Encountered a reference to a deleted entity {value} while serializing {EntMan.ToPrettyString(CurrentEntity)}." : $"Encountered a reference to a missing entity: {value} while serializing {EntMan.ToPrettyString(CurrentEntity)}.");
			return InvalidNode;
		case MissingEntityBehaviour.Ignore:
			return InvalidNode;
		case MissingEntityBehaviour.IncludeNullspace:
		{
			if (!EntMan.TryGetComponent<TransformComponent>(value, out TransformComponent component) || component.ParentUid != EntityUid.Invalid || _gridQuery.HasComp(value) || _mapQuery.HasComp(value))
			{
				goto case MissingEntityBehaviour.Error;
			}
			goto case MissingEntityBehaviour.PartialInclude;
		}
		case MissingEntityBehaviour.PartialInclude:
		case MissingEntityBehaviour.AutoInclude:
		{
			LogLevel? logAutoInclude = Options.LogAutoInclude;
			if (logAutoInclude.HasValue)
			{
				LogLevel valueOrDefault = logAutoInclude.GetValueOrDefault();
				_log.Log(valueOrDefault, $"Auto-including entity {EntMan.ToPrettyString(value)} referenced by {EntMan.ToPrettyString(CurrentEntity)}");
			}
			_autoInclude.Add(value);
			return new ValueDataNode(GetYamlUid(value).ToString(CultureInfo.InvariantCulture));
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	EntityUid ITypeReader<EntityUid, ValueDataNode>.Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context, ISerializationManager.InstantiationDelegate<EntityUid>? _)
	{
		if (!(node.Value == "invalid"))
		{
			return EntityUid.Parse(node.Value.AsSpan());
		}
		return EntityUid.Invalid;
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
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

	public NetEntity Read(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<NetEntity>? instanceProvider = null)
	{
		if (!(node.Value == "invalid"))
		{
			return NetEntity.Parse(node.Value.AsSpan());
		}
		return NetEntity.Invalid;
	}

	public DataNode Write(ISerializationManager serializationManager, NetEntity value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		EntityUid entity = EntMan.GetEntity(value);
		return serializationManager.WriteValue(entity, alwaysWrite, context);
	}

	ValidationNode ITypeValidator<MapId, ValueDataNode>.Validate(ISerializationManager seri, ValueDataNode node, IDependencyCollection deps, ISerializationContext? context)
	{
		return seri.ValidateNode<EntityUid>(node, context);
	}

	MapId ITypeReader<MapId, ValueDataNode>.Read(ISerializationManager seri, ValueDataNode node, IDependencyCollection deps, SerializationHookContext hookCtx, ISerializationContext? ctx, ISerializationManager.InstantiationDelegate<MapId>? instanceProvider)
	{
		if (!EntMan.TryGetComponent<MapComponent>(seri.Read<EntityUid>(node, ctx), out MapComponent component))
		{
			return MapId.Nullspace;
		}
		return component.MapId;
	}

	DataNode ITypeWriter<MapId>.Write(ISerializationManager seri, MapId value, IDependencyCollection deps, bool alwaysWrite, ISerializationContext? ctx)
	{
		if (_map.TryGetMap(value, out var uid))
		{
			return seri.WriteValue(uid, alwaysWrite, ctx);
		}
		_log.Error($"Attempted to serialize invalid map id {value} while serializing component '{CurrentComponent}' on entity '{EntMan.ToPrettyString(uid)}'");
		return new ValueDataNode("invalid");
	}
}
