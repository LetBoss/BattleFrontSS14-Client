using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Prometheus;
using Robust.Shared.Exceptions;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Profiling;
using Robust.Shared.Reflection;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameObjects;

public sealed class EntitySystemManager : IEntitySystemManager, IPostInjectInit
{
	private struct UpdateReg
	{
		[ViewVariables]
		public IEntitySystem System;

		[ViewVariables]
		public Child Monitor;

		public override string? ToString()
		{
			return System.ToString();
		}
	}

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

	internal DependencyCollection SystemDependencyCollection;

	private readonly List<Type> _systemTypes = new List<Type>();

	private static readonly Histogram _tickUsageHistogram;

	[ViewVariables]
	private readonly List<Type> _extraLoadedTypes = new List<Type>();

	private readonly Stopwatch _stopwatch = new Stopwatch();

	private bool _initialized;

	[ViewVariables]
	private UpdateReg[] _updateOrder = Array.Empty<UpdateReg>();

	[ViewVariables]
	private IEntitySystem[] _frameUpdateOrder = Array.Empty<IEntitySystem>();

	public IDependencyCollection DependencyCollection
	{
		get
		{
			if (_initialized)
			{
				return SystemDependencyCollection;
			}
			throw new InvalidOperationException("EntitySystemManager has not been initialized.");
		}
	}

	public bool MetricsEnabled { get; set; }

	internal IEnumerable<Type> FrameUpdateOrder => _frameUpdateOrder.Select((IEntitySystem c) => c.GetType());

	internal IEnumerable<Type> TickUpdateOrder => _updateOrder.Select((UpdateReg c) => c.System.GetType());

	public event EventHandler<SystemChangedArgs>? SystemLoaded;

	public event EventHandler<SystemChangedArgs>? SystemUnloaded;

	public T GetEntitySystem<T>() where T : IEntitySystem
	{
		return SystemDependencyCollection.Resolve<T>();
	}

	public T? GetEntitySystemOrNull<T>() where T : IEntitySystem
	{
		SystemDependencyCollection.TryResolveType<T>(out T instance);
		return instance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Resolve<T>([NotNull] ref T? instance) where T : IEntitySystem
	{
		SystemDependencyCollection.Resolve(ref instance);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Resolve<T1, T2>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2) where T1 : IEntitySystem where T2 : IEntitySystem
	{
		SystemDependencyCollection.Resolve(ref instance1, ref instance2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Resolve<T1, T2, T3>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3) where T1 : IEntitySystem where T2 : IEntitySystem where T3 : IEntitySystem
	{
		SystemDependencyCollection.Resolve(ref instance1, ref instance2, ref instance3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Resolve<T1, T2, T3, T4>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3, [NotNull] ref T4? instance4) where T1 : IEntitySystem where T2 : IEntitySystem where T3 : IEntitySystem where T4 : IEntitySystem
	{
		SystemDependencyCollection.Resolve(ref instance1, ref instance2, ref instance3, ref instance4);
	}

	public bool TryGetEntitySystem<T>([NotNullWhen(true)] out T? entitySystem) where T : IEntitySystem
	{
		return SystemDependencyCollection.TryResolveType<T>(out entitySystem);
	}

	public void Initialize(bool discover = true)
	{
		if (_initialized)
		{
			return;
		}
		HashSet<Type> hashSet = new HashSet<Type>();
		SystemDependencyCollection = new DependencyCollection(_dependencyCollection);
		Dictionary<Type, Type> dictionary = new Dictionary<Type, Type>();
		_systemTypes.Clear();
		IEnumerable<Type> enumerable = ((!discover) ? _extraLoadedTypes : _reflectionManager.GetAllChildren<IEntitySystem>().Concat(_extraLoadedTypes));
		foreach (Type item3 in enumerable)
		{
			_sawmill.Debug("Initializing entity system {0}", item3);
			SystemDependencyCollection.Register(item3);
			_systemTypes.Add(item3);
			hashSet.Add(item3);
			if (dictionary.ContainsKey(item3))
			{
				dictionary.Remove(item3);
			}
			foreach (Type baseType in GetBaseTypes(item3))
			{
				if (!hashSet.Contains(baseType))
				{
					if (dictionary.ContainsKey(baseType))
					{
						dictionary.Remove(baseType);
						hashSet.Add(baseType);
					}
					else
					{
						dictionary.Add(baseType, item3);
					}
				}
			}
		}
		foreach (var (type3, implementation) in dictionary)
		{
			SystemDependencyCollection.Register(type3, implementation, null, overwrite: true);
			_systemTypes.Remove(type3);
		}
		SystemDependencyCollection.BuildGraph();
		foreach (Type systemType in _systemTypes)
		{
			IEntitySystem entitySystem = (IEntitySystem)SystemDependencyCollection.ResolveType(systemType);
			entitySystem.Initialize();
			this.SystemLoaded?.Invoke(this, new SystemChangedArgs(entitySystem));
		}
		(IEnumerable<IEntitySystem> frameUpd, IEnumerable<IEntitySystem> upd) tuple = CalculateUpdateOrder(_systemTypes, dictionary, SystemDependencyCollection);
		IEnumerable<IEntitySystem> item = tuple.frameUpd;
		IEnumerable<IEntitySystem> item2 = tuple.upd;
		_frameUpdateOrder = item.ToArray();
		_updateOrder = item2.Select((IEntitySystem s) => new UpdateReg
		{
			System = s,
			Monitor = ((Collector<Child>)(object)_tickUsageHistogram).WithLabels(new string[1] { s.GetType().Name })
		}).ToArray();
		_initialized = true;
	}

	private static (IEnumerable<IEntitySystem> frameUpd, IEnumerable<IEntitySystem> upd) CalculateUpdateOrder(List<Type> systemTypes, Dictionary<Type, Type> subTypes, DependencyCollection dependencyCollection)
	{
		List<TopologicalSort.GraphNode<IEntitySystem>> list = new List<TopologicalSort.GraphNode<IEntitySystem>>();
		Dictionary<Type, TopologicalSort.GraphNode<IEntitySystem>> dictionary = new Dictionary<Type, TopologicalSort.GraphNode<IEntitySystem>>();
		foreach (Type systemType in systemTypes)
		{
			TopologicalSort.GraphNode<IEntitySystem> graphNode = new TopologicalSort.GraphNode<IEntitySystem>((IEntitySystem)dependencyCollection.ResolveType(systemType));
			dictionary.Add(systemType, graphNode);
			list.Add(graphNode);
		}
		foreach (KeyValuePair<Type, Type> subType in subTypes)
		{
			subType.Deconstruct(out var key, out var value);
			Type key2 = key;
			Type key3 = value;
			TopologicalSort.GraphNode<IEntitySystem> value2 = dictionary[key3];
			dictionary[key2] = value2;
		}
		foreach (TopologicalSort.GraphNode<IEntitySystem> value3 in dictionary.Values)
		{
			foreach (Type item4 in value3.Value.UpdatesAfter)
			{
				dictionary[item4].Dependant.Add(value3);
			}
			foreach (Type item5 in value3.Value.UpdatesBefore)
			{
				TopologicalSort.GraphNode<IEntitySystem> item = dictionary[item5];
				value3.Dependant.Add(item);
			}
		}
		IEntitySystem[] source = TopologicalSort.Sort(list).ToArray();
		IEnumerable<IEntitySystem> item2 = source.Where((IEntitySystem p) => NeedsFrameUpdate(p.GetType()));
		IEnumerable<IEntitySystem> item3 = source.Where((IEntitySystem p) => NeedsUpdate(p.GetType()));
		return (frameUpd: item2, upd: item3);
	}

	private static IEnumerable<Type> GetBaseTypes(Type type)
	{
		if (type.BaseType == null)
		{
			return type.GetInterfaces();
		}
		return Enumerable.Repeat(type.BaseType, 1).Concat<Type>(type.GetInterfaces()).Concat(type.GetInterfaces().SelectMany(GetBaseTypes))
			.Concat(GetBaseTypes(type.BaseType));
	}

	public void Shutdown()
	{
		foreach (Type systemType in _systemTypes)
		{
			if (SystemDependencyCollection != null)
			{
				IEntitySystem entitySystem = (IEntitySystem)SystemDependencyCollection.ResolveType(systemType);
				this.SystemUnloaded?.Invoke(this, new SystemChangedArgs(entitySystem));
				entitySystem.Shutdown();
				_entityManager.EventBus.UnsubscribeEvents(entitySystem);
			}
		}
		Clear();
	}

	public void Clear()
	{
		_extraLoadedTypes.Clear();
		_systemTypes.Clear();
		_updateOrder = Array.Empty<UpdateReg>();
		_frameUpdateOrder = Array.Empty<IEntitySystem>();
		_initialized = false;
		SystemDependencyCollection?.Clear();
	}

	public void TickUpdate(float frameTime, bool noPredictions)
	{
		UpdateReg[] updateOrder = _updateOrder;
		for (int i = 0; i < updateOrder.Length; i++)
		{
			UpdateReg updateReg = updateOrder[i];
			if (noPredictions && !updateReg.System.UpdatesOutsidePrediction)
			{
				continue;
			}
			if (MetricsEnabled)
			{
				_stopwatch.Restart();
			}
			try
			{
				using (_profManager.Value(updateReg.System.GetType().Name))
				{
					updateReg.System.Update(frameTime);
				}
			}
			catch (Exception exception)
			{
				_runtimeLog.LogException(exception, "entsys");
			}
			if (MetricsEnabled)
			{
				updateReg.Monitor.Observe(_stopwatch.Elapsed.TotalSeconds);
			}
		}
	}

	public void FrameUpdate(float frameTime)
	{
		IEntitySystem[] frameUpdateOrder = _frameUpdateOrder;
		foreach (IEntitySystem entitySystem in frameUpdateOrder)
		{
			try
			{
				using (_profManager.Value(entitySystem.GetType().Name))
				{
					entitySystem.FrameUpdate(frameTime);
				}
			}
			catch (Exception exception)
			{
				_runtimeLog.LogException(exception, "entsys");
			}
		}
	}

	public void LoadExtraSystemType<T>() where T : IEntitySystem, new()
	{
		if (_initialized)
		{
			throw new InvalidOperationException("Cannot use LoadExtraSystemType when the entity system manager is initialized.");
		}
		_extraLoadedTypes.Add(typeof(T));
	}

	public IEnumerable<Type> GetEntitySystemTypes()
	{
		return _systemTypes;
	}

	public bool TryGetEntitySystem(Type sysType, [NotNullWhen(true)] out object? system)
	{
		return SystemDependencyCollection.TryResolveType(sysType, out system);
	}

	public object GetEntitySystem(Type sysType)
	{
		return SystemDependencyCollection.ResolveType(sysType);
	}

	private static bool NeedsUpdate(Type type)
	{
		if (!typeof(EntitySystem).IsAssignableFrom(type))
		{
			return true;
		}
		return type.GetMethod("Update", new Type[1] { typeof(float) }).DeclaringType != typeof(EntitySystem);
	}

	private static bool NeedsFrameUpdate(Type type)
	{
		if (!typeof(EntitySystem).IsAssignableFrom(type))
		{
			return true;
		}
		return type.GetMethod("FrameUpdate", new Type[1] { typeof(float) }).DeclaringType != typeof(EntitySystem);
	}

	void IPostInjectInit.PostInject()
	{
		_sawmill = _logManager.GetSawmill("go.sys");
	}

	static EntitySystemManager()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		HistogramConfiguration val = new HistogramConfiguration();
		((MetricConfiguration)val).LabelNames = new string[1] { "system" };
		val.Buckets = Histogram.ExponentialBuckets(1E-06, 1.5, 25);
		_tickUsageHistogram = Metrics.CreateHistogram("robust_entity_systems_update_usage", "Amount of time spent processing each entity system", val);
	}
}
