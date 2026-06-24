using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Robust.Shared.GameObjects;

public struct AllEntityQueryEnumerator<TComp1>(Dictionary<EntityUid, IComponent> traitDict) : IDisposable where TComp1 : IComponent
{
	private Dictionary<EntityUid, IComponent>.Enumerator _traitDict = traitDict.GetEnumerator();

	public bool MoveNext(out EntityUid uid, [NotNullWhen(true)] out TComp1? comp1)
	{
		KeyValuePair<EntityUid, IComponent> current;
		do
		{
			if (!_traitDict.MoveNext())
			{
				uid = default(EntityUid);
				comp1 = default(TComp1);
				return false;
			}
			current = _traitDict.Current;
		}
		while (current.Value.Deleted);
		uid = current.Key;
		comp1 = (TComp1)current.Value;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext([NotNullWhen(true)] out TComp1? comp1)
	{
		EntityUid uid;
		return MoveNext(out uid, out comp1);
	}

	public void Dispose()
	{
		_traitDict.Dispose();
	}
}
public struct AllEntityQueryEnumerator<TComp1, TComp2>(Dictionary<EntityUid, IComponent> traitDict, Dictionary<EntityUid, IComponent> traitDict2) : IDisposable where TComp1 : IComponent where TComp2 : IComponent
{
	private Dictionary<EntityUid, IComponent>.Enumerator _traitDict = traitDict.GetEnumerator();

	private readonly Dictionary<EntityUid, IComponent> _traitDict2 = traitDict2;

	public bool MoveNext(out EntityUid uid, [NotNullWhen(true)] out TComp1? comp1, [NotNullWhen(true)] out TComp2? comp2)
	{
		KeyValuePair<EntityUid, IComponent> current;
		IComponent value;
		do
		{
			if (!_traitDict.MoveNext())
			{
				uid = default(EntityUid);
				comp1 = default(TComp1);
				comp2 = default(TComp2);
				return false;
			}
			current = _traitDict.Current;
		}
		while (current.Value.Deleted || !_traitDict2.TryGetValue(current.Key, out value) || value.Deleted);
		uid = current.Key;
		comp1 = (TComp1)current.Value;
		comp2 = (TComp2)value;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext([NotNullWhen(true)] out TComp1? comp1, [NotNullWhen(true)] out TComp2? comp2)
	{
		EntityUid uid;
		return MoveNext(out uid, out comp1, out comp2);
	}

	public void Dispose()
	{
		_traitDict.Dispose();
	}
}
public struct AllEntityQueryEnumerator<TComp1, TComp2, TComp3>(Dictionary<EntityUid, IComponent> traitDict, Dictionary<EntityUid, IComponent> traitDict2, Dictionary<EntityUid, IComponent> traitDict3) : IDisposable where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent
{
	private Dictionary<EntityUid, IComponent>.Enumerator _traitDict = traitDict.GetEnumerator();

	private readonly Dictionary<EntityUid, IComponent> _traitDict2 = traitDict2;

	private readonly Dictionary<EntityUid, IComponent> _traitDict3 = traitDict3;

	public bool MoveNext(out EntityUid uid, [NotNullWhen(true)] out TComp1? comp1, [NotNullWhen(true)] out TComp2? comp2, [NotNullWhen(true)] out TComp3? comp3)
	{
		KeyValuePair<EntityUid, IComponent> current;
		IComponent value;
		IComponent value2;
		do
		{
			if (!_traitDict.MoveNext())
			{
				uid = default(EntityUid);
				comp1 = default(TComp1);
				comp2 = default(TComp2);
				comp3 = default(TComp3);
				return false;
			}
			current = _traitDict.Current;
		}
		while (current.Value.Deleted || !_traitDict2.TryGetValue(current.Key, out value) || value.Deleted || !_traitDict3.TryGetValue(current.Key, out value2) || value2.Deleted);
		uid = current.Key;
		comp1 = (TComp1)current.Value;
		comp2 = (TComp2)value;
		comp3 = (TComp3)value2;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext([NotNullWhen(true)] out TComp1? comp1, [NotNullWhen(true)] out TComp2? comp2, [NotNullWhen(true)] out TComp3? comp3)
	{
		EntityUid uid;
		return MoveNext(out uid, out comp1, out comp2, out comp3);
	}

	public void Dispose()
	{
		_traitDict.Dispose();
	}
}
public struct AllEntityQueryEnumerator<TComp1, TComp2, TComp3, TComp4>(Dictionary<EntityUid, IComponent> traitDict, Dictionary<EntityUid, IComponent> traitDict2, Dictionary<EntityUid, IComponent> traitDict3, Dictionary<EntityUid, IComponent> traitDict4) : IDisposable where TComp1 : IComponent where TComp2 : IComponent where TComp3 : IComponent where TComp4 : IComponent
{
	private Dictionary<EntityUid, IComponent>.Enumerator _traitDict = traitDict.GetEnumerator();

	private readonly Dictionary<EntityUid, IComponent> _traitDict2 = traitDict2;

	private readonly Dictionary<EntityUid, IComponent> _traitDict3 = traitDict3;

	private readonly Dictionary<EntityUid, IComponent> _traitDict4 = traitDict4;

	public bool MoveNext(out EntityUid uid, [NotNullWhen(true)] out TComp1? comp1, [NotNullWhen(true)] out TComp2? comp2, [NotNullWhen(true)] out TComp3? comp3, [NotNullWhen(true)] out TComp4? comp4)
	{
		KeyValuePair<EntityUid, IComponent> current;
		IComponent value;
		IComponent value2;
		IComponent value3;
		do
		{
			if (!_traitDict.MoveNext())
			{
				uid = default(EntityUid);
				comp1 = default(TComp1);
				comp2 = default(TComp2);
				comp3 = default(TComp3);
				comp4 = default(TComp4);
				return false;
			}
			current = _traitDict.Current;
		}
		while (current.Value.Deleted || !_traitDict2.TryGetValue(current.Key, out value) || value.Deleted || !_traitDict3.TryGetValue(current.Key, out value2) || value2.Deleted || !_traitDict4.TryGetValue(current.Key, out value3) || value3.Deleted);
		uid = current.Key;
		comp1 = (TComp1)current.Value;
		comp2 = (TComp2)value;
		comp3 = (TComp3)value2;
		comp4 = (TComp4)value3;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext([NotNullWhen(true)] out TComp1? comp1, [NotNullWhen(true)] out TComp2? comp2, [NotNullWhen(true)] out TComp3? comp3, [NotNullWhen(true)] out TComp4? comp4)
	{
		EntityUid uid;
		return MoveNext(out uid, out comp1, out comp2, out comp3, out comp4);
	}

	public void Dispose()
	{
		_traitDict.Dispose();
	}
}
