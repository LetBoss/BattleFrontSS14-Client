// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.IDependencyCollection
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.IoC;

[NotContentImplementable]
public interface IDependencyCollection
{
  IDependencyCollection FromParent(IDependencyCollection parentCollection);

  IEnumerable<Type> GetRegisteredTypes();

  void Register<TInterface, TImplementation>(bool overwrite = false)
    where TInterface : class
    where TImplementation : class, TInterface;

  void Register<TInterface, TImplementation>(
    DependencyFactoryDelegate<TImplementation> factory,
    bool overwrite = false)
    where TInterface : class
    where TImplementation : class, TInterface;

  void Register(Type implementation, DependencyFactoryDelegate<object>? factory = null, bool overwrite = false);

  void Register(
    Type interfaceType,
    Type implementation,
    DependencyFactoryDelegate<object>? factory = null,
    bool overwrite = false);

  void RegisterInstance<TInterface>(object implementation, bool overwrite = false) where TInterface : class;

  void RegisterInstance(Type type, object implementation, bool overwrite = false);

  void Clear();

  T Resolve<T>();

  void Resolve<T>([NotNull] ref T? instance);

  void Resolve<T1, T2>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2);

  void Resolve<T1, T2, T3>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3);

  void Resolve<T1, T2, T3, T4>(
    [NotNull] ref T1? instance1,
    [NotNull] ref T2? instance2,
    [NotNull] ref T3? instance3,
    [NotNull] ref T4? instance4);

  object ResolveType(Type type);

  bool TryResolveType<T>([NotNullWhen(true)] out T? instance);

  bool TryResolveType(Type objectType, [MaybeNullWhen(false)] out object instance);

  void BuildGraph();

  void InjectDependencies(object obj, bool oneOff = false);
}
