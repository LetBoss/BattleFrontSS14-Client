using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesPathSelector : ViewVariablesObjectSelector
{
	public string Path { get; }

	public ViewVariablesPathSelector(string path)
	{
		Path = path;
	}
}
