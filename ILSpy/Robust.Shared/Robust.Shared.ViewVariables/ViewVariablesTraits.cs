using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public enum ViewVariablesTraits : byte
{
	Members,
	Enumerable,
	Entity
}
