using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesTupleIndexSelector(int index)
{
	public int Index { get; set; } = index;
}
