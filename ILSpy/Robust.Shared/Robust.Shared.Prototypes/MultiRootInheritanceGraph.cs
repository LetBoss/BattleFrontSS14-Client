using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Utility;

namespace Robust.Shared.Prototypes;

public sealed class MultiRootInheritanceGraph<T> where T : notnull
{
	private readonly HashSet<T> _rootNodes = new HashSet<T>();

	private readonly Dictionary<T, HashSet<T>> _edges = new Dictionary<T, HashSet<T>>();

	private readonly Dictionary<T, T[]> _parents = new Dictionary<T, T[]>();

	public IReadOnlySet<T> RootNodes => _rootNodes;

	public bool Add(T id)
	{
		return _rootNodes.Add(id);
	}

	public IReadOnlySet<T>? GetChildren(T id)
	{
		return _edges.GetValueOrDefault(id);
	}

	public bool TryGetChildren(T id, [NotNullWhen(true)] out IReadOnlySet<T>? set)
	{
		set = GetChildren(id);
		return set != null;
	}

	public T[]? GetParents(T id)
	{
		return _parents.GetValueOrDefault(id);
	}

	public int GetParentsCount(T id)
	{
		T[]? valueOrDefault = _parents.GetValueOrDefault(id);
		if (valueOrDefault == null)
		{
			return 0;
		}
		return valueOrDefault.Length;
	}

	public bool TryGetParents(T id, [NotNullWhen(true)] out T[]? parents)
	{
		parents = GetParents(id);
		return parents != null;
	}

	public void Add(T id, params T[] parents)
	{
		T[] array = parents;
		foreach (T val in array)
		{
			if (EqualityComparer<T>.Default.Equals(val, id))
			{
				throw new InvalidOperationException($"Self Inheritance detected for id \"{id}\"!");
			}
			T[] parents2 = GetParents(val);
			if (parents2 == null)
			{
				continue;
			}
			Queue<T> queue = new Queue<T>(parents2);
			T result;
			while (queue.TryDequeue(out result))
			{
				if (EqualityComparer<T>.Default.Equals(result, id))
				{
					throw new InvalidOperationException($"Circular Inheritance detected for id \"{id}\" and parent \"{val}\"");
				}
				T[] parents3 = GetParents(result);
				if (parents3 != null)
				{
					T[] array2 = parents3;
					foreach (T item in array2)
					{
						queue.Enqueue(item);
					}
				}
			}
		}
		_rootNodes.Remove(id);
		array = parents;
		foreach (T val2 in array)
		{
			_edges.GetOrNew(val2).Add(id);
			_parents[id] = parents;
			if (!_parents.ContainsKey(val2))
			{
				_rootNodes.Add(val2);
			}
		}
	}

	public bool Remove(T id, bool force = false)
	{
		if (!force && _edges.ContainsKey(id))
		{
			throw new InvalidOperationException("Cannot remove node that has remaining children");
		}
		bool result = _rootNodes.Remove(id);
		if (_parents.TryGetValue(id, out T[] value))
		{
			result = true;
			T[] array = value;
			foreach (T key in array)
			{
				_edges[key].Remove(id);
			}
			_parents.Remove(id);
		}
		if (force && _edges.TryGetValue(id, out HashSet<T> value2))
		{
			foreach (T item in value2)
			{
				T[] array2 = _parents[item];
				T[] array3 = new T[array2.Length - 1];
				int num = 0;
				T[] array = array2;
				foreach (T val in array)
				{
					if (!object.Equals(val, id))
					{
						array3[num++] = val;
					}
				}
				if (array3.Length == 0)
				{
					_rootNodes.Add(item);
					_parents.Remove(item);
				}
				else
				{
					_parents[item] = array3;
				}
			}
		}
		return result;
	}
}
