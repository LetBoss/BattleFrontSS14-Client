using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesRequestAllPrototypes : ViewVariablesRequest
{
	public string Variant { get; }

	public ViewVariablesRequestAllPrototypes(string variant)
	{
		Variant = variant;
	}
}
