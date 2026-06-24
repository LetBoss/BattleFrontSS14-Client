// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.DependencyCollection
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace Robust.Shared.IoC;

internal sealed class DependencyCollection : IDependencyCollection
{
  private static readonly Type[] InjectorParameters = new Type[2]
  {
    typeof (object),
    typeof (object[])
  };
  private FrozenDictionary<Type, object> _services = FrozenDictionary<Type, object>.Empty;
  private readonly Dictionary<Type, Type> _resolveTypes = new Dictionary<Type, Type>();
  private readonly Dictionary<Type, DependencyCollection.DependencyFactoryDelegateInternal<object>> _resolveFactories = new Dictionary<Type, DependencyCollection.DependencyFactoryDelegateInternal<object>>();
  private readonly Queue<Type> _pendingResolves = new Queue<Type>();
  private readonly object _serviceBuildLock = new object();
  private readonly Dictionary<Type, DependencyCollection.CachedInjector> _injectorCache = new Dictionary<Type, DependencyCollection.CachedInjector>();
  private readonly ReaderWriterLockSlim _injectorCacheLock = new ReaderWriterLockSlim();
  private readonly IDependencyCollection? _parentCollection;

  public DependencyCollection()
  {
  }

  public DependencyCollection(IDependencyCollection parentCollection)
  {
    this._parentCollection = parentCollection;
  }

  public IDependencyCollection FromParent(IDependencyCollection parentCollection)
  {
    return (IDependencyCollection) new DependencyCollection(parentCollection);
  }

  public IEnumerable<Type> GetRegisteredTypes()
  {
    return this._parentCollection == null ? (IEnumerable<Type>) this._services.Keys : this._services.Keys.Concat<Type>(this._parentCollection.GetRegisteredTypes());
  }

  public Type[] GetCachedInjectorTypes()
  {
    using (this._injectorCacheLock.ReadGuard())
      return this._injectorCache.Where<KeyValuePair<Type, DependencyCollection.CachedInjector>>((Func<KeyValuePair<Type, DependencyCollection.CachedInjector>, bool>) (kv => kv.Value.Delegate != null)).Select<KeyValuePair<Type, DependencyCollection.CachedInjector>, Type>((Func<KeyValuePair<Type, DependencyCollection.CachedInjector>, Type>) (kv => kv.Key)).ToArray<Type>();
  }

  public bool TryResolveType<T>([NotNullWhen(true)] out T? instance)
  {
    object instance1;
    if (this.TryResolveType(typeof (T), out instance1) && instance1 is T obj)
    {
      instance = obj;
      return true;
    }
    instance = default (T);
    return false;
  }

  public bool TryResolveType(Type objectType, [MaybeNullWhen(false)] out object instance)
  {
    return this.TryResolveType(objectType, this._services, out instance);
  }

  private bool TryResolveType(
    Type objectType,
    FrozenDictionary<Type, object> services,
    [MaybeNullWhen(false)] out object instance)
  {
    if (services.TryGetValue(objectType, out instance))
      return true;
    return this._parentCollection != null && this._parentCollection.TryResolveType(objectType, out instance);
  }

  private bool TryResolveType(
    Type objectType,
    IReadOnlyDictionary<Type, object> services,
    [MaybeNullWhen(false)] out object instance)
  {
    if (services.TryGetValue(objectType, out instance))
      return true;
    return this._parentCollection != null && this._parentCollection.TryResolveType(objectType, out instance);
  }

  public void Register<TInterface, TImplementation>(bool overwrite = false)
    where TInterface : class
    where TImplementation : class, TInterface
  {
    this.Register<TInterface, TImplementation>((DependencyCollection.DependencyFactoryDelegateInternal<TImplementation>) (services =>
    {
      ConstructorInfo[] constructors = typeof (TImplementation).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ConstructorInfo constructorInfo = constructors.Length == 1 ? constructors[0] : throw new InvalidOperationException($"Dependency '{typeof (TImplementation).FullName}' requires exactly one constructor.");
      ParameterInfo[] parameters1 = constructors[0].GetParameters();
      object[] parameters2 = new object[parameters1.Length];
      for (int index = 0; index < parameters1.Length; ++index)
      {
        ParameterInfo parameterInfo = parameters1[index];
        object instance;
        if (this.TryResolveType(parameterInfo.ParameterType, services, out instance))
        {
          parameters2[index] = instance;
        }
        else
        {
          if (this._resolveTypes.ContainsKey(parameterInfo.ParameterType))
            throw new InvalidOperationException($"Dependency '{typeof (TImplementation).FullName}' ctor requires {parameterInfo.ParameterType.FullName} registered before it.");
          throw new InvalidOperationException($"Dependency '{typeof (TImplementation).FullName}' ctor has unknown dependency {parameterInfo.ParameterType.FullName}");
        }
      }
      return (TImplementation) constructorInfo.Invoke(parameters2);
    }), overwrite);
  }

  public void Register<TInterface, TImplementation>(
    DependencyFactoryDelegate<TImplementation> factory,
    bool overwrite = false)
    where TInterface : class
    where TImplementation : class, TInterface
  {
    this.Register(typeof (TInterface), typeof (TImplementation), (DependencyFactoryDelegate<object>) factory, overwrite);
  }

  private void Register<TInterface, TImplementation>(
    DependencyCollection.DependencyFactoryDelegateInternal<TImplementation> factory,
    bool overwrite = false)
    where TInterface : class
    where TImplementation : class, TInterface
  {
    this.Register(typeof (TInterface), typeof (TImplementation), (DependencyCollection.DependencyFactoryDelegateInternal<object>) factory, overwrite);
  }

  public void Register(
    Type implementation,
    DependencyFactoryDelegate<object>? factory = null,
    bool overwrite = false)
  {
    this.Register(implementation, implementation, factory, overwrite);
  }

  public void Register(
    Type interfaceType,
    Type implementation,
    DependencyFactoryDelegate<object>? factory = null,
    bool overwrite = false)
  {
    this.Register(interfaceType, implementation, DependencyCollection.FactoryToInternal<object>(factory), overwrite);
  }

  private void Register(
    Type interfaceType,
    Type implementation,
    DependencyCollection.DependencyFactoryDelegateInternal<object>? factory = null,
    bool overwrite = false)
  {
    this.CheckRegisterInterface(interfaceType, implementation, overwrite);
    lock (this._serviceBuildLock)
    {
      this._resolveTypes[interfaceType] = implementation;
      this._resolveFactories[implementation] = factory ?? new DependencyCollection.DependencyFactoryDelegateInternal<object>(DefaultFactory);
      this._pendingResolves.Enqueue(interfaceType);
    }

    object DefaultFactory(IReadOnlyDictionary<Type, object> services)
    {
      ConstructorInfo[] constructors = implementation.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ConstructorInfo constructorInfo = constructors.Length == 1 ? constructors[0] : throw new InvalidOperationException($"Dependency '{implementation.FullName}' requires exactly one constructor.");
      ParameterInfo[] parameters1 = constructorInfo.GetParameters();
      object[] parameters2 = new object[parameters1.Length];
      for (int index = 0; index < parameters1.Length; ++index)
      {
        ParameterInfo parameterInfo = parameters1[index];
        object instance;
        if (this.TryResolveType(parameterInfo.ParameterType, services, out instance))
        {
          parameters2[index] = instance;
        }
        else
        {
          if (this._resolveTypes.ContainsKey(parameterInfo.ParameterType))
            throw new InvalidOperationException($"Dependency '{implementation.FullName}' ctor requires {parameterInfo.ParameterType.FullName} registered before it.");
          throw new InvalidOperationException($"Dependency '{implementation.FullName}' ctor has unknown dependency {parameterInfo.ParameterType.FullName}");
        }
      }
      return constructorInfo.Invoke(parameters2);
    }
  }

  private void CheckRegisterInterface(Type interfaceType, Type implementationType, bool overwrite)
  {
    lock (this._serviceBuildLock)
    {
      if (!this._resolveTypes.ContainsKey(interfaceType))
        return;
      if (!overwrite)
        throw new InvalidOperationException($"Attempted to register already registered interface {interfaceType}. New implementation: {implementationType}, Old implementation: {this._resolveTypes[interfaceType]}");
      if (this._services.ContainsKey(interfaceType))
        throw new InvalidOperationException($"Attempted to overwrite already instantiated interface {interfaceType}.");
    }
  }

  public void RegisterInstance<TInterface>(object implementation, bool overwrite = false) where TInterface : class
  {
    this.RegisterInstance(typeof (TInterface), implementation, overwrite);
  }

  public void RegisterInstance(Type type, object implementation, bool overwrite = false)
  {
    if (implementation == null)
      throw new ArgumentNullException(nameof (implementation));
    if (!implementation.GetType().IsAssignableTo(type))
      throw new InvalidOperationException($"Implementation type {implementation.GetType()} is not assignable to type {type}");
    this.Register(type, implementation.GetType(), (DependencyFactoryDelegate<object>) (() => implementation), overwrite);
  }

  public void Clear()
  {
    foreach (IDisposable disposable in this._services.Values.OfType<IDisposable>().Distinct<IDisposable>())
      disposable.Dispose();
    this._services = FrozenDictionary<Type, object>.Empty;
    lock (this._serviceBuildLock)
    {
      this._resolveTypes.Clear();
      this._resolveFactories.Clear();
      this._pendingResolves.Clear();
    }
    using (this._injectorCacheLock.WriteGuard())
      this._injectorCache.Clear();
  }

  public T Resolve<T>() => (T) this.ResolveType(typeof (T));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Resolve<T>([NotNull] ref T? instance)
  {
    if ((object) instance != null)
      return;
    instance = this.Resolve<T>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Resolve<T1, T2>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2)
  {
    this.Resolve<T1>(ref instance1);
    this.Resolve<T2>(ref instance2);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Resolve<T1, T2, T3>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3)
  {
    this.Resolve<T1, T2>(ref instance1, ref instance2);
    this.Resolve<T3>(ref instance3);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Resolve<T1, T2, T3, T4>(
    [NotNull] ref T1? instance1,
    [NotNull] ref T2? instance2,
    [NotNull] ref T3? instance3,
    [NotNull] ref T4? instance4)
  {
    this.Resolve<T1, T2>(ref instance1, ref instance2);
    this.Resolve<T3, T4>(ref instance3, ref instance4);
  }

  public object ResolveType(Type type)
  {
    object instance;
    if (this.TryResolveType(type, out instance))
      return instance;
    if (this._resolveTypes.ContainsKey(type))
      throw new InvalidOperationException($"Attempted to resolve type {type} before the object graph for it has been populated.");
    if (type == typeof (IDependencyCollection))
      return (object) this;
    throw new UnregisteredTypeException(type);
  }

  public void BuildGraph()
  {
    lock (this._serviceBuildLock)
    {
      List<object> source = new List<object>();
      Dictionary<Type, object> newDeps = this._services.ToDictionary<Type, object>();
      while (this._pendingResolves.Count > 0)
      {
        Type key1 = this._pendingResolves.Dequeue();
        Type value = this._resolveTypes[key1];
        (Type key2, Type _) = this._resolveTypes.FirstOrDefault<KeyValuePair<Type, Type>>((Func<KeyValuePair<Type, Type>, bool>) (p => newDeps.ContainsKey(p.Key) && p.Value == value));
        if (key2 != (Type) null)
        {
          newDeps[key1] = newDeps[key2];
        }
        else
        {
          try
          {
            object obj = this._resolveFactories[value]((IReadOnlyDictionary<Type, object>) newDeps);
            newDeps[key1] = obj;
            source.Add(obj);
          }
          catch (TargetInvocationException ex)
          {
            throw new ImplementationConstructorException(value, ex.InnerException);
          }
        }
      }
      this._resolveFactories.Clear();
      this._services = newDeps.ToFrozenDictionary<Type, object>();
      foreach (object obj in source)
        this.InjectDependenciesReflection(obj, this._services);
      foreach (IPostInjectInit postInjectInit in source.OfType<IPostInjectInit>())
        postInjectInit.PostInject();
    }
  }

  public void InjectDependencies(object obj, bool oneOff = false)
  {
    Type type = obj.GetType();
    DependencyCollection.CachedInjector cachedInjector;
    bool flag;
    using (this._injectorCacheLock.ReadGuard())
      flag = this._injectorCache.TryGetValue(type, out cachedInjector);
    if (!flag)
    {
      if (oneOff)
      {
        this.InjectDependenciesReflection(obj);
        return;
      }
      cachedInjector = this.CacheInjector(type);
    }
    (DependencyCollection.InjectorDelegate Delegate, object[] objArray) = cachedInjector;
    if (Delegate == null)
      return;
    Delegate(obj, objArray);
  }

  private void InjectDependenciesReflection(object obj)
  {
    this.InjectDependenciesReflection(obj, this._services);
  }

  private void InjectDependenciesReflection(object obj, FrozenDictionary<Type, object> services)
  {
    Type type = obj.GetType();
    foreach (FieldInfo allField in type.GetAllFields())
    {
      if (Attribute.IsDefined((MemberInfo) allField, typeof (DependencyAttribute)))
      {
        object instance;
        if (this.TryResolveType(allField.FieldType, services, out instance))
        {
          allField.SetValue(obj, instance);
        }
        else
        {
          if (!(allField.FieldType == typeof (IDependencyCollection)))
            throw new UnregisteredDependencyException(type, allField.FieldType, allField.Name);
          allField.SetValue(obj, (object) this);
        }
      }
    }
  }

  private DependencyCollection.CachedInjector CacheInjector(Type type)
  {
    using (this._injectorCacheLock.WriteGuard())
    {
      DependencyCollection.CachedInjector cachedInjector1;
      if (this._injectorCache.TryGetValue(type, out cachedInjector1))
        return cachedInjector1;
      List<FieldInfo> fieldInfoList = new List<FieldInfo>();
      foreach (FieldInfo allField in type.GetAllFields())
      {
        if (Attribute.IsDefined((MemberInfo) allField, typeof (DependencyAttribute)))
          fieldInfoList.Add(allField);
      }
      if (fieldInfoList.Count == 0)
      {
        this._injectorCache.Add(type, new DependencyCollection.CachedInjector());
        return new DependencyCollection.CachedInjector();
      }
      DynamicMethod dynamicMethod = new DynamicMethod($"_injector<>{type}", (Type) null, DependencyCollection.InjectorParameters, type, true);
      dynamicMethod.DefineParameter(1, ParameterAttributes.In, "target");
      dynamicMethod.DefineParameter(2, ParameterAttributes.In, "services");
      int num = 0;
      List<object> objectList = new List<object>();
      ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
      foreach (FieldInfo field in fieldInfoList)
      {
        ilGenerator.Emit(OpCodes.Ldarg_0);
        object instance;
        if (!this.TryResolveType(field.FieldType, out instance))
        {
          if (!(field.FieldType == typeof (IDependencyCollection)))
            throw new UnregisteredDependencyException(type, field.FieldType, field.Name);
          instance = (object) this;
        }
        objectList.Add(instance);
        ilGenerator.Emit(OpCodes.Ldarg_1);
        ilGenerator.Emit(OpCodes.Ldc_I4, num++);
        ilGenerator.Emit(OpCodes.Ldelem_Ref);
        ilGenerator.Emit(OpCodes.Stfld, field);
      }
      ilGenerator.Emit(OpCodes.Ret);
      DependencyCollection.CachedInjector cachedInjector2 = new DependencyCollection.CachedInjector((DependencyCollection.InjectorDelegate) dynamicMethod.CreateDelegate(typeof (DependencyCollection.InjectorDelegate)), objectList.ToArray());
      this._injectorCache.Add(type, cachedInjector2);
      return cachedInjector2;
    }
  }

  [return: NotNullIfNotNull("factory")]
  private static DependencyCollection.DependencyFactoryDelegateInternal<T>? FactoryToInternal<T>(
    DependencyFactoryDelegate<T>? factory)
    where T : class
  {
    return factory == null ? (DependencyCollection.DependencyFactoryDelegateInternal<T>) null : (DependencyCollection.DependencyFactoryDelegateInternal<T>) (_ => factory());
  }

  private delegate void InjectorDelegate(object target, object[] services);

  private delegate T DependencyFactoryDelegateInternal<out T>(
    IReadOnlyDictionary<Type, object> services)
    where T : class;

  private record struct CachedInjector(
    DependencyCollection.InjectorDelegate? Delegate,
    object[]? Services)
  ;
}
