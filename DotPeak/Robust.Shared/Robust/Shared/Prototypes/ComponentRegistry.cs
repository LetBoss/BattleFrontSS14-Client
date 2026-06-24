// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.ComponentRegistry
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Prototypes;

public sealed class ComponentRegistry : 
  Dictionary<string, EntityPrototype.ComponentRegistryEntry>,
  IEntityLoadContext
{
  public ComponentRegistry()
  {
  }

  public ComponentRegistry(
    Dictionary<string, EntityPrototype.ComponentRegistryEntry> components)
    : base((IDictionary<string, EntityPrototype.ComponentRegistryEntry>) components)
  {
  }

  public bool TryGetComponent(string componentName, [NotNullWhen(true)] out IComponent? component)
  {
    EntityPrototype.ComponentRegistryEntry componentRegistryEntry;
    int num = this.TryGetValue(componentName, out componentRegistryEntry) ? 1 : 0;
    component = componentRegistryEntry?.Component;
    return num != 0;
  }

  public bool TryGetComponent<TComponent>(
    IComponentFactory componentFactory,
    [NotNullWhen(true)] out TComponent? component)
    where TComponent : class, IComponent, new()
  {
    component = default (TComponent);
    IComponent component1;
    if (!this.TryGetComponent(componentFactory.GetComponentName<TComponent>(), out component1))
      return false;
    component = (TComponent) component1;
    return true;
  }

  public IEnumerable<string> GetExtraComponentTypes() => (IEnumerable<string>) this.Keys;

  public bool ShouldSkipComponent(string compName) => false;
}
