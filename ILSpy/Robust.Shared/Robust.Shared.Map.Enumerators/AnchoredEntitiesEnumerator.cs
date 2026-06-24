using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Map.Enumerators;

public struct AnchoredEntitiesEnumerator : IDisposable
{
	private static readonly List<EntityUid> Dummy = new List<EntityUid>();

	public static readonly AnchoredEntitiesEnumerator Empty = new AnchoredEntitiesEnumerator(Dummy.GetEnumerator());

	private List<EntityUid>.Enumerator _enumerator;

	internal AnchoredEntitiesEnumerator(List<EntityUid>.Enumerator enumerator)
	{
		_enumerator = enumerator;
	}

	public bool MoveNext([NotNullWhen(true)] out EntityUid? uid)
	{
		if (!_enumerator.MoveNext())
		{
			uid = null;
			return false;
		}
		uid = _enumerator.Current;
		return true;
	}

	public void Dispose()
	{
		_enumerator.Dispose();
	}
}
