using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesEntitySystemSelector : ViewVariablesObjectSelector
{
	public string TypeName { get; }

	public ViewVariablesEntitySystemSelector(string typeName)
	{
		TypeName = typeName;
	}
}
