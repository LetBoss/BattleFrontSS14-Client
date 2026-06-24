using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires;

[Serializable]
[NetSerializable]
public struct StatusEntry(object key, object value)
{
	public readonly object Key = key;

	public readonly object Value = value;

	public override string ToString()
	{
		return $"{Key}, {Value}";
	}
}
