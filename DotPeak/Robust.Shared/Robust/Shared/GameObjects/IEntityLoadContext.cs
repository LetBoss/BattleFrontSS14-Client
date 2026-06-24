// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IEntityLoadContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.GameObjects;

internal interface IEntityLoadContext
{
  bool TryGetComponent(string componentName, [NotNullWhen(true)] out IComponent? component);

  bool TryGetComponent<TComponent>(IComponentFactory componentFactory, [NotNullWhen(true)] out TComponent? component) where TComponent : class, IComponent, new();

  IEnumerable<string> GetExtraComponentTypes();

  bool ShouldSkipComponent(string compName);
}
