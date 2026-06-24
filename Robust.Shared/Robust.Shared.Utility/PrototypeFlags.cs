using System.Collections;
using System.Collections.Generic;
using Robust.Shared.Prototypes;

namespace Robust.Shared.Utility;

public sealed class PrototypeFlags<T> : IReadOnlyPrototypeFlags<T>, IEnumerable<string>, IEnumerable where T : class, IPrototype
{
	private readonly HashSet<string> _flags;

	public int Count => _flags.Count;

	public PrototypeFlags()
	{
		_flags = new HashSet<string>();
	}

	public PrototypeFlags(params string[] flags)
	{
		_flags = new HashSet<string>(flags);
	}

	public PrototypeFlags(IEnumerable<string> flags)
	{
		_flags = new HashSet<string>(flags);
	}

	public bool Add(string flag, IPrototypeManager prototypeManager)
	{
		if (prototypeManager.HasIndex<T>(flag))
		{
			return _flags.Add(flag);
		}
		return false;
	}

	internal void UnionWith(PrototypeFlags<T> flags)
	{
		_flags.UnionWith(flags._flags);
	}

	public bool Contains(string flag)
	{
		return _flags.Contains(flag);
	}

	public bool ContainsAll(IEnumerable<string> flags)
	{
		foreach (string flag in flags)
		{
			if (!Contains(flag))
			{
				return false;
			}
		}
		return true;
	}

	public bool ContainsAll(params string[] flags)
	{
		return ContainsAll((IEnumerable<string>)flags);
	}

	public bool ContainsAny(IEnumerable<string> flags)
	{
		foreach (string flag in flags)
		{
			if (Contains(flag))
			{
				return true;
			}
		}
		return false;
	}

	public bool ContainsAny(params string[] flags)
	{
		return ContainsAny((IEnumerable<string>)flags);
	}

	public bool Remove(string flag)
	{
		return _flags.Remove(flag);
	}

	public void Clear()
	{
		_flags.Clear();
	}

	public IEnumerable<T> GetPrototypes(IPrototypeManager prototypeManager)
	{
		foreach (string flag in _flags)
		{
			yield return prototypeManager.Index<T>(flag);
		}
	}

	public IEnumerator<string> GetEnumerator()
	{
		return _flags.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
