// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IEntitySystemManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.GameObjects;

public interface IEntitySystemManager
{
  bool MetricsEnabled { get; set; }

  event EventHandler<SystemChangedArgs> SystemLoaded;

  event EventHandler<SystemChangedArgs> SystemUnloaded;

  T GetEntitySystem<T>() where T : IEntitySystem;

  T? GetEntitySystemOrNull<T>() where T : IEntitySystem;

  void Resolve<T>([NotNull] ref T? instance) where T : IEntitySystem;

  void Resolve<T1, T2>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2)
    where T1 : IEntitySystem
    where T2 : IEntitySystem;

  void Resolve<T1, T2, T3>([NotNull] ref T1? instance1, [NotNull] ref T2? instance2, [NotNull] ref T3? instance3)
    where T1 : IEntitySystem
    where T2 : IEntitySystem
    where T3 : IEntitySystem;

  void Resolve<T1, T2, T3, T4>(
    [NotNull] ref T1? instance1,
    [NotNull] ref T2? instance2,
    [NotNull] ref T3? instance3,
    [NotNull] ref T4? instance4)
    where T1 : IEntitySystem
    where T2 : IEntitySystem
    where T3 : IEntitySystem
    where T4 : IEntitySystem;

  bool TryGetEntitySystem<T>([NotNullWhen(true)] out T? entitySystem) where T : IEntitySystem;

  void Initialize(bool discover = true);

  void Shutdown();

  void Clear();

  void TickUpdate(float frameTime, bool noPredictions);

  void FrameUpdate(float frameTime);

  void LoadExtraSystemType<T>() where T : IEntitySystem, new();

  IEnumerable<Type> GetEntitySystemTypes();

  bool TryGetEntitySystem(Type sysType, [NotNullWhen(true)] out object? system);

  object GetEntitySystem(Type sysType);

  IDependencyCollection DependencyCollection { get; }
}
