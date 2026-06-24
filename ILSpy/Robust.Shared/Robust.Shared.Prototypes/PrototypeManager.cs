using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;
using Robust.Shared.Asynchronous;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.IoC.Exceptions;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Random;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Robust.Shared.Prototypes;

public abstract class PrototypeManager : IPrototypeManagerInternal, IPrototypeManager
{
	private sealed class InheritancePushDatum
	{
		public MappingDataNode Result;

		public int CountParentsRemaining;

		public InheritancePushDatum(MappingDataNode result, int countParentsRemaining)
		{
			Result = result;
			CountParentsRemaining = countParentsRemaining;
		}
	}

	private sealed class KindData(Type kind, string name)
	{
		public Dictionary<string, IPrototype>? UnfrozenInstances;

		public FrozenDictionary<string, IPrototype> Instances = FrozenDictionary<string, IPrototype>.Empty;

		public Dictionary<string, MappingDataNode> Results = new Dictionary<string, MappingDataNode>();

		public readonly Dictionary<string, MappingDataNode> RawResults = new Dictionary<string, MappingDataNode>();

		public readonly Type Type = kind;

		public readonly string Name = name;

		public MultiRootInheritanceGraph<string>? Inheritance;

		public object InstancesDirect;

		private MethodInfo _freezeDirectInfo = typeof(KindData).GetMethod("FreezeDirect", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(kind);

		private void FreezeDirect<T>()
		{
			Dictionary<string, T> dictionary = new Dictionary<string, T>();
			foreach (var (key, prototype2) in Instances)
			{
				dictionary.Add(key, (T)prototype2);
			}
			InstancesDirect = dictionary.ToFrozenDictionary();
		}

		public void Freeze()
		{
			Instances = UnfrozenInstances?.ToFrozenDictionary() ?? FrozenDictionary<string, IPrototype>.Empty;
			UnfrozenInstances = null;
			_freezeDirectInfo.Invoke(this, null);
		}
	}

	private sealed record ExtractedMappingData(Type Kind, string Id, string[]? Parents, MappingDataNode Data);

	private sealed class PrototypeValidationData
	{
		public readonly string Id;

		public MappingDataNode Mapping;

		public readonly string File;

		public bool Pushed;

		public string[]? Parents;

		public MappingDataNode[]? ParentMappings;

		public PrototypeValidationData(string id, MappingDataNode mapping, string file)
		{
			Id = id;
			File = file;
			Mapping = mapping;
		}
	}

	private (string, EntityCategoryAttribute)[]? _autoComps;

	[Dependency]
	private readonly IReflectionManager _reflectionManager;

	[Dependency]
	protected readonly IResourceManager Resources;

	[Dependency]
	protected readonly ITaskManager TaskManager;

	[Dependency]
	private readonly ISerializationManager _serializationManager;

	[Dependency]
	private readonly ILogManager _logManager;

	[Dependency]
	private readonly ILocalizationManager _locMan;

	[Dependency]
	private readonly IComponentFactory _factory;

	[Dependency]
	private readonly IEntityManager _entMan;

	[Dependency]
	private readonly IRobustRandom _random;

	private readonly Dictionary<string, Dictionary<string, MappingDataNode>> _prototypeDataCache = new Dictionary<string, Dictionary<string, MappingDataNode>>();

	private EntityDiffContext _context = new EntityDiffContext();

	private readonly Dictionary<string, Type> _kindNames = new Dictionary<string, Type>();

	private readonly Dictionary<Type, int> _kindPriorities = new Dictionary<Type, int>();

	protected ISawmill Sawmill;

	private bool _initialized;

	private bool _hasEverBeenReloaded;

	private FrozenDictionary<Type, KindData> _kinds = FrozenDictionary<Type, KindData>.Empty;

	private readonly HashSet<string> _ignoredPrototypeTypes = new HashSet<string>();

	private readonly List<ResPath> _abstractFiles = new List<ResPath>();

	private readonly List<ResPath> _abstractDirectories = new List<ResPath>();

	private static readonly char[] DisallowedIdChars = new char[2] { ' ', '.' };

	public FrozenDictionary<ProtoId<EntityCategoryPrototype>, IReadOnlyList<EntityPrototype>> Categories { get; private set; } = FrozenDictionary<ProtoId<EntityCategoryPrototype>, IReadOnlyList<EntityPrototype>>.Empty;

	public event Action<PrototypesReloadedEventArgs>? PrototypesReloaded;

	public event Action<DataNodeDocument>? LoadedData;

	private void UpdateCategories()
	{
		Dictionary<string, HashSet<EntityCategoryPrototype>> automaticCategories = GetAutomaticCategories();
		Dictionary<EntProtoId, IReadOnlySet<EntityCategoryPrototype>> cache = new Dictionary<EntProtoId, IReadOnlySet<EntityCategoryPrototype>>(Count<EntityPrototype>());
		Dictionary<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>> dictionary = new Dictionary<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>(Count<EntityCategoryPrototype>());
		foreach (EntityPrototype item in EnumeratePrototypes<EntityPrototype>())
		{
			UpdateCategories(item, cache, automaticCategories, dictionary);
		}
		foreach (EntityCategoryPrototype item2 in EnumeratePrototypes<EntityCategoryPrototype>())
		{
			dictionary.GetOrNew(item2.ID);
		}
		Categories = ((IEnumerable<KeyValuePair<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>>)dictionary).ToFrozenDictionary((Func<KeyValuePair<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>, ProtoId<EntityCategoryPrototype>>)((KeyValuePair<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>> x) => x.Key), (Func<KeyValuePair<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>, IReadOnlyList<EntityPrototype>>)((KeyValuePair<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>> x) => x.Value), (IEqualityComparer<ProtoId<EntityCategoryPrototype>>?)null);
	}

	private Dictionary<string, HashSet<EntityCategoryPrototype>> GetAutomaticCategories()
	{
		Dictionary<string, HashSet<EntityCategoryPrototype>> dictionary = new Dictionary<string, HashSet<EntityCategoryPrototype>>();
		foreach (EntityCategoryPrototype item2 in EnumeratePrototypes<EntityCategoryPrototype>())
		{
			if (item2.Components == null)
			{
				continue;
			}
			foreach (string component in item2.Components)
			{
				dictionary.GetOrNew(component).Add(item2);
			}
		}
		if (_autoComps == null)
		{
			_autoComps = (from x in _factory.GetAllRegistrations()
				where x.Type.HasCustomAttribute<EntityCategoryAttribute>()
				select (Name: x.Name, x.Type.GetCustomAttribute<EntityCategoryAttribute>())).ToArray();
		}
		(string, EntityCategoryAttribute)[] autoComps = _autoComps;
		for (int num = 0; num < autoComps.Length; num++)
		{
			(string, EntityCategoryAttribute) tuple = autoComps[num];
			string item = tuple.Item1;
			string[] categories = tuple.Item2.Categories;
			foreach (string text in categories)
			{
				if (TryIndex(text, out EntityCategoryPrototype prototype))
				{
					dictionary.GetOrNew(item).Add(prototype);
					continue;
				}
				Sawmill.Error($"Component {item} has invalid {"EntityCategoryAttribute"} argument: {text}");
			}
		}
		return dictionary;
	}

	private IReadOnlySet<EntityCategoryPrototype> UpdateCategories(EntProtoId id, Dictionary<EntProtoId, IReadOnlySet<EntityCategoryPrototype>> cache, Dictionary<string, HashSet<EntityCategoryPrototype>> autoCategories, Dictionary<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>> categories)
	{
		if (cache.TryGetValue(id, out IReadOnlySet<EntityCategoryPrototype> value))
		{
			return value;
		}
		HashSet<EntityCategoryPrototype> hashSet = new HashSet<EntityCategoryPrototype>();
		if (!TryGetMapping(typeof(EntityPrototype), id, out MappingDataNode mappings))
		{
			throw new UnknownPrototypeException(id, typeof(EntityPrototype));
		}
		if (mappings.TryGet("categories", out SequenceDataNode node))
		{
			foreach (ValueDataNode item2 in node)
			{
				string value2 = item2.Value;
				if (TryIndex(value2, out EntityCategoryPrototype prototype))
				{
					hashSet.Add(prototype);
					continue;
				}
				Sawmill.Error($"Entity prototype {id} specifies an invalid {"EntityCategoryPrototype"}: {value2}");
			}
		}
		foreach (var item3 in EnumerateAllParents<EntityPrototype>(id))
		{
			string item = item3.id;
			foreach (EntityCategoryPrototype item4 in UpdateCategories(item, cache, autoCategories, categories))
			{
				if (item4.Inheritable)
				{
					hashSet.Add(item4);
				}
			}
		}
		if (!TryIndex(id, out EntityPrototype prototype2))
		{
			cache.Add(id, hashSet);
			return hashSet;
		}
		foreach (string key in prototype2.Components.Keys)
		{
			if (autoCategories.TryGetValue(key, out HashSet<EntityCategoryPrototype> value3))
			{
				hashSet.UnionWith(value3);
			}
		}
		cache.Add(id, hashSet);
		prototype2.Categories = hashSet;
		foreach (EntityCategoryPrototype item5 in hashSet)
		{
			if (item5.HideSpawnMenu)
			{
				prototype2.HideSpawnMenu = true;
			}
			categories.GetOrNew(item5).Add(prototype2);
		}
		return hashSet;
	}

	public virtual void Initialize()
	{
		if (!_initialized)
		{
			Sawmill = _logManager.GetSawmill("proto");
			_initialized = true;
			ReloadPrototypeKinds();
			PrototypesReloaded += OnReload;
		}
	}

	public IEnumerable<string> GetPrototypeKinds()
	{
		return _kindNames.Keys;
	}

	public int Count<T>() where T : class, IPrototype
	{
		return _kinds[typeof(T)].Instances.Count;
	}

	public IEnumerable<T> EnumeratePrototypes<T>() where T : class, IPrototype
	{
		return GetInstances<T>().Values;
	}

	public IEnumerable<IPrototype> EnumeratePrototypes(Type kind)
	{
		if (!_hasEverBeenReloaded)
		{
			throw new InvalidOperationException("No prototypes have been loaded yet.");
		}
		return _kinds[kind].Instances.Values;
	}

	public IEnumerable<IPrototype> EnumeratePrototypes(string variant)
	{
		return EnumeratePrototypes(GetKindType(variant));
	}

	public IEnumerable<T> EnumerateParents<T>(T proto, bool includeSelf = false) where T : class, IPrototype, IInheritingPrototype
	{
		return EnumerateParents<T>(proto.ID, includeSelf);
	}

	public IEnumerable<T> EnumerateParents<T>(string id, bool includeSelf = false) where T : class, IPrototype, IInheritingPrototype
	{
		if (!_hasEverBeenReloaded)
		{
			throw new InvalidOperationException("No prototypes have been loaded yet.");
		}
		if (!TryIndex(id, out T prototype))
		{
			yield break;
		}
		if (includeSelf)
		{
			yield return prototype;
		}
		if (prototype.Parents == null)
		{
			yield break;
		}
		Queue<string> queue = new Queue<string>(prototype.Parents);
		string result;
		while (queue.TryDequeue(out result))
		{
			if (!TryIndex(result, out T parent))
			{
				continue;
			}
			yield return parent;
			if (parent.Parents != null)
			{
				string[] parents = parent.Parents;
				foreach (string item in parents)
				{
					queue.Enqueue(item);
				}
				parent = null;
			}
		}
	}

	public IEnumerable<IPrototype> EnumerateParents(Type kind, string id, bool includeSelf = false)
	{
		if (!_hasEverBeenReloaded)
		{
			throw new InvalidOperationException("No prototypes have been loaded yet.");
		}
		if (!kind.IsAssignableTo(typeof(IInheritingPrototype)))
		{
			throw new InvalidOperationException("The provided prototype type is not an inheriting prototype");
		}
		if (!TryIndex(kind, id, out IPrototype prototype))
		{
			yield break;
		}
		if (includeSelf)
		{
			yield return prototype;
		}
		IInheritingPrototype inheritingPrototype = (IInheritingPrototype)prototype;
		if (inheritingPrototype.Parents == null)
		{
			yield break;
		}
		Queue<string> queue = new Queue<string>(inheritingPrototype.Parents);
		string result;
		while (queue.TryDequeue(out result))
		{
			if (!TryIndex(kind, result, out IPrototype parent))
			{
				continue;
			}
			yield return parent;
			inheritingPrototype = (IInheritingPrototype)parent;
			if (inheritingPrototype.Parents != null)
			{
				string[] parents = inheritingPrototype.Parents;
				foreach (string item in parents)
				{
					queue.Enqueue(item);
				}
				parent = null;
			}
		}
	}

	public IEnumerable<(string id, T?)> EnumerateAllParents<T>(string id, bool includeSelf = false) where T : class, IPrototype, IInheritingPrototype
	{
		if (!_hasEverBeenReloaded)
		{
			throw new InvalidOperationException("No prototypes have been loaded yet.");
		}
		if (!_kinds.TryGetValue(typeof(T), out KindData kindData))
		{
			throw new UnknownPrototypeException(id, typeof(T));
		}
		if (!kindData.Results.ContainsKey(id))
		{
			yield break;
		}
		IPrototype value;
		if (includeSelf)
		{
			kindData.Instances.TryGetValue(id, out value);
			T item = value as T;
			yield return (id: id, item);
		}
		if (!kindData.Inheritance.TryGetParents(id, out string[] parents))
		{
			yield break;
		}
		Queue<string> queue = new Queue<string>(parents);
		string prototypeId;
		while (queue.TryDequeue(out prototypeId))
		{
			if (!kindData.Results.ContainsKey(prototypeId))
			{
				Sawmill.Error($"Encountered invalid prototype while enumerating parents. Kind: {typeof(T).Name}. Child: {id}. Invalid: {prototypeId}");
				continue;
			}
			kindData.Instances.TryGetValue(prototypeId, out value);
			T item = value as T;
			yield return (id: prototypeId, item);
			if (kindData.Inheritance.TryGetParents(prototypeId, out parents))
			{
				string[] array = parents;
				foreach (string item2 in array)
				{
					queue.Enqueue(item2);
				}
			}
		}
	}

	public IEnumerable<Type> EnumeratePrototypeKinds()
	{
		if (!_hasEverBeenReloaded)
		{
			throw new InvalidOperationException("No prototypes have been loaded yet.");
		}
		return _kinds.Keys;
	}

	public T Index<T>(string id) where T : class, IPrototype
	{
		if (!_hasEverBeenReloaded)
		{
			throw new InvalidOperationException("No prototypes have been loaded yet.");
		}
		try
		{
			return (T)_kinds[typeof(T)].Instances[id];
		}
		catch (KeyNotFoundException)
		{
			throw new UnknownPrototypeException(id, typeof(T));
		}
	}

	public EntityPrototype Index(EntProtoId id)
	{
		return Index<EntityPrototype>(id.Id);
	}

	public T Index<T>(ProtoId<T> id) where T : class, IPrototype
	{
		return Index<T>(id.Id);
	}

	public IPrototype Index(Type kind, string id)
	{
		if (!_hasEverBeenReloaded)
		{
			throw new InvalidOperationException("No prototypes have been loaded yet.");
		}
		try
		{
			return _kinds[kind].Instances[id];
		}
		catch (KeyNotFoundException)
		{
			throw new UnknownPrototypeException(id, kind);
		}
	}

	public void Clear()
	{
		_kindNames.Clear();
		_kinds = FrozenDictionary<Type, KindData>.Empty;
	}

	public void Reset()
	{
		Dictionary<Type, HashSet<string>> dictionary = _kinds.ToDictionary<KeyValuePair<Type, KindData>, Type, HashSet<string>>((KeyValuePair<Type, KindData> x) => x.Key, (KeyValuePair<Type, KindData> x) => x.Value.Instances.Keys.ToHashSet());
		ReloadPrototypeKinds();
		Dictionary<Type, HashSet<string>> dictionary2 = new Dictionary<Type, HashSet<string>>();
		LoadDefaultPrototypes(dictionary2);
		foreach (var (key, other) in dictionary2)
		{
			if (dictionary.TryGetValue(key, out var value))
			{
				value.ExceptWith(other);
				if (value.Count == 0)
				{
					dictionary.Remove(key);
				}
			}
		}
		ReloadPrototypes(dictionary2, dictionary);
		_locMan.ReloadLocalizations();
	}

	public abstract void LoadDefaultPrototypes(Dictionary<Type, HashSet<string>>? changed = null);

	private int SortPrototypesByPriority(Type a, Type b)
	{
		return _kindPriorities[b].CompareTo(_kindPriorities[a]);
	}

	protected void ReloadPrototypes(IEnumerable<ResPath> filePaths)
	{
	}

	public void ReloadPrototypes(Dictionary<Type, HashSet<string>> modified, Dictionary<Type, HashSet<string>>? removed = null)
	{
		List<Type> list = modified.Keys.ToList();
		list.Sort(SortPrototypesByPriority);
		Dictionary<Type, PrototypesReloadedEventArgs.PrototypeChangeSet> dictionary = new Dictionary<Type, PrototypesReloadedEventArgs.PrototypeChangeSet>();
		HashSet<KindData> hashSet = new HashSet<KindData>();
		HashSet<string> toProcess = new HashSet<string>();
		Queue<string> processQueue = new Queue<string>();
		foreach (Type item2 in list)
		{
			Dictionary<string, IPrototype> dictionary2 = new Dictionary<string, IPrototype>();
			KindData kindData = _kinds[item2];
			MultiRootInheritanceGraph<string> tree = kindData.Inheritance;
			toProcess.Clear();
			processQueue.Clear();
			foreach (string item3 in modified[item2])
			{
				AddToQueue(item3);
			}
			string result;
			while (processQueue.TryDequeue(out result))
			{
				if (tree != null)
				{
					if (tree.TryGetParents(result, out string[] parents))
					{
						bool flag = false;
						string[] array = parents;
						foreach (string item in array)
						{
							if (toProcess.Contains(item))
							{
								processQueue.Enqueue(result);
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}
						if (parents.Length == 1)
						{
							kindData.Results[result] = _serializationManager.PushCompositionWithGenericNode(item2, kindData.Results[parents[0]], kindData.RawResults[result]);
						}
						else
						{
							MappingDataNode[] array2 = new MappingDataNode[parents.Length];
							for (int j = 0; j < array2.Length; j++)
							{
								array2[j] = kindData.Results[parents[j]];
							}
							kindData.Results[result] = _serializationManager.PushCompositionWithGenericNode(item2, array2, kindData.RawResults[result]);
						}
					}
					else
					{
						kindData.Results[result] = kindData.RawResults[result];
					}
				}
				toProcess.Remove(result);
				IPrototype prototype = TryReadPrototype(item2, result, kindData.Results[result], SerializationHookContext.DontSkipHooks);
				if (prototype != null)
				{
					KindData kindData2 = kindData;
					if (kindData2.UnfrozenInstances == null)
					{
						kindData2.UnfrozenInstances = kindData.Instances.ToDictionary();
					}
					kindData.UnfrozenInstances[result] = prototype;
					dictionary2.Add(result, prototype);
				}
			}
			if (dictionary2.Count != 0)
			{
				dictionary.Add(kindData.Type, new PrototypesReloadedEventArgs.PrototypeChangeSet(dictionary2));
				hashSet.Add(kindData);
			}
			void AddToQueue(string id)
			{
				if (toProcess.Add(id))
				{
					processQueue.Enqueue(id);
					if (tree != null && tree.TryGetChildren(id, out IReadOnlySet<string> set))
					{
						foreach (string item4 in set)
						{
							AddToQueue(item4);
						}
					}
				}
			}
		}
		Freeze(hashSet);
		if (hashSet.Any((KindData x) => x.Type == typeof(EntityPrototype) || x.Type == typeof(EntityCategoryPrototype)))
		{
			UpdateCategories();
		}
		HashSet<Type> hashSet2 = new HashSet<Type>(dictionary.Keys);
		if (removed != null)
		{
			hashSet2.UnionWith(removed.Keys);
		}
		PrototypesReloadedEventArgs e = new PrototypesReloadedEventArgs(hashSet2, dictionary, removed);
		this.PrototypesReloaded?.Invoke(e);
		_entMan.EventBus.RaiseEvent(EventSource.Local, e);
	}

	private void Freeze(IEnumerable<KindData> kinds)
	{
		RStopwatch rStopwatch = RStopwatch.StartNew();
		foreach (KindData kind in kinds)
		{
			kind.Freeze();
		}
		Sawmill?.Verbose($"Freezing prototype instances took {rStopwatch.Elapsed.TotalMilliseconds:f2}ms");
	}

	public void ResolveResults()
	{
		Dictionary<Type, Task> dictionary = new Dictionary<Type, Task>();
		foreach (KeyValuePair<Type, KindData> kind in _kinds)
		{
			var (k, v) = (KeyValuePair<Type, KindData>)(ref kind);
			if (v.Inheritance != null)
			{
				Task value = Task.Run(() => PushKindInheritance(k, v));
				dictionary.Add(k, value);
			}
		}
		foreach (IGrouping<int, Type> item in from key in _kinds.Keys
			group key by _kindPriorities[key] into grouping
			orderby grouping.Key descending
			select grouping)
		{
			KindData[] kinds = item.Select((Type key) => _kinds[key]).ToArray();
			InstantiateKinds(kinds, dictionary);
		}
		UpdateCategories();
	}

	private void InstantiateKinds(KindData[] kinds, Dictionary<Type, Task> inheritanceTasks)
	{
		KindData[] array = kinds;
		foreach (KindData kindData in array)
		{
			if (inheritanceTasks.TryGetValue(kindData.Type, out Task value))
			{
				value.Wait();
			}
		}
		(KindData KindData, string Id, MappingDataNode Mapping, IPrototype Instance)[] results = (from data in kinds
			from keyValuePair in data.Results
			select ((KindData KindData, string Id, MappingDataNode Mapping, IPrototype Instance))(KindData: data, Id: keyValuePair.Key, Mapping: keyValuePair.Value, Instance: null)).ToArray();
		_random.Shuffle(results.AsSpan());
		UnboundedChannelOptions options = new UnboundedChannelOptions
		{
			SingleReader = true,
			SingleWriter = false,
			AllowSynchronousContinuations = true
		};
		Channel<ISerializationHooks> hooksChannel = Channel.CreateUnbounded<ISerializationHooks>(options);
		Task task = Task.Run(delegate
		{
			InstantiatePrototypes(kinds, results, hooksChannel);
		});
		ChannelReader<ISerializationHooks> reader = hooksChannel.Reader;
		while (reader.WaitToReadAsync().AsTask().Result)
		{
			ISerializationHooks item;
			while (reader.TryRead(out item))
			{
				item.AfterDeserialization();
			}
		}
		task.Wait();
	}

	private void InstantiatePrototypes(KindData[] kinds, (KindData KindData, string Id, MappingDataNode Mapping, IPrototype? Instance)[] results, Channel<ISerializationHooks> hooks)
	{
		SerializationHookContext hookCtx = new SerializationHookContext(hooks.Writer, skipHooks: false);
		try
		{
			Parallel.For(0, results.Length, delegate(int i)
			{
				ref(KindData, string, MappingDataNode, IPrototype) reference = ref results[i];
				reference.Item4 = TryReadPrototype(reference.Item1.Type, reference.Item2, reference.Item3, hookCtx);
			});
			(KindData, string, MappingDataNode, IPrototype)[] array = results;
			for (int num = 0; num < array.Length; num++)
			{
				(KindData, string, MappingDataNode, IPrototype) tuple = array[num];
				if (tuple.Item4 != null)
				{
					var (kindData, _, _, _) = tuple;
					if (kindData.UnfrozenInstances == null)
					{
						kindData.UnfrozenInstances = tuple.Item1.Instances.ToDictionary();
					}
					tuple.Item1.UnfrozenInstances[tuple.Item2] = tuple.Item4;
				}
			}
			Freeze(kinds.Where((KindData data) => data.UnfrozenInstances != null));
		}
		finally
		{
			hooks.Writer.Complete();
		}
	}

	private IPrototype? TryReadPrototype(Type kind, string id, MappingDataNode mapping, SerializationHookContext hookCtx)
	{
		if (mapping.TryGet("abstract", out ValueDataNode node) && node.AsBool())
		{
			return null;
		}
		try
		{
			return (IPrototype)_serializationManager.Read(kind, mapping, hookCtx);
		}
		catch (Exception value)
		{
			Sawmill.Error($"Reading {kind}({id}) threw the following exception: {value}");
			return null;
		}
	}

	private async Task PushKindInheritance(Type kind, KindData data)
	{
		MultiRootInheritanceGraph<string> tree = data.Inheritance;
		if (tree == null)
		{
			return;
		}
		Dictionary<string, InheritancePushDatum> results = data.RawResults.ToDictionary<KeyValuePair<string, MappingDataNode>, string, InheritancePushDatum>((KeyValuePair<string, MappingDataNode> k) => k.Key, (KeyValuePair<string, MappingDataNode> k) => new InheritancePushDatum(k.Value, tree.GetParentsCount(k.Key)));
		CountdownEvent countDown = new CountdownEvent(results.Count);
		try
		{
			foreach (string root in tree.RootNodes)
			{
				ThreadPool.QueueUserWorkItem(delegate
				{
					ProcessItem(root, results[root]);
				});
			}
			await WaitHandleHelpers.WaitOneAsync(countDown.WaitHandle);
			data.Results.Clear();
			foreach (var (key, inheritancePushDatum2) in results)
			{
				data.Results[key] = inheritancePushDatum2.Result;
			}
		}
		finally
		{
			if (countDown != null)
			{
				((IDisposable)countDown).Dispose();
			}
		}
		void ProcessItem(string id, InheritancePushDatum datum)
		{
			try
			{
				if (tree.TryGetParents(id, out string[] parents))
				{
					if (parents.Length == 1)
					{
						datum.Result = _serializationManager.PushCompositionWithGenericNode(kind, results[parents[0]].Result, datum.Result);
					}
					else
					{
						MappingDataNode[] array = new MappingDataNode[parents.Length];
						for (int i = 0; i < parents.Length; i++)
						{
							array[i] = results[parents[i]].Result;
						}
						datum.Result = _serializationManager.PushCompositionWithGenericNode(kind, array, datum.Result);
					}
				}
				if (tree.TryGetChildren(id, out IReadOnlySet<string> set))
				{
					foreach (string child in set)
					{
						InheritancePushDatum childDatum = results[child];
						if (Interlocked.Decrement(ref childDatum.CountParentsRemaining) == 0)
						{
							ThreadPool.QueueUserWorkItem(delegate
							{
								ProcessItem(child, childDatum);
							});
						}
					}
				}
				countDown.Signal();
			}
			catch (Exception value)
			{
				Sawmill.Error($"Failed to push composition for {kind.Name} prototype with id: {id}. Exception: {value}");
				throw;
			}
		}
	}

	public void ReloadPrototypeKinds()
	{
		Clear();
		Dictionary<Type, KindData> dictionary = new Dictionary<Type, KindData>();
		foreach (Type allChild in _reflectionManager.GetAllChildren<IPrototype>())
		{
			RegisterKind(allChild, dictionary);
		}
		Freeze(dictionary);
	}

	public bool HasIndex<T>(string id) where T : class, IPrototype
	{
		if (!_kinds.TryGetValue(typeof(T), out KindData value))
		{
			throw new UnknownPrototypeException(id, typeof(T));
		}
		return value.Instances.ContainsKey(id);
	}

	public bool HasIndex(EntProtoId id)
	{
		return HasIndex<EntityPrototype>(id.Id);
	}

	public bool HasIndex<T>(ProtoId<T> id) where T : class, IPrototype
	{
		return HasIndex<T>(id.Id);
	}

	public bool HasIndex(EntProtoId? id)
	{
		if (!id.HasValue)
		{
			return false;
		}
		return HasIndex(id.Value);
	}

	public bool HasIndex<T>(ProtoId<T>? id) where T : class, IPrototype
	{
		if (!id.HasValue)
		{
			return false;
		}
		return HasIndex(id.Value);
	}

	public bool TryIndex<T>(string id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
	{
		IPrototype prototype2;
		bool result = TryIndex(typeof(T), id, out prototype2);
		prototype = (prototype2 ?? null) as T;
		return result;
	}

	public bool TryIndex(Type kind, string id, [NotNullWhen(true)] out IPrototype? prototype)
	{
		if (!_kinds.TryGetValue(kind, out KindData value))
		{
			throw new UnknownPrototypeException(id, kind);
		}
		return value.Instances.TryGetValue(id, out prototype);
	}

	public bool Resolve(EntProtoId id, [NotNullWhen(true)] out EntityPrototype? prototype)
	{
		if (TryIndex(id.Id, out prototype))
		{
			return true;
		}
		Sawmill.Error($"Attempted to resolve invalid {"EntProtoId"}: {id.Id}\n{Environment.StackTrace}");
		return false;
	}

	[Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
	public bool TryIndex(EntProtoId id, [NotNullWhen(true)] out EntityPrototype? prototype, bool logError = true)
	{
		if (logError)
		{
			return Resolve(id, out prototype);
		}
		return TryIndex(id, out prototype);
	}

	public bool TryIndex([ForbidLiteral] EntProtoId id, [NotNullWhen(true)] out EntityPrototype? prototype)
	{
		return TryIndex(id.Id, out prototype);
	}

	public bool Resolve<T>(ProtoId<T> id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
	{
		if (TryIndex(id.Id, out prototype))
		{
			return true;
		}
		Sawmill.Error($"Attempted to resolve invalid ProtoId<{typeof(T).Name}>: {id.Id}\n{Environment.StackTrace}");
		return false;
	}

	[Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
	public bool TryIndex<T>(ProtoId<T> id, [NotNullWhen(true)] out T? prototype, bool logError = true) where T : class, IPrototype
	{
		if (logError)
		{
			return Resolve(id, out prototype);
		}
		return TryIndex(id, out prototype);
	}

	public bool TryIndex<T>(ProtoId<T> id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
	{
		return TryIndex(id.Id, out prototype);
	}

	public bool Resolve(EntProtoId? id, [NotNullWhen(true)] out EntityPrototype? prototype)
	{
		if (!id.HasValue)
		{
			prototype = null;
			return false;
		}
		return Resolve(id.Value, out prototype);
	}

	[Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
	public bool TryIndex(EntProtoId? id, [NotNullWhen(true)] out EntityPrototype? prototype, bool logError = true)
	{
		if (logError)
		{
			return Resolve(id, out prototype);
		}
		return TryIndex(id, out prototype);
	}

	public bool TryIndex(EntProtoId? id, [NotNullWhen(true)] out EntityPrototype? prototype)
	{
		if (!id.HasValue)
		{
			prototype = null;
			return false;
		}
		return TryIndex(id.Value, out prototype);
	}

	public bool Resolve<T>(ProtoId<T>? id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
	{
		if (!id.HasValue)
		{
			prototype = null;
			return false;
		}
		return Resolve(id.Value, out prototype);
	}

	[Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
	public bool TryIndex<T>(ProtoId<T>? id, [NotNullWhen(true)] out T? prototype, bool logError = true) where T : class, IPrototype
	{
		if (logError)
		{
			return Resolve(id, out prototype);
		}
		return TryIndex(id, out prototype);
	}

	public bool TryIndex<T>(ProtoId<T>? id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
	{
		if (!id.HasValue)
		{
			prototype = null;
			return false;
		}
		return TryIndex(id.Value, out prototype);
	}

	public bool HasMapping<T>(string id)
	{
		if (!_kinds.TryGetValue(typeof(T), out KindData value))
		{
			throw new UnknownPrototypeException(id, typeof(T));
		}
		return value.Results.ContainsKey(id);
	}

	public bool TryGetMapping(Type kind, string id, [NotNullWhen(true)] out MappingDataNode? mappings)
	{
		return _kinds[kind].Results.TryGetValue(id, out mappings);
	}

	public bool HasKind(string kind)
	{
		return _kindNames.ContainsKey(kind);
	}

	public Type GetKindType(string kind)
	{
		return _kindNames[kind];
	}

	public bool TryGetKindType(string kind, [NotNullWhen(true)] out Type? prototype)
	{
		return _kindNames.TryGetValue(kind, out prototype);
	}

	public bool TryGetKindFrom(Type type, [NotNullWhen(true)] out string? kind)
	{
		kind = null;
		if (!_kinds.TryGetValue(type, out KindData value))
		{
			return false;
		}
		kind = value.Name;
		return true;
	}

	public FrozenDictionary<string, T> GetInstances<T>() where T : IPrototype
	{
		if (TryGetInstances(out FrozenDictionary<string, T> instances))
		{
			return instances;
		}
		throw new Exception("Failed to fetch instances for kind T");
	}

	public bool TryGetInstances<T>([NotNullWhen(true)] out FrozenDictionary<string, T>? instances) where T : IPrototype
	{
		if (!TryGetInstances(typeof(T), out object instances2))
		{
			instances = null;
			return false;
		}
		instances = instances2 as FrozenDictionary<string, T>;
		if (instances == null)
		{
			instances = FrozenDictionary<string, T>.Empty;
		}
		return true;
	}

	private bool TryGetInstances(Type kind, [NotNullWhen(true)] out object? instances)
	{
		if (!_hasEverBeenReloaded)
		{
			throw new InvalidOperationException("No prototypes have been loaded yet.");
		}
		if (!_kinds.TryGetValue(kind, out KindData value))
		{
			instances = null;
			return false;
		}
		instances = value.InstancesDirect;
		return true;
	}

	public bool TryGetKindFrom(IPrototype prototype, [NotNullWhen(true)] out string? kind)
	{
		return TryGetKindFrom(prototype.GetType(), out kind);
	}

	public bool TryGetKindFrom<T>([NotNullWhen(true)] out string? kind) where T : class, IPrototype
	{
		return TryGetKindFrom(typeof(T), out kind);
	}

	public bool IsIgnored(string name)
	{
		return _ignoredPrototypeTypes.Contains(name);
	}

	public void RegisterIgnore(string name)
	{
		_ignoredPrototypeTypes.Add(name);
	}

	private static string CalculatePrototypeName(Type type)
	{
		return PrototypeUtility.CalculatePrototypeName(type.Name);
	}

	public void RegisterKind(params Type[] kinds)
	{
		Dictionary<Type, KindData> dictionary = _kinds.ToDictionary();
		foreach (Type kind in kinds)
		{
			RegisterKind(kind, dictionary);
		}
		Freeze(dictionary);
	}

	private void Freeze(Dictionary<Type, KindData> dict)
	{
		RStopwatch rStopwatch = RStopwatch.StartNew();
		_kinds = dict.ToFrozenDictionary();
		Sawmill?.Verbose($"Freezing prototype kinds took {rStopwatch.Elapsed.TotalMilliseconds:f2}ms");
	}

	private void RegisterKind(Type kind, Dictionary<Type, KindData> kinds)
	{
		if (!typeof(IPrototype).IsAssignableFrom(kind))
		{
			throw new InvalidOperationException("Type must implement IPrototype.");
		}
		PrototypeAttribute prototypeAttribute = (PrototypeAttribute)Attribute.GetCustomAttribute(kind, typeof(PrototypeAttribute));
		if (prototypeAttribute == null)
		{
			throw new InvalidImplementationException(kind, typeof(IPrototype), "No PrototypeAttribute to give it a type string.");
		}
		string text = prototypeAttribute.Type ?? CalculatePrototypeName(kind);
		if (_ignoredPrototypeTypes.Contains(text))
		{
			Sawmill.Warning($"Registering an ignored prototype {kind}");
		}
		if (_kindNames.TryGetValue(text, out Type value))
		{
			throw new InvalidImplementationException(kind, typeof(IPrototype), $"Duplicate prototype type ID: {prototypeAttribute.Type}. Current: {value}");
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		foreach (AbstractFieldInfo allPropertiesAndField in kind.GetAllPropertiesAndFields())
		{
			bool num = allPropertiesAndField.HasAttribute<IdDataFieldAttribute>();
			bool flag4 = allPropertiesAndField.HasAttribute<ParentDataFieldAttribute>();
			if (num)
			{
				if (flag)
				{
					throw new InvalidImplementationException(kind, typeof(IPrototype), "Found two IdDataFieldAttribute");
				}
				flag = true;
			}
			if (flag4)
			{
				if (flag2)
				{
					throw new InvalidImplementationException(kind, typeof(IInheritingPrototype), "Found two ParentDataFieldAttribute");
				}
				flag2 = true;
			}
			if (num && flag4)
			{
				throw new InvalidImplementationException(kind, typeof(IPrototype), $"Prototype {kind} has the Id- & ParentDatafield on single member {allPropertiesAndField.Name}");
			}
			if (allPropertiesAndField.HasAttribute<AbstractDataFieldAttribute>())
			{
				if (flag3)
				{
					throw new InvalidImplementationException(kind, typeof(IInheritingPrototype), "Found two AbstractDataFieldAttribute");
				}
				flag3 = true;
			}
		}
		if (!flag)
		{
			throw new InvalidImplementationException(kind, typeof(IPrototype), "Did not find any member annotated with the IdDataFieldAttribute");
		}
		if (kind.IsAssignableTo(typeof(IInheritingPrototype)) && (!flag2 || !flag3))
		{
			throw new InvalidImplementationException(kind, typeof(IInheritingPrototype), "Did not find any member annotated with the ParentDataFieldAttribute and/or AbstractDataFieldAttribute");
		}
		_kindNames[text] = kind;
		_kindPriorities[kind] = prototypeAttribute.LoadPriority;
		KindData kindData = (kinds[kind] = new KindData(kind, text));
		if (kind.IsAssignableTo(typeof(IInheritingPrototype)))
		{
			kindData.Inheritance = new MultiRootInheritanceGraph<string>();
		}
		else
		{
			kindData.Results = kindData.RawResults;
		}
	}

	private void OnReload(PrototypesReloadedEventArgs args)
	{
		if (args.ByType.TryGetValue(typeof(EntityPrototype), out PrototypesReloadedEventArgs.PrototypeChangeSet value))
		{
			foreach (string key in value.Modified.Keys)
			{
				_prototypeDataCache.Remove(key);
			}
		}
		if (args.Removed == null || !args.Removed.TryGetValue(typeof(EntityPrototype), out HashSet<string> value2))
		{
			return;
		}
		foreach (string item in value2)
		{
			_prototypeDataCache.Remove(item);
		}
	}

	public IReadOnlyDictionary<string, MappingDataNode> GetPrototypeData(EntityPrototype prototype)
	{
		if (_prototypeDataCache.TryGetValue(prototype.ID, out Dictionary<string, MappingDataNode> value))
		{
			return value;
		}
		_context.WritingReadingPrototypes = true;
		value = new Dictionary<string, MappingDataNode>();
		string name = _factory.GetRegistration(typeof(TransformComponent)).Name;
		try
		{
			foreach (var (text2, componentRegistryEntry2) in prototype.Components)
			{
				if (!(text2 == name))
				{
					MappingDataNode value2 = _serializationManager.WriteValueAs<MappingDataNode>(componentRegistryEntry2.Component.GetType(), componentRegistryEntry2.Component, alwaysWrite: true, _context);
					value.Add(text2, value2);
				}
			}
		}
		catch (Exception ex)
		{
			Sawmill.Error("Failed to convert prototype " + prototype.ID + " into yaml. Exception: " + ex.Message);
		}
		_context.WritingReadingPrototypes = false;
		_prototypeDataCache[prototype.ID] = value;
		return value;
	}

	public bool TryGetRandom<T>(IRobustRandom random, [NotNullWhen(true)] out IPrototype? prototype) where T : class, IPrototype
	{
		int num = Count<T>();
		if (num == 0)
		{
			prototype = null;
			return false;
		}
		int num2 = 0;
		int num3 = random.Next(num);
		foreach (T item in EnumeratePrototypes<T>())
		{
			if (num2 == num3)
			{
				prototype = item;
				return true;
			}
			num2++;
		}
		throw new ArgumentOutOfRangeException($"Unable to pick valid prototype for {typeof(T)}?");
	}

	public List<string> ValidateStaticFields(Dictionary<Type, HashSet<string>> prototypes)
	{
		List<string> list = new List<string>();
		foreach (Type item in _reflectionManager.FindAllTypes())
		{
			if (!item.IsAbstract)
			{
				ValidateStaticFieldsInternal(item, list, prototypes);
			}
		}
		return list;
	}

	public List<string> ValidateStaticFields(Type type, Dictionary<Type, HashSet<string>> prototypes)
	{
		List<string> list = new List<string>();
		ValidateStaticFieldsInternal(type, list, prototypes);
		return list;
	}

	private void ValidateStaticFieldsInternal(Type type, List<string> errors, Dictionary<Type, HashSet<string>> prototypes)
	{
		Type type2 = type;
		BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		while (type2 != null)
		{
			FieldInfo[] fields = type2.GetFields(bindingAttr);
			foreach (FieldInfo field in fields)
			{
				ValidateStaticField(field, type, errors, prototypes);
			}
			type2 = type2.BaseType;
		}
	}

	private void ValidateStaticField(FieldInfo field, Type type, List<string> errors, Dictionary<Type, HashSet<string>> prototypes)
	{
		if (!TryGetFieldPrototype(field, out Type proto))
		{
			return;
		}
		if (!prototypes.TryGetValue(proto, out HashSet<string> value))
		{
			errors.Add($"Prototype id field failed validation. Unknown prototype kind {proto.Name}. Field: {field.Name} in {type.FullName}");
		}
		else
		{
			if (!TryGetIds(field, out string[] ids))
			{
				return;
			}
			string[] array = ids;
			foreach (string text in array)
			{
				if (!value.Contains(text))
				{
					errors.Add($"Prototype id field failed validation. Unknown prototype: {text} of type {proto.Name}. Field: {field.Name} in {type.FullName}");
				}
			}
		}
	}

	private bool TryGetIds(FieldInfo field, [NotNullWhen(true)] out string[]? ids)
	{
		ids = null;
		object value = field.GetValue(null);
		if (value == null)
		{
			return false;
		}
		if (value is string text)
		{
			ids = new string[1] { text };
			return true;
		}
		if (value is IEnumerable<string> source)
		{
			ids = source.ToArray();
			return true;
		}
		if (value is EntProtoId entProtoId)
		{
			ids = new string[1] { entProtoId };
			return true;
		}
		if (value is IEnumerable<EntProtoId> source2)
		{
			ids = source2.Select((EntProtoId x) => x.Id).ToArray();
			return true;
		}
		if (field.FieldType.IsGenericType)
		{
			Type genericTypeDefinition = field.FieldType.GetGenericTypeDefinition();
			if (genericTypeDefinition == typeof(ProtoId<>) || genericTypeDefinition == typeof(EntProtoId<>))
			{
				ids = new string[1] { value.ToString() };
				return true;
			}
		}
		Type[] interfaces = field.FieldType.GetInterfaces();
		foreach (Type type in interfaces)
		{
			if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(IEnumerable<>))
			{
				continue;
			}
			Type type2 = type.GetGenericArguments().Single();
			if (type2.IsGenericType)
			{
				Type genericTypeDefinition2 = type2.GetGenericTypeDefinition();
				if (!(genericTypeDefinition2 != typeof(ProtoId<>)) || !(genericTypeDefinition2 != typeof(EntProtoId<>)))
				{
					ids = GetEnumerableIds((IEnumerable)value).ToArray();
					return true;
				}
			}
		}
		return false;
	}

	private IEnumerable<string> GetEnumerableIds(IEnumerable ids)
	{
		foreach (object id in ids)
		{
			yield return id.ToString();
		}
	}

	private bool TryGetFieldPrototype(FieldInfo field, [NotNullWhen(true)] out Type? proto)
	{
		Attribute customAttribute = field.GetCustomAttribute(typeof(ValidatePrototypeIdAttribute<>), inherit: false);
		if (customAttribute != null)
		{
			proto = customAttribute.GetType().GetGenericArguments().First();
			return true;
		}
		if (TryGetPrototypeFromType(field.FieldType, out proto))
		{
			return true;
		}
		foreach (Type item in from x in field.FieldType.GetInterfaces()
			where x.IsGenericType
			select x)
		{
			if (!(item.GetGenericTypeDefinition() != typeof(IEnumerable<>)))
			{
				Type type = item.GetGenericArguments().Single();
				if (TryGetPrototypeFromType(type, out proto))
				{
					return true;
				}
			}
		}
		proto = null;
		return false;
	}

	private bool TryGetPrototypeFromType(Type type, [NotNullWhen(true)] out Type? proto)
	{
		if (type == typeof(EntProtoId))
		{
			proto = typeof(EntityPrototype);
			return true;
		}
		if (!type.IsGenericType)
		{
			proto = null;
			return false;
		}
		if (type.GetGenericTypeDefinition() == typeof(EntProtoId<>))
		{
			proto = typeof(EntityPrototype);
			return true;
		}
		if (type.GetGenericTypeDefinition() == typeof(ProtoId<>))
		{
			proto = type.GetGenericArguments().Single();
			return true;
		}
		proto = null;
		return false;
	}

	public void LoadDirectory(ResPath path, bool overwrite = false, Dictionary<Type, HashSet<string>>? changed = null)
	{
		_hasEverBeenReloaded = true;
		ResPath[] array = (from filePath in Resources.ContentFindFiles(path)
			where filePath.Extension == "yml" && !filePath.Filename.StartsWith(".")
			select filePath).ToArray();
		new System.Random().Shuffle(array.AsSpan());
		ISawmill sawmill = _logManager.GetSawmill("eng");
		foreach (var item3 in array.AsParallel().Select(delegate(ResPath file)
		{
			try
			{
				bool flag = IsFileAbstract(file);
				using StreamReader streamReader = ReadFile(file, !overwrite);
				if (streamReader == null)
				{
					return ((ResPath, IEnumerable<ExtractedMappingData>))(file, Array.Empty<ExtractedMappingData>());
				}
				List<ExtractedMappingData> list = new List<ExtractedMappingData>();
				int num = 0;
				foreach (DataNodeDocument item in DataNodeParser.ParseYamlStream((TextReader)streamReader, true))
				{
					num++;
					this.LoadedData?.Invoke(item);
					DataNode root = item.Root;
					if (!(root is SequenceDataNode sequenceDataNode))
					{
						if (!(root is ValueDataNode { Value: "" }))
						{
							sawmill.Error($"{file} document #{num} is not a sequence! Did you forget to indent your prototype with a '-'?");
						}
					}
					else
					{
						foreach (DataNode item2 in sequenceDataNode.Sequence)
						{
							ExtractedMappingData extractedMappingData = ExtractMapping((MappingDataNode)item2);
							if (extractedMappingData != null)
							{
								if (flag)
								{
									AbstractPrototype(extractedMappingData.Data);
								}
								list.Add(extractedMappingData);
							}
						}
					}
				}
				return ((ResPath, IEnumerable<ExtractedMappingData>))(file, list);
			}
			catch (Exception value3)
			{
				sawmill.Error($"Exception whilst loading prototypes from {file}:\n{value3}");
				return ((ResPath, IEnumerable<ExtractedMappingData>))(file, Array.Empty<ExtractedMappingData>());
			}
		}))
		{
			var (value, _) = item3;
			foreach (ExtractedMappingData item4 in item3.Item2)
			{
				try
				{
					MergeMapping(item4, overwrite, changed);
				}
				catch (Exception value2)
				{
					sawmill.Error($"Exception whilst loading prototypes from {value}:\n{value2}");
				}
			}
		}
	}

	private StreamReader? ReadFile(ResPath file, bool @throw = true)
	{
		int num = 0;
		while (true)
		{
			try
			{
				return new StreamReader(Resources.ContentFileRead(file), EncodingHelpers.UTF8);
			}
			catch (IOException value)
			{
				if (num > 10)
				{
					if (@throw)
					{
						throw;
					}
					Sawmill.Error($"Error reloading prototypes in file {file}:\n{value}");
					return null;
				}
				num++;
				Thread.Sleep(10);
			}
		}
	}

	public void LoadFile(ResPath file, bool overwrite = false, Dictionary<Type, HashSet<string>>? changed = null)
	{
		try
		{
			bool flag = IsFileAbstract(file);
			using StreamReader streamReader = ReadFile(file, !overwrite);
			if (streamReader == null)
			{
				return;
			}
			int num = 0;
			foreach (DataNodeDocument item in DataNodeParser.ParseYamlStream((TextReader)streamReader, true))
			{
				this.LoadedData?.Invoke(item);
				try
				{
					foreach (DataNode item2 in ((SequenceDataNode)item.Root).Sequence)
					{
						ExtractedMappingData extractedMappingData = ExtractMapping((MappingDataNode)item2);
						if (!(extractedMappingData == null))
						{
							if (flag)
							{
								AbstractPrototype(extractedMappingData.Data);
							}
							MergeMapping(extractedMappingData, overwrite, changed);
						}
					}
				}
				catch (Exception value)
				{
					Sawmill.Error($"Exception whilst loading prototypes from {file}#{num}:\n{value}");
				}
				num++;
			}
		}
		catch (Exception ex)
		{
			Sawmill.Error("YamlException whilst loading prototypes from {0}: {1}", file, ex.Message);
		}
	}

	private ExtractedMappingData? ExtractMapping(MappingDataNode dataNode)
	{
		string value = dataNode.Get<ValueDataNode>("type").Value;
		if (_ignoredPrototypeTypes.Contains(value))
		{
			return null;
		}
		if (!_kindNames.TryGetValue(value, out Type value2))
		{
			throw new PrototypeLoadException("Unknown prototype type: '" + value + "'");
		}
		KindData obj = _kinds[value2];
		if (!dataNode.TryGet("id", out ValueDataNode node))
		{
			throw new PrototypeLoadException("Prototype type " + value + " is missing an 'id' datafield.");
		}
		string value3 = node.Value;
		string[] parents = null;
		if (obj.Inheritance != null && dataNode.TryGet("parent", out DataNode node2))
		{
			parents = _serializationManager.Read<string[]>(node2, null, skipHook: false, null, notNullableOverride: true);
		}
		return new ExtractedMappingData(value2, value3, parents, dataNode);
	}

	private void MergeMapping(ExtractedMappingData mapping, bool overwrite, Dictionary<Type, HashSet<string>>? changed)
	{
		mapping.Deconstruct(out Type Kind, out string Id, out string[] Parents, out MappingDataNode Data);
		Type type = Kind;
		string text = Id;
		string[] array = Parents;
		MappingDataNode value = Data;
		KindData kindData = _kinds[type];
		if (!overwrite && kindData.RawResults.ContainsKey(text))
		{
			throw new PrototypeLoadException($"Duplicate ID: '{text}' for kind '{type}");
		}
		kindData.RawResults[text] = value;
		MultiRootInheritanceGraph<string> inheritance = kindData.Inheritance;
		if (inheritance != null)
		{
			if (array != null)
			{
				inheritance.Add(text, array);
			}
			else
			{
				inheritance.Add(text);
			}
		}
		changed?.GetOrNew(type).Add(text);
	}

	public void LoadFromStream(TextReader stream, bool overwrite = false, Dictionary<Type, HashSet<string>>? changed = null)
	{
		_hasEverBeenReloaded = true;
		int num = 0;
		foreach (DataNodeDocument item in DataNodeParser.ParseYamlStream(stream, internStrings: true))
		{
			this.LoadedData?.Invoke(item);
			try
			{
				foreach (MappingDataNode item2 in ((SequenceDataNode)item.Root).Cast<MappingDataNode>())
				{
					ExtractedMappingData extractedMappingData = ExtractMapping(item2);
					if (!(extractedMappingData == null))
					{
						MergeMapping(extractedMappingData, overwrite, changed);
					}
				}
				num++;
			}
			catch (Exception inner)
			{
				throw new PrototypeLoadException($"Failed to load prototypes from document#{num}", inner);
			}
		}
	}

	public void LoadString(string str, bool overwrite = false, Dictionary<Type, HashSet<string>>? changed = null)
	{
		LoadFromStream(new StringReader(str), overwrite, changed);
	}

	public void RemoveString(string prototypes)
	{
		StringReader reader = new StringReader(prototypes);
		HashSet<KindData> hashSet = new HashSet<KindData>();
		foreach (DataNodeDocument item in DataNodeParser.ParseYamlStream((TextReader)reader))
		{
			foreach (MappingDataNode item2 in ((SequenceDataNode)item.Root).Cast<MappingDataNode>())
			{
				string value = item2.Get<ValueDataNode>("type").Value;
				if (_kindNames.TryGetValue(value, out Type value2))
				{
					KindData kindData = _kinds[value2];
					string value3 = item2.Get<ValueDataNode>("id").Value;
					kindData.Inheritance?.Remove(value3, force: true);
					KindData kindData2 = kindData;
					if (kindData2.UnfrozenInstances == null)
					{
						kindData2.UnfrozenInstances = kindData.Instances.ToDictionary();
					}
					kindData.UnfrozenInstances.Remove(value3);
					kindData.Results.Remove(value3);
					kindData.RawResults.Remove(value3);
					hashSet.Add(kindData);
				}
			}
		}
		Freeze(hashSet);
	}

	public void AbstractFile(ResPath path)
	{
		_abstractFiles.Add(path);
	}

	public void AbstractDirectory(ResPath path)
	{
		_abstractDirectories.Add(path);
	}

	private bool IsFileAbstract(ResPath file)
	{
		ResPath? relative;
		if (_abstractFiles.Count > 0)
		{
			foreach (ResPath abstractFile in _abstractFiles)
			{
				if (file.TryRelativeTo(abstractFile, out relative))
				{
					return true;
				}
			}
		}
		if (_abstractDirectories.Count > 0)
		{
			foreach (ResPath abstractDirectory in _abstractDirectories)
			{
				if (file.TryRelativeTo(abstractDirectory, out relative))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void AbstractPrototype(MappingDataNode mapping)
	{
		if (mapping.TryGet("abstract", out DataNode node))
		{
			if (!(node is ValueDataNode valueDataNode))
			{
				mapping["abstract"] = new ValueDataNode("true");
			}
			else
			{
				valueDataNode.Value = "true";
			}
		}
		else
		{
			mapping.Add("abstract", "true");
		}
	}

	public Dictionary<string, HashSet<ErrorNode>> ValidateDirectory(ResPath path)
	{
		Dictionary<Type, HashSet<string>> protos;
		return ValidateDirectory(path, out protos);
	}

	public Dictionary<string, HashSet<ErrorNode>> ValidateDirectory(ResPath path, out Dictionary<Type, HashSet<string>> protos)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		ParallelQuery<ResPath> parallelQuery = from filePath in Resources.ContentFindFiles(path).ToList().AsParallel()
			where filePath.Extension == "yml" && !filePath.Filename.StartsWith(".")
			select filePath;
		Dictionary<string, HashSet<ErrorNode>> dictionary = new Dictionary<string, HashSet<ErrorNode>>();
		Dictionary<Type, Dictionary<string, PrototypeValidationData>> dictionary2 = new Dictionary<Type, Dictionary<string, PrototypeValidationData>>();
		foreach (ResPath item2 in parallelQuery)
		{
			using StreamReader streamReader = ReadFile(item2);
			if (streamReader == null)
			{
				continue;
			}
			YamlStream val = new YamlStream();
			try
			{
				val.Load((TextReader)streamReader);
			}
			catch (Exception value)
			{
				throw new PrototypeLoadException($"Error loading file: '{item2}'\n{value}");
			}
			foreach (YamlDocument document in val.Documents)
			{
				foreach (YamlMappingNode item3 in ((IEnumerable)(YamlSequenceNode)document.RootNode).Cast<YamlMappingNode>())
				{
					string text = item3.GetNode("type").AsString();
					if (!_ignoredPrototypeTypes.Contains(text))
					{
						if (!_kindNames.TryGetValue(text, out Type value2))
						{
							throw new PrototypeLoadException("Unknown prototype type: '" + text + "'");
						}
						MappingDataNode mappingDataNode = ((YamlNode)(object)item3).ToDataNodeCast<MappingDataNode>();
						string id = mappingDataNode.Get<ValueDataNode>("id").Value;
						PrototypeValidationData prototypeValidationData = new PrototypeValidationData(id, mappingDataNode, item2.ToString());
						mappingDataNode.Remove("type");
						if (DisallowedIdChars.TryFirstOrNull((char c) => id.Contains(c), out var element))
						{
							dictionary.GetOrNew(prototypeValidationData.File).Add(new ErrorNode(mappingDataNode, $"Prototype '{id}' ({value2}) contains disallowed character '{element}'."));
						}
						if (!dictionary2.GetOrNew(value2).TryAdd(id, prototypeValidationData))
						{
							ErrorNode item = new ErrorNode(mappingDataNode, $"Found dupe prototype ID of {id} for {value2}");
							dictionary.GetOrNew(prototypeValidationData.File).Add(item);
						}
					}
				}
			}
		}
		YamlValidationContext context = new YamlValidationContext();
		List<ErrorNode> list = new List<ErrorNode>();
		Type key;
		Dictionary<string, PrototypeValidationData> value3;
		foreach (KeyValuePair<Type, Dictionary<string, PrototypeValidationData>> item4 in dictionary2)
		{
			item4.Deconstruct(out key, out value3);
			Type type = key;
			Dictionary<string, PrototypeValidationData> dictionary3 = value3;
			foreach (var (value4, prototypeValidationData3) in dictionary3)
			{
				list.Clear();
				EnsurePushed(prototypeValidationData3, dictionary3, type);
				if (prototypeValidationData3.Mapping.TryGet("abstract", out ValueDataNode node) && bool.Parse(node.Value))
				{
					continue;
				}
				list.AddRange(_serializationManager.ValidateNode(type, prototypeValidationData3.Mapping).GetErrors());
				if (list.Count > 0)
				{
					dictionary.GetOrNew(prototypeValidationData3.File).UnionWith(list);
				}
				try
				{
					object value5 = _serializationManager.Read(type, prototypeValidationData3.Mapping, context);
					DataNode node2 = _serializationManager.WriteValue(type, value5, alwaysWrite: true, context);
					list.AddRange(_serializationManager.ValidateNode(type, node2, context).GetErrors());
					if (list.Count > 0)
					{
						dictionary.GetOrNew(prototypeValidationData3.File).UnionWith(list);
					}
				}
				catch (Exception value6)
				{
					list.Add(new ErrorNode(new ValueDataNode(), $"Caught Exception while validating {type} prototype {value4}. Exception: {value6}"));
				}
			}
		}
		protos = new Dictionary<Type, HashSet<string>>(dictionary2.Count);
		foreach (KeyValuePair<Type, Dictionary<string, PrototypeValidationData>> item5 in dictionary2)
		{
			item5.Deconstruct(out key, out value3);
			Type key2 = key;
			Dictionary<string, PrototypeValidationData> dictionary4 = value3;
			protos[key2] = dictionary4.Keys.ToHashSet();
		}
		return dictionary;
	}

	public Dictionary<Type, Dictionary<string, HashSet<ErrorNode>>> ValidateAllPrototypesSerializable(ISerializationContext? ctx)
	{
		Dictionary<Type, Dictionary<string, HashSet<ErrorNode>>> dictionary = new Dictionary<Type, Dictionary<string, HashSet<ErrorNode>>>();
		Dictionary<string, HashSet<ErrorNode>> dictionary2 = new Dictionary<string, HashSet<ErrorNode>>();
		foreach (KeyValuePair<Type, KindData> kind in _kinds)
		{
			kind.Deconstruct(out var key, out var value);
			Type type = key;
			ImmutableArray<IPrototype>.Enumerator enumerator2 = value.Instances.Values.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				IPrototype current = enumerator2.Current;
				bool caughtException;
				HashSet<ErrorNode> hashSet = ValidateProto(type, current, ctx, out caughtException);
				if (hashSet.Count > 0)
				{
					dictionary2.GetOrNew(current.ID).UnionWith(hashSet);
				}
				if (caughtException)
				{
					break;
				}
			}
			if (dictionary2.Count > 0)
			{
				dictionary[type] = dictionary2;
				dictionary2 = new Dictionary<string, HashSet<ErrorNode>>();
			}
		}
		return dictionary;
	}

	private HashSet<ErrorNode> ValidateProto(Type type, IPrototype instance, ISerializationContext? ctx, out bool caughtException)
	{
		caughtException = false;
		DataNode node;
		try
		{
			node = _serializationManager.WriteValue(type, instance, alwaysWrite: true, ctx);
		}
		catch (Exception value)
		{
			caughtException = true;
			string errorReason = $"Caught exception while writing. Exception: {value}";
			return new HashSet<ErrorNode>
			{
				new ErrorNode(new ValueDataNode(""), errorReason)
			};
		}
		try
		{
			return _serializationManager.ValidateNode(type, node, ctx).GetErrors().ToHashSet();
		}
		catch (Exception value2)
		{
			caughtException = true;
			string errorReason2 = $"Caught exception while validating. Exception: {value2}";
			return new HashSet<ErrorNode>
			{
				new ErrorNode(new ValueDataNode(""), errorReason2)
			};
		}
	}

	private void EnsurePushed(PrototypeValidationData data, Dictionary<string, PrototypeValidationData> prototypes, Type type)
	{
		if (data.Pushed)
		{
			return;
		}
		data.Pushed = true;
		if (data.Mapping.TryGet("parent", out DataNode node))
		{
			data.Parents = _serializationManager.Read<string[]>(node, null, skipHook: false, null, notNullableOverride: true);
			data.ParentMappings = new MappingDataNode[data.Parents.Length];
			int num = 0;
			string[] parents = data.Parents;
			foreach (string key in parents)
			{
				PrototypeValidationData prototypeValidationData = prototypes[key];
				EnsurePushed(prototypeValidationData, prototypes, type);
				data.ParentMappings[num++] = prototypeValidationData.Mapping;
			}
			data.Mapping = _serializationManager.PushCompositionWithGenericNode(type, data.ParentMappings, data.Mapping);
		}
	}
}
