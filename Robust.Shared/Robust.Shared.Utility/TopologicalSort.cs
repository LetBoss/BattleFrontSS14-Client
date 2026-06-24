using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Robust.Shared.Utility;

public sealed class TopologicalSort
{
	[DebuggerDisplay("GraphNode: {Value}")]
	public sealed class GraphNode<T>
	{
		public readonly T Value;

		public readonly List<GraphNode<T>> Dependant = new List<GraphNode<T>>();

		internal int DependsOnCount;

		public GraphNode(T value)
		{
			Value = value;
		}
	}

	public static IEnumerable<T> Sort<T>(IEnumerable<GraphNode<T>> nodes)
	{
		int totalVerts = 0;
		Queue<GraphNode<T>> empty = new Queue<GraphNode<T>>();
		GraphNode<T>[] array = nodes.ToArray();
		GraphNode<T>[] array2 = array;
		foreach (GraphNode<T> obj in array2)
		{
			totalVerts++;
			foreach (GraphNode<T> item in obj.Dependant)
			{
				item.DependsOnCount++;
			}
		}
		array2 = array;
		foreach (GraphNode<T> graphNode in array2)
		{
			if (graphNode.DependsOnCount == 0)
			{
				empty.Enqueue(graphNode);
			}
		}
		GraphNode<T> node;
		while (empty.TryDequeue(out node))
		{
			yield return node.Value;
			totalVerts--;
			foreach (GraphNode<T> item2 in node.Dependant)
			{
				item2.DependsOnCount--;
				if (item2.DependsOnCount == 0)
				{
					empty.Enqueue(item2);
				}
			}
		}
		if (totalVerts != 0)
		{
			throw new InvalidOperationException("Graph contained cycle(s).");
		}
	}

	public static IEnumerable<GraphNode<TValue>> FromBeforeAfter<TDatum, TValue>(IEnumerable<TDatum> data, Func<TDatum, TValue> keySelector, Func<TDatum, IEnumerable<TValue>> beforeSelector, Func<TDatum, IEnumerable<TValue>> afterSelector, bool allowMissing = false) where TValue : notnull
	{
		return FromBeforeAfter(data, keySelector, keySelector, afterSelector, beforeSelector, allowMissing);
	}

	public static IEnumerable<GraphNode<TValue>> FromBeforeAfter<TDatum, TKey, TValue>(IEnumerable<TDatum> data, Func<TDatum, TKey> keySelector, Func<TDatum, TValue> valueSelector, Func<TDatum, IEnumerable<TKey>> beforeSelector, Func<TDatum, IEnumerable<TKey>> afterSelector, bool allowMissing = false) where TKey : notnull
	{
		Dictionary<TKey, (TDatum, GraphNode<TValue>)> dictionary = new Dictionary<TKey, (TDatum, GraphNode<TValue>)>();
		foreach (TDatum datum in data)
		{
			TKey key = keySelector(datum);
			TValue value = valueSelector(datum);
			dictionary.Add(key, (datum, new GraphNode<TValue>(value)));
		}
		foreach (KeyValuePair<TKey, (TDatum, GraphNode<TValue>)> item in dictionary)
		{
			item.Deconstruct(out var key2, out var value2);
			(TDatum, GraphNode<TValue>) tuple = value2;
			TKey value3 = key2;
			var (arg, graphNode) = tuple;
			foreach (TKey item2 in beforeSelector(arg))
			{
				if (dictionary.TryGetValue(item2, out var value4))
				{
					graphNode.Dependant.Add(value4.Item2);
				}
				else if (!allowMissing)
				{
					throw new InvalidOperationException($"Vertex '{item2}' referenced by '{value3}' was not found in the graph.");
				}
			}
			foreach (TKey item3 in afterSelector(arg))
			{
				if (dictionary.TryGetValue(item3, out var value5))
				{
					value5.Item2.Dependant.Add(graphNode);
				}
				else if (!allowMissing)
				{
					throw new InvalidOperationException($"Vertex '{item3}' referenced by '{value3}' was not found in the graph.");
				}
			}
		}
		return dictionary.Values.Select<(TDatum, GraphNode<TValue>), GraphNode<TValue>>(((TDatum datum, GraphNode<TValue> node) c) => c.node);
	}
}
