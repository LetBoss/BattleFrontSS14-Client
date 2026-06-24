using System.Collections.Generic;

namespace Robust.Shared.GameObjects;

public readonly struct NetComponentEnumerable(Dictionary<ushort, IComponent> dictionary)
{
	private readonly Dictionary<ushort, IComponent> _dictionary = dictionary;

	public NetComponentEnumerator GetEnumerator()
	{
		return new NetComponentEnumerator(_dictionary);
	}
}
