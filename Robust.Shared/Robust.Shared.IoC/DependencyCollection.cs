using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Shared.IoC.Exceptions;
using Robust.Shared.Utility;

namespace Robust.Shared.IoC;

internal sealed class DependencyCollection : IDependencyCollection
{
	private delegate void InjectorDelegate(object target, object[] services);

	private delegate T DependencyFactoryDelegateInternal<out T>(IReadOnlyDictionary<Type, object> services) where T : class;

	private record struct CachedInjector(InjectorDelegate? Delegate, object[]? Services);

	private static readonly Type[] InjectorParameters = new Type[2]
	{
		typeof(object),
		typeof(object[])
	};

	private FrozenDictionary<Type, object> _services = FrozenDictionary<Type, object>.Empty;

	private readonly Dictionary<Type, Type> _resolveTypes = new Dictionary<Type, Type>();

	private readonly Dictionary<Type, DependencyFactoryDelegateInternal<object>> _resolveFactories = new Dictionary<Type, DependencyFactoryDelegateInternal<object>>();

	private readonly Queue<Type> _pendingResolves = new Queue<Type>();

	private readonly object _serviceBuildLock = new object();

	private readonly Dictionary<Type, CachedInjector> _injectorCache = new Dictionary<Type, CachedInjector>();

	private readonly ReaderWriterLockSlim _injectorCacheLock = new ReaderWriterLockSlim();

	private readonly IDependencyCollection? _parentCollection;

	public DependencyCollection()
	{
	}

	public DependencyCollection(IDependencyCollection parentCollection)
	{
		_parentCollection = parentCollection;
	}

	public IDependencyCollection FromParent(IDependencyCollection parentCollection)
	{
		return new DependencyCollection(parentCollection);
	}

	public IEnumerable<Type> GetRegisteredTypes()
	{
		if (_parentCollection == null)
		{
			return _services.Keys;
		}
		return _services.Keys.Concat(_parentCollection.GetRegisteredTypes());
	}

	public Type[] GetCachedInjectorTypes()
	{
		using (_injectorCacheLock.ReadGuard())
		{
			return (from kv in _injectorCache
				where kv.Value.Delegate != null
				select kv.Key).ToArray();
		}
	}

	public bool TryResolveType<T>([NotNullWhen(true)] out T? instance)
	{
		if (TryResolveType(typeof(T), out object instance2) && instance2 is T val)
		{
			instance = val;
			return true;
		}
		instance = default(T);
		return false;
	}

	public bool TryResolveType(Type objectType, [MaybeNullWhen(false)] out object instance)
	{
		return TryResolveType(objectType, _services, out instance);
	}

	private bool TryResolveType(Type objectType, FrozenDictionary<Type, object> services, [MaybeNullWhen(false)] out object instance)
	{
		if (!services.TryGetValue(objectType, out instance))
		{
			if (_parentCollection != null)
			{
				return _parentCollection.TryResolveType(objectType, out instance);
			}
			return false;
		}
		return true;
	}

	private bool TryResolveType(Type objectType, IReadOnlyDictionary<Type, object> services, [MaybeNullWhen(false)] out object instance)
	{
		if (!services.TryGetValue(objectType, out instance))
		{
			if (_parentCollection != null)
			{
				return _parentCollection.TryResolveType(objectType, out instance);
			}
			return false;
		}
		return true;
	}

	public void Register<TInterface, TImplementation>(bool overwrite = false) where TInterface : class where TImplementation : class, TInterface
	{
		Register<TInterface, TImplementation>((DependencyFactoryDelegateInternal<TImplementation>)delegate(IReadOnlyDictionary<Type, object> services)
		{
			ConstructorInfo[] constructors = typeof(TImplementation).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (constructors.Length != 1)
			{
				throw new InvalidOperationException("Dependency '" + typeof(TImplementation).FullName + "' requires exactly one constructor.");
			}
			ConstructorInfo constructorInfo = constructors[0];
			ParameterInfo[] parameters = constructors[0].GetParameters();
			object[] array = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (!TryResolveType(parameterInfo.ParameterType, services, out object instance))
				{
					if (_resolveTypes.ContainsKey(parameterInfo.ParameterType))
					{
						throw new InvalidOperationException($"Dependency '{typeof(TImplementation).FullName}' ctor requires {parameterInfo.ParameterType.FullName} registered before it.");
					}
					throw new InvalidOperationException("Dependency '" + typeof(TImplementation).FullName + "' ctor has unknown dependency " + parameterInfo.ParameterType.FullName);
				}
				array[i] = instance;
			}
			return (TImplementation)constructorInfo.Invoke(array);
		}, overwrite);
	}

	public void Register<TInterface, TImplementation>(DependencyFactoryDelegate<TImplementation> factory, bool overwrite = false) where TInterface : class where TImplementation : class, TInterface
	{
		Register(typeof(TInterface), typeof(TImplementation), factory, overwrite);
	}

	private void Register<TInterface, TImplementation>(DependencyFactoryDelegateInternal<TImplementation> factory, bool overwrite = false) where TInterface : class where TImplementation : class, TInterface
	{
		Register(typeof(TInterface), typeof(TImplementation), factory, overwrite);
	}

	public void Register(Type implementation, DependencyFactoryDelegate<object>? factory = null, bool overwrite = false)
	{
		Register(implementation, implementation, factory, overwrite);
	}

	public void Register(Type interfaceType, Type implementation, DependencyFactoryDelegate<object>? factory = null, bool overwrite = false)
	{
		Register(interfaceType, implementation, FactoryToInternal(factory), overwrite);
	}

	private void Register(Type interfaceType, Type implementation, DependencyFactoryDelegateInternal<object>? factory = null, bool overwrite = false)
	{
		CheckRegisterInterface(interfaceType, implementation, overwrite);
		lock (_serviceBuildLock)
		{
			_resolveTypes[interfaceType] = implementation;
			_resolveFactories[implementation] = factory ?? new DependencyFactoryDelegateInternal<object>(DefaultFactory);
			_pendingResolves.Enqueue(interfaceType);
		}
		object DefaultFactory(IReadOnlyDictionary<Type, object> services)
		{
			ConstructorInfo[] constructors = implementation.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (constructors.Length != 1)
			{
				throw new InvalidOperationException("Dependency '" + implementation.FullName + "' requires exactly one constructor.");
			}
			ConstructorInfo constructorInfo = constructors[0];
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			object[] array = new object[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (!TryResolveType(parameterInfo.ParameterType, services, out object instance))
				{
					if (_resolveTypes.ContainsKey(parameterInfo.ParameterType))
					{
						throw new InvalidOperationException($"Dependency '{implementation.FullName}' ctor requires {parameterInfo.ParameterType.FullName} registered before it.");
					}
					throw new InvalidOperationException("Dependency '" + implementation.FullName + "' ctor has unknown dependency " + parameterInfo.ParameterType.FullName);
				}
				array[i] = instance;
			}
			return constructorInfo.Invoke(array);
		}
	}

	private void CheckRegisterInterface(Type interfaceType, Type implementationType, bool overwrite)
	{
		lock (_serviceBuildLock)
		{
			if (_resolveTypes.ContainsKey(interfaceType))
			{
				if (!overwrite)
				{
					throw new InvalidOperationException($"Attempted to register already registered interface {interfaceType}. New implementation: {implementationType}, Old implementation: {_resolveTypes[interfaceType]}");
				}
				if (_services.ContainsKey(interfaceType))
				{
					throw new InvalidOperationException($"Attempted to overwrite already instantiated interface {interfaceType}.");
				}
			}
		}
	}

	public void RegisterInstance<TInterface>(object implementation, bool overwrite = false) where TInterface : class
	{
		RegisterInstance(typeof(TInterface), implementation, overwrite);
	}

	public void RegisterInstance(Type type, object implementation, bool overwrite = false)
	{
		if (implementation == null)
		{
			throw new ArgumentNullException("implementation");
		}
		if (!implementation.GetType().IsAssignableTo(type))
		{
			throw new InvalidOperationException($"Implementation type {implementation.GetType()} is not assignable to type {type}");
		}
		Register(type, implementation.GetType(), () => implementation, overwrite);
	}

	public void Clear()
	{
		foreach (IDisposable item in _services.Values.OfType<IDisposable>().Distinct())
		{
			item.Dispose();
		}
		_services = FrozenDictionary<Type, object>.Empty;
		lock (_serviceBuildLock)
		{
			_resolveTypes.Clear();
			_resolveFactories.Clear();
			_pendingResolves.Clear();
		}
		using (_injectorCacheLock.WriteGuard())
		{
			_injectorCache.Clear();
		}
	}

	public T Resolve<T>()
	{
		return (T)ResolveType(typeof(T));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Resolve<T>([NotNull] ref T? instance)
	{
		T val = instance;
		if (val == null)
		{
			instance = Resolve<T>();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Resolve<T1, T2>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2)
	{
		Resolve(ref instance1);
		Resolve(ref instance2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Resolve<T1, T2, T3>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3)
	{
		Resolve(ref instance1, ref instance2);
		Resolve(ref instance3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Resolve<T1, T2, T3, T4>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3, [NotNull] ref T4? instance4)
	{
		Resolve(ref instance1, ref instance2);
		Resolve(ref instance3, ref instance4);
	}

	public object ResolveType(Type type)
	{
		if (TryResolveType(type, out object instance))
		{
			return instance;
		}
		if (_resolveTypes.ContainsKey(type))
		{
			throw new InvalidOperationException($"Attempted to resolve type {type} before the object graph for it has been populated.");
		}
		if (type == typeof(IDependencyCollection))
		{
			return this;
		}
		throw new UnregisteredTypeException(type);
	}

	public void BuildGraph()
	{
		lock (_serviceBuildLock)
		{
			List<object> list = new List<object>();
			Dictionary<Type, object> newDeps = _services.ToDictionary();
			while (_pendingResolves.Count > 0)
			{
				Type key = _pendingResolves.Dequeue();
				Type value = _resolveTypes[key];
				var (type3, _) = (KeyValuePair<Type, Type>)(ref _resolveTypes.FirstOrDefault<KeyValuePair<Type, Type>>((KeyValuePair<Type, Type> p) => newDeps.ContainsKey(p.Key) && p.Value == value));
				if (type3 != null)
				{
					newDeps[key] = newDeps[type3];
					continue;
				}
				try
				{
					object obj = _resolveFactories[value](newDeps);
					newDeps[key] = obj;
					list.Add(obj);
				}
				catch (TargetInvocationException ex)
				{
					throw new ImplementationConstructorException(value, ex.InnerException);
				}
			}
			_resolveFactories.Clear();
			_services = newDeps.ToFrozenDictionary();
			foreach (object item in list)
			{
				InjectDependenciesReflection(item, _services);
			}
			foreach (IPostInjectInit item2 in list.OfType<IPostInjectInit>())
			{
				item2.PostInject();
			}
		}
	}

	public void InjectDependencies(object obj, bool oneOff = false)
	{
		Type type = obj.GetType();
		bool flag;
		CachedInjector value;
		using (_injectorCacheLock.ReadGuard())
		{
			flag = _injectorCache.TryGetValue(type, out value);
		}
		if (!flag)
		{
			if (oneOff)
			{
				InjectDependenciesReflection(obj);
				return;
			}
			value = CacheInjector(type);
		}
		CachedInjector cachedInjector = value;
		var (injectorDelegate2, services) = (CachedInjector)(ref cachedInjector);
		injectorDelegate2?.Invoke(obj, services);
	}

	private void InjectDependenciesReflection(object obj)
	{
		InjectDependenciesReflection(obj, _services);
	}

	private void InjectDependenciesReflection(object obj, FrozenDictionary<Type, object> services)
	{
		Type type = obj.GetType();
		foreach (FieldInfo allField in type.GetAllFields())
		{
			if (!Attribute.IsDefined(allField, typeof(DependencyAttribute)))
			{
				continue;
			}
			if (TryResolveType(allField.FieldType, services, out object instance))
			{
				allField.SetValue(obj, instance);
				continue;
			}
			if (allField.FieldType == typeof(IDependencyCollection))
			{
				allField.SetValue(obj, this);
				continue;
			}
			throw new UnregisteredDependencyException(type, allField.FieldType, allField.Name);
		}
	}

	private CachedInjector CacheInjector(Type type)
	{
		using (_injectorCacheLock.WriteGuard())
		{
			if (_injectorCache.TryGetValue(type, out var value))
			{
				return value;
			}
			List<FieldInfo> list = new List<FieldInfo>();
			foreach (FieldInfo allField in type.GetAllFields())
			{
				if (Attribute.IsDefined(allField, typeof(DependencyAttribute)))
				{
					list.Add(allField);
				}
			}
			if (list.Count == 0)
			{
				_injectorCache.Add(type, default(CachedInjector));
				return default(CachedInjector);
			}
			DynamicMethod dynamicMethod = new DynamicMethod($"_injector<>{type}", null, InjectorParameters, type, skipVisibility: true);
			dynamicMethod.DefineParameter(1, ParameterAttributes.In, "target");
			dynamicMethod.DefineParameter(2, ParameterAttributes.In, "services");
			int num = 0;
			List<object> list2 = new List<object>();
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			foreach (FieldInfo item in list)
			{
				iLGenerator.Emit(OpCodes.Ldarg_0);
				if (!TryResolveType(item.FieldType, out object instance))
				{
					if (!(item.FieldType == typeof(IDependencyCollection)))
					{
						throw new UnregisteredDependencyException(type, item.FieldType, item.Name);
					}
					instance = this;
				}
				list2.Add(instance);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Ldc_I4, num++);
				iLGenerator.Emit(OpCodes.Ldelem_Ref);
				iLGenerator.Emit(OpCodes.Stfld, item);
			}
			iLGenerator.Emit(OpCodes.Ret);
			InjectorDelegate injectorDelegate = (InjectorDelegate)dynamicMethod.CreateDelegate(typeof(InjectorDelegate));
			value = new CachedInjector(injectorDelegate, list2.ToArray());
			_injectorCache.Add(type, value);
			return value;
		}
	}

	[return: NotNullIfNotNull("factory")]
	private static DependencyFactoryDelegateInternal<T>? FactoryToInternal<T>(DependencyFactoryDelegate<T>? factory) where T : class
	{
		if (factory == null)
		{
			return null;
		}
		return (IReadOnlyDictionary<Type, object> _) => factory();
	}
}
