using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesMemberSelector
{
	public int Index { get; set; }

	public ViewVariablesMemberSelector(int index)
	{
		Index = index;
	}
}
