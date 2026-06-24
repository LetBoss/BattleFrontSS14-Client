using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesRequestEnumerable : ViewVariablesRequest
{
	public int FromIndex { get; }

	public int ToIndex { get; }

	public bool Refresh { get; }

	public ViewVariablesRequestEnumerable(int fromIndex, int toIndex, bool refresh)
	{
		FromIndex = fromIndex;
		ToIndex = toIndex;
		Refresh = refresh;
	}
}
