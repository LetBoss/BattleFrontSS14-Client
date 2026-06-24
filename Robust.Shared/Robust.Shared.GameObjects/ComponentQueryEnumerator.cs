using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Robust.Shared.GameObjects;

public struct ComponentQueryEnumerator(Dictionary<EntityUid, IComponent> traitDict) : IDisposable
{
	private Dictionary<EntityUid, IComponent>.Enumerator _traitDict = traitDict.GetEnumerator();

	public bool MoveNext(out EntityUid uid, [NotNullWhen(true)] out IComponent? comp1)
	{
		KeyValuePair<EntityUid, IComponent> current;
		do
		{
			if (!_traitDict.MoveNext())
			{
				uid = default(EntityUid);
				comp1 = null;
				return false;
			}
			current = _traitDict.Current;
		}
		while (current.Value.Deleted);
		uid = current.Key;
		comp1 = current.Value;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext([NotNullWhen(true)] out IComponent? comp1)
	{
		EntityUid uid;
		return MoveNext(out uid, out comp1);
	}

	public void Dispose()
	{
		_traitDict.Dispose();
	}
}
