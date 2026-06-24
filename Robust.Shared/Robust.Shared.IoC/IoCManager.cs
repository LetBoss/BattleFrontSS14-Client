using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Robust.Shared.IoC;

public static class IoCManager
{
	private const string NoContextAssert = "IoC has no context on this thread. Are you calling IoC from the wrong thread or did you forget to initialize it?";

	private static readonly ThreadLocal<IDependencyCollection> _container = new ThreadLocal<IDependencyCollection>();

	public static IDependencyCollection? Instance
	{
		get
		{
			if (!_container.IsValueCreated)
			{
				return null;
			}
			return _container.Value;
		}
	}

	public static IDependencyCollection InitThread()
	{
		if (_container.IsValueCreated)
		{
			return _container.Value;
		}
		DependencyCollection dependencyCollection = new DependencyCollection();
		_container.Value = dependencyCollection;
		return dependencyCollection;
	}

	public static void InitThread(IDependencyCollection collection, bool replaceExisting = false)
	{
		if (_container.IsValueCreated && !replaceExisting)
		{
			throw new InvalidOperationException("This thread has already been initialized.");
		}
		_container.Value = collection;
	}

	public static void Register<TInterface, TImplementation>(bool overwrite = false) where TInterface : class where TImplementation : class, TInterface
	{
		_container.Value.Register<TInterface, TImplementation>(overwrite);
	}

	public static void Register<T>(bool overwrite = false) where T : class
	{
		Register<T, T>(overwrite);
	}

	public static void Register<TInterface, TImplementation>(DependencyFactoryDelegate<TImplementation> factory, bool overwrite = false) where TInterface : class where TImplementation : class, TInterface
	{
		_container.Value.Register<TInterface, TImplementation>(factory, overwrite);
	}

	public static void RegisterInstance<TInterface>(object implementation, bool overwrite = false) where TInterface : class
	{
		_container.Value.RegisterInstance<TInterface>(implementation, overwrite);
	}

	public static void Clear()
	{
		if (_container.IsValueCreated)
		{
			_container.Value.Clear();
		}
	}

	public static T Resolve<T>()
	{
		return _container.Value.Resolve<T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Resolve<T>([NotNull] ref T? instance)
	{
		T val = instance;
		if (val == null)
		{
			instance = Resolve<T>();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Resolve<T1, T2>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2)
	{
		_container.Value.Resolve(ref instance1, ref instance2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Resolve<T1, T2, T3>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3)
	{
		_container.Value.Resolve(ref instance1, ref instance2, ref instance3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Resolve<T1, T2, T3, T4>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3, [NotNull] ref T4? instance4)
	{
		_container.Value.Resolve(ref instance1, ref instance2, ref instance3, ref instance4);
	}

	public static object ResolveType(Type type)
	{
		return _container.Value.ResolveType(type);
	}

	public static void BuildGraph()
	{
		_container.Value.BuildGraph();
	}

	public static T InjectDependencies<T>(T obj) where T : notnull
	{
		_container.Value.InjectDependencies(obj);
		return obj;
	}
}
