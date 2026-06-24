// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.TopologicalSort
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class TopologicalSort
{
  public static IEnumerable<T> Sort<T>(IEnumerable<TopologicalSort.GraphNode<T>> nodes)
  {
    int totalVerts = 0;
    Queue<TopologicalSort.GraphNode<T>> empty = new Queue<TopologicalSort.GraphNode<T>>();
    TopologicalSort.GraphNode<T>[] array = nodes.ToArray<TopologicalSort.GraphNode<T>>();
    foreach (TopologicalSort.GraphNode<T> graphNode1 in array)
    {
      ++totalVerts;
      foreach (TopologicalSort.GraphNode<T> graphNode2 in graphNode1.Dependant)
        ++graphNode2.DependsOnCount;
    }
    foreach (TopologicalSort.GraphNode<T> graphNode in array)
    {
      if (graphNode.DependsOnCount == 0)
        empty.Enqueue(graphNode);
    }
    TopologicalSort.GraphNode<T> node;
    while (empty.TryDequeue(out node))
    {
      yield return node.Value;
      --totalVerts;
      foreach (TopologicalSort.GraphNode<T> graphNode in node.Dependant)
      {
        --graphNode.DependsOnCount;
        if (graphNode.DependsOnCount == 0)
          empty.Enqueue(graphNode);
      }
    }
    if (totalVerts != 0)
      throw new InvalidOperationException("Graph contained cycle(s).");
  }

  public static IEnumerable<TopologicalSort.GraphNode<TValue>> FromBeforeAfter<TDatum, TValue>(
    IEnumerable<TDatum> data,
    Func<TDatum, TValue> keySelector,
    Func<TDatum, IEnumerable<TValue>> beforeSelector,
    Func<TDatum, IEnumerable<TValue>> afterSelector,
    bool allowMissing = false)
    where TValue : notnull
  {
    return TopologicalSort.FromBeforeAfter<TDatum, TValue, TValue>(data, keySelector, keySelector, afterSelector, beforeSelector, allowMissing);
  }

  public static IEnumerable<TopologicalSort.GraphNode<TValue>> FromBeforeAfter<TDatum, TKey, TValue>(
    IEnumerable<TDatum> data,
    Func<TDatum, TKey> keySelector,
    Func<TDatum, TValue> valueSelector,
    Func<TDatum, IEnumerable<TKey>> beforeSelector,
    Func<TDatum, IEnumerable<TKey>> afterSelector,
    bool allowMissing = false)
    where TKey : notnull
  {
    Dictionary<TKey, (TDatum, TopologicalSort.GraphNode<TValue>)> dictionary = new Dictionary<TKey, (TDatum, TopologicalSort.GraphNode<TValue>)>();
    foreach (TDatum datum in data)
    {
      TKey key = keySelector(datum);
      TValue obj = valueSelector(datum);
      dictionary.Add(key, (datum, new TopologicalSort.GraphNode<TValue>(obj)));
    }
    foreach ((TKey key1, (TDatum datum1, TopologicalSort.GraphNode<TValue> graphNode)) in dictionary)
    {
      foreach (TKey key2 in beforeSelector(datum1))
      {
        (TDatum, TopologicalSort.GraphNode<TValue>) valueTuple;
        if (dictionary.TryGetValue(key2, out valueTuple))
          graphNode.Dependant.Add(valueTuple.Item2);
        else if (!allowMissing)
          throw new InvalidOperationException($"Vertex '{key2}' referenced by '{key1}' was not found in the graph.");
      }
      foreach (TKey key3 in afterSelector(datum1))
      {
        (TDatum, TopologicalSort.GraphNode<TValue>) valueTuple;
        if (dictionary.TryGetValue(key3, out valueTuple))
          valueTuple.Item2.Dependant.Add(graphNode);
        else if (!allowMissing)
          throw new InvalidOperationException($"Vertex '{key3}' referenced by '{key1}' was not found in the graph.");
      }
    }
    return dictionary.Values.Select<(TDatum, TopologicalSort.GraphNode<TValue>), TopologicalSort.GraphNode<TValue>>((Func<(TDatum, TopologicalSort.GraphNode<TValue>), TopologicalSort.GraphNode<TValue>>) (c => c.node));
  }

  [DebuggerDisplay("GraphNode: {Value}")]
  public sealed class GraphNode<T>
  {
    public readonly T Value;
    public readonly List<TopologicalSort.GraphNode<T>> Dependant = new List<TopologicalSort.GraphNode<T>>();
    internal int DependsOnCount;

    public GraphNode(T value) => this.Value = value;
  }
}
