using System.Collections.Generic;

namespace Robust.Shared.GameObjects;

public struct NetComponentEnumerator(Dictionary<ushort, IComponent> dictionary)
{
	private Dictionary<ushort, IComponent>.Enumerator _dictEnum = dictionary.GetEnumerator();

	public (ushort netId, IComponent component) Current
	{
		get
		{
			KeyValuePair<ushort, IComponent> current = _dictEnum.Current;
			return (netId: current.Key, component: current.Value);
		}
	}

	public bool MoveNext()
	{
		return _dictEnum.MoveNext();
	}
}
