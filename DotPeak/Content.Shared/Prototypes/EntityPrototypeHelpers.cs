// Decompiled with JetBrains decompiler
// Type: Content.Shared.Prototypes.EntityPrototypeHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Prototypes;

public static class EntityPrototypeHelpers
{
  public static bool HasComponent<T>(
    this EntityPrototype prototype,
    IComponentFactory? componentFactory = null)
    where T : IComponent
  {
    return prototype.HasComponent(typeof (T), componentFactory);
  }

  public static bool HasComponent(
    this EntityPrototype prototype,
    Type component,
    IComponentFactory? componentFactory = null)
  {
    if (componentFactory == null)
      componentFactory = IoCManager.Resolve<IComponentFactory>();
    ComponentRegistration registration = componentFactory.GetRegistration(component);
    return prototype.Components.ContainsKey(registration.Name);
  }

  public static bool HasComponent<T>(
    string prototype,
    IPrototypeManager? prototypeManager = null,
    IComponentFactory? componentFactory = null)
    where T : IComponent
  {
    return EntityPrototypeHelpers.HasComponent(prototype, typeof (T), prototypeManager, componentFactory);
  }

  public static bool HasComponent(
    string prototype,
    Type component,
    IPrototypeManager? prototypeManager = null,
    IComponentFactory? componentFactory = null)
  {
    if (prototypeManager == null)
      prototypeManager = IoCManager.Resolve<IPrototypeManager>();
    EntityPrototype prototype1;
    return prototypeManager.TryIndex<EntityPrototype>(prototype, out prototype1) && prototype1.HasComponent(component, componentFactory);
  }
}
