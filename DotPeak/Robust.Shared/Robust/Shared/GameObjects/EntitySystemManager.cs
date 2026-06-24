// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntitySystemManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Prometheus;
using Robust.Shared.Exceptions;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Profiling;
using Robust.Shared.Reflection;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

public sealed class EntitySystemManager : IEntitySystemManager, IPostInjectInit
{
  [Robust.Shared.IoC.Dependency]
  private readonly IReflectionManager _reflectionManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IEntityManager _entityManager;
  [Robust.Shared.IoC.Dependency]
  private readonly ProfManager _profManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IDependencyCollection _dependencyCollection;
  [Robust.Shared.IoC.Dependency]
  private readonly ILogManager _logManager;
  [Robust.Shared.IoC.Dependency]
  private readonly IRuntimeLog _runtimeLog;
  private ISawmill _sawmill;
  internal Robust.Shared.IoC.DependencyCollection SystemDependencyCollection;
  private readonly List<Type> _systemTypes = new List<Type>();
  private static readonly Histogram _tickUsageHistogram;
  [Robust.Shared.ViewVariables.ViewVariables]
  private readonly List<Type> _extraLoadedTypes = new List<Type>();
  private readonly Stopwatch _stopwatch = new Stopwatch();
  private bool _initialized;
  [Robust.Shared.ViewVariables.ViewVariables]
  private EntitySystemManager.UpdateReg[] _updateOrder = Array.Empty<EntitySystemManager.UpdateReg>();
  [Robust.Shared.ViewVariables.ViewVariables]
  private IEntitySystem[] _frameUpdateOrder = Array.Empty<IEntitySystem>();

  public IDependencyCollection DependencyCollection
  {
    get
    {
      if (this._initialized)
        return (IDependencyCollection) this.SystemDependencyCollection;
      throw new InvalidOperationException("EntitySystemManager has not been initialized.");
    }
  }

  public bool MetricsEnabled { get; set; }

  public event EventHandler<SystemChangedArgs>? SystemLoaded;

  public event EventHandler<SystemChangedArgs>? SystemUnloaded;

  public T GetEntitySystem<T>() where T : IEntitySystem
  {
    return this.SystemDependencyCollection.Resolve<T>();
  }

  public T? GetEntitySystemOrNull<T>() where T : IEntitySystem
  {
    T instance;
    this.SystemDependencyCollection.TryResolveType<T>(out instance);
    return instance;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Resolve<T>([NotNull] ref T? instance) where T : IEntitySystem
  {
    this.SystemDependencyCollection.Resolve<T>(ref instance);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Resolve<T1, T2>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2)
    where T1 : IEntitySystem
    where T2 : IEntitySystem
  {
    this.SystemDependencyCollection.Resolve<T1, T2>(ref instance1, ref instance2);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Resolve<T1, T2, T3>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3)
    where T1 : IEntitySystem
    where T2 : IEntitySystem
    where T3 : IEntitySystem
  {
    this.SystemDependencyCollection.Resolve<T1, T2, T3>(ref instance1, ref instance2, ref instance3);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Resolve<T1, T2, T3, T4>(
    [NotNull] ref T1? instance1,
    [NotNull] ref T2? instance2,
    [NotNull] ref T3? instance3,
    [NotNull] ref T4? instance4)
    where T1 : IEntitySystem
    where T2 : IEntitySystem
    where T3 : IEntitySystem
    where T4 : IEntitySystem
  {
    this.SystemDependencyCollection.Resolve<T1, T2, T3, T4>(ref instance1, ref instance2, ref instance3, ref instance4);
  }

  public bool TryGetEntitySystem<T>([NotNullWhen(true)] out T? entitySystem) where T : IEntitySystem
  {
    return this.SystemDependencyCollection.TryResolveType<T>(out entitySystem);
  }

  public void Initialize(bool discover = true)
  {
    if (this._initialized)
      return;
    HashSet<Type> typeSet = new HashSet<Type>();
    this.SystemDependencyCollection = new Robust.Shared.IoC.DependencyCollection(this._dependencyCollection);
    Dictionary<Type, Type> subTypes = new Dictionary<Type, Type>();
    this._systemTypes.Clear();
    foreach (Type type in !discover ? (IEnumerable<Type>) this._extraLoadedTypes : this._reflectionManager.GetAllChildren<IEntitySystem>().Concat<Type>((IEnumerable<Type>) this._extraLoadedTypes))
    {
      this._sawmill.Debug("Initializing entity system {0}", (object) type);
      this.SystemDependencyCollection.Register(type, (DependencyFactoryDelegate<object>) null, false);
      this._systemTypes.Add(type);
      typeSet.Add(type);
      if (subTypes.ContainsKey(type))
        subTypes.Remove(type);
      foreach (Type baseType in EntitySystemManager.GetBaseTypes(type))
      {
        if (!typeSet.Contains(baseType))
        {
          if (subTypes.ContainsKey(baseType))
          {
            subTypes.Remove(baseType);
            typeSet.Add(baseType);
          }
          else
            subTypes.Add(baseType, type);
        }
      }
    }
    foreach ((Type type, Type implementation) in subTypes)
    {
      this.SystemDependencyCollection.Register(type, implementation, (DependencyFactoryDelegate<object>) null, true);
      this._systemTypes.Remove(type);
    }
    this.SystemDependencyCollection.BuildGraph();
    foreach (Type systemType in this._systemTypes)
    {
      IEntitySystem system = (IEntitySystem) this.SystemDependencyCollection.ResolveType(systemType);
      system.Initialize();
      EventHandler<SystemChangedArgs> systemLoaded = this.SystemLoaded;
      if (systemLoaded != null)
        systemLoaded((object) this, new SystemChangedArgs(system));
    }
    (IEnumerable<IEntitySystem> entitySystems1, IEnumerable<IEntitySystem> entitySystems2) = EntitySystemManager.CalculateUpdateOrder(this._systemTypes, subTypes, this.SystemDependencyCollection);
    this._frameUpdateOrder = entitySystems1.ToArray<IEntitySystem>();
    this._updateOrder = entitySystems2.Select<IEntitySystem, EntitySystemManager.UpdateReg>((Func<IEntitySystem, EntitySystemManager.UpdateReg>) (s => new EntitySystemManager.UpdateReg()
    {
      System = s,
      Monitor = ((Collector<Histogram.Child>) EntitySystemManager._tickUsageHistogram).WithLabels(new string[1]
      {
        s.GetType().Name
      })
    })).ToArray<EntitySystemManager.UpdateReg>();
    this._initialized = true;
  }

  private static (IEnumerable<IEntitySystem> frameUpd, IEnumerable<IEntitySystem> upd) CalculateUpdateOrder(
    List<Type> systemTypes,
    Dictionary<Type, Type> subTypes,
    Robust.Shared.IoC.DependencyCollection dependencyCollection)
  {
    List<TopologicalSort.GraphNode<IEntitySystem>> nodes = new List<TopologicalSort.GraphNode<IEntitySystem>>();
    Dictionary<Type, TopologicalSort.GraphNode<IEntitySystem>> dictionary = new Dictionary<Type, TopologicalSort.GraphNode<IEntitySystem>>();
    foreach (Type systemType in systemTypes)
    {
      TopologicalSort.GraphNode<IEntitySystem> graphNode = new TopologicalSort.GraphNode<IEntitySystem>((IEntitySystem) dependencyCollection.ResolveType(systemType));
      dictionary.Add(systemType, graphNode);
      nodes.Add(graphNode);
    }
    foreach ((Type key1, Type key2) in subTypes)
    {
      TopologicalSort.GraphNode<IEntitySystem> graphNode = dictionary[key2];
      dictionary[key1] = graphNode;
    }
    foreach (TopologicalSort.GraphNode<IEntitySystem> graphNode1 in dictionary.Values)
    {
      foreach (Type key in graphNode1.Value.UpdatesAfter)
        dictionary[key].Dependant.Add(graphNode1);
      foreach (Type key in graphNode1.Value.UpdatesBefore)
      {
        TopologicalSort.GraphNode<IEntitySystem> graphNode2 = dictionary[key];
        graphNode1.Dependant.Add(graphNode2);
      }
    }
    IEntitySystem[] array = TopologicalSort.Sort<IEntitySystem>((IEnumerable<TopologicalSort.GraphNode<IEntitySystem>>) nodes).ToArray<IEntitySystem>();
    return (((IEnumerable<IEntitySystem>) array).Where<IEntitySystem>((Func<IEntitySystem, bool>) (p => EntitySystemManager.NeedsFrameUpdate(p.GetType()))), ((IEnumerable<IEntitySystem>) array).Where<IEntitySystem>((Func<IEntitySystem, bool>) (p => EntitySystemManager.NeedsUpdate(p.GetType()))));
  }

  private static IEnumerable<Type> GetBaseTypes(Type type)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return type.BaseType == (Type) null ? (IEnumerable<Type>) type.GetInterfaces() : Enumerable.Repeat<Type>(type.BaseType, 1).Concat<Type>((IEnumerable<Type>) type.GetInterfaces()).Concat<Type>(((IEnumerable<Type>) type.GetInterfaces()).SelectMany<Type, Type>(EntitySystemManager.\u003C\u003EO.\u003C0\u003E__GetBaseTypes ?? (EntitySystemManager.\u003C\u003EO.\u003C0\u003E__GetBaseTypes = new Func<Type, IEnumerable<Type>>(EntitySystemManager.GetBaseTypes)))).Concat<Type>(EntitySystemManager.GetBaseTypes(type.BaseType));
  }

  public void Shutdown()
  {
    foreach (Type systemType in this._systemTypes)
    {
      if (this.SystemDependencyCollection != null)
      {
        IEntitySystem entitySystem = (IEntitySystem) this.SystemDependencyCollection.ResolveType(systemType);
        EventHandler<SystemChangedArgs> systemUnloaded = this.SystemUnloaded;
        if (systemUnloaded != null)
          systemUnloaded((object) this, new SystemChangedArgs(entitySystem));
        entitySystem.Shutdown();
        this._entityManager.EventBus.UnsubscribeEvents((IEntityEventSubscriber) entitySystem);
      }
    }
    this.Clear();
  }

  public void Clear()
  {
    this._extraLoadedTypes.Clear();
    this._systemTypes.Clear();
    this._updateOrder = Array.Empty<EntitySystemManager.UpdateReg>();
    this._frameUpdateOrder = Array.Empty<IEntitySystem>();
    this._initialized = false;
    this.SystemDependencyCollection?.Clear();
  }

  public void TickUpdate(float frameTime, bool noPredictions)
  {
    foreach (EntitySystemManager.UpdateReg updateReg in this._updateOrder)
    {
      if (!noPredictions || updateReg.System.UpdatesOutsidePrediction)
      {
        if (this.MetricsEnabled)
          this._stopwatch.Restart();
        try
        {
          using (this._profManager.Value(updateReg.System.GetType().Name))
            updateReg.System.Update(frameTime);
        }
        catch (Exception ex)
        {
          this._runtimeLog.LogException(ex, "entsys");
        }
        if (this.MetricsEnabled)
          updateReg.Monitor.Observe(this._stopwatch.Elapsed.TotalSeconds);
      }
    }
  }

  public void FrameUpdate(float frameTime)
  {
    foreach (IEntitySystem entitySystem in this._frameUpdateOrder)
    {
      try
      {
        using (this._profManager.Value(entitySystem.GetType().Name))
          entitySystem.FrameUpdate(frameTime);
      }
      catch (Exception ex)
      {
        this._runtimeLog.LogException(ex, "entsys");
      }
    }
  }

  public void LoadExtraSystemType<T>() where T : IEntitySystem, new()
  {
    if (this._initialized)
      throw new InvalidOperationException("Cannot use LoadExtraSystemType when the entity system manager is initialized.");
    this._extraLoadedTypes.Add(typeof (T));
  }

  public IEnumerable<Type> GetEntitySystemTypes() => (IEnumerable<Type>) this._systemTypes;

  public bool TryGetEntitySystem(Type sysType, [NotNullWhen(true)] out object? system)
  {
    return this.SystemDependencyCollection.TryResolveType(sysType, out system);
  }

  public object GetEntitySystem(Type sysType)
  {
    return this.SystemDependencyCollection.ResolveType(sysType);
  }

  private static bool NeedsUpdate(Type type)
  {
    if (!typeof (EntitySystem).IsAssignableFrom(type))
      return true;
    return type.GetMethod("Update", new Type[1]
    {
      typeof (float)
    }).DeclaringType != typeof (EntitySystem);
  }

  private static bool NeedsFrameUpdate(Type type)
  {
    if (!typeof (EntitySystem).IsAssignableFrom(type))
      return true;
    return type.GetMethod("FrameUpdate", new Type[1]
    {
      typeof (float)
    }).DeclaringType != typeof (EntitySystem);
  }

  internal IEnumerable<Type> FrameUpdateOrder
  {
    get
    {
      return ((IEnumerable<IEntitySystem>) this._frameUpdateOrder).Select<IEntitySystem, Type>((Func<IEntitySystem, Type>) (c => c.GetType()));
    }
  }

  internal IEnumerable<Type> TickUpdateOrder
  {
    get
    {
      return ((IEnumerable<EntitySystemManager.UpdateReg>) this._updateOrder).Select<EntitySystemManager.UpdateReg, Type>((Func<EntitySystemManager.UpdateReg, Type>) (c => c.System.GetType()));
    }
  }

  void IPostInjectInit.PostInject() => this._sawmill = this._logManager.GetSawmill("go.sys");

  static EntitySystemManager()
  {
    HistogramConfiguration histogramConfiguration = new HistogramConfiguration();
    ((MetricConfiguration) histogramConfiguration).LabelNames = new string[1]
    {
      "system"
    };
    histogramConfiguration.Buckets = Histogram.ExponentialBuckets(1E-06, 1.5, 25);
    EntitySystemManager._tickUsageHistogram = Metrics.CreateHistogram("robust_entity_systems_update_usage", "Amount of time spent processing each entity system", histogramConfiguration);
  }

  private struct UpdateReg
  {
    [Robust.Shared.ViewVariables.ViewVariables]
    public IEntitySystem System;
    [Robust.Shared.ViewVariables.ViewVariables]
    public Histogram.Child Monitor;

    public override string? ToString() => this.System.ToString();
  }
}
