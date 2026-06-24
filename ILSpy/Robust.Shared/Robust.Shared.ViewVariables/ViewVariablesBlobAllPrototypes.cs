using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesBlobAllPrototypes : ViewVariablesBlob
{
	public List<string> Prototypes { get; set; } = new List<string>();

	public string Variant { get; set; } = string.Empty;
}
