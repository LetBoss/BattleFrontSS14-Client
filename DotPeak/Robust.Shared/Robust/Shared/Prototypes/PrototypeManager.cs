// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.PrototypeManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Robust.Shared.Prototypes;

public abstract class PrototypeManager : IPrototypeManagerInternal, IPrototypeManager
{
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
  private FrozenDictionary<Type, PrototypeManager.KindData> _kinds = FrozenDictionary<Type, PrototypeManager.KindData>.Empty;
  private readonly HashSet<string> _ignoredPrototypeTypes = new HashSet<string>();
  private readonly List<ResPath> _abstractFiles = new List<ResPath>();
  private readonly List<ResPath> _abstractDirectories = new List<ResPath>();
  private static readonly char[] DisallowedIdChars = new char[2]
  {
    ' ',
    '.'
  };

  public FrozenDictionary<ProtoId<EntityCategoryPrototype>, IReadOnlyList<EntityPrototype>> Categories { get; private set; } = FrozenDictionary<ProtoId<EntityCategoryPrototype>, IReadOnlyList<EntityPrototype>>.Empty;

  private void UpdateCategories()
  {
    Dictionary<string, HashSet<EntityCategoryPrototype>> automaticCategories = this.GetAutomaticCategories();
    Dictionary<EntProtoId, IReadOnlySet<EntityCategoryPrototype>> cache = new Dictionary<EntProtoId, IReadOnlySet<EntityCategoryPrototype>>(this.Count<EntityPrototype>());
    Dictionary<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>> dictionary = new Dictionary<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>(this.Count<EntityCategoryPrototype>());
    foreach (EntityPrototype enumeratePrototype in this.EnumeratePrototypes<EntityPrototype>())
      this.UpdateCategories((EntProtoId) enumeratePrototype, cache, automaticCategories, dictionary);
    foreach (EntityCategoryPrototype enumeratePrototype in this.EnumeratePrototypes<EntityCategoryPrototype>())
      dictionary.GetOrNew<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>((ProtoId<EntityCategoryPrototype>) enumeratePrototype.ID);
    this.Categories = dictionary.ToFrozenDictionary<KeyValuePair<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>, ProtoId<EntityCategoryPrototype>, IReadOnlyList<EntityPrototype>>((Func<KeyValuePair<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>, ProtoId<EntityCategoryPrototype>>) (x => x.Key), (Func<KeyValuePair<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>, IReadOnlyList<EntityPrototype>>) (x => (IReadOnlyList<EntityPrototype>) x.Value));
  }

  private Dictionary<string, HashSet<EntityCategoryPrototype>> GetAutomaticCategories()
  {
    Dictionary<string, HashSet<EntityCategoryPrototype>> dict = new Dictionary<string, HashSet<EntityCategoryPrototype>>();
    foreach (EntityCategoryPrototype enumeratePrototype in this.EnumeratePrototypes<EntityCategoryPrototype>())
    {
      if (enumeratePrototype.Components != null)
      {
        foreach (string component in enumeratePrototype.Components)
          dict.GetOrNew<string, HashSet<EntityCategoryPrototype>>(component).Add(enumeratePrototype);
      }
    }
    if (this._autoComps == null)
      this._autoComps = this._factory.GetAllRegistrations().Where<ComponentRegistration>((Func<ComponentRegistration, bool>) (x => x.Type.HasCustomAttribute<EntityCategoryAttribute>())).Select<ComponentRegistration, (string, EntityCategoryAttribute)>((Func<ComponentRegistration, (string, EntityCategoryAttribute)>) (x => (x.Name, x.Type.GetCustomAttribute<EntityCategoryAttribute>()))).ToArray<(string, EntityCategoryAttribute)>();
    foreach ((string, EntityCategoryAttribute) autoComp in this._autoComps)
    {
      string key = autoComp.Item1;
      foreach (string category in autoComp.Item2.Categories)
      {
        EntityCategoryPrototype prototype;
        if (this.TryIndex<EntityCategoryPrototype>(category, out prototype))
          dict.GetOrNew<string, HashSet<EntityCategoryPrototype>>(key).Add(prototype);
        else
          this.Sawmill.Error($"Component {key} has invalid {"EntityCategoryAttribute"} argument: {category}");
      }
    }
    return dict;
  }

  private IReadOnlySet<EntityCategoryPrototype> UpdateCategories(
    EntProtoId id,
    Dictionary<EntProtoId, IReadOnlySet<EntityCategoryPrototype>> cache,
    Dictionary<string, HashSet<EntityCategoryPrototype>> autoCategories,
    Dictionary<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>> categories)
  {
    IReadOnlySet<EntityCategoryPrototype> readOnlySet;
    if (cache.TryGetValue(id, out readOnlySet))
      return readOnlySet;
    HashSet<EntityCategoryPrototype> categoryPrototypeSet = new HashSet<EntityCategoryPrototype>();
    MappingDataNode mappings;
    if (!this.TryGetMapping(typeof (EntityPrototype), (string) id, out mappings))
      throw new UnknownPrototypeException((string) id, typeof (EntityPrototype));
    SequenceDataNode node;
    if (mappings.TryGet<SequenceDataNode>(nameof (categories), out node))
    {
      foreach (ValueDataNode valueDataNode in node)
      {
        string id1 = valueDataNode.Value;
        EntityCategoryPrototype prototype;
        if (this.TryIndex<EntityCategoryPrototype>(id1, out prototype))
          categoryPrototypeSet.Add(prototype);
        else
          this.Sawmill.Error($"Entity prototype {id} specifies an invalid {"EntityCategoryPrototype"}: {id1}");
      }
    }
    foreach ((string id, EntityPrototype) enumerateAllParent in this.EnumerateAllParents<EntityPrototype>((string) id, false))
    {
      foreach (EntityCategoryPrototype updateCategory in (IEnumerable<EntityCategoryPrototype>) this.UpdateCategories((EntProtoId) enumerateAllParent.id, cache, autoCategories, categories))
      {
        if (updateCategory.Inheritable)
          categoryPrototypeSet.Add(updateCategory);
      }
    }
    EntityPrototype prototype1;
    if (!this.TryIndex(id, out prototype1))
    {
      cache.Add(id, (IReadOnlySet<EntityCategoryPrototype>) categoryPrototypeSet);
      return (IReadOnlySet<EntityCategoryPrototype>) categoryPrototypeSet;
    }
    foreach (string key in prototype1.Components.Keys)
    {
      HashSet<EntityCategoryPrototype> other;
      if (autoCategories.TryGetValue(key, out other))
        categoryPrototypeSet.UnionWith((IEnumerable<EntityCategoryPrototype>) other);
    }
    cache.Add(id, (IReadOnlySet<EntityCategoryPrototype>) categoryPrototypeSet);
    prototype1.Categories = (IReadOnlySet<EntityCategoryPrototype>) categoryPrototypeSet;
    foreach (EntityCategoryPrototype key in categoryPrototypeSet)
    {
      if (key.HideSpawnMenu)
        prototype1.HideSpawnMenu = true;
      categories.GetOrNew<ProtoId<EntityCategoryPrototype>, List<EntityPrototype>>((ProtoId<EntityCategoryPrototype>) key).Add(prototype1);
    }
    return (IReadOnlySet<EntityCategoryPrototype>) categoryPrototypeSet;
  }

  public virtual void Initialize()
  {
    if (this._initialized)
      return;
    this.Sawmill = this._logManager.GetSawmill("proto");
    this._initialized = true;
    this.ReloadPrototypeKinds();
    this.PrototypesReloaded += new Action<PrototypesReloadedEventArgs>(this.OnReload);
  }

  public IEnumerable<string> GetPrototypeKinds() => (IEnumerable<string>) this._kindNames.Keys;

  public int Count<T>() where T : class, IPrototype => this._kinds[typeof (T)].Instances.Count;

  public IEnumerable<T> EnumeratePrototypes<T>() where T : class, IPrototype
  {
    return (IEnumerable<T>) this.GetInstances<T>().Values;
  }

  public IEnumerable<IPrototype> EnumeratePrototypes(Type kind)
  {
    if (!this._hasEverBeenReloaded)
      throw new InvalidOperationException("No prototypes have been loaded yet.");
    return (IEnumerable<IPrototype>) this._kinds[kind].Instances.Values;
  }

  public IEnumerable<IPrototype> EnumeratePrototypes(string variant)
  {
    return this.EnumeratePrototypes(this.GetKindType(variant));
  }

  public IEnumerable<T> EnumerateParents<T>(T proto, bool includeSelf = false) where T : class, IPrototype, IInheritingPrototype
  {
    return this.EnumerateParents<T>(proto.ID, includeSelf);
  }

  public IEnumerable<T> EnumerateParents<T>(string id, bool includeSelf = false) where T : class, IPrototype, IInheritingPrototype
  {
    if (!this._hasEverBeenReloaded)
      throw new InvalidOperationException("No prototypes have been loaded yet.");
    T prototype;
    if (this.TryIndex<T>(id, out prototype))
    {
      if (includeSelf)
        yield return prototype;
      if (prototype.Parents != null)
      {
        Queue<string> queue = new Queue<string>((IEnumerable<string>) prototype.Parents);
        string result;
        while (queue.TryDequeue(out result))
        {
          T parent;
          if (this.TryIndex<T>(result, out parent))
          {
            yield return parent;
            if (parent.Parents != null)
            {
              foreach (string parent1 in parent.Parents)
                queue.Enqueue(parent1);
              parent = default (T);
            }
          }
        }
      }
    }
  }

  public IEnumerable<IPrototype> EnumerateParents(Type kind, string id, bool includeSelf = false)
  {
    if (!this._hasEverBeenReloaded)
      throw new InvalidOperationException("No prototypes have been loaded yet.");
    if (!kind.IsAssignableTo(typeof (IInheritingPrototype)))
      throw new InvalidOperationException("The provided prototype type is not an inheriting prototype");
    IPrototype prototype;
    if (this.TryIndex(kind, id, out prototype))
    {
      if (includeSelf)
        yield return prototype;
      IInheritingPrototype inheritingPrototype1 = (IInheritingPrototype) prototype;
      if (inheritingPrototype1.Parents != null)
      {
        Queue<string> queue = new Queue<string>((IEnumerable<string>) inheritingPrototype1.Parents);
        string result;
        while (queue.TryDequeue(out result))
        {
          IPrototype parent;
          if (this.TryIndex(kind, result, out parent))
          {
            yield return parent;
            IInheritingPrototype inheritingPrototype2 = (IInheritingPrototype) parent;
            if (inheritingPrototype2.Parents != null)
            {
              foreach (string parent1 in inheritingPrototype2.Parents)
                queue.Enqueue(parent1);
              parent = (IPrototype) null;
            }
          }
        }
      }
    }
  }

  public IEnumerable<(string id, T?)> EnumerateAllParents<T>(string id, bool includeSelf = false) where T : class, IPrototype, IInheritingPrototype
  {
    if (!this._hasEverBeenReloaded)
      throw new InvalidOperationException("No prototypes have been loaded yet.");
    PrototypeManager.KindData kindData;
    if (!this._kinds.TryGetValue(typeof (T), out kindData))
      throw new UnknownPrototypeException(id, typeof (T));
    if (kindData.Results.ContainsKey(id))
    {
      if (includeSelf)
      {
        IPrototype prototype;
        kindData.Instances.TryGetValue(id, out prototype);
        yield return (id, prototype as T);
      }
      string[] parents1;
      if (kindData.Inheritance.TryGetParents(id, out parents1))
      {
        Queue<string> queue = new Queue<string>((IEnumerable<string>) parents1);
        string prototypeId;
        while (queue.TryDequeue(out prototypeId))
        {
          if (!kindData.Results.ContainsKey(prototypeId))
          {
            this.Sawmill.Error($"Encountered invalid prototype while enumerating parents. Kind: {typeof (T).Name}. Child: {id}. Invalid: {prototypeId}");
          }
          else
          {
            IPrototype prototype;
            kindData.Instances.TryGetValue(prototypeId, out prototype);
            yield return (prototypeId, prototype as T);
            string[] parents2;
            if (kindData.Inheritance.TryGetParents(prototypeId, out parents2))
            {
              foreach (string str in parents2)
                queue.Enqueue(str);
            }
          }
        }
      }
    }
  }

  public IEnumerable<Type> EnumeratePrototypeKinds()
  {
    if (!this._hasEverBeenReloaded)
      throw new InvalidOperationException("No prototypes have been loaded yet.");
    return (IEnumerable<Type>) this._kinds.Keys;
  }

  public T Index<T>(string id) where T : class, IPrototype
  {
    if (!this._hasEverBeenReloaded)
      throw new InvalidOperationException("No prototypes have been loaded yet.");
    try
    {
      return (T) this._kinds[typeof (T)].Instances[id];
    }
    catch (KeyNotFoundException ex)
    {
      throw new UnknownPrototypeException(id, typeof (T));
    }
  }

  public EntityPrototype Index(EntProtoId id) => this.Index<EntityPrototype>(id.Id);

  public T Index<T>(ProtoId<T> id) where T : class, IPrototype => this.Index<T>(id.Id);

  public IPrototype Index(Type kind, string id)
  {
    if (!this._hasEverBeenReloaded)
      throw new InvalidOperationException("No prototypes have been loaded yet.");
    try
    {
      return this._kinds[kind].Instances[id];
    }
    catch (KeyNotFoundException ex)
    {
      throw new UnknownPrototypeException(id, kind);
    }
  }

  public void Clear()
  {
    this._kindNames.Clear();
    this._kinds = FrozenDictionary<Type, PrototypeManager.KindData>.Empty;
  }

  public void Reset()
  {
    Dictionary<Type, HashSet<string>> dictionary1 = this._kinds.ToDictionary<KeyValuePair<Type, PrototypeManager.KindData>, Type, HashSet<string>>((Func<KeyValuePair<Type, PrototypeManager.KindData>, Type>) (x => x.Key), (Func<KeyValuePair<Type, PrototypeManager.KindData>, HashSet<string>>) (x => x.Value.Instances.Keys.ToHashSet<string>()));
    this.ReloadPrototypeKinds();
    Dictionary<Type, HashSet<string>> dictionary2 = new Dictionary<Type, HashSet<string>>();
    this.LoadDefaultPrototypes(dictionary2);
    foreach ((Type key, HashSet<string> other) in dictionary2)
    {
      HashSet<string> stringSet;
      if (dictionary1.TryGetValue(key, out stringSet))
      {
        stringSet.ExceptWith((IEnumerable<string>) other);
        if (stringSet.Count == 0)
          dictionary1.Remove(key);
      }
    }
    this.ReloadPrototypes(dictionary2, dictionary1);
    this._locMan.ReloadLocalizations();
  }

  public abstract void LoadDefaultPrototypes(Dictionary<Type, HashSet<string>>? changed = null);

  private int SortPrototypesByPriority(Type a, Type b)
  {
    return this._kindPriorities[b].CompareTo(this._kindPriorities[a]);
  }

  protected void ReloadPrototypes(IEnumerable<ResPath> filePaths)
  {
  }

  public void ReloadPrototypes(
    Dictionary<Type, HashSet<string>> modified,
    Dictionary<Type, HashSet<string>>? removed = null)
  {
    List<Type> list = modified.Keys.ToList<Type>();
    list.Sort(new Comparison<Type>(this.SortPrototypesByPriority));
    Dictionary<Type, PrototypesReloadedEventArgs.PrototypeChangeSet> ByType = new Dictionary<Type, PrototypesReloadedEventArgs.PrototypeChangeSet>();
    HashSet<PrototypeManager.KindData> kindDataSet = new HashSet<PrototypeManager.KindData>();
    HashSet<string> toProcess = new HashSet<string>();
    Queue<string> processQueue = new Queue<string>();
    foreach (Type type in list)
    {
      Dictionary<string, IPrototype> Modified = new Dictionary<string, IPrototype>();
      PrototypeManager.KindData kindData1 = this._kinds[type];
      MultiRootInheritanceGraph<string> tree = kindData1.Inheritance;
      toProcess.Clear();
      processQueue.Clear();
      foreach (string id in modified[type])
        AddToQueue(id);
      string result;
      while (processQueue.TryDequeue(out result))
      {
        if (tree != null)
        {
          string[] parents1;
          if (tree.TryGetParents(result, out parents1))
          {
            bool flag = false;
            foreach (string str in parents1)
            {
              if (toProcess.Contains(str))
              {
                processQueue.Enqueue(result);
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              if (parents1.Length == 1)
              {
                kindData1.Results[result] = this._serializationManager.PushCompositionWithGenericNode<MappingDataNode>(type, kindData1.Results[parents1[0]], kindData1.RawResults[result]);
              }
              else
              {
                MappingDataNode[] parents2 = new MappingDataNode[parents1.Length];
                for (int index = 0; index < parents2.Length; ++index)
                  parents2[index] = kindData1.Results[parents1[index]];
                kindData1.Results[result] = this._serializationManager.PushCompositionWithGenericNode<MappingDataNode>(type, parents2, kindData1.RawResults[result]);
              }
            }
            else
              continue;
          }
          else
            kindData1.Results[result] = kindData1.RawResults[result];
        }
        toProcess.Remove(result);
        IPrototype prototype = this.TryReadPrototype(type, result, kindData1.Results[result], SerializationHookContext.DontSkipHooks);
        if (prototype != null)
        {
          PrototypeManager.KindData kindData2 = kindData1;
          if (kindData2.UnfrozenInstances == null)
            kindData2.UnfrozenInstances = kindData1.Instances.ToDictionary<string, IPrototype>();
          kindData1.UnfrozenInstances[result] = prototype;
          Modified.Add(result, prototype);
        }
      }
      if (Modified.Count != 0)
      {
        ByType.Add(kindData1.Type, new PrototypesReloadedEventArgs.PrototypeChangeSet((IReadOnlyDictionary<string, IPrototype>) Modified));
        kindDataSet.Add(kindData1);
      }

      void AddToQueue(string id)
      {
        if (!toProcess.Add(id))
          return;
        processQueue.Enqueue(id);
        IReadOnlySet<string> set;
        if (tree == null || !tree.TryGetChildren(id, out set))
          return;
        foreach (string id1 in (IEnumerable<string>) set)
          AddToQueue(id1);
      }
    }
    this.Freeze((IEnumerable<PrototypeManager.KindData>) kindDataSet);
    if (kindDataSet.Any<PrototypeManager.KindData>((Func<PrototypeManager.KindData, bool>) (x => x.Type == typeof (EntityPrototype) || x.Type == typeof (EntityCategoryPrototype))))
      this.UpdateCategories();
    HashSet<Type> Modified1 = new HashSet<Type>((IEnumerable<Type>) ByType.Keys);
    if (removed != null)
      Modified1.UnionWith((IEnumerable<Type>) removed.Keys);
    PrototypesReloadedEventArgs toRaise = new PrototypesReloadedEventArgs(Modified1, (IReadOnlyDictionary<Type, PrototypesReloadedEventArgs.PrototypeChangeSet>) ByType, (IReadOnlyDictionary<Type, HashSet<string>>) removed);
    Action<PrototypesReloadedEventArgs> prototypesReloaded = this.PrototypesReloaded;
    if (prototypesReloaded != null)
      prototypesReloaded(toRaise);
    this._entMan.EventBus.RaiseEvent<PrototypesReloadedEventArgs>(EventSource.Local, toRaise);
  }

  private void Freeze(IEnumerable<PrototypeManager.KindData> kinds)
  {
    RStopwatch rstopwatch = RStopwatch.StartNew();
    foreach (PrototypeManager.KindData kind in kinds)
      kind.Freeze();
    ISawmill sawmill = this.Sawmill;
    if (sawmill == null)
      return;
    sawmill.Verbose($"Freezing prototype instances took {rstopwatch.Elapsed.TotalMilliseconds:f2}ms");
  }

  public void ResolveResults()
  {
    Dictionary<Type, Task> inheritanceTasks = new Dictionary<Type, Task>();
    foreach ((Type type, PrototypeManager.KindData data) in this._kinds)
    {
      if (data.Inheritance != null)
      {
        Task task = Task.Run((Func<Task>) (() => this.PushKindInheritance(type, data)));
        inheritanceTasks.Add(type, task);
      }
    }
    foreach (IEnumerable<Type> source in (IEnumerable<IGrouping<int, Type>>) this._kinds.Keys.GroupBy<Type, int>((Func<Type, int>) (k => this._kindPriorities[k])).OrderByDescending<IGrouping<int, Type>, int>((Func<IGrouping<int, Type>, int>) (k => k.Key)))
      this.InstantiateKinds(source.Select<Type, PrototypeManager.KindData>((Func<Type, PrototypeManager.KindData>) (k => this._kinds[k])).ToArray<PrototypeManager.KindData>(), inheritanceTasks);
    this.UpdateCategories();
  }

  private void InstantiateKinds(
    PrototypeManager.KindData[] kinds,
    Dictionary<Type, Task> inheritanceTasks)
  {
    foreach (PrototypeManager.KindData kind in kinds)
    {
      Task task;
      if (inheritanceTasks.TryGetValue(kind.Type, out task))
        task.Wait();
    }
    (PrototypeManager.KindData, string, MappingDataNode, IPrototype)[] results1 = ((IEnumerable<PrototypeManager.KindData>) kinds).SelectMany<PrototypeManager.KindData, KeyValuePair<string, MappingDataNode>, (PrototypeManager.KindData, string, MappingDataNode, IPrototype)>((Func<PrototypeManager.KindData, IEnumerable<KeyValuePair<string, MappingDataNode>>>) (data => (IEnumerable<KeyValuePair<string, MappingDataNode>>) data.Results), (Func<PrototypeManager.KindData, KeyValuePair<string, MappingDataNode>, (PrototypeManager.KindData, string, MappingDataNode, IPrototype)>) ((data, results) => (data, results.Key, results.Value, (IPrototype) null))).ToArray<(PrototypeManager.KindData, string, MappingDataNode, IPrototype)>();
    this._random.Shuffle<(PrototypeManager.KindData, string, MappingDataNode, IPrototype)>(results1.AsSpan<(PrototypeManager.KindData, string, MappingDataNode, IPrototype)>());
    UnboundedChannelOptions options = new UnboundedChannelOptions();
    options.SingleReader = true;
    options.SingleWriter = false;
    options.AllowSynchronousContinuations = true;
    Channel<ISerializationHooks> hooksChannel = Channel.CreateUnbounded<ISerializationHooks>(options);
    Task task1 = Task.Run((Action) (() => this.InstantiatePrototypes(kinds, results1, hooksChannel)));
    ChannelReader<ISerializationHooks> reader = hooksChannel.Reader;
label_8:
    while (reader.WaitToReadAsync().AsTask().Result)
    {
      while (true)
      {
        ISerializationHooks serializationHooks;
        if (reader.TryRead(out serializationHooks))
          serializationHooks.AfterDeserialization();
        else
          goto label_8;
      }
    }
    task1.Wait();
  }

  private void InstantiatePrototypes(
    PrototypeManager.KindData[] kinds,
    (PrototypeManager.KindData KindData, string Id, MappingDataNode Mapping, IPrototype? Instance)[] results,
    Channel<ISerializationHooks> hooks)
  {
    SerializationHookContext hookCtx = new SerializationHookContext(hooks.Writer, false);
    try
    {
      Parallel.For(0, results.Length, (Action<int>) (i =>
      {
        ref (PrototypeManager.KindData, string, MappingDataNode, IPrototype) local = ref results[i];
        local.Item4 = this.TryReadPrototype(local.Item1.Type, local.Item2, local.Item3, hookCtx);
      }));
      foreach ((PrototypeManager.KindData KindData, string Id, MappingDataNode Mapping, IPrototype Instance) result in results)
      {
        if (result.Instance != null)
        {
          PrototypeManager.KindData kindData = result.KindData;
          if (kindData.UnfrozenInstances == null)
            kindData.UnfrozenInstances = result.KindData.Instances.ToDictionary<string, IPrototype>();
          result.KindData.UnfrozenInstances[result.Id] = result.Instance;
        }
      }
      this.Freeze(((IEnumerable<PrototypeManager.KindData>) kinds).Where<PrototypeManager.KindData>((Func<PrototypeManager.KindData, bool>) (data => data.UnfrozenInstances != null)));
    }
    finally
    {
      hooks.Writer.Complete();
    }
  }

  private IPrototype? TryReadPrototype(
    Type kind,
    string id,
    MappingDataNode mapping,
    SerializationHookContext hookCtx)
  {
    ValueDataNode node;
    if (mapping.TryGet<ValueDataNode>("abstract", out node) && node.AsBool())
      return (IPrototype) null;
    try
    {
      return (IPrototype) this._serializationManager.Read(kind, (DataNode) mapping, hookCtx);
    }
    catch (Exception ex)
    {
      this.Sawmill.Error($"Reading {kind}({id}) threw the following exception: {ex}");
      return (IPrototype) null;
    }
  }

  private async Task PushKindInheritance(Type kind, PrototypeManager.KindData data)
  {
    MultiRootInheritanceGraph<string> tree1 = data.Inheritance;
    if (tree1 == null)
      ;
    else
    {
      Dictionary<string, PrototypeManager.InheritancePushDatum> results = data.RawResults.ToDictionary<KeyValuePair<string, MappingDataNode>, string, PrototypeManager.InheritancePushDatum>((Func<KeyValuePair<string, MappingDataNode>, string>) (k => k.Key), (Func<KeyValuePair<string, MappingDataNode>, PrototypeManager.InheritancePushDatum>) (k => new PrototypeManager.InheritancePushDatum(k.Value, tree1.GetParentsCount(k.Key))));
      using (CountdownEvent countDown1 = new CountdownEvent(results.Count))
      {
        foreach (string rootNode in (IEnumerable<string>) tree1.RootNodes)
        {
          string root = rootNode;
          ThreadPool.QueueUserWorkItem((WaitCallback) (_ => ProcessItem(root, results[root])));
        }
        await WaitHandleHelpers.WaitOneAsync(countDown1.WaitHandle);
        data.Results.Clear();
        using (Dictionary<string, PrototypeManager.InheritancePushDatum>.Enumerator enumerator = results.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            (string key, PrototypeManager.InheritancePushDatum inheritancePushDatum) = enumerator.Current;
            data.Results[key] = inheritancePushDatum.Result;
          }
        }
      }
    }
    MultiRootInheritanceGraph<string> tree2;
    Dictionary<string, PrototypeManager.InheritancePushDatum> results1;
    PrototypeManager prototypeManager;
    Type kind1;
    CountdownEvent countDown;

    void ProcessItem(string id, PrototypeManager.InheritancePushDatum datum)
    {
      try
      {
        string[] parents1;
        if (tree2.TryGetParents(id, out parents1))
        {
          if (parents1.Length == 1)
          {
            datum.Result = prototypeManager._serializationManager.PushCompositionWithGenericNode<MappingDataNode>(kind1, results1[parents1[0]].Result, datum.Result);
          }
          else
          {
            MappingDataNode[] parents2 = new MappingDataNode[parents1.Length];
            for (int index = 0; index < parents1.Length; ++index)
              parents2[index] = results1[parents1[index]].Result;
            datum.Result = prototypeManager._serializationManager.PushCompositionWithGenericNode<MappingDataNode>(kind1, parents2, datum.Result);
          }
        }
        IReadOnlySet<string> set;
        if (tree2.TryGetChildren(id, out set))
        {
          foreach (string str in (IEnumerable<string>) set)
          {
            string child = str;
            PrototypeManager.InheritancePushDatum childDatum = results1[child];
            if (Interlocked.Decrement(ref childDatum.CountParentsRemaining) == 0)
              ThreadPool.QueueUserWorkItem((WaitCallback) (_ => ProcessItem(child, childDatum)));
          }
        }
        countDown.Signal();
      }
      catch (Exception ex)
      {
        prototypeManager.Sawmill.Error($"Failed to push composition for {kind1.Name} prototype with id: {id}. Exception: {ex}");
        throw;
      }
    }
  }

  public void ReloadPrototypeKinds()
  {
    this.Clear();
    Dictionary<Type, PrototypeManager.KindData> dictionary = new Dictionary<Type, PrototypeManager.KindData>();
    foreach (Type allChild in this._reflectionManager.GetAllChildren<IPrototype>())
      this.RegisterKind(allChild, dictionary);
    this.Freeze(dictionary);
  }

  public bool HasIndex<T>(string id) where T : class, IPrototype
  {
    PrototypeManager.KindData kindData;
    if (!this._kinds.TryGetValue(typeof (T), out kindData))
      throw new UnknownPrototypeException(id, typeof (T));
    return kindData.Instances.ContainsKey(id);
  }

  public bool HasIndex(EntProtoId id) => this.HasIndex<EntityPrototype>(id.Id);

  public bool HasIndex<T>(ProtoId<T> id) where T : class, IPrototype => this.HasIndex<T>(id.Id);

  public bool HasIndex(EntProtoId? id) => id.HasValue && this.HasIndex(id.Value);

  public bool HasIndex<T>(ProtoId<T>? id) where T : class, IPrototype
  {
    return id.HasValue && this.HasIndex<T>(id.Value);
  }

  public bool TryIndex<T>(string id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
  {
    IPrototype prototype1;
    int num = this.TryIndex(typeof (T), id, out prototype1) ? 1 : 0;
    prototype = (prototype1 ?? (IPrototype) null) as T;
    return num != 0;
  }

  public bool TryIndex(Type kind, string id, [NotNullWhen(true)] out IPrototype? prototype)
  {
    PrototypeManager.KindData kindData;
    if (!this._kinds.TryGetValue(kind, out kindData))
      throw new UnknownPrototypeException(id, kind);
    return kindData.Instances.TryGetValue(id, out prototype);
  }

  public bool Resolve(EntProtoId id, [NotNullWhen(true)] out EntityPrototype? prototype)
  {
    if (this.TryIndex<EntityPrototype>(id.Id, out prototype))
      return true;
    this.Sawmill.Error($"Attempted to resolve invalid {"EntProtoId"}: {id.Id}\n{Environment.StackTrace}");
    return false;
  }

  [Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
  public bool TryIndex(EntProtoId id, [NotNullWhen(true)] out EntityPrototype? prototype, bool logError = true)
  {
    return logError ? this.Resolve(id, out prototype) : this.TryIndex(id, out prototype);
  }

  public bool TryIndex([ForbidLiteral] EntProtoId id, [NotNullWhen(true)] out EntityPrototype? prototype)
  {
    return this.TryIndex<EntityPrototype>(id.Id, out prototype);
  }

  public bool Resolve<T>(ProtoId<T> id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
  {
    if (this.TryIndex<T>(id.Id, out prototype))
      return true;
    this.Sawmill.Error($"Attempted to resolve invalid ProtoId<{typeof (T).Name}>: {id.Id}\n{Environment.StackTrace}");
    return false;
  }

  [Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
  public bool TryIndex<T>(ProtoId<T> id, [NotNullWhen(true)] out T? prototype, bool logError = true) where T : class, IPrototype
  {
    return logError ? this.Resolve<T>(id, out prototype) : this.TryIndex<T>(id, out prototype);
  }

  public bool TryIndex<T>(ProtoId<T> id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
  {
    return this.TryIndex<T>(id.Id, out prototype);
  }

  public bool Resolve(EntProtoId? id, [NotNullWhen(true)] out EntityPrototype? prototype)
  {
    if (id.HasValue)
      return this.Resolve(id.Value, out prototype);
    prototype = (EntityPrototype) null;
    return false;
  }

  [Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
  public bool TryIndex(EntProtoId? id, [NotNullWhen(true)] out EntityPrototype? prototype, bool logError = true)
  {
    return logError ? this.Resolve(id, out prototype) : this.TryIndex(id, out prototype);
  }

  public bool TryIndex(EntProtoId? id, [NotNullWhen(true)] out EntityPrototype? prototype)
  {
    if (id.HasValue)
      return this.TryIndex(id.Value, out prototype);
    prototype = (EntityPrototype) null;
    return false;
  }

  public bool Resolve<T>(ProtoId<T>? id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
  {
    if (id.HasValue)
      return this.Resolve<T>(id.Value, out prototype);
    prototype = default (T);
    return false;
  }

  [Obsolete("Use Resolve() if you want to get a prototype without throwing but while still logging an error.")]
  public bool TryIndex<T>(ProtoId<T>? id, [NotNullWhen(true)] out T? prototype, bool logError = true) where T : class, IPrototype
  {
    return logError ? this.Resolve<T>(id, out prototype) : this.TryIndex<T>(id, out prototype);
  }

  public bool TryIndex<T>(ProtoId<T>? id, [NotNullWhen(true)] out T? prototype) where T : class, IPrototype
  {
    if (id.HasValue)
      return this.TryIndex<T>(id.Value, out prototype);
    prototype = default (T);
    return false;
  }

  public bool HasMapping<T>(string id)
  {
    PrototypeManager.KindData kindData;
    if (!this._kinds.TryGetValue(typeof (T), out kindData))
      throw new UnknownPrototypeException(id, typeof (T));
    return kindData.Results.ContainsKey(id);
  }

  public bool TryGetMapping(Type kind, string id, [NotNullWhen(true)] out MappingDataNode? mappings)
  {
    return this._kinds[kind].Results.TryGetValue(id, out mappings);
  }

  public bool HasKind(string kind) => this._kindNames.ContainsKey(kind);

  public Type GetKindType(string kind) => this._kindNames[kind];

  public bool TryGetKindType(string kind, [NotNullWhen(true)] out Type? prototype)
  {
    return this._kindNames.TryGetValue(kind, out prototype);
  }

  public bool TryGetKindFrom(Type type, [NotNullWhen(true)] out string? kind)
  {
    kind = (string) null;
    PrototypeManager.KindData kindData;
    if (!this._kinds.TryGetValue(type, out kindData))
      return false;
    kind = kindData.Name;
    return true;
  }

  public FrozenDictionary<string, T> GetInstances<T>() where T : IPrototype
  {
    FrozenDictionary<string, T> instances;
    if (this.TryGetInstances<T>(out instances))
      return instances;
    throw new Exception("Failed to fetch instances for kind T");
  }

  public bool TryGetInstances<T>([NotNullWhen(true)] out FrozenDictionary<string, T>? instances) where T : IPrototype
  {
    object instances1;
    if (!this.TryGetInstances(typeof (T), out instances1))
    {
      instances = (FrozenDictionary<string, T>) null;
      return false;
    }
    instances = instances1 as FrozenDictionary<string, T>;
    if (instances == null)
      instances = FrozenDictionary<string, T>.Empty;
    return true;
  }

  private bool TryGetInstances(Type kind, [NotNullWhen(true)] out object? instances)
  {
    if (!this._hasEverBeenReloaded)
      throw new InvalidOperationException("No prototypes have been loaded yet.");
    PrototypeManager.KindData kindData;
    if (!this._kinds.TryGetValue(kind, out kindData))
    {
      instances = (object) null;
      return false;
    }
    instances = kindData.InstancesDirect;
    return true;
  }

  public bool TryGetKindFrom(IPrototype prototype, [NotNullWhen(true)] out string? kind)
  {
    return this.TryGetKindFrom(prototype.GetType(), out kind);
  }

  public bool TryGetKindFrom<T>([NotNullWhen(true)] out string? kind) where T : class, IPrototype
  {
    return this.TryGetKindFrom(typeof (T), out kind);
  }

  public bool IsIgnored(string name) => this._ignoredPrototypeTypes.Contains(name);

  public void RegisterIgnore(string name) => this._ignoredPrototypeTypes.Add(name);

  private static string CalculatePrototypeName(Type type)
  {
    return PrototypeUtility.CalculatePrototypeName(type.Name);
  }

  public void RegisterKind(params Type[] kinds)
  {
    Dictionary<Type, PrototypeManager.KindData> dictionary = this._kinds.ToDictionary<Type, PrototypeManager.KindData>();
    foreach (Type kind in kinds)
      this.RegisterKind(kind, dictionary);
    this.Freeze(dictionary);
  }

  private void Freeze(Dictionary<Type, PrototypeManager.KindData> dict)
  {
    RStopwatch rstopwatch = RStopwatch.StartNew();
    this._kinds = dict.ToFrozenDictionary<Type, PrototypeManager.KindData>();
    ISawmill sawmill = this.Sawmill;
    if (sawmill == null)
      return;
    sawmill.Verbose($"Freezing prototype kinds took {rstopwatch.Elapsed.TotalMilliseconds:f2}ms");
  }

  private void RegisterKind(Type kind, Dictionary<Type, PrototypeManager.KindData> kinds)
  {
    PrototypeAttribute prototypeAttribute = typeof (IPrototype).IsAssignableFrom(kind) ? (PrototypeAttribute) Attribute.GetCustomAttribute((MemberInfo) kind, typeof (PrototypeAttribute)) : throw new InvalidOperationException("Type must implement IPrototype.");
    if (prototypeAttribute == null)
      throw new InvalidImplementationException(kind, typeof (IPrototype), "No PrototypeAttribute to give it a type string.");
    string str = prototypeAttribute.Type ?? PrototypeManager.CalculatePrototypeName(kind);
    if (this._ignoredPrototypeTypes.Contains(str))
      this.Sawmill.Warning($"Registering an ignored prototype {kind}");
    Type type;
    if (this._kindNames.TryGetValue(str, out type))
      throw new InvalidImplementationException(kind, typeof (IPrototype), $"Duplicate prototype type ID: {prototypeAttribute.Type}. Current: {type}");
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    foreach (AbstractFieldInfo propertiesAndField in kind.GetAllPropertiesAndFields())
    {
      int num = propertiesAndField.HasAttribute<IdDataFieldAttribute>() ? 1 : 0;
      bool flag4 = propertiesAndField.HasAttribute<ParentDataFieldAttribute>();
      if (num != 0)
        flag1 = !flag1 ? true : throw new InvalidImplementationException(kind, typeof (IPrototype), "Found two IdDataFieldAttribute");
      if (flag4)
        flag2 = !flag2 ? true : throw new InvalidImplementationException(kind, typeof (IInheritingPrototype), "Found two ParentDataFieldAttribute");
      if ((num & (flag4 ? 1 : 0)) != 0)
        throw new InvalidImplementationException(kind, typeof (IPrototype), $"Prototype {kind} has the Id- & ParentDatafield on single member {propertiesAndField.Name}");
      if (propertiesAndField.HasAttribute<AbstractDataFieldAttribute>())
        flag3 = !flag3 ? true : throw new InvalidImplementationException(kind, typeof (IInheritingPrototype), "Found two AbstractDataFieldAttribute");
    }
    if (!flag1)
      throw new InvalidImplementationException(kind, typeof (IPrototype), "Did not find any member annotated with the IdDataFieldAttribute");
    if (kind.IsAssignableTo(typeof (IInheritingPrototype)) && (!flag2 || !flag3))
      throw new InvalidImplementationException(kind, typeof (IInheritingPrototype), "Did not find any member annotated with the ParentDataFieldAttribute and/or AbstractDataFieldAttribute");
    this._kindNames[str] = kind;
    this._kindPriorities[kind] = prototypeAttribute.LoadPriority;
    PrototypeManager.KindData kindData = new PrototypeManager.KindData(kind, str);
    kinds[kind] = kindData;
    if (kind.IsAssignableTo(typeof (IInheritingPrototype)))
      kindData.Inheritance = new MultiRootInheritanceGraph<string>();
    else
      kindData.Results = kindData.RawResults;
  }

  public event Action<PrototypesReloadedEventArgs>? PrototypesReloaded;

  private void OnReload(PrototypesReloadedEventArgs args)
  {
    PrototypesReloadedEventArgs.PrototypeChangeSet prototypeChangeSet;
    if (args.ByType.TryGetValue(typeof (EntityPrototype), out prototypeChangeSet))
    {
      foreach (string key in prototypeChangeSet.Modified.Keys)
        this._prototypeDataCache.Remove(key);
    }
    HashSet<string> stringSet;
    if (args.Removed == null || !args.Removed.TryGetValue(typeof (EntityPrototype), out stringSet))
      return;
    foreach (string key in stringSet)
      this._prototypeDataCache.Remove(key);
  }

  public IReadOnlyDictionary<string, MappingDataNode> GetPrototypeData(EntityPrototype prototype)
  {
    Dictionary<string, MappingDataNode> prototypeData1;
    if (this._prototypeDataCache.TryGetValue(prototype.ID, out prototypeData1))
      return (IReadOnlyDictionary<string, MappingDataNode>) prototypeData1;
    this._context.WritingReadingPrototypes = true;
    Dictionary<string, MappingDataNode> prototypeData2 = new Dictionary<string, MappingDataNode>();
    string name = this._factory.GetRegistration(typeof (TransformComponent)).Name;
    try
    {
      foreach ((string key, EntityPrototype.ComponentRegistryEntry componentRegistryEntry) in (Dictionary<string, EntityPrototype.ComponentRegistryEntry>) prototype.Components)
      {
        if (!(key == name))
        {
          MappingDataNode mappingDataNode = this._serializationManager.WriteValueAs<MappingDataNode>(componentRegistryEntry.Component.GetType(), (object) componentRegistryEntry.Component, true, (ISerializationContext) this._context);
          prototypeData2.Add(key, mappingDataNode);
        }
      }
    }
    catch (Exception ex)
    {
      this.Sawmill.Error($"Failed to convert prototype {prototype.ID} into yaml. Exception: {ex.Message}");
    }
    this._context.WritingReadingPrototypes = false;
    this._prototypeDataCache[prototype.ID] = prototypeData2;
    return (IReadOnlyDictionary<string, MappingDataNode>) prototypeData2;
  }

  public bool TryGetRandom<T>(IRobustRandom random, [NotNullWhen(true)] out IPrototype? prototype) where T : class, IPrototype
  {
    int maxValue = this.Count<T>();
    if (maxValue == 0)
    {
      prototype = (IPrototype) null;
      return false;
    }
    int num1 = 0;
    int num2 = random.Next(maxValue);
    foreach (T enumeratePrototype in this.EnumeratePrototypes<T>())
    {
      if (num1 == num2)
      {
        prototype = (IPrototype) enumeratePrototype;
        return true;
      }
      ++num1;
    }
    throw new ArgumentOutOfRangeException($"Unable to pick valid prototype for {typeof (T)}?");
  }

  public List<string> ValidateStaticFields(Dictionary<Type, HashSet<string>> prototypes)
  {
    List<string> errors = new List<string>();
    foreach (Type allType in this._reflectionManager.FindAllTypes())
    {
      if (!allType.IsAbstract)
        this.ValidateStaticFieldsInternal(allType, errors, prototypes);
    }
    return errors;
  }

  public List<string> ValidateStaticFields(Type type, Dictionary<Type, HashSet<string>> prototypes)
  {
    List<string> errors = new List<string>();
    this.ValidateStaticFieldsInternal(type, errors, prototypes);
    return errors;
  }

  private void ValidateStaticFieldsInternal(
    Type type,
    List<string> errors,
    Dictionary<Type, HashSet<string>> prototypes)
  {
    Type type1 = type;
    BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    for (; type1 != (Type) null; type1 = type1.BaseType)
    {
      foreach (FieldInfo field in type1.GetFields(bindingAttr))
        this.ValidateStaticField(field, type, errors, prototypes);
    }
  }

  private void ValidateStaticField(
    FieldInfo field,
    Type type,
    List<string> errors,
    Dictionary<Type, HashSet<string>> prototypes)
  {
    Type proto;
    if (!this.TryGetFieldPrototype(field, out proto))
      return;
    HashSet<string> stringSet;
    if (!prototypes.TryGetValue(proto, out stringSet))
    {
      errors.Add($"Prototype id field failed validation. Unknown prototype kind {proto.Name}. Field: {field.Name} in {type.FullName}");
    }
    else
    {
      string[] ids;
      if (!this.TryGetIds(field, out ids))
        return;
      foreach (string str in ids)
      {
        if (!stringSet.Contains(str))
          errors.Add($"Prototype id field failed validation. Unknown prototype: {str} of type {proto.Name}. Field: {field.Name} in {type.FullName}");
      }
    }
  }

  private bool TryGetIds(FieldInfo field, [NotNullWhen(true)] out string[]? ids)
  {
    ids = (string[]) null;
    object ids1 = field.GetValue((object) null);
    switch (ids1)
    {
      case null:
        return false;
      case string str:
        ids = new string[1]{ str };
        return true;
      case IEnumerable<string> source1:
        ids = source1.ToArray<string>();
        return true;
      case EntProtoId entProtoId:
        ids = new string[1]{ (string) entProtoId };
        return true;
      case IEnumerable<EntProtoId> source2:
        ids = source2.Select<EntProtoId, string>((Func<EntProtoId, string>) (x => x.Id)).ToArray<string>();
        return true;
      default:
        if (field.FieldType.IsGenericType)
        {
          Type genericTypeDefinition = field.FieldType.GetGenericTypeDefinition();
          if (genericTypeDefinition == typeof (ProtoId<>) || genericTypeDefinition == typeof (EntProtoId<>))
          {
            ids = new string[1]{ ids1.ToString() };
            return true;
          }
        }
        foreach (Type type1 in field.FieldType.GetInterfaces())
        {
          if (type1.IsGenericType && !(type1.GetGenericTypeDefinition() != typeof (IEnumerable<>)))
          {
            Type type2 = ((IEnumerable<Type>) type1.GetGenericArguments()).Single<Type>();
            if (type2.IsGenericType)
            {
              Type genericTypeDefinition = type2.GetGenericTypeDefinition();
              if (!(genericTypeDefinition != typeof (ProtoId<>)) || !(genericTypeDefinition != typeof (EntProtoId<>)))
              {
                ids = this.GetEnumerableIds((IEnumerable) ids1).ToArray<string>();
                return true;
              }
            }
          }
        }
        return false;
    }
  }

  private IEnumerable<string> GetEnumerableIds(IEnumerable ids)
  {
    foreach (object id in ids)
      yield return id.ToString();
  }

  private bool TryGetFieldPrototype(FieldInfo field, [NotNullWhen(true)] out Type? proto)
  {
    Attribute customAttribute = field.GetCustomAttribute(typeof (ValidatePrototypeIdAttribute<>), false);
    if (customAttribute != null)
    {
      proto = ((IEnumerable<Type>) customAttribute.GetType().GetGenericArguments()).First<Type>();
      return true;
    }
    if (this.TryGetPrototypeFromType(field.FieldType, out proto))
      return true;
    foreach (Type type in ((IEnumerable<Type>) field.FieldType.GetInterfaces()).Where<Type>((Func<Type, bool>) (x => x.IsGenericType)))
    {
      if (!(type.GetGenericTypeDefinition() != typeof (IEnumerable<>)) && this.TryGetPrototypeFromType(((IEnumerable<Type>) type.GetGenericArguments()).Single<Type>(), out proto))
        return true;
    }
    proto = (Type) null;
    return false;
  }

  private bool TryGetPrototypeFromType(Type type, [NotNullWhen(true)] out Type? proto)
  {
    if (type == typeof (EntProtoId))
    {
      proto = typeof (EntityPrototype);
      return true;
    }
    if (!type.IsGenericType)
    {
      proto = (Type) null;
      return false;
    }
    if (type.GetGenericTypeDefinition() == typeof (EntProtoId<>))
    {
      proto = typeof (EntityPrototype);
      return true;
    }
    if (type.GetGenericTypeDefinition() == typeof (ProtoId<>))
    {
      proto = ((IEnumerable<Type>) type.GetGenericArguments()).Single<Type>();
      return true;
    }
    proto = (Type) null;
    return false;
  }

  public event Action<DataNodeDocument>? LoadedData;

  public void LoadDirectory(
    ResPath path,
    bool overwrite = false,
    Dictionary<Type, HashSet<string>>? changed = null)
  {
    this._hasEverBeenReloaded = true;
    ResPath[] array = this.Resources.ContentFindFiles(new ResPath?(path)).Where<ResPath>((Func<ResPath, bool>) (filePath => filePath.Extension == "yml" && !filePath.Filename.StartsWith("."))).ToArray<ResPath>();
    new System.Random().Shuffle<ResPath>(array.AsSpan<ResPath>());
    ISawmill sawmill = this._logManager.GetSawmill("eng");
    foreach ((ResPath, IEnumerable<PrototypeManager.ExtractedMappingData>) tuple in ((IEnumerable<ResPath>) array).AsParallel<ResPath>().Select<ResPath, (ResPath, IEnumerable<PrototypeManager.ExtractedMappingData>)>((Func<ResPath, (ResPath, IEnumerable<PrototypeManager.ExtractedMappingData>)>) (file =>
    {
      try
      {
        bool flag = this.IsFileAbstract(file);
        using (StreamReader reader = this.ReadFile(file, !overwrite))
        {
          if (reader == null)
            return (file, (IEnumerable<PrototypeManager.ExtractedMappingData>) Array.Empty<PrototypeManager.ExtractedMappingData>());
          List<PrototypeManager.ExtractedMappingData> extractedMappingDataList = new List<PrototypeManager.ExtractedMappingData>();
          int num = 0;
          foreach (DataNodeDocument dataNodeDocument in DataNodeParser.ParseYamlStream((TextReader) reader, true))
          {
            ++num;
            Action<DataNodeDocument> loadedData = this.LoadedData;
            if (loadedData != null)
              loadedData(dataNodeDocument);
            switch (dataNodeDocument.Root)
            {
              case SequenceDataNode sequenceDataNode2:
                using (IEnumerator<DataNode> enumerator = sequenceDataNode2.Sequence.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    PrototypeManager.ExtractedMappingData mapping = this.ExtractMapping((MappingDataNode) enumerator.Current);
                    if (mapping != (PrototypeManager.ExtractedMappingData) null)
                    {
                      if (flag)
                        this.AbstractPrototype(mapping.Data);
                      extractedMappingDataList.Add(mapping);
                    }
                  }
                  continue;
                }
              case ValueDataNode valueDataNode2:
                if (valueDataNode2.Value == "")
                  continue;
                break;
            }
            sawmill.Error($"{file} document #{num} is not a sequence! Did you forget to indent your prototype with a '-'?");
          }
          return (file, (IEnumerable<PrototypeManager.ExtractedMappingData>) extractedMappingDataList);
        }
      }
      catch (Exception ex)
      {
        sawmill.Error($"Exception whilst loading prototypes from {file}:\n{ex}");
        return (file, (IEnumerable<PrototypeManager.ExtractedMappingData>) Array.Empty<PrototypeManager.ExtractedMappingData>());
      }
    })))
    {
      ResPath resPath = tuple.Item1;
      foreach (PrototypeManager.ExtractedMappingData mapping in tuple.Item2)
      {
        try
        {
          this.MergeMapping(mapping, overwrite, changed);
        }
        catch (Exception ex)
        {
          sawmill.Error($"Exception whilst loading prototypes from {resPath}:\n{ex}");
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
        return new StreamReader(this.Resources.ContentFileRead(file), EncodingHelpers.UTF8);
      }
      catch (IOException ex)
      {
        if (num > 10)
        {
          if (@throw)
            throw;
          this.Sawmill.Error($"Error reloading prototypes in file {file}:\n{ex}");
          return (StreamReader) null;
        }
        ++num;
        Thread.Sleep(10);
      }
    }
  }

  public void LoadFile(ResPath file, bool overwrite = false, Dictionary<Type, HashSet<string>>? changed = null)
  {
    try
    {
      bool flag = this.IsFileAbstract(file);
      using (StreamReader reader = this.ReadFile(file, !overwrite))
      {
        if (reader == null)
          return;
        int num = 0;
        foreach (DataNodeDocument dataNodeDocument in DataNodeParser.ParseYamlStream((TextReader) reader, true))
        {
          Action<DataNodeDocument> loadedData = this.LoadedData;
          if (loadedData != null)
            loadedData(dataNodeDocument);
          try
          {
            foreach (MappingDataNode dataNode in (IEnumerable<DataNode>) ((SequenceDataNode) dataNodeDocument.Root).Sequence)
            {
              PrototypeManager.ExtractedMappingData mapping = this.ExtractMapping(dataNode);
              if (!(mapping == (PrototypeManager.ExtractedMappingData) null))
              {
                if (flag)
                  this.AbstractPrototype(mapping.Data);
                this.MergeMapping(mapping, overwrite, changed);
              }
            }
          }
          catch (Exception ex)
          {
            this.Sawmill.Error($"Exception whilst loading prototypes from {file}#{num}:\n{ex}");
          }
          ++num;
        }
      }
    }
    catch (Exception ex)
    {
      this.Sawmill.Error("YamlException whilst loading prototypes from {0}: {1}", (object) file, (object) ex.Message);
    }
  }

  private PrototypeManager.ExtractedMappingData? ExtractMapping(MappingDataNode dataNode)
  {
    string key = dataNode.Get<ValueDataNode>("type").Value;
    if (this._ignoredPrototypeTypes.Contains(key))
      return (PrototypeManager.ExtractedMappingData) null;
    Type type;
    if (!this._kindNames.TryGetValue(key, out type))
      throw new PrototypeLoadException($"Unknown prototype type: '{key}'");
    PrototypeManager.KindData kindData = this._kinds[type];
    ValueDataNode node1;
    if (!dataNode.TryGet<ValueDataNode>("id", out node1))
      throw new PrototypeLoadException($"Prototype type {key} is missing an 'id' datafield.");
    string Id = node1.Value;
    string[] Parents = (string[]) null;
    DataNode node2;
    if (kindData.Inheritance != null && dataNode.TryGet("parent", out node2))
      Parents = this._serializationManager.Read<string[]>(node2, notNullableOverride: true);
    return new PrototypeManager.ExtractedMappingData(type, Id, Parents, dataNode);
  }

  private void MergeMapping(
    PrototypeManager.ExtractedMappingData mapping,
    bool overwrite,
    Dictionary<Type, HashSet<string>>? changed)
  {
    (Type type, string str, string[] Parents, MappingDataNode Data) = mapping;
    PrototypeManager.KindData kindData = this._kinds[type];
    if (!overwrite && kindData.RawResults.ContainsKey(str))
      throw new PrototypeLoadException($"Duplicate ID: '{str}' for kind '{type}");
    kindData.RawResults[str] = Data;
    MultiRootInheritanceGraph<string> inheritance = kindData.Inheritance;
    if (inheritance != null)
    {
      if (Parents != null)
        inheritance.Add(str, Parents);
      else
        inheritance.Add(str);
    }
    if (changed == null)
      return;
    changed.GetOrNew<Type, HashSet<string>>(type).Add(str);
  }

  public void LoadFromStream(
    TextReader stream,
    bool overwrite = false,
    Dictionary<Type, HashSet<string>>? changed = null)
  {
    this._hasEverBeenReloaded = true;
    int num = 0;
    foreach (DataNodeDocument dataNodeDocument in DataNodeParser.ParseYamlStream(stream, true))
    {
      Action<DataNodeDocument> loadedData = this.LoadedData;
      if (loadedData != null)
        loadedData(dataNodeDocument);
      try
      {
        foreach (MappingDataNode dataNode in ((IEnumerable) dataNodeDocument.Root).Cast<MappingDataNode>())
        {
          PrototypeManager.ExtractedMappingData mapping = this.ExtractMapping(dataNode);
          if (!(mapping == (PrototypeManager.ExtractedMappingData) null))
            this.MergeMapping(mapping, overwrite, changed);
        }
        ++num;
      }
      catch (Exception ex)
      {
        throw new PrototypeLoadException($"Failed to load prototypes from document#{num}", ex);
      }
    }
  }

  public void LoadString(string str, bool overwrite = false, Dictionary<Type, HashSet<string>>? changed = null)
  {
    this.LoadFromStream((TextReader) new StringReader(str), overwrite, changed);
  }

  public void RemoveString(string prototypes)
  {
    StringReader reader = new StringReader(prototypes);
    HashSet<PrototypeManager.KindData> kinds = new HashSet<PrototypeManager.KindData>();
    foreach (DataNodeDocument dataNodeDocument in DataNodeParser.ParseYamlStream((TextReader) reader))
    {
      foreach (MappingDataNode mappingDataNode in ((IEnumerable) dataNodeDocument.Root).Cast<MappingDataNode>())
      {
        Type key;
        if (this._kindNames.TryGetValue(mappingDataNode.Get<ValueDataNode>("type").Value, out key))
        {
          PrototypeManager.KindData kindData1 = this._kinds[key];
          string str = mappingDataNode.Get<ValueDataNode>("id").Value;
          kindData1.Inheritance?.Remove(str, true);
          PrototypeManager.KindData kindData2 = kindData1;
          if (kindData2.UnfrozenInstances == null)
            kindData2.UnfrozenInstances = kindData1.Instances.ToDictionary<string, IPrototype>();
          kindData1.UnfrozenInstances.Remove(str);
          kindData1.Results.Remove(str);
          kindData1.RawResults.Remove(str);
          kinds.Add(kindData1);
        }
      }
    }
    this.Freeze((IEnumerable<PrototypeManager.KindData>) kinds);
  }

  public void AbstractFile(ResPath path) => this._abstractFiles.Add(path);

  public void AbstractDirectory(ResPath path) => this._abstractDirectories.Add(path);

  private bool IsFileAbstract(ResPath file)
  {
    ResPath? relative;
    if (this._abstractFiles.Count > 0)
    {
      foreach (ResPath abstractFile in this._abstractFiles)
      {
        if (file.TryRelativeTo(abstractFile, out relative))
          return true;
      }
    }
    if (this._abstractDirectories.Count > 0)
    {
      foreach (ResPath abstractDirectory in this._abstractDirectories)
      {
        if (file.TryRelativeTo(abstractDirectory, out relative))
          return true;
      }
    }
    return false;
  }

  private void AbstractPrototype(MappingDataNode mapping)
  {
    DataNode node;
    if (mapping.TryGet("abstract", out node))
    {
      if (!(node is ValueDataNode valueDataNode))
        mapping["abstract"] = (DataNode) new ValueDataNode("true");
      else
        valueDataNode.Value = "true";
    }
    else
      mapping.Add("abstract", "true");
  }

  public Dictionary<string, HashSet<ErrorNode>> ValidateDirectory(ResPath path)
  {
    return this.ValidateDirectory(path, out Dictionary<Type, HashSet<string>> _);
  }

  public Dictionary<string, HashSet<ErrorNode>> ValidateDirectory(
    ResPath path,
    out Dictionary<Type, HashSet<string>> protos)
  {
    ParallelQuery<ResPath> parallelQuery = this.Resources.ContentFindFiles(new ResPath?(path)).ToList<ResPath>().AsParallel<ResPath>().Where<ResPath>((Func<ResPath, bool>) (filePath => filePath.Extension == "yml" && !filePath.Filename.StartsWith(".")));
    Dictionary<string, HashSet<ErrorNode>> dict1 = new Dictionary<string, HashSet<ErrorNode>>();
    Dictionary<Type, Dictionary<string, PrototypeManager.PrototypeValidationData>> dict2 = new Dictionary<Type, Dictionary<string, PrototypeManager.PrototypeValidationData>>();
    foreach (ResPath file in parallelQuery)
    {
      using (StreamReader streamReader = this.ReadFile(file))
      {
        if (streamReader != null)
        {
          YamlStream yamlStream = new YamlStream();
          try
          {
            yamlStream.Load((TextReader) streamReader);
          }
          catch (Exception ex)
          {
            throw new PrototypeLoadException($"Error loading file: '{file}'\n{ex}");
          }
          foreach (YamlDocument document in (IEnumerable<YamlDocument>) yamlStream.Documents)
          {
            foreach (YamlMappingNode yamlMappingNode in ((IEnumerable) document.RootNode).Cast<YamlMappingNode>())
            {
              string key1 = yamlMappingNode.GetNode("type").AsString();
              if (!this._ignoredPrototypeTypes.Contains(key1))
              {
                Type key2;
                if (!this._kindNames.TryGetValue(key1, out key2))
                  throw new PrototypeLoadException($"Unknown prototype type: '{key1}'");
                MappingDataNode dataNodeCast = ((YamlNode) yamlMappingNode).ToDataNodeCast<MappingDataNode>();
                string id = dataNodeCast.Get<ValueDataNode>("id").Value;
                PrototypeManager.PrototypeValidationData prototypeValidationData = new PrototypeManager.PrototypeValidationData(id, dataNodeCast, file.ToString());
                dataNodeCast.Remove("type");
                char? element;
                if (((IEnumerable<char>) PrototypeManager.DisallowedIdChars).TryFirstOrNull<char>((Func<char, bool>) (c => id.Contains(c)), out element))
                  dict1.GetOrNew<string, HashSet<ErrorNode>>(prototypeValidationData.File).Add(new ErrorNode((DataNode) dataNodeCast, $"Prototype '{id}' ({key2}) contains disallowed character '{element}'."));
                if (!dict2.GetOrNew<Type, Dictionary<string, PrototypeManager.PrototypeValidationData>>(key2).TryAdd(id, prototypeValidationData))
                {
                  ErrorNode errorNode = new ErrorNode((DataNode) dataNodeCast, $"Found dupe prototype ID of {id} for {key2}");
                  dict1.GetOrNew<string, HashSet<ErrorNode>>(prototypeValidationData.File).Add(errorNode);
                }
              }
            }
          }
        }
      }
    }
    YamlValidationContext context = new YamlValidationContext();
    List<ErrorNode> other = new List<ErrorNode>();
    foreach ((Type key5, Dictionary<string, PrototypeManager.PrototypeValidationData> dictionary1) in dict2)
    {
      Type type = key5;
      Dictionary<string, PrototypeManager.PrototypeValidationData> prototypes = dictionary1;
      foreach ((string key4, PrototypeManager.PrototypeValidationData data) in prototypes)
      {
        other.Clear();
        this.EnsurePushed(data, prototypes, type);
        ValueDataNode node1;
        if (!data.Mapping.TryGet<ValueDataNode>("abstract", out node1) || !bool.Parse(node1.Value))
        {
          other.AddRange(this._serializationManager.ValidateNode(type, (DataNode) data.Mapping).GetErrors());
          if (other.Count > 0)
            dict1.GetOrNew<string, HashSet<ErrorNode>>(data.File).UnionWith((IEnumerable<ErrorNode>) other);
          try
          {
            object obj = this._serializationManager.Read(type, (DataNode) data.Mapping, (ISerializationContext) context);
            DataNode node2 = this._serializationManager.WriteValue(type, obj, true, (ISerializationContext) context);
            other.AddRange(this._serializationManager.ValidateNode(type, node2, (ISerializationContext) context).GetErrors());
            if (other.Count > 0)
              dict1.GetOrNew<string, HashSet<ErrorNode>>(data.File).UnionWith((IEnumerable<ErrorNode>) other);
          }
          catch (Exception ex)
          {
            other.Add(new ErrorNode((DataNode) new ValueDataNode(), $"Caught Exception while validating {type} prototype {key4}. Exception: {ex}"));
          }
        }
      }
    }
    protos = new Dictionary<Type, HashSet<string>>(dict2.Count);
    foreach ((key5, dictionary1) in dict2)
    {
      Type key6 = key5;
      Dictionary<string, PrototypeManager.PrototypeValidationData> dictionary2 = dictionary1;
      protos[key6] = dictionary2.Keys.ToHashSet<string>();
    }
    return dict1;
  }

  public Dictionary<Type, Dictionary<string, HashSet<ErrorNode>>> ValidateAllPrototypesSerializable(
    ISerializationContext? ctx)
  {
    Dictionary<Type, Dictionary<string, HashSet<ErrorNode>>> dictionary = new Dictionary<Type, Dictionary<string, HashSet<ErrorNode>>>();
    Dictionary<string, HashSet<ErrorNode>> dict = new Dictionary<string, HashSet<ErrorNode>>();
    foreach ((Type type, PrototypeManager.KindData kindData) in this._kinds)
    {
      foreach (IPrototype instance in kindData.Instances.Values)
      {
        bool caughtException;
        HashSet<ErrorNode> other = this.ValidateProto(type, instance, ctx, out caughtException);
        if (other.Count > 0)
          dict.GetOrNew<string, HashSet<ErrorNode>>(instance.ID).UnionWith((IEnumerable<ErrorNode>) other);
        if (caughtException)
          break;
      }
      if (dict.Count > 0)
      {
        dictionary[type] = dict;
        dict = new Dictionary<string, HashSet<ErrorNode>>();
      }
    }
    return dictionary;
  }

  private HashSet<ErrorNode> ValidateProto(
    Type type,
    IPrototype instance,
    ISerializationContext? ctx,
    out bool caughtException)
  {
    caughtException = false;
    DataNode node;
    try
    {
      node = this._serializationManager.WriteValue(type, (object) instance, true, ctx);
    }
    catch (Exception ex)
    {
      caughtException = true;
      return new HashSet<ErrorNode>()
      {
        new ErrorNode((DataNode) new ValueDataNode(""), $"Caught exception while writing. Exception: {ex}")
      };
    }
    try
    {
      return this._serializationManager.ValidateNode(type, node, ctx).GetErrors().ToHashSet<ErrorNode>();
    }
    catch (Exception ex)
    {
      caughtException = true;
      return new HashSet<ErrorNode>()
      {
        new ErrorNode((DataNode) new ValueDataNode(""), $"Caught exception while validating. Exception: {ex}")
      };
    }
  }

  private void EnsurePushed(
    PrototypeManager.PrototypeValidationData data,
    Dictionary<string, PrototypeManager.PrototypeValidationData> prototypes,
    Type type)
  {
    if (data.Pushed)
      return;
    data.Pushed = true;
    DataNode node;
    if (!data.Mapping.TryGet("parent", out node))
      return;
    data.Parents = this._serializationManager.Read<string[]>(node, notNullableOverride: true);
    data.ParentMappings = new MappingDataNode[data.Parents.Length];
    int num = 0;
    foreach (string parent in data.Parents)
    {
      PrototypeManager.PrototypeValidationData prototype = prototypes[parent];
      this.EnsurePushed(prototype, prototypes, type);
      data.ParentMappings[num++] = prototype.Mapping;
    }
    data.Mapping = this._serializationManager.PushCompositionWithGenericNode<MappingDataNode>(type, data.ParentMappings, data.Mapping);
  }

  private sealed class InheritancePushDatum
  {
    public MappingDataNode Result;
    public int CountParentsRemaining;

    public InheritancePushDatum(MappingDataNode result, int countParentsRemaining)
    {
      this.Result = result;
      this.CountParentsRemaining = countParentsRemaining;
    }
  }

  private sealed class KindData
  {
    public Dictionary<string, IPrototype>? UnfrozenInstances;
    public FrozenDictionary<string, IPrototype> Instances;
    public Dictionary<string, MappingDataNode> Results;
    public readonly Dictionary<string, MappingDataNode> RawResults;
    public readonly Type Type;
    public readonly string Name;
    public MultiRootInheritanceGraph<string>? Inheritance;
    public object InstancesDirect;
    private MethodInfo _freezeDirectInfo;

    public KindData(Type kind, string name)
    {
      this.Type = kind;
      this.Name = name;
      this._freezeDirectInfo = typeof (PrototypeManager.KindData).GetMethod("FreezeDirect", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(kind);
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    private void FreezeDirect<T>()
    {
      Dictionary<string, T> source = new Dictionary<string, T>();
      foreach ((string key, IPrototype prototype) in this.Instances)
        source.Add(key, (T) prototype);
      this.InstancesDirect = (object) source.ToFrozenDictionary<string, T>();
    }

    public void Freeze()
    {
      Dictionary<string, IPrototype> unfrozenInstances = this.UnfrozenInstances;
      this.Instances = (unfrozenInstances != null ? unfrozenInstances.ToFrozenDictionary<string, IPrototype>() : (FrozenDictionary<string, IPrototype>) null) ?? FrozenDictionary<string, IPrototype>.Empty;
      this.UnfrozenInstances = (Dictionary<string, IPrototype>) null;
      this._freezeDirectInfo.Invoke((object) this, (object[]) null);
    }
  }

  private sealed record ExtractedMappingData(
    Type Kind,
    string Id,
    string[]? Parents,
    MappingDataNode Data)
  ;

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
      this.Id = id;
      this.File = file;
      this.Mapping = mapping;
    }
  }
}
