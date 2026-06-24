// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.CompRegistryEntityEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.GameObjects;

public struct CompRegistryEntityEnumerator : IDisposable
{
  private IEntityManager _entManager;
  private Dictionary<EntityUid, IComponent>.Enumerator _traitDict;
  private ComponentRegistry _registry;

  public CompRegistryEntityEnumerator(
    IEntityManager entManager,
    Dictionary<EntityUid, IComponent> traitDict,
    ComponentRegistry registry)
  {
    this._entManager = entManager;
    this._traitDict = traitDict.GetEnumerator();
    this._registry = registry;
  }

  public bool MoveNext(out EntityUid uid)
  {
    while (this._traitDict.MoveNext())
    {
      KeyValuePair<EntityUid, IComponent> current = this._traitDict.Current;
      if (!current.Value.Deleted)
      {
        int num = -1;
        bool flag = true;
        foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> keyValuePair in (Dictionary<string, EntityPrototype.ComponentRegistryEntry>) this._registry)
        {
          ++num;
          IComponent component;
          if (num != 0 && (!this._entManager.TryGetComponent(current.Key, keyValuePair.Value.Component.GetType(), out component) || component.Deleted))
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          uid = current.Key;
          return true;
        }
      }
    }
    uid = new EntityUid();
    return false;
  }

  public void Dispose() => this._traitDict.Dispose();
}
