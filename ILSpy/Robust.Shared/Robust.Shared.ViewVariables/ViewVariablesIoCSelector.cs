using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesIoCSelector : ViewVariablesObjectSelector
{
	public string TypeName { get; }

	public ViewVariablesIoCSelector(string typeName)
	{
		TypeName = typeName;
	}
}
