// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.MultiRootInheritanceGraph`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Prototypes;

public sealed class MultiRootInheritanceGraph<T> where T : notnull
{
  private readonly HashSet<T> _rootNodes = new HashSet<T>();
  private readonly Dictionary<T, HashSet<T>> _edges = new Dictionary<T, HashSet<T>>();
  private readonly Dictionary<T, T[]> _parents = new Dictionary<T, T[]>();

  public bool Add(T id) => this._rootNodes.Add(id);

  public IReadOnlySet<T> RootNodes => (IReadOnlySet<T>) this._rootNodes;

  public IReadOnlySet<T>? GetChildren(T id)
  {
    return (IReadOnlySet<T>) this._edges.GetValueOrDefault<T, HashSet<T>>(id);
  }

  public bool TryGetChildren(T id, [NotNullWhen(true)] out IReadOnlySet<T>? set)
  {
    set = this.GetChildren(id);
    return set != null;
  }

  public T[]? GetParents(T id) => this._parents.GetValueOrDefault<T, T[]>(id);

  public int GetParentsCount(T id)
  {
    T[] valueOrDefault = this._parents.GetValueOrDefault<T, T[]>(id);
    return valueOrDefault == null ? 0 : valueOrDefault.Length;
  }

  public bool TryGetParents(T id, [NotNullWhen(true)] out T[]? parents)
  {
    parents = this.GetParents(id);
    return parents != null;
  }

  public void Add(T id, params T[] parents)
  {
    foreach (T parent in parents)
    {
      if (EqualityComparer<T>.Default.Equals(parent, id))
        throw new InvalidOperationException($"Self Inheritance detected for id \"{id}\"!");
      T[] parents1 = this.GetParents(parent);
      if (parents1 != null)
      {
        Queue<T> objQueue = new Queue<T>((IEnumerable<T>) parents1);
        T result;
        while (objQueue.TryDequeue(out result))
        {
          if (EqualityComparer<T>.Default.Equals(result, id))
            throw new InvalidOperationException($"Circular Inheritance detected for id \"{id}\" and parent \"{parent}\"");
          T[] parents2 = this.GetParents(result);
          if (parents2 != null)
          {
            foreach (T obj in parents2)
              objQueue.Enqueue(obj);
          }
        }
      }
    }
    this._rootNodes.Remove(id);
    foreach (T parent in parents)
    {
      this._edges.GetOrNew<T, HashSet<T>>(parent).Add(id);
      this._parents[id] = parents;
      if (!this._parents.ContainsKey(parent))
        this._rootNodes.Add(parent);
    }
  }

  public bool Remove(T id, bool force = false)
  {
    if (!force && this._edges.ContainsKey(id))
      throw new InvalidOperationException("Cannot remove node that has remaining children");
    bool flag = this._rootNodes.Remove(id);
    T[] objArray1;
    if (this._parents.TryGetValue(id, out objArray1))
    {
      flag = true;
      foreach (T key in objArray1)
        this._edges[key].Remove(id);
      this._parents.Remove(id);
    }
    HashSet<T> objSet;
    if (force && this._edges.TryGetValue(id, out objSet))
    {
      foreach (T key in objSet)
      {
        T[] parent = this._parents[key];
        T[] objArray2 = new T[parent.Length - 1];
        int num = 0;
        foreach (T objA in parent)
        {
          if (!object.Equals((object) objA, (object) id))
            objArray2[num++] = objA;
        }
        if (objArray2.Length == 0)
        {
          this._rootNodes.Add(key);
          this._parents.Remove(key);
        }
        else
          this._parents[key] = objArray2;
      }
    }
    return flag;
  }
}
