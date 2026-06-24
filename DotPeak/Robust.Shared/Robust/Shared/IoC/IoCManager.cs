// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.IoCManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace Robust.Shared.IoC;

public static class IoCManager
{
  private const string NoContextAssert = "IoC has no context on this thread. Are you calling IoC from the wrong thread or did you forget to initialize it?";
  private static readonly ThreadLocal<IDependencyCollection> _container = new ThreadLocal<IDependencyCollection>();

  public static IDependencyCollection? Instance
  {
    get
    {
      return !IoCManager._container.IsValueCreated ? (IDependencyCollection) null : IoCManager._container.Value;
    }
  }

  public static IDependencyCollection InitThread()
  {
    if (IoCManager._container.IsValueCreated)
      return IoCManager._container.Value;
    DependencyCollection dependencyCollection = new DependencyCollection();
    IoCManager._container.Value = (IDependencyCollection) dependencyCollection;
    return (IDependencyCollection) dependencyCollection;
  }

  public static void InitThread(IDependencyCollection collection, bool replaceExisting = false)
  {
    if (IoCManager._container.IsValueCreated && !replaceExisting)
      throw new InvalidOperationException("This thread has already been initialized.");
    IoCManager._container.Value = collection;
  }

  public static void Register<TInterface, TImplementation>(bool overwrite = false)
    where TInterface : class
    where TImplementation : class, TInterface
  {
    IoCManager._container.Value.Register<TInterface, TImplementation>(overwrite);
  }

  public static void Register<T>(bool overwrite = false) where T : class
  {
    IoCManager.Register<T, T>(overwrite);
  }

  public static void Register<TInterface, TImplementation>(
    DependencyFactoryDelegate<TImplementation> factory,
    bool overwrite = false)
    where TInterface : class
    where TImplementation : class, TInterface
  {
    IoCManager._container.Value.Register<TInterface, TImplementation>(factory, overwrite);
  }

  public static void RegisterInstance<TInterface>(object implementation, bool overwrite = false) where TInterface : class
  {
    IoCManager._container.Value.RegisterInstance<TInterface>(implementation, overwrite);
  }

  public static void Clear()
  {
    if (!IoCManager._container.IsValueCreated)
      return;
    IoCManager._container.Value.Clear();
  }

  public static T Resolve<T>() => IoCManager._container.Value.Resolve<T>();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Resolve<T>([NotNull] ref T? instance)
  {
    if ((object) instance != null)
      return;
    instance = IoCManager.Resolve<T>();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Resolve<T1, T2>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2)
  {
    IoCManager._container.Value.Resolve<T1, T2>(ref instance1, ref instance2);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Resolve<T1, T2, T3>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3)
  {
    IoCManager._container.Value.Resolve<T1, T2, T3>(ref instance1, ref instance2, ref instance3);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Resolve<T1, T2, T3, T4>(
    [NotNull] ref T1? instance1,
    [NotNull] ref T2? instance2,
    [NotNull] ref T3? instance3,
    [NotNull] ref T4? instance4)
  {
    IoCManager._container.Value.Resolve<T1, T2, T3, T4>(ref instance1, ref instance2, ref instance3, ref instance4);
  }

  public static object ResolveType(Type type) => IoCManager._container.Value.ResolveType(type);

  public static void BuildGraph() => IoCManager._container.Value.BuildGraph();

  public static T InjectDependencies<T>(T obj) where T : notnull
  {
    IoCManager._container.Value.InjectDependencies((object) obj);
    return obj;
  }
}
