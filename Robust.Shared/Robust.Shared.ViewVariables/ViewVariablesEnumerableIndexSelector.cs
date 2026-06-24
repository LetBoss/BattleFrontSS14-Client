using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesEnumerableIndexSelector
{
	public int Index { get; set; }

	public ViewVariablesEnumerableIndexSelector(int index)
	{
		Index = index;
	}
}
